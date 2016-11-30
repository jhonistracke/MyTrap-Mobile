using MyTrapApp.Models.Base;
using Newtonsoft.Json;

namespace MyTrapApp.Models.Result
{
    public class BuyIntentApiResult : BaseApiResult
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}