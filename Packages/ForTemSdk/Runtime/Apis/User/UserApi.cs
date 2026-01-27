using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    /// <summary>
    /// User management API operations.
    /// </summary>
    public sealed class UserApi : ForTemApiBase
    {
        internal UserApi(ForTemClient client) : base(client)
        {
        }

        /// <summary>
        /// Retrieves ForTem user information based on a Sui wallet address.
        /// Useful to verify whether a given wallet address belongs to a registered ForTem user.
        /// </summary>
        public async Task<GetUserResponse> GetUser(string walletAddress)
        {
            var accessToken = await _client.Authenticate(forMinting: false);

            string endpoint = $"{_client.Config.GetApiBaseUrl()}/api/v1/developers/users/{walletAddress}";
            using var request = UnityWebRequest.Get(endpoint);
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await SendWebRequest<GetUserResponse>(request);

            return response;
        }
    }
}
