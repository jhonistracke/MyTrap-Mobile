using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HockeyApp;
using MyTrap.Droid;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Request;
using MyTrapApp.Services;
using System;
using System.Collections.Generic;
using Xamarin.Facebook;
using Xamarin.Facebook.Login;

namespace MyTrapApp.Droid
{
    [Activity(Name = "com.mytrap.LoginActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class LoginActivity : Activity, View.IOnClickListener
    {
        ICallbackManager callbackManager;
        IFacebookCallback loginCallback;
        GraphRequest.IGraphJSONObjectCallback graphCallback;

        Button btn_login_facebook;
        ProgressBar progress_bar;

        bool isLoading = false;

        protected override void OnCreate(Bundle bundle)
        {
            try
            {
                ActionBar.Hide();

                base.OnCreate(bundle);

                SetContentView(Resource.Layout.activity_login);

                CrashManager.Register(this);

                RegisterGoogleMessage();

                InitializeComponents();

                //MyTrapDroidFunctions.PrintAppHash(ApplicationContext);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void RegisterGoogleMessage()
        {
            try
            {
                Gcm.Client.GcmClient.CheckDevice(this);
                Gcm.Client.GcmClient.CheckManifest(this);
                Gcm.Client.GcmClient.Register(this, GcmBroadcastReceiver.SENDER_IDS);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void InitializeComponents()
        {
            try
            {
                progress_bar = FindViewById<ProgressBar>(Resource.Id.progress_bar_login);

                btn_login_facebook = FindViewById<Button>(Resource.Id.btn_login_facebook);

                btn_login_facebook.SetOnClickListener(this);

                FacebookSdk.SdkInitialize(ApplicationContext);

                callbackManager = CallbackManagerFactory.Create();

                graphCallback = new GraphJSONObjectCallback
                {
                    HandleSuccess = email =>
                    {
                        LoginWithFacebook(Profile.CurrentProfile, email);
                    }
                };

                loginCallback = new FacebookCallback<LoginResult>
                {
                    HandleSuccess = loginResult =>
                    {
                        var request = GraphRequest.NewMeRequest(loginResult.AccessToken, graphCallback);

                        Bundle parameters = new Bundle();

                        parameters.PutString("fields", "id, name, email");

                        request.Parameters = parameters;

                        request.ExecuteAsync();
                    },
                    HandleCancel = () =>
                    {
                        StopLoading();
                    },
                    HandleError = loginError =>
                    {
                        StopLoading();
                    }
                };

                Xamarin.Facebook.Login.LoginManager.Instance.RegisterCallback(callbackManager, loginCallback);

                CheckUserLogged();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void RequestLoginWithFacebook()
        {
            try
            {
                StartLoading();

                List<string> permissions = new List<string>();

                permissions.Add("public_profile");
                permissions.Add("user_about_me");
                permissions.Add("email");

                Xamarin.Facebook.Login.LoginManager.Instance.LogInWithReadPermissions(this, permissions);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (data != null)
            {
                base.OnActivityResult(requestCode, resultCode, data);

                callbackManager.OnActivityResult(requestCode, (int)resultCode, data);
            }
        }

        public void OnClick(View v)
        {
            if (v.Id == btn_login_facebook.Id)
            {
                RequestLoginWithFacebook();
            }
        }

        private async void LoginWithFacebook(Profile profile, string email)
        {
            try
            {
                StartLoading();

                UserApiRequest user = new UserApiRequest();

                user.Name = profile.Name.ToString();
                user.RegisterType = (int)ERegisterType.FACEBOOK;
                user.RegisterProfileId = profile.Id.ToString();
                user.Email = email;

                user.ProfilePicture = new ImageApiRequest();
                user.ProfilePicture.Url = profile.GetProfilePictureUri(200, 200).ToString();

                var response = await UserApiService.Login(user);

                if (ResponseValidator.Validate(response, this))
                {
                    AppStatus.UserLogged = response;
                    CheckUserLogged();
                }
                else
                {
                    StopLoading();
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public void CheckUserLogged()
        {
            try
            {
                StartLoading();

                if (AppStatus.UserLogged != null)
                {
                    MyTrapDroidFunctions.OpenHome(this);
                }

                StopLoading();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void StartLoading()
        {
            if (!isLoading)
            {
                progress_bar.Visibility = ViewStates.Visible;
                isLoading = true;
                btn_login_facebook.Enabled = false;
            }
        }

        private void StopLoading()
        {
            progress_bar.Visibility = ViewStates.Gone;
            isLoading = false;
            btn_login_facebook.Enabled = true;
        }

        protected override void OnResume()
        {
            base.OnResume();

            Tracking.StartUsage(this);
        }

        protected override void OnPause()
        {
            base.OnPause();

            Tracking.StopUsage(this);
        }
    }

    class GraphJSONObjectCallback : Java.Lang.Object, GraphRequest.IGraphJSONObjectCallback
    {
        public Action<string> HandleSuccess;

        public void OnCompleted(Org.Json.JSONObject p0, GraphResponse p1)
        {
            var c = HandleSuccess;

            if (c != null)
            {
                var email = p0.OptString("email");
                c(email);
            }
        }
    }

    class FacebookCallback<TResult> : Java.Lang.Object, IFacebookCallback where TResult : Java.Lang.Object
    {
        public Action HandleCancel { get; set; }
        public Action<FacebookException> HandleError { get; set; }
        public Action<TResult> HandleSuccess { get; set; }

        public void OnCancel()
        {
            var c = HandleCancel;

            if (c != null)
            {
                c();
            }
        }

        public void OnError(FacebookException error)
        {
            var c = HandleError;

            if (c != null)
            {
                c(error);
            }
        }

        public void OnSuccess(Java.Lang.Object result)
        {
            var c = HandleSuccess;

            if (c != null)
            {
                c(result.JavaCast<TResult>());
            }
        }
    }
}