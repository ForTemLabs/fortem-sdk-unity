using System;
using System.Collections.Generic;
using System.Net.Http;
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
        private ForTemHttpClientAsync _httpClient;
        private string _accessToken;

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
            _httpClient = new ForTemHttpClientAsync(_config, this);
            
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
        internal ForTemHttpClientAsync HttpClient => _httpClient;

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

        /// <summary>
        /// Helper method to parse API responses.
        /// </summary>
        internal static ApiCallResult<T> ParseResponse<T>(string responseText) where T : class
        {
            var result = new ApiCallResult<T>
            {
                Success = false,
                Error = "Unknown error"
            };

            if (string.IsNullOrEmpty(responseText))
            {
                result.Error = "Empty response from server";
                return result;
            }

            try
            {
                // Parse the standard API response wrapper
                var apiResponse = JsonUtility.FromJson<ApiResponse<T>>(responseText);
                
                result.StatusCode = apiResponse.statusCode;
                result.Success = apiResponse.statusCode == 200;
                result.Data = apiResponse.data;

                if (!result.Success)
                {
                    result.Error = $"API returned status code {apiResponse.statusCode}";
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Error = $"Failed to parse response: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Disposes the HTTP client and resources.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
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
        public async Task<ApiCallResult<NonceResponse>> GetNonceAsync()
        {
            try
            {
                var customHeaders = new Dictionary<string, string>
                {
                    { "x-api-key", _client.Config.ApiKey }
                };

                string response = await _client.HttpClient.SendRequestAsync(
                    "/api/v1/developers/auth/nonce",
                    System.Net.Http.HttpMethod.Post,
                    null,
                    customHeaders
                );

                return ForTemClientAsync.ParseResponse<NonceResponse>(response);
            }
            catch (Exception ex)
            {
                return new ApiCallResult<NonceResponse>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// Asynchronously exchange nonce for access token.
        /// </summary>
        public async Task<ApiCallResult<AccessTokenResponse>> GetAccessTokenAsync(string nonce)
        {
            try
            {
                var request = new TokenExchangeRequest(nonce);
                string body = JsonUtility.ToJson(request);

                var customHeaders = new Dictionary<string, string>
                {
                    { "x-api-key", _client.Config.ApiKey }
                };

                string response = await _client.HttpClient.SendRequestAsync(
                    "/api/v1/developers/auth/access-token",
                    System.Net.Http.HttpMethod.Post,
                    body,
                    customHeaders
                );

                var result = ForTemClientAsync.ParseResponse<AccessTokenResponse>(response);
                
                // Store token if successful
                if (result.Success && result.Data != null)
                {
                    _client.SetAccessToken(result.Data.AccessToken);
                }
                
                return result;
            }
            catch (Exception ex)
            {
                return new ApiCallResult<AccessTokenResponse>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
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
        public async Task<ApiCallResult<User>> GetUserAsync(string walletAddress)
        {
            try
            {
                string endpoint = $"/api/v1/developers/users/{walletAddress}";
                string response = await _client.HttpClient.SendRequestAsync(
                    endpoint,
                    System.Net.Http.HttpMethod.Get
                );

                return ForTemClientAsync.ParseResponse<User>(response);
            }
            catch (Exception ex)
            {
                return new ApiCallResult<User>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
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
        public async Task<ApiCallResult<List<Collection>>> GetCollectionsAsync()
        {
            try
            {
                string response = await _client.HttpClient.SendRequestAsync(
                    "/api/v1/developers/collections",
                    System.Net.Http.HttpMethod.Get
                );

                return ForTemClientAsync.ParseResponse<List<Collection>>(response);
            }
            catch (Exception ex)
            {
                return new ApiCallResult<List<Collection>>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// Asynchronously create a new collection.
        /// </summary>
        public async Task<ApiCallResult<Collection>> CreateCollectionAsync(string name, string description)
        {
            try
            {
                var request = new CreateCollectionRequest(name, description);
                string body = JsonUtility.ToJson(request);

                string response = await _client.HttpClient.SendRequestAsync(
                    "/api/v1/developers/collections",
                    System.Net.Http.HttpMethod.Post,
                    body
                );

                return ForTemClientAsync.ParseResponse<Collection>(response);
            }
            catch (Exception ex)
            {
                return new ApiCallResult<Collection>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// Asynchronously get a specific item from a collection.
        /// </summary>
        public async Task<ApiCallResult<CollectionItem>> GetCollectionItemAsync(int collectionId, string itemCode)
        {
            try
            {
                string endpoint = $"/api/v1/developers/collections/{collectionId}/items/{itemCode}";
                string response = await _client.HttpClient.SendRequestAsync(
                    endpoint,
                    System.Net.Http.HttpMethod.Get
                );

                return ForTemClientAsync.ParseResponse<CollectionItem>(response);
            }
            catch (Exception ex)
            {
                return new ApiCallResult<CollectionItem>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// Asynchronously create a new item in a collection.
        /// </summary>
        public async Task<ApiCallResult<ItemCreationResponse>> CreateCollectionItemAsync(
            int collectionId,
            string name,
            int quantity,
            string redeemCode,
            string description = "",
            string itemImageCid = "",
            List<ItemAttribute> attributes = null,
            string recipientAddress = "")
        {
            try
            {
                var itemData = new
                {
                    name,
                    quantity,
                    redeemCode,
                    description,
                    itemImage = itemImageCid,
                    attributes,
                    recipientAddress
                };
                string body = JsonUtility.ToJson(itemData);

                string endpoint = $"/api/v1/developers/collections/{collectionId}/items";
                string response = await _client.HttpClient.SendRequestAsync(
                    endpoint,
                    System.Net.Http.HttpMethod.Post,
                    body
                );

                return ForTemClientAsync.ParseResponse<ItemCreationResponse>(response);
            }
            catch (Exception ex)
            {
                return new ApiCallResult<ItemCreationResponse>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        /// <summary>
        /// Asynchronously upload an image for collection items.
        /// </summary>
        public async Task<ApiCallResult<ImageUploadResponse>> UploadImageAsync(int collectionId, byte[] imageData)
        {
            try
            {
                string endpoint = $"/api/v1/developers/collections/{collectionId}/items/image-upload";
                string response = await _client.HttpClient.SendRequestMultipartAsync(
                    endpoint,
                    imageData,
                    "itemImage"
                );

                return ForTemClientAsync.ParseResponse<ImageUploadResponse>(response);
            }
            catch (Exception ex)
            {
                return new ApiCallResult<ImageUploadResponse>
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }
    }
}
