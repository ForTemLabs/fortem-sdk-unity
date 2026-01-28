using System;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ForTemSdk
{
    internal struct WebRequestResponse
    {
        public string Text;
        public string Error;
        public long ResponseCode;
        public UnityWebRequest.Result Result;
    }
    internal interface IWebRequestSender
    {
        //ForTemConfig Config { get; }
        //Task<string> Authenticate(bool forMinting = false);
        Task<WebRequestResponse> Send(UnityWebRequest request);
    }

    internal sealed class ForTemClientHelper
    {
        private readonly IWebRequestSender _webRequestSender;
        //private readonly AuthApi _authApi;

        public ForTemClientHelper(IWebRequestSender webRequestSender, ForTemConfig config)
        {
            _webRequestSender = Ensure.ArgumentNotNull(webRequestSender);
            //_authApi = authApi;
            Config = Ensure.ArgumentNotNull(config);
        }

        public ForTemConfig Config { get; }

        public async Task<T> SendWebRequest<T>(UnityWebRequest request)
        {
            Config.Logger.Log($"[ForTem] Requesting: {request.method} {request.url}");

            await _webRequestSender.Send(request);

            string responseBody = request.downloadHandler.text;

            if (request.result != UnityWebRequest.Result.Success)
            {
                Throw(request, responseBody);
            }

            var response = ParseResponse<T>(responseBody);

            Config.Logger.Log($"[ForTem] Response: {responseBody}");

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

    internal abstract class ForTemApiBase
    {
        protected readonly IWebRequestSender _webRequestSender;
        protected readonly AuthApi _authApi;

        public ForTemApiBase(IWebRequestSender webRequestSender, AuthApi authApi)
        {
            _webRequestSender = Ensure.ArgumentNotNull(webRequestSender);
            _authApi = authApi;
        }

        protected async Task<T> SendWebRequest<T>(UnityWebRequest request)
        {
            //Config.Logger.Log($"[ForTem] Requesting: {request.method} {request.url}");

            await SendRequest(request);

            string responseBody = request.downloadHandler.text;

            if (request.result != UnityWebRequest.Result.Success)
            {
                Throw(request, responseBody);
            }

            var response = ParseResponse<T>(responseBody);

            //Config.Logger.Log($"[ForTem] Response: {responseBody}");

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

        private Task SendRequest(UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<bool>();
            var operation = request.SendWebRequest();
            operation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}
