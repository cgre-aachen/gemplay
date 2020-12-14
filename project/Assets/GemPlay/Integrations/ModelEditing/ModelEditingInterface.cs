    using GemPlay.Modules.Projects;
using GemPlay.Modules.Rest;
using GemPlay.Modules.Spawning;

namespace GemPlay.Integrations.ModelEditing
{
    public class ModelEditingInterface
    {
        readonly ProjectsInterface projects;

        readonly RestInterface rest;

        readonly SpawningInterface spawning;

        public ModelEditingInterface(
            in ProjectsInterface projects,
            in RestInterface rest,
            in SpawningInterface spawning)
        {
            this.projects = projects;

            this.rest = rest;

            this.spawning = spawning;
        }

        public RSG.IPromise EditSurfacePoint(
            in Core.Data.GemPy.DataPoint modelInputDataPoint,
            in Project project)
        {
            var _project = project;

            return rest.EditModelSurfacePoint(
                geoModelUrn: project.Guid,
                dataPoint: modelInputDataPoint).
            Then(() =>
            {
                return AfterPostingEdit(_project);
                
            });
        }

        public RSG.IPromise EditOrientation(
            in Core.Data.GemPy.DataPoint modelInputDataPoint,
            in Project project)
        {
            var _project = project;

            return rest.EditModelOrientation(
                geoModelUrn: project.Guid,
                dataPoint: modelInputDataPoint).
            Then(() =>
            {
                return AfterPostingEdit(_project);
            });
        }

        private RSG.IPromise AfterPostingEdit(in Project project)
        {
            var modelOutput = new Core.Data.GemPy.Output();

            var _project = project;

            return rest.GetModelOutput(geoModelUrn: project.Guid).
            Then(_modelOutput =>
            {
                modelOutput = _modelOutput;

                return spawning.DestroyModelOutput(outputContainer: _project.ModelGameObjects.Output);
            })
            .Then(() =>
            {
                return spawning.SpawnModelOutput(
                    outputData: modelOutput,
                    outputContainer: _project.ModelGameObjects.Output);
            })
            .Then(() =>
            {
                return projects.CorrectRexMeshCoordinatesFromGemPyWriter(ref _project);
            })
            .Then(() =>
            {
                projects.TransformAllOutputGameObjectsToGameOriginAndScale(ref _project);
            });
        }
    }
}
