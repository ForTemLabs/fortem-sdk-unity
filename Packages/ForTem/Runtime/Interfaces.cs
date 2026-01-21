using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ForTemSdk
{
    /// <summary>
    /// Interface for HTTP client operations in coroutine-based API.
    /// </summary>
    public interface IForTemHttpClient
    {
        /// <summary>
        /// Sends an HTTP request to the ForTem API.
        /// </summary>
        IEnumerator SendRequest<T>(
            string endpoint,
            HttpMethod method,
            string body = null,
            Dictionary<string, string> customHeaders = null,
            bool useCache = true)
            where T : class;

        /// <summary>
        /// Sends a multipart form data request for file uploads.
        /// </summary>
        IEnumerator SendRequestMultipart<T>(
            string endpoint,
            byte[] fileData,
            string fieldName = "file")
            where T : class;

        /// <summary>
        /// Clears all cached responses.
        /// </summary>
        void ClearCache();
    }

    /// <summary>
    /// Interface for async HTTP client operations.
    /// </summary>
    public interface IForTemHttpClientAsync : IDisposable
    {
        /// <summary>
        /// Sends an async HTTP request to the ForTem API.
        /// </summary>
        Task<string> SendRequestAsync(
            string endpoint,
            System.Net.Http.HttpMethod method,
            string body = null,
            Dictionary<string, string> customHeaders = null,
            bool useCache = true);

        /// <summary>
        /// Sends an async multipart form data request for file uploads.
        /// </summary>
        Task<string> SendRequestMultipartAsync(
            string endpoint,
            byte[] fileData,
            string fieldName = "file",
            Dictionary<string, string> customHeaders = null);

        /// <summary>
        /// Clears all cached responses.
        /// </summary>
        void ClearCache();
    }

    /// <summary>
    /// Interface for SDK configuration.
    /// </summary>
    public interface IForTemConfig
    {
        /// <summary>
        /// API key for authentication (x-api-key header).
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Target environment (Testnet or Mainnet).
        /// </summary>
        ForTemEnvironment Environment { get; }

        /// <summary>
        /// Enable debug logging for API calls.
        /// </summary>
        bool DebugLogging { get; }

        /// <summary>
        /// Request timeout in seconds.
        /// </summary>
        int TimeoutSeconds { get; }

        /// <summary>
        /// Gets the API base URL for the configured environment.
        /// </summary>
        string GetApiBaseUrl();

        /// <summary>
        /// Gets the service URL for the configured environment.
        /// </summary>
        string GetServiceUrl();
    }
}
