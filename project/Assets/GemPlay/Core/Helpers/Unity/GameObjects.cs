using UnityEngine;


namespace GemPlay.Core.Helpers.Unity
{
    public static class GameObjects
    {
        public static Bounds BoundsOfAllMeshes(in GameObject container)
        {
            var bounds = new Bounds();

            var meshFilters = container.GetComponentsInChildren<MeshFilter>();

            foreach (var meshFilter in meshFilters)
            {
                bounds.Encapsulate(meshFilter.mesh.bounds);
            }

            return bounds;
        }

        public static Bounds BoundsOfAllMeshRenderers(in GameObject container)
        {
            var bounds = new Bounds();

            var meshRenderers = container.GetComponentsInChildren<MeshRenderer>();

            foreach (var meshRenderer in meshRenderers)
            {
                bounds.Encapsulate(meshRenderer.bounds);
            }

            return bounds;
        }

        public static void AddBoxColliderContainingAllMeshes(ref GameObject container)
        {
            var bounds = GameObjects.BoundsOfAllMeshRenderers(container: container);

            var boxCollider = container.AddComponent<BoxCollider>();

            boxCollider.size = bounds.size;
        }

        public static void AddCollidersToAllMeshes(ref GameObject container)
        {
            foreach (var meshRenderer in container.GetComponentsInChildren<MeshRenderer>())
            {
                meshRenderer.gameObject.AddComponent<MeshCollider>();
            }
        }

        public static void SetLocalScaleFromGlobal(ref Transform transform, in Vector3 globalScale)
        {
            transform.localScale = Vector3.one;

            transform.localScale = new Vector3(
                globalScale.x / transform.lossyScale.x,
                globalScale.y / transform.lossyScale.y,
                globalScale.z / transform.lossyScale.z);
        }
    }
}
