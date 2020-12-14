using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Core = GemPlay.Core;
using Rest = GemPlay.Modules.Rest;

namespace Tests.GemPlay.Modules
{
    [TestFixture]
    public class RestTests
    {
        Rest.RestInterface rest;

        const string testModelName = "model:TestModel";

        [SetUp]
        public void Setup()
        {
            rest = new Rest.RestInterface();
        }

        [UnityTest]
        public IEnumerator TestGetModelNames()
        {
            string [] modelNames = new string[] {"",};

            bool waiting  = true;

            rest.GetModelNames()
                .Then(_modelNames =>
                {
                    modelNames = _modelNames;

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

            CollectionAssert.Contains(modelNames, testModelName);
        }

        [UnityTest]
        public IEnumerator TestLoadModel()
        {
            bool loaded = false;

            bool waiting  = true;

            rest.LoadModel(geoModelUrn: testModelName)
                .Then(() =>
                {
                    loaded = true;

                    waiting = false;
                })
                .Catch(error =>
                {
                    Debug.Log(error.Message);

                    waiting = false;

                    throw error;
                });

            while (waiting)
            {
                yield return null;
            }

            Assert.IsTrue(loaded);
        }

        [UnityTest]
        public IEnumerator TestGetModelInput()
        {
            var modelInput = new Core.Data.GemPy.Input();

            bool waiting = true;

            rest.LoadModel(geoModelUrn: testModelName)
                .Then(() =>
                {
                    return rest.GetModelInput(geoModelUrn: testModelName);
                })
                .Then(_modelInput =>
                {
                    modelInput = _modelInput;

                    waiting = false;
                });

            while (waiting)
            {
                yield return null;
            }

            Assert.AreEqual(27, modelInput.DataPoints.Count);

            Assert.AreEqual(4, modelInput.Surfaces.Count);
        }

        [UnityTest]
        public IEnumerator TestGetModelOutput()
        {
            var modelOutput = new Core.Data.GemPy.Output
            {
                RexMeshData = new byte[] { 0x20, }
            };

            bool waiting = true;

            rest.LoadModel(geoModelUrn: testModelName)
                .Then(() =>
                {
                    return rest.GetModelOutput(geoModelUrn: testModelName);
                })
                .Then(_modelOutput =>
                {
                    modelOutput = _modelOutput;

                    waiting = false;
                });

            while (waiting)
            {
                yield return null;
            }

            Assert.AreEqual(modelOutput.RexMeshData.Length, 760214);
        }
    }
}
