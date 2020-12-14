using UnityEngine;

namespace GemPlay.Modules.UIUX.XR.Components
{
    /// <summary>
    /// This component class is useful e.g. for keeping interactables at a constant global scale for easier interaction.
    /// </summary>
    public class Rescaler : MonoBehaviour
    {
        public float globalScale = 0.05f;

        void Update()
        {
            var transform = this.transform;

            var globalScaleVector = new Vector3(globalScale, globalScale, globalScale);

            Core.Helpers.Unity.GameObjects.SetLocalScaleFromGlobal(
                transform: ref transform,
                globalScale: in globalScaleVector);
        }
    }
}
