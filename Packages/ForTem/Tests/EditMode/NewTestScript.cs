using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ForTemSdk
{
    /// <summary>
    /// Integration tests for ForTem SDK initialization and basic workflows.
    /// </summary>
    public class ForTemSDKIntegrationTests
    {
        //[SetUp]
        //public void SetUp()
        //{
        //    // Clear any previous state
        //    if (ForTemClient.Instance != null)
        //    {
        //        var config = new ForTemConfig("test-integration-key", ForTemEnvironment.Testnet, debugLogging: false);
        //        ForTemClient.Initialize(config);
        //    }
        //}

        [UnityTest]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            var requestEnum = GetString();
            yield return requestEnum;
            var result = requestEnum.Current as string;
            Assert.AreEqual("Hello, ForTem!", result);
            Debug.Log(result);
        }

        private IEnumerator GetString()
        {
            yield return "Hello, 1";
            yield return "Hello, ForTem!";
        }

        [Test]
        public void SDK_CanBeInitialized()
        {
            var config = new ForTemConfig("integration-test-key", ForTemEnvironment.Testnet);
            
            Assert.DoesNotThrow(() => ForTemClient.Initialize(config));
            Assert.AreEqual("integration-test-key", ForTemClient.Instance.Config.ApiKey);
        }

        [Test]
        public void SDK_CanBeInitializedWithMainnet()
        {
            var config = new ForTemConfig("integration-test-key", ForTemEnvironment.Mainnet);
            
            ForTemClient.Initialize(config);
            
            Assert.AreEqual(ForTemEnvironment.Mainnet, ForTemClient.Instance.Config.Environment);
            Assert.AreEqual("https://api.fortem.gg", ForTemClient.Instance.Config.GetApiBaseUrl());
        }

        [Test]
        public void SDK_CanBeInitializedWithTestnet()
        {
            var config = new ForTemConfig("integration-test-key", ForTemEnvironment.Testnet);
            
            ForTemClient.Initialize(config);
            
            Assert.AreEqual(ForTemEnvironment.Testnet, ForTemClient.Instance.Config.Environment);
            Assert.AreEqual("https://testnet-api.fortem.gg", ForTemClient.Instance.Config.GetApiBaseUrl());
        }

        [Test]
        public void SDK_AllAPIsAreAccessible()
        {
            var config = new ForTemConfig("test-key");
            ForTemClient.Initialize(config);

            Assert.IsNotNull(ForTemClient.Instance.Auth);
            Assert.IsNotNull(ForTemClient.Instance.User);
            Assert.IsNotNull(ForTemClient.Instance.Collections);
        }

        [UnityTest]
        public IEnumerator SDK_CanCreateHttpClient()
        {
            var config = new ForTemConfig("test-key");
            var httpClient = new ForTemHttpClient(config);

            Assert.IsNotNull(httpClient);

            yield return null;
        }

        [Test]
        public void AsyncSDK_CanBeCreated()
        {
            var config = new ForTemConfig("async-test-key", ForTemEnvironment.Testnet);
            var client = new ForTemClientAsync(config);

            Assert.IsNotNull(client);
            Assert.AreEqual("async-test-key", client.Config.ApiKey);
            
            client.Dispose();
        }

        [Test]
        public void AsyncSDK_AllAPIsAreAccessible()
        {
            var config = new ForTemConfig("async-test-key");
            var client = new ForTemClientAsync(config);

            Assert.IsNotNull(client.Auth);
            Assert.IsNotNull(client.User);
            Assert.IsNotNull(client.Collections);

            client.Dispose();
        }

        [Test]
        public void MultipleConfigs_CanBeCreated()
        {
            var config1 = new ForTemConfig("key1", ForTemEnvironment.Testnet);
            var config2 = new ForTemConfig("key2", ForTemEnvironment.Mainnet);
            var config3 = new ForTemConfig("key3");

            Assert.AreNotEqual(config1.GetApiBaseUrl(), config2.GetApiBaseUrl());
            Assert.AreEqual(config2.GetApiBaseUrl(), config3.GetApiBaseUrl());
        }

        [Test]
        public void ResponseParsing_WorksWithMultipleTypes()
        {
            var userJson = "{\"statusCode\":200,\"data\":{\"isUser\":true,\"nickname\":\"Test\"}}";
            var userResult = ForTemClient.ParseResponse<User>(userJson);

            Assert.IsTrue(userResult.Success);

            var nonceJson = "{\"statusCode\":200,\"data\":{\"nonce\":\"test-nonce\"}}";
            var nonceResult = ForTemClient.ParseResponse<NonceResponse>(nonceJson);

            Assert.IsTrue(nonceResult.Success);
        }
    }
}
