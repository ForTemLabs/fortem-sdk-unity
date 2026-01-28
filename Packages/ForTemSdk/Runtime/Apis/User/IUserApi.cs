using System.Threading.Tasks;

namespace ForTemSdk
{
    public interface IUserApi
    {
        Task<GetUserResponse> GetUser(string walletAddress);
    }
}