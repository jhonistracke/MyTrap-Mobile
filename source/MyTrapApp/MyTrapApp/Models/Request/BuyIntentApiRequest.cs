using Newtonsoft.Json;
using System;

namespace MyTrapApp.Models.Request
{
    public class BuyIntentApiRequest
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "availableTrapId")]
        public string AvailableTrapId { get; set; }

        [JsonProperty(PropertyName = "storeKey")]
        public string StoreKey { get; set; }
    }
}