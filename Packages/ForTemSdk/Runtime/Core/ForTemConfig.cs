using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Configuration for ForTem SDK client.
    /// </summary>
    public sealed class ForTemConfig
    {
        /// <summary>
        /// ForTem API key for authentication (x-api-key header).
        /// </summary>
        public string ApiKey { get; private set; }

        /// <summary>
        /// Target environment (Testnet or Mainnet).
        /// </summary>
        public ForTemEnvironment Environment { get; private set; }

        /// <summary>
        /// Enable debug logging for API calls.
        /// </summary>
        internal Logger Logger { get; private set; }

        /// <summary>
        /// Creates a new ForTem configuration.
        /// </summary>
        public ForTemConfig(string apiKey, ForTemEnvironment environment = ForTemEnvironment.Mainnet, bool debugLogging = false)
        {
            ApiKey = Ensure.ArgumentNotNullOrEmpty(apiKey);
            Environment = environment;

            Logger = new Logger(Debug.unityLogger.logHandler)
            {
                filterLogType = debugLogging ? LogType.Log : LogType.Error
            };
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
    }
}
