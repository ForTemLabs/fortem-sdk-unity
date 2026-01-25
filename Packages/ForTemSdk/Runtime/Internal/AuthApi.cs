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
        /// Requests a nonce for authentication.
        /// </summary>
        public async Task<string> GetNonce()
        {
            var endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/auth/nonce";
            using var request = new UnityWebRequest(endpoint, "POST");
            request.SetRequestHeader("x-api-key", _client.Config.ApiKey);
            request.downloadHandler = new DownloadHandlerBuffer();
            var response = await SendWebRequest<NonceResponse>(request);

            return response.Nonce;
        }

        /// <summary>
        /// Exchanges a nonce for an access token.
        /// </summary>
        public async Task<string> GetAccessToken(string nonce)
        {
            var body = new AccessTokenRequest { Nonce = nonce };
            string bodyJson = JsonUtility.ToJson(body);
            var endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/auth/access-token";
            using var request = UnityWebRequestEx.Post(endpoint, bodyJson, "application/json");
            request.SetRequestHeader("x-api-key", _client.Config.ApiKey);
            var response = await SendWebRequest<AccessTokenResponse>(request);

            return response.AccessToken;
        }
    }
}
