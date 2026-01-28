using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ForTemSdk
{
    internal sealed class WebRequestHelper
    {
        private readonly IWebRequestSender _webRequestSender;

        public WebRequestHelper(IWebRequestSender webRequestSender, ForTemConfig config)
        {
            _webRequestSender = webRequestSender;
            Config = config;
        }

        public ForTemConfig Config { get; }

        public async Task<T> SendWebRequest<T>(UnityWebRequest request)
        {
            Config.Logger.Log($"[ForTem] Requesting: {request.method} {request.url}");

            var response = await _webRequestSender.Send(request);

            if (response.Result != UnityWebRequest.Result.Success)
            {
                Throw(ref response);
            }

            var fortemResponse = ParseResponse<T>(response.Text);

            Config.Logger.Log($"[ForTem] Response: {response.Text}");

            return fortemResponse;
        }

        private static Exception Throw(ref WebRequestResponse response)
        {
            if (string.IsNullOrEmpty(response.Text))
            {
                throw new HttpRequestException($"HTTP {response.Error} ({response.ResponseCode})");
            }

            try
            {
                var error = JsonUtility.FromJson<ForTemError>(response.Text);
                throw new ForTemApiException(error);
            }
            catch (Exception ex) when (ex is not ForTemApiException)
            {
                throw new HttpRequestException($"HTTP {response.Error} ({response.ResponseCode}). Response: {response.Text}", ex);
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
