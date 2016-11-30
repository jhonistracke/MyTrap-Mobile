using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Result;
using MyTrapApp.Services;
using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace MyTrapApp.WP.Views
{
    public sealed partial class PlacedTrapsPage : Page
    {
        public PlacedTrapsPage()
        {
            this.InitializeComponent();

            map.Loaded += Map_Loaded;
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

                map.ZoomLevel = 16;

                LoadUserTraps();
            }
            else
            {
                AppShell.Current.AppFrame.Navigate(typeof(HomePage));
            }
        }

        private async void LoadUserTraps()
        {
            try
            {
                List<ArmedTrapApiResult> response = await TrapApiService.ListArmed();

                if (response != null && response.Count > 0)
                {
                    List<BasicGeoposition> basicPositions = new List<BasicGeoposition>();

                    foreach (var trapPlanted in response)
                    {
                        RandomAccessStreamReference mapIconStreamReference = null;

                        if (trapPlanted.NameKey == ETrap.BEAR.ToString())
                        {
                            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/trap_bear_trap_selected_icon.png"));
                        }
                        else if (trapPlanted.NameKey == ETrap.MINE.ToString())
                        {
                            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/trap_mine_trap_selected_icon.png"));
                        }
                        else if (trapPlanted.NameKey == ETrap.PIT.ToString())
                        {
                            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/trap_pit_trap_selected_icon.png"));
                        }
                        else if (trapPlanted.NameKey == ETrap.DOGS.ToString())
                        {
                            mapIconStreamReference = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/trap_dogs_trap_selected_icon.png"));
                        }
                        else
                        {
                            continue;
                        }

                        MapIcon mapIcon = new MapIcon();

                        mapIcon.Location = new Geopoint(new BasicGeoposition() { Latitude = trapPlanted.Latitude, Longitude = trapPlanted.Longitude });
                        mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                        mapIcon.Title = trapPlanted.Date.ToLocalTime().ToString();
                        mapIcon.Image = mapIconStreamReference;
                        mapIcon.ZIndex = 0;

                        map.MapElements.Add(mapIcon);

                        basicPositions.Add(new BasicGeoposition() { Latitude = trapPlanted.Latitude, Longitude = trapPlanted.Longitude });
                    }

                    if (response.Count > 1)
                    {
                        await map.TrySetViewBoundsAsync(GeoboundingBox.TryCompute(basicPositions), null, MapAnimationKind.Default);
                    }
                    else
                    {
                        map.Center = new Geopoint(new BasicGeoposition()
                        {
                            Latitude = response[0].Latitude,
                            Longitude = response[0].Longitude
                        });
                    }
                }
                else
                {
                    var dialog = new MessageDialog("You didn't put traps :(");

                    dialog.Title = "Warning";

                    dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });

                    var res = await dialog.ShowAsync();

                    if ((int)res.Id == 0)
                    {
                        AppShell.Current.AppFrame.Navigate(typeof(HomePage));
                    }
                }
            }
            catch (Exception exception)
            {

            }
        }
    }
}