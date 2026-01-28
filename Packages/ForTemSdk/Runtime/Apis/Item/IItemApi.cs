using System.Threading.Tasks;

namespace ForTemSdk
{
    public interface IItemApi
    {
        Task<CreateItemResponse> CreateItem(int collectionId, CreateItemRequest requestBody);
        Task<CreateItemResponse> CreateItemWithImage(int collectionId, CreateItemRequest item, byte[] imageData, string fileName);
        Task<GetItemResponse> GetItem(int collectionId, string redeemCode);
    }
}