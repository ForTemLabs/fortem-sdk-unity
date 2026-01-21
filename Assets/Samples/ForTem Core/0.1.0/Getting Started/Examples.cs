using UnityEngine;

namespace ForTemSdk.Samples
{

    /// <summary>
    /// Example usage of the ForTem SDK for authentication.
    /// </summary>
    public class AuthenticationExample : MonoBehaviour
    {
        private void Start()
        {
            // Initialize the SDK with your API key
            var config = new ForTemConfig(
                apiKey: "your-api-key-here",
                environment: ForTemEnvironment.Testnet,
                debugLogging: true
            );
            ForTemClient.Initialize(config);

            // Example: Get nonce and then access token (nonce-based auth flow)
            StartCoroutine(ExampleAuthenticate());
        }

        private System.Collections.IEnumerator ExampleAuthenticate()
        {
            // Step 1: Get a nonce
            yield return ForTemClient.Instance.Auth.GetNonce((result) =>
            {
                if (result.Success)
                {
                    Debug.Log($"Got nonce: {result.Data.nonce}");
                    
                    // Step 2: Exchange nonce for access token
                    StartCoroutine(GetAccessToken(result.Data.nonce));
                }
                else
                {
                    Debug.LogError($"Failed to get nonce: {result.Error}");
                }
            });
        }

        private System.Collections.IEnumerator GetAccessToken(string nonce)
        {
            yield return ForTemClient.Instance.Auth.GetAccessToken(nonce, (result) =>
            {
                if (result.Success)
                {
                    Debug.Log($"Got access token!");
                    // Token is automatically stored in the client
                }
                else
                {
                    Debug.LogError($"Failed to get access token: {result.Error}");
                }
            });
        }
    }

    /// <summary>
    /// Example usage of the ForTem SDK for user operations.
    /// </summary>
    public class UserExample : MonoBehaviour
    {
        private void Start()
        {
            // Get user info by wallet address
            StartCoroutine(ExampleGetUser());
        }

        private System.Collections.IEnumerator ExampleGetUser()
        {
            // Replace with an actual wallet address
            string walletAddress = "0x904bb53d5508de51fdf1d3c3960fd597e52cb39ae11c562ca22f1acbb2702d8b";
            
            yield return ForTemClient.Instance.User.GetUser(walletAddress, (result) =>
            {
                if (result.Success)
                {
                    Debug.Log($"User: {result.Data.Nickname}");
                    Debug.Log($"Wallet: {result.Data.WalletAddress}");
                    Debug.Log($"Profile Image: {result.Data.ProfileImage}");
                }
                else
                {
                    Debug.LogError($"Failed to get user: {result.Error}");
                }
            });
        }
    }

    /// <summary>
    /// Example usage of the ForTem SDK for collections and items.
    /// </summary>
    public class CollectionsExample : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(ExampleGetCollections());
        }

        private System.Collections.IEnumerator ExampleGetCollections()
        {
            // Get all collections
            yield return ForTemClient.Instance.Collections.GetCollections((result) =>
            {
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
            });
        }

        private System.Collections.IEnumerator ExampleCreateCollection()
        {
            // Create a new collection
            yield return ForTemClient.Instance.Collections.CreateCollection(
                "My Game Collection",
                "A collection for my game items",
                callback: (result) =>
                {
                    if (result.Success)
                    {
                        Debug.Log($"Created collection: {result.Data.Name} (ID: {result.Data.ID})");
                    }
                    else
                    {
                        Debug.LogError($"Failed to create collection: {result.Error}");
                    }
                }
            );
        }

        private System.Collections.IEnumerator ExampleGetCollectionItem(int collectionId, string itemCode)
        {
            // Get a specific item from a collection
            yield return ForTemClient.Instance.Collections.GetCollectionItem(
                collectionId,
                itemCode,
                callback: (result) =>
                {
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
            );
        }

        private System.Collections.IEnumerator ExampleCreateCollectionItem(int collectionId)
        {
            // Create a new item in a collection
            var attributes = new System.Collections.Generic.List<ItemAttribute>
            {
                new ItemAttribute { Name = "Rarity", Value = "Epic" },
                new ItemAttribute { Name = "Element", Value = "Fire" }
            };

            yield return ForTemClient.Instance.Collections.CreateCollectionItem(
                collectionId: collectionId,
                name: "Legendary Sword",
                quantity: 5,
                redeemCode: "ITEM-2026-001",
                description: "A legendary sword of great power",
                attributes: attributes,
                callback: (result) =>
                {
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
            );
        }

        private System.Collections.IEnumerator ExampleUploadImage(int collectionId, byte[] imageData)
        {
            // Upload an image for items
            yield return ForTemClient.Instance.Collections.UploadImage(
                collectionId,
                imageData,
                callback: (result) =>
                {
                    if (result.Success)
                    {
                        Debug.Log($"Image uploaded successfully!");
                        Debug.Log($"Image CID: {result.Data.ItemImage}");
                        // Use this CID when creating items
                    }
                    else
                    {
                        Debug.LogError($"Failed to upload image: {result.Error}");
                    }
                }
            );
        }
    }
}
