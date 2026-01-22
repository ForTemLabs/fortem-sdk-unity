using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ForTemSdk
{
    public abstract class ForTemApiBase
    {
        protected readonly ForTemClient _client;

        public ForTemApiBase(ForTemClient client)
        {
            _client = client;
        }

        protected async Task<T> SendWebRequest<T>(UnityWebRequest request)
        {
            if (_client.Config.DebugLogging)
            {
                Debug.Log($"[ForTem] Requesting: {request.method} {request.url}");
            }

            await request.SendWebRequest();

            string responseBody = request.downloadHandler.text;

            if (request.result != UnityWebRequest.Result.Success)
            {
                var errorMsg = string.IsNullOrEmpty(responseBody)
                    ? $"HTTP {request.error} ({request.responseCode})"
                    : responseBody;
                throw new HttpRequestException($"Error creating collection item: {errorMsg}");
            }

            if (_client.Config.DebugLogging)
            {
                Debug.Log($"[ForTem] Response: {responseBody}");
            }

            var response = ParseResponse<ApiResponse<T>>(responseBody);
            return response.data;
        }

        protected T ParseResponse<T>(string responseBody) where T : class
        {
            if (string.IsNullOrEmpty(responseBody))
            {
                throw new HttpRequestException("Empty response from server");
            }

            ApiResponse<T> apiResponse;
            try
            {
                apiResponse = JsonUtility.FromJson<ApiResponse<T>>(responseBody);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Failed to parse json response '{responseBody}': {ex.Message}", ex);
            }

            if (apiResponse.statusCode != 200)
            {
                throw new HttpRequestException($"Server returned status code {apiResponse.statusCode}. data: {apiResponse.data}");
            }

            if (apiResponse.data == null)
            {
                throw new HttpRequestException("Server returned null data (statusCode: 200)");
            }

            return apiResponse.data;
        }
    }
}
