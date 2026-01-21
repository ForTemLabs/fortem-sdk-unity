using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace ForTemSdk
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
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
    }
}
