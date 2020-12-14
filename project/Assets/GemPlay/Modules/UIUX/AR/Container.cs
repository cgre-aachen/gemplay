using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace GemPlay.Modules.UIUX.AR
{
    public class Container
    {
        public GameObject GameObject { get; }

        public ARSession Session { get; }

        public ARSessionOrigin SessionOrigin { get; }

        public ARCameraBackground CameraBackground { get; }

        public ARGestureInteractor GestureInteractor { get; }

        public Camera Camera { get; }

        public Container(
            XRInteractionManager interactionManager,
            bool constructScene)
        {
            if (constructScene)
            {
                GameObject = new GameObject("AR");

                var aRSessionGameObject = new GameObject("Session");

                aRSessionGameObject.transform.SetParent(GameObject.transform);

                Session = aRSessionGameObject.AddComponent<ARSession>();

                aRSessionGameObject.AddComponent<ARInputManager>();


                var aRSessionOriginGameObject = new GameObject("Session Origin");

                aRSessionOriginGameObject.transform.SetParent(GameObject.transform);

                SessionOrigin = aRSessionOriginGameObject.AddComponent<ARSessionOrigin>();


                var aRCameraGameObject = new GameObject("Camera");

                aRCameraGameObject.transform.SetParent(aRSessionOriginGameObject.transform);

                Camera = aRCameraGameObject.AddComponent<Camera>();

                Camera.tag = "MainCamera";

                Camera.clearFlags = CameraClearFlags.SolidColor;

                Camera.backgroundColor = Color.black;

                aRCameraGameObject.AddComponent<ARCameraManager>();

                CameraBackground = aRCameraGameObject.AddComponent<ARCameraBackground>();

                aRCameraGameObject.AddComponent<ARPoseDriver>();

                GestureInteractor = aRCameraGameObject.AddComponent<ARGestureInteractor>();

                GestureInteractor.interactionManager = interactionManager;

                aRCameraGameObject.AddComponent<AudioListener>();

                aRCameraGameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
                

                SessionOrigin.camera = Camera;

                SessionOrigin.gameObject.AddComponent<ARRaycastManager>();


                var planeManager = SessionOrigin.gameObject.AddComponent<ARPlaneManager>();

                // @todo Create planePrefab in code instead of loading .prefab resource
                planeManager.planePrefab = Resources.Load("GemPlay/Prefabs/ARPlane") as GameObject;

                planeManager.detectionMode = UnityEngine.XR.ARSubsystems.PlaneDetectionMode.Horizontal;
            }
            else
            {
                GameObject = GameObject.Find("AR");

                Session = GameObject.GetComponentInChildren<ARSession>();

                SessionOrigin = GameObject.GetComponentInChildren<ARSessionOrigin>();

                Camera = GameObject.GetComponentInChildren<Camera>();

                CameraBackground = GameObject.GetComponentInChildren<ARCameraBackground>();

                GestureInteractor = GameObject.GetComponentInChildren<ARGestureInteractor>();
            }

            GestureLogger.RegisterGestureLoggingEvents(gestureInteractor: GestureInteractor);
        }

        public void SetEnabled(bool enabled)
        {
            Camera.enabled = enabled;

            GestureInteractor.enabled = enabled;
        }
    }
}
