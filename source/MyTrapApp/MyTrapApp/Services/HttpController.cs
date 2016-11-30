using MyTrapApp.Models.Base;
using MyTrapApp.Models.Result;
using Newtonsoft.Json;
using NodaTime;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MyTrapApp.Services
{
    public class HttpController
    {
        public static async Task<T> PostData<T>(string url, object request)
        {
            T result = (T)Activator.CreateInstance(typeof(T));

            try
            {
                string jsonObject = JsonConvert.SerializeObject(request);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                UserApiResult user = UserApiService.GetUserLogged();

                if (user != null)
                {
                    httpWebRequest.Headers["Authorization"] = user.Token;
                }

                httpWebRequest.Headers["X-App-Platform"] = ((int)AppStatus.Platform).ToString();
                httpWebRequest.Headers["X-App-TimeZone"] = GetTimeZone();
                httpWebRequest.Headers["X-App-Registration"] = AppStatus.AppRegistration;
                httpWebRequest.Headers["Accept-Language"] = AppStatus.Language;

                Stream streamRequest = await httpWebRequest.GetRequestStreamAsync();

                using (var streamWriter = new StreamWriter(streamRequest))
                {
                    streamWriter.Write(jsonObject);
                    streamWriter.Flush();
                }

                WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync();

                var httpResponse = (HttpWebResponse)httpWebResponse;

                string responseBody = "";

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseBody = streamReader.ReadToEnd();
                }

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<T>(responseBody);
                }
            }
            catch (Exception ex)
            {
                BaseApiResult errorResult = new BaseApiResult();

                errorResult.Message = ex.Message + "|" + ex.StackTrace;
                errorResult.Error = true;

                result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(errorResult));
            }

            return result;
        }

        public static async Task<T> GetData<T>(string url, params object[] @params)
        {
            T result = (T)Activator.CreateInstance(typeof(T));

            try
            {
                url = string.Format(url, @params);

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "GET";

                UserApiResult user = UserApiService.GetUserLogged();

                if (user != null)
                {
                    httpWebRequest.Headers["Authorization"] = user.Token;
                }

                httpWebRequest.Headers["X-App-Platform"] = ((int)AppStatus.Platform).ToString();
                httpWebRequest.Headers["X-App-TimeZone"] = GetTimeZone();
                httpWebRequest.Headers["X-App-Registration"] = AppStatus.AppRegistration;
                httpWebRequest.Headers["Accept-Language"] = AppStatus.Language;

                WebResponse httpWebResponse = await httpWebRequest.GetResponseAsync();

                var httpResponse = (HttpWebResponse)httpWebResponse;

                string responseBody = "";

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    responseBody = streamReader.ReadToEnd();
                }

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    result = JsonConvert.DeserializeObject<T>(responseBody);
                }
            }
            catch (Exception ex)
            {
                BaseApiResult errorResult = new BaseApiResult();

                errorResult.Message = ex.Message + "|" + ex.StackTrace;
                errorResult.Error = true;

                result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(errorResult));
            }

            return result;
        }

        private static string GetTimeZone()
        {
            Instant now = SystemClock.Instance.Now;

            DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetSystemDefault();

            return tz.Id;
        }
    }
}