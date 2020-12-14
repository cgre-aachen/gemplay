using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GemPlay.Modules.Spawning.Rex
{
    ///<summary>
    /// This <c>ConverterFactory</c> class creates a RexConverter component.
    ///</summary>
    public static class ConverterFactory
    {
        /// <summary>Method <c>RexConverter</c> creates a `RexConverter` and attaches it to `gameObject`.</summary>
        public static RoboticEyes.Rex.RexFileReader.RexConverter RexConverter(GameObject gameObject)
        {
            var rexConverterComponent = gameObject.AddComponent<RoboticEyes.Rex.RexFileReader.RexConverter>();


            // Mesh
            // The game object hierarchy and components here match the prefab provided in the example from Robotic Eyes.
            rexConverterComponent.meshMaterialSolid = Resources.Load("GemPlay/Materials/VertexColored") as Material;


            var rexMeshGameObject = new GameObject("RexMesh");

            rexMeshGameObject.transform.SetParent(gameObject.transform);

            var rexMeshComponent = rexMeshGameObject.AddComponent<Components.RexMesh>();

            rexConverterComponent.meshPrefab = rexMeshGameObject;

            
            var meshGameObject = new GameObject("Mesh");

            meshGameObject.transform.SetParent(rexMeshGameObject.transform);

            rexMeshComponent.meshFilter = meshGameObject.AddComponent<MeshFilter>();

            rexMeshComponent.meshRenderer = meshGameObject.AddComponent<MeshRenderer>();


            rexConverterComponent.markMeshesNoLongerReadable = false;
            //


            return rexConverterComponent;
        }
    }
}
