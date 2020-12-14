using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Main = GemPlay.Applications.Main;

namespace Tests.GemPlay.Applications
{
    [TestFixture]
    public class MainTests
    {
        GameObject mainGameObject;
        
        [SetUp]
        public void Setup()
        {
            mainGameObject = new GameObject();

            mainGameObject.AddComponent<Main.Main>();
        }

        [TearDown]
        public void DestroyObjects()
        {
            Object.Destroy(mainGameObject);
        }

        [UnityTest]
        public IEnumerator TestSetup()
        {
            yield return null;
        }
    }
}
