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
        private readonly ForTemClientAsync _client;

        public ForTemHttpClientAsyncUnityWebRequest(ForTemConfig config, ForTemClientAsync client = null)
        {
            _config = config;
            _client = client;
        }

        /// <summary>
        /// Sends an async HTTP request to the ForTem API using UnityWebRequest with await.
        /// Parses the response into the specified type.
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
            string url = $"{_config.GetApiBaseUrl()}{endpoint}";

            using UnityWebRequest request = CreateWebRequest(url, method, body, bodyBytes, customHeaders);

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

                return ParseResponse<T>(responseText);
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
        /// Parses the response into the specified type.
        /// </summary>
        public async Task<T> SendRequestMultipartAsync<T>(
            string endpoint,
            byte[] fileData,
            string fieldName = "file",
            Dictionary<string, string> customHeaders = null)
            where T : class
        {
            string url = $"{_config.GetApiBaseUrl()}{endpoint}";

            // 1. Prepare your form data sections
            var formData = new List<IMultipartFormSection>
            {
                //new MultipartFormDataSection("user_name", "TestUser"),
                // string fileName = "example.png";
                //new MultipartFormFileSection("profile_picture", fileData, fileName, "image/png"),
                new MultipartFormFileSection("file", fileData, "my-image.jpg", "image/jpeg") // Invalid file type. Allowed types: image/jpeg, image/png, image/webp
            };

            using UnityWebRequest request = UnityWebRequest.Post(url, formData);

            // 3. Change the request method from POST to PUT
            request.method = "PUT";

            request.SetRequestHeader("Authorization", $"Bearer {_client?.GetAccessToken() ?? ""}");

            if (_config.DebugLogging)
            {
                Debug.Log($"[ForTem] Uploading file to: {endpoint}");
            }

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseText = request.downloadHandler.text;

                if (_config.DebugLogging)
                {
                    Debug.Log($"[ForTem] Upload response: {responseText}");
                }
                
                return ParseResponse<T>(responseText);
            }
            else
            {
                Debug.LogError($"[ForTem] Upload failed: {request.error}");

                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError($"[ForTem] Response: {request.downloadHandler.text}");
                }

                throw new HttpRequestException($"File upload failed with status code: {request.responseCode}. Error: {request.error}");
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

        private T ParseResponse<T>(string responseText) where T : class
        {
            if (string.IsNullOrEmpty(responseText))
            {
                throw new HttpRequestException("Empty response from server");
            }

            try
            {
                // Parse the standard API response wrapper
                var apiResponse = JsonUtility.FromJson<ApiResponse<T>>(responseText);

                if (apiResponse.statusCode != 200)
                {
                    throw new HttpRequestException($"API returned status code {apiResponse.statusCode}");
                }

                if (apiResponse.data == null)
                {
                    throw new HttpRequestException("API returned null data");
                }

                return apiResponse.data;
            }
            catch (Exception ex) when (!(ex is HttpRequestException))
            {
                throw new HttpRequestException($"Failed to parse response: {ex.Message}", ex);
            }
        }

        private UnityWebRequest CreateWebRequest(
            string url,
            System.Net.Http.HttpMethod method,
            string body,
            byte[] bodyBytes,
            Dictionary<string, string> customHeaders)
        {
            UnityWebRequest request;

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
                if (bodyBytes != null)
                {
                    request = UnityWebRequest.Put(url, bodyBytes);
                }
                else
                {
                    request = string.IsNullOrEmpty(body)
                        ? UnityWebRequest.Put(url, "")
                        : UnityWebRequest.Put(url, Encoding.UTF8.GetBytes(body));
                }
            }
            else
            {
                throw new NotSupportedException($"HTTP method {method.Method} is not supported.");
            }

            //request.downloadHandler = new DownloadHandlerBuffer();
            request.timeout = _config.TimeoutSeconds;

            // Set default headers
            request.SetRequestHeader("Content-Type", "application/json");

            // Add authorization if we have a token
            if (!string.IsNullOrEmpty(_client.GetAccessToken()))
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
