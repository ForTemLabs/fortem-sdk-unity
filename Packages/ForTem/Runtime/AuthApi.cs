using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    /// <summary>
    /// Authentication API operations.
    /// </summary>
    internal sealed class AuthApi : ForTemApiBase
    {
        public AuthApi(ForTemClient client) : base(client)
        {
        }

        /// <summary>
        /// Request a nonce for authentication.
        /// </summary>
        public async Task<string> GetNonce()
        {
            var endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/auth/nonce";
            var request = UnityWebRequest.PostWwwForm(endpoint, "");
            request.SetRequestHeader("x-api-key", _client.Config.ApiKey);
            var response = await SendWebRequest<NonceResponseData>(request);

            return response.Nonce;
        }

        /// <summary>
        /// Exchange nonce for an access token.
        /// </summary>
        public async Task<string> GetAccessToken(string nonce)
        {
            var body = new AccessTokenRequest { Nonce = nonce };
            string bodyJson = JsonUtility.ToJson(body);
            var endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/auth/access-token";
            var request = UnityWebRequest.Post(endpoint, bodyJson, "application/json");
            request.SetRequestHeader("x-api-key", _client.Config.ApiKey);
            var response = await SendWebRequest<AccessTokenResponseData>(request);

            return response.AccessToken;
        }
    }
}
