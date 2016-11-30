using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;
using HockeyApp;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Result;
using MyTrapApp.Services;
using System;
using System.Collections.Generic;

namespace MyTrapApp.Droid
{
    [Activity(Name = "com.mytrap.ArmedTrapsActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class ArmedTrapsFragment : Fragment
    {
        MapFragment mapFrag;
        GoogleMap googleMap;

        ProgressDialog progressDialog;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                view = inflater.Inflate(MyTrap.Droid.Resource.Layout.fragment_armed_traps, container, false);

                InitializeViews(view, savedInstanceState);

                progressDialog = ProgressDialog.Show(Activity, Resources.GetString(MyTrap.Droid.Resource.String.loading), Resources.GetString(MyTrap.Droid.Resource.String.loading_traps));
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }

            return view;
        }

        private void InitializeViews(View view, Bundle savedInstanceState)
        {
            try
            {
                mapFrag = (MapFragment)FragmentManager.FindFragmentById(MyTrap.Droid.Resource.Id.map_traps_armed);

                mapFrag.GetMapAsync(new MyOnMapReadyCallback(this));
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private async void LoadUserTraps()
        {
            try
            {
                List<ArmedTrapApiResult> response = await TrapApiService.ListArmed();

                if (response != null && response.Count > 0)
                {
                    var boundsBuilders = new LatLngBounds.Builder();

                    foreach (var trapPlanted in response)
                    {
                        var idIcon = 0;

                        if (trapPlanted.NameKey == ETrap.BEAR.ToString())
                        {
                            idIcon = MyTrap.Droid.Resource.Drawable.trap_bear_trap_selected_icon;
                        }
                        else if (trapPlanted.NameKey == ETrap.MINE.ToString())
                        {
                            idIcon = MyTrap.Droid.Resource.Drawable.trap_mine_trap_selected_icon;
                        }
                        else if (trapPlanted.NameKey == ETrap.PIT.ToString())
                        {
                            idIcon = MyTrap.Droid.Resource.Drawable.trap_pit_trap_selected_icon;
                        }
                        else if (trapPlanted.NameKey == ETrap.DOGS.ToString())
                        {
                            idIcon = MyTrap.Droid.Resource.Drawable.trap_dogs_trap_selected_icon;
                        }
                        //else if (trapPlanted.NameKey == ETrap.LOOP.ToString())
                        else
                        {
                            //idIcon = MyTrap.Droid.Resource.Drawable.trap_loop_trap_icon;
                            continue;
                        }

                        var position = new LatLng(trapPlanted.Latitude, trapPlanted.Longitude);

                        googleMap.AddMarker(new MarkerOptions()
                            .SetPosition(position)
                            .SetTitle(trapPlanted.Date.ToLocalTime().ToString(Resources.GetString(MyTrap.Droid.Resource.String.format_date_hour_minute)))
                            .SetIcon(BitmapDescriptorFactory.FromResource(idIcon))
                         );

                        int fillColor = Int32.Parse("80f15f4b", System.Globalization.NumberStyles.HexNumber);

                        CircleOptions circleOptions = new CircleOptions();

                        circleOptions.InvokeCenter(position);
                        circleOptions.InvokeFillColor(fillColor);
                        circleOptions.InvokeStrokeWidth(3);

                        googleMap.AddCircle(circleOptions);

                        boundsBuilders.Include(new LatLng(trapPlanted.Latitude, trapPlanted.Longitude));
                    }

                    if (response.Count > 1)
                    {
                        var bounds = boundsBuilders.Build();

                        googleMap.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds, 100));
                    }
                    else
                    {
                        googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(response[0].Latitude, response[0].Longitude), 16));
                    }
                }
                else
                {
                    HomeActivity homeActivity = (HomeActivity)Activity;

                    new AlertDialog.Builder(Activity)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .SetTitle(MyTrap.Droid.Resource.String.alert_title_error)
                    .SetMessage(MyTrap.Droid.Resource.String.no_traps_armed)
                    .SetPositiveButton(MyTrap.Droid.Resource.String.ok, new MyDialogClickListener(homeActivity))
                    .SetCancelable(false)
                    .Show();
                }

                progressDialog.Cancel();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override void OnResume()
        {
            base.OnResume();

            Tracking.StartUsage(Activity);

            if (mapFrag != null)
            {
                mapFrag.OnResume();
            }
        }

        public override void OnPause()
        {
            base.OnPause();

            Tracking.StopUsage(Activity);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            if (mapFrag != null && !Activity.IsFinishing)
            {
                FragmentManager.BeginTransaction().Remove(mapFrag).Commit();
            }
        }

        public override void OnLowMemory()
        {
            base.OnLowMemory();

            if (mapFrag != null)
            {
                mapFrag.OnLowMemory();
            }
        }

        private class MyDialogClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            HomeActivity homeActivity = null;

            public MyDialogClickListener(HomeActivity activity)
            {
                homeActivity = activity;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                if (AppStatus.UserLogged.ContainsTraps())
                {
                    homeActivity.OnSectionAttached(HomeActivity.PLANT_MENU_INDEX);
                }
                else
                {
                    homeActivity.OnSectionAttached(HomeActivity.BUY_MENU_INDEX);
                }
            }
        }

        private class MyOnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
        {
            public ArmedTrapsFragment trapsPlantedFragment;

            public MyOnMapReadyCallback(ArmedTrapsFragment fragment)
            {
                trapsPlantedFragment = fragment;
            }

            public void OnMapReady(GoogleMap map)
            {
                trapsPlantedFragment.googleMap = map;

                trapsPlantedFragment.googleMap.UiSettings.MyLocationButtonEnabled = false;
                trapsPlantedFragment.googleMap.MyLocationEnabled = false;
                trapsPlantedFragment.googleMap.UiSettings.MapToolbarEnabled = false;

                trapsPlantedFragment.LoadUserTraps();
            }
        }
    }
}