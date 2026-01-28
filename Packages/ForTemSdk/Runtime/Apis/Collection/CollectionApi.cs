using System.Collections.Generic;
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
    internal sealed class CollectionApi : /*ForTemApiBase,*/ ICollectionApi
    {
        /// <summary>
        /// This regex matches any JSON key with an empty string value, e.g. "key":""<br/>
        /// This is a workaround for Unity's JsonUtility which serializes null strings as empty strings.
        /// </summary>
        private static readonly Regex JsonRequestRegex = new(
            "\"[^\"]+\":\"\"[,]?",
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private readonly ForTemClientHelper _helper;
        private readonly AuthApi _authApi;

        public CollectionApi(ForTemClientHelper helper, AuthApi authApi)
            //: base(webRequestSender, authApi)
        {
            _helper = helper;
            _authApi = authApi;
        }

        /// <summary>
        /// Retrieves the list of collections associated with the developer account.
        /// </summary>
        public async Task<List<CollectionResponse>> GetCollections()
        {
            var accessToken = await _authApi.Authenticate(forMinting: false);

            var endpoint = $"{_helper.Config.GetApiBaseUrl()}/api/v1/developers/collections";
            using var request = UnityWebRequest.Get(endpoint);
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await _helper.SendWebRequest<List<CollectionResponse>>(request);

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
            var accessToken = await _authApi.Authenticate(forMinting: true);

            string bodyJson = JsonUtility.ToJson(requestBody);
            bodyJson = JsonRequestRegex.Replace(bodyJson, string.Empty).Replace(",}", "}");
            var endpoint = $"{_helper.Config.GetApiBaseUrl()}/api/v1/developers/collections";
            using var request = UnityWebRequestEx.Post(endpoint, bodyJson, "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await _helper.SendWebRequest<CollectionResponse>(request);

            return response;
        }
    }
}
