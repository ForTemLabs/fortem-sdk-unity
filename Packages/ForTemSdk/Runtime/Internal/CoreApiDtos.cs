using System;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Standard response wrapper for all ForTem API requests.
    /// </summary>
    [Serializable]
    internal class ApiResponse<T>
    {
        public int statusCode;
        public T data;
        public ResponseMetadata metadata;
    }

    /// <summary>
    /// Metadata for API responses.
    /// </summary>
    [Serializable]
    internal class ResponseMetadata
    {
        public Pagination pagination;
    }

    /// <summary>
    /// Pagination details for API responses.
    /// </summary>
    [Serializable]
    internal class Pagination
    {
        public int totalItems;
    }

    // {"statusCode":400,"timestamp":"2026-01-24T13:27:24.667Z","path":"/api/v1/developers/auth/access-token","method":"POST","message":"nonce should not be empty"}
    [Serializable]
    public sealed class ForTemError
    {
        [SerializeField] private int statusCode;
        [SerializeField] private string timestamp;
        [SerializeField] private string path;
        [SerializeField] private string method;
        [SerializeField] private string message;

        public int StatusCode => statusCode;
        public string Timestamp => timestamp;
        public string Path => path;
        public string Method => method;
        public string Message => message;
    }

    public sealed class ForTemException : Exception
    {
        public ForTemException(ForTemError error)
            : base($"ForTem API Error {error.StatusCode}: {error.Message}")
        {
            Error = error;
        }

        public ForTemError Error { get; }
    }
}
