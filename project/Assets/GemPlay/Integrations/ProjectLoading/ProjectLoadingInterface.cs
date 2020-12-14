using UnityEngine;
using GemPlay.Modules.Projects;
using GemPlay.Modules.Rest;
using GemPlay.Modules.Spawning;
using GemPlay.Modules.UIUX;

namespace GemPlay.Integrations.ProjectLoading
{
    public class ProjectLoadingInterface
    {
        readonly ProjectsInterface projects;

        readonly RestInterface rest;

        readonly SpawningInterface spawning;

        readonly UIUXInterface uiux;

        public ProjectLoadingInterface(
            in ProjectsInterface projects,
            in RestInterface rest,
            in SpawningInterface spawning,
            in UIUXInterface uiux)
        {
            this.projects = projects;

            this.rest = rest;

            this.spawning = spawning;

            this.uiux = uiux;
        }

        public RSG.IPromise<Project> Load(
            in string projectGuid,
            in float gameObjectBoundsSize = 1.0f)
        {
            var promise = new RSG.Promise<Project>();

            var project = projects.AddProject(guid: projectGuid);

            var projectGameObject = project.GameObject;

            var _projectGuid = projectGuid;

            var _gameObjectBoundsSize = gameObjectBoundsSize;

            rest.LoadModel(geoModelUrn: projectGuid)
            .Then(() =>
            {
                return rest.GetModelOutput(geoModelUrn: _projectGuid);
            })
            .Then(modelOutput =>
            {
                project.ModelData.Output = modelOutput;

                return rest.GetModelInput(geoModelUrn: _projectGuid);
            })
            .Then(modelInput =>
            {
                project.ModelData.Input = modelInput;

                return RSG.Promise.All(
                    spawning.SpawnModelOutput(
                        outputData: project.ModelData.Output,
                        outputContainer: project.ModelGameObjects.Output),
                    spawning.SpawnModelInput(
                        inputData: project.ModelData.Input,
                        inputContainer: project.ModelGameObjects.Input));
            })
            .Then(() =>
            {
                return projects.CorrectRexMeshCoordinatesFromGemPyWriter(ref project);
            })
            .Then(() =>
            {
                return projects.Bounds(project);
            })
            .Then(bounds =>
            {
                var scale = _gameObjectBoundsSize / bounds.size.magnitude;

                projects.SetDataToGameCoordinatesConversionParameters(
                    project: ref project,
                    shift: new Vector3 {
                        x = -bounds.center.x,
                        y = -bounds.center.y,
                        z = -bounds.center.z},
                    scale: in scale);

                return projects.TransformAllGameObjectsFromDataToGameCoordinates(ref project);
            })
            .Then(() =>
            {
                uiux.VirtualCamera.transform.position = new Vector3(0, 0, -1);

                if (uiux.AREnabled)
                {
                    uiux.AddMissingARComponents(gameObject: ref projectGameObject);
                }

                // @todo Once projects can be loaded in XR mode, need to add something similar to what was done for AR here.
                
                promise.Resolve(project);
            })
            .Catch(error =>
            {
                Debug.LogError(error.Message + "\n" + error.StackTrace);

                throw error;
            });

            return promise;
        }
    }
}
