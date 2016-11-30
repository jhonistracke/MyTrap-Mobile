using MyTrapApp.Models.Base;
using MyTrapApp.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MyTrapApp.Models.Result
{
    public class UserApiResult : BaseApiResult
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "profilePicture")]
        public ImageApiResult ProfilePicture { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "points")]
        public int Points { get; set; }

        [JsonProperty(PropertyName = "traps")]
        public List<UserTrapApiResult> Traps { get; set; }

        public int GetAmountTraps()
        {
            int result = 0;

            if (Traps != null && Traps.Count > 0)
            {
                for (int cont = 0; cont < Traps.Count; cont++)
                {
                    UserTrapApiResult userTrap = Traps[cont];

                    if (userTrap != null && userTrap.Amount > 0)
                    {
                        result += userTrap.Amount;
                    }
                }
            }

            return result;
        }

        public bool ContainsTraps()
        {
            bool result = false;

            if (GetAmountTraps() > 0)
            {
                result = true;
            }

            return result;
        }

        public bool ContainsProfilePictureBase64()
        {
            if (ProfilePicture != null && !StringUtils.IsNullOrEmpty(ProfilePicture.Base64))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}