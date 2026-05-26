using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class RedeemPageSample : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField _redeemCodeInput;
        [SerializeField] private Button _redeemBtn;
        [SerializeField] private ItemDetailsUI _itemDetailsUI;

        [Header("Application")]
        [SerializeField] private ForTemClientProvider _forTemClientProvider;
        [SerializeField] private AppContext _appContext;
        [SerializeField] private GameObject _busyOverlay;

        private void Awake()
        {
            _redeemBtn.onClick.AddListener(async () => await GetItemInfo());
        }

        private async Task GetItemInfo()
        {
            if (_redeemCodeInput == null || string.IsNullOrEmpty(_redeemCodeInput.text))
            {
                return;
            }

            if (_appContext == null || _appContext.SelectedCollectionId == 0)
            {
                Debug.LogError("Select a collection from the Collections tab");
                return;
            }

            try
            {
                _busyOverlay.SetActive(true);
                var forTemClient = await _forTemClientProvider.GetClient();
                var result = await forTemClient.GetItem(_appContext.SelectedCollectionId, _redeemCodeInput.text);
                Debug.Log($"User: {JsonUtility.ToJson(result, true)}");
                _itemDetailsUI.Bind(result);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get item: {ex.Message}");
            }
            finally
            {
                _busyOverlay.SetActive(false);
            }
        }
    }
}
