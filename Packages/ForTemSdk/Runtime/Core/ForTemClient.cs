using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

#nullable enable

namespace ForTemSdk
{
    /// <summary>
    /// Main client class for interacting with the ForTem Rest API.
    /// </summary>
    public sealed class ForTemClient
    {
        private readonly ForTemConfig _config;

        // Token management
        private string? _accessToken;
        private long _expiresAt;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        // API endpoints
        private readonly AuthApi _authApi;
        private readonly UserApi _userApi;
        private readonly CollectionApi _collectionApi;
        private readonly ItemApi _itemApi;

        public ForTemClient(ForTemConfig config)
        {
            _config = config;
            _config.Logger.Log($"[ForTem] SDK initialized - Environment: {_config.Environment}");
            _authApi = new AuthApi(this);
            _userApi = new UserApi(this);
            _collectionApi = new CollectionApi(this);
            _itemApi = new ItemApi(this);
        }

        /// <summary>
        /// Gets the User API for user management operations.
        /// </summary>
        public UserApi UserApi => _userApi;

        /// <summary>
        /// Gets the Collections API for game collections and items.
        /// </summary>
        public CollectionApi CollectionApi => _collectionApi;

        /// <summary>
        /// Gets the Collections API for game collections and items.
        /// </summary>
        public ItemApi ItemApi => _itemApi;

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
