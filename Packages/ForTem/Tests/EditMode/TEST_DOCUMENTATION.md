# ForTem SDK Test Suite Documentation

## Overview
Comprehensive test suite for the ForTem SDK with 60+ test cases covering all major components. Tests use NUnit and NSubstitute for mocking.

## Test Coverage

### 1. **ForTemConfigTests** (`ForTemConfigTests.cs`)
Tests for SDK configuration management:
- Constructor initialization with API key
- Environment selection (Testnet/Mainnet)
- Debug logging flags
- Timeout configuration
- API base URL generation
- Service URL generation
- Interface compliance

**10 test cases**

### 2. **ModelsTests** (`ModelsTests.cs`)
Tests for data models and API response types:
- `ApiCallResult<T>` generic properties and error handling
- `NonceResponse` nonce storage
- `AccessTokenResponse` token storage
- `User` profile information
- `Collection` collection metadata
- `CollectionItem` item details
- `ItemCreationResponse` creation responses
- `ImageUploadResponse` image upload results
- `ItemAttribute` attribute storage

**9 test cases**

### 3. **ForTemHttpClientTests** (`ForTemHttpClientTests.cs`)
Tests for coroutine-based HTTP client:
- Client initialization
- Interface implementation
- Cache clearing
- GET request handling
- POST request handling
- Custom header support
- Multipart form data for file uploads
- Caching behavior
- Empty file handling

**9 test cases**

### 4. **ForTemHttpClientAsyncTests** (`ForTemHttpClientAsyncTests.cs`)
Tests for async/await HTTP client:
- Client initialization
- Interface implementation (`IForTemHttpClientAsync`)
- `IDisposable` pattern
- Async GET requests
- Async POST requests
- Custom header support in async calls
- Async multipart form data uploads
- Multiple dispose calls (idempotent)

**8 test cases**

### 5. **ForTemClientTests** (`ForTemClientTests.cs`)
Tests for main coroutine-based client:

**Parsing Tests (ForTemClientParsingTests):**
- Valid JSON response parsing
- Empty response handling
- Null response handling
- Invalid JSON handling
- Error status codes
- List response parsing
- Nonce response parsing
- Access token response parsing

**API Tests (ForTemClientAPITests):**
- Singleton instance pattern
- Auth API access
- User API access
- Collections API access
- Configuration persistence
- Access token management
- HTTP client access

**Result Tests (ApiCallResultTests):**
- Generic result creation with different types
- Error message storage
- Status code storage

**18 test cases**

### 6. **ForTemClientAsyncTests** (`ForTemClientAsyncTests.cs`)
Tests for async/await main client:

**Main Client Tests (ForTemClientAsyncTests):**
- Client initialization
- Auth API access
- User API access
- Collections API access
- Configuration access
- Token management
- `IDisposable` implementation
- Multiple dispose calls

**Parsing Tests (ForTemClientAsyncParsingTests):**
- Valid JSON parsing
- Error status code handling
- Empty string handling
- Null string handling
- Invalid JSON handling

**Auth API Tests (AuthApiAsyncTests):**
- API access
- `GetNonceAsync()` return type
- `GetAccessTokenAsync()` return type

**User API Tests (UserApiAsyncTests):**
- API access
- `GetUserAsync()` return type

**Collections API Tests (CollectionsApiAsyncTests):**
- API access
- `GetCollectionsAsync()` return type
- `CreateCollectionAsync()` return type
- `GetCollectionItemAsync()` return type
- `CreateCollectionItemAsync()` return type
- `UploadImageAsync()` return type

**20 test cases**

### 7. **InterfaceComplianceTests** (`InterfaceComplianceTests.cs`)
Tests for interface contracts and enum validation:

**Interface Tests:**
- `ForTemConfig` implements `IForTemConfig`
- `ForTemHttpClient` implements `IForTemHttpClient`
- `ForTemHttpClientAsync` implements `IForTemHttpClientAsync`
- Interface property availability
- Interface method availability

**Environment Enum Tests:**
- Testnet value exists
- Mainnet value exists
- Environment comparison
- Enum conversion

**HTTP Method Enum Tests:**
- All HTTP methods defined (GET, POST, PUT, DELETE, PATCH)
- Method differentiation

**8 test cases**

### 8. **ForTemSDKIntegrationTests** (`NewTestScript.cs`)
Integration tests for complete SDK workflows:
- SDK initialization
- Mainnet initialization
- Testnet initialization
- API access after initialization
- HTTP client creation
- Async SDK creation
- Multiple configuration creation
- Response parsing with multiple types

**8 test cases**

## Testability Improvements

### New Interfaces (Interfaces.cs)
1. **IForTemConfig** - SDK configuration contract
   - ApiKey property
   - Environment property
   - DebugLogging property
   - TimeoutSeconds property
   - GetApiBaseUrl() method
   - GetServiceUrl() method

2. **IForTemHttpClient** - Coroutine HTTP client contract
   - SendRequest<T>() method
   - SendRequestMultipart<T>() method
   - ClearCache() method

3. **IForTemHttpClientAsync** - Async HTTP client contract
   - SendRequestAsync() method
   - SendRequestMultipartAsync() method
   - ClearCache() method
   - IDisposable implementation

### Implementations Updated
- `ForTemConfig` now implements `IForTemConfig`
- `ForTemHttpClient` now implements `IForTemHttpClient`
- `ForTemHttpClientAsync` now implements `IForTemHttpClientAsync`

## Running the Tests

### In Unity Editor:
1. Window > General > Test Runner
2. Select "EditMode"
3. Click "Run All" to execute all tests

### From Command Line:
```bash
unity -runTests -testPlatform editmode -testCategory "ForTemSdk"
```

## Test Statistics

- **Total Test Files**: 8
- **Total Test Cases**: 60+
- **Test Coverage**: Configuration, Models, HTTP Clients, API Clients, Interfaces, Integration

## Best Practices Demonstrated

1. **Isolation** - Each test class focuses on a specific component
2. **Clear Naming** - Test method names clearly describe what's being tested
3. **Setup/Teardown** - Proper initialization and cleanup in SetUp/TearDown methods
4. **Async Testing** - Proper async test patterns with timeout handling
5. **Interface Testing** - Verification of interface contracts
6. **Edge Cases** - Testing error conditions, null values, and edge cases
7. **Integration Testing** - End-to-end workflow testing

## Dependencies

- **NUnit** - Testing framework
- **NSubstitute** - Mocking library (referenced but usage can be expanded)
- **UnityEngine.TestTools** - Unity-specific testing utilities

## Future Enhancements

1. Mock HTTP responses with NSubstitute
2. Add performance benchmarks
3. Add stress tests for concurrent requests
4. Add security validation tests
5. Add networking error simulation tests
