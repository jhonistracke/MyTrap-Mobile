using System;
using Newtonsoft.Json;

namespace MyTrapApp.Models.Request
{
    public class PositionApiRequest
    {
        [JsonIgnore]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public float Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public float Longitude { get; set; }

		[JsonProperty(PropertyName = "date")]
		public DateTime Date { get; set; }
    }
}