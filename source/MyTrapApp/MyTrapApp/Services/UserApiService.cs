using MyTrapApp.Models.App;
using MyTrapApp.Models.Request;
using MyTrapApp.Models.Result;
using MyTrapApp.Repository;
using MyTrapApp.Utils;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MyTrapApp.Services
{
    public class UserApiService
    {
        public static async Task<UserApiResult> Login(UserApiRequest request)
        {
            UserApiResult result = null;

            try
            {
                result = await HttpController.PostData<UserApiResult>(ApiMethods.LoginUrl(), request);

                if (ResponseValidator.Validate(result))
                {
                    SaveUserLogged(result);
                }
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        public static async Task<UserApiResult> Get()
        {
            UserApiResult result = null;

            try
            {
                result = await HttpController.GetData<UserApiResult>(ApiMethods.GetUser());

                if (ResponseValidator.Validate(result))
                {
                    SaveUserLogged(result);
                }
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        public static void SaveUserLogged(UserApiResult user)
        {
            if (user != null)
            {
                UserApiResult actualUser = GetUserLogged();

                if (actualUser == null || !actualUser.ProfilePicture.Url.Equals(user.ProfilePicture.Url))
                {
                    string blobImageUrl = user.ProfilePicture != null ? user.ProfilePicture.Url : StringUtils.EMPTY;

                    user.ProfilePicture.Base64 = Functions.GetBase64ImageFromUrl(blobImageUrl);
                }
                else if (actualUser != null && actualUser.ContainsProfilePictureBase64())
                {
                    user.ProfilePicture = actualUser.ProfilePicture;
                }

                string jsonUser = JsonConvert.SerializeObject(user);

                PreferenceRepository.Save(Preference.USER_LOGGED_JSON, jsonUser);
            }
        }

        public static UserApiResult GetUserLogged()
        {
            UserApiResult user = null;

            Preference preferenceUser = PreferenceRepository.GetByKey(Preference.USER_LOGGED_JSON);

            if (preferenceUser != null)
            {
                user = JsonConvert.DeserializeObject<UserApiResult>(preferenceUser.Value);
            }

            return user;
        }
    }
}