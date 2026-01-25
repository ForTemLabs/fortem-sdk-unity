using UnityEngine;

namespace ForTemSdk.Samples
{
    /// <summary>
    /// Rotates a RectTransform continuously.
    /// </summary>
    public class RotatingRt : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private float _rotationSpeed = 30f; // degrees per second
        private void Update()
        {
            if (_rectTransform != null)
            {
                _rectTransform.Rotate(0f, 0f, _rotationSpeed * Time.deltaTime);
            }
        }
    }
}
