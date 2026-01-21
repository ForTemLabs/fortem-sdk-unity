# ForTem Core Package

A lightweight framework for Unity development with a consumer-friendly wrapper SDK around the ForTem Web3 Gaming API.

## Features

- **Dual API Support** - Choose between coroutines or modern async/await
- **ForTem SDK** - Easy-to-use wrapper for the ForTem Web3 API
- **Authentication** - Seamless nonce-based login and token management
- **User Management** - Get and update user profiles
- **NFT Collections** - Browse and manage game collections and items
- **Caching & Performance** - Built-in response caching and optimization
- **Multiple Environments** - Support for Testnet and Mainnet
- **Debug Tools** - Optional detailed logging for development
- **Well-Documented** - Comprehensive API documentation and examples

## Installation

This package is designed to be installed via Git URL or local path through the Package Manager.

### Via Git URL
```
https://github.com/ForTemLabs/fortem-sdk.git#main
```

### Via Local Path
In `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.fortem.core": "file:../ForTem.Unity/Packages/ForTem"
  }
}
```

## APIs

The SDK provides **two implementations** - choose what works best for your project:

### 1. Coroutine-Based API (Traditional)
Uses `UnityWebRequest` and coroutines. Great for traditional Unity workflows.

```csharp
using ForTem.SDK;

// Initialize
var config = new ForTemConfig("your-api-key", ForTemEnvironment.Mainnet);
ForTemClient.Initialize(config);

// Use with callbacks
ForTemClient.Instance.Auth.GetNonce((result) =>
{
    if (result.Success)
        Debug.Log("Got nonce: " + result.Data.nonce);
});
```

**See:** [SDK.md](./Documentation~/SDK.md) for complete coroutine API documentation

### 2. Async/Await API (Modern)
Uses `HttpClient` and modern async/await patterns. Better performance and cleaner code.

```csharp
using ForTem.SDK.Async;

// Initialize
var config = new ForTemConfig("your-api-key", ForTemEnvironment.Mainnet);
var client = new ForTemClientAsync(config);

// Use with async/await
var result = await client.Auth.GetNonceAsync();
if (result.Success)
    Debug.Log("Got nonce: " + result.Data.nonce);
```

**See:** [AsyncAPI.md](./Documentation~/AsyncAPI.md) for complete async API documentation

## Quick Comparison

| Feature | Coroutines | Async/Await |
|---------|-----------|-----------|
| **Class** | `ForTemClient` | `ForTemClientAsync` |
| **HTTP Client** | UnityWebRequest | HttpClient |
| **Pattern** | Callbacks | async/await |
| **Use Case** | Traditional Unity | Modern C# |
| **Performance** | Good | Better |

Choose **coroutines** if you prefer traditional Unity patterns.  
Choose **async/await** for modern C# syntax and better composability.

## Quick Start - Coroutine API

### 1. Initialize the SDK
```csharp
using ForTem.SDK;

var config = new ForTemConfig("your-api-key", ForTemEnvironment.Mainnet);
ForTemClient.Initialize(config);
```

### 2. Authenticate (Nonce-Based)
```csharp
// Step 1: Get nonce
ForTemClient.Instance.Auth.GetNonce((result) =>
{
    if (result.Success)
    {
        // Step 2: Exchange for token
        ForTemClient.Instance.Auth.GetAccessToken(result.Data.nonce, (tokenResult) =>
        {
            if (tokenResult.Success)
                Debug.Log("Authenticated!");
        });
    }
});
```

### 3. Get User Info
```csharp
string walletAddress = "0x...";

ForTemClient.Instance.User.GetUser(walletAddress, (result) =>
{
    if (result.Success)
        Debug.Log($"User: {result.Data.nickname}");
});
```

### 4. Browse Collections
```csharp
ForTemClient.Instance.Collections.GetCollections((result) =>
{
    if (result.Success)
    {
        foreach (var collection in result.Data)
            Debug.Log(collection.name);
    }
});
```

## Quick Start - Async/Await API

```csharp
using ForTem.SDK.Async;

var config = new ForTemConfig("your-api-key", ForTemEnvironment.Mainnet);
var client = new ForTemClientAsync(config);

// Authenticate
var nonce = await client.Auth.GetNonceAsync();
var token = await client.Auth.GetAccessTokenAsync(nonce.Data.nonce);

// Query data
var user = await client.User.GetUserAsync("0x...");
var collections = await client.Collections.GetCollectionsAsync();
```

## Documentation

- **[SDK Documentation (Coroutines)](./Documentation~/SDK.md)** - Complete API reference for coroutine-based API
- **[Async API Documentation](./Documentation~/AsyncAPI.md)** - Complete API reference for async/await API
- **[Core API](./Documentation~/README.md)** - Framework core documentation


## License

MIT License - See LICENSE.md for details.
