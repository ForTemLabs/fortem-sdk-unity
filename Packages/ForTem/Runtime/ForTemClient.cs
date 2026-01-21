using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

namespace ForTemSdk
{
    /// <summary>
    /// Event wrapper for callback-based API operations.
    /// </summary>
    public sealed class ApiCallResult<T> where T : class
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string Error { get; set; }
        public int StatusCode { get; set; }
    }

    /// <summary>
    /// Main ForTem SDK client for easy access to all ForTem APIs.
    /// </summary>
    public sealed class ForTemClient : MonoBehaviour
    {
        private static ForTemClient _instance;
        private ForTemConfig _config;
        private ForTemHttpClient _httpClient;
        private string _accessToken;

        // API endpoints
        private readonly AuthApi _authApi;
        private readonly UserApi _userApi;
        private readonly CollectionsApi _collectionsApi;

        public static ForTemClient Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject obj = new GameObject("ForTemClient");
                    _instance = obj.AddComponent<ForTemClient>();
                    DontDestroyOnLoad(obj);
                }
                return _instance;
            }
        }

        public ForTemClient()
        {
            _authApi = new AuthApi(this);
            _userApi = new UserApi(this);
            _collectionsApi = new CollectionsApi(this);
        }

        /// <summary>
        /// Initializes the ForTem SDK with the given configuration.
        /// </summary>
        public static void Initialize(ForTemConfig config)
        {
            Instance.SetConfig(config);
        }

        private void SetConfig(ForTemConfig newConfig)
        {
            _config = newConfig;
            _httpClient = new ForTemHttpClient(newConfig, this);
            
            if (_config.DebugLogging)
            {
                Debug.Log($"[ForTem] SDK initialized - Environment: {_config.Environment}");
            }
        }

        /// <summary>
        /// Gets the Auth API for authentication operations.
        /// </summary>
        public AuthApi Auth => _authApi;

        /// <summary>
        /// Gets the User API for user management operations.
        /// </summary>
        public UserApi User => _userApi;

        /// <summary>
        /// Gets the Collections API for game collections and items.
        /// </summary>
        public CollectionsApi Collections => _collectionsApi;

        /// <summary>
        /// Internal: Gets the HTTP client for making API requests.
        /// </summary>
        internal ForTemHttpClient HttpClient => _httpClient ?? (_httpClient = new ForTemHttpClient(_config, this));

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
            catch (System.Exception ex)
            {
                result.Error = $"Failed to parse response: {ex.Message}";
                return result;
            }
        }
    }

    /// <summary>
    /// Authentication API operations using nonce-based flow.
    /// </summary>
    public sealed class AuthApi
    {
        private readonly ForTemClient _client;

        public AuthApi(ForTemClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Step 1: Request a nonce for authentication.
        /// </summary>
        public IEnumerator GetNonce(UnityAction<ApiCallResult<NonceResponse>> callback)
        {
            var customHeaders = new Dictionary<string, string>
            {
                { "x-api-key", _client.Config.ApiKey }
            };

            var requestEnum = _client.HttpClient.SendRequest<NonceResponse>(
                "/api/v1/developers/auth/nonce",
                HttpMethod.Post,
                null,
                customHeaders
            );
            yield return requestEnum;

            var result = requestEnum.Current as string;
            var parsedResult = ForTemClient.ParseResponse<NonceResponse>(result);
            callback?.Invoke(parsedResult);
        }

        //private IEnumerator GetNonceCoroutine(UnityAction<ApiCallResult<NonceResponse>> callback)
        //{
        //}

        /// <summary>
        /// Step 2: Exchange nonce for access token.
        /// </summary>
        public IEnumerator GetAccessToken(string nonce, UnityAction<ApiCallResult<AccessTokenResponse>> callback)
        {
            var request = new TokenExchangeRequest(nonce);
            string body = JsonUtility.ToJson(request);

            var customHeaders = new Dictionary<string, string>
            {
                { "x-api-key", _client.Config.ApiKey }
            };

            var requestEnum = _client.HttpClient.SendRequest<AccessTokenResponse>(
                "/api/v1/developers/auth/access-token",
                HttpMethod.Post,
                body,
                customHeaders
            );
            yield return requestEnum;

            var result = requestEnum.Current as string;
            var parsedResult = ForTemClient.ParseResponse<AccessTokenResponse>(result);

            if (parsedResult.Success && parsedResult.Data != null)
            {
                _client.SetAccessToken(parsedResult.Data.AccessToken);
            }

            callback?.Invoke(parsedResult);
        }

        //private IEnumerator GetAccessTokenCoroutine(string nonce, UnityAction<ApiCallResult<AccessTokenResponse>> callback)
        //{
        //}
    }

    /// <summary>
    /// User management API operations.
    /// </summary>
    public sealed class UserApi
    {
        private readonly ForTemClient _client;

        public UserApi(ForTemClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets user information by wallet address.
        /// </summary>
        public Coroutine GetUser(string walletAddress, UnityAction<ApiCallResult<User>> callback)
        {
            return _client.StartCoroutine(GetUserCoroutine(walletAddress, callback));
        }

        private IEnumerator GetUserCoroutine(string walletAddress, UnityAction<ApiCallResult<User>> callback)
        {
            string endpoint = $"/api/v1/developers/users/{walletAddress}";
            var requestEnum = _client.HttpClient.SendRequest<User>(endpoint, HttpMethod.Get);
            yield return requestEnum;

            var result = requestEnum.Current as string;
            var parsedResult = ForTemClient.ParseResponse<User>(result);
            callback?.Invoke(parsedResult);
        }
    }

    /// <summary>
    /// Collections and items API operations.
    /// </summary>
    public sealed class CollectionsApi
    {
        private readonly ForTemClient _client;

        public CollectionsApi(ForTemClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets all game collections.
        /// </summary>
        public Coroutine GetCollections(UnityAction<ApiCallResult<List<Collection>>> callback)
        {
            return _client.StartCoroutine(GetCollectionsCoroutine(callback));
        }

        private IEnumerator GetCollectionsCoroutine(UnityAction<ApiCallResult<List<Collection>>> callback)
        {
            var requestEnum = _client.HttpClient.SendRequest<List<Collection>>(
                "/api/v1/developers/collections",
                HttpMethod.Get
            );
            yield return requestEnum;

            var result = requestEnum.Current as string;
            var parsedResult = ForTemClient.ParseResponse<List<Collection>>(result);
            callback?.Invoke(parsedResult);
        }

        /// <summary>
        /// Creates a new collection.
        /// </summary>
        public Coroutine CreateCollection(string name, string description, UnityAction<ApiCallResult<Collection>> callback)
        {
            return _client.StartCoroutine(CreateCollectionCoroutine(name, description, callback));
        }

        private IEnumerator CreateCollectionCoroutine(string name, string description, UnityAction<ApiCallResult<Collection>> callback)
        {
            var request = new CreateCollectionRequest(name, description);
            string body = JsonUtility.ToJson(request);

            var requestEnum = _client.HttpClient.SendRequest<Collection>(
                "/api/v1/developers/collections",
                HttpMethod.Post,
                body
            );
            yield return requestEnum;

            var result = requestEnum.Current as string;
            var parsedResult = ForTemClient.ParseResponse<Collection>(result);
            callback?.Invoke(parsedResult);
        }

        /// <summary>
        /// Gets a specific item from a collection.
        /// </summary>
        public Coroutine GetCollectionItem(int collectionId, string itemCode, UnityAction<ApiCallResult<CollectionItem>> callback)
        {
            return _client.StartCoroutine(GetCollectionItemCoroutine(collectionId, itemCode, callback));
        }

        private IEnumerator GetCollectionItemCoroutine(int collectionId, string itemCode, UnityAction<ApiCallResult<CollectionItem>> callback)
        {
            string endpoint = $"/api/v1/developers/collections/{collectionId}/items/{itemCode}";
            var requestEnum = _client.HttpClient.SendRequest<CollectionItem>(endpoint, HttpMethod.Get);
            yield return requestEnum;

            var result = requestEnum.Current as string;
            var parsedResult = ForTemClient.ParseResponse<CollectionItem>(result);
            callback?.Invoke(parsedResult);
        }

        /// <summary>
        /// Creates a new item in a collection.
        /// </summary>
        public Coroutine CreateCollectionItem(
            int collectionId,
            string name,
            int quantity,
            string redeemCode,
            string description = "",
            string itemImageCid = "",
            List<ItemAttribute> attributes = null,
            string recipientAddress = "",
            UnityAction<ApiCallResult<ItemCreationResponse>> callback = null)
        {
            return _client.StartCoroutine(CreateCollectionItemCoroutine(
                collectionId, name, quantity, redeemCode, description, itemImageCid, attributes, recipientAddress, callback));
        }

        private IEnumerator CreateCollectionItemCoroutine(
            int collectionId,
            string name,
            int quantity,
            string redeemCode,
            string description,
            string itemImageCid,
            List<ItemAttribute> attributes,
            string recipientAddress,
            UnityAction<ApiCallResult<ItemCreationResponse>> callback)
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
            var requestEnum = _client.HttpClient.SendRequest<ItemCreationResponse>(
                endpoint,
                HttpMethod.Post,
                body
            );
            yield return requestEnum;

            var result = requestEnum.Current as string;
            var parsedResult = ForTemClient.ParseResponse<ItemCreationResponse>(result);
            callback?.Invoke(parsedResult);
        }

        /// <summary>
        /// Uploads an image for collection items.
        /// </summary>
        public Coroutine UploadImage(int collectionId, byte[] imageData, UnityAction<ApiCallResult<ImageUploadResponse>> callback)
        {
            return _client.StartCoroutine(UploadImageCoroutine(collectionId, imageData, callback));
        }

        private IEnumerator UploadImageCoroutine(int collectionId, byte[] imageData, UnityAction<ApiCallResult<ImageUploadResponse>> callback)
        {
            string endpoint = $"/api/v1/developers/collections/{collectionId}/items/image-upload";
            var requestEnum = _client.HttpClient.SendRequestMultipart<ImageUploadResponse>(
                endpoint,
                imageData,
                "itemImage"
            );
            yield return requestEnum;

            var result = requestEnum.Current as string;
            var parsedResult = ForTemClient.ParseResponse<ImageUploadResponse>(result);
            callback?.Invoke(parsedResult);
        }
    }
}
