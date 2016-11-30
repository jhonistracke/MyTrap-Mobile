using Android.App;
using Android.Content;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Services;
using System;

namespace MyTrapApp.Droid
{
    [BroadcastReceiver(Enabled = true, Exported = false)]
    [IntentFilter(new[] { Intent.ActionBootCompleted, "RESTART_SERVICE" }, Categories = new[] { Intent.CategoryDefault })]
    public class BootReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            //Toast.MakeText(context, intent.Action, ToastLength.Long).Show();

            try
            {
                if (AppStatus.UserLogged == null)
                {
                    MyTrapBDConfig.Initialize();

                    AppStatus.UserLogged = UserApiService.GetUserLogged();
                }

                if (AppStatus.UserLogged != null)
                {
                    PositionService.Start(context);
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }
    }
}