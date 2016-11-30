using Android.App;
using Android.Content;
using Gcm.Client;
using Java.Util.Concurrent.Atomic;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Services;
using System;

namespace MyTrapApp.Droid
{
    [BroadcastReceiver(Permission = Constants.PERMISSION_GCM_INTENTS)]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_MESSAGE },
    Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_REGISTRATION_CALLBACK },
    Categories = new string[] { "@PACKAGE_NAME@" })]
    [IntentFilter(new string[] { Constants.INTENT_FROM_GCM_LIBRARY_RETRY },
    Categories = new string[] { "@PACKAGE_NAME@" })]
    public class GcmBroadcastReceiver : GcmBroadcastReceiverBase<GcmService>
    {
        public static string[] SENDER_IDS = new string[] { "" };
    }

    [Service]
    public class GcmService : GcmServiceBase
    {
        public GcmService() : base(GcmBroadcastReceiver.SENDER_IDS) { }

        protected override void OnRegistered(Context context, string registrationId)
        {
            try
            {
                if (!string.IsNullOrEmpty(registrationId))
                {
                    AppStatus.AppRegistration = registrationId;
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        protected override void OnUnRegistered(Context context, string registrationId)
        {

        }

        protected override void OnMessage(Context context, Intent intent)
        {
            try
            {
                string show = intent.Extras.GetString("show");

                if (show == "1")
                {
                    string message = intent.Extras.GetString("message");

                    bool owner = intent.Extras.GetString("owner") == "1" ? true : false;
                    string points = intent.Extras.GetString("points");

                    string ownerValue = "";

                    if (owner)
                    {
                        ownerValue = "owner";
                    }

                    int notificationId = NotificationID.GetNewId();

                    Intent notificationIntent = new Intent(context, typeof(HomeActivity));

                    notificationIntent.PutExtras(intent.Extras);

                    notificationIntent.PutExtra("showNotification", true);
                    notificationIntent.PutExtra("owner", ownerValue);

                    PendingIntent pIntent = PendingIntent.GetActivity(context, notificationId, notificationIntent, 0);

                    Notification notification = new Notification.Builder(context)
                            .SetContentTitle(context.Resources.GetString(MyTrap.Droid.Resource.String.new_trap_notification))
                            .SetContentText(message)
                            .SetContentIntent(pIntent)
                            .SetSmallIcon(MyTrap.Droid.Resource.Drawable.ic_launcher)
                            .SetAutoCancel(true)
                            .Build();

                    NotificationManager notificationManager = (NotificationManager)context.GetSystemService(NotificationService);

                    notificationManager.Notify(notificationId, notification);
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        protected override void OnError(Context context, string errorId)
        {

        }
    }

    public class NotificationID
    {
        private static AtomicInteger c = new AtomicInteger(0);

        public static int GetNewId()
        {
            return c.IncrementAndGet();
        }
    }
}