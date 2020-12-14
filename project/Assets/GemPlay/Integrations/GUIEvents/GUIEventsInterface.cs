using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using GemPlay.Modules.Rest;
using GemPlay.Modules.Projects;
using GemPlay.Modules.UIUX;
using GemPlay.Integrations.ProjectLoading;
using GemPlay.Integrations.XRModelEditing;

namespace GemPlay.Integrations.GUIEvents
{
    public class GUIEventsInterface
    {
        readonly MonoBehaviour coroutineManager;

        readonly ProjectsInterface projects;

        readonly RestInterface rest;

        readonly UIUXInterface uiux;

        readonly ProjectLoadingInterface loading;

        readonly XRModelEditingInterface xrModelEditing;

        public GUIEventsInterface(
            in MonoBehaviour coroutineManager,
            in ProjectsInterface projects,
            in RestInterface rest,
            in UIUXInterface uiux,
            in ProjectLoadingInterface loading,
            in XRModelEditingInterface xrModelEditing)
        {
            this.coroutineManager = coroutineManager;

            this.projects = projects;

            this.rest = rest;

            this.uiux = uiux;

            this.loading = loading;

            this.xrModelEditing = xrModelEditing;

            Register();
        }

        public void Register()
        {
            var guiOpenRegion = uiux.GUITreeRoot.Q<VisualElement>("open-region");

            uiux.GUITreeRoot.Q<Button>("debug").clicked += () => EnterDebugMode();


            uiux.GUITreeRoot.Q<Button>("reset-camera").clicked += () => ResetCamera();
            

            var openProjectButton = uiux.GUITreeRoot.Q<Button>("open-project");

            openProjectButton.clicked += () => coroutineManager.StartCoroutine(
                OpenProject(
                    button: openProjectButton,
                    parentOfProjectListView: guiOpenRegion));


            var toggleARButton = uiux.GUITreeRoot.Q<Button>("toggle-ar");

            toggleARButton.clicked += () => ToggleAR();


            var toggleXRButton = uiux.GUITreeRoot.Q<Button>("toggle-xr");

            toggleXRButton.clicked += () => ToggleXR();    
        }

        private void ResetCamera()
        {
            uiux.ResetCamera();
        }

        private IEnumerator OpenProject(
            Button button,
            VisualElement parentOfProjectListView)
        {
            button.SetEnabled(false);

            bool waiting = true;

            rest.GetModelNames().
            Then(names =>
            {
                MakeOpenProjectGUI(
                    projectNames: names,
                    button: button,
                    parentOfListView: parentOfProjectListView,
                    itemHeight: 16);

                waiting = false;
            });

            while (waiting)
            {
                yield return null;
            }
        }

        private VisualElement MakeLabel() => new Label();

        private void MakeOpenProjectGUI(
            string[] projectNames,
            Button button,
            VisualElement parentOfListView,
            int itemHeight)
        {
            void bindItem(VisualElement e, int i) => 
                (e as Label).text = projectNames[i];

            var listView = new ListView(
                itemsSource: projectNames,
                itemHeight: itemHeight, 
                makeItem: MakeLabel,
                bindItem: bindItem);

            parentOfListView.Add(listView);

            var cancelButton = new Button
            {
                text = "Cancel"
            };

            cancelButton.clicked += () => CancelOpeningProject(
                openProjectButton: button,
                projectListView: listView,
                cancelButton: cancelButton);

            parentOfListView.Add(cancelButton);

            listView.style.flexGrow = 1.0f;

            listView.onItemsChosen += objects => OpenProjectStep2(
                openProjectButton: button,
                cancelButton: cancelButton,
                projectName: (string)objects.FirstOrDefault(),
                listView: listView);
        }

        private static void CancelOpeningProject(
            Button openProjectButton,
            ListView projectListView,
            Button cancelButton)
        {
            projectListView.RemoveFromHierarchy();

            cancelButton.RemoveFromHierarchy();

            openProjectButton.SetEnabled(true);
        }

        private void OpenProjectStep2(
            Button openProjectButton,
            Button cancelButton,
            string projectName,
            ListView listView)
        {
            loading.Load(projectGuid: projectName)
            .Then(project =>
            {
                openProjectButton.SetEnabled(true);
            })
            .Catch(error =>
            {
                Debug.LogError(error.Message + "\n" + error.StackTrace);

                throw error;
            });

            listView.RemoveFromHierarchy();

            cancelButton.RemoveFromHierarchy();
        }


        private void EnterDebugMode()
        {
            uiux.ShowConsole();
        }

        private void ToggleAR()
        {
            if (uiux.AREnabled)
            {
                DisableAR();
            }
            else
            {
                EnableAR();
            }
        }
        
        private void EnableAR()
        {
            uiux.EnableAR();

            foreach (var project in projects.Projects)
            {
                var gameObject = project.GameObject;

                uiux.AddMissingARComponents(gameObject: ref gameObject);
            }

            uiux.VirtualCamera.enabled = false;

            uiux.VirtualCameraMovement(enabled: false);
        }

        private void DisableAR()
        {
            uiux.DisableAR();

            uiux.VirtualCamera.enabled = true;

            uiux.VirtualCameraMovement(enabled: true); 
        }

        private void ToggleXR()
        {
            if (uiux.XREnabled)
            {
                DisableXR();
            }
            else
            {
                EnableXR();
            }
        }

        private void EnableXR()
        {
            xrModelEditing.AddMissingInteractionComponentsToProjects();

            uiux.InitializeAndStartXRSubsystems()
            .Then(() =>
            {
                uiux.EnableXR();

                uiux.VirtualCamera.enabled = false;

                uiux.VirtualCameraMovement(enabled: false);
            });
        }

        private void DisableXR()
        {
            uiux.DisableXR();

            uiux.StopAndDeinitializeXRSubystems();

            uiux.VirtualCamera.enabled = true;

            uiux.VirtualCameraMovement(enabled: true);
        }
    }
}
