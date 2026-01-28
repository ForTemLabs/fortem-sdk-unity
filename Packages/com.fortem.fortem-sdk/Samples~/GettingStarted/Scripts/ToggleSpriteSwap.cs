using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class ToggleSpriteSwap : MonoBehaviour
    {
        [SerializeField] private Sprite _spriteOn;
        [SerializeField] private Toggle _toggle;

        private Image _targetGraphic;

        private void Start()
        {
            _targetGraphic = _toggle.targetGraphic as Image;
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
            OnToggleValueChanged(_toggle.isOn);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            _targetGraphic.overrideSprite = isOn ? _spriteOn : null;
        }
    }
}
