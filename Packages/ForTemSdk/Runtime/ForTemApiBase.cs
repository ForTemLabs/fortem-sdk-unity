using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
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
                // TODO: Custom exception
                throw new HttpRequestException(errorMsg);
            }

            var response = ParseResponse<T>(responseBody);

            _client.Config.Logger.Log($"[ForTem] Response: {responseBody}");

            return response;
        }

        private T ParseResponse<T>(string responseBody)
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

#if !UNITY_2023_1_OR_NEWER
    public static class UnityWebRequestAsyncOperationExtensions
    {
        public static UnityWebRequestAsyncOperationAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAsyncOperationAwaiter(asyncOp);
        }
    }

    public struct UnityWebRequestAsyncOperationAwaiter : INotifyCompletion
    {
        private readonly UnityWebRequestAsyncOperation _asyncOp;
        private Action _continuation;

        public UnityWebRequestAsyncOperationAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            _continuation = null;
            _asyncOp = asyncOp;
            _asyncOp.completed += OnRequestCompleted;
        }

        public bool IsCompleted => _asyncOp.isDone;

        public void OnCompleted(Action continuation)
        {
            _continuation = continuation;
        }

        private void OnRequestCompleted(UnityEngine.AsyncOperation op)
        {
            _continuation?.Invoke();
        }

        public UnityWebRequest GetResult()
        {
            return _asyncOp.webRequest;
        }
    }
#endif
}
