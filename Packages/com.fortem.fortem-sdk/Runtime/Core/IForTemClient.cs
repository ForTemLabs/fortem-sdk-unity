using System.Collections.Generic;
using System.Threading.Tasks;

#nullable enable

namespace ForTemSdk
{
    public interface IForTemClient
    {
        Task<GetUserResponse> GetUser(string walletAddress);

        Task<List<CollectionResponse>> GetCollections();
        Task<CollectionResponse> CreateCollection(CreateCollectionRequest requestBody);

        Task<GetItemResponse> GetItem(int collectionId, string redeemCode);
        Task<CreateItemResponse> CreateItem(int collectionId, CreateItemRequest requestBody);
        Task<CreateItemResponse> CreateItemWithImage(
            int collectionId,
            CreateItemRequest item,
            byte[] imageData,
            string fileName);
    }
}
