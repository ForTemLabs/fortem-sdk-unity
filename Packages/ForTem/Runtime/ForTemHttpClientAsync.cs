using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Async HTTP client for ForTem API requests using C# HttpClient.
    /// Provides an alternative to the coroutine-based implementation.
    /// </summary>
    public sealed class ForTemHttpClientAsync : IForTemHttpClientAsync
    {
        private readonly ForTemConfig _config;
        private readonly ForTemClientAsync _client;
        private readonly HttpClient _httpClient;
        private readonly Dictionary<string, CacheEntry> _responseCache;
        private const int CACHE_DURATION_SECONDS = 300;

        private sealed class CacheEntry
        {
            public string Data { get; set; }
            public DateTime ExpiryTime { get; set; }

            public bool IsExpired => DateTime.UtcNow > ExpiryTime;
        }

        public ForTemHttpClientAsync(ForTemConfig config, ForTemClientAsync client = null)
        {
            _config = config;
            _client = client;
            _responseCache = new Dictionary<string, CacheEntry>();
            
            // Create and configure HttpClient
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
            };
        }

        /// <summary>
        /// Sends an async HTTP request to the ForTem API.
        /// </summary>
        public async Task<string> SendRequestAsync(
            string endpoint,
            System.Net.Http.HttpMethod method,
            string body = null,
            Dictionary<string, string> customHeaders = null,
            bool useCache = true)
        {
            string url = $"{_config.GetApiBaseUrl()}{endpoint}";
            string cacheKey = $"{method}:{url}";

            // Check cache for GET requests
            if (useCache && method == System.Net.Http.HttpMethod.Get && _responseCache.TryGetValue(cacheKey, out var cached))
            {
                if (!cached.IsExpired)
                {
                    if (_config.DebugLogging)
                    {
                        Debug.Log($"[ForTem] Cache hit: {endpoint}");
                    }

                    return cached.Data;
                }
                else
                {
                    _responseCache.Remove(cacheKey);
                }
            }

            try
            {
                using var request = new HttpRequestMessage(method, url);

                // Set body if provided
                if (!string.IsNullOrEmpty(body) && method != System.Net.Http.HttpMethod.Get)
                {
                    request.Content = new StringContent(body, Encoding.UTF8, "application/json");
                }

                // Add custom headers
                if (customHeaders != null)
                {
                    foreach (var header in customHeaders)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                // Add bearer token if available
                var accessToken = _client?.GetAccessToken();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Requesting: {method} {url}");
                }

                var response = await _httpClient.SendAsync(request);
                string responseText = await response.Content.ReadAsStringAsync();

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Response: {responseText}");
                }

                // Cache successful GET responses
                if (useCache && method == System.Net.Http.HttpMethod.Get && response.IsSuccessStatusCode)
                {
                    _responseCache[cacheKey] = new CacheEntry
                    {
                        Data = responseText,
                        ExpiryTime = DateTime.UtcNow.AddSeconds(CACHE_DURATION_SECONDS)
                    };
                }

                return responseText;
            }
            catch (Exception ex)
            {
                string errorMsg = $"[ForTem] Request failed: {ex.Message}\nURL: {url}";
                Debug.LogError(errorMsg);
                throw;
            }
        }

        /// <summary>
        /// Sends an async multipart form data request for file uploads.
        /// </summary>
        public async Task<string> SendRequestMultipartAsync(
            string endpoint,
            byte[] fileData,
            string fieldName = "file",
            Dictionary<string, string> customHeaders = null)
        {
            string url = $"{_config.GetApiBaseUrl()}{endpoint}";

            try
            {
                using var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, url);
                using var content = new MultipartFormDataContent();
                var fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                content.Add(fileContent, fieldName, "image.png");

                request.Content = content;

                // Add custom headers
                if (customHeaders != null)
                {
                    foreach (var header in customHeaders)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                // Add bearer token if available
                var accessToken = _client?.GetAccessToken();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Uploading file to: {endpoint}");
                }

                var response = await _httpClient.SendAsync(request);
                string responseText = await response.Content.ReadAsStringAsync();

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Upload response: {responseText}");
                }

                return responseText;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ForTem] Upload failed: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Clears all cached responses.
        /// </summary>
        public void ClearCache()
        {
            _responseCache.Clear();
            if (_config.DebugLogging)
            {
                Debug.Log("[ForTem] Cache cleared");
            }
        }

        /// <summary>
        /// Disposes the HttpClient.
        /// </summary>
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
