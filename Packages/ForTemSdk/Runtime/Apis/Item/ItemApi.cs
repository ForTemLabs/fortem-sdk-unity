using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    /// <summary>
    /// Create or retrieve items within a specific collection.
    /// </summary>
    public sealed class ItemApi : ForTemApiBase
    {
        /// <summary>
        /// This regex matches any JSON key with an empty string value, e.g. "key":""<br/>
        /// This is a workaround for Unity's JsonUtility which serializes null strings as empty strings.
        /// </summary>
        private static readonly Regex JsonRequestRegex = new Regex(
            "\"[^\"]+\":\"\"[,]?",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        internal ItemApi(ForTemClient client) : base(client)
        {
        }

        /// <summary>
        /// Retrieves item information based on its redeem code within a specific collection.
        /// </summary>
        public async Task<GetItemResponse> GetItem(int collectionId, string redeemCode)
        {
            var accessToken = await _client.Authenticate(forMinting: false);

            string endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/collections/{collectionId}/items/{redeemCode}";
            using var request = UnityWebRequest.Get(endpoint);
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<GetItemResponse>(request);

            return response;
        }

        /// <summary>
        /// Creates a new item in a collection, with an image upload.
        /// The <see cref="CreateItemRequest.RecipientAddress"/>  must belong to a ForTem user.
        /// </summary>
        public async Task<CreateItemResponse> CreateItemWithImage(
            int collectionId,
            CreateItemRequest item,
            byte[] imageData,
            string fileName)
        {
            if (imageData != null && imageData.Length > 0)
            {
                var imageCid = await UploadImage(collectionId, imageData, fileName);
                item.ItemImage = imageCid;
            }

            return await CreateItem(collectionId, item);
        }

        /// <summary>
        /// Creates a new item in a collection.
        /// The <see cref="CreateItemRequest.RecipientAddress"/>  must belong to a ForTem user.
        /// </summary>
        /// <remarks>
        /// Custom images can be uploaded separately through the image-upload endpoint.
        /// If you do not upload a custom image, the item will automatically display ForTem�s default item image.
        /// </remarks>
        public async Task<CreateItemResponse> CreateItem(int collectionId, CreateItemRequest requestBody)
        {
            var accessToken = await _client.Authenticate(forMinting: true);

            string bodyJson = JsonUtility.ToJson(requestBody);
            bodyJson = JsonRequestRegex.Replace(bodyJson, string.Empty).Replace(",}", "}");
            string endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/collections/{collectionId}/items";
            using var request = UnityWebRequestEx.Post(endpoint, bodyJson, "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<CreateItemResponse>(request);

            return response;
        }

        /// <summary>
        /// Upload an image for an item.
        /// Returns an IPFS CID (Content Identifier) that can be referenced when creating the item.
        /// All uploaded images are securely stored and managed on the IPFS server.
        /// </summary>
        /// <remarks>
        /// Allowed types: image/jpeg, image/png, image/webp
        /// </remarks>
        private async Task<string> UploadImage(int collectionId, byte[] imageData, string fileName)
        {
            var accessToken = await _client.Authenticate(forMinting: false);

            var extension = System.IO.Path.GetExtension(fileName).ToLower();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => throw new ArgumentException("Unsupported image file type. Allowed types: .png, .jpg, .jpeg, .webp")
            };

            var formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormFileSection("file", imageData, fileName, contentType));
            string endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/collections/{collectionId}/items/image-upload";
            using var request = UnityWebRequest.Post(endpoint, formData);
            request.method = "PUT";
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<ImageUploadResponse>(request);

            return response.ItemImage;
        }
    }
}
