using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    /// <summary>
    /// Authentication API operations.
    /// </summary>
    internal sealed class AuthApi
    {
        // Token management
        private string? _accessToken;
        private long _expiresAt;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private readonly WebRequestHelper _webRequestHelper;

        public AuthApi(WebRequestHelper helper)
        {
            _webRequestHelper = helper;
        }

        internal async Task<string> Authenticate(bool isSingleUse)
        {
            if (isSingleUse)
            {
                string nonce = await GetNonce();
                string accessToken = await GetAccessToken(nonce);
                return accessToken;
            }

            await _semaphore.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(_accessToken) || DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= _expiresAt - 30000)
                {
                    var nonce = await GetNonce();
                    var accessToken = await GetAccessToken(nonce);
                    _accessToken = accessToken;
                    _expiresAt = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + 5 * 60 * 1000; // 5 minutes
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return _accessToken;
        }

        /// <summary>
        /// Requests a nonce for authentication.
        /// </summary>
        public async Task<string> GetNonce()
        {
            var endpoint = $"{_webRequestHelper.Config.GetApiBaseUrl()}/api/v1/developers/auth/nonce";
            using var request = new UnityWebRequest(endpoint, "POST");
            request.SetRequestHeader("x-api-key", _webRequestHelper.Config.ApiKey);
            request.downloadHandler = new DownloadHandlerBuffer();
            var response = await _webRequestHelper.SendWebRequest<NonceResponse>(request);

            return response.Nonce;
        }

        /// <summary>
        /// Exchanges a nonce for an access token.
        /// </summary>
        public async Task<string> GetAccessToken(string nonce)
        {
            var body = new AccessTokenRequest { Nonce = nonce };
            string bodyJson = JsonUtility.ToJson(body);
            var endpoint = $"{_webRequestHelper.Config.GetApiBaseUrl()}/api/v1/developers/auth/access-token";
            using var request = UnityWebRequestEx.Post(endpoint, bodyJson, "application/json");
            request.SetRequestHeader("x-api-key", _webRequestHelper.Config.ApiKey);
            var response = await _webRequestHelper.SendWebRequest<AccessTokenResponse>(request);

            return response.AccessToken;
        }
    }
}
