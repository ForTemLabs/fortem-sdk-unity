# ForTem SDK - API Wrapper Documentation

## Overview

The ForTem SDK provides a consumer-friendly wrapper around the ForTem Web3 Gaming API. It simplifies authentication, user management, and NFT collection operations for Unity games.

## Key Features

- **Nonce-Based Authentication** - Secure developer API key authentication flow
- **User Lookup** - Query user information by wallet address
- **Collection Management** - Create and browse game NFT collections
- **Item Management** - Create, query, and manage NFT items
- **Image Upload** - Upload item images via IPFS integration
- **Automatic Caching** - Built-in response caching for better performance
- **Retry Logic** - Automatic request handling with timeouts
- **Debug Logging** - Optional detailed logging for development
- **Environment Support** - Seamlessly switch between Testnet and Mainnet

## Quick Start

### 1. Initialize the SDK

```csharp
using ForTem.SDK;

var config = new ForTemConfig(
    apiKey: "your-api-key",
    environment: ForTemEnvironment.Mainnet,
    debugLogging: true
);

ForTemClient.Initialize(config);
```

### 2. Authenticate (Nonce-Based Flow)

```csharp
// Step 1: Get a nonce
ForTemClient.Instance.Auth.GetNonce((result) =>
{
    if (result.Success)
    {
        string nonce = result.Data.nonce;
        
        // Step 2: Exchange nonce for access token
        ForTemClient.Instance.Auth.GetAccessToken(nonce, (tokenResult) =>
        {
            if (tokenResult.Success)
            {
                Debug.Log("Authenticated!");
            }
        });
    }
});
```

### 3. Get User Information

```csharp
string walletAddress = "0x904bb53d5508de51fdf1d3c3960fd597e52cb39ae11c562ca22f1acbb2702d8b";

ForTemClient.Instance.User.GetUser(walletAddress, (result) =>
{
    if (result.Success)
    {
        Debug.Log($"User: {result.Data.nickname}");
        Debug.Log($"Wallet: {result.Data.walletAddress}");
    }
});
```

### 4. Browse Collections

```csharp
ForTemClient.Instance.Collections.GetCollections((result) =>
{
    if (result.Success)
    {
        foreach (var collection in result.Data)
        {
            Debug.Log($"{collection.name}: {collection.itemCount} items");
        }
    }
});
```

## API Reference

### ForTemClient

Main entry point for all SDK operations. Singleton instance that manages configuration and delegates requests to sub-APIs.

**Properties:**
- `Auth` - Authentication operations
- `User` - User management operations
- `Collections` - Collection and item operations

**Methods:**
- `Initialize(ForTemConfig)` - Static method to initialize the SDK

### AuthApi

Handles nonce-based authentication operations.

**Methods:**
- `GetNonce(callback)` - Get a nonce for authentication
- `GetAccessToken(nonce, callback)` - Exchange nonce for access token

### UserApi

Manages user profile operations.

**Methods:**
- `GetUser(walletAddress, callback)` - Get user information by wallet address

### CollectionsApi

Manages game collections and NFT items.

**Methods:**
- `GetCollections(callback)` - List all collections
- `CreateCollection(name, description, callback)` - Create a new collection
- `GetCollectionItem(collectionId, itemCode, callback)` - Get specific item by code
- `CreateCollectionItem(collectionId, name, quantity, redeemCode, ...)` - Create new item
- `UploadImage(collectionId, imageData, callback)` - Upload item image

## Data Models

### AccessTokenResponse
```csharp
public class AccessTokenResponse
{
    public string accessToken { get; set; }  // JWT token for API requests
}
```

### NonceResponse
```csharp
public class NonceResponse
{
    public string nonce { get; set; }  // Random nonce for authentication
}
```

### User
```csharp
public class User
{
    public bool isUser { get; set; }
    public string nickname { get; set; }
    public string profileImage { get; set; }
    public string walletAddress { get; set; }
}
```

### Collection
```csharp
public class Collection
{
    public int id { get; set; }
    public string objectId { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public string tradeVolume { get; set; }
    public int itemCount { get; set; }
    public Link link { get; set; }
    public long createdAt { get; set; }
    public long updatedAt { get; set; }
}
```

### CollectionItem
```csharp
public class CollectionItem
{
    public int id { get; set; }
    public string objectId { get; set; }
    public string name { get; set; }
    public string description { get; set; }
    public int nftNumber { get; set; }
    public string itemImage { get; set; }
    public int quantity { get; set; }
    public List<Attribute> attributes { get; set; }
    public Owner owner { get; set; }
    public string status { get; set; }  // REDEEMED, PROCESSING, etc.
    public long createdAt { get; set; }
    public long updatedAt { get; set; }
}
```

### ItemCreationResponse
```csharp
public class ItemCreationResponse
{
    public int itemId { get; set; }
    public string name { get; set; }
    public int collectionId { get; set; }
    public string redeemCode { get; set; }
    public string status { get; set; }  // PROCESSING, REDEEMED, etc.
    // ... other fields
}
```

## Configuration

### ForTemConfig

Configure SDK behavior on initialization:

```csharp
var config = new ForTemConfig(
    apiKey: "your-api-key",
    environment: ForTemEnvironment.Mainnet, // or Testnet
    debugLogging: false
)
{
    TimeoutSeconds = 30 // Request timeout
};
```

**Properties:**
- `ApiKey` - Your ForTem API key (used in x-api-key header)
- `Environment` - Target environment (Testnet/Mainnet)
- `DebugLogging` - Enable debug output to console
- `TimeoutSeconds` - HTTP request timeout (default: 30s)

**Methods:**
- `GetApiBaseUrl()` - Get the API base URL for the environment
- `GetServiceUrl()` - Get the service URL for the environment

## Authentication Flow

The ForTem API uses a nonce-based authentication system for developer applications:

```
1. Client calls /api/v1/developers/auth/nonce with x-api-key header
2. Server returns a nonce string
3. Client exchanges nonce for access token via /api/v1/developers/auth/access-token
4. Client uses Bearer token for all subsequent API calls
```

## Endpoints

### Authentication
- `POST /api/v1/developers/auth/nonce` - Get authentication nonce
- `POST /api/v1/developers/auth/access-token` - Get access token

### User
- `GET /api/v1/developers/users/:walletAddress` - Get user by wallet address

### Collections
- `GET /api/v1/developers/collections` - List all collections
- `POST /api/v1/developers/collections` - Create collection
- `GET /api/v1/developers/collections/:collectionId/items/:code` - Get item by code
- `POST /api/v1/developers/collections/:collectionId/items` - Create item
- `PUT /api/v1/developers/collections/:collectionId/items/image-upload` - Upload image

## Response Format

All API responses follow this standard format:

```json
{
  "statusCode": 200,
  "data": {
    // Response data
  }
}
```

## Advanced Usage

### Enable Debug Logging

```csharp
var config = new ForTemConfig("api-key", debugLogging: true);
```

This logs all API requests and responses to the Unity Console.

### Handle Responses

All callbacks receive an `ApiCallResult<T>` with the following properties:

```csharp
public class ApiCallResult<T> where T : class
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Error { get; set; }
    public int StatusCode { get; set; }
}
```

### Caching

GET requests are automatically cached for 5 minutes. Clear the cache with:

```csharp
ForTemClient.Instance.HttpClient.ClearCache();
```

## Environments

### Testnet
- API: https://testnet-api.fortem.gg
- Service: https://testnet.fortem.gg

### Mainnet
- API: https://api.fortem.gg
- Service: https://fortem.gg

## Error Handling

Always check the `Success` property before accessing data:

```csharp
callback: (result) =>
{
    if (result.Success)
    {
        // Use result.Data
    }
    else
    {
        Debug.LogError($"Error ({result.StatusCode}): {result.Error}");
    }
}
```

## Examples

See `Examples.cs` in the Runtime folder for complete working examples of:
- Nonce-based authentication
- User lookup
- Collection browsing
- Item creation
- Image uploads

## Support

For issues or questions about the ForTem API, visit: https://docs.fortem.gg
