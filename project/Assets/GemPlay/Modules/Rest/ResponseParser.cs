using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GemPlay.Modules.Rest
{
    [System.Serializable]
    struct SurfacePointJObject
    {
        [JsonProperty]
        public float X { get; set; }

        [JsonProperty]
        public float Y { get; set; }

        [JsonProperty]
        public float Z { get; set; }

        [JsonProperty]
        public string surface { get; set; }
    }

    struct OrientationJObject
    {
        [JsonProperty]
        public float X { get; set; }

        [JsonProperty]
        public float Y { get; set; }

        [JsonProperty]
        public float Z { get; set; }

        [JsonProperty]
        public string surface { get; set; }

        [JsonProperty]
        public float G_x { get; set; }

        [JsonProperty]
        public float G_y { get; set; }

        [JsonProperty]
        public float G_z { get; set; }
    }

    struct SurfaceJObject
    {
        [JsonProperty]
        public string surface { get; set; }

        [JsonProperty]
        public string series { get; set; }

        [JsonProperty]
        public string color { get; set; }

        [JsonProperty]
        public int id { get; set; }
    }

    struct Operation
    {
        [JsonProperty]
        public string opStatus { get; set; }
    }

    public static class ResponseParser
    {
        public static string[] GetModelNames(Proyecto26.ResponseHelper response)
        {
            return JObject.Parse(response.Text)
                .Properties().Select(p => p.Name).ToArray();
        }

        public static string PostOperationLoadModel(Proyecto26.ResponseHelper response)
        {
            string loadOperationUrn = JObject.Parse(response.Text)
                .Properties().Select(p => p.Value).ToString();

            return loadOperationUrn;
        }

        public static string GetOperationStatus(Proyecto26.ResponseHelper response)
        {
            var operationData = JsonConvert.DeserializeObject<Operation>(response.Text);

            return operationData.opStatus;
        }

        public static Core.Data.GemPy.Input GetModelInput(Proyecto26.ResponseHelper response)
        {
            var modelInput = new Core.Data.GemPy.Input
            {
                Surfaces = new Dictionary<string,Core.Data.GemPy.Surface>(),

                DataPoints = new List<Core.Data.GemPy.DataPoint>(),
            };
            
            foreach (var items in JObject.Parse(response.Text))
            {
                if (items.Key == "surfaces")
                {
                    foreach (var data in items.Value)
                    {
                        var jObject = data.First.ToObject<SurfaceJObject>();

                        ColorUtility.TryParseHtmlString(jObject.color, out Color color);

                        var surface = new Core.Data.GemPy.Surface
                        {
                            surface = jObject.surface,
                            series = jObject.series,
                            id = int.Parse(data.Path.Split('.')[1]),
                            color = color
                        };

                        modelInput.Surfaces.Add(
                            key: jObject.surface,
                            value: surface);
                    }
                }
                else if (items.Key == "orientations")
                {
                    foreach (var data in items.Value)
                    {
                        var jObject = data.First.ToObject<OrientationJObject>();

                        var orientation = new Core.Data.GemPy.DataPoint
                        {
                            type = Core.Data.GemPy.InputType.Orientation,
                            position = new Vector3(jObject.X, jObject.Y, jObject.Z),
                            gradient = new Vector3(jObject.G_x, jObject.G_y, jObject.G_z),
                            surface = jObject.surface,
                            id = int.Parse(data.Path.Split('.')[1])
                        };

                        modelInput.DataPoints.Add(orientation);
                    }
                }
                else if (items.Key == "sp")
                {
                    foreach (var data in items.Value)
                    {
                        var jObject = data.First.ToObject<SurfacePointJObject>();

                        var point = new Core.Data.GemPy.DataPoint
                        {
                            type = Core.Data.GemPy.InputType.SurfacePoint,
                            position = new Vector3(jObject.X, jObject.Y, jObject.Z),
                            surface = jObject.surface,
                            id = int.Parse(data.Path.Split('.')[1])
                        };

                        modelInput.DataPoints.Add(point);
                    }
                }
            }

            return modelInput;
        }

        public static Core.Data.GemPy.Output GetModelOutput(Proyecto26.ResponseHelper response)
        {
            var output = new Core.Data.GemPy.Output
            {
                RexMeshData = response.Data
            };

            return output;
        }
    }
}
