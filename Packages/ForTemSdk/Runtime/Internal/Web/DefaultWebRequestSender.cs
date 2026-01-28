using System.Threading.Tasks;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    internal sealed class DefaultWebRequestSender : IWebRequestSender
    {
        public Task<WebRequestResponse> Send(UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<WebRequestResponse>();
            var operation = request.SendWebRequest();
            operation.completed += _ =>
            {
                var response = new WebRequestResponse
                {
                    Text = request.downloadHandler.text,
                    Error = request.error,
                    ResponseCode = request.responseCode,
                    Result = request.result,
                };
                tcs.SetResult(response);
            };

            return tcs.Task;
        }
    }
}
