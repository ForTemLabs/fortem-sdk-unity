using System.Threading.Tasks;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    /// <summary>
    /// User management API operations.
    /// </summary>
    public sealed class UserApi : ForTemApiBase
    {
        public UserApi(ForTemClient client) : base(client)
        {
        }

        /// <summary>
        /// Get user information by wallet address.
        /// </summary>
        public async Task<UserResponseData> GetUser(string walletAddress)
        {
            var accessToken = await _client.Authenticate(forMinting: false);

            string endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/users/{walletAddress}";
            var request = UnityWebRequest.Get(endpoint);
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<UserResponseData>(request);

            return response;
        }
    }
}
