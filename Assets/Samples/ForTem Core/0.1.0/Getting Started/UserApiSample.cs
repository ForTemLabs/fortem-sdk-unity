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

        [Header("Item Image")]
        [SerializeField] private Image _profileImage;
        [SerializeField] private UrlToUIImage _urlToUIImage;

        [Header("ForTem")]
        [SerializeField] private ForTemClientProvider _forTemClientProvider;

        private void Awake()
        {
            _getUserInfoBtn.onClick.AddListener(async () => await GetUserInfo());
        }

        private async Task GetUserInfo()
        {
            try
            {
                var forTemClient = await _forTemClientProvider.GetClient();
                var result = await forTemClient.UserApi.GetUser(_walletAddressInput.text);
                Debug.Log($"User: {JsonUtility.ToJson(result, true)}");
                if (!string.IsNullOrEmpty(result.ProfileImage))
                {
                    _urlToUIImage.SetImageFromUrl(_profileImage, result.ProfileImage);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get user: {ex.Message}");
            }
        }
    }
}
