using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class ItemsApiSample : MonoBehaviour
    {
        [Header("Create Item")]
        [SerializeField] private Button _createItemBtn;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_InputField _descriptionInput;
        [SerializeField] private TMP_InputField _redeemCodeInput;
        [SerializeField] private TMP_InputField _recipientAddressInput;
        [SerializeField] private TMP_InputField _quantityInput;
        [SerializeField] private TMP_InputField _collectionIdInput;
        [SerializeField] private TMP_InputField _imagePathInput;

        [Header("Get Item")]
        [SerializeField] private Button _getItemBtn;
        [SerializeField] private TMP_InputField _getItemCollectionIdInput;
        [SerializeField] private TMP_InputField _getItemRedeemCodeInput;

        [Header("ForTem")]
        [SerializeField] private ForTemClientProvider _forTemClientProvider;

        private void Awake()
        {
            _getItemBtn.onClick.AddListener(async () => await GetItem());
            _createItemBtn.onClick.AddListener(async () => await CreateItem());
        }

        private async Task GetItem()
        {
            try
            {
                var forTemClient = await _forTemClientProvider.GetClient();
                var collectionId = int.Parse(_getItemCollectionIdInput.text);
                var redeemCode = _getItemRedeemCodeInput.text;
                var result = await forTemClient.ItemApi.GetItem(collectionId, redeemCode);
                Debug.Log($"Retrieved Item: {JsonUtility.ToJson(result, true)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get collection item: {ex.Message}");
            }
        }

        private async Task CreateItem()
        {
            var attributes = new List<ItemAttribute>
            {
                new ItemAttribute { Name = "Rarity", Value = "Common" },
                //new ItemAttribute { Name = "Rarity", Value = "Epic" },
                //new ItemAttribute { Name = "Element", Value = "Fire" }
            };

            string fileName = null;
            byte[] imageData = null;
            if (!string.IsNullOrWhiteSpace(_imagePathInput.text))
            {
                // Load image from local file path and pass it to the image util to reduce dimensions if bigger than 256x256.
                imageData = System.IO.File.ReadAllBytes(_imagePathInput.text);
                var tex = new Texture2D(2, 2);
                tex.LoadImage(imageData);
                fileName = System.IO.Path.GetFileName(_imagePathInput.text);
                imageData = ImageUtil.TextureToByteArray(tex);
            }

            try
            {
                var collectionId = int.Parse(_collectionIdInput.text);
                var requestBody = new CreateItemRequest
                {
                    Name = _nameInput.text,
                    Description = _descriptionInput.text,
                    RedeemCode = _redeemCodeInput.text,
                    RedeemUrl = null,
                    Quantity = int.Parse(_quantityInput.text),
                    Attributes = attributes,
                    RecipientAddress = _recipientAddressInput.text,
                };

                var forTemClient = await _forTemClientProvider.GetClient();
                var result = await forTemClient.ItemApi.CreateItemWithImage(collectionId, requestBody, imageData, fileName);
                Debug.Log($"Created item: {JsonUtility.ToJson(result, true)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create item: {ex.Message}");
            }
        }
    }

    [Serializable]
    public class CreateItemUI
    {
        public Button createBtn;
        public TMP_InputField nameInput;
        public TMP_InputField descriptionInput;
        public TMP_InputField redeemCodeInput;
        public TMP_InputField recipientAddressInput;
        public TMP_InputField quantityInput;
        public TMP_InputField imagePathInput;
    }
}
