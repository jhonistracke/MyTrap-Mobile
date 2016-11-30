using MyTrapApp.Models.Request;
using MyTrapApp.Models.Result;
using MyTrapApp.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyTrapApp.Services
{
    public class TrapApiService
    {
        public static async Task<UserApiResult> Arm(ArmedTrapApiRequest request)
        {
            UserApiResult result = null;

            try
            {
                result = await HttpController.PostData<UserApiResult>(ApiMethods.ArmTrap(), request);

                if (ResponseValidator.Validate(result))
                {
                    UserApiService.SaveUserLogged(result);
                }
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        public static async Task<List<ArmedTrapApiResult>> ListArmed()
        {
            List<ArmedTrapApiResult> result = null;

            try
            {
                result = await HttpController.GetData<List<ArmedTrapApiResult>>(ApiMethods.GetArmedTraps());
            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }
    }
}