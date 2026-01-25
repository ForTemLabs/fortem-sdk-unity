using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Converts Sprites and Textures to PNG byte arrays.
    /// Enforces maximum dimensions (256×256px) required by ForTem.
    /// </summary>
    public static class ImageUtil
    {
        private const int MaxWidth = 256;
        private const int MaxHeight = 256;

        public static byte[] SpriteToByteArray(Sprite sprite)
        {
            var sourceTex = sprite.texture;
            if (sprite.rect.width == sourceTex.width)
            {
                return TextureToByteArray(sourceTex);
            }

            Texture2D atlasTex = sourceTex;
            if (!atlasTex.isReadable)
            {
                RenderTexture rt = RenderTexture.GetTemporary(atlasTex.width, atlasTex.height, 0);
                Graphics.Blit(atlasTex, rt);

                RenderTexture prev = RenderTexture.active;
                RenderTexture.active = rt;

                atlasTex = new Texture2D(atlasTex.width, atlasTex.height, TextureFormat.RGBA32, false);
                atlasTex.ReadPixels(new Rect(0, 0, atlasTex.width, atlasTex.height), 0, 0);
                atlasTex.Apply();

                RenderTexture.active = prev;
                RenderTexture.ReleaseTemporary(rt);
            }

            Rect rect = sprite.textureRect;
            var newTex = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGBA32, false);

            Color[] pixels = atlasTex.GetPixels(
                (int)rect.x,
                (int)rect.y,
                (int)rect.width,
                (int)rect.height);

            newTex.SetPixels(pixels);
            newTex.Apply();
            var bytes = TextureToByteArray(newTex);

            GameObject.DestroyImmediate(newTex);
            if (atlasTex != sourceTex)
            {
                GameObject.DestroyImmediate(atlasTex);
            }

            return bytes;
        }

        public static byte[] TextureToByteArray(Texture2D source)
        {
            var needsResize = source.width > MaxWidth || source.height > MaxHeight;
            if (source.isReadable && !needsResize)
            {
                return source.EncodeToPNG();
            }

            int newWidth = source.width;
            int newHeight = source.height;
            if (needsResize)
            {
                float aspectRatio = (float)source.width / source.height;

                if (source.width > source.height)
                {
                    newWidth = MaxWidth;
                    newHeight = Mathf.RoundToInt(MaxHeight / aspectRatio);
                }
                else
                {
                    newHeight = MaxHeight;
                    newWidth = Mathf.RoundToInt(MaxWidth * aspectRatio);
                }

                // Clamp to ensure we don't exceed limits due to rounding
                newWidth = Mathf.Min(newWidth, MaxWidth);
                newHeight = Mathf.Min(newHeight, MaxHeight);
            }

            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight, 0);
            Graphics.Blit(source, rt);

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            var result = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);
            result.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            result.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            var bytes = result.EncodeToPNG();
            GameObject.DestroyImmediate(result);

            return bytes;
        }
    }
}
