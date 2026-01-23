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
    /// Collection and item API operations.
    /// </summary>
    public sealed class CollectionsApi : ForTemApiBase
    {
        /// <summary>
        /// This regex matches any JSON key with an empty string value, e.g. "key":""<br/>
        /// This is a workaround for Unity's JsonUtility which serializes null strings as empty strings.
        /// </summary>
        private static readonly Regex JsonRequestRegex = new Regex(
            "\"[^\"]+\":\"\"[,]?",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public CollectionsApi(ForTemClient client) : base(client)
        {
        }

        /// <summary>
        /// Get all game collections.
        /// </summary>
        public async Task<List<CollectionResponseData>> GetCollections()
        {
            var accessToken = await _client.Authenticate(forMinting: false);

            var endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/collections";
            var request = UnityWebRequest.Get(endpoint);
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<List<CollectionResponseData>>(request);

            return response;
        }

        /// <summary>
        /// Create a new collection.
        /// </summary>
        /// <remarks>
        /// Maximum 5 collections allowed per developer account.
        /// </remarks>
        public async Task<CollectionResponseData> CreateCollection(CreateCollectionRequest requestBody) // string name, string description, string website
        {
            var accessToken = await _client.Authenticate(forMinting: true);

            string bodyJson = JsonUtility.ToJson(requestBody);
            bodyJson = JsonRequestRegex.Replace(bodyJson, string.Empty).Replace(",}", "}");
            var endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/collections";
            var request = UnityWebRequest.Post(endpoint, bodyJson, "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<CollectionResponseData>(request);

            return response;
        }

        /// <summary>
        /// Get a specific item from a collection.
        /// </summary>
        public async Task<ItemResponseData> GetItem(int collectionId, string itemCode)
        {
            var accessToken = await _client.Authenticate(forMinting: false);

            string endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/collections/{collectionId}/items/{itemCode}";
            var request = UnityWebRequest.Get(endpoint);
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<ItemResponseData>(request);

            return response;
        }

        /// <summary>
        /// Create a new item in a collection, with an image upload.
        /// </summary>
        public async Task<ItemCreationResponse> CreateItemWithImage(
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
        /// Create a new item in a collection.
        /// </summary>
        public async Task<ItemCreationResponse> CreateItem(int collectionId, CreateItemRequest requestBody)
        {
            var accessToken = await _client.Authenticate(forMinting: true);

            string bodyJson = JsonUtility.ToJson(requestBody);
            bodyJson = JsonRequestRegex.Replace(bodyJson, string.Empty).Replace(",}", "}");
            string endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/collections/{collectionId}/items";
            var request = UnityWebRequest.Post(endpoint, bodyJson, "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<ItemCreationResponse>(request);

            return response;
        }

        /// <summary>
        /// Asynchronously upload an image for collection items.
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
            var request = UnityWebRequest.Post(endpoint, formData);
            request.method = "PUT";
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<ImageUploadResponse>(request);

            return response.ItemImage;
        }
    }
}
