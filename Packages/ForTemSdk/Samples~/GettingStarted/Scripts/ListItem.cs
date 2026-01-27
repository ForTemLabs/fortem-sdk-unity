using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class ListItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _primaryLbl;
        [SerializeField] private TextMeshProUGUI _secondaryLbl;
        [SerializeField] private Image _iconImg;
        [SerializeField] private UrlToUIImage _urlToUIImage;
        [SerializeField] private Toggle _selectionToggle;

        public Toggle SelectionToggle => _selectionToggle;

        public void Bind(string primaryText, string secondaryText, string imageUrl)
        {
            if (_primaryLbl != null) _primaryLbl.text = primaryText;
            if (_secondaryLbl != null) _secondaryLbl.text = secondaryText;
            if (_iconImg != null && _urlToUIImage != null) _urlToUIImage.SetImageFromUrl(_iconImg, imageUrl);
        }
    }
}
