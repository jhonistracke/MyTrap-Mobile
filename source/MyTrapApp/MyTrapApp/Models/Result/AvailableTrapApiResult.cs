using Newtonsoft.Json;

namespace MyTrapApp.Models.Result
{
    public class AvailableTrapApiResult
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public int Amount { get; set; }

        [JsonProperty(PropertyName = "nameKey")]
        public string NameKey { get; set; }

        [JsonProperty(PropertyName = "keyGoogle")]
        public string KeyGoogle { get; set; }

        [JsonProperty(PropertyName = "keyApple")]
        public string KeyApple { get; set; }

        [JsonProperty(PropertyName = "keyWindows")]
        public string KeyWindows { get; set; }

        [JsonProperty(PropertyName = "value")]
        public double Value { get; set; }

        [JsonIgnore]
        public string Price { get; set; }
    }
}