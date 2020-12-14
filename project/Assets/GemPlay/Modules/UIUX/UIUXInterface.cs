using System.Collections;
using UnityEngine;

namespace GemPlay.Modules.UIUX
{
    struct CameraData
    {
        public Vector3 position;

        public Vector3 eulerAngles;
    }

    enum Layer
    {
        AR = 8
    }

    ///<summary>
    /// This <c>UIUXInterface</c> class is the interface to GemPlay's UI/UX functionality.
    ///</summary>
    public class UIUXInterface
    {
        public GameObject GameObject { get; }

        public UnityEngine.UIElements.EventSystem GUIEventSystem { get; }

        public UnityEngine.UIElements.VisualElement GUITreeRoot { get; }

        public Camera VirtualCamera { get; }

        public Camera ARCamera { get; }

        public Camera XRCamera { get; }

        public Transform ARSessionOriginTransform
        { get => ARContainer.SessionOrigin.transform; }

        public UnityEngine.XR.Interaction.Toolkit.XRInteractionManager XRInteractionManager
        { get => XRContainer.InteractionManager; }

        public bool AREnabled { get; private set; }

        public bool XREnabled { get; private set; }

        private readonly CameraData virtualCameraDataAtStart;

        private AR.Container ARContainer { get; }

        private XR.Container XRContainer { get; }

        private readonly MonoBehaviour coroutineManager;

        public UIUXInterface(
            in GameObject parentGameObject,
            in MonoBehaviour coroutineManager,
            in bool constructScene)
        {
            this.coroutineManager = coroutineManager;

            if (constructScene)
            {
                GameObject = new GameObject("UI/UX");

                GameObject.transform.SetParent(parentGameObject.transform);

                var lightsGameObject = new GameObject("Lights");

                lightsGameObject.transform.SetParent(GameObject.transform);

                var directionalLightGameObject = new GameObject("Directional Light");

                directionalLightGameObject.transform.SetParent(lightsGameObject.transform);

                directionalLightGameObject.transform.SetPositionAndRotation(
                    position: new Vector3(0, 3, 0),
                    rotation: Quaternion.Euler(new Vector3(50, -30, 0)));

                var light = directionalLightGameObject.AddComponent<Light>();

                light.type = LightType.Directional;

                light.shadows = LightShadows.Soft;

                var virtualCameraGameObject = new GameObject("Virtual Camera");

                virtualCameraGameObject.transform.SetParent(GameObject.transform);

                VirtualCamera = virtualCameraGameObject.AddComponent<Camera>();

                VirtualCamera.tag = "MainCamera";

                VirtualCamera.cullingMask = VirtualCamera.cullingMask & ~(1 << (int)Layer.AR);  // Hide the AR layer

                virtualCameraGameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();

                VirtualCamera.nearClipPlane = 0.1f;


                var guiDocumentComponent = GameObject.AddComponent<UnityEngine.UIElements.UIDocument>();

                guiDocumentComponent.panelSettings = Resources.Load("GemPlay/ScriptableObjects/PanelSettings") as UnityEngine.UIElements.PanelSettings;

                guiDocumentComponent.visualTreeAsset = Resources.Load("GemPlay/UXML/Screen") as UnityEngine.UIElements.VisualTreeAsset;

                GUITreeRoot = guiDocumentComponent.rootVisualElement;

                GUIEventSystem = GameObject.GetComponent<UnityEngine.UIElements.EventSystem>();


                GameObject.AddComponent<UnityEngine.UIElements.EventSystem>();


                var cameraMovementComponent = GameObject.AddComponent<Mouse.CameraMovement>();

                cameraMovementComponent.cameraGameObject = Camera.main.gameObject;
            }
            else
            {
                GameObject = GameObject.Find("UI/UX");

                var virtualCameraGameObject = GameObject.Find("Virtual Camera");

                VirtualCamera = virtualCameraGameObject.GetComponent<Camera>();

                GUITreeRoot = GameObject.GetComponent<UnityEngine.UIElements.UIDocument>().rootVisualElement;

                GUIEventSystem = GameObject.GetComponent<UnityEngine.UIElements.EventSystem>();
            }


            virtualCameraDataAtStart = new CameraData
            {
                position = Camera.main.transform.position,
                eulerAngles = Camera.main.transform.eulerAngles
            };
            

            // XR
            XRContainer = new XR.Container(constructScene: constructScene);

            if (constructScene)
            {
                XRContainer.GameObject.transform.SetParent(GameObject.transform);
            }

            XRCamera = XRContainer.Camera;

            
            // AR
            ARContainer = new AR.Container(
                interactionManager: XRContainer.InteractionManager,
                constructScene: constructScene);

            if (constructScene)
            {
                ARContainer.GameObject.transform.SetParent(GameObject.transform);
            }
            
            ARCamera = ARContainer.Camera;


            // Mobile console
            Mobile.Console.Setup();


            // Set initial state
            DisableAR();

            DisableXR();
        }

        public void ResetCamera()
        {
            Camera.main.transform.position = virtualCameraDataAtStart.position;

            Camera.main.transform.eulerAngles = virtualCameraDataAtStart.eulerAngles;
        }

        public RSG.IPromise InitializeAndStartXRSubsystems()
        {
            var promise = new RSG.Promise();

            IEnumerator Coroutine()
            {
                Debug.Log("Initializing XR");

                yield return UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.InitializeLoader();

                if (UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.activeLoader == null)
                {
                    Debug.LogError("XR initialization failed");

                    promise.Reject(new System.Exception("XR initialization failed"));
                }
                else
                {
                    UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StartSubsystems();

                    promise.Resolve();
                }
            }

            coroutineManager.StartCoroutine(Coroutine());

            return promise;
        }

        public void StopAndDeinitializeXRSubystems()
        {
            if (!(UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager == null)
                &&UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.isInitializationComplete)
            {
                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.StopSubsystems();

                UnityEngine.XR.Management.XRGeneralSettings.Instance.Manager.DeinitializeLoader();
            }
        }

        public void EnableAR()
        {// Assuming XR Plug-in Management already initialized and started XR subsystems during start.
            ARContainer.SetEnabled(true);

            Debug.Log("AR enabled.");
        }

        public void DisableAR()
        {
            ARContainer.SetEnabled(false);

            Debug.Log("AR disabled.");
        }

        public void AddMissingARComponents(ref GameObject gameObject)
        {
            var boxCollider = gameObject.GetComponent<BoxCollider>();

            if (boxCollider == null)
            {
                Core.Helpers.Unity.GameObjects.AddBoxColliderContainingAllMeshes(container: ref gameObject);
            }

            AR.Interactions.AddMissingInteractableComponents(gameObject: ref gameObject).ForEach(component =>
            {
                component.interactionManager = XRContainer.InteractionManager;
            });
        }

        public void EnableXR()
        {
            SetXREnabled(true);

            XRContainer.Rig.transform.SetPositionAndRotation(
                position: VirtualCamera.transform.position,
                rotation: VirtualCamera.transform.rotation);

            Debug.Log("XR Enabled.");
        }

        public void DisableXR()
        {
            SetXREnabled(false);

            Debug.Log("XR disabled.");
        }

        private void SetXREnabled(bool enabled)
        {
            XREnabled = enabled;

            XRContainer.SetEnabled(enabled);
        }

        public void AddMissingMesoXRInteractionComponents(ref GameObject gameObject)
        {
            XRContainer.AddMissingMesoInteractionComponents(ref gameObject);
        }

        public void AddMissingMicroXRInteractionComponents(ref GameObject gameObject)
        {
            XRContainer.AddMissingMicroInteractionComponents(ref gameObject);
        }

        public void ShowConsole()
        {
            Mobile.Console.Show();
        }

        public void HideConsole()
        {
            Mobile.Console.Hide();
        }

        public void VirtualCameraMovement(in bool enabled)
        {
            GameObject.GetComponent<Mouse.CameraMovement>().enabled = enabled;
        }
    }
}
