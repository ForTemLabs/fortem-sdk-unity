using NUnit.Framework;
using UnityEngine;

namespace ForTemSdk
{
    /// <summary>
    /// Tests for ForTemConfig configuration class.
    /// </summary>
    public class ForTemConfigTests
    {
        [Test]
        public void Constructor_WithValidApiKey_SetsApiKey()
        {
            var config = new ForTemConfig("test-api-key");

            Assert.AreEqual("test-api-key", config.ApiKey);
        }

        [Test]
        public void Constructor_WithMainnetEnvironment_DefaultIsMainnet()
        {
            var config = new ForTemConfig("test-api-key");

            Assert.AreEqual(ForTemEnvironment.Mainnet, config.Environment);
        }

        [Test]
        public void Constructor_WithTestnetEnvironment_SetsTestnet()
        {
            var config = new ForTemConfig("test-api-key", ForTemEnvironment.Testnet);

            Assert.AreEqual(ForTemEnvironment.Testnet, config.Environment);
        }

        [Test]
        public void Constructor_WithDebugLoggingFlag_SetsDebugLogging()
        {
            var config = new ForTemConfig("test-api-key", ForTemEnvironment.Mainnet, true);

            Assert.IsTrue(config.DebugLogging);
        }

        [Test]
        public void TimeoutSeconds_DefaultsTo30()
        {
            var config = new ForTemConfig("test-api-key");

            Assert.AreEqual(30, config.TimeoutSeconds);
        }

        [Test]
        public void TimeoutSeconds_CanBeModified()
        {
            var config = new ForTemConfig("test-api-key");
            config.TimeoutSeconds = 60;

            Assert.AreEqual(60, config.TimeoutSeconds);
        }

        [Test]
        public void GetApiBaseUrl_WithMainnet_ReturnsMainnetUrl()
        {
            var config = new ForTemConfig("test-api-key", ForTemEnvironment.Mainnet);

            Assert.AreEqual("https://api.fortem.gg", config.GetApiBaseUrl());
        }

        [Test]
        public void GetApiBaseUrl_WithTestnet_ReturnsTestnetUrl()
        {
            var config = new ForTemConfig("test-api-key", ForTemEnvironment.Testnet);

            Assert.AreEqual("https://testnet-api.fortem.gg", config.GetApiBaseUrl());
        }

        [Test]
        public void GetServiceUrl_WithMainnet_ReturnsMainnetServiceUrl()
        {
            var config = new ForTemConfig("test-api-key", ForTemEnvironment.Mainnet);

            Assert.AreEqual("https://fortem.gg", config.GetServiceUrl());
        }

        [Test]
        public void GetServiceUrl_WithTestnet_ReturnsTestnetServiceUrl()
        {
            var config = new ForTemConfig("test-api-key", ForTemEnvironment.Testnet);

            Assert.AreEqual("https://testnet.fortem.gg", config.GetServiceUrl());
        }

        [Test]
        public void ImplementsIForTemConfig_Interface()
        {
            var config = new ForTemConfig("test-api-key");

            Assert.IsInstanceOf<IForTemConfig>(config);
        }
    }
}
