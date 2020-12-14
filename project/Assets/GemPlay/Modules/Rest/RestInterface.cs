using UnityEngine;

namespace GemPlay.Modules.Rest
{
    enum StatusCode : long { OK = 200L, AcceptedForProcessing = 202L };


    ///<summary>
    /// This <c>RestInterface</c> class is the interface to GemPlay's REST client-server functionality.
    ///</summary>
    public class RestInterface
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public RestInterface(in string serverSettingsPath = "GemPlay/ScriptableObjects/ServerSettings")
        {
            var serverSettings = Resources.Load(serverSettingsPath) as ScriptableObjects.ServerSettings;

            Host = serverSettings.host;

            Port = serverSettings.port;
        }

        public RSG.IPromise<string[]> GetModelNames()
        {
            var modelNames = new RSG.Promise<string[]>();

            Client.Request(RequestFormatter.GetModelNames(
                host: Host,
                port: Port))
            .Then(response =>
            {
                if (response.StatusCode == ((long)StatusCode.OK))
                {
                    modelNames.Resolve(ResponseParser.GetModelNames(response));
                }
                else
                {
                    Debug.LogError("Failed to get model names from server.");
                }
            });

            return modelNames;
        }

        public RSG.IPromise LoadModel(in string geoModelUrn)
        {
            var opUrn = System.Guid.NewGuid().ToString();

            var promise = new RSG.Promise();

            Client.Request(RequestFormatter.PostOperationLoadModel(
                    host: Host,
                    port: Port,
                    geoModelUrn: geoModelUrn,
                    operationUrn: opUrn))
            .Then(response =>
            {
                // It is a bug in the server that it returns a 202 response when the model is already loaded.
                // 202 usually means "accepted for processing".
                // There are actually two (successful) cases with the buggy server:
                //     200: Model was not yet loaded and server responded after completing the load operation,
                //     202: Model was already loaded previously.
                if ((response.StatusCode == ((long)StatusCode.OK)) || 
                    (response.StatusCode == ((long)StatusCode.AcceptedForProcessing)))
                {
                    promise.Resolve();
                }
                else
                {
                    Debug.LogError("Failed to load model on server.");
                }
            });

            return promise;
        }

        public RSG.IPromise<Core.Data.GemPy.Input> GetModelInput(in string geoModelUrn)
        {
            var promise = new RSG.Promise<Core.Data.GemPy.Input>();

            Client.Request(RequestFormatter.GetModelInput(
                host: Host,
                port: Port,
                geoModelUrn: geoModelUrn))
            .Then(response =>
            {
                if (response.StatusCode == ((long)StatusCode.OK))
                {
                    promise.Resolve(ResponseParser.GetModelInput(response));
                }
                else
                {
                    Debug.LogError("Failed to get model input from server.");
                }
            });

            return promise;
        }

        public RSG.IPromise<Core.Data.GemPy.Output> GetModelOutput(in string geoModelUrn)
        {
            var promise = new RSG.Promise<Core.Data.GemPy.Output>();

            Client.Request(RequestFormatter.GetModelOutput(
                    host: Host,
                    port: Port,
                    geoModelUrn: geoModelUrn))
            .Then(response =>
            {
                if (response.StatusCode == ((long) StatusCode.OK))
                {
                    promise.Resolve(ResponseParser.GetModelOutput(response));
                }
                else
                {
                    Debug.LogError("Failed to get model output from server.");
                }
            });

            return promise;
        }

        public RSG.IPromise EditModelSurfacePoint(
            in string geoModelUrn,
            in Core.Data.GemPy.DataPoint dataPoint)
        {
            var opUrn = System.Guid.NewGuid().ToString();

            var promise = new RSG.Promise();

            Client.Request(RequestFormatter.PostOperationEditModelSurfacePoint(
                    host: Host,
                    port: Port,
                    geoModelUrn: geoModelUrn,
                    dataPoint: dataPoint,
                    operationUrn: opUrn))
            .Then(response =>
            {
                if ((response.StatusCode == ((long)StatusCode.OK)) ||
                    (response.StatusCode == ((long)StatusCode.AcceptedForProcessing)))
                {
                    promise.Resolve();
                }
                else
                {
                    Debug.LogError("Failed to edit model on server.");
                }
            });

            return promise;
        }

        public RSG.IPromise EditModelOrientation(
            in string geoModelUrn,
            in Core.Data.GemPy.DataPoint dataPoint)
        {
            var opUrn = System.Guid.NewGuid().ToString();

            var promise = new RSG.Promise();

            Client.Request(RequestFormatter.PostOperationEditModelOrientation(
                    host: Host,
                    port: Port,
                    geoModelUrn: geoModelUrn,
                    dataPoint: dataPoint,
                    operationUrn: opUrn))
            .Then(response =>
            {
                if ((response.StatusCode == ((long)StatusCode.OK)) ||
                    (response.StatusCode == ((long)StatusCode.AcceptedForProcessing)))
                {
                    promise.Resolve();
                }
                else
                {
                    Debug.LogError("Failed to edit model on server.");
                }
            });

            return promise;
        }
    }
}
