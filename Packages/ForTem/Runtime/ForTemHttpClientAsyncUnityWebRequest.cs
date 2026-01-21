using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ForTemSdk
{
    /// <summary>
    /// Async HTTP client for ForTem API requests using UnityWebRequest with async/await.
    /// Provides async/await syntax with UnityWebRequest functionality.
    /// This implementation handles only HTTP transport, caching is handled by a decorator.
    /// </summary>
    public sealed class ForTemHttpClientAsyncUnityWebRequest : IForTemHttpClientAsync
    {
        private readonly ForTemConfig _config;
        private readonly ForTemClient _client;

        public ForTemHttpClientAsyncUnityWebRequest(ForTemConfig config, ForTemClient client = null)
        {
            _config = config;
            _client = client;
        }

        /// <summary>
        /// Sends an async HTTP request to the ForTem API using UnityWebRequest with await.
        /// </summary>
        public async Task<string> SendRequestAsync(
            string endpoint,
            System.Net.Http.HttpMethod method,
            string body = null,
            Dictionary<string, string> customHeaders = null,
            bool useCache = true)
        {
            string url = $"{_config.GetApiBaseUrl()}{endpoint}";

            using UnityWebRequest request = CreateWebRequest(url, method, body, customHeaders);

            if (_config.DebugLogging)
            {
                Debug.Log($"[ForTem] Requesting: {method} {url}");
            }

            // Send request and await the result
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Response: {responseText}");
                }

                return responseText;
            }
            else
            {
                string errorMsg = $"[ForTem] Request failed: {request.error}\nURL: {url}\nStatus: {request.responseCode}";
                Debug.LogError(errorMsg);

                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError($"[ForTem] Response: {request.downloadHandler.text}");
                }

                throw new HttpRequestException($"Request failed with status code: {request.responseCode}. Error: {request.error}");
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

            using UnityWebRequest request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

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

            // Add custom headers
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    request.SetRequestHeader(header.Key, header.Value);
                }
            }

            if (_config.DebugLogging)
            {
                Debug.Log($"[ForTem] Uploading file to: {endpoint}");
            }

            // Send request and await the result
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Upload response: {responseText}");
                }

                return responseText;
            }
            else
            {
                Debug.LogError($"[ForTem] Upload failed: {request.error}");
                throw new HttpRequestException($"Upload failed with status code: {request.responseCode}. Error: {request.error}");
            }
        }

        /// <summary>
        /// Clears all cached responses.
        /// </summary>
        public void ClearCache()
        {
            if (_config.DebugLogging)
            {
                Debug.Log("[ForTem] Cache clear requested (no caching in base implementation)");
            }
        }

        /// <summary>
        /// Disposes resources (no-op for this implementation).
        /// </summary>
        public void Dispose()
        {
            // No unmanaged resources to dispose
        }

        private UnityWebRequest CreateWebRequest(
            string url,
            System.Net.Http.HttpMethod method,
            string body,
            Dictionary<string, string> customHeaders)
        {
            UnityWebRequest request;

            // Convert System.Net.Http.HttpMethod to UnityWebRequest method string
            if (method == System.Net.Http.HttpMethod.Get)
            {
                request = UnityWebRequest.Get(url);
            }
            else if (method == System.Net.Http.HttpMethod.Post)
            {
                request = UnityWebRequest.PostWwwForm(url, "");
                if (!string.IsNullOrEmpty(body))
                {
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
                }
            }
            else if (method == System.Net.Http.HttpMethod.Put)
            {
                request = string.IsNullOrEmpty(body)
                    ? UnityWebRequest.Put(url, "")
                    : UnityWebRequest.Put(url, Encoding.UTF8.GetBytes(body));
            }
            else if (method == System.Net.Http.HttpMethod.Delete)
            {
                request = UnityWebRequest.Delete(url);
            }
            else if (method.Method == "PATCH")
            {
                request = new UnityWebRequest(url, "PATCH");
                if (!string.IsNullOrEmpty(body))
                {
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
                }
            }
            else
            {
                throw new NotSupportedException($"HTTP method {method.Method} is not supported.");
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
