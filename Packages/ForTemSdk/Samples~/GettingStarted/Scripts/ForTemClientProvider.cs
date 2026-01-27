using System.Threading.Tasks;
using UnityEngine;

namespace ForTemSdk.Samples
{
    public class ForTemClientProvider : MonoBehaviour
    {
        [SerializeField] private string _apiKey;

        private ForTemClient _forTemClient;
        private bool _hasError;

        public async ValueTask<ForTemClient> GetClient()
        {
            while (_forTemClient == null && !_hasError)
            {
                await Task.Delay(100);
            }

            return _forTemClient;
        }

        private async void Awake()
        {
            if (string.IsNullOrEmpty(_apiKey))
            {
                Debug.LogError("Assign API key in the inspector");
                _hasError = true;
                return;
            }

            await InitializeForTem();
        }

        private async Task InitializeForTem()
        {
            await GetApiKey();

            var config = new ForTemConfig(
                apiKey: _apiKey,
                environment: ForTemEnvironment.Testnet,
                debugLogging: true
            );

            _forTemClient = new ForTemClient(config);
        }

        private async Task GetApiKey()
        {
            try
            {
                // Simulate fetching API key from backend
                await Task.Delay(1000);
                Debug.Log("Fetched API key from backend");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to fetch API key: {ex.Message}");
                _hasError = true;
            }
        }
    }
}
