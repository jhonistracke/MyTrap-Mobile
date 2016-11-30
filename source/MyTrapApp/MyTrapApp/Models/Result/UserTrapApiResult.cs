using Newtonsoft.Json;

namespace MyTrapApp.Models.Result
{
    public class UserTrapApiResult
    {
        [JsonProperty(PropertyName = "nameKey")]
        public string NameKey { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public int Amount { get; set; }
    }
}