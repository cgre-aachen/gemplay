using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.AR;

namespace GemPlay.Modules.UIUX.AR
{
    public static class Interactions
    {
        public static List<XRBaseInteractable> AddMissingInteractableComponents(ref GameObject gameObject)
        {
            var componentTypes = new List<System.Type>() { 
                typeof(ARSelectionInteractable),
                typeof(ARScaleInteractable),
                typeof(ARRotationInteractable),
                typeof(ARTranslationInteractable)};

            var missingComponents = new List<XRBaseInteractable>();

            var _gameObject = gameObject;

            componentTypes.ForEach(componentType =>
            {
                if (_gameObject.GetComponent(type: componentType) == null)
                {
                    var component = (XRBaseInteractable)_gameObject.AddComponent(componentType: componentType);

                    if (componentType == typeof(ARSelectionInteractable))
                    {
                        ConfigureSelection(interactable: (ARSelectionInteractable)component);
                    }
                    else if (componentType == typeof(ARTranslationInteractable))
                    {
                        ConfigureTranslation(interactable: (ARTranslationInteractable)component);
                    }
                    else if (componentType == typeof(ARScaleInteractable))
                    {
                        ConfigureScale(
                            interactable: (ARScaleInteractable)component,
                            minScale:  0.1f,
                            maxScale: 10.0f);
                    }

                    missingComponents.Add(component);
                }
            });

            return missingComponents;
        }

        private static void ConfigureSelection(ARSelectionInteractable interactable)
        {
            interactable.selectionVisualization = GameObject.CreatePrimitive(PrimitiveType.Cube);

            interactable.selectionVisualization.name = "AR Selection Visualization";

            interactable.selectionVisualization.GetComponent<MeshRenderer>().material = 
                Resources.Load("GemPlay/Materials/BoundingBox") as Material;

            var bounds = Core.Helpers.Unity.GameObjects.BoundsOfAllMeshRenderers(interactable.gameObject);

            interactable.selectionVisualization.transform.SetPositionAndRotation(
                position: bounds.center,
                rotation: interactable.gameObject.transform.rotation);

            interactable.selectionVisualization.transform.localScale = bounds.size;

            interactable.selectionVisualization.transform.SetParent(interactable.gameObject.transform);

            interactable.selectionVisualization.SetActive(false);
        }

        private static void ConfigureTranslation(ARTranslationInteractable interactable)
        {
            interactable.objectGestureTranslationMode = GestureTransformationUtility.GestureTranslationMode.Any;

            interactable.maxTranslationDistance = 1;  // @todo Expose this as a setting
            // A low value was set because the translation on Android is not accurately restricted to the deteced plane,
            // and the model can translate very far away in an unintuitive way.
            // This is the same issue we saw with legacy GemPlay, where it wasn't an issue on iOS.
            // iOS hasn't been tested yet here.
        }

        private static void ConfigureScale(
            ARScaleInteractable interactable,
            float minScale,
            float maxScale)
        {
            interactable.minScale = minScale;

            interactable.maxScale = maxScale;

            // There is a bug in XR Interaction Toolkit's ARScaleInteractable (at least version 0.9.4-preview)
            // where the setters for minScale and maxScale don't update other important internal variables,
            // screwing up the initial scaling.
            // But ARScaleInteractable.OnEnable updates the important internal variable (`m_CurrentScaleRatio`) 
            // so here's an awesome hack:
            interactable.enabled = false;

            interactable.enabled = true;


            // Also the sensitivity apparently needs to be manually updated for this to behave reasonably.
            // Maybe the following is good enough for now.
            interactable.sensitivity /= maxScale - minScale;
        }
    }
}
