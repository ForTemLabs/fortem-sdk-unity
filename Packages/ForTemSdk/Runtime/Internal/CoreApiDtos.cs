using System;

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
}
