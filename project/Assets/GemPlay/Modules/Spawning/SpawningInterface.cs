using System.Collections;
using UnityEngine;

namespace GemPlay.Modules.Spawning
{
    ///<summary>
    /// This <c>SpawningInterface</c> class is the interface to GemPlay's Spawning functionality.
    ///</summary>
    public class SpawningInterface
    {
        readonly RoboticEyes.Rex.RexFileReader.RexConverter rexConverter;

        public GameObject GameObject { get; }

        private readonly MonoBehaviour coroutineManager;

        private readonly GameObject gemPySurfacePointPrefab;

        private readonly GameObject gemPyOrientationPrefab;

        public SpawningInterface(
            in GameObject parentGameObject,
            in MonoBehaviour coroutineManager,
            in bool constructScene)
        {
            this.coroutineManager = coroutineManager;

            if (constructScene)
            {
                GameObject = new GameObject("Spawning");

                GameObject.transform.SetParent(parentGameObject.transform);

                rexConverter = Rex.ConverterFactory.RexConverter(GameObject);
            }
            else
            {
                GameObject = GameObject.Find("Spawning");

                rexConverter = GameObject.GetComponent<RoboticEyes.Rex.RexFileReader.RexConverter>();
            }

            string gemPySurfacePointPrefabPath = "GemPlay/Prefabs/GemPySurfacePoint";
            // @todo Construct the prefab in code rather than with prefab file.
            gemPySurfacePointPrefab = Resources.Load(gemPySurfacePointPrefabPath) as GameObject;

            if (gemPySurfacePointPrefab == null)
            {
                Debug.LogError(gemPySurfacePointPrefabPath + ".prefab not found");
            }


            string gemPyOrientationPrefabPath = "GemPlay/Prefabs/GemPyOrientation";
            // @todo Construct the prefab in code rather than with prefab file.
            gemPyOrientationPrefab = Resources.Load(gemPyOrientationPrefabPath) as GameObject;

            if (gemPyOrientationPrefab == null)
            {
                Debug.LogError(gemPyOrientationPrefabPath + ".prefab not found");
            }
        }

        public RSG.IPromise SpawnModelOutput(
            in Core.Data.GemPy.Output outputData,
            in Core.Data.GemPy.GameObjectContainers.Output outputContainer)
        {
            return SpawnRexMeshes(
                rexMeshData: outputData.RexMeshData,
                targetParent: outputContainer.RexMeshesGameObject);
        }

        
        public RSG.IPromise DestroyModelOutput(
            in Core.Data.GemPy.GameObjectContainers.Output outputContainer)
        {
            var promise = new RSG.Promise();

            var _outputContainer = outputContainer;

            IEnumerator Coroutine()
            {
                foreach (Transform transform in _outputContainer.RexMeshesGameObject.transform)
                {
                    Object.Destroy(transform.gameObject);
                }

                promise.Resolve();

                yield return null;
            }

            coroutineManager.StartCoroutine(Coroutine());

            return promise;
        }
        
        private RSG.IPromise SpawnRexMeshes(
            in byte[] rexMeshData,
            in GameObject targetParent)
        {
            var promise = new RSG.Promise();

            Rex.Spawner.Spawn(
                rexFileData: rexMeshData,
                rexConverter: rexConverter,
                targetParent: targetParent)
            .Then(newRexGameObjects =>
            {
                promise.Resolve();
            })
            .Catch(error =>
            {
                Debug.LogError(error.Message + "\n" + error.StackTrace);

                throw error;
            });

            return promise;
        }

        public RSG.IPromise SpawnModelInput(
            in Core.Data.GemPy.Input inputData,
            in Core.Data.GemPy.GameObjectContainers.Input inputContainer)
        {
            var promise = new RSG.Promise();

            var _inputData = inputData;

            var _inputContainer = inputContainer;

            IEnumerator Coroutine()
            {
                for (int i = 0; i < _inputData.DataPoints.Count; i++)
                {
                    var dataPoint = _inputData.DataPoints[i];
                    
                    SpawnInputDataPoint(
                        dataPoint: dataPoint,
                        color: _inputData.Surfaces[dataPoint.surface].color,
                        parentGameObject: _inputContainer.DataPointsGameObject);
                };

                promise.Resolve();

                yield return null;
            }

            coroutineManager.StartCoroutine(Coroutine());

            return promise;
        }

        private GameObject SpawnInputDataPoint(
            in Core.Data.GemPy.DataPoint dataPoint,
            in Color color,
            in GameObject parentGameObject)
        {
            GameObject prefab = gemPySurfacePointPrefab;

            if (dataPoint.type == Core.Data.GemPy.InputType.SurfacePoint)
            {
                prefab = gemPySurfacePointPrefab;
            }
            else if (dataPoint.type == Core.Data.GemPy.InputType.Orientation)
            {
                prefab = gemPyOrientationPrefab;
            }

            var gameObject = (GameObject)Object.Instantiate(
                original: prefab,
                parent: parentGameObject.transform,
                instantiateInWorldSpace: false);

            gameObject.name += dataPoint.id;

            gameObject.transform.localPosition = new Vector3(
                dataPoint.position.x,
                dataPoint.position.y,
                dataPoint.position.z);

            if (dataPoint.type == Core.Data.GemPy.InputType.Orientation)
            {
                gameObject.transform.localRotation *= Quaternion.FromToRotation(
                    gameObject.transform.forward,  // The prefab is an arrow pointing in the positive-z direction, i.e. the forward direction in Unity.
                    dataPoint.gradient);  // We want the arrow to point in the same direction as the gradient vector.
            }


            var modelInputDataComponent = gameObject.AddComponent<Core.Data.GemPy.Components.ModelInputData>(); 

            modelInputDataComponent.dataPoint = dataPoint;


            var block = new MaterialPropertyBlock();

            block.SetColor(
                name: "_BaseColor",
                value: color);

            gameObject.GetComponent<Renderer>().SetPropertyBlock(block);


            return gameObject;
        }
    }
}
