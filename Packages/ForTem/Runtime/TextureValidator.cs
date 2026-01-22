using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Validates Texture2D images against ForTem SDK requirements.
    /// Enforces maximum dimensions (256×256px) and file size (200KB).
    /// </summary>
    public static class TextureValidator
    {
        private const int MaxWidth = 256;
        private const int MaxHeight = 256;
        private const int MaxFileSizeBytes = 200 * 1024; // 200KB

        /// <summary>
        /// Validates a texture against ForTem requirements.
        /// </summary>
        /// <param name="texture">The texture to validate</param>
        /// <param name="errorMessage">Output error message if validation fails</param>
        /// <returns>True if texture is valid, false otherwise</returns>
        public static bool IsValid(Texture2D texture, out string errorMessage)
        {
            errorMessage = null;

            if (texture == null)
            {
                errorMessage = "Texture cannot be null";
                return false;
            }

            // Check dimensions
            if (texture.width > MaxWidth || texture.height > MaxHeight)
            {
                errorMessage = $"Texture dimensions exceed maximum size. " +
                    $"Current: {texture.width}×{texture.height}px, " +
                    $"Maximum: {MaxWidth}×{MaxHeight}px";
                return false;
            }

            // Check file size - estimate based on PNG encoding
            byte[] encodedData = texture.EncodeToPNG();
            if (encodedData == null || encodedData.Length == 0)
            {
                errorMessage = "Failed to encode texture to PNG";
                return false;
            }

            if (encodedData.Length > MaxFileSizeBytes)
            {
                int fileSizeKB = encodedData.Length / 1024;
                int maxSizeKB = MaxFileSizeBytes / 1024;
                errorMessage = $"Texture file size exceeds maximum. " +
                    $"Current: {fileSizeKB}KB, Maximum: {maxSizeKB}KB";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the file size in bytes that the texture would be when encoded to PNG.
        /// </summary>
        /// <param name="texture">The texture to measure</param>
        /// <returns>File size in bytes, or -1 if encoding fails</returns>
        public static int GetEncodedFileSizeBytes(Texture2D texture)
        {
            if (texture == null)
            {
                return -1;
            }

            byte[] encodedData = texture.EncodeToPNG();
            return encodedData?.Length ?? -1;
        }

        /// <summary>
        /// Gets the maximum allowed dimensions.
        /// </summary>
        /// <returns>Maximum width and height in pixels</returns>
        public static (int width, int height) GetMaxDimensions()
        {
            return (MaxWidth, MaxHeight);
        }

        /// <summary>
        /// Gets the maximum allowed file size in bytes.
        /// </summary>
        /// <returns>Maximum file size in bytes</returns>
        public static int GetMaxFileSizeBytes()
        {
            return MaxFileSizeBytes;
        }

        /// <summary>
        /// Resizes a texture to fit within maximum dimensions while maintaining aspect ratio.
        /// </summary>
        /// <param name="texture">The texture to resize</param>
        /// <returns>Resized texture, or original if already within limits</returns>
        public static Texture2D ResizeTexture(Texture2D texture)
        {
            if (texture == null)
            {
                return null;
            }

            // Check if resize is needed
            if (texture.width <= MaxWidth && texture.height <= MaxHeight)
            {
                return texture;
            }

            // Calculate new dimensions maintaining aspect ratio
            int newWidth, newHeight;
            float aspectRatio = (float)texture.width / texture.height;

            if (texture.width > texture.height)
            {
                newWidth = MaxWidth;
                newHeight = Mathf.RoundToInt(MaxWidth / aspectRatio);
            }
            else
            {
                newHeight = MaxHeight;
                newWidth = Mathf.RoundToInt(MaxHeight * aspectRatio);
            }

            // Clamp to ensure we don't exceed limits due to rounding
            newWidth = Mathf.Min(newWidth, MaxWidth);
            newHeight = Mathf.Min(newHeight, MaxHeight);

            // Create resized texture
            var resized = new Texture2D(newWidth, newHeight, texture.format, false);
            
            // Use Unity's built-in texture scaling
            Graphics.ConvertTexture(texture, resized);
            
            return resized;
        }

        /// <summary>
        /// Reduces texture file size by recompressing if it exceeds the limit.
        /// Progressively reduces quality until file size is acceptable.
        /// </summary>
        /// <param name="texture">The texture to compress</param>
        /// <returns>Compressed texture, or null if resizing failed</returns>
        public static Texture2D ReduceFileSize(Texture2D texture)
        {
            if (texture == null)
            {
                return null;
            }

            byte[] encoded = texture.EncodeToPNG();
            if (encoded == null || encoded.Length <= MaxFileSizeBytes)
            {
                return texture;
            }

            // Create a copy and progressively reduce quality by scaling down
            var compressed = texture;
            int scaleFactor = 2;
            int attempts = 0;
            int maxAttempts = 3;

            while (compressed != texture && attempts < maxAttempts)
            {
                // Clean up previous attempt if it was a copy
                if (compressed != texture)
                {
                    Object.DestroyImmediate(compressed);
                }

                // Create smaller version
                int newWidth = Mathf.Max(16, texture.width / scaleFactor);
                int newHeight = Mathf.Max(16, texture.height / scaleFactor);

                compressed = new Texture2D(newWidth, newHeight, texture.format, false);
                if (!Graphics.ConvertTexture(texture, compressed))
                {
                    Object.DestroyImmediate(compressed);
                    return null;
                }

                scaleFactor *= 2;
                attempts++;

                encoded = compressed.EncodeToPNG();
                if (encoded.Length <= MaxFileSizeBytes)
                {
                    return compressed;
                }
            }

            if (compressed != texture)
            {
                Object.DestroyImmediate(compressed);
            }

            return null;
        }

        /// <summary>
        /// Validates and fixes a texture to meet all ForTem requirements.
        /// Resizes if too large and recompresses if file size exceeds limit.
        /// </summary>
        /// <param name="texture">The texture to validate and fix</param>
        /// <param name="errorMessage">Output error message if validation fails, null if successful</param>
        /// <returns>Fixed texture that meets all requirements, or null if unfixable</returns>
        public static Texture2D ValidateAndFix(Texture2D texture, out string errorMessage)
        {
            errorMessage = null;

            if (texture == null)
            {
                errorMessage = "Texture cannot be null";
                return null;
            }

            // First, resize if dimensions exceed limits
            Texture2D fixedTexture = texture;
            if (texture.width > MaxWidth || texture.height > MaxHeight)
            {
                fixedTexture = ResizeTexture(texture);
            }

            // Then, reduce file size if needed
            if (!IsValid(fixedTexture, out string sizeError))
            {
                if (sizeError != null && sizeError.Contains("file size"))
                {
                    fixedTexture = ReduceFileSize(fixedTexture);
                }
            }

            // Final validation
            if (!IsValid(fixedTexture, out string finalError))
            {
                errorMessage = finalError;
                if (fixedTexture != texture)
                {
                    Object.DestroyImmediate(fixedTexture);
                }

                return null;
            }

            return fixedTexture;
        }
    }
}
