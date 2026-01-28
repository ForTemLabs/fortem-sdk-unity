using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ForTemSdk
{
    internal interface IWebRequestSender
    {
        Task<WebRequestResponse> Send(UnityWebRequest request);
    }
}
