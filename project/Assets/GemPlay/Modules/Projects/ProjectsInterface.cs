using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GemPlay.Modules.Projects
{
    ///<summary>
    /// This <c>ProjectsInterface</c> class is the interface to GemPlay's Projects functionality.
    ///</summary>
    public class ProjectsInterface
    {
        public GameObject GameObject { get; }

        public Project SelectedProject { get; private set; }

        public List<Project> Projects { get; private set; }

        readonly private MonoBehaviour coroutineManager;

        public ProjectsInterface(
            in GameObject parentGameObject,
            in MonoBehaviour coroutineManager,
            in bool constructScene)
        {
            if (constructScene)
            {
                GameObject = new GameObject("Projects");

                GameObject.transform.SetParent(parentGameObject.transform);
            }
            else
            {
                GameObject = GameObject.Find("Projects");
            }
            
            Projects = new List<Project>();

            this.coroutineManager = coroutineManager;
        }

        public Project AddProject(in string guid)
        {
            var project = new Project(guid: guid);

            project.GameObject.transform.SetParent(GameObject.transform);

            Projects.Add(project);

            SelectProject(project);

            return project;
        }

        public RSG.IPromise<Bounds> Bounds(in Project project)
        {
            var promise = new RSG.Promise<Bounds>();

            var gameObject = project.GameObject;

            IEnumerator Coroutine()
            {
                promise.Resolve(Core.Helpers.Unity.GameObjects.BoundsOfAllMeshes(container: in gameObject));

                yield return null;
            }

            coroutineManager.StartCoroutine(Coroutine());

            return promise;
        }

        public void SetDataToGameCoordinatesConversionParameters(
            ref Project project,
            in Vector3 shift,
            in float scale)
        {
            project.Shift = shift;

            project.Scale = scale;
        }

        public RSG.IPromise CorrectRexMeshCoordinatesFromGemPyWriter(ref Project project)
        {
            var promise = new RSG.Promise();

            IEnumerator Coroutine(Project _project)
            {
                _project.CorrectRexMeshCoordinatesFromGemPyWriter();

                promise.Resolve();

                yield return null;
            }

            coroutineManager.StartCoroutine(Coroutine(project));

            return promise;
        }

        public RSG.IPromise TransformAllGameObjectsFromDataToGameCoordinates(ref Project project)
        {
            return RSG.Promise.All(
                TransformAllOutputGameObjectsToGameOriginAndScale(ref project),
                TransformAllInputGameObjectsFromDataToGameCoordinates(ref project));
        }
        
        public RSG.IPromise TransformAllOutputGameObjectsToGameOriginAndScale(ref Project project)
        {
            var promise = new RSG.Promise();

            IEnumerator Coroutine(Project _project)
            {
                _project.TransformAllOutputGameObjectsToGameOriginAndScale();

                promise.Resolve();

                yield return null;
            }

            coroutineManager.StartCoroutine(Coroutine(project));

            return promise;
        }

        public RSG.IPromise TransformAllInputGameObjectsFromDataToGameCoordinates(ref Project project)
        {
            var promise = new RSG.Promise();

            IEnumerator Coroutine(Project _project)
            {
                _project.TransformAllInputGameObjectsFromDataToGameCoordinates();

                promise.Resolve();

                yield return null;
            }

            coroutineManager.StartCoroutine(Coroutine(project));

            return promise;
        }

        public void TransformFromDataToGameCoordinates(
            in Project project,
            ref GameObject gameObject)
        {
            project.TransformFromDataToGameCoordinates(transform: gameObject.transform);
        }

        public void TransformFromGameToDataCoordinates(
            in Project project,
            ref GameObject gameObject)
        {
            project.TransformFromGameToDataCoordinates(transform: gameObject.transform);
        }

        public void SetInputDataFromGameObject(
            in Project project,
            in GameObject gameObject,
            ref Core.Data.GemPy.DataPoint dataPoint)
        {
            project.SetInputDataFromGameObject(
                gameObject: in gameObject,
                dataPoint: ref dataPoint);
        }

        public void SelectProject(in Project project)
        {
            if (!Projects.Contains(project))
            {
                Debug.LogError("Cannot select project " + project.Guid 
                    + " because it is not in the projects list.");
            }

            SelectedProject = project;
        }
    }
}
