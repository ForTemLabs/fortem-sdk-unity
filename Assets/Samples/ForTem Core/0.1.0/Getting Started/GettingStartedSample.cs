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

    /// <summary>
    /// Advanced example showing error handling and task composition with async/await.
    /// </summary>
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

        private ForTemClientAsync _forTemClient;

        private void Start()
        {
            _authenticateBtn.onClick.AddListener(async () => await Authenticate());
            _getUserInfoBtn.onClick.AddListener(async () => await GetUserInfo());
            _getCollectionsBtn.onClick.AddListener(async () => await GetCollections());
            _createCollectionUI.createBtn.onClick.AddListener(async () => await CreateCollection());
            _redeemItemBtn.onClick.AddListener(async () => await RedeemItem());
            _createItemUI.createBtn.onClick.AddListener(async () => await CreateItem());

            var config = new ForTemConfig(
                apiKey: _apiKey,
                environment: ForTemEnvironment.Testnet,
                debugLogging: true
            );

            _forTemClient = new ForTemClientAsync(config);
        }

        private async Task Authenticate()
        {
            try
            {
                // Step 1: Get nonce
                var nonceResult = await _forTemClient.Auth.GetNonceAsync();
                Debug.Log($"Got nonce: {nonceResult.nonce}");
                
                // Step 2: Exchange nonce for token
                var tokenResult = await _forTemClient.Auth.GetAccessTokenAsync(nonceResult.nonce);
                Debug.Log($"Got Access Token: {tokenResult.AccessToken}");
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        private async Task GetUserInfo()
        {
            try
            {
                var result = await _forTemClient.User.GetUserAsync(_walletAddressInput.text);
                Debug.Log($"User: {JsonUtility.ToJson(result, true)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get user: {ex}");
            }
        }

        private async Task GetCollections()
        {
            try
            {
                var result = await _forTemClient.Collections.GetCollectionsAsync();
                Debug.Log($"Found {result.Count} collections");
                foreach (var collection in result)
                {
                    Debug.Log($"Collection: {JsonUtility.ToJson(collection, true)}");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get collections: {ex}");
            }
        }

        private async Task CreateCollection()
        {
            try
            {
                var result = await _forTemClient.Collections.CreateCollectionAsync(
                    _createCollectionUI.nameInput.text,
                    _createCollectionUI.descriptionInput.text
                );
                Debug.Log($"Created collection: {JsonUtility.ToJson(result)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create collection: {ex}");
            }
        }

        private async Task RedeemItem()
        {
            try
            {
                var collectionId = int.Parse(_collectionIdInput.text);
                var redeemCode = _redeemCodeInput.text;
                var result = await _forTemClient.Collections.GetCollectionItemAsync(collectionId, redeemCode);
                Debug.Log($"Collection Item: {JsonUtility.ToJson(result, true)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get collection item: {ex}");
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

            ImageUploadResponse imageUploadResult = null;
            if (!string.IsNullOrWhiteSpace(_createItemUI.imagePathInput.text))
            {
                var imageBytes = System.IO.File.ReadAllBytes(_createItemUI.imagePathInput.text);
                imageUploadResult = await UploadImage(imageBytes);
            }

            try
            {
                var collectionId = int.Parse(_collectionIdInput.text);
                var result = await _forTemClient.Collections.CreateCollectionItemAsync(
                    collectionId: collectionId,
                    name: _createItemUI.nameInput.text,
                    quantity: int.Parse(_createItemUI.quantityInput.text),
                    redeemCode: _createItemUI.redeemCodeInput.text,
                    description: _createItemUI.descriptionInput.text,
                    attributes: attributes,
                    itemImageCid: imageUploadResult?.ItemImage,
                    recipientAddress: _createItemUI.recipientAddressInput.text
                );

                Debug.Log($"Created item: {JsonUtility.ToJson(result, true)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create item: {ex}");
            }
        }

        private async Task<ImageUploadResponse> UploadImage(byte[] imageData)
        {
            try
            {
                var collectionId = int.Parse(_collectionIdInput.text);
                var result = await _forTemClient.Collections.UploadImageAsync(collectionId, imageData);
                Debug.Log($"Uploaded image URL: {JsonUtility.ToJson(result, true)}");
                return result;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to upload image: {ex}");
                throw;
            }
        }

        private void OnDestroy()
        {
            // Clean up resources
            _forTemClient?.Dispose();
        }
    }
}
