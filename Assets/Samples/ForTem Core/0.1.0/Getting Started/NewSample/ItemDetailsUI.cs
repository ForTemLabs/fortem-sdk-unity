using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class ItemDetailsUI : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI _nameLbl;
        [SerializeField] private TextMeshProUGUI _quantity;
        [SerializeField] private TextMeshProUGUI _nftNumLbl;
        [SerializeField] private TextMeshProUGUI _ownerLbl;
        [SerializeField] private TextMeshProUGUI _createdDateLbl;
        [SerializeField] private TextMeshProUGUI _statusLbl;
        [SerializeField] private Image _iconImg;
        [SerializeField] private UrlToUIImage _urlToUIImage;

        public void Bind(GetItemResponse itemInfo)
        {
            if (itemInfo == null)
            {
                _nameLbl.text = "";
                _quantity.text = "";
                _nftNumLbl.text = "";
                _ownerLbl.text = "";
                _createdDateLbl.text = "";
                _statusLbl.text = "";
                _iconImg.sprite = null;
                return;
            }

            var createdAt = System.DateTimeOffset.FromUnixTimeMilliseconds(itemInfo.CreatedAt).LocalDateTime;

            _nameLbl.text = itemInfo.Name;
            _quantity.text = itemInfo.Quantity.ToString();
            _nftNumLbl.text = itemInfo.NftNumber.ToString();
            _ownerLbl.text = itemInfo.Owner?.Nickname ?? "N/A";
            _createdDateLbl.text = createdAt.ToString("yyyy/MM/dd");
            _statusLbl.text = itemInfo.RawStatus;
            _urlToUIImage.SetImageFromUrl(_iconImg, itemInfo.ItemImage);
        }
    }
}
