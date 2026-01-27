using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class CollectionsPageSample : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private RectTransform _listViewContent;
        [SerializeField] private ListItem _listItemPrefab;
        [SerializeField] private Button _getCollectionsBtn;
        [SerializeField] private CollectionDetailsUI _collectionDetailsUI;

        [Header("Application")]
        [SerializeField] private ForTemClientProvider _forTemClientProvider;
        [SerializeField] private AppContext _appContext;
        [SerializeField] private GameObject _busyOverlay;

        private ObjectPool<ListItem> _listItemPool;
        private readonly List<ListItem> _listItems = new();
        private ToggleGroup _toggleGroup;

        private void Awake()
        {
            _getCollectionsBtn.onClick.AddListener(async () => await GetCollections());
            _listItemPool = new ObjectPool<ListItem>(
                createFunc: () => Instantiate(_listItemPrefab, _listViewContent),
                actionOnGet: item => item.gameObject.SetActive(true),
                actionOnRelease: item => item.gameObject.SetActive(false),
                actionOnDestroy: item => Destroy(item.gameObject),
                collectionCheck: true,
                defaultCapacity: 3,
                maxSize: 10);

            _toggleGroup = _listViewContent.GetComponent<ToggleGroup>();
        }

        private async Task GetCollections()
        {
            foreach (var item in _listItems)
            {
                item.SelectionToggle.onValueChanged.RemoveAllListeners();
                _listItemPool.Release(item);
            }

            _listItems.Clear();

            try
            {
                _busyOverlay.SetActive(true);
                var forTemClient = await _forTemClientProvider.GetClient();
                var result = await forTemClient.CollectionApi.GetCollections();
                Debug.Log($"Found {result.Count} collections");
                foreach (var collection in result)
                {
                    Debug.Log($"Collection: {JsonUtility.ToJson(collection, true)}");
                    var listItem = _listItemPool.Get();
                    listItem.Bind(collection.Name, collection.Description, null);
                    _listItems.Add(listItem);
                    listItem.SelectionToggle.group = _toggleGroup;
                    listItem.SelectionToggle.onValueChanged.AddListener(isOn =>
                    {
                        if (isOn)
                        {
                            _collectionDetailsUI.Bind(collection);
                            _appContext.SelectedCollectionId = collection.ID;
                        }
                    });
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to get collections: {ex.Message}");
            }
            finally
            {
                _busyOverlay.SetActive(false);
            }
        }
    }
}
