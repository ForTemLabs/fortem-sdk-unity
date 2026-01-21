using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;

namespace ForTemSdk
{

    /// <summary>
    /// HTTP client for ForTem API requests with built-in retry logic and caching.
    /// </summary>
    public sealed class ForTemHttpClient : IForTemHttpClient
    {
        private readonly ForTemConfig _config;
        private readonly ForTemClient _client;
        private readonly Dictionary<string, CacheEntry> _responseCache;
        private const int CACHE_DURATION_SECONDS = 300;

        private sealed class CacheEntry
        {
            public string Data { get; set; }
            public DateTime ExpiryTime { get; set; }

            public bool IsExpired => DateTime.UtcNow > ExpiryTime;
        }

        public ForTemHttpClient(ForTemConfig config, ForTemClient client = null)
        {
            _config = config;
            _client = client;
            _responseCache = new Dictionary<string, CacheEntry>();
        }

        /// <summary>
        /// Sends an HTTP request to the ForTem API.
        /// </summary>
        public IEnumerator SendRequest<T>(
            string endpoint,
            HttpMethod method,
            string body = null,
            Dictionary<string, string> customHeaders = null,
            bool useCache = true)
            where T : class
        {
            string url = $"{_config.GetApiBaseUrl()}{endpoint}";
            string cacheKey = $"{method}:{url}";

            // Check cache for GET requests
            if (useCache && method == HttpMethod.Get && _responseCache.TryGetValue(cacheKey, out var cached))
            {
                if (!cached.IsExpired)
                {
                    if (_config.DebugLogging)
                    {
                        Debug.Log($"[ForTem] Cache hit: {endpoint}");
                    }

                    yield return cached.Data;
                    yield break;
                }
                else
                {
                    _responseCache.Remove(cacheKey);
                }
            }

            using UnityWebRequest request = CreateWebRequest(url, method, body, customHeaders);

            if (_config.DebugLogging)
            {
                Debug.Log($"[ForTem] Requesting: {method} {url}");
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                // Cache successful GET responses
                if (useCache && method == HttpMethod.Get)
                {
                    _responseCache[cacheKey] = new CacheEntry
                    {
                        Data = responseText,
                        ExpiryTime = DateTime.UtcNow.AddSeconds(CACHE_DURATION_SECONDS)
                    };
                }

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Response: {responseText}");
                }

                yield return responseText;
            }
            else
            {
                string errorMsg = $"[ForTem] Request failed: {request.error}\nURL: {url}\nStatus: {request.responseCode}";
                Debug.LogError(errorMsg);

                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError($"[ForTem] Response: {request.downloadHandler.text}");
                }

                yield return null;
            }
        }

        /// <summary>
        /// Sends a multipart form data request for file uploads.
        /// </summary>
        public IEnumerator SendRequestMultipart<T>(
            string endpoint,
            byte[] fileData,
            string fieldName = "file")
            where T : class
        {
            string url = $"{_config.GetApiBaseUrl()}{endpoint}";

            using (UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
            {
                // Create multipart form data
                byte[] boundary = Encoding.ASCII.GetBytes("----WebKitFormBoundary7MA4YWxkTrZu0gW");
                byte[] boundaryDashes = Encoding.ASCII.GetBytes("--");
                byte[] crlf = Encoding.ASCII.GetBytes("\r\n");

                var formData = new List<byte>();

                // Add boundary and field headers
                formData.AddRange(boundaryDashes);
                formData.AddRange(boundary);
                formData.AddRange(crlf);
                formData.AddRange(Encoding.ASCII.GetBytes($"Content-Disposition: form-data; name=\"{fieldName}\"; filename=\"image.png\"\r\n"));
                formData.AddRange(Encoding.ASCII.GetBytes("Content-Type: image/png\r\n\r\n"));

                // Add file data
                formData.AddRange(fileData);
                formData.AddRange(crlf);

                // Add closing boundary
                formData.AddRange(boundaryDashes);
                formData.AddRange(boundary);
                formData.AddRange(boundaryDashes);
                formData.AddRange(crlf);

                request.uploadHandler = new UploadHandlerRaw(formData.ToArray());
                request.downloadHandler = new DownloadHandlerBuffer();
                request.timeout = _config.TimeoutSeconds;

                // Set headers
                request.SetRequestHeader("Authorization", $"Bearer {_client?.GetAccessToken() ?? ""}");
                request.SetRequestHeader("Content-Type", $"multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Uploading file to: {endpoint}");
                }

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseText = request.downloadHandler.text;

                    if (_config.DebugLogging)
                    {
                        Debug.Log($"[ForTem] Upload response: {responseText}");
                    }

                    yield return responseText;
                }
                else
                {
                    Debug.LogError($"[ForTem] Upload failed: {request.error}");
                    yield return null;
                }
            }
        }

        /// <summary>
        /// Sends a multipart form data request for file uploads.
        /// </summary>
        public IEnumerator SendRequestMultipart2<T>(
            string endpoint,
            byte[] fileData,
            string fieldName = "file")
            where T : class
        {
            string url = $"{_config.GetApiBaseUrl()}{endpoint}";

            // 1. Prepare your form data sections
            var formData = new List<IMultipartFormSection>
            {
                // Add a standard text field
                new MultipartFormDataSection("user_name", "TestUser"),
                // Add a file section (e.g., an image file converted to bytes)
                // Replace 'fileBytes' and 'fileName' with actual data and name
                // byte[] fileBytes = ...;
                // string fileName = "example.png";
                // new MultipartFormFileSection("profile_picture", fileBytes, fileName, "image/png")
            };

            // 2. Manually generate the multipart form data payload
            // Unity's internal utility can be mimicked or a custom solution used.
            // A common approach is to use the existing UnityWebRequest.Post setup internally 
            // to handle the serialization of the form sections, then change the method to PUT.

            // The below line is a common pattern to leverage Unity's built-in form serialization
            // although it uses the 'Post' function name, it is a setup utility, and we change the method later.
            using UnityWebRequest request = UnityWebRequest.Post(url, formData);

            // 3. Change the request method from POST to PUT
            request.method = "PUT"; // This is the key step

            // Set headers
            request.SetRequestHeader("Authorization", $"Bearer {_client?.GetAccessToken() ?? ""}");

            if (_config.DebugLogging)
            {
                Debug.Log($"[ForTem] Uploading file to: {endpoint}");
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Upload response: {responseText}");
                }

                yield return responseText;
            }
            else
            {
                Debug.LogError($"[ForTem] Upload failed: {request.error}");
                yield return null;
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

        private UnityWebRequest CreateWebRequest(
            string url,
            HttpMethod method,
            string body,
            Dictionary<string, string> customHeaders)
        {
            UnityWebRequest request;

            switch (method)
            {
                case HttpMethod.Get:
                    request = UnityWebRequest.Get(url);
                    break;
                case HttpMethod.Post:
                    request = UnityWebRequest.PostWwwForm(url, "");
                    if (!string.IsNullOrEmpty(body))
                    {
                        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
                    }

                    break;
                case HttpMethod.Put:
                    request = string.IsNullOrEmpty(body)
                        ? UnityWebRequest.Put(url, "")
                        : UnityWebRequest.Put(url, Encoding.UTF8.GetBytes(body));
                    break;
                default:
                    throw new NotSupportedException($"HTTP method {method} is not supported.");
            }

            request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = _config.TimeoutSeconds;

            // Set default headers
            request.SetRequestHeader("Content-Type", "application/json");

            // Add authorization if we have a token
            if (_client != null && !string.IsNullOrEmpty(_client.GetAccessToken()))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_client.GetAccessToken()}");
            }

            // Add custom headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            return request;
        }
    }
}
