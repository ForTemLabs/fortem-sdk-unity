using System;
using UnityEngine;

namespace ForTemSdk.Samples
{
    public class AppContext : MonoBehaviour
    {
        public event Action<GetUserResponse> UserChanged;

        private GetUserResponse _currentUser;

        public GetUserResponse CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                UserChanged?.Invoke(_currentUser);
            }
        }

        public int SelectedCollectionId { get; set; }
    }
}
