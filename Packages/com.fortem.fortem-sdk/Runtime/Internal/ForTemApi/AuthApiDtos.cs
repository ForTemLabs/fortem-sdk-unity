using System;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Nonce response for authentication initiation.
    /// </summary>
    [Serializable]
    internal class NonceResponse
    {
        [SerializeField] private string nonce;

        /// <summary>
        /// Nonce value for authentication.
        /// </summary>
        public string Nonce => nonce;
    }

    /// <summary>
    /// Request to exchange nonce for access token.
    /// </summary>
    [Serializable]
    internal class AccessTokenRequest
    {
        [SerializeField] private string nonce;

        /// <summary>
        /// Nonce value obtained from the authentication initiation.
        /// </summary>
        public string Nonce { get => nonce; set => nonce = value; }
    }

    /// <summary>
    /// Access token response from authentication.
    /// </summary>
    [Serializable]
    internal class AccessTokenResponse
    {
        [SerializeField] private string accessToken;

        /// <summary>
        /// Access token for authenticated API requests. Valid for up to 5 minutes.
        /// When minting a Collection or Item, the access token is single-use for that request and cannot be reused afterward.
        /// </summary>
        public string AccessToken => accessToken;
    }
}
