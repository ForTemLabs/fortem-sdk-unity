using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class CollectionsApiSample : MonoBehaviour
    {
        [Header("Create Collection")]
        [SerializeField] private Button _createBtn;
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private TMP_InputField _descriptionInput;
        [SerializeField] private TMP_InputField _websiteInput;

        [Header("Get Collections")]
        [SerializeField] private Button _getCollectionsBtn;
        [SerializeField] private TMP_InputField _collectionIdInput;

        [Header("ForTem")]
        [SerializeField] private ForTemClientProvider _forTemClientProvider;

        private void Awake()
        {
            _getCollectionsBtn.onClick.AddListener(async () => await GetCollections());
            _createBtn.onClick.AddListener(async () => await CreateCollection());
        }

        private async Task GetCollections()
        {
            try
            {
                var forTemClient = await _forTemClientProvider.GetClient();
                var result = await forTemClient.CollectionApi.GetCollections();
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
                    Name = _nameInput.text,
                    Description = _descriptionInput.text,
                    Link = new CollectionLink { Website = _websiteInput.text }
                };

                var forTemClient = await _forTemClientProvider.GetClient();
                var result = await forTemClient.CollectionApi.CreateCollection(requestBody);
                Debug.Log($"Created collection: {JsonUtility.ToJson(result)}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to create collection: {ex.Message}");
            }
        }
    }

    [Serializable]
    public class CreateCollectionUI
    {
        public Button createBtn;
        public TMP_InputField nameInput;
        public TMP_InputField descriptionInput;
    }
}
