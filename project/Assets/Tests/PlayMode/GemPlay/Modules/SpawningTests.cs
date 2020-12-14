using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Core = GemPlay.Core;
using Rest = GemPlay.Modules.Rest;
using Spawning = GemPlay.Modules.Spawning;

namespace Tests.GemPlay.Modules
{
    [TestFixture]
    public class SpawningTests
    {
        const string testModelName = "model:TestModel";

        GameObject content;

        Rest.RestInterface rest;

        Spawning.SpawningInterface spawning;

        [SetUp]
        public void Setup()
        {
            content = new GameObject("Content");

            var coroutineManager = content.AddComponent<CoroutineManager>();
            
            rest = new Rest.RestInterface();

            spawning = new Spawning.SpawningInterface(
                parentGameObject: content,
                coroutineManager: coroutineManager,
                constructScene: true);
        }

        [TearDown]
        public void DestroyObjects()
        { // Destroy any game objects that were created during Setup.
            UnityEngine.Object.Destroy(content);
        }

        [UnityTest]
        public IEnumerator TestSpawnModelOutput()
        {
            bool waiting = true;

            int expectedSpawnedMeshesCount = 6;

            var container = new Core.Data.GemPy.GameObjectContainers.Output();

            rest.LoadModel(geoModelUrn: testModelName)
                .Then(() =>
                {
                    return rest.GetModelOutput(geoModelUrn: testModelName);
                })
                .Then(modelOutput =>
                {
                    return spawning.SpawnModelOutput(
                        outputData: modelOutput,
                        outputContainer: container);
                })
                .Then(() =>
                {
                    waiting = false;
                })
                .Catch(error =>
                {
                    Debug.Log(error.Message);

                    waiting = false;
                });

            while (waiting)
            {
                yield return null;
            }

            int meshRendererCount = container.RexMeshesGameObject.GetComponentsInChildren<MeshRenderer>().Length;

            Debug.Log("Spawned " + meshRendererCount + " game objects with mesh renderers.");

            Assert.AreEqual(expectedSpawnedMeshesCount, meshRendererCount);
        }
    }
}
