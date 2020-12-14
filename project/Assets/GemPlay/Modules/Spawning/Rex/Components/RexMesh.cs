using UnityEngine;


namespace GemPlay.Modules.Spawning.Rex.Components
{
    public class RexMesh : RoboticEyes.Rex.RexFileReader.RexMeshObject
    {
        public MeshFilter meshFilter;

        public MeshRenderer meshRenderer;

        private void OnDestroy()
        {
            if ((meshRenderer != null) && (meshRenderer.sharedMaterial) != null)
            {
                Destroy(meshRenderer.sharedMaterial.mainTexture);
            }
            
            Destroy (meshRenderer.sharedMaterial);

            Destroy (meshFilter.sharedMesh);
        }

        public override bool SetMeshData (Mesh mesh, Material material)
        {
            meshRenderer.sharedMaterial = material;

            meshFilter.sharedMesh = mesh;

            Bounds = new Bounds
            {
                center = meshFilter.sharedMesh.bounds.center,
                extents = meshFilter.sharedMesh.bounds.extents
            };

            return true;
        }

        public override void SetRendererEnabled (bool enabled)
        {
            meshRenderer.enabled = enabled;
        }

        public override void SetLayer (int layer)
        {
            meshRenderer.gameObject.layer = layer;
        }
    }
}