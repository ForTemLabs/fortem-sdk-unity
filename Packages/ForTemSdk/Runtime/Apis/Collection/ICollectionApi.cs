using System.Collections.Generic;
using System.Threading.Tasks;

namespace ForTemSdk
{
    public interface ICollectionApi
    {
        Task<CollectionResponse> CreateCollection(CreateCollectionRequest requestBody);
        Task<List<CollectionResponse>> GetCollections();
    }
}