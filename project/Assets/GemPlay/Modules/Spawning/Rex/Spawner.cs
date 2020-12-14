using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GemPlay.Modules.Spawning.Rex
{
    ///<summary>
    /// This <c>Spawner</c> class spawns game objects defined by REXfile data.
    ///</summary>
    ///
    /// This covers e.g. the legacy code 
	/// - GemPlay/Scripts/RexSpawner/RexSpawner.cs
    public static class Spawner
    {
        public static RSG.IPromise<List<GameObject>> Spawn(
            in byte[] rexFileData,
            RoboticEyes.Rex.RexFileReader.RexConverter rexConverter,
            GameObject targetParent)
        {
            var promise = new RSG.Promise<List<GameObject>>();

            var newMeshes = new List<GameObject>();

            var stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start();

            rexConverter.ConvertFromRex(rexFileData, (success, loadedObjects) =>
            {
                if (!success)
                {
                    Debug.Log("ConvertFromRex failed.");

                    throw new System.Exception("ConvertFromRex failed.");
                }

                foreach (var item in loadedObjects.Meshes)
                {
                    item.gameObject.SetActive(true);

                    item.gameObject.transform.SetParent(targetParent.transform, false);

                    newMeshes.Add(item.gameObject);
                }
                
                Debug.Log("Spawned Rex objects after " + stopwatch.Elapsed.Milliseconds + " milliseconds.");

                promise.Resolve(newMeshes);
            });

            return promise;
        }
    }
}
