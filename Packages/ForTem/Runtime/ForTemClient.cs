using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Primary ForTem SDK client for asynchronous operations.
    /// </summary>
    public sealed class ForTemClient
    {
        private ForTemConfig _config;

        // Token management
        private string _accessToken;
        private long _expiresAt;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        // API endpoints
        private readonly AuthApi _authApi;
        private readonly UserApi _userApi;
        private readonly CollectionsApi _collectionsApi;

        public ForTemClient(ForTemConfig config)
        {
            SetConfig(config);
            _authApi = new AuthApi(this);
            _userApi = new UserApi(this);
            _collectionsApi = new CollectionsApi(this);
        }

        private void SetConfig(ForTemConfig newConfig)
        {
            _config = newConfig;

            if (_config.DebugLogging)
            {
                Debug.Log($"[ForTem] Async SDK initialized - Environment: {_config.Environment}");
            }
        }

        /// <summary>
        /// Gets the Auth API for authentication operations.
        /// </summary>
        internal AuthApi Auth => _authApi;

        /// <summary>
        /// Gets the User API for user management operations.
        /// </summary>
        public UserApi User => _userApi;

        /// <summary>
        /// Gets the Collections API for game collections and items.
        /// </summary>
        public CollectionsApi Collections => _collectionsApi;

        /// <summary>
        /// Internal: Gets the current configuration.
        /// </summary>
        internal ForTemConfig Config => _config;

        /// <summary>
        /// Internal: Gets the current access token.
        /// </summary>
        internal string GetAccessToken()
        {
            return _accessToken;
        }

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
