using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Projects = GemPlay.Modules.Projects;
using Core = GemPlay.Core;

namespace Tests.GemPlay.Modules
{
    [TestFixture]
    public class ProjectsTests
    {
        GameObject mainGameObject;

        Projects.ProjectsInterface projects;

        [SetUp]
        public void Setup()
        {
            mainGameObject = new GameObject("ProjectsTests.Main");

            projects = new Projects.ProjectsInterface(
                parentGameObject: in mainGameObject,
                coroutineManager: mainGameObject.AddComponent<CoroutineManager>(),
                constructScene: true);
        }

        [TearDown]
        public void DestroyObjects()
        { // Destroy any game objects that were created during Setup.
            Object.Destroy(mainGameObject);
        }

        [UnityTest]
        public IEnumerator TestSurfacePointGameObjectCoordinateTransformation()
        {
            var expectedPosition = new Vector3(1.23f, 4.56f, 7.89f);

            var project = projects.AddProject("foo");

            projects.SetDataToGameCoordinatesConversionParameters(
                project: ref project,
                shift: new Vector3(-0.12f, 3.45f, 6.78f),
                scale: 0.123f);

            var gameObject = new GameObject("Surface Point");

            gameObject.transform.localPosition = new Vector3(
                expectedPosition.x,
                expectedPosition.y,
                expectedPosition.z);

            projects.TransformFromDataToGameCoordinates(
                project: in project,
                gameObject: ref gameObject);

            // https://docs.unity3d.com/ScriptReference/Vector3.Equals.html says to use == for approximate test
            Assert.IsFalse(gameObject.transform.localPosition == expectedPosition);

            projects.TransformFromGameToDataCoordinates(
                project: in project,
                gameObject: ref gameObject);

            Assert.IsTrue(gameObject.transform.localPosition == expectedPosition);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestOrientationGameObjectCoordinateTransformation()
        {
            var expectedPosition = new Vector3(1.23f, 4.56f, 7.89f);

            var expectedRotation = Quaternion.Euler(12.3f, 45.6f, 78.9f);

            var project = projects.AddProject("foo");

            projects.SetDataToGameCoordinatesConversionParameters(
                project: ref project,
                shift: new Vector3(-0.12f, 3.45f, 6.78f),
                scale: 0.123f);

            var gameObject = new GameObject("Surface Point");

            gameObject.transform.localPosition = new Vector3(
                expectedPosition.x,
                expectedPosition.y,
                expectedPosition.z);

            gameObject.transform.localRotation = expectedRotation;

            projects.TransformFromDataToGameCoordinates(
                project: in project,
                gameObject: ref gameObject);
                
            // https://docs.unity3d.com/ScriptReference/Vector3.Equals.html says to use == for approximate test
            Assert.IsFalse(gameObject.transform.localPosition == expectedPosition);

            Assert.IsFalse(gameObject.transform.localRotation == expectedRotation);

            projects.TransformFromGameToDataCoordinates(
                project: in project,
                gameObject: ref gameObject);

            Assert.IsTrue(gameObject.transform.localPosition == expectedPosition);

            Assert.IsTrue(gameObject.transform.localRotation == expectedRotation);

            yield return null;
        }

        [UnityTest]
        public IEnumerator TestSurfacePointDataCoordinateTransformation()
        {
            var expectedPosition = new Vector3(1.23f, 4.56f, 7.89f);

            var project = projects.AddProject("foo");

            projects.SetDataToGameCoordinatesConversionParameters(
                project: ref project,
                shift: new Vector3(-0.12f, 3.45f, 6.78f),
                scale: 0.123f);

            var data = new Core.Data.GemPy.DataPoint
            {
                position = new Vector3(
                    expectedPosition.x,
                    expectedPosition.y,
                    expectedPosition.z),
                type = Core.Data.GemPy.InputType.SurfacePoint
            };

            var gameObject = new GameObject("Surface Point");

            gameObject.transform.localPosition = new Vector3(
                data.position.x,
                data.position.y,
                data.position.z);

            projects.TransformFromDataToGameCoordinates(
                project: in project,
                gameObject: ref gameObject);

            projects.SetInputDataFromGameObject(
                project: in project,
                gameObject: in gameObject,
                dataPoint: ref data);

            Assert.IsFalse(data.position == expectedPosition);

            projects.TransformFromGameToDataCoordinates(
                project: in project,
                gameObject: ref gameObject);

            projects.SetInputDataFromGameObject(
                project: in project,
                gameObject: in gameObject,
                dataPoint: ref data);

            Assert.IsTrue(data.position == expectedPosition);

            yield return null;
        }


        [UnityTest]
        public IEnumerator TestOrientationDataCoordinateTransformation()
        {
            var expectedPosition = new Vector3(1.23f, 4.56f, 7.89f);

            var expectedGradient = new Vector3(0.1f, -0.4f, 0.8f).normalized;

            var project = projects.AddProject("foo");

            projects.SetDataToGameCoordinatesConversionParameters(
                project: ref project,
                shift: new Vector3(-0.12f, 3.45f, 6.78f),
                scale: 0.123f);

            var data = new Core.Data.GemPy.DataPoint
            {
                position = new Vector3(
                    expectedPosition.x,
                    expectedPosition.y,
                    expectedPosition.z),
                gradient = new Vector3(
                    expectedGradient.x,
                    expectedGradient.y,
                    expectedGradient.z),
                type = Core.Data.GemPy.InputType.Orientation
            };

            var gameObject = new GameObject("Orientation");

            gameObject.transform.localPosition = new Vector3(
                data.position.x,
                data.position.y,
                data.position.z);

            // Here it gets a bit complicated to mock the spawning.
            // Maybe it'd be better just to do an integration test.
            gameObject.transform.localRotation *= Quaternion.FromToRotation(
                gameObject.transform.forward,
                data.gradient);
            //

            projects.TransformFromDataToGameCoordinates(
                project: in project,
                gameObject: ref gameObject);

            projects.SetInputDataFromGameObject(
                project: in project,
                gameObject: in gameObject,
                dataPoint: ref data);

            Assert.IsFalse(data.position == expectedPosition);

            Assert.IsFalse(data.gradient == expectedGradient);

            projects.TransformFromGameToDataCoordinates(
                project: in project,
                gameObject: ref gameObject);

            projects.SetInputDataFromGameObject(
                project: in project,
                gameObject: in gameObject,
                dataPoint: ref data);

            Assert.IsTrue(data.position == expectedPosition);

            Assert.IsTrue(data.gradient == expectedGradient);

            yield return null;
        }
    }
}
