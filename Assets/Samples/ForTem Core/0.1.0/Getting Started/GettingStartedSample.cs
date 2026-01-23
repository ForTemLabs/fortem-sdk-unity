using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace ForTemSdk.Samples
{
    public class GettingStartedSample : MonoBehaviour
    {
        [SerializeField]
        private string _apiKey;

        private ForTemClient _forTemClient;

        public Sprite sprite;

        [ContextMenu("SpriteToByteArray")]
        public void SpriteToByteArray()
        {
            var imageData = ImageUtil.SpriteToByteArray(sprite);
            File.WriteAllBytes(Path.Combine(Application.dataPath, "sprite_image.png"), imageData);
        }

        public async ValueTask<ForTemClient> GetClient()
        {
            while (_forTemClient == null)
            {
                await Task.Delay(100);
            }

            return _forTemClient;
        }

        private async void Awake()
        {
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
            }
        }
    }
}
