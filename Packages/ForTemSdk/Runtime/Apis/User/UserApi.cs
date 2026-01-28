using System.Threading.Tasks;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    /// <summary>
    /// User management API operations.
    /// </summary>
    internal sealed class UserApi : /*ForTemApiBase,*/ IUserApi
    {
        private readonly ForTemClientHelper _helper;
        private readonly AuthApi _authApi;

        public UserApi(ForTemClientHelper helper, AuthApi authApi)
            //: base(webRequestSender, authApi)
        {
            _helper = helper;
            _authApi = authApi;
        }

        /// <summary>
        /// Retrieves ForTem user information based on a Sui wallet address.
        /// Useful to verify whether a given wallet address belongs to a registered ForTem user.
        /// </summary>
        public async Task<GetUserResponse> GetUser(string walletAddress)
        {
            var accessToken = await _authApi.Authenticate(forMinting: false);

            string endpoint = $"{_helper.Config.GetApiBaseUrl()}/api/v1/developers/users/{walletAddress}";
            using var request = UnityWebRequest.Get(endpoint);
            request.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            var response = await _helper.SendWebRequest<GetUserResponse>(request);

            return response;
        }
    }
}
