using Facebook;
using MyTrapApp.WP.Utils;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.Security.Authentication.Web;

namespace MyTrapApp.WP.Helpers
{
    public class FacebookHelper
    {
        FacebookClient _fb = new FacebookClient();
        readonly Uri _callbackUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri();
        readonly Uri _loginUrl;
        private const string FacebookAppId = "563110880559548";
        private const string FacebookPermissions = "user_about_me,public_profile,email";

        public string AccessToken
        {
            get { return _fb.AccessToken; }
        }

        public FacebookHelper()
        {
            _loginUrl = _fb.GetLoginUrl(new
            {
                client_id = FacebookAppId,
                redirect_uri = _callbackUri.AbsoluteUri,
                scope = FacebookPermissions,
                display = "popup",
                response_type = "token"
            });

            Debug.WriteLine(_callbackUri);
        }

        private void ValidateAndProccessResult(WebAuthenticationResult result)
        {
            if (result.ResponseStatus == WebAuthenticationStatus.Success)
            {
                var responseUri = new Uri(result.ResponseData.ToString());
                var facebookOAuthResult = _fb.ParseOAuthCallbackUrl(responseUri);

                if (string.IsNullOrWhiteSpace(facebookOAuthResult.Error))
                    _fb.AccessToken = facebookOAuthResult.AccessToken;
                else
                {
                }
            }
            else if (result.ResponseStatus == WebAuthenticationStatus.ErrorHttp)
            {
            }
            else
            {
                _fb.AccessToken = null;
            }
        }

        public async Task LoginAndContinue()
        {
            if (DeviceTypeHelper.GetDeviceFormFactorType() == DeviceFormFactorType.Desktop)
            {
                WebAuthenticationResult result = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, _loginUrl);

                ValidateAndProccessResult(result);
            }
            else
            {
                WebAuthenticationBroker.AuthenticateAndContinue(_loginUrl);
            }
        }

        public void ContinueAuthentication(WebAuthenticationBrokerContinuationEventArgs args)
        {
            ValidateAndProccessResult(args.WebAuthenticationResult);
        }
    }
}