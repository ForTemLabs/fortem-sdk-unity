using System;
using UnityEngine;

namespace ForTemSdk
{
    [Serializable]
    public sealed class ForTemError
    {
        [SerializeField] private int statusCode;
        [SerializeField] private string path;
        [SerializeField] private string method;
        [SerializeField] private string message;

        public int StatusCode => statusCode;
        public string Path => path;
        public string Method => method;
        public string Message => message;
    }

    public sealed class ForTemApiException : Exception
    {
        public ForTemApiException(ForTemError error)
            : base($"ForTem API Error {error.StatusCode}: {error.Message}")
        {
            Error = error;
        }

        public ForTemError Error { get; }
    }
}
