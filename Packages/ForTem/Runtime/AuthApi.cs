using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ForTemSdk
{
    /// <summary>
    /// Async authentication API operations.
    /// </summary>
    internal sealed class AuthApi : ForTemApiBase
    {
        public AuthApi(ForTemClient client) : base(client)
        {
        }

        /// <summary>
        /// Asynchronously request a nonce for authentication.
        /// </summary>
        public async Task<string> GetNonce()
        {
            var request = UnityWebRequest.PostWwwForm("/api/v1/developers/auth/nonce", "");
            request.SetRequestHeader("x-api-key", _client.Config.ApiKey);
            var response = await SendWebRequest<NonceResponseData>(request);

            return response.Nonce;
        }

        /// <summary>
        /// Asynchronously exchange nonce for access token.
        /// </summary>
        public async Task<string> GetAccessToken(string nonce)
        {
            var body = new AccessTokenRequest { Nonce = nonce };
            string bodyJson = JsonUtility.ToJson(body);
            var request = UnityWebRequest.Post("/api/v1/developers/auth/access-token", bodyJson, "application/json");
            request.SetRequestHeader("x-api-key", _client.Config.ApiKey);
            var response = await SendWebRequest<AccessTokenResponseData>(request);

            return response.AccessToken;
        }
    }
}
