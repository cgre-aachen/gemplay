using UnityEngine;
using GemPlay.Modules.Projects;
using GemPlay.Modules.Rest;
using GemPlay.Modules.Spawning;
using GemPlay.Modules.UIUX;
using GemPlay.Integrations.ProjectLoading;
using GemPlay.Integrations.ModelEditing;
using GemPlay.Integrations.XRModelEditing;
using GemPlay.Integrations.GUIEvents;

namespace GemPlay.Applications.Main
{
    public class Main : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Construct the scene hierarchy from scratch with Module game objects and their components")]
        public bool constructSceneDuringAwake = true;

        [SerializeField]
        [Tooltip("Force XR subsystems to stop and deinitialize after startup.")]
        public bool stopXRSubsystemsAfterStart = true;
        // It seems that XR Management is initializing and starting the subsystems when running in the Unity Editor even when this is disabled for PC, Max & Standalone platform.
        // This causes intermittent unexpected behavior and errors.
        // For now, here is a patch to force the expected state just after Start.
        // Unfortunately, we do want XR Management to do the startup for the handheld AR app, since there is an issue with starting/stopping ARCore during runtime.
        // So as of now, this option should be `true` unless building the handheld AR app.


        private bool stoppedXRSubsystemsAfterStart = false;

        private UIUXInterface uiux;

        void Awake()
        {
            var _gameObject = gameObject;

            var coroutineManager = (MonoBehaviour)this;

            // Modules
            var projects = new ProjectsInterface(
                parentGameObject: in _gameObject,
                coroutineManager: in coroutineManager,
                constructScene: in constructSceneDuringAwake);

            var rest = new RestInterface();

            var spawning = new SpawningInterface(
                parentGameObject: in _gameObject,
                coroutineManager: in coroutineManager,
                constructScene: in constructSceneDuringAwake);

            uiux = new UIUXInterface(
                parentGameObject: in _gameObject,
                coroutineManager: in coroutineManager,
                constructScene: in constructSceneDuringAwake);


            // Integrations
            _ = new GUIEventsInterface(
                coroutineManager: in coroutineManager,
                projects: in projects,
                rest: in rest,
                uiux: in uiux,
                loading: new ProjectLoadingInterface(
                    projects: in projects,
                    rest: in rest,
                    spawning: in spawning,
                    uiux: in uiux),
                xrModelEditing: new XRModelEditingInterface(
                    projects: in projects,
                    uiux: in uiux,
                    modelEditing: new ModelEditingInterface(
                        projects: in projects,
                        rest: in rest,
                        spawning: in spawning)));
        }

        private void Update()
        {
            if (stopXRSubsystemsAfterStart && !stoppedXRSubsystemsAfterStart)
            {
                uiux.StopAndDeinitializeXRSubystems();

                stoppedXRSubsystemsAfterStart = true;
            }
        }
    }
};
