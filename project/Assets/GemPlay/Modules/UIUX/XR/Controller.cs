using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace GemPlay.Modules.UIUX.XR
{
    public class Controller
    {
        public GameObject GameObject { get; }

        public UnityEngine.XR.Interaction.Toolkit.XRController XRITController { get; }

        public InputDevice Device { get; private set; }

        public XRNode XRNode { get; }

        public Controller(
            XRNode node,
            string gameObjectName,
            bool constructScene)
        {
            XRNode = node;

            if (constructScene)
            {
                GameObject = new GameObject(gameObjectName);

                XRITController = GameObject.AddComponent<UnityEngine.XR.Interaction.Toolkit.XRController>();

                XRITController.selectUsage = InputHelpers.Button.Trigger;

                XRITController.controllerNode = node;
            }
            else
            {
                GameObject = GameObject.Find(gameObjectName);

                XRITController = GameObject.GetComponent<UnityEngine.XR.Interaction.Toolkit.XRController>();

                if (XRITController.controllerNode != node)
                {
                    Debug.LogError("Expected " + node + " but found " + XRITController.controllerNode);
                }
            }
        }

        public void SetDeviceFromXRNode()
        {
            var devices = new List<InputDevice>();

            InputDevices.GetDevicesAtXRNode(XRNode, devices);

            Device = devices.FirstOrDefault();
        }
    }

    public class ControllerWithRayInteractor : Controller
    {
        public XRRayInteractor XRRayInteractor { get; }

        public LineRenderer LineRenderer { get; }

        public XRInteractorLineVisual XRInteractorLineVisual { get; }

        public ControllerWithRayInteractor(
            XRInteractionManager interactionManager,
            XRNode node,
            string gameObjectName,
            bool constructScene) 
            : base(node: node, gameObjectName: gameObjectName, constructScene: constructScene)
        {
            if (constructScene)
            {
                LineRenderer = GameObject.AddComponent<LineRenderer>();

                LineRenderer.numCornerVertices = 4;

                LineRenderer.numCapVertices = 4;

                string lineMaterialResourcePath = "GemPlay/Materials/XRRaycast";

                var material = Resources.Load(lineMaterialResourcePath) as Material;

                if (material == null)
                {
                    Debug.LogError("Found nothing at " + lineMaterialResourcePath);
                }

                LineRenderer.material = material;

                LineRenderer.sortingOrder = 5;


                XRRayInteractor = GameObject.AddComponent<XRRayInteractor>();


                XRInteractorLineVisual = GameObject.AddComponent<XRInteractorLineVisual>();

                XRInteractorLineVisual.lineWidth = 0.005f;

                XRInteractorLineVisual.lineLength = 2.0f;

                var invalidGradientColorKey = new GradientColorKey[2];

                invalidGradientColorKey[0].color = Color.white;

                invalidGradientColorKey[1].color = Color.white;

                var alphaKey = new GradientAlphaKey[2];

                alphaKey[0].alpha = 1.0f;

                alphaKey[0].time = 0.0f;

                alphaKey[1].alpha = 0.0f;

                alphaKey[1].time = 1.0f;

                XRInteractorLineVisual.invalidColorGradient.SetKeys(invalidGradientColorKey, alphaKey);

                var validGradientColorKey = new GradientColorKey[2];

                validGradientColorKey[0].color = Color.green;

                validGradientColorKey[1].color = Color.green;

                var validGradient = new Gradient();

                validGradient.SetKeys(validGradientColorKey, alphaKey);

                XRInteractorLineVisual.validColorGradient.SetKeys(validGradientColorKey, alphaKey);
            }
            else
            {
                LineRenderer = GameObject.GetComponent<LineRenderer>();

                XRRayInteractor = GameObject.GetComponent<XRRayInteractor>();

                XRInteractorLineVisual = GameObject.GetComponent<XRInteractorLineVisual>();
            }
            
            XRRayInteractor.interactionManager = interactionManager;
        }
    }
}
