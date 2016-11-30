using MyTrapApp.Models.Base;
using MyTrapApp.Models.Request;
using MyTrapApp.Models.Result;
using MyTrapApp.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyTrapApp.Services
{
    public class PurchaseApiService
    {
        public static async Task<List<AvailableTrapApiResult>> ListAvailableTraps()
        {
            List<AvailableTrapApiResult> result = null;

            try
            {
                result = await HttpController.GetData<List<AvailableTrapApiResult>>(ApiMethods.AvailableTraps());

            }
            catch (Exception)
            {
                result = null;
            }

            return result;
        }

        public static async Task<bool> InsertBuyIntent(BuyIntentApiRequest request)
        {
            bool result = false;

            try
            {
                var response = await HttpController.PostData<BuyIntentApiResult>(ApiMethods.InsertBuyIntent(), request);

                if (ResponseValidator.Validate(response))
                {
                    request.Id = response.Id;

                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public static async Task<bool> InvalidateBuyIntent(BuyIntentApiRequest request)
        {
            bool result = false;

            try
            {
                var response = await HttpController.PostData<BaseApiResult>(ApiMethods.InvalidateBuyIntent(), request);

                if (ResponseValidator.Validate(response))
                {
                    result = true;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }

        public static async Task<bool> RegisterPurchase(BuyIntentApiRequest request)
        {
            var result = false;

            var response = await HttpController.PostData<UserApiResult>(ApiMethods.RegisterPurchase(), request);

            if (ResponseValidator.Validate(response))
            {
                result = true;

                UserApiService.SaveUserLogged(response);

                result = true;
            }

            return result;
        }
    }
}