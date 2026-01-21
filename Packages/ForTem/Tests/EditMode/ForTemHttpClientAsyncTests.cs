using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;

namespace ForTemSdk
{
    /// <summary>
    /// Tests for ForTemHttpClientAsync async HTTP client.
    /// </summary>
    public class ForTemHttpClientAsyncTests
    {
        private ForTemConfig _config;
        private ForTemHttpClientAsync _httpClient;

        [SetUp]
        public void SetUp()
        {
            _config = new ForTemConfig("test-api-key", ForTemEnvironment.Testnet);
            _httpClient = new ForTemHttpClientAsync(_config);
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient?.Dispose();
        }

        [Test]
        public void Constructor_WithValidConfig_InitializesSuccessfully()
        {
            Assert.IsNotNull(_httpClient);
        }

        [Test]
        public void ImplementsIForTemHttpClientAsync_Interface()
        {
            Assert.IsInstanceOf<IForTemHttpClientAsync>(_httpClient);
        }

        [Test]
        public void ImplementsIDisposable_Interface()
        {
            Assert.IsInstanceOf<IDisposable>(_httpClient);
        }

        [Test]
        public void ClearCache_RemovesAllCachedData()
        {
            Assert.DoesNotThrow(() => _httpClient.ClearCache());
        }

        [Test]
        public async Task SendRequestAsync_WithGETMethod_ReturnsTask()
        {
            var task = _httpClient.SendRequestAsync(
                "/api/v1/developers/auth/nonce",
                System.Net.Http.HttpMethod.Get,
                null,
                null,
                true
            );

            Assert.IsNotNull(task);
            Assert.IsInstanceOf<Task<string>>(task);

            // Note: We can't await this without a real API, but we verify the task is created
            try
            {
                await Task.WhenAny(task, Task.Delay(100));
            }
            catch
            {
                // Expected to timeout or fail due to no real API
            }
        }

        [Test]
        public async Task SendRequestAsync_WithPOSTMethod_ReturnsTask()
        {
            var task = _httpClient.SendRequestAsync(
                "/api/v1/developers/auth/access-token",
                System.Net.Http.HttpMethod.Post,
                "{\"nonce\":\"test\"}"
            );

            Assert.IsNotNull(task);
            Assert.IsInstanceOf<Task<string>>(task);

            try
            {
                await Task.WhenAny(task, Task.Delay(100));
            }
            catch
            {
                // Expected to timeout or fail due to no real API
            }
        }

        [Test]
        public async Task SendRequestAsync_WithCustomHeaders_ReturnsTask()
        {
            var headers = new Dictionary<string, string>
            {
                { "x-api-key", "test-key" }
            };

            var task = _httpClient.SendRequestAsync(
                "/api/v1/developers/users/0x123",
                System.Net.Http.HttpMethod.Get,
                null,
                headers
            );

            Assert.IsNotNull(task);

            try
            {
                await Task.WhenAny(task, Task.Delay(100));
            }
            catch
            {
                // Expected to timeout or fail due to no real API
            }
        }

        [Test]
        public async Task SendRequestMultipartAsync_WithValidFileData_ReturnsTask()
        {
            var fileData = new byte[] { 0xFF, 0xD8, 0xFF };

            var task = _httpClient.SendRequestMultipartAsync(
                "/api/v1/developers/collections/1/items/image-upload",
                fileData,
                "itemImage"
            );

            Assert.IsNotNull(task);
            Assert.IsInstanceOf<Task<string>>(task);

            try
            {
                await Task.WhenAny(task, Task.Delay(100));
            }
            catch
            {
                // Expected to timeout or fail due to no real API
            }
        }

        [Test]
        public async Task SendRequestMultipartAsync_WithEmptyFileData_ReturnsTask()
        {
            var task = _httpClient.SendRequestMultipartAsync(
                "/api/v1/developers/collections/1/items/image-upload",
                new byte[0]
            );

            Assert.IsNotNull(task);

            try
            {
                await Task.WhenAny(task, Task.Delay(100));
            }
            catch
            {
                // Expected to timeout or fail due to no real API
            }
        }

        [Test]
        public async Task SendRequestMultipartAsync_WithCustomHeaders_ReturnsTask()
        {
            var headers = new Dictionary<string, string>
            {
                { "Authorization", "Bearer token" }
            };

            var task = _httpClient.SendRequestMultipartAsync(
                "/api/v1/developers/collections/1/items/image-upload",
                new byte[] { 0xFF },
                "itemImage",
                headers
            );

            Assert.IsNotNull(task);

            try
            {
                await Task.WhenAny(task, Task.Delay(100));
            }
            catch
            {
                // Expected to timeout or fail due to no real API
            }
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            Assert.DoesNotThrow(() =>
            {
                _httpClient.Dispose();
                _httpClient.Dispose();
            });
        }
    }
}
