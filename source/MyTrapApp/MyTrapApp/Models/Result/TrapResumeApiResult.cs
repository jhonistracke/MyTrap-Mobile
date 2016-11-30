using Newtonsoft.Json;

namespace MyTrapApp.Models.Result
{
    public class TrapResumeApiResult
    {
        [JsonProperty(PropertyName = "nameKey")]
        public string NameKey { get; set; }
    }
}