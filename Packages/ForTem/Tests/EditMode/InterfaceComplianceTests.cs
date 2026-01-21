using NUnit.Framework;

namespace ForTemSdk
{
    /// <summary>
    /// Tests for SDK interfaces and contract compliance.
    /// </summary>
    public class InterfaceComplianceTests
    {
        [Test]
        public void ForTemConfig_ImplementsIForTemConfig()
        {
            var config = new ForTemConfig("test-key");

            Assert.IsInstanceOf<IForTemConfig>(config);
        }

        [Test]
        public void ForTemHttpClient_ImplementsIForTemHttpClient()
        {
            var config = new ForTemConfig("test-key");
            var httpClient = new ForTemHttpClient(config);

            Assert.IsInstanceOf<IForTemHttpClient>(httpClient);
        }

        [Test]
        public void ForTemHttpClientAsync_ImplementsIForTemHttpClientAsync()
        {
            var config = new ForTemConfig("test-key");
            var httpClient = new ForTemHttpClientAsync(config);

            Assert.IsInstanceOf<IForTemHttpClientAsync>(httpClient);
        }

        [Test]
        public void IForTemConfig_HasRequiredProperties()
        {
            IForTemConfig config = new ForTemConfig("test-key");

            Assert.IsNotNull(config.ApiKey);
            Assert.NotZero((int)config.Environment);
            Assert.NotZero(config.TimeoutSeconds);
            Assert.IsNotEmpty(config.GetApiBaseUrl());
            Assert.IsNotEmpty(config.GetServiceUrl());
        }

        [Test]
        public void IForTemHttpClient_HasRequiredMethods()
        {
            IForTemHttpClient httpClient = new ForTemHttpClient(new ForTemConfig("test-key"));

            Assert.IsNotNull(httpClient);
            var hasMethod = httpClient.GetType().GetMethod("SendRequest", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(hasMethod);
        }

        [Test]
        public void IForTemHttpClientAsync_HasRequiredMethods()
        {
            var config = new ForTemConfig("test-key");
            IForTemHttpClientAsync httpClient = new ForTemHttpClientAsync(config);

            Assert.IsNotNull(httpClient);
            var sendMethod = httpClient.GetType().GetMethod("SendRequestAsync",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            Assert.IsNotNull(sendMethod);
        }
    }

    /// <summary>
    /// Tests for ForTemEnvironment enum.
    /// </summary>
    public class ForTemEnvironmentTests
    {
        [Test]
        public void ForTemEnvironment_HasTestnetValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ForTemEnvironment), ForTemEnvironment.Testnet));
        }

        [Test]
        public void ForTemEnvironment_HasMainnetValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ForTemEnvironment), ForTemEnvironment.Mainnet));
        }

        [Test]
        public void ForTemEnvironment_CanCompare()
        {
            var testnet = ForTemEnvironment.Testnet;
            var mainnet = ForTemEnvironment.Mainnet;

            Assert.AreNotEqual(testnet, mainnet);
        }

        [Test]
        public void ForTemEnvironment_CanConvertFromInt()
        {
            var env = (ForTemEnvironment)0;

            Assert.AreEqual(ForTemEnvironment.Testnet, env);
        }
    }

    /// <summary>
    /// Tests for HttpMethod enum.
    /// </summary>
    public class HttpMethodTests
    {
        [Test]
        public void HttpMethod_HasGetValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(HttpMethod), HttpMethod.Get));
        }

        [Test]
        public void HttpMethod_HasPostValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(HttpMethod), HttpMethod.Post));
        }

        [Test]
        public void HttpMethod_HasPutValue()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(HttpMethod), HttpMethod.Put));
        }

        [Test]
        public void HttpMethod_GetAndPost_AreDifferent()
        {
            Assert.AreNotEqual(HttpMethod.Get, HttpMethod.Post);
        }
    }
}
