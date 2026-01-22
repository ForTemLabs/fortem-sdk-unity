using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Async/await version of ForTem SDK client using HttpClient.
    /// Provides modern async/await API as an alternative to coroutines.
    /// </summary>
    public sealed class ForTemClientAsync : IDisposable
    {
        private ForTemConfig _config;
        private ForTemHttpClientAsyncUnityWebRequest _httpClientUnityWebRequest;
        private string _accessToken;
        private long _expiresAt;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        // API endpoints
        private readonly AuthApiAsync _authApi;
        private readonly UserApiAsync _userApi;
        private readonly CollectionsApiAsync _collectionsApi;

        public ForTemClientAsync(ForTemConfig config)
        {
            SetConfig(config);
            _authApi = new AuthApiAsync(this);
            _userApi = new UserApiAsync(this);
            _collectionsApi = new CollectionsApiAsync(this);
        }

        private void SetConfig(ForTemConfig newConfig)
        {
            _config = newConfig;
            _httpClientUnityWebRequest = new ForTemHttpClientAsyncUnityWebRequest(_config, this);

            if (_config.DebugLogging)
                Debug.Log($"[ForTem] Async SDK initialized - Environment: {_config.Environment}");
        }

        /// <summary>
        /// Gets the Auth API for authentication operations.
        /// </summary>
        public AuthApiAsync Auth => _authApi;

        /// <summary>
        /// Gets the User API for user management operations.
        /// </summary>
        public UserApiAsync User => _userApi;

        /// <summary>
        /// Gets the Collections API for game collections and items.
        /// </summary>
        public CollectionsApiAsync Collections => _collectionsApi;

        /// <summary>
        /// Internal: Gets the HTTP client for making API requests.
        /// </summary>
        internal ForTemHttpClientAsyncUnityWebRequest HttpClient => _httpClientUnityWebRequest;

        /// <summary>
        /// Internal: Gets the current configuration.
        /// </summary>
        internal ForTemConfig Config => _config;

        /// <summary>
        /// Internal: Sets the access token after successful authentication.
        /// </summary>
        internal void SetAccessToken(string token)
        {
            _accessToken = token;
        }

        /// <summary>
        /// Internal: Gets the current access token.
        /// </summary>
        internal string GetAccessToken()
        {
            return _accessToken;
        }

        internal async Task<string> GetOrRefreshAccessToken(bool forMinting)
        {
            if (forMinting)
            {
                var nonceResponse = await Auth.GetNonceAsync();
                var accessTokenResponse = await Auth.GetAccessTokenAsync(nonceResponse.nonce);
                return accessTokenResponse.AccessToken;
            }

            //var dummyStart = System.Diagnostics.Stopwatch.GetTimestamp();
            //var dummyEnd = System.Diagnostics.Stopwatch.GetTimestamp();
            //var elapsedMs = (dummyEnd - dummyStart) * 1000 / System.Diagnostics.Stopwatch.Frequency;
            await _semaphore.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(_accessToken) || DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= _expiresAt - 30000)
                {
                    var nonceResponse = await Auth.GetNonceAsync();
                    var accessTokenResponse = await Auth.GetAccessTokenAsync(nonceResponse.nonce);
                    _accessToken = accessTokenResponse.AccessToken;
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
        /// Disposes the HTTP client and resources.
        /// </summary>
        public void Dispose()
        {
            //_httpClient?.Dispose();
            _httpClientUnityWebRequest?.Dispose();
        }
    }

    /// <summary>
    /// Async authentication API operations.
    /// </summary>
    public sealed class AuthApiAsync
    {
        private readonly ForTemClientAsync _client;

        public AuthApiAsync(ForTemClientAsync client)
        {
            _client = client;
        }

        /// <summary>
        /// Asynchronously request a nonce for authentication.
        /// </summary>
        public async Task<NonceResponse> GetNonceAsync()
        {
            var customHeaders = new Dictionary<string, string>
            {
                { "x-api-key", _client.Config.ApiKey }
            };

            return await _client.HttpClient.SendRequestAsync<NonceResponse>(
                "/api/v1/developers/auth/nonce",
                System.Net.Http.HttpMethod.Post,
                null,
                null,
                customHeaders
            );
        }

        /// <summary>
        /// Asynchronously exchange nonce for access token.
        /// </summary>
        public async Task<AccessTokenResponse> GetAccessTokenAsync(string nonce)
        {
            var request = new TokenExchangeRequest(nonce);
            string body = JsonUtility.ToJson(request);

            var customHeaders = new Dictionary<string, string>
            {
                { "x-api-key", _client.Config.ApiKey }
            };

            var result = await _client.HttpClient.SendRequestAsync<AccessTokenResponse>(
                "/api/v1/developers/auth/access-token",
                System.Net.Http.HttpMethod.Post,
                body,
                null,
                customHeaders
            );
            
            // Store token if successful
            if (result != null)
            {
                _client.SetAccessToken(result.AccessToken);
            }
            
            return result;
        }
    }

    /// <summary>
    /// Async user management API operations.
    /// </summary>
    public sealed class UserApiAsync
    {
        private readonly ForTemClientAsync _client;

        public UserApiAsync(ForTemClientAsync client)
        {
            _client = client;
        }

        /// <summary>
        /// Asynchronously get user information by wallet address.
        /// </summary>
        public async Task<User> GetUserAsync(string walletAddress)
        {
            string endpoint = $"/api/v1/developers/users/{walletAddress}";
            return await _client.HttpClient.SendRequestAsync<User>(
                endpoint,
                System.Net.Http.HttpMethod.Get
            );
        }
    }

    /// <summary>
    /// Async collections and items API operations.
    /// </summary>
    public sealed class CollectionsApiAsync
    {
        private readonly ForTemClientAsync _client;

        public CollectionsApiAsync(ForTemClientAsync client)
        {
            _client = client;
        }

        /// <summary>
        /// Asynchronously get all game collections.
        /// </summary>
        public async Task<List<Collection>> GetCollectionsAsync()
        {
            return await _client.HttpClient.SendRequestAsync<List<Collection>>(
                "/api/v1/developers/collections",
                System.Net.Http.HttpMethod.Get
            );
        }

        /// <summary>
        /// Asynchronously create a new collection.
        /// </summary>
        /// <remarks>
        /// Maximum 5 collections allowed per developer account.
        /// </remarks>
        public async Task<Collection> CreateCollectionAsync(string name, string description)
        {
            var request = new CreateCollectionRequest(name, description);
            string body = JsonUtility.ToJson(request);

            return await _client.HttpClient.SendRequestAsync<Collection>(
                "/api/v1/developers/collections",
                System.Net.Http.HttpMethod.Post,
                body
            );
        }

        /// <summary>
        /// Asynchronously get a specific item from a collection.
        /// </summary>
        public async Task<CollectionItem> GetCollectionItemAsync(int collectionId, string itemCode)
        {
            string endpoint = $"/api/v1/developers/collections/{collectionId}/items/{itemCode}";
            return await _client.HttpClient.SendRequestAsync<CollectionItem>(
                endpoint,
                System.Net.Http.HttpMethod.Get
            );
        }

        /// <summary>
        /// Asynchronously create a new item in a collection.
        /// </summary>
        public async Task<ItemCreationResponse> CreateCollectionItemAsync(
            int collectionId,
            string name,
            int quantity,
            string redeemCode,
            string redeemUrl = "",
            string description = "",
            string itemImageCid = "",
            List<ItemAttribute> attributes = null,
            string recipientAddress = "")
        {
            var request = new CreateCollectionItemRequest(
                name,
                quantity,
                redeemCode,
                redeemUrl,
                description,
                itemImageCid,
                attributes,
                recipientAddress
            );

            string body = JsonUtility.ToJson(request);
            body = System.Text.RegularExpressions.Regex.Replace(body, "\"[^\"]+\":\"\"[,]?", string.Empty).Replace(",}", "}");
            string endpoint = $"/api/v1/developers/collections/{collectionId}/items";

            return await _client.HttpClient.SendRequestAsync<ItemCreationResponse>(
                endpoint,
                System.Net.Http.HttpMethod.Post,
                body
            );
        }

        /// <summary>
        /// Asynchronously upload an image for collection items.
        /// </summary>
        public async Task<ImageUploadResponse> UploadImageAsync(int collectionId, byte[] imageData)
        {
            string endpoint = $"/api/v1/developers/collections/{collectionId}/items/image-upload";
            return await _client.HttpClient.SendRequestMultipartAsync<ImageUploadResponse>(
                endpoint,
                imageData,
                "itemImage"
            );
            //return await _client.HttpClient.SendRequestAsync<ImageUploadResponse>(
            //    endpoint,
            //    System.Net.Http.HttpMethod.Put,
            //    body: null,
            //    bodyBytes: imageData
            //);
        }
    }
}
