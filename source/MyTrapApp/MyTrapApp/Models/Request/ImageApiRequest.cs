using Newtonsoft.Json;

namespace MyTrapApp.Models.Request
{
    public class ImageApiRequest
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}