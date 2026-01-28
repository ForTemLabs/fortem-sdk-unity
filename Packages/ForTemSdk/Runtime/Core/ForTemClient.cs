using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    internal sealed class DefaultWebRequestSender : IWebRequestSender
    {
        public DefaultWebRequestSender(ForTemConfig config)
        {
            //Config = config;
        }

        //public ForTemConfig Config { get; }

        public Task<WebRequestResponse> Send(UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<WebRequestResponse>();
            var operation = request.SendWebRequest();
            var response = new WebRequestResponse
            {
                Text = request.downloadHandler.text,
                Error = request.error,
                ResponseCode = request.responseCode,
                Result = request.result,
            };
            operation.completed += _ => tcs.SetResult(response);
            return tcs.Task;
        }
    }

    /// <summary>
    /// Main client class for interacting with the ForTem Rest API.
    /// </summary>
    public sealed class ForTemClient
    {
        private readonly ForTemConfig _config;

        // Token management
        private string? _accessToken;
        private long _expiresAt;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        // API endpoints
        private readonly AuthApi _authApi;
        private readonly UserApi _userApi;
        private readonly CollectionApi _collectionApi;
        private readonly ItemApi _itemApi;

        public ForTemClient(ForTemConfig config)
        {
            var webRequestSender = new DefaultWebRequestSender(config);
            var helper = new ForTemClientHelper(webRequestSender, config);
            _config = config;
            _config.Logger.Log($"[ForTem] SDK initialized - Environment: {_config.Environment}");

            _authApi = new AuthApi(helper);
            _userApi = new UserApi(helper, _authApi);
            _collectionApi = new CollectionApi(helper, _authApi);
            _itemApi = new ItemApi(helper, _authApi);
        }

        /// <summary>
        /// Gets the User API for user management operations.
        /// </summary>
        public IUserApi UserApi => _userApi;

        /// <summary>
        /// Gets the Collections API for game collections and items.
        /// </summary>
        public ICollectionApi CollectionApi => _collectionApi;

        /// <summary>
        /// Gets the Collections API for game collections and items.
        /// </summary>
        public IItemApi ItemApi => _itemApi;

        /// <summary>
        /// Gets the Auth API for authentication operations.
        /// </summary>
        internal AuthApi Auth => _authApi;

        /// <summary>
        /// Internal: Gets the current configuration.
        /// </summary>
        internal ForTemConfig Config => _config;

        internal async Task<string> Authenticate(bool forMinting)
        {
            if (forMinting)
            {
                string nonce = await Auth.GetNonce();
                string accessToken = await Auth.GetAccessToken(nonce);
                return accessToken;
            }

            await _semaphore.WaitAsync();
            try
            {
                if (string.IsNullOrEmpty(_accessToken) || DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() >= _expiresAt - 30000)
                {
                    var nonce = await Auth.GetNonce();
                    var accessToken = await Auth.GetAccessToken(nonce);
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
    }
}
