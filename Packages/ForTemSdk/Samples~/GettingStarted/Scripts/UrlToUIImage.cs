using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace ForTemSdk.Samples
{
    public class UrlToUIImage : MonoBehaviour
    {
        /// <summary>
        /// Loads an image from a URL and assigns it to the given UI Image.
        /// </summary>
        public void SetImageFromUrl(Image targetImage, string url)
        {
            if (targetImage == null || string.IsNullOrEmpty(url))
            {
                Debug.LogError("UIImageLoader: Target image or URL is null/empty.");
                return;
            }

            StartCoroutine(LoadImageCoroutine(targetImage, url));
        }

        private IEnumerator LoadImageCoroutine(Image targetImage, string url)
        {
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
            {
                yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
#else
            if (request.isNetworkError || request.isHttpError)
#endif
                {
                    Debug.LogError("UIImageLoader: Failed to load image from URL: " + request.error);
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(request);
                    Sprite sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f)
                    );

                    targetImage.sprite = sprite;
                }
            }
        }
    }
}
