using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class MintPageSample : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform _listViewContent;
        [SerializeField] private Button _mintItemBtn;

        [Header("Details UI")]
        [SerializeField] private TextMeshProUGUI _nameLbl;
        [SerializeField] private TextMeshProUGUI _quantity;
        [SerializeField] private Image _iconImg;

        [Header("Application")]
        [SerializeField] private ForTemClientProvider _forTemClientProvider;
        [SerializeField] private AppContext _appContext;
        [SerializeField] private GameObject _busyOverlay;

        private List<InventoryItem> _listItems;
        private ToggleGroup _toggleGroup;

        private void Awake()
        {
            _mintItemBtn.onClick.AddListener(async () => await MintItem());
            _toggleGroup = _listViewContent.GetComponent<ToggleGroup>();
        }

        private void Start()
        {
            _listItems = _listViewContent
                .GetComponentsInChildren<InventoryItem>(false)
                .ToList();

            foreach (InventoryItem item in _listItems)
            {
                item.Toggle.onValueChanged.AddListener(isOn =>
                {
                    if (!_toggleGroup.AnyTogglesOn())
                    {
                        _nameLbl.text = "";
                        _quantity.text = "";
                        _iconImg.sprite = null;
                        return;
                    }
                    if (isOn)
                    {
                        _nameLbl.text = item.Name;
                        _quantity.text = item.Quantity.ToString();
                        _iconImg.sprite = item.Sprite;
                    }
                });
            }
        }

        private async Task MintItem()
        {
            if (_appContext == null || _appContext.SelectedCollectionId == 0)
            {
                Debug.LogError("Select a collection from the Collections tab");
                return;
            }
            if (string.IsNullOrEmpty(_appContext.CurrentUser?.WalletAddress))
            {
                Debug.LogError("Need to connect your wallet first.");
                return;
            }

            InventoryItem selectedItem = _listItems
                .FirstOrDefault(x => x.gameObject.activeSelf == true && x.Toggle.isOn);

            if (selectedItem == null)
            {
                return;
            }

            string fileName = $"{selectedItem.Sprite.name}.png";
            byte[] imageData = ImageUtil.SpriteToByteArray(selectedItem.Sprite);

            try
            {
                var requestBody = new CreateItemRequest
                {
                    Name = selectedItem.Name,
                    Description = selectedItem.Description,
                    RedeemCode = selectedItem.RedeemCode,
                    RedeemUrl = null,
                    Quantity = selectedItem.Quantity,
                    Attributes = null,
                    RecipientAddress = _appContext.CurrentUser.WalletAddress,
                };

                _busyOverlay.SetActive(true);
                var forTemClient = await _forTemClientProvider.GetClient();
                var result = await forTemClient.CreateItemWithImage(_appContext.SelectedCollectionId, requestBody, imageData, fileName);
                Debug.Log($"Created item: {JsonUtility.ToJson(result, true)}");

                selectedItem.MarkAsRedeemed();

            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create item: {ex.Message}");
            }
            finally
            {
                _busyOverlay.SetActive(false);
            }
        }
    }
}
