//using System;
//using NUnit.Framework;
//using UnityEngine;

//namespace ForTemSdk
//{
//    /// <summary>
//    /// Tests for ForTemClient response parsing and API result handling.
//    /// </summary>
//    public class ForTemClientParsingTests
//    {
//        [Test]
//        public void ParseResponse_WithValidJsonResponse_ReturnsSuccessfulResult()
//        {
//            var jsonResponse = JsonUtility.ToJson(new ApiResponse<User>
//            {
//                statusCode = 200,
//                data = new User
//                {
//                    IsUser = true,
//                    Nickname = "TestUser",
//                    WalletAddress = "0xabc123",
//                    ProfileImage = "https://example.com/profile.png"
//                }
//            });

//            var result = ForTemClient.ParseResponse<User>(jsonResponse);

//            Assert.IsTrue(result.Success);
//            Assert.AreEqual(200, result.StatusCode);
//            Assert.IsNotNull(result.Data);
//            Assert.AreEqual("TestUser", result.Data.Nickname);
//        }

//        [Test]
//        public void ParseResponse_WithEmptyResponse_ReturnsError()
//        {
//            var result = ForTemClient.ParseResponse<User>(string.Empty);

//            Assert.IsFalse(result.Success);
//            Assert.AreEqual("Empty response from server", result.Error);
//        }

//        [Test]
//        public void ParseResponse_WithNullResponse_ReturnsError()
//        {
//            var result = ForTemClient.ParseResponse<User>(null);

//            Assert.IsFalse(result.Success);
//            Assert.AreEqual("Empty response from server", result.Error);
//        }

//        [Test]
//        public void ParseResponse_WithInvalidJson_ReturnsError()
//        {
//            var result = ForTemClient.ParseResponse<User>("not valid json");

//            Assert.IsFalse(result.Success);
//            StringAssert.Contains("Failed to parse response", result.Error);
//        }

//        [Test]
//        public void ParseResponse_WithErrorStatusCode_ReturnsError()
//        {
//            var jsonResponse = JsonUtility.ToJson(new ApiResponse<User>
//            {
//                statusCode = 404,
//                data = null
//            });

//            var result = ForTemClient.ParseResponse<User>(jsonResponse);

//            Assert.IsFalse(result.Success);
//            Assert.AreEqual(404, result.StatusCode);
//            StringAssert.Contains("404", result.Error);
//        }

//        [Test]
//        public void ParseResponse_WithList_ReturnsSuccessfulResult()
//        {
//            // Note: JsonUtility doesn't support serializing lists directly,
//            // but we test with the response wrapper
//            var jsonResponse = "{\"statusCode\":200,\"data\":[]}";

//            var result = ForTemClient.ParseResponse<System.Collections.Generic.List<Collection>>(jsonResponse);

//            Assert.AreEqual(200, result.StatusCode);
//        }

//        [Test]
//        public void ParseResponse_WithNonceResponse_ReturnsSuccessfulResult()
//        {
//            var jsonResponse = JsonUtility.ToJson(new ApiResponse<NonceResponse>
//            {
//                statusCode = 200,
//                data = new NonceResponse { nonce = "test-nonce-123" }
//            });

//            var result = ForTemClient.ParseResponse<NonceResponse>(jsonResponse);

//            Assert.IsTrue(result.Success);
//            Assert.AreEqual("test-nonce-123", result.Data.nonce);
//        }

//        [Test]
//        public void ParseResponse_WithAccessTokenResponse_ReturnsSuccessfulResult()
//        {
//            var jsonResponse = JsonUtility.ToJson(new ApiResponse<AccessTokenResponse>
//            {
//                statusCode = 200,
//                data = new AccessTokenResponse { AccessToken = "test-token-abc" }
//            });

//            var result = ForTemClient.ParseResponse<AccessTokenResponse>(jsonResponse);

//            Assert.IsTrue(result.Success);
//            Assert.AreEqual("test-token-abc", result.Data.AccessToken);
//        }
//    }

//    /// <summary>
//    /// Tests for ForTemClient API access and configuration.
//    /// </summary>
//    public class ForTemClientAPITests
//    {
//        [Test]
//        public void Instance_ReturnsNotNull()
//        {
//            var instance = ForTemClient.Instance;

//            Assert.IsNotNull(instance);
//        }

//        [Test]
//        public void Instance_ReturnsSingletonInstance()
//        {
//            var instance1 = ForTemClient.Instance;
//            var instance2 = ForTemClient.Instance;

//            Assert.AreSame(instance1, instance2);
//        }

//        [Test]
//        public void Auth_ReturnsAuthApi()
//        {
//            var auth = ForTemClient.Instance.Auth;

//            Assert.IsNotNull(auth);
//        }

//        [Test]
//        public void User_ReturnsUserApi()
//        {
//            var user = ForTemClient.Instance.User;

//            Assert.IsNotNull(user);
//        }

//        [Test]
//        public void Collections_ReturnsCollectionsApi()
//        {
//            var collections = ForTemClient.Instance.Collections;

//            Assert.IsNotNull(collections);
//        }

//        [Test]
//        public void Initialize_WithValidConfig_SetsConfiguration()
//        {
//            var config = new ForTemConfig("test-key", ForTemEnvironment.Testnet);

//            ForTemClient.Initialize(config);

//            Assert.AreEqual("test-key", ForTemClient.Instance.Config.ApiKey);
//            Assert.AreEqual(ForTemEnvironment.Testnet, ForTemClient.Instance.Config.Environment);
//        }

//        [Test]
//        public void GetAccessToken_InitiallyReturnsNull()
//        {
//            var token = ForTemClient.Instance.GetAccessToken();

//            // Initially should be null or empty
//            Assert.IsTrue(string.IsNullOrEmpty(token));
//        }

//        [Test]
//        public void SetAccessToken_StoresToken()
//        {
//            var testToken = "test-token-" + Guid.NewGuid();
//            ForTemClient.Instance.SetAccessToken(testToken);

//            var retrievedToken = ForTemClient.Instance.GetAccessToken();

//            Assert.AreEqual(testToken, retrievedToken);
//        }

//        [Test]
//        public void Config_ReturnsCurrentConfiguration()
//        {
//            var config = new ForTemConfig("test-key");
//            ForTemClient.Initialize(config);

//            var currentConfig = ForTemClient.Instance.Config;

//            Assert.IsNotNull(currentConfig);
//            Assert.AreEqual("test-key", currentConfig.ApiKey);
//        }

//        [Test]
//        public void HttpClient_ReturnsNotNull()
//        {
//            var httpClient = ForTemClient.Instance.HttpClient;

//            Assert.IsNotNull(httpClient);
//        }
//    }

//    /// <summary>
//    /// Tests for ApiCallResult generic type.
//    /// </summary>
//    public class ApiCallResultTests
//    {
//        [Test]
//        public void ApiCallResult_CanCreateWithUser()
//        {
//            var result = new ApiCallResult<User>
//            {
//                Success = true,
//                Data = new User { Nickname = "Test" }
//            };

//            Assert.IsTrue(result.Success);
//            Assert.AreEqual("Test", result.Data.Nickname);
//        }

//        [Test]
//        public void ApiCallResult_CanCreateWithCollection()
//        {
//            var result = new ApiCallResult<Collection>
//            {
//                Success = true,
//                Data = new Collection { Name = "TestCollection" }
//            };

//            Assert.IsTrue(result.Success);
//            Assert.AreEqual("TestCollection", result.Data.Name);
//        }

//        [Test]
//        public void ApiCallResult_CanStoreErrorMessage()
//        {
//            var result = new ApiCallResult<User>
//            {
//                Success = false,
//                Error = "Connection timeout"
//            };

//            Assert.IsFalse(result.Success);
//            Assert.AreEqual("Connection timeout", result.Error);
//        }

//        [Test]
//        public void ApiCallResult_CanStoreStatusCode()
//        {
//            var result = new ApiCallResult<User>
//            {
//                StatusCode = 401
//            };

//            Assert.AreEqual(401, result.StatusCode);
//        }
//    }
//}
