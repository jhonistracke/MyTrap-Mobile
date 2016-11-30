using Newtonsoft.Json;
using System;

namespace MyTrapApp.Models.Result
{
    public class ArmedTrapApiResult
    {
		[JsonProperty(PropertyName = "nameKey")]
		public string NameKey { get; set; }

		[JsonProperty(PropertyName = "date")]
		public DateTime Date { get; set; }

		[JsonProperty(PropertyName = "lat")]
		public double Latitude { get; set; }

		[JsonProperty(PropertyName = "lng")]
		public double Longitude { get; set; }
    }
}