using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ForTemSdk
{
    /// <summary>
    /// Async user management API operations.
    /// </summary>
    public sealed class UserApi : ForTemApiBase
    {
        public UserApi(ForTemClient client) : base(client)
        {
        }

        /// <summary>
        /// Asynchronously get user information by wallet address.
        /// </summary>
        public async Task<UserResponseData> GetUser(string walletAddress)
        {
            var accessToken = await _client.Authenticate(forMinting: false);

            string endpoint = $"/api/v1/developers/users/{walletAddress}";
            var request = UnityWebRequest.Get(endpoint);
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<UserResponseData>(request);

            return response;
        }
    }
}
