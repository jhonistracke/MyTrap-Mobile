using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Request;
using MyTrapApp.Repository;
using MyTrapApp.Services;
using System;
using System.Threading.Tasks;

namespace MyTrapApp.Droid
{
    [Service]
    public class PositionService : Service, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        private static bool IsStarted = false;
        private static Context Context;
        public static string Token;

        GoogleApiClient apiClient;
        LocationRequest locRequest;

        private static long LOCATION_FASTEST_INTERVAL = 120000;//2 Minutes
        private static long LOCATION_INTERVAL = 600000;//10 Minutes

        public static void Start(Context context)
        {
            try
            {
                if (!IsStarted)
                {
                    Context = context;

                    Intent serviceIntent = new Intent(Context, typeof(PositionService));

                    context.StartService(serviceIntent);
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            try
            {
                if (AppStatus.UserLogged == null)
                {
                    MyTrapBDConfig.Initialize();

                    AppStatus.UserLogged = UserApiService.GetUserLogged();
                }

                if (AppStatus.UserLogged != null)
                {
                    Token = AppStatus.UserLogged.Token;

                    apiClient = new GoogleApiClient.Builder(Context, this, this).AddApi(LocationServices.API).Build();

                    locRequest = new LocationRequest();

                    apiClient.Connect();

                    IsStarted = true;
                }
                else
                {
                    StopSelf();
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            apiClient.Disconnect();
            locRequest.Dispose();

            IsStarted = false;

            SendBroadcast(new Intent("RESTART_SERVICE"));
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private async void StartTracking()
        {
            try
            {
                locRequest.SetPriority(LocationRequest.PriorityLowPower);
                locRequest.SetFastestInterval(LOCATION_FASTEST_INTERVAL);
                locRequest.SetInterval(LOCATION_INTERVAL);

                await LocationServices.FusedLocationApi.RequestLocationUpdates(apiClient, locRequest, this);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public void OnConnected(Bundle connectionHint)
        {
            StartTracking();
        }

        public async void OnLocationChanged(Location location)
        {
            try
            {
                if (location != null && location.Latitude != 0)
                {
                    PositionApiRequest position = new PositionApiRequest();

                    position.Latitude = (float)location.Latitude;
                    position.Longitude = (float)location.Longitude;

                    if (location.Time > 0)
                    {
                        var date = new DateTime(1970, 1, 1, 0, 0, 0);

                        date = date.AddMilliseconds(location.Time);

                        position.Date = date;
                    }
                    else
                    {
                        position.Date = DateTime.UtcNow;
                    }

                    AppStatus.Platform = EPlatform.ANDROID;

                    await SendPosition(position);
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private async Task SendPosition(PositionApiRequest position)
        {
            try
            {
                var response = await PositionApiService.Send(position, true);

                if (!response)
                {
                    PositionApiService.Save(position);
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }
        public void OnProviderDisabled(string provider)
        {

        }

        public void OnProviderEnabled(string provider)
        {

        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {

        }

        public void OnConnectionSuspended(int cause)
        {

        }

        public void OnConnectionFailed(ConnectionResult result)
        {

        }
    }
}