using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class UserApiSample : MonoBehaviour
    {
        [Header("Get User Info")]
        [SerializeField] private TMP_InputField _walletAddressInput;
        [SerializeField] private Button _getUserInfoBtn;

        [Header("ForTem")]
        [SerializeField] private GettingStartedSample _forTemClientProvider;

        private void Awake()
        {
            _getUserInfoBtn.onClick.AddListener(async () => await GetUserInfo());
        }

        private async Task GetUserInfo()
        {
            try
            {
                var forTemClient = await _forTemClientProvider.GetClient();
                var result = await forTemClient.User.GetUser(_walletAddressInput.text);
                Debug.Log($"User: {JsonUtility.ToJson(result, true)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get user: {ex.Message}");
            }
        }
    }
}
