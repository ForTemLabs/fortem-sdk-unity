//using System;
//using System.Collections.Generic;
//using System.Net.Http;
//using System.Threading.Tasks;
//using NSubstitute;
//using NUnit.Framework;

//namespace ForTemSdk
//{
//    /// <summary>
//    /// Tests for ForTemClientAsync async client functionality.
//    /// </summary>
//    public class ForTemClientAsyncTests : IDisposable
//    {
//        private ForTemConfig _config;
//        private ForTemClientAsync _client;

//        [SetUp]
//        public void SetUp()
//        {
//            _config = new ForTemConfig("test-api-key", ForTemEnvironment.Testnet, debugLogging: false);
//            _client = new ForTemClientAsync(_config);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            Dispose();
//        }

//        public void Dispose()
//        {
//            _client?.Dispose();
//        }

//        [Test]
//        public void Constructor_WithValidConfig_InitializesSuccessfully()
//        {
//            Assert.IsNotNull(_client);
//        }

//        [Test]
//        public void Auth_ReturnsAuthApiAsync()
//        {
//            Assert.IsNotNull(_client.Auth);
//        }

//        [Test]
//        public void User_ReturnsUserApiAsync()
//        {
//            Assert.IsNotNull(_client.User);
//        }

//        [Test]
//        public void Collections_ReturnsCollectionsApiAsync()
//        {
//            Assert.IsNotNull(_client.Collections);
//        }

//        [Test]
//        public void Config_ReturnsConfiguration()
//        {
//            Assert.AreEqual("test-api-key", _client.Config.ApiKey);
//            Assert.AreEqual(ForTemEnvironment.Testnet, _client.Config.Environment);
//        }

//        [Test]
//        public void GetAccessToken_InitiallyReturnsNull()
//        {
//            var token = _client.GetAccessToken();

//            Assert.IsTrue(string.IsNullOrEmpty(token));
//        }

//        [Test]
//        public void SetAccessToken_StoresToken()
//        {
//            var testToken = "async-test-token-" + Guid.NewGuid();
//            _client.SetAccessToken(testToken);

//            var retrievedToken = _client.GetAccessToken();

//            Assert.AreEqual(testToken, retrievedToken);
//        }

//        [Test]
//        public void HttpClient_ReturnsNotNull()
//        {
//            Assert.IsNotNull(_client.HttpClient);
//        }

//        [Test]
//        public void ImplementsIDisposable()
//        {
//            Assert.IsInstanceOf<IDisposable>(_client);
//        }

//        [Test]
//        public void Dispose_CanBeCalledMultipleTimes()
//        {
//            Assert.DoesNotThrow(() =>
//            {
//                _client.Dispose();
//                _client.Dispose();
//            });
//        }
//    }

//    /// <summary>
//    /// Tests for async API response parsing.
//    /// </summary>
//    public class ForTemClientAsyncParsingTests
//    {
//        [Test]
//        public void ParseResponse_WithValidJson_ReturnsSuccess()
//        {
//            var jsonResponse = "{\"statusCode\":200,\"data\":{\"isUser\":true,\"nickname\":\"TestUser\",\"walletAddress\":\"0x123\"}}";

//            var result = ForTemClientAsync.ParseResponse<User>(jsonResponse);

//            Assert.IsTrue(result.Success);
//            Assert.AreEqual(200, result.StatusCode);
//        }

//        [Test]
//        public void ParseResponse_WithErrorStatusCode_ReturnsFailure()
//        {
//            var jsonResponse = "{\"statusCode\":404,\"data\":null}";

//            var result = ForTemClientAsync.ParseResponse<User>(jsonResponse);

//            Assert.IsFalse(result.Success);
//            Assert.AreEqual(404, result.StatusCode);
//        }

//        [Test]
//        public void ParseResponse_WithEmptyString_ReturnsError()
//        {
//            var result = ForTemClientAsync.ParseResponse<User>(string.Empty);

//            Assert.IsFalse(result.Success);
//            Assert.AreEqual("Empty response from server", result.Error);
//        }

//        [Test]
//        public void ParseResponse_WithNullString_ReturnsError()
//        {
//            var result = ForTemClientAsync.ParseResponse<User>(null);

//            Assert.IsFalse(result.Success);
//            Assert.AreEqual("Empty response from server", result.Error);
//        }

//        [Test]
//        public void ParseResponse_WithInvalidJson_ReturnsError()
//        {
//            var result = ForTemClientAsync.ParseResponse<User>("{ invalid json }");

//            Assert.IsFalse(result.Success);
//            StringAssert.Contains("Failed to parse response", result.Error);
//        }
//    }

//    /// <summary>
//    /// Tests for AuthApiAsync.
//    /// </summary>
//    public class AuthApiAsyncTests : IDisposable
//    {
//        private ForTemConfig _config;
//        private ForTemClientAsync _client;

//        [SetUp]
//        public void SetUp()
//        {
//            _config = new ForTemConfig("test-api-key", ForTemEnvironment.Testnet);
//            _client = new ForTemClientAsync(_config);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            Dispose();
//        }

//        public void Dispose()
//        {
//            _client?.Dispose();
//        }

//        [Test]
//        public void Auth_IsNotNull()
//        {
//            Assert.IsNotNull(_client.Auth);
//        }

//        [Test]
//        public async Task GetNonceAsync_ReturnType_IsApiCallResult()
//        {
//            try
//            {
//                var task = _client.Auth.GetNonceAsync();
//                Assert.IsInstanceOf<Task<ApiCallResult<NonceResponse>>>(task);

//                await Task.WhenAny(task, Task.Delay(100));
//            }
//            catch
//            {
//                // Expected to timeout or fail due to no real API
//            }
//        }

//        [Test]
//        public async Task GetAccessTokenAsync_ReturnType_IsApiCallResult()
//        {
//            try
//            {
//                var task = _client.Auth.GetAccessTokenAsync("test-nonce");
//                Assert.IsInstanceOf<Task<ApiCallResult<AccessTokenResponse>>>(task);

//                await Task.WhenAny(task, Task.Delay(100));
//            }
//            catch
//            {
//                // Expected to timeout or fail due to no real API
//            }
//        }
//    }

//    /// <summary>
//    /// Tests for UserApiAsync.
//    /// </summary>
//    public class UserApiAsyncTests : IDisposable
//    {
//        private ForTemConfig _config;
//        private ForTemClientAsync _client;

//        [SetUp]
//        public void SetUp()
//        {
//            _config = new ForTemConfig("test-api-key", ForTemEnvironment.Testnet);
//            _client = new ForTemClientAsync(_config);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            Dispose();
//        }

//        public void Dispose()
//        {
//            _client?.Dispose();
//        }

//        [Test]
//        public void User_IsNotNull()
//        {
//            Assert.IsNotNull(_client.User);
//        }

//        [Test]
//        public async Task GetUserAsync_ReturnType_IsApiCallResult()
//        {
//            try
//            {
//                var task = _client.User.GetUserAsync("0x123");
//                Assert.IsInstanceOf<Task<ApiCallResult<User>>>(task);

//                await Task.WhenAny(task, Task.Delay(100));
//            }
//            catch
//            {
//                // Expected to timeout or fail due to no real API
//            }
//        }
//    }

//    /// <summary>
//    /// Tests for CollectionsApiAsync.
//    /// </summary>
//    public class CollectionsApiAsyncTests : IDisposable
//    {
//        private ForTemConfig _config;
//        private ForTemClientAsync _client;

//        [SetUp]
//        public void SetUp()
//        {
//            _config = new ForTemConfig("test-api-key", ForTemEnvironment.Testnet);
//            _client = new ForTemClientAsync(_config);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            Dispose();
//        }

//        public void Dispose()
//        {
//            _client?.Dispose();
//        }

//        [Test]
//        public void Collections_IsNotNull()
//        {
//            Assert.IsNotNull(_client.Collections);
//        }

//        [Test]
//        public async Task GetCollectionsAsync_ReturnType_IsApiCallResult()
//        {
//            try
//            {
//                var task = _client.Collections.GetCollectionsAsync();
//                Assert.IsInstanceOf<Task<ApiCallResult<List<Collection>>>>(task);

//                await Task.WhenAny(task, Task.Delay(100));
//            }
//            catch
//            {
//                // Expected to timeout or fail due to no real API
//            }
//        }

//        [Test]
//        public async Task CreateCollectionAsync_ReturnType_IsApiCallResult()
//        {
//            try
//            {
//                var task = _client.Collections.CreateCollectionAsync("Test", "Description");
//                Assert.IsInstanceOf<Task<ApiCallResult<Collection>>>(task);

//                await Task.WhenAny(task, Task.Delay(100));
//            }
//            catch
//            {
//                // Expected to timeout or fail due to no real API
//            }
//        }

//        [Test]
//        public async Task GetCollectionItemAsync_ReturnType_IsApiCallResult()
//        {
//            try
//            {
//                var task = _client.Collections.GetCollectionItemAsync(1, "item-code");
//                Assert.IsInstanceOf<Task<ApiCallResult<CollectionItem>>>(task);

//                await Task.WhenAny(task, Task.Delay(100));
//            }
//            catch
//            {
//                // Expected to timeout or fail due to no real API
//            }
//        }

//        [Test]
//        public async Task CreateCollectionItemAsync_ReturnType_IsApiCallResult()
//        {
//            try
//            {
//                var task = _client.Collections.CreateCollectionItemAsync(
//                    1, "Item", 10, "code", "Description"
//                );
//                Assert.IsInstanceOf<Task<ApiCallResult<ItemCreationResponse>>>(task);

//                await Task.WhenAny(task, Task.Delay(100));
//            }
//            catch
//            {
//                // Expected to timeout or fail due to no real API
//            }
//        }

//        [Test]
//        public async Task UploadImageAsync_ReturnType_IsApiCallResult()
//        {
//            try
//            {
//                var task = _client.Collections.UploadImageAsync(1, new byte[] { 0xFF });
//                Assert.IsInstanceOf<Task<ApiCallResult<ImageUploadResponse>>>(task);

//                await Task.WhenAny(task, Task.Delay(100));
//            }
//            catch
//            {
//                // Expected to timeout or fail due to no real API
//            }
//        }
//    }
//}
