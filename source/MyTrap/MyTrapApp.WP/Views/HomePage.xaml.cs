using MyTrapApp.Services;
using MyTrapApp.WP.BackgroundLocation;
using System;
using Windows.ApplicationModel.Background;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MyTrapApp.WP.Views
{
    public sealed partial class HomePage : Page
    {
        // For background task registration
        private const string BackgroundTaskName = "MyTrapBackgroundTask";
        private const string BackgroundTaskEntryPoint = "MyTrapApp.WP.BackgroundLocation.LocationBackgroundTask";
        private IBackgroundTaskRegistration _geolocTask = null;

        bool placeTrap = true;

        public HomePage()
        {
            this.InitializeComponent();

            UpdateInfoProfile();

            RequestProfile();

            CheckBackgroundTask();

            LocationBackgroundTask.GetNewPosition();
        }

        private async void RequestProfile()
        {
            var response = await UserApiService.Get();

            if (response != null && !response.Error)
            {
                AppStatus.UserLogged = response;

                UpdateInfoProfile();
            }
        }

        private void UpdateInfoProfile()
        {
            if (AppStatus.UserLogged.ProfilePicture != null && AppStatus.UserLogged.ContainsProfilePictureBase64())
            {
                BitmapImage image = new BitmapImage();

                var imageBytes = Convert.FromBase64String(AppStatus.UserLogged.ProfilePicture.Base64);

                using (InMemoryRandomAccessStream ms = new InMemoryRandomAccessStream())
                {
                    using (DataWriter writer = new DataWriter(ms.GetOutputStreamAt(0)))
                    {
                        writer.WriteBytes((byte[])imageBytes);
                        writer.StoreAsync().GetResults();
                    }

                    image.SetSource(ms);
                }

                imgUserProfile.ImageSource = image;
            }

            lblUserName.Text = AppStatus.UserLogged.Name;
            lblAmountPoints.Text = AppStatus.UserLogged.Points.ToString();
            lblAmountTraps.Text = AppStatus.UserLogged.GetAmountTraps().ToString();

            if (AppStatus.UserLogged.GetAmountTraps() > 0)
            {
                lblMsgHome.Text = "Place a trap and wait for someone to get caught to earn points";
                btnHomeAction.Content = "Place Trap";

                placeTrap = true;
            }
            else
            {
                lblMsgHome.Text = "You have no more traps. Buy some traps now";
                btnHomeAction.Content = "Buy Traps";

                placeTrap = false;
            }
        }

        private void btnHomeAction_Click_1(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (placeTrap)
            {
                AppShell.Current.AppFrame.Navigate(typeof(PlaceTrapPage));
            }
            else
            {
                AppShell.Current.AppFrame.Navigate(typeof(TrapsPage));
            }
        }

        private async void CheckBackgroundTask()
        {
            foreach (var t in BackgroundTaskRegistration.AllTasks)
            {
                if (t.Value.Name == BackgroundTaskName)
                {
                    t.Value.Unregister(true);
                    break;
                }
            }

            BackgroundAccessStatus backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();

            BackgroundTaskBuilder geolocTaskBuilder = new BackgroundTaskBuilder();

            geolocTaskBuilder.Name = BackgroundTaskName;
            geolocTaskBuilder.TaskEntryPoint = BackgroundTaskEntryPoint;
            geolocTaskBuilder.SetTrigger(new TimeTrigger(15, false));

            _geolocTask = geolocTaskBuilder.Register();

            _geolocTask.Completed += OnCompleted;

            var accessStatus = await Geolocator.RequestAccessAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Loop through all background tasks to see if SampleBackgroundTaskName is already registered
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == BackgroundTaskName)
                {
                    _geolocTask = cur.Value;
                    break;
                }
            }

            if (_geolocTask != null)
            {
                // Associate an event handler with the existing background task
                _geolocTask.Completed += OnCompleted;
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            if (_geolocTask != null)
            {
                // Remove the event handler
                _geolocTask.Completed -= OnCompleted;
            }

            base.OnNavigatingFrom(e);
        }

        private async void OnCompleted(IBackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs e)
        {
            if (sender != null)
            {
                // Update the UI with progress reported by the background task
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    try
                    {
                        // If the background task threw an exception, display the exception in
                        // the error text box.
                        e.CheckResult();
                    }
                    catch (Exception ex)
                    {

                    }
                });
            }
        }
    }
}