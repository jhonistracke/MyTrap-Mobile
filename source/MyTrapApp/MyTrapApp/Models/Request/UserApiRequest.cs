using Newtonsoft.Json;

namespace MyTrapApp.Models.Request
{
    public class UserApiRequest
    {
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "registerType")]
        public int RegisterType { get; set; }

        [JsonProperty(PropertyName = "registerProfileId")]
        public string RegisterProfileId { get; set; }

        [JsonProperty(PropertyName = "profilePicture")]
        public ImageApiRequest ProfilePicture { get; set; }
    }
}