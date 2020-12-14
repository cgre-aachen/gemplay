using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;

namespace GemPlay.Modules.UIUX.XR.Components
{
    /// <summary>
    /// This component should be attached to a parent game object containing children that should be affected by XR controller meso scale interactions,
    /// e.g. moving, rotating, or scaling all of the game objects in a project.
    /// </summary>
    /// 
    /// <remarks>
    /// Rotation feels best when the center of the game object's bounds is at its local origin, which is what GemPlay's transformation to game coordiantes enforces anyway.
    /// An old version of this code handled this center and how it relates to zooming and rotating more explicitly.
    /// 
    /// Ideally there would be a way to handle the gestures similar to XR Interaction Toolkit's `ARGestureInteractor`, e.g. a `XRGestureInteractor`.
    /// In that sort of system, a `XRGestureInteractable` would be attached to the parent game object and `XRGestureInteractor` would be attached to the controllers.
    /// This current class and related classes don't directly map to the XRIT Interactable concept.
    /// Still, in practice, this current class is a component that should be attached to what XRIT calls an "interactable".
    /// At some point the current class should be replaced with an XRIT Interactable.
    /// </remarks>
    public class MesoInteractions : MonoBehaviour
    {
        public float distanceBetweenGrippers;

        public Vector3 directionFromFirstToSecondGripper;


        // New organization here
        public Controller leftController, rightController;

        public string leftControllerDeviceName, rightControllerDeviceName;

        public Controller firstGripper, secondGripper;

        public bool leftGripping, rightGripping;

        private int gripCount = 0;

        private int gripCountLastFrame;

        private bool leftGrippingLastFrame, rightGrippingLastFrame;

        private Vector3 firstGripperPositionLastFrame;

        private float distanceLastFrame;

        private Vector3 directionLastFrame;

        private void UpdateDevicesState()
        {
            // So far I haven't had any luck just doing this once rather than during the update cycle
            leftController.SetDeviceFromXRNode();

            rightController.SetDeviceFromXRNode();
            //

            gripCountLastFrame = gripCount;

            leftGrippingLastFrame = leftGripping;

            rightGrippingLastFrame = rightGripping;

            
            distanceLastFrame = distanceBetweenGrippers;

            directionLastFrame = directionFromFirstToSecondGripper;


            leftControllerDeviceName = leftController.Device.name;

            rightControllerDeviceName = rightController.Device.name;


            leftController.Device.TryGetFeatureValue(CommonUsages.gripButton, out leftGripping);

            rightController.Device.TryGetFeatureValue(CommonUsages.gripButton, out rightGripping);


            if (leftGripping && rightGripping)
            {
                gripCount = 2;
            }
            else if (leftGripping || rightGripping)
            {
                gripCount = 1;
            }
            else
            {
                gripCount = 0;
            }


            if (gripCount == 0)
            {
                firstGripper = null;

                secondGripper = null;
            }
            else if (gripCount == 1)
            {
                secondGripper = null;

                if (leftGripping)
                {
                    firstGripper = leftController;
                }
                else if (rightGripping)
                {
                    firstGripper = rightController;
                }
                else
                {
                    Debug.LogError("Invalid controller grip state.");
                }                
            }
            else if (gripCount == 2)
            {
                if (gripCountLastFrame == 0)
                {
                    firstGripper = leftController;

                    secondGripper = rightController;
                }
                else if (gripCountLastFrame == 1)
                {
                    if (leftGripping && !leftGrippingLastFrame)
                    {
                        secondGripper = leftController;
                    }
                    else if (rightGripping && !rightGrippingLastFrame)
                    {
                        secondGripper = rightController;
                    }
                    else
                    {
                        Debug.LogError("Invalid controller grip state.");
                    }
                }
                else if (gripCountLastFrame == 2)
                {
                    // Nothing to do here
                }
                else
                {
                    Debug.LogError("Invalid controller grip state.");
                }
            }
            else
            {
                Debug.LogError("Invalid controller grip state.");
            }
            
            if (secondGripper != null)
            {
                distanceBetweenGrippers = Vector3.Distance(
                    firstGripper.GameObject.transform.position,
                    secondGripper.GameObject.transform.position);

                directionFromFirstToSecondGripper = (secondGripper.GameObject.transform.position - firstGripper.GameObject.transform.position).normalized;
            }
        }

        private void Update()
        {
            UpdateDevicesState();
            

            if (gripCount == 0)
            {
                // Do nothing.
            }
            else if ((gripCount == 1) && (gripCountLastFrame != 1))
            {
                // Do nothing until the grip is held for a frame.
            }
            else if ((gripCount == 2) && (gripCountLastFrame != 2))
            {
                // Do nothing until the grip is held for a frame.
            }
            else if ((gripCount == 1) && (gripCountLastFrame == 1))
            {
                transform.position += firstGripper.GameObject.transform.position - firstGripperPositionLastFrame;
            }
            else if ((gripCount == 2) && (gripCountLastFrame == 2))
            {
                transform.Rotate(
                    axis: Vector3.up,
                    angle: Vector3.SignedAngle(
                        from: directionLastFrame,
                        to: directionFromFirstToSecondGripper,
                        axis: Vector3.up));

                var _transform = transform;

                Core.Helpers.Unity.GameObjects.SetLocalScaleFromGlobal(
                    ref _transform,
                    transform.lossyScale * (distanceBetweenGrippers / distanceLastFrame));
            }
            else
            {
                Debug.LogError("Invalid controller grip state.");
            }


            if (firstGripper != null)
            {
                firstGripperPositionLastFrame = firstGripper.GameObject.transform.position;
            }
        }
    }
}
