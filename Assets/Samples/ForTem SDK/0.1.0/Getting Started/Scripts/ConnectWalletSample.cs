using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class ConnectWalletSample : MonoBehaviour
    {
        [Header("Request")]
        [SerializeField] private TMP_InputField _walletAddressInput;
        [SerializeField] private Button _connectBtn;

        [Header("Application")]
        [SerializeField] private ForTemClientProvider _forTemClientProvider;
        [SerializeField] private AppContext _appContext;
        [SerializeField] private GameObject _busyOverlay;

        private void Awake()
        {
            _connectBtn.onClick.AddListener(async () => await GetUserInfo());
        }

        private async Task GetUserInfo()
        {
            try
            {
                _busyOverlay.SetActive(true);
                var forTemClient = await _forTemClientProvider.GetClient();
                var result = await forTemClient.UserApi.GetUser(_walletAddressInput.text);
                Debug.Log($"User: {JsonUtility.ToJson(result, true)}");
                _appContext.CurrentUser = result;
                gameObject.SetActive(false);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get user: {ex.Message}");
            }
            finally
            {
                _busyOverlay.SetActive(false);
            }
        }
    }
}
