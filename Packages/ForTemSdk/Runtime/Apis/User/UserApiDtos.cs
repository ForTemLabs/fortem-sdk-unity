using System;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// User information from ForTem API.
    /// </summary>
    [Serializable]
    public sealed class GetUserResponse
    {
        [SerializeField] private bool isUser;
        [SerializeField] private string nickname;
        [SerializeField] private string profileImage;
        [SerializeField] private string walletAddress;

        /// <summary>
        /// Indicates if the user is registered on ForTem.
        /// </summary>
        public bool IsUser => isUser;

        /// <summary>
        /// User's nickname.
        /// </summary>
        public string Nickname => nickname;

        /// <summary>
        /// URL of the user's profile image.<br/>
        /// Default: profile/default.png
        /// </summary>
        public string ProfileImage => profileImage;

        /// <summary>
        /// User's wallet address.
        /// </summary>
        public string WalletAddress => walletAddress;
    }
}
