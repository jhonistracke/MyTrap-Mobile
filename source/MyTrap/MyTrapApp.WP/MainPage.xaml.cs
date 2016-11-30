using Facebook;
using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Request;
using MyTrapApp.Services;
using MyTrapApp.WP.Helpers;
using MyTrapApp.WP.Utils;
using MyTrapApp.WP.Views;
using System.Dynamic;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyTrapApp.WP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page, IWebAuthenticationBrokerContinuable
    {
        FacebookHelper ObjFBHelper = new FacebookHelper();
        FacebookClient fbclient = new FacebookClient();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void BtnFaceBookLogin_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            StartLoading();

            await ObjFBHelper.LoginAndContinue();

            if (DeviceTypeHelper.GetDeviceFormFactorType() == DeviceFormFactorType.Desktop)
            {
                ContinueWithWebAuthenticationBroker(null);
            }
        }

        public async void ContinueWithWebAuthenticationBroker(WebAuthenticationBrokerContinuationEventArgs args)
        {
            if (args != null)
            {
                ObjFBHelper.ContinueAuthentication(args);
            }

            if (ObjFBHelper.AccessToken != null)
            {
                fbclient = new FacebookClient(ObjFBHelper.AccessToken);

                dynamic parameters = new ExpandoObject();
                parameters.access_token = ObjFBHelper.AccessToken;
                parameters.fields = "id,name,email";

                //Fetch facebook UserProfile:  
                dynamic profileDetails = await fbclient.GetTaskAsync("me", parameters);

                string id = profileDetails.id;
                string email = profileDetails.email;
                string name = profileDetails.name;
                string profilePicture = string.Format("https://graph.facebook.com/{0}/picture?type={1}&access_token={2}", id, "large", ObjFBHelper.AccessToken);

                LoginWithFacebook(id, email, name, profilePicture);
            }
        }

        private async void LoginWithFacebook(string id, string email, string name, string profilePicture)
        {
            UserApiRequest user = new UserApiRequest();

            user.RegisterProfileId = id;
            user.Email = email;
            user.Name = name;
            user.ProfilePicture = new ImageApiRequest() { Url = profilePicture };
            user.RegisterType = (int)ERegisterType.FACEBOOK;

            var response = await UserApiService.Login(user);

            if (ResponseValidator.Validate(response))
            {
                AppStatus.UserLogged = response;
                CheckUserLogged();
                StopLoading();
            }
            else
            {
                StopLoading();
            }
        }

        private void CheckUserLogged()
        {
            if (AppStatus.UserLogged != null)
            {
                AppShell appShell = new AppShell();

                appShell.AppFrame.Navigate(typeof(HomePage), null, new Windows.UI.Xaml.Media.Animation.SuppressNavigationTransitionInfo());

                Window.Current.Content = appShell;
            }
        }

        private void StartLoading()
        {
            BtnLogin.IsEnabled = false;
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Visible;
        }

        private void StopLoading()
        {
            BtnLogin.IsEnabled = true;
            progressBar.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }
    }
}