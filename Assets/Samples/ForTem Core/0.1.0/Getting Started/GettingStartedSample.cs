using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ForTemSdk.Samples
{
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

    [Serializable]
    public class CreateCollectionUI
    {
        public Button createBtn;
        public TMP_InputField nameInput;
        public TMP_InputField descriptionInput;
    }

    public class GettingStartedSample : MonoBehaviour
    {
        [SerializeField]
        private string _apiKey;

        [SerializeField]
        private Button _authenticateBtn;

        [SerializeField] private TMP_InputField _walletAddressInput;
        [SerializeField] private Button _getUserInfoBtn;

        [SerializeField]
        private Button _getCollectionsBtn;

        [SerializeField]
        private TMP_InputField _collectionIdInput;

        //[SerializeField]
        //private Button _createCollectionBtn;

        //[SerializeField]
        //private TMP_InputField _collectionNameInput;

        //[SerializeField]
        //private TMP_InputField _collectionDescriptionInput;

        [SerializeField] private Button _redeemItemBtn;
        [SerializeField] private TMP_InputField _redeemCodeInput;

        [SerializeField] private CreateCollectionUI _createCollectionUI;
        [SerializeField] private CreateItemUI _createItemUI;

        private ForTemClient _forTemClient;

        private async void Start()
        {
            _getUserInfoBtn.onClick.AddListener(async () => await GetUserInfo());
            _getCollectionsBtn.onClick.AddListener(async () => await GetCollections());
            _createCollectionUI.createBtn.onClick.AddListener(async () => await CreateCollection());
            _redeemItemBtn.onClick.AddListener(async () => await GetItem());
            _createItemUI.createBtn.onClick.AddListener(async () => await CreateItem());

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

        private async Task GetUserInfo()
        {
            try
            {
                var result = await _forTemClient.User.GetUser(_walletAddressInput.text);
                Debug.Log($"User: {JsonUtility.ToJson(result, true)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get user: {ex.Message}");
            }
        }

        private async Task GetCollections()
        {
            try
            {
                var result = await _forTemClient.Collections.GetCollections();
                Debug.Log($"Found {result.Count} collections");
                foreach (var collection in result)
                {
                    Debug.Log($"Collection: {JsonUtility.ToJson(collection, true)}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get collections: {ex.Message}");
            }
        }

        private async Task CreateCollection()
        {
            try
            {
                var requestBody = new CreateCollectionRequest
                {
                    Name = _createCollectionUI.nameInput.text,
                    Description = _createCollectionUI.descriptionInput.text
                };

                var result = await _forTemClient.Collections.CreateCollection(requestBody);
                Debug.Log($"Created collection: {JsonUtility.ToJson(result)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create collection: {ex.Message}");
            }
        }

        private async Task GetItem()
        {
            try
            {
                var collectionId = int.Parse(_collectionIdInput.text);
                var redeemCode = _redeemCodeInput.text;
                var result = await _forTemClient.Collections.GetItem(collectionId, redeemCode);
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

            string imageCid = null;
            if (!string.IsNullOrWhiteSpace(_createItemUI.imagePathInput.text))
            {
                var imageData = System.IO.File.ReadAllBytes(_createItemUI.imagePathInput.text);
                imageCid = await UploadImage(imageData, _createItemUI.imagePathInput.text);
            }

            try
            {
                var collectionId = int.Parse(_collectionIdInput.text);
                var requestBody = new CreateItemRequest
                {
                    Name = _createItemUI.nameInput.text,
                    Description = _createItemUI.descriptionInput.text,
                    RedeemCode = _createItemUI.redeemCodeInput.text,
                    RedeemUrl = null,
                    Quantity = int.Parse(_createItemUI.quantityInput.text),
                    Attributes = attributes,
                    ItemImage = imageCid,
                    RecipientAddress = _createItemUI.recipientAddressInput.text,
                };

                //var result = await _forTemClient.Collections.CreateItemWithImage(collectionId, requestBody, imageData, fileName);
                var result = await _forTemClient.Collections.CreateItem(collectionId, requestBody);
                Debug.Log($"Created item: {JsonUtility.ToJson(result, true)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create item: {ex.Message}");
            }
        }

        private async Task<string> UploadImage(byte[] imageData, string fileName)
        {
            try
            {
                var collectionId = int.Parse(_collectionIdInput.text);
                var result = await _forTemClient.Collections.UploadImage(collectionId, imageData, fileName);
                Debug.Log($"Uploaded image: {JsonUtility.ToJson(result, true)}");
                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to upload image: {ex.Message}");
                throw;
            }
        }
    }
}
