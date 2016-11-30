using Android.App;
using Android.Content.PM;
using Android.OS;
using Java.Util;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Services;
using System.Threading;

namespace MyTrapApp.Droid
{
    [Activity(MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            ActionBar.Hide();

            base.OnCreate(bundle);

            Xamarin.Forms.Forms.Init(this, bundle);

            MyTrapBDConfig.Initialize();

            SetContentView(MyTrap.Droid.Resource.Layout.activity_splash);

            AppStatus.Language = Locale.Default.ToString();

            AppStatus.Platform = Models.Enums.EPlatform.ANDROID;

            Thread thread = new Thread(() =>
            {
                LoadUser();
            });

            thread.Start();
        }

        private void LoadUser()
        {
            AppStatus.UserLogged = UserApiService.GetUserLogged();

            if (AppStatus.UserLogged == null)
            {
                StartActivity(typeof(LoginActivity));
            }
            else
            {
                MyTrapDroidFunctions.OpenHome(this);
            }
        }
    }
}