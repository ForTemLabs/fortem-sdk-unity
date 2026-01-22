using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Decorator pattern implementation for adding caching to any IForTemHttpClientAsync implementation.
    /// Separates caching concerns from the HTTP transport implementation.
    /// </summary>
    public sealed class ForTemHttpClientCacheDecorator : IForTemHttpClientAsync
    {
        private readonly IForTemHttpClientAsync _innerClient;
        private readonly Dictionary<string, CacheEntry> _responseCache;
        private const int CACHE_DURATION_SECONDS = 300;
        private readonly bool _debugLogging;

        private sealed class CacheEntry
        {
            public object ParsedData { get; set; }
            public DateTime ExpiryTime { get; set; }

            public bool IsExpired => DateTime.UtcNow > ExpiryTime;
        }

        /// <summary>
        /// Creates a new cache decorator wrapping the provided HTTP client.
        /// </summary>
        /// <param name="innerClient">The HTTP client to wrap with caching</param>
        /// <param name="debugLogging">Enable debug logging for cache operations</param>
        public ForTemHttpClientCacheDecorator(IForTemHttpClientAsync innerClient, bool debugLogging = false)
        {
            _innerClient = innerClient ?? throw new ArgumentNullException(nameof(innerClient));
            _responseCache = new Dictionary<string, CacheEntry>();
            _debugLogging = debugLogging;
        }

        /// <summary>
        /// Sends an async HTTP request with caching support for GET requests.
        /// </summary>
        public async Task<T> SendRequestAsync<T>(
            string endpoint,
            System.Net.Http.HttpMethod method,
            string body = null,
            byte[] bodyBytes = null,
            Dictionary<string, string> customHeaders = null,
            bool useCache = true)
            where T : class
        {
            string url = $"{endpoint}"; // Using endpoint as part of cache key
            string cacheKey = $"{method}:{url}";

            // Check cache for GET requests
            if (useCache && method == System.Net.Http.HttpMethod.Get && _responseCache.TryGetValue(cacheKey, out var cached))
            {
                if (!cached.IsExpired)
                {
                    if (_debugLogging)
                    {
                        Debug.Log($"[ForTem] Cache hit: {endpoint}");
                    }

                    return cached.ParsedData as T;
                }
                else
                {
                    _responseCache.Remove(cacheKey);
                }
            }

            // Delegate to inner client
            var response = await _innerClient.SendRequestAsync<T>(endpoint, method, body, bodyBytes, customHeaders, useCache: false);

            // Cache successful GET responses
            if (useCache && method == System.Net.Http.HttpMethod.Get)
            {
                _responseCache[cacheKey] = new CacheEntry
                {
                    ParsedData = response,
                    ExpiryTime = DateTime.UtcNow.AddSeconds(CACHE_DURATION_SECONDS)
                };
            }

            return response;
        }

        /// <summary>
        /// Sends an async multipart form data request for file uploads.
        /// File uploads are not cached.
        /// </summary>
        public async Task<T> SendRequestMultipartAsync<T>(
            string endpoint,
            byte[] fileData,
            string fieldName = "file",
            Dictionary<string, string> customHeaders = null)
            where T : class
        {
            // Delegate to inner client - file uploads are not cached
            return await _innerClient.SendRequestMultipartAsync<T>(endpoint, fileData, fieldName, customHeaders);
        }

        /// <summary>
        /// Clears all cached responses.
        /// </summary>
        public void ClearCache()
        {
            _responseCache.Clear();
            if (_debugLogging)
            {
                Debug.Log("[ForTem] Cache cleared");
            }
        }

        /// <summary>
        /// Disposes the wrapped inner client if it implements IDisposable.
        /// </summary>
        public void Dispose()
        {
            _innerClient?.Dispose();
        }
    }
}

