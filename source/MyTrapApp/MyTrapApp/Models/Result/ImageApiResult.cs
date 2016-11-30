using Newtonsoft.Json;

namespace MyTrapApp.Models.Result
{
    public class ImageApiResult
    {
        public string Base64 { get; set; }

        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }
}