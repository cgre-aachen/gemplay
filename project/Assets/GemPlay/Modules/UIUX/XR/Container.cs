using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SpatialTracking;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace GemPlay.Modules.UIUX.XR
{
    public class Container
    {
        public GameObject GameObject { get; }

        public UnityEngine.EventSystems.EventSystem XREventSystem { get; }

        public XRInteractionManager InteractionManager { get; }

        public XRRig Rig { get; }

        public Camera Camera { get; }

        public TrackedPoseDriver TrackedPoseDriver { get; }

        public ControllerWithRayInteractor LeftController { get; }

        public ControllerWithRayInteractor RightController { get; }

        public Container(in bool constructScene)
        {
            if (constructScene)
            {
                GameObject = new GameObject("XR");

                InteractionManager = GameObject.AddComponent<XRInteractionManager>();

                XREventSystem = GameObject.AddComponent<UnityEngine.EventSystems.EventSystem>();

                var rigGameObject = new GameObject("Rig");

                Rig = rigGameObject.AddComponent<XRRig>();

                Rig.rig = Rig.gameObject;

                Rig.cameraFloorOffsetObject = Rig.gameObject;  // XRRig.Awake still throws a warning about this not being set because it's dumb.

                Rig.cameraYOffset = 0.0f;

                Rig.transform.SetParent(GameObject.transform);


                var cameraGameObject = new GameObject("Camera");

                cameraGameObject.transform.SetParent(Rig.transform);

                Camera = cameraGameObject.AddComponent<Camera>();

                Camera.tag = "MainCamera";

                Camera.clearFlags = CameraClearFlags.Skybox;

                Camera.nearClipPlane = 0.1f;

                TrackedPoseDriver = cameraGameObject.AddComponent<TrackedPoseDriver>();

                cameraGameObject.AddComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();


                Rig.cameraGameObject = cameraGameObject;
            }
            else
            {
                GameObject = GameObject.Find("XR");

                InteractionManager = GameObject.GetComponent<XRInteractionManager>();

                Rig = GameObject.GetComponentInChildren<XRRig>();

                Camera = GameObject.GetComponentInChildren<Camera>();

                TrackedPoseDriver = Camera.gameObject.GetComponent<TrackedPoseDriver>();

                XREventSystem = GameObject.GetComponent<UnityEngine.EventSystems.EventSystem>();
            }
            
            LeftController = new ControllerWithRayInteractor(
                interactionManager: InteractionManager,
                node: XRNode.LeftHand,
                gameObjectName: "Left Controller",
                constructScene: constructScene);

            RightController = new ControllerWithRayInteractor(
                interactionManager: InteractionManager,
                node: XRNode.RightHand,
                gameObjectName: "Right Controller",
                constructScene: constructScene);

            if (constructScene)
            {
                LeftController.GameObject.transform.SetParent(Rig.transform);

                RightController.GameObject.transform.SetParent(Rig.transform);
            }
        }
 
        public void AddMissingMesoInteractionComponents(ref GameObject gameObject)
        {
            var component = gameObject.GetComponent<Components.MesoInteractions>();

            if (component == null)
            {
                component = gameObject.AddComponent<Components.MesoInteractions>();
            }

            component.leftController = LeftController;

            component.rightController = RightController;
        }

        public void AddMissingMicroInteractionComponents(ref GameObject gameObject)
        {
            var interactable = gameObject.GetComponent<XRGrabInteractable>();

            if (interactable == null)
            {
                interactable = gameObject.AddComponent<XRGrabInteractable>();
            }

            interactable.interactionManager = InteractionManager;

            interactable.trackPosition = true;

            interactable.trackRotation = true;

            interactable.throwOnDetach = false;


            var rescaler = gameObject.GetComponent<Components.Rescaler>();

            if (rescaler == null)
            {
                gameObject.AddComponent<Components.Rescaler>();
            }
        }

        public void SetEnabled(bool enabled)
        {
            if (enabled)
            {
                foreach (var controller in new List<ControllerWithRayInteractor> { LeftController, RightController })
                {
                    controller.SetDeviceFromXRNode();
                }
            }
            
            Camera.enabled = enabled;

            TrackedPoseDriver.enabled = enabled;

            Rig.enabled = enabled;

            foreach (var controller in new List<ControllerWithRayInteractor> { LeftController, RightController})
            {
                controller.XRInteractorLineVisual.enabled = enabled;

                controller.XRRayInteractor.enabled = enabled;

                controller.XRITController.enabled = enabled;
            }
        }
    }
}
