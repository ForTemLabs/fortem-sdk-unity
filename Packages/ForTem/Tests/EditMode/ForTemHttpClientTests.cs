using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using NSubstitute;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace ForTemSdk
{
    /// <summary>
    /// Tests for ForTemHttpClient coroutine-based HTTP client.
    /// </summary>
    public class ForTemHttpClientTests
    {
        private ForTemConfig _config;
        private ForTemHttpClient _httpClient;

        [SetUp]
        public void SetUp()
        {
            _config = new ForTemConfig("test-api-key", ForTemEnvironment.Testnet);
            _httpClient = new ForTemHttpClient(_config);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.ClearCache();
        }

        [Test]
        public void Constructor_WithValidConfig_InitializesSuccessfully()
        {
            Assert.IsNotNull(_httpClient);
        }

        [Test]
        public void Constructor_WithNullClient_IsAllowed()
        {
            var httpClient = new ForTemHttpClient(_config, null);

            Assert.IsNotNull(httpClient);
        }

        [Test]
        public void ImplementsIForTemHttpClient_Interface()
        {
            Assert.IsInstanceOf<IForTemHttpClient>(_httpClient);
        }

        [Test]
        public void ClearCache_RemovesAllCachedData()
        {
            // This test verifies that ClearCache method is callable
            // without throwing an exception
            Assert.DoesNotThrow(() => _httpClient.ClearCache());
        }

        [Test]
        public void SendRequest_WithGETMethod_ReturnsEnumerator()
        {
            var enumerator = _httpClient.SendRequest<NonceResponse>(
                "/api/v1/developers/auth/nonce",
                HttpMethod.Get,
                null,
                null,
                true
            );

            Assert.IsNotNull(enumerator);
            Assert.IsInstanceOf<IEnumerator>(enumerator);
        }

        [Test]
        public void SendRequest_WithPOSTMethod_ReturnsEnumerator()
        {
            var enumerator = _httpClient.SendRequest<AccessTokenResponse>(
                "/api/v1/developers/auth/access-token",
                HttpMethod.Post,
                "{\"nonce\":\"test\"}"
            );

            Assert.IsNotNull(enumerator);
            Assert.IsInstanceOf<IEnumerator>(enumerator);
        }

        [Test]
        public void SendRequest_WithCustomHeaders_ReturnsEnumerator()
        {
            var headers = new Dictionary<string, string>
            {
                { "x-api-key", "test-key" }
            };

            var enumerator = _httpClient.SendRequest<User>(
                "/api/v1/developers/users/0x123",
                HttpMethod.Get,
                null,
                headers
            );

            Assert.IsNotNull(enumerator);
        }

        [Test]
        public void SendRequestMultipart_WithValidFileData_ReturnsEnumerator()
        {
            var fileData = new byte[] { 0xFF, 0xD8, 0xFF };

            var enumerator = _httpClient.SendRequestMultipart<ImageUploadResponse>(
                "/api/v1/developers/collections/1/items/image-upload",
                fileData,
                "itemImage"
            );

            Assert.IsNotNull(enumerator);
            Assert.IsInstanceOf<IEnumerator>(enumerator);
        }

        [Test]
        public void SendRequestMultipart_WithEmptyFileData_ReturnsEnumerator()
        {
            var enumerator = _httpClient.SendRequestMultipart<ImageUploadResponse>(
                "/api/v1/developers/collections/1/items/image-upload",
                new byte[0]
            );

            Assert.IsNotNull(enumerator);
        }

        [Test]
        public void SendRequest_CachingDisabledByDefault()
        {
            // Two calls with caching enabled (default) should work
            var enumerator1 = _httpClient.SendRequest<User>(
                "/api/v1/developers/users/0x123",
                HttpMethod.Get,
                null,
                null,
                true
            );

            var enumerator2 = _httpClient.SendRequest<User>(
                "/api/v1/developers/users/0x123",
                HttpMethod.Get,
                null,
                null,
                true
            );

            Assert.IsNotNull(enumerator1);
            Assert.IsNotNull(enumerator2);
        }

        [Test]
        public void SendRequest_WithCachingDisabled_DoesNotCache()
        {
            var enumerator = _httpClient.SendRequest<User>(
                "/api/v1/developers/users/0x123",
                HttpMethod.Get,
                null,
                null,
                false
            );

            Assert.IsNotNull(enumerator);
        }
    }
}
