using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ForTemSdk.Samples
{

    /// <summary>
    /// Example usage of the async ForTem SDK with async/await pattern.
    /// </summary>
    public class AsyncAuthenticationExample : MonoBehaviour
    {
        [SerializeField]
        private string _apiKey;

        private ForTemClientAsync forTemClient;

        private async void Start()
        {
            //var obj = Print<NonceResponse>(
            //    @"{
            //        ""statusCode"": 200,
            //        ""data"": {
            //            ""nonce"": ""example-nonce-value""
            //        }
            //    }"
            //);
            //Debug.Log($"Status code: {obj.statusCode}");
            //Debug.Log($"Parsed nonce: {obj?.data?.nonce}");
            //return;

            // Initialize the SDK
            var config = new ForTemConfig(
                apiKey: _apiKey,
                environment: ForTemEnvironment.Testnet,
                debugLogging: true
            );

            forTemClient = new ForTemClientAsync(config);

            // Example: Perform nonce-based authentication
            await ExampleAuthenticate();
        }

        private ApiResponse<T> Print<T>(string json)
        {
            return JsonUtility.FromJson<ApiResponse<T>>(json);
        }

        private async Task ExampleAuthenticate()
        {
            // Step 1: Get nonce
            var nonceResult = await forTemClient.Auth.GetNonceAsync();
            
            if (nonceResult.Success)
            {
                Debug.Log($"Got nonce: {nonceResult.Data.nonce}");
                
                // Step 2: Exchange nonce for token
                var tokenResult = await forTemClient.Auth.GetAccessTokenAsync(nonceResult.Data.nonce);
                
                if (tokenResult.Success)
                {
                    Debug.Log($"Got Access Token: {tokenResult.Data.AccessToken}");
                    await ExampleCreateCollection();
                }
                else
                {
                    Debug.LogError($"Failed to get access token: {tokenResult.Error}");
                }
            }
            else
            {
                Debug.LogError($"Failed to get nonce: {nonceResult.Error}");
            }
        }

        private async Task ExampleCreateCollection()
        {
            var result = await forTemClient.Collections.CreateCollectionAsync(
                "My Game Collection",
                "A collection for my game items"
            );

            if (result.Success)
            {
                Debug.Log($"Created collection: {result.Data.Name}: {JsonUtility.ToJson(result.Data)}");
            }
            else
            {
                Debug.LogError($"Failed to create collection: {result.Error}");
            }
        }

        private void OnDestroy()
        {
            // Clean up resources
            forTemClient?.Dispose();
        }
    }

    /// <summary>
    /// Example of async user operations.
    /// </summary>
    public class AsyncUserExample : MonoBehaviour
    {
        private ForTemClientAsync forTemClient;

        private async void Start()
        {
            var config = new ForTemConfig("your-api-key", ForTemEnvironment.Mainnet);
            forTemClient = new ForTemClientAsync(config);

            await ExampleGetUser();
        }

        private async Task ExampleGetUser()
        {
            string walletAddress = "0x904bb53d5508de51fdf1d3c3960fd597e52cb39ae11c562ca22f1acbb2702d8b";
            
            var result = await forTemClient.User.GetUserAsync(walletAddress);
            
            if (result.Success)
            {
                Debug.Log($"IsUser: {result.Data.IsUser}");
                Debug.Log($"User: {result.Data.Nickname}");
                Debug.Log($"Wallet: {result.Data.WalletAddress}");
                Debug.Log($"Profile Image: {result.Data.ProfileImage}");
            }
            else
            {
                Debug.LogError($"Failed to get user: {result.Error}");
            }
        }

        private void OnDestroy()
        {
            forTemClient?.Dispose();
        }
    }

    /// <summary>
    /// Example of async collection operations.
    /// </summary>
    public class AsyncCollectionsExample : MonoBehaviour
    {
        private ForTemClientAsync forTemClient;

        private async void Start()
        {
            var config = new ForTemConfig("your-api-key");
            forTemClient = new ForTemClientAsync(config);

            await ExampleGetCollections();
        }

        private async Task ExampleGetCollections()
        {
            var result = await forTemClient.Collections.GetCollectionsAsync();
            
            if (result.Success)
            {
                Debug.Log($"Found {result.Data.Count} collections");
                foreach (var collection in result.Data)
                {
                    Debug.Log($"Collection: {collection.Name} (ID: {collection.ID}, Items: {collection.ItemCount})");
                }
            }
            else
            {
                Debug.LogError($"Failed to get collections: {result.Error}");
            }
        }

        private async Task ExampleCreateCollection()
        {
            var result = await forTemClient.Collections.CreateCollectionAsync(
                "My Game Collection",
                "A collection for my game items"
            );

            if (result.Success)
            {
                Debug.Log($"Created collection: {result.Data.Name} (ID: {result.Data.ID})");
            }
            else
            {
                Debug.LogError($"Failed to create collection: {result.Error}");
            }
        }

        private async Task ExampleGetCollectionItem(int collectionId, string itemCode)
        {
            var result = await forTemClient.Collections.GetCollectionItemAsync(collectionId, itemCode);
            
            if (result.Success)
            {
                Debug.Log($"Item: {result.Data.Name}");
                Debug.Log($"Owner: {result.Data.Owner.Nickname} ({result.Data.Owner.WalletAddress})");
                Debug.Log($"Quantity: {result.Data.Quantity}");
                Debug.Log($"Status: {result.Data.Status}");
            }
            else
            {
                Debug.LogError($"Failed to get item: {result.Error}");
            }
        }

        private async Task ExampleCreateCollectionItem(int collectionId)
        {
            var attributes = new List<ItemAttribute>
            {
                new ItemAttribute { Name = "Rarity", Value = "Epic" },
                new ItemAttribute { Name = "Element", Value = "Fire" }
            };

            var result = await forTemClient.Collections.CreateCollectionItemAsync(
                collectionId: collectionId,
                name: "Legendary Sword",
                quantity: 5,
                redeemCode: "ITEM-2026-001",
                description: "A legendary sword of great power",
                attributes: attributes
            );

            if (result.Success)
            {
                Debug.Log($"Created item: {result.Data.Name} (ID: {result.Data.ItemID})");
                Debug.Log($"Redeem code: {result.Data.RedeemCode}");
                Debug.Log($"Status: {result.Data.Status}");
            }
            else
            {
                Debug.LogError($"Failed to create item: {result.Error}");
            }
        }

        private async Task ExampleUploadImage(int collectionId, byte[] imageData)
        {
            var result = await forTemClient.Collections.UploadImageAsync(collectionId, imageData);
            
            if (result.Success)
            {
                Debug.Log($"Image uploaded successfully!");
                Debug.Log($"Image CID: {result.Data.ItemImage}");
            }
            else
            {
                Debug.LogError($"Failed to upload image: {result.Error}");
            }
        }

        private void OnDestroy()
        {
            forTemClient?.Dispose();
        }
    }

    /// <summary>
    /// Advanced example showing error handling and task composition with async/await.
    /// </summary>
    public class AsyncAdvancedExample : MonoBehaviour
    {
        private ForTemClientAsync forTemClient;

        private async void Start()
        {
            var config = new ForTemConfig("your-api-key", debugLogging: true);
            forTemClient = new ForTemClientAsync(config);

            await ExampleCompleteWorkflow();
        }

        private async Task ExampleCompleteWorkflow()
        {
            try
            {
                // Step 1: Authenticate
                var nonceResult = await forTemClient.Auth.GetNonceAsync();
                if (!nonceResult.Success)
                {
                    Debug.LogError($"Authentication failed: {nonceResult.Error}");
                    return;
                }

                var tokenResult = await forTemClient.Auth.GetAccessTokenAsync(nonceResult.Data.nonce);
                if (!tokenResult.Success)
                {
                    Debug.LogError($"Token exchange failed: {tokenResult.Error}");
                    return;
                }

                Debug.Log("Authentication successful!");

                // Step 2: Get collections in parallel
                var collectionsResult = await forTemClient.Collections.GetCollectionsAsync();
                if (collectionsResult.Success)
                {
                    Debug.Log($"Collections: {collectionsResult.Data.Count}");
                }

                // Step 3: For each collection, get an item
                foreach (var collection in collectionsResult.Data)
                {
                    var itemResult = await forTemClient.Collections.GetCollectionItemAsync(collection.ID, "item-1");
                    if (itemResult.Success)
                    {
                        Debug.Log($"Item in collection {collection.Name}: {itemResult.Data.Name}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Workflow failed: {ex.Message}");
            }
        }

        private void OnDestroy()
        {
            forTemClient?.Dispose();
        }
    }
}
