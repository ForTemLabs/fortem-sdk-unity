using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class InventoryItem : MonoBehaviour
    {
        public string Name;
        public string Description;
        public string RedeemCode;
        public int Quantity;
        public Sprite Sprite;
        public Toggle Toggle;

        private void Awake()
        {
            if (PlayerPrefs.HasKey(RedeemCode))
            {
                gameObject.SetActive(false);
            }
        }

        public void MarkAsRedeemed()
        {
            PlayerPrefs.SetInt(RedeemCode, 1);
            gameObject.SetActive(false);
        }
    }
}
