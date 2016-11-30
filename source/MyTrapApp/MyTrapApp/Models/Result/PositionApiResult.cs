using Newtonsoft.Json;

namespace MyTrapApp.Models.Result
{
    public class PositionApiResult
    {
        [JsonProperty(PropertyName = "lat")]
        public float Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public float Longitude { get; set; }
    }
}