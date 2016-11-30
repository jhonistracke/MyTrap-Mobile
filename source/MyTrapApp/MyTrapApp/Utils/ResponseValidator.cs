using MyTrapApp.Models.Base;
using System;

namespace MyTrapApp.Utils
{
    public class ResponseValidator
    {
        public static bool Validate(BaseApiResult response)
        {
            bool result = false;

            try
            {
                if (response != null)
                {
                    result = !response.Error;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
    }
}