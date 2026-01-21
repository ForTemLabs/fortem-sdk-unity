# Interface Improvements for Testability

## Summary
To improve testability, three new interfaces were introduced that define contracts for key SDK components. These interfaces enable mocking and dependency injection.

## Interfaces Added

### 1. IForTemConfig
**Location**: `Runtime/Interfaces.cs`

Defines the contract for SDK configuration.

```csharp
public interface IForTemConfig
{
    string ApiKey { get; }
    ForTemEnvironment Environment { get; }
    bool DebugLogging { get; }
    int TimeoutSeconds { get; }
    string GetApiBaseUrl();
    string GetServiceUrl();
}
```

**Implemented by**: `ForTemConfig`

**Usage**:
```csharp
IForTemConfig config = new ForTemConfig("api-key");
var apiUrl = config.GetApiBaseUrl();
```

**Benefits**:
- Can mock configuration in tests
- Allows injectable dependencies
- Clear contract for configuration needs

---

### 2. IForTemHttpClient
**Location**: `Runtime/Interfaces.cs`

Defines the contract for coroutine-based HTTP client.

```csharp
public interface IForTemHttpClient
{
    IEnumerator SendRequest<T>(
        string endpoint,
        HttpMethod method,
        string body = null,
        Dictionary<string, string> customHeaders = null,
        bool useCache = true)
        where T : class;

    IEnumerator SendRequestMultipart<T>(
        string endpoint,
        byte[] fileData,
        string fieldName = "file")
        where T : class;

    void ClearCache();
}
```

**Implemented by**: `ForTemHttpClient`

**Usage**:
```csharp
IForTemHttpClient client = new ForTemHttpClient(config);
var request = client.SendRequest<User>(endpoint, HttpMethod.Get);
```

**Benefits**:
- Can mock HTTP responses in tests
- Can implement custom HTTP clients
- Separates HTTP concerns from business logic

---

### 3. IForTemHttpClientAsync
**Location**: `Runtime/Interfaces.cs`

Defines the contract for async/await HTTP client.

```csharp
public interface IForTemHttpClientAsync : IDisposable
{
    Task<string> SendRequestAsync(
        string endpoint,
        HttpMethod method,
        string body = null,
        Dictionary<string, string> customHeaders = null,
        bool useCache = true);

    Task<string> SendRequestMultipartAsync(
        string endpoint,
        byte[] fileData,
        string fieldName = "file",
        Dictionary<string, string> customHeaders = null);

    void ClearCache();
}
```

**Implemented by**: `ForTemHttpClientAsync`

**Usage**:
```csharp
using (IForTemHttpClientAsync client = new ForTemHttpClientAsync(config))
{
    var response = await client.SendRequestAsync(endpoint, HttpMethod.Get);
}
```

**Benefits**:
- Enables testing of async operations
- Supports resource cleanup via `IDisposable`
- Clear separation of concerns

---

## Implementation Details

### ForTemConfig : IForTemConfig
```csharp
public sealed class ForTemConfig : IForTemConfig
{
    public string ApiKey { get; set; }
    public ForTemEnvironment Environment { get; set; }
    public bool DebugLogging { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    
    public string GetApiBaseUrl() { /* ... */ }
    public string GetServiceUrl() { /* ... */ }
}
```

### ForTemHttpClient : IForTemHttpClient
```csharp
public sealed class ForTemHttpClient : IForTemHttpClient
{
    public IEnumerator SendRequest<T>(...) { /* ... */ }
    public IEnumerator SendRequestMultipart<T>(...) { /* ... */ }
    public void ClearCache() { /* ... */ }
}
```

### ForTemHttpClientAsync : IForTemHttpClientAsync
```csharp
public sealed class ForTemHttpClientAsync : IForTemHttpClientAsync
{
    public Task<string> SendRequestAsync(...) { /* ... */ }
    public Task<string> SendRequestMultipartAsync(...) { /* ... */ }
    public void ClearCache() { /* ... */ }
    public void Dispose() { /* ... */ }
}
```

---

## Testing with Interfaces

### Mock Configuration in Tests

**Before (No Interface)**:
```csharp
var config = new ForTemConfig("api-key");
// Hard to mock in tests
```

**After (With Interface)**:
```csharp
IForTemConfig config = Substitute.For<IForTemConfig>();
config.ApiKey.Returns("test-key");
config.GetApiBaseUrl().Returns("https://test-api.fortem.gg");
```

### Mock HTTP Client in Tests

**Before (No Interface)**:
```csharp
var httpClient = new ForTemHttpClient(config);
// Limited ability to test failure scenarios
```

**After (With Interface)**:
```csharp
IForTemHttpClient httpClient = Substitute.For<IForTemHttpClient>();
var fakeEnumerator = GetFakeEnumerator();
httpClient.SendRequest<User>(Arg.Any<string>(), Arg.Any<HttpMethod>())
    .Returns(fakeEnumerator);
```

---

## Benefits Summary

| Aspect | Before | After |
|--------|--------|-------|
| **Testability** | Limited | Full mocking support |
| **Flexibility** | Hard-coupled | Pluggable implementations |
| **Documentation** | Implicit contracts | Explicit interface contracts |
| **Maintenance** | Coupled changes | Loosely coupled |
| **DI Support** | Not possible | Fully supported |

---

## Backward Compatibility

All interfaces are additions with no breaking changes:
- Existing code using concrete classes continues to work
- New code can use interfaces for better testability
- No migration required for existing applications

---

## Future Enhancement Possibilities

1. **Dependency Injection Container**
   ```csharp
   // Example: Using DI container with interfaces
   container.Register<IForTemConfig>(new ForTemConfig("api-key"));
   container.Register<IForTemHttpClient, ForTemHttpClient>();
   ```

2. **Custom HTTP Client Implementation**
   ```csharp
   public class CustomHttpClient : IForTemHttpClient
   {
       // Custom implementation with logging, retry logic, etc.
   }
   ```

3. **Configuration Factory**
   ```csharp
   public class ConfigFactory
   {
       public IForTemConfig CreateConfig(string environment)
       {
           return new ForTemConfig("api-key", 
               environment == "testnet" ? ForTemEnvironment.Testnet : ForTemEnvironment.Mainnet);
       }
   }
   ```

---

## Conclusion

The addition of these three interfaces significantly improves the SDK's testability while maintaining backward compatibility. They provide clear contracts and enable flexible, testable implementations.
