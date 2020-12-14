using UnityEngine;

namespace GemPlay.Modules.Projects
{
    public class Project
    {
        public string Guid { get; }

        public GameObject GameObject { get; }

        public Core.Data.GemPy.Model ModelData { get; }

        public Core.Data.GemPy.GameObjectContainers.Model ModelGameObjects { get; }

        public Vector3 Shift { get; set; }

        public float Scale { get; set; }

        public Project(string guid)
        {
            Guid = guid;

            GameObject = new GameObject(guid)
            {
                tag = "GemPlay Project"
            };

            ModelData = new Core.Data.GemPy.Model();

            ModelGameObjects = new Core.Data.GemPy.GameObjectContainers.Model();

            ModelGameObjects.GameObject.transform.SetParent(GameObject.transform);
        }

        internal void CorrectRexMeshCoordinatesFromGemPyWriter()
        {
            var meshFilters = ModelGameObjects.Output.GameObject.GetComponentsInChildren<MeshFilter>();

            Debug.Log("Correcting coordinate system of " + meshFilters.Length + " RexMeshes "
                + " from incorrect GemPy writer.");

            foreach (var meshFilter in meshFilters)
            {
                // @todo 
                // There should be a way to modify the vertices in place
                // rather than creating the copy and having to move data around so much;
                // but so far e.g. https://stackoverflow.com/questions/3867961/c-altering-values-for-every-item-in-an-array didn't work.
                var oldVertices = meshFilter.mesh.vertices;

                var newVertices = new Vector3[oldVertices.Length];

                for (var i = 0; i < oldVertices.Length; i++)
                {
                    var oldV = oldVertices[i];

                    var newV = new Vector3(
                        oldV.x,
                        -oldV.z,
                        oldV.y);

                    newVertices[i] = newV;
                }

                meshFilter.mesh.vertices = newVertices;

                meshFilter.mesh.RecalculateNormals();

                meshFilter.mesh.RecalculateBounds();
            };
        }

        internal void TransformAllOutputGameObjectsToGameOriginAndScale()
        {
            var meshFilters = ModelGameObjects.Output.GameObject.GetComponentsInChildren<MeshFilter>();

            Debug.Log("Shifting " + meshFilters.Length + " meshes "
                + " by vector of " + Shift
                + " and scaling them by factor of " + Scale);

            foreach (var meshFilter in meshFilters)
            {
                // @todo 
                // There should be a way to modify the vertices in place
                // rather than creating the copy and having to move data around so much;
                // but so far e.g. https://stackoverflow.com/questions/3867961/c-altering-values-for-every-item-in-an-array didn't work.
                var oldVertices = meshFilter.mesh.vertices;

                var newVertices = new Vector3[oldVertices.Length];

                for (var i = 0; i < oldVertices.Length; i++)
                {
                    var oldV = oldVertices[i];

                    var newV = new Vector3(
                        oldV.x,
                        oldV.y,
                        // RexMeshes should already be in the Unity coordinate system rather than the GemPy one,
                        // so there should be no need to switch any axes here.
                        oldV.z);

                    newV = (newV + Shift) * Scale;

                    newVertices[i] = newV;
                }

                meshFilter.mesh.vertices = newVertices;

                meshFilter.mesh.RecalculateNormals();

                meshFilter.mesh.RecalculateBounds();
            };
        }

        internal void TransformAllInputGameObjectsFromDataToGameCoordinates()
        {
            foreach (Transform transform in ModelGameObjects.Input.DataPointsGameObject.transform)
            {
                TransformFromDataToGameCoordinates(transform);
            };
        }

        internal void TransformFromDataToGameCoordinates(Transform transform)
        {
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.z,
                // Recall: Y and Z axes are switched between GemPy and Unity coordinate systems.
                transform.localPosition.y);

            transform.localPosition = (transform.localPosition + Shift) * Scale;

            transform.localRotation = 
                Quaternion.AngleAxis(
                    angle: -90,
                    axis: GameObject.transform.right) // Recall: Rotations in Unity are left-handed
                * transform.localRotation;
        }

        internal void TransformFromGameToDataCoordinates(Transform transform)
        {
            transform.localPosition = transform.localPosition / Scale - Shift;


            // @todo Have server provide data using REX/Unity coordinate system rather than fixing this in Unity.
            // In the meantime:
            transform.localPosition = new Vector3(
                transform.localPosition.x,
                transform.localPosition.z,
                // Recall: Y and Z axes are switched between GemPy and Unity coordinate systems.
                transform.localPosition.y);

            transform.localRotation =
                Quaternion.AngleAxis(
                    angle: 90,
                    axis: GameObject.transform.right) // Recall: Rotations in Unity are left-handed
                * transform.localRotation;
        }

        internal void SetInputDataFromGameObject(
            in GameObject gameObject,
            ref Core.Data.GemPy.DataPoint dataPoint)
        {
            dataPoint.position = new Vector3(
                gameObject.transform.localPosition.x,
                gameObject.transform.localPosition.y,
                gameObject.transform.localPosition.z);

            dataPoint.gradient = new Vector3(
                gameObject.transform.forward.x,
                gameObject.transform.forward.y,
                gameObject.transform.forward.z);
        }
    };
}
