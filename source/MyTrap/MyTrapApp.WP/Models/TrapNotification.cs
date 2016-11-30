using Newtonsoft.Json;

namespace MyTrapApp.WP.Models
{
    public class TrapNotification
    {
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "owner")]
        public bool IsOwner { get; set; }

        [JsonProperty(PropertyName = "points")]
        public int Points { get; set; }

        [JsonProperty(PropertyName = "show")]
        public bool ShowNotification { get; set; }

        [JsonProperty(PropertyName = "trap")]
        public string TrapNameKey { get; set; }

        [JsonProperty(PropertyName = "lat")]
        public float Latitude { get; set; }

        [JsonProperty(PropertyName = "lng")]
        public float Longitude { get; set; }

        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "userName")]
        public string OtherUserName { get; set; }

        [JsonProperty(PropertyName = "img")]
        public string OtherUserImg { get; set; }
    }
}