using UnityEngine.Networking;

namespace ForTemSdk
{
    internal struct WebRequestResponse
    {
        public string Text;
        public string Error;
        public long ResponseCode;
        public UnityWebRequest.Result Result;
    }
}
