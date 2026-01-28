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
    }
}
