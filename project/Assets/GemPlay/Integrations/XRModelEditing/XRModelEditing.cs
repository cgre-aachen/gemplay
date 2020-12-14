using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using GemPlay.Modules.Projects;
using GemPlay.Modules.UIUX;
using GemPlay.Integrations.ModelEditing;

namespace GemPlay.Integrations.XRModelEditing
{
    public class XRModelEditingInterface
    {
        readonly ProjectsInterface projects;

        readonly UIUXInterface uiux;

        readonly ModelEditingInterface modelEditing;

        public XRModelEditingInterface(
            in ProjectsInterface projects,
            in UIUXInterface uiux,
            in ModelEditingInterface modelEditing)
        {
            this.projects = projects;

            this.uiux = uiux;

            this.modelEditing = modelEditing;
        }
        
        public void AddMissingInteractionComponentsToProjects()
        {
            foreach (var project in projects.Projects)
            {
                var gameObject = project.GameObject;

                uiux.AddMissingMesoXRInteractionComponents(gameObject: ref gameObject);

                AddMissingMicroInteractionComponents(project: in project);
            }
        }
        
        private void AddMissingMicroInteractionComponents(in Project project)
        {
            var _project = project;

            foreach (Transform transform in project.ModelGameObjects.Input.DataPointsGameObject.transform)
            {
                var gameObject = transform.gameObject;

                uiux.AddMissingMicroXRInteractionComponents(gameObject: ref gameObject);

                var interactable = gameObject.GetComponent<XRGrabInteractable>();

                var modelInputDataComponent = gameObject.GetComponent<Core.Data.GemPy.Components.ModelInputData>();

                var grabManager = new InputGrabManager(
                    gameObject: gameObject,
                    modelInputDataPoint: modelInputDataComponent.dataPoint,
                    grabInteractable: interactable);

                void Grab(XRBaseInteractor interactor)
                {
                    Debug.Log("Grabbed model input game object");

                    interactable.attachTransform = interactor.transform;
                }

                grabManager.Actions["Grab"] += Grab;

                interactable.onSelectEnter.AddListener(grabManager.Actions["Grab"]);


                void Release(XRBaseInteractor interactor)
                {
                    Debug.Log("Released model input game object");

                    Debug.Log("Input game object's forward direction = " + grabManager.GameObject.transform.forward);

                    var tempGameObject = new GameObject("Temp");

                    tempGameObject.transform.localPosition = new Vector3(
                        gameObject.transform.localPosition.x,
                        gameObject.transform.localPosition.y,
                        gameObject.transform.localPosition.z);

                    tempGameObject.transform.localRotation = gameObject.transform.localRotation;

                    projects.TransformFromGameToDataCoordinates(
                        project: in _project,
                        gameObject: ref tempGameObject);

                    projects.SetInputDataFromGameObject(
                        project: in _project,
                        gameObject: in tempGameObject,
                        dataPoint: ref modelInputDataComponent.dataPoint);

                    Object.Destroy(tempGameObject);

                    Debug.Log("Input data's gradient vector = " + grabManager.InputDataPoint.gradient);

                    var modelOutput = new Core.Data.GemPy.Output();

                    if (grabManager.InputDataPoint.type == Core.Data.GemPy.InputType.SurfacePoint)
                    {
                        modelEditing.EditSurfacePoint(
                            modelInputDataPoint: grabManager.InputDataPoint,
                            project: _project);
                    }
                    else if (grabManager.InputDataPoint.type == Core.Data.GemPy.InputType.Orientation)
                    {
                        modelEditing.EditOrientation(
                            modelInputDataPoint: grabManager.InputDataPoint,
                            project: _project);
                    }
                }

                grabManager.Actions["Release"] += Release;

                interactable.onSelectExit.AddListener(grabManager.Actions["Release"]);
            }
        }
    }
}
