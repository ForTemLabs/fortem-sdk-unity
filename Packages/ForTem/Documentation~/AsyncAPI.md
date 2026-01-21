# ForTem SDK - Async/Await API Documentation

## Overview

The ForTem SDK provides an async/await version alongside the traditional coroutine-based API. This modern async/await implementation uses C#'s `HttpClient` instead of Unity's `UnityWebRequest`, offering better performance and more intuitive code flow.

## When to Use Async API vs Coroutines

### Use Async/Await When:
- You prefer modern C# async/await syntax
- You want cleaner, more readable code
- You're building a task-heavy application
- You need better error handling and debugging
- You want to compose multiple async operations easily

### Use Coroutines When:
- You need to maintain compatibility with older code
- You prefer UnityEngine's standard patterns
- You're working with UI animations or visual effects
- You want to integrate with MonoBehaviour lifecycle

## Quick Start

### 1. Create and Initialize Client

```csharp
using ForTem.SDK.Async;

var config = new ForTemConfig(
    apiKey: "your-api-key",
    environment: ForTemEnvironment.Mainnet,
    debugLogging: true
);

var client = new ForTemClientAsync(config);
```

### 2. Authenticate

```csharp
// Get nonce
var nonceResult = await client.Auth.GetNonceAsync();
if (nonceResult.Success)
{
    // Exchange for token
    var tokenResult = await client.Auth.GetAccessTokenAsync(nonceResult.Data.nonce);
    if (tokenResult.Success)
    {
        Debug.Log("Authenticated!");
    }
}
```

### 3. Query Data

```csharp
// Get user info
var userResult = await client.User.GetUserAsync("0x...");
if (userResult.Success)
{
    Debug.Log($"User: {userResult.Data.nickname}");
}

// Get collections
var collectionsResult = await client.Collections.GetCollectionsAsync();
if (collectionsResult.Success)
{
    foreach (var collection in collectionsResult.Data)
    {
        Debug.Log(collection.name);
    }
}
```

## API Reference

### ForTemClientAsync

Main async client for all ForTem operations.

**Constructor:**
```csharp
var client = new ForTemClientAsync(ForTemConfig config);
```

**Properties:**
- `Auth` - AuthApiAsync for authentication
- `User` - UserApiAsync for user operations
- `Collections` - CollectionsApiAsync for collections

**Methods:**
- `Dispose()` - Cleanup resources (recommended to call in OnDestroy)

### AuthApiAsync

Async authentication operations.

**Methods:**
```csharp
Task<ApiCallResult<NonceResponse>> GetNonceAsync()
Task<ApiCallResult<AccessTokenResponse>> GetAccessTokenAsync(string nonce)
```

### UserApiAsync

Async user operations.

**Methods:**
```csharp
Task<ApiCallResult<User>> GetUserAsync(string walletAddress)
```

### CollectionsApiAsync

Async collection and item operations.

**Methods:**
```csharp
Task<ApiCallResult<List<Collection>>> GetCollectionsAsync()

Task<ApiCallResult<Collection>> CreateCollectionAsync(
    string name, 
    string description)

Task<ApiCallResult<CollectionItem>> GetCollectionItemAsync(
    int collectionId, 
    string itemCode)

Task<ApiCallResult<ItemCreationResponse>> CreateCollectionItemAsync(
    int collectionId,
    string name,
    int quantity,
    string redeemCode,
    string description = "",
    string itemImageCid = "",
    List<Attribute> attributes = null,
    string recipientAddress = "")

Task<ApiCallResult<ImageUploadResponse>> UploadImageAsync(
    int collectionId, 
    byte[] imageData)
```

## Response Handling

All methods return `ApiCallResult<T>`:

```csharp
public class ApiCallResult<T> where T : class
{
    public bool Success { get; set; }
    public T Data { get; set; }
    public string Error { get; set; }
    public int StatusCode { get; set; }
}
```

**Pattern:**
```csharp
var result = await client.Collections.GetCollectionsAsync();

if (result.Success)
{
    // result.Data contains the response
    ProcessCollections(result.Data);
}
else
{
    // result.Error contains the error message
    Debug.LogError($"Error: {result.Error}");
}
```

## Advanced Usage

### Parallel Operations

```csharp
// Execute multiple requests in parallel
var collectionsTask = client.Collections.GetCollectionsAsync();
var userTask = client.User.GetUserAsync(walletAddress);

await Task.WhenAll(collectionsTask, userTask);

var collections = collectionsTask.Result;
var user = userTask.Result;
```

### Error Handling

```csharp
try
{
    var result = await client.Auth.GetAccessTokenAsync(nonce);
    if (!result.Success)
    {
        Debug.LogError($"API Error: {result.Error} (Code: {result.StatusCode})");
        return;
    }
    
    // Process success
}
catch (OperationCanceledException)
{
    Debug.LogError("Request timed out");
}
catch (Exception ex)
{
    Debug.LogError($"Unexpected error: {ex.Message}");
}
```

### Composing Async Operations

```csharp
private async Task CompleteWorkflow()
{
    // Authenticate
    var nonce = await GetAuthenticationToken();
    if (nonce == null) return;
    
    // Get collections
    var collections = await GetUserCollections();
    if (collections == null || collections.Count == 0) return;
    
    // Process first collection
    await ProcessCollection(collections[0]);
}

private async Task<string> GetAuthenticationToken()
{
    var nonceResult = await client.Auth.GetNonceAsync();
    if (!nonceResult.Success) return null;
    
    var tokenResult = await client.Auth.GetAccessTokenAsync(nonceResult.Data.nonce);
    return tokenResult.Success ? tokenResult.Data.accessToken : null;
}

private async Task<List<Collection>> GetUserCollections()
{
    var result = await client.Collections.GetCollectionsAsync();
    return result.Success ? result.Data : null;
}

private async Task ProcessCollection(Collection collection)
{
    // Work with the collection
}
```

### MonoBehaviour Integration

```csharp
public class MyGameManager : MonoBehaviour
{
    private ForTemClientAsync forTemClient;

    private async void Start()
    {
        var config = new ForTemConfig("api-key");
        forTemClient = new ForTemClientAsync(config);
        
        // Start async operations
        await InitializeGame();
    }

    private async Task InitializeGame()
    {
        var collections = await forTemClient.Collections.GetCollectionsAsync();
        if (collections.Success)
        {
            // Initialize game with collections
        }
    }

    private void OnDestroy()
    {
        // Clean up
        forTemClient?.Dispose();
    }
}
```

## Caching

The async HTTP client includes the same caching mechanism as the coroutine version:

```csharp
// GET requests are automatically cached for 5 minutes
var result1 = await client.Collections.GetCollectionsAsync();
var result2 = await client.Collections.GetCollectionsAsync(); // Uses cache

// Clear cache if needed
var httpClient = typeof(ForTemClientAsync)
    .GetProperty("HttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
    .GetValue(client) as ForTemHttpClientAsync;

httpClient?.ClearCache();
```

## Performance Considerations

- **HttpClient** vs **UnityWebRequest**: HttpClient is typically more efficient for multiple concurrent requests
- **Async/Await** vs **Coroutines**: Async/await has lower overhead and better CPU efficiency
- **Connection Pooling**: HttpClient automatically manages connection pooling for better performance
- **Caching**: Both implementations cache GET requests for 5 minutes

## Thread Safety

All async operations are safe to call from the main thread. Internally, they use proper synchronization with Unity's main thread where needed.

## Examples

See `ExamplesAsync.cs` for complete examples of:
- Authentication with async/await
- User queries
- Collection management
- Parallel operations
- Error handling
- Complex workflows

## Disposing Resources

Always dispose the client when done:

```csharp
client?.Dispose(); // In OnDestroy or when shutting down
```

This ensures all resources (HttpClient connections) are properly released.

## Comparison: Coroutines vs Async/Await

| Feature | Coroutines | Async/Await |
|---------|-----------|-----------|
| **Syntax** | yield return | await |
| **HTTP Client** | UnityWebRequest | HttpClient |
| **Learning Curve** | Steeper | Gentler |
| **Code Readability** | Good | Excellent |
| **Performance** | Good | Better |
| **Debugging** | Harder | Easier |
| **Unity Integration** | Native | Requires care |
| **Error Handling** | try/catch | try/catch |
| **Parallel Operations** | StartCoroutine multiple | Task.WhenAll |

## Troubleshooting

### "Async operation started but not awaited"
Make sure you're awaiting the task:
```csharp
// Wrong
client.Collections.GetCollectionsAsync();

// Right
await client.Collections.GetCollectionsAsync();
```

### "NullReferenceException after Dispose"
Check if client is disposed before using:
```csharp
if (forTemClient == null) return;
var result = await forTemClient.Collections.GetCollectionsAsync();
```

### TimeoutException
Increase timeout in config:
```csharp
var config = new ForTemConfig("api-key");
config.TimeoutSeconds = 60; // Increased timeout
var client = new ForTemClientAsync(config);
```

## Support

For issues or questions about the ForTem API, visit: https://docs.fortem.gg
