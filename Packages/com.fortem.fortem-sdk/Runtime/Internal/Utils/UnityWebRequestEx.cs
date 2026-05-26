using System.Text;
using UnityEngine.Networking;

#nullable enable

namespace ForTemSdk
{
    internal static class UnityWebRequestEx
    {
        /// <summary>
        /// Creates a UnityWebRequest configured to send form data to a server via HTTP POST.
        /// </summary>
        /// <remarks>
        /// Back compat: Unity introduced this overload in 2022.2.
        /// </remarks>
        /// <param name="uri">The target URI to which the string will be transmitted.</param>
        /// <param name="postData">Form body data. Will be converted to UTF-8 string.</param>
        /// <param name="contentType">Value for the Content-Type header, for example application/json.</param>
        /// <returns>A UnityWebRequest configured to send string to uri via POST.</returns>
        public static UnityWebRequest Post(string uri, string postData, string contentType)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "POST");
            SetupPost(request, postData, contentType);
            return request;
        }

        public static UnityWebRequest Put(string uri, string postData, string contentType)
        {
            UnityWebRequest request = new UnityWebRequest(uri, "PUT");
            SetupPost(request, postData, contentType);
            return request;
        }

        private static void SetupPost(UnityWebRequest request, string postData, string contentType)
        {
            request.downloadHandler = new DownloadHandlerBuffer();
            if (string.IsNullOrEmpty(postData))
            {
                request.SetRequestHeader("Content-Type", contentType);
                return;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(postData);
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.uploadHandler.contentType = contentType;
        }
    }
}
