using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class UserPageSample : MonoBehaviour
    {
        [Header("Response")]
        [SerializeField] private Image _profileImage;
        [SerializeField] private UrlToUIImage _urlToUIImage;
        [SerializeField] private TextMeshProUGUI _nicknameLbl;
        [SerializeField] private TextMeshProUGUI _isForTemUserLbl;

        [Header("Application")]
        [SerializeField] private ForTemClientProvider _forTemClientProvider;
        [SerializeField] private AppContext _appContext;

        private void Awake()
        {
            _appContext.UserChanged += BindUserInfo;
        }

        private void OnDestroy()
        {
            _appContext.UserChanged -= BindUserInfo;
        }

        private void BindUserInfo(GetUserResponse userInfo)
        {
            if (userInfo == null)
            {
                _profileImage.sprite = null;
                _nicknameLbl.text = "No User";
                _isForTemUserLbl.text = string.Empty;
                return;
            }

            _nicknameLbl.text = $"Nickname: {userInfo.Nickname}";
            _isForTemUserLbl.text = userInfo.IsUser ? "ForTem User" : "Guest User";
            if (userInfo.IsUser)
            {
                _urlToUIImage.SetImageFromUrl(_profileImage, userInfo.ProfileImage);
            }
        }
    }
}
