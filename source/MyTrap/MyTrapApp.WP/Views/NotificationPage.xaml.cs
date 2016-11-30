using MyTrapApp.Models.Enums;
using MyTrapApp.WP.Models;
using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace MyTrapApp.WP.Views
{
    public sealed partial class NotificationPage : Page
    {
        TrapNotification trapNotification = null;

        public NotificationPage()
        {
            this.InitializeComponent();
            map.Loaded += MapLoaded;
        }

        private void MapLoaded(object sender, RoutedEventArgs e)
        {
            SetMapLocation();
        }

        private void SetMapLocation()
        {
            if (map != null && trapNotification != null)
            {
                map.Center = new Geopoint(new BasicGeoposition()
                {
                    Latitude = trapNotification.Latitude,
                    Longitude = trapNotification.Longitude
                });

                map.ZoomLevel = 16;

                RandomAccessStreamReference mapIconStreamReference = null;

                if (trapNotification.TrapNameKey == ETrap.BEAR.ToString())
                {
                    mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/trap_bear_trap_selected_icon.png"));
                }
                else if (trapNotification.TrapNameKey == ETrap.MINE.ToString())
                {
                    mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/trap_mine_trap_selected_icon.png"));
                }
                else if (trapNotification.TrapNameKey == ETrap.PIT.ToString())
                {
                    mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/trap_pit_trap_selected_icon.png"));
                }
                else if (trapNotification.TrapNameKey == ETrap.DOGS.ToString())
                {
                    mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/trap_dogs_trap_selected_icon.png"));
                }
                else
                {
                    return;
                }

                MapIcon mapIcon = new MapIcon();

                mapIcon.Location = new Geopoint(new BasicGeoposition() { Latitude = trapNotification.Latitude, Longitude = trapNotification.Longitude });
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon.Title = trapNotification.Date;
                mapIcon.Image = mapIconStreamReference;
                mapIcon.ZIndex = 0;

                map.MapElements.Add(mapIcon);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is TrapNotification)
            {
                trapNotification = (TrapNotification)e.Parameter;

                imgOtherUserProfile.ImageSource = new BitmapImage(new Uri(trapNotification.OtherUserImg));

                if (trapNotification.IsOwner)
                {
                    string msg = "You caught {0} and won {1} points!";

                    msg = string.Format(msg, trapNotification.OtherUserName, trapNotification.Points);

                    lblNotificationMsg.Text = msg;
                }
                else
                {
                    string msg = "You were caught by {0}!";

                    msg = string.Format(msg, trapNotification.OtherUserName);

                    lblNotificationMsg.Text = msg;
                }
            }
        }
    }
}