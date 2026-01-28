#if !UNITY_2023_1_OR_NEWER
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Networking;

namespace ForTemSdk
{
    /// <summary>
    /// Unity 2023.1 introduced support for a simplified asynchronous programming model using C# async and await keywords.<br/>
    /// This allows `await request.SendWebRequest()`
    /// </summary>
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
}
#endif
