using NUnit.Framework;
using UnityEngine;

namespace ForTemSdk.Tests
{
    [TestFixture]
    public sealed class TextureValidatorTests
    {
        private Texture2D _validTexture;
        private Texture2D _oversizedTexture;

        [SetUp]
        public void Setup()
        {
            // Create a valid texture (128×128)
            _validTexture = new Texture2D(128, 128, TextureFormat.RGBA32, false);
            FillTexture(_validTexture, Color.white);

            // Create an oversized texture (512×512)
            _oversizedTexture = new Texture2D(512, 512, TextureFormat.RGBA32, false);
            FillTexture(_oversizedTexture, Color.white);
        }

        [TearDown]
        public void Cleanup()
        {
            Object.DestroyImmediate(_validTexture);
            Object.DestroyImmediate(_oversizedTexture);
        }

        [Test]
        public void IsValid_WithValidTexture_ReturnsTrue()
        {
            // Act
            bool isValid = TextureValidator.IsValid(_validTexture, out string errorMessage);

            // Assert
            Assert.IsTrue(isValid);
            Assert.IsNull(errorMessage);
        }

        [Test]
        public void IsValid_WithNullTexture_ReturnsFalse()
        {
            // Act
            bool isValid = TextureValidator.IsValid(null, out string errorMessage);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual("Texture cannot be null", errorMessage);
        }

        [Test]
        public void IsValid_WithOversizedDimensions_ReturnsFalse()
        {
            // Act
            bool isValid = TextureValidator.IsValid(_oversizedTexture, out string errorMessage);

            // Assert
            Assert.IsFalse(isValid);
            Assert.That(errorMessage, Does.Contain("dimensions exceed maximum size"));
            Assert.That(errorMessage, Does.Contain("512×512"));
            Assert.That(errorMessage, Does.Contain("256×256"));
        }

        [Test]
        public void IsValid_WithWidthExceedingMax_ReturnsFalse()
        {
            // Arrange
            var texture = new Texture2D(257, 100, TextureFormat.RGBA32, false);
            FillTexture(texture, Color.white);

            try
            {
                // Act
                bool isValid = TextureValidator.IsValid(texture, out string errorMessage);

                // Assert
                Assert.IsFalse(isValid);
                Assert.That(errorMessage, Does.Contain("257×100"));
            }
            finally
            {
                Object.DestroyImmediate(texture);
            }
        }

        [Test]
        public void IsValid_WithHeightExceedingMax_ReturnsFalse()
        {
            // Arrange
            var texture = new Texture2D(100, 257, TextureFormat.RGBA32, false);
            FillTexture(texture, Color.white);

            try
            {
                // Act
                bool isValid = TextureValidator.IsValid(texture, out string errorMessage);

                // Assert
                Assert.IsFalse(isValid);
                Assert.That(errorMessage, Does.Contain("100×257"));
            }
            finally
            {
                Object.DestroyImmediate(texture);
            }
        }

        [Test]
        public void IsValid_WithMaxDimensions_ReturnsTrue()
        {
            // Arrange
            var texture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            FillTexture(texture, Color.white);

            try
            {
                // Act
                bool isValid = TextureValidator.IsValid(texture, out string errorMessage);

                // Assert
                Assert.IsTrue(isValid);
                Assert.IsNull(errorMessage);
            }
            finally
            {
                Object.DestroyImmediate(texture);
            }
        }

        [Test]
        public void GetEncodedFileSizeBytes_WithValidTexture_ReturnsPositiveSize()
        {
            // Act
            int fileSize = TextureValidator.GetEncodedFileSizeBytes(_validTexture);

            // Assert
            Assert.Greater(fileSize, 0);
            Assert.LessOrEqual(fileSize, 200 * 1024); // Should be under 200KB
        }

        [Test]
        public void GetEncodedFileSizeBytes_WithNullTexture_ReturnsMinus1()
        {
            // Act
            int fileSize = TextureValidator.GetEncodedFileSizeBytes(null);

            // Assert
            Assert.AreEqual(-1, fileSize);
        }

        [Test]
        public void GetMaxDimensions_ReturnsCorrectValues()
        {
            // Act
            var (width, height) = TextureValidator.GetMaxDimensions();

            // Assert
            Assert.AreEqual(256, width);
            Assert.AreEqual(256, height);
        }

        [Test]
        public void GetMaxFileSizeBytes_ReturnsCorrectValue()
        {
            // Act
            int maxSize = TextureValidator.GetMaxFileSizeBytes();

            // Assert
            Assert.AreEqual(200 * 1024, maxSize);
        }

        [Test]
        public void ResizeTexture_WithValidTexture_ReturnsOriginal()
        {
            // Act
            Texture2D result = TextureValidator.ResizeTexture(_validTexture);

            // Assert
            Assert.AreEqual(_validTexture, result);
        }

        [Test]
        public void ResizeTexture_WithOversizedTexture_ScalesDownMaintainingAspectRatio()
        {
            // Act
            Texture2D result = TextureValidator.ResizeTexture(_oversizedTexture);

            try
            {
                // Assert
                Assert.NotNull(result);
                Assert.LessOrEqual(result.width, 256);
                Assert.LessOrEqual(result.height, 256);

                // Check aspect ratio is maintained (allow small rounding error)
                float originalAspect = (float)_oversizedTexture.width / _oversizedTexture.height;
                float resizedAspect = (float)result.width / result.height;
                Assert.That(resizedAspect, Is.EqualTo(originalAspect).Within(0.1f));
            }
            finally
            {
                if (result != _oversizedTexture)
                    Object.DestroyImmediate(result);
            }
        }

        [Test]
        public void ResizeTexture_WithWideTexture_MainsAspectRatioWidth()
        {
            // Arrange
            var wideTexture = new Texture2D(400, 100, TextureFormat.RGBA32, false);
            FillTexture(wideTexture, Color.white);

            Texture2D result = null;
            try
            {
                // Act
                result = TextureValidator.ResizeTexture(wideTexture);

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(256, result.width);
                Assert.AreEqual(64, result.height);
            }
            finally
            {
                if (result != wideTexture)
                    Object.DestroyImmediate(result);
                Object.DestroyImmediate(wideTexture);
            }
        }

        [Test]
        public void ResizeTexture_WithTallTexture_MaintainsAspectRatioHeight()
        {
            // Arrange
            var tallTexture = new Texture2D(100, 400, TextureFormat.RGBA32, false);
            FillTexture(tallTexture, Color.white);

            Texture2D result = null;
            try
            {
                // Act
                result = TextureValidator.ResizeTexture(tallTexture);

                // Assert
                Assert.NotNull(result);
                Assert.AreEqual(64, result.width);
                Assert.AreEqual(256, result.height);
            }
            finally
            {
                if (result != tallTexture)
                    Object.DestroyImmediate(result);
                Object.DestroyImmediate(tallTexture);
            }
        }

        [Test]
        public void ReduceFileSize_WithValidTexture_ReturnsOriginal()
        {
            // Act
            Texture2D result = TextureValidator.ReduceFileSize(_validTexture);

            // Assert
            Assert.AreEqual(_validTexture, result);
        }

        [Test]
        public void ReduceFileSize_ReducesFileSizeBelow200KB()
        {
            // Arrange - Create a texture that might exceed 200KB
            var largeTexture = new Texture2D(256, 256, TextureFormat.RGBA32, false);
            FillTexture(largeTexture, Color.white);

            try
            {
                int originalSize = TextureValidator.GetEncodedFileSizeBytes(largeTexture);

                // Act
                Texture2D result = TextureValidator.ReduceFileSize(largeTexture);

                try
                {
                    // Assert
                    Assert.NotNull(result);
                    int resultSize = TextureValidator.GetEncodedFileSizeBytes(result);
                    Assert.LessOrEqual(resultSize, 200 * 1024);
                }
                finally
                {
                    if (result != largeTexture)
                        Object.DestroyImmediate(result);
                }
            }
            finally
            {
                Object.DestroyImmediate(largeTexture);
            }
        }

        [Test]
        public void ValidateAndFix_WithValidTexture_ReturnsOriginal()
        {
            // Act
            Texture2D result = TextureValidator.ValidateAndFix(_validTexture, out string error);

            // Assert
            Assert.AreEqual(_validTexture, result);
            Assert.IsNull(error);
        }

        [Test]
        public void ValidateAndFix_WithOversizedTexture_ResizesAndReturnsValidTexture()
        {
            // Act
            Texture2D result = TextureValidator.ValidateAndFix(_oversizedTexture, out string error);

            try
            {
                // Assert
                Assert.NotNull(result);
                Assert.IsNull(error);
                Assert.IsTrue(TextureValidator.IsValid(result, out _));
                Assert.LessOrEqual(result.width, 256);
                Assert.LessOrEqual(result.height, 256);
            }
            finally
            {
                if (result != _oversizedTexture)
                    Object.DestroyImmediate(result);
            }
        }

        [Test]
        public void ValidateAndFix_WithNullTexture_ReturnsNullWithError()
        {
            // Act
            Texture2D result = TextureValidator.ValidateAndFix(null, out string error);

            // Assert
            Assert.IsNull(result);
            Assert.That(error, Does.Contain("null"));
        }

        private void FillTexture(Texture2D texture, Color color)
        {
            Color[] pixels = new Color[texture.width * texture.height];
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }
            texture.SetPixels(pixels);
            texture.Apply();
        }
    }
}
