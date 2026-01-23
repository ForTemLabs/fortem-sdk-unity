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
            _client = Ensure.ArgumentNotNull(client);
        }

        protected async Task<T> SendWebRequest<T>(UnityWebRequest request)
        {
            _client.Config.Logger.Log($"[ForTem] Requesting: {request.method} {request.url}");

            await request.SendWebRequest();

            string responseBody = request.downloadHandler.text;

            if (request.result != UnityWebRequest.Result.Success)
            {
                var errorMsg = string.IsNullOrEmpty(responseBody)
                    ? $"HTTP {request.error} ({request.responseCode})"
                    : responseBody;
                throw new HttpRequestException($"Error creating collection item: {errorMsg}");
            }

            var response = ParseResponse<ApiResponse<T>>(responseBody);

            _client.Config.Logger.Log($"[ForTem] Response: {responseBody}");

            return response.data;
        }

        protected T ParseResponse<T>(string responseBody) where T : class
        {
            if (string.IsNullOrEmpty(responseBody))
            {
                throw new HttpRequestException("Empty response body from server");
            }

            ApiResponse<T> apiResponse;
            try
            {
                apiResponse = JsonUtility.FromJson<ApiResponse<T>>(responseBody);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Failed to parse response as json: {responseBody}\n{ex.Message}", ex);
            }

            if (apiResponse.statusCode != 200)
            {
                throw new HttpRequestException($"Server returned status code '{apiResponse.statusCode}'. data: {apiResponse.data}");
            }

            if (apiResponse.data == null)
            {
                throw new HttpRequestException("Server returned null data (statusCode: 200)");
            }

            return apiResponse.data;
        }
    }
}
