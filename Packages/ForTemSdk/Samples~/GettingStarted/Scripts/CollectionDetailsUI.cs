using TMPro;
using UnityEngine;

namespace ForTemSdk.Samples
{
    public class CollectionDetailsUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _tradeVolumeLbl;
        [SerializeField] private TextMeshProUGUI _itemCountLbl;
        [SerializeField] private TextMeshProUGUI _createdDateLbl;
        [SerializeField] private TextMeshProUGUI _updatedDateLbl;

        public void Bind(CollectionResponse collectionDetails)
        {
            if (collectionDetails == null)
            {
                _tradeVolumeLbl.text = "";
                _itemCountLbl.text = "";
                _createdDateLbl.text = "";
                _updatedDateLbl.text = "";
                return;
            }

            var createdAt = System.DateTimeOffset.FromUnixTimeMilliseconds(collectionDetails.CreatedAt).LocalDateTime;
            var updatedAt = System.DateTimeOffset.FromUnixTimeMilliseconds(collectionDetails.CreatedAt).LocalDateTime;

            _tradeVolumeLbl.text = collectionDetails.TradeVolume;
            _itemCountLbl.text = collectionDetails.ItemCount.ToString();
            _createdDateLbl.text = createdAt.ToString("yyyy/MM/dd");
            _updatedDateLbl.text = updatedAt.ToString("yyyy/MM/dd");
        }
    }
}
