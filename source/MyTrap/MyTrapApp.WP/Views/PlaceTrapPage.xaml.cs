using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Request;
using MyTrapApp.Models.Result;
using MyTrapApp.Services;
using MyTrapApp.WP.Utils;
using System;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;

namespace MyTrapApp.WP.Views
{
    public sealed partial class PlaceTrapPage : Page
    {
        private string TRAP_NAMEKEY_SELECTED;
        private float LAST_LATITUDE = 0;
        private float LAST_LONGITUDE = 0;

        public PlaceTrapPage()
        {
            this.InitializeComponent();
            map.Loaded += Map_Loaded;

            if (AppStatus.UserLogged.ContainsTraps())
            {
                btnPlaceTrap.IsEnabled = false;
            }
            else
            {
                AppShell.Current.AppFrame.Navigate(typeof(TrapsPage));
            }
        }

        private async void Map_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = 20 };

                Geoposition pos = await geolocator.GetGeopositionAsync();

                map.Center = new Geopoint(new BasicGeoposition()
                {
                    Latitude = pos.Coordinate.Latitude,
                    Longitude = pos.Coordinate.Longitude
                });

                LAST_LATITUDE = (float)pos.Coordinate.Latitude;
                LAST_LONGITUDE = (float)pos.Coordinate.Longitude;

                map.ZoomLevel = 16;

                MapIcon mapIcon = new MapIcon();

                mapIcon.Location = map.Center;
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/icon_trap_map.png"));
                mapIcon.ZIndex = 0;

                map.MapElements.Add(mapIcon);

                LoadUserTraps();
            }
            else
            {
                AppShell.Current.AppFrame.Navigate(typeof(HomePage));
            }
        }

        private void LoadUserTraps()
        {
            try
            {
                AppStatus.UserLogged.Traps = AppStatus.UserLogged.Traps.ToList();

                DisableAllTraps();

                foreach (UserTrapApiResult trapUser in AppStatus.UserLogged.Traps.Where(obj => obj.Amount > 0).ToList())
                {
                    Enable(trapUser.NameKey);
                }

                UnselectTraps();
            }
            catch (Exception exception)
            {

            }
        }

        private void Enable(string nameKey)
        {
            switch (nameKey)
            {
                case ETrap.BEAR:
                    btnBearTrap.IsEnabled = true;
                    break;

                case ETrap.DOGS:
                    btnDogTrap.IsEnabled = true;
                    break;

                case ETrap.MINE:
                    btnMineTrap.IsEnabled = true;
                    break;

                case ETrap.PIT:
                    btnPitTrap.IsEnabled = true;
                    break;

                default:
                    break;
            }
        }

        private void DisableAllTraps()
        {
            btnBearTrap.IsEnabled = false;
            btnDogTrap.IsEnabled = false;
            btnMineTrap.IsEnabled = false;
            btnPitTrap.IsEnabled = false;
        }

        private void UnselectTraps()
        {
            ((Image)((btnBearTrap.ContentTemplateRoot as StackPanel).Children[0])).Source = new BitmapImage(new Uri("ms-appx:///Assets/trap_bear_trap_icon.png", UriKind.RelativeOrAbsolute));
            ((Image)((btnDogTrap.ContentTemplateRoot as StackPanel).Children[0])).Source = new BitmapImage(new Uri("ms-appx:///Assets/trap_dogs_trap_icon.png", UriKind.RelativeOrAbsolute));
            ((Image)((btnMineTrap.ContentTemplateRoot as StackPanel).Children[0])).Source = new BitmapImage(new Uri("ms-appx:///Assets/trap_mine_trap_icon.png", UriKind.RelativeOrAbsolute));
            ((Image)((btnPitTrap.ContentTemplateRoot as StackPanel).Children[0])).Source = new BitmapImage(new Uri("ms-appx:///Assets/trap_pit_trap_icon.png", UriKind.RelativeOrAbsolute));
        }

        private void btnBearTrap_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UnselectTraps();

            ((Image)((btnBearTrap.ContentTemplateRoot as StackPanel).Children[0])).Source = new BitmapImage(new Uri("ms-appx:///Assets/trap_bear_trap_selected_icon.png", UriKind.RelativeOrAbsolute));

            btnPlaceTrap.IsEnabled = true;

            TRAP_NAMEKEY_SELECTED = ETrap.BEAR;
        }

        private void btnDogTrap_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UnselectTraps();

            ((Image)((btnDogTrap.ContentTemplateRoot as StackPanel).Children[0])).Source = new BitmapImage(new Uri("ms-appx:///Assets/trap_dogs_trap_selected_icon.png", UriKind.RelativeOrAbsolute));

            btnPlaceTrap.IsEnabled = true;

            TRAP_NAMEKEY_SELECTED = ETrap.DOGS;
        }

        private void btnMineTrap_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UnselectTraps();

            ((Image)((btnMineTrap.ContentTemplateRoot as StackPanel).Children[0])).Source = new BitmapImage(new Uri("ms-appx:///Assets/trap_mine_trap_selected_icon.png", UriKind.RelativeOrAbsolute));

            btnPlaceTrap.IsEnabled = true;

            TRAP_NAMEKEY_SELECTED = ETrap.MINE;
        }

        private void btnPitTrap_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            UnselectTraps();

            ((Image)((btnPitTrap.ContentTemplateRoot as StackPanel).Children[0])).Source = new BitmapImage(new Uri("ms-appx:///Assets/trap_pit_trap_selected_icon.png", UriKind.RelativeOrAbsolute));

            btnPlaceTrap.IsEnabled = true;

            TRAP_NAMEKEY_SELECTED = ETrap.PIT;
        }

        private async void btnPlaceTrap_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var dialog = new MessageDialog("You want to place the trap at the current position?");

            dialog.Title = "Place Trap";

            dialog.Commands.Add(new UICommand { Label = "Yes", Id = 0 });
            dialog.Commands.Add(new UICommand { Label = "No", Id = 1 });

            var res = await dialog.ShowAsync();

            if ((int)res.Id == 0)
            {
                ArmedTrapApiRequest armedTrap = new ArmedTrapApiRequest();

                armedTrap.NameKey = TRAP_NAMEKEY_SELECTED;
                armedTrap.Latitude = LAST_LATITUDE;
                armedTrap.Longitude = LAST_LONGITUDE;

                var response = await TrapApiService.Arm(armedTrap);

                if (ResponseValidator.Validate(response))
                {
                    var dialogSuccess = new MessageDialog("Trap was placed. You will be notified when someone is gaught!");

                    dialogSuccess.Title = "Placed Trap!";

                    dialogSuccess.Commands.Add(new UICommand { Label = "Ok", Id = 0 });

                    res = await dialogSuccess.ShowAsync();

                    AppShell.Current.AppFrame.Navigate(typeof(HomePage));
                }
            }
        }
    }
}