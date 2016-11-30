using Newtonsoft.Json;

namespace MyTrapApp.Models.Request
{
    public class ArmedTrapApiRequest
    {
        [JsonProperty(PropertyName = "nameKey")]
		public string NameKey { get; set; }

		[JsonProperty(PropertyName = "lat")]
		public double Latitude { get; set; }

		[JsonProperty(PropertyName = "lng")]
		public double Longitude { get; set; }
    }
}