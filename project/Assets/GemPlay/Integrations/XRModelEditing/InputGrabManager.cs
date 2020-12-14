using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace GemPlay.Integrations.XRModelEditing
{
    internal class InputGrabManager
    {
        public GameObject GameObject { get; }

        public XRGrabInteractable Interactable { get; }

        public Core.Data.GemPy.DataPoint InputDataPoint { get; }

        public Dictionary<string, UnityAction<XRBaseInteractor>> Actions { get; }

        private UnityAction<XRBaseInteractor> Grab { get; }

        private UnityAction<XRBaseInteractor> Release { get; }

        public InputGrabManager(
            GameObject gameObject,
            XRGrabInteractable grabInteractable,
            Core.Data.GemPy.DataPoint modelInputDataPoint)
        {
            GameObject = gameObject;

            Interactable = grabInteractable;

            InputDataPoint = modelInputDataPoint;

            Actions = new Dictionary<string, UnityAction<XRBaseInteractor>>();

            Actions.Add(key: "Grab", Grab);

            Actions.Add(key: "Release", Release);
        }
    }
}
