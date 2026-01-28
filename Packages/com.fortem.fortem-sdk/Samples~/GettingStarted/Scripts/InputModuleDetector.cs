using UnityEngine;
using UnityEngine.EventSystems;

public class InputModuleDetector : MonoBehaviour
{
    [SerializeField] private Component _inputSystem;
    [SerializeField] private StandaloneInputModule _legacyModule;

    void Awake()
    {
#if ENABLE_INPUT_SYSTEM
        Destroy(_legacyModule);
#else
        Destroy(_inputSystem);
#endif
    }
}
