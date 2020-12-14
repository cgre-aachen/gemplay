using System.Collections.Generic;
using UnityEngine;

namespace GemPlay.Core.Data.GemPy
{
    public enum InputType { SurfacePoint, Orientation }

    public class DataPoint
    {
        public InputType type;

        public Vector3 position;  // Surface points and orientations have these

        public Vector3 gradient;  // Only orientations have these

        public string surface;

        public int id;
    }

    public class Surface
    {
        public string surface;

        public string series;

        public Color color;

        public int id;
    }

    public class Input
    {
        public List<DataPoint> DataPoints;

        public Dictionary<string,Surface> Surfaces;
    }

    public class Output
    {
        public byte[] RexMeshData;
    }

    public class Model
    {
        public Input Input;

        public Output Output;
    }


    namespace GameObjectContainers
    {
        public class Input
        {
            public GameObject GameObject { get; }

            public GameObject DataPointsGameObject { get; }

            public Input()
            {
                GameObject = new GameObject("Input");

                DataPointsGameObject = new GameObject("Surface Points and Orientations");

                DataPointsGameObject.transform.SetParent(GameObject.transform);
            }
        }

        public class Output
        {
            public GameObject GameObject { get; }

            public GameObject RexMeshesGameObject { get; }

            public Output()
            {
                GameObject = new GameObject("Output");

                RexMeshesGameObject = new GameObject("REX Meshes");

                RexMeshesGameObject.transform.SetParent(GameObject.transform);
            }
        }

        public class Model
        {
            public GameObject GameObject { get; }

            public Input Input { get; }

            public Output Output { get; }

            public Model()
            {
                GameObject = new GameObject("GemPy Model");

                Input = new Input();

                Input.GameObject.transform.SetParent(GameObject.transform);

                Output = new Output();

                Output.GameObject.transform.SetParent(GameObject.transform);
            }
        }
    }

    namespace Components
    {
        public class ModelInputData : MonoBehaviour
        {
            public DataPoint dataPoint;
        }
    }
}
