using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.Gms.Common.Apis;
using Android.Gms.Location;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using HockeyApp;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Models.Request;
using MyTrapApp.Models.Result;
using MyTrapApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTrapApp.Droid
{
    [Activity(Name = "com.mytrap.ArmActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class PlantFragment : Fragment, View.IOnClickListener, GoogleApiClient.IConnectionCallbacks, GoogleApiClient.IOnConnectionFailedListener, Android.Gms.Location.ILocationListener
    {
        MapFragment mapFrag;
        GoogleMap googleMap;
        Location lastLocation;
        bool firstLocationLoaded = false;

        LinearLayout group_arm_choose_traps;
        Button btn_arm;

        List<UserTrapApiResult> userAvailableTraps = new List<UserTrapApiResult>();

        UserTrapApiResult selectedTrap;

        Circle circle;
        Marker marker;

        List<ButtonChooseTrap> buttonsForChoose = new List<ButtonChooseTrap>();

        GoogleApiClient apiClient;
        LocationRequest locRequest;

        ProgressDialog progressDialog;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                view = inflater.Inflate(MyTrap.Droid.Resource.Layout.fragment_arm, container, false);

                InitializeViews(view, savedInstanceState);

                progressDialog = ProgressDialog.Show(Activity, Resources.GetString(MyTrap.Droid.Resource.String.loading), Resources.GetString(MyTrap.Droid.Resource.String.getting_target));

                apiClient = new GoogleApiClient.Builder(this.Context, this, this).AddApi(LocationServices.API).Build();

                locRequest = new LocationRequest();
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
                group_arm_choose_traps = view.FindViewById<LinearLayout>(MyTrap.Droid.Resource.Id.group_arm_choose_traps);

                btn_arm = view.FindViewById<Button>(MyTrap.Droid.Resource.Id.btn_arm_trap);

                btn_arm.SetOnClickListener(this);

                mapFrag = (MapFragment)FragmentManager.FindFragmentById(MyTrap.Droid.Resource.Id.map_arm);

                mapFrag.GetMapAsync(new MyOnMapReadyCallback(this));

                LoadUserTraps();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private async void StartMonitoring()
        {
            try
            {
                locRequest.SetPriority(100);
                locRequest.SetFastestInterval(500);
                locRequest.SetInterval(3000);

                await LocationServices.FusedLocationApi.RequestLocationUpdates(apiClient, locRequest, this);
            }
            catch (Exception exception)
            {
                Toast.MakeText(Activity, exception.Message, ToastLength.Long).Show();
            }
        }

        public async void StopMonitoring()
        {
            if (apiClient.IsConnected)
            {
                await LocationServices.FusedLocationApi.RemoveLocationUpdates(apiClient, this);

                apiClient.Disconnect();
            }
        }

        private void LoadUserTraps()
        {
            try
            {
                if (AppStatus.UserLogged.ContainsTraps())
                {
                    AppStatus.UserLogged.Traps = AppStatus.UserLogged.Traps.ToList();

                    foreach (UserTrapApiResult trapUser in AppStatus.UserLogged.Traps.Where(obj => obj.Amount > 0).ToList())
                    {
                        userAvailableTraps.Add(trapUser);

                        ButtonChooseTrap btnTrap = new ButtonChooseTrap(Activity.ApplicationContext);

                        var layoutParameters = new LinearLayout.LayoutParams(160, 160);

                        layoutParameters.SetMargins(10, 5, 10, 5);

                        btnTrap.LayoutParameters = layoutParameters;

                        btnTrap.TrapKey = trapUser.NameKey;

                        btnTrap.SetOnClickListener(this);

                        group_arm_choose_traps.AddView(btnTrap);

                        buttonsForChoose.Add(btnTrap);
                    }

                    UnselectTraps();
                }
                else if (!AppStatus.UserLogged.ContainsTraps())
                {
                    //Nao pode armar pois nao possui
                    return;
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public void UnselectTraps()
        {
            foreach (var button in buttonsForChoose)
            {
                var imageResourceName = "trap_" + button.TrapKey.ToLower() + "_icon";

                var resourceId = Resources.GetIdentifier(imageResourceName, "drawable", Activity.PackageName);

                button.SetBackgroundResource(resourceId);
            }
        }

        public void UpdateRegionMap()
        {
            try
            {
                if (selectedTrap != null)
                {
                    if (circle != null)
                    {
                        circle.Remove();
                    }

                    LatLng position = new LatLng(lastLocation.Latitude, lastLocation.Longitude);

                    int fillColor = int.Parse("80f15f4b", System.Globalization.NumberStyles.HexNumber);

                    CircleOptions circleOptions = new CircleOptions();

                    circleOptions.InvokeCenter(position);
                    //circleOptions.InvokeRadius(selectedTrap.Trap.Meters);
                    circleOptions.InvokeFillColor(fillColor);
                    circleOptions.InvokeStrokeWidth(3);

                    circle = googleMap.AddCircle(circleOptions);
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void UpdateIconMap()
        {
            if (marker != null)
            {
                marker.Remove();
            }

            marker = googleMap.AddMarker(new MarkerOptions()
                .SetPosition(new LatLng(lastLocation.Latitude, lastLocation.Longitude))
                .SetIcon(BitmapDescriptorFactory.FromResource(MyTrap.Droid.Resource.Drawable.icon_trap_map)));
        }

        public void OnClick(View v)
        {
            if (v.Id == btn_arm.Id)
            {
                ArmTrap();
            }
            else if (v is ButtonChooseTrap)
            {
                ButtonChooseTrap button = (ButtonChooseTrap)v;

                SelectTrap(button.TrapKey);
            }
        }

        private void SelectTrap(string trapKey)
        {
            try
            {
                UnselectTraps();

                foreach (UserTrapApiResult trapUser in userAvailableTraps)
                {
                    if (trapUser.NameKey.Equals(trapKey))
                    {
                        selectedTrap = trapUser;

                        foreach (ButtonChooseTrap btn in buttonsForChoose)
                        {
                            if (btn.TrapKey.Equals(trapKey))
                            {
                                var imageResourceName = "trap_" + btn.TrapKey.ToLower() + "_selected_icon";

                                var resourceId = Resources.GetIdentifier(imageResourceName, "drawable", Activity.PackageName);

                                btn.SetBackgroundResource(resourceId);
                            }
                        }

                        break;
                    }
                }

                UpdateRegionMap();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public void ArmTrap()
        {
            try
            {
                if (lastLocation != null && selectedTrap != null)
                {
                    new AlertDialog.Builder(Activity)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .SetTitle(MyTrap.Droid.Resource.String.arm_trap_title_alert)
                    .SetMessage(MyTrap.Droid.Resource.String.msg_confirm_arm_trap)
                    .SetPositiveButton(MyTrap.Droid.Resource.String.yes, new MyDialogClickListener(this))
                    .SetNegativeButton(MyTrap.Droid.Resource.String.no, new MyDialogClickListener(this))
                    .Show();
                }
                else if (lastLocation == null)
                {
                    //Nao foi possivel obter localizacao
                }
                else if (selectedTrap == null)
                {
                    //Selecione uma trapa
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private async void SendArmedTrap()
        {
            try
            {
                progressDialog = ProgressDialog.Show(Activity, Resources.GetString(MyTrap.Droid.Resource.String.loading), Resources.GetString(MyTrap.Droid.Resource.String.arming_trap));

                ArmedTrapApiRequest armedTrap = new ArmedTrapApiRequest();

                armedTrap.NameKey = selectedTrap.NameKey;
                armedTrap.Latitude = (float)lastLocation.Latitude;
                armedTrap.Longitude = (float)lastLocation.Longitude;

                var response = await TrapApiService.Arm(armedTrap);

                if (ResponseValidator.Validate(response, Activity))
                {
                    ShowAlertArmedTrapSuccessfuly();
                }

                progressDialog.Cancel();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void ShowAlertArmedTrapSuccessfuly()
        {
            HomeActivity homeActivity = (HomeActivity)Activity;

            new AlertDialog.Builder(Activity)
            .SetIcon(Android.Resource.Drawable.IcDialogAlert)
            .SetTitle(MyTrap.Droid.Resource.String.arm_trap_title_alert)
            .SetMessage(MyTrap.Droid.Resource.String.msg_success_arm_trap)
            .SetPositiveButton(MyTrap.Droid.Resource.String.ok, new MyDialogClickListener(this, homeActivity))
            .SetCancelable(false)
            .Show();
        }

        public override void OnResume()
        {
            base.OnResume();

            apiClient.Connect();

            Tracking.StartUsage(Activity);

            if (mapFrag != null)
            {
                mapFrag.OnResume();
            }
        }

        public override void OnPause()
        {
            base.OnPause();

            StopMonitoring();

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

        public void OnLocationChanged(Location location)
        {
            lastLocation = location;

            UpdateRegionMap();
            UpdateIconMap();

            if (!firstLocationLoaded)
            {
                progressDialog.Cancel();

                LatLng latLng = new LatLng(location.Latitude, location.Longitude);

                CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngZoom(latLng, 15);
                googleMap.AnimateCamera(cameraUpdate);

                firstLocationLoaded = true;

                //StartMonitoring();
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

        public void OnConnected(Bundle connectionHint)
        {
            StartMonitoring();
        }

        public void OnConnectionSuspended(int cause)
        {

        }

        public void OnConnectionFailed(ConnectionResult result)
        {

        }

        private class MyDialogClickListener : Java.Lang.Object, IDialogInterfaceOnClickListener
        {
            PlantFragment armFragment;
            HomeActivity homeActivity = null;

            public MyDialogClickListener(PlantFragment fragment)
            {
                armFragment = fragment;
            }

            public MyDialogClickListener(PlantFragment fragment, HomeActivity activity)
            {
                armFragment = fragment;
                homeActivity = activity;
            }

            public void OnClick(IDialogInterface dialog, int which)
            {
                if (homeActivity == null)
                {
                    if (which == -1)
                    {
                        armFragment.SendArmedTrap();
                    }
                }
                else
                {
                    homeActivity.OnSectionAttached(HomeActivity.HOME_MENU_INDEX, true);
                }
            }
        }

        private class MyOnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
        {
            public PlantFragment armFragment;

            public MyOnMapReadyCallback(PlantFragment fragment)
            {
                armFragment = fragment;
            }

            public void OnMapReady(GoogleMap map)
            {
                armFragment.googleMap = map;

                armFragment.googleMap.UiSettings.MyLocationButtonEnabled = false;
                armFragment.googleMap.MyLocationEnabled = false;
                armFragment.googleMap.UiSettings.MapToolbarEnabled = false;
            }
        }
    }
}