using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Request;
using MyTrapApp.Services;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Storage;

namespace MyTrapApp.WP.BackgroundLocation
{
    public sealed class LocationBackgroundTask : IBackgroundTask
    {
        private CancellationTokenSource _cts = null;

        async void IBackgroundTask.Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            try
            {
                AppStatus.Platform = EPlatform.WP;
                AppStatus.DirectoryBD = ApplicationData.Current.LocalFolder.Path;
                AppStatus.SQLitePlatform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();

                // Associate a cancellation handler with the background task.
                taskInstance.Canceled += OnCanceled;

                // Get cancellation token
                if (_cts == null)
                {
                    _cts = new CancellationTokenSource();
                }

                CancellationToken token = _cts.Token;

                Geolocator geolocator = new Geolocator { DesiredAccuracy = PositionAccuracy.High };

                // Make the request for the current position
                Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);

                await OnLocationChanged(pos);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                _cts = null;
                deferral.Complete();
            }
        }

        public static async void GetNewPosition()
        {
            // Create geolocator object
            Geolocator geolocator = new Geolocator { DesiredAccuracy = PositionAccuracy.High };

            // Make the request for the current position
            Geoposition pos = await geolocator.GetGeopositionAsync();

            await OnLocationChanged(pos);
        }

        private static async Task OnLocationChanged(Geoposition pos)
        {
            try
            {
                if (pos != null && pos.Coordinate.Latitude != 0)
                {
                    PositionApiRequest position = new PositionApiRequest();

                    position.Latitude = (float)pos.Coordinate.Latitude;
                    position.Longitude = (float)pos.Coordinate.Longitude;
                    position.Date = DateTime.UtcNow;

                    await SendPosition(position);
                }
            }
            catch (Exception exception)
            {

            }
        }

        private static async Task SendPosition(PositionApiRequest position)
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

            }
        }

        private void OnCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts = null;
            }
        }
    }
}