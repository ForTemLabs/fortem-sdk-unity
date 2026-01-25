using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    /// <summary>
    /// Create or retrieve collections that serve as game item groups.
    /// A game collection must be created before any items can be registered.
    /// Each developer account can have up to five collections.
    /// </summary>
    public sealed class CollectionApi : ForTemApiBase
    {
        /// <summary>
        /// This regex matches any JSON key with an empty string value, e.g. "key":""<br/>
        /// This is a workaround for Unity's JsonUtility which serializes null strings as empty strings.
        /// </summary>
        private static readonly Regex JsonRequestRegex = new(
            "\"[^\"]+\":\"\"[,]?",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public CollectionApi(ForTemClient client) : base(client)
        {
        }

        /// <summary>
        /// Retrieves the list of collections associated with the developer account.
        /// </summary>
        public async Task<List<CollectionResponse>> GetCollections()
        {
            var accessToken = await _client.Authenticate(forMinting: false);

            var endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/collections";
            using var request = UnityWebRequest.Get(endpoint);
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<List<CollectionResponse>>(request);

            return response;
        }

        /// <summary>
        /// Creates a new collection for grouping game items.
        /// </summary>
        /// <remarks>
        /// Maximum 5 collections allowed per developer account.
        /// </remarks>
        public async Task<CollectionResponse> CreateCollection(CreateCollectionRequest requestBody)
        {
            var accessToken = await _client.Authenticate(forMinting: true);

            string bodyJson = JsonUtility.ToJson(requestBody);
            bodyJson = JsonRequestRegex.Replace(bodyJson, string.Empty).Replace(",}", "}");
            var endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/collections";
            using var request = UnityWebRequestEx.Post(endpoint, bodyJson, "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<CollectionResponse>(request);

            return response;
        }
    }

    public static class UnityWebRequestEx
    {
        //
        // Summary:
        //     Creates a UnityWebRequest configured to send form data to a server via HTTP POST.
        //
        //
        // Parameters:
        //   uri:
        //     The target URI to which the string will be transmitted.
        //
        //   postData:
        //     Form body data. Will be converted to UTF-8 string.
        //
        //   contentType:
        //     Value for the Content-Type header, for example application/json.
        //
        // Returns:
        //     A UnityWebRequest configured to send string to uri via POST.
        public static UnityWebRequest Post(string uri, string postData, string contentType)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            SetupPost(request, postData, contentType);
            return request;
        }

        private static void SetupPost(UnityWebRequest request, string postData, string contentType)
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            if (string.IsNullOrEmpty(postData))
            {
                request.SetRequestHeader("Content-Type", contentType);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.uploadHandler.contentType = contentType;
        }
    }
}
