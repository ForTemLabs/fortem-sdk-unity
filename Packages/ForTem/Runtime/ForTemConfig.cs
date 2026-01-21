namespace ForTemSdk
{
    /// <summary>
    /// Configuration for ForTem SDK client.
    /// </summary>
    public sealed class ForTemConfig : IForTemConfig
    {
        /// <summary>
        /// API key for authentication (x-api-key header).
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Target environment (Testnet or Mainnet).
        /// </summary>
        public ForTemEnvironment Environment { get; set; }

        /// <summary>
        /// Enable debug logging for API calls.
        /// </summary>
        public bool DebugLogging { get; set; }

        /// <summary>
        /// Request timeout in seconds.
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Creates a new ForTem configuration.
        /// </summary>
        /// <param name="apiKey">Your ForTem API key</param>
        /// <param name="environment">Target environment</param>
        /// <param name="debugLogging">Enable debug logging</param>
        public ForTemConfig(string apiKey, ForTemEnvironment environment = ForTemEnvironment.Mainnet, bool debugLogging = false)
        {
            ApiKey = apiKey ?? throw new System.ArgumentNullException(nameof(apiKey));
            Environment = environment;
            DebugLogging = debugLogging;
        }

        /// <summary>
        /// Gets the API base URL for the configured environment.
        /// </summary>
        public string GetApiBaseUrl()
        {
            return Environment == ForTemEnvironment.Testnet
                ? "https://testnet-api.fortem.gg"
                : "https://api.fortem.gg";
        }

        /// <summary>
        /// Gets the service URL for the configured environment.
        /// </summary>
        public string GetServiceUrl()
        {
            return Environment == ForTemEnvironment.Testnet
                ? "https://testnet.fortem.gg"
                : "https://fortem.gg";
        }
    }
}
