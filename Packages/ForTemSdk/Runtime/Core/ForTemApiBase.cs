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
                Throw(request, responseBody);
            }

            var response = ParseResponse<T>(responseBody);

            _client.Config.Logger.Log($"[ForTem] Response: {responseBody}");

            return response;
        }

        private static Exception Throw(UnityWebRequest request, string responseBody)
        {
            if (string.IsNullOrEmpty(responseBody))
            {
                throw new HttpRequestException($"HTTP {request.error} ({request.responseCode})");
            }

            try
            {
                var error = JsonUtility.FromJson<ForTemError>(responseBody);
                throw new ForTemApiException(error);
            }
            catch (Exception ex) when (ex is not ForTemApiException)
            {
                throw new HttpRequestException($"HTTP {request.error} ({request.responseCode}). Response: {responseBody}", ex);
            }
        }

        private static T ParseResponse<T>(string responseBody)
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
