using System.Collections.Generic;
using Newtonsoft.Json;

namespace GemPlay.Modules.Rest
{
    [System.Serializable]
    readonly struct OperationBody
    {
        [JsonProperty]
        readonly string op_name;

        [JsonProperty]
        readonly string op_urn;

        [JsonProperty]
        readonly Dictionary<string, dynamic> parameters;

        public OperationBody(string operationName, string operationUrn, Dictionary<string, dynamic> parameters)
        {
            op_name = operationName;

            op_urn = operationUrn;

            this.parameters = parameters;
        }
    }

    public static class RequestFormatter
    {
        public static Proyecto26.RequestHelper GetModelNames(
            in string host, in int port)

        {
            return new Proyecto26.RequestHelper
            {
                Method = "GET",
                Uri = "http://" + host + ":" + port + "/geomodels/"
            };
        }

        public static Proyecto26.RequestHelper PostOperation(
            in string host,
            in int port,
            string geoModelUrn,
            in string bodyString)
        {
            return new Proyecto26.RequestHelper
            {
                Method = "POST",
                Uri = "http://" + host + ":" + port + "/geomodels/" + geoModelUrn + "/operations",
                BodyString = bodyString,
            };
        }

        /// <summary>
        /// POST load operation to endpoint /geomodels/{geoModelUrn}/operations 
        /// of GemPy server.
        ///</summary>
        /// <remarks> 
        /// A long timeout is set by default to allow for the case 
        /// that no model has yet been loaded on the GemPy server.
        /// The server unfortunately does not respond until after the entire operation is complete,
        /// and when loading a model for the first time, Theano spends a long time compiling stuff.
        ///</remarks>
        public static Proyecto26.RequestHelper PostOperationLoadModel(
            in string host,
            in int port,
            in string geoModelUrn,
            in int timeoutSeconds = 300, // Longest needed so far in testing was 190
            in string operationUrn = "")
        {
            string _operationUrn;

            if (operationUrn.Equals(""))
            {
                _operationUrn = System.Guid.NewGuid().ToString();
            }
            else
            {
                _operationUrn = string.Copy(operationUrn);
            }
            
            return new Proyecto26.RequestHelper
                {
                    Method = "POST",
                    Uri = "http://" + host + ":" + port + "/geomodels/" + geoModelUrn + "/operations",
                    Timeout = timeoutSeconds,
                    BodyString = JsonConvert.SerializeObject(new OperationBody(
                        operationName: "load",
                        operationUrn: _operationUrn,
                        parameters: new Dictionary<string, dynamic> {
                            {"modelUrn", geoModelUrn}})).ToString(),
                };
        }

        public static Proyecto26.RequestHelper GetOperationStatus(
            in string host,
            in int port,
            in string geoModelUrn,
            in string operationUrn)

        {
            return new Proyecto26.RequestHelper
            {
                Method = "GET",
                Uri = "http://" + host + ":" + port + "/geomodels/" + geoModelUrn + 
                      "/operations/" + operationUrn + "/status"
            };
        }

        public static Proyecto26.RequestHelper GetModelInput(
            in string host, in int port, in string geoModelUrn)
        {
            return new Proyecto26.RequestHelper
            {
                Method = "GET",
                Uri = "http://" + host + ":" + port + "/geomodels/" + geoModelUrn + "/input",
            };
        }

        public static Proyecto26.RequestHelper GetModelOutput(
            in string host, in int port, in string geoModelUrn)
        {
            return new Proyecto26.RequestHelper
            {
                Method = "GET",
                Uri = "http://" + host + ":" + port + "/geomodels/" + geoModelUrn + "/output",
            };
        }

        public static Proyecto26.RequestHelper PostOperationEditModelSurfacePoint(
            in string host,
            in int port,
            in string geoModelUrn,
            in Core.Data.GemPy.DataPoint dataPoint,
            in int timeoutSeconds = 300, // Longest needed so far in testing was 190
            in string operationUrn = "")
        {
            string _operationUrn;

            if (operationUrn.Equals(""))
            {
                _operationUrn = System.Guid.NewGuid().ToString();
            }
            else
            {
                _operationUrn = string.Copy(operationUrn);
            }

            return new Proyecto26.RequestHelper
            {
                Method = "POST",
                Uri = "http://" + host + ":" + port + "/geomodels/" + geoModelUrn + "/operations",
                Timeout = timeoutSeconds,
                BodyString = JsonConvert.SerializeObject(new OperationBody(
                    operationName: "edit",
                    operationUrn: _operationUrn,
                    parameters: new Dictionary<string, dynamic> {
                        {"modelUrn", geoModelUrn},
                        {"method", "modify_surface_points"},
                        {"indices", dataPoint.id},
                        {"X", dataPoint.position.x},
                        {"Y", dataPoint.position.y},
                        {"Z", dataPoint.position.z}
                    })).ToString(),
            };
        }

        public static Proyecto26.RequestHelper PostOperationEditModelOrientation(
            in string host,
            in int port,
            in string geoModelUrn,
            in Core.Data.GemPy.DataPoint dataPoint,
            in int timeoutSeconds = 300, // Longest needed so far in testing was 190
            in string operationUrn = "")
        {
            string _operationUrn;

            if (operationUrn.Equals(""))
            {
                _operationUrn = System.Guid.NewGuid().ToString();
            }
            else
            {
                _operationUrn = string.Copy(operationUrn);
            }

            return new Proyecto26.RequestHelper
            {
                Method = "POST",
                Uri = "http://" + host + ":" + port + "/geomodels/" + geoModelUrn + "/operations",
                Timeout = timeoutSeconds,
                BodyString = JsonConvert.SerializeObject(new OperationBody(
                    operationName: "edit",
                    operationUrn: _operationUrn,
                    parameters: new Dictionary<string, dynamic> {
                        {"modelUrn", geoModelUrn},
                        {"method", "modify_orientations"},
                        {"idx", dataPoint.id}, // Note that this was called "indices" instead of "idx" when editing surface points.
                        {"X", dataPoint.position.x},
                        {"Y", dataPoint.position.y},
                        {"Z", dataPoint.position.z},
                        {"G_x", dataPoint.gradient.x},
                        {"G_y", dataPoint.gradient.y},
                        {"G_z", dataPoint.gradient.z}
                    })).ToString(),
            };
        }
    }
}
