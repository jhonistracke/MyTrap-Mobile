using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using HockeyApp;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Models.Enums;
using MyTrapApp.Services;
using MyTrapApp.Utils;
using System;

namespace MyTrapApp.Droid
{
    [Activity(Name = "com.mytrap.TrapNotificationFragment", ScreenOrientation = ScreenOrientation.Portrait)]
    public class TrapNotificationFragment : Fragment
    {
        MapFragment mapFrag;
        GoogleMap googleMap;

        private ImageView img_notification_other_user;
        private TextView lbl_notification_msg;

        public int pointsEarned;
        public string trapNameKey;
        public float latitude;
        public float longitude;
        public DateTime date;
        public string otherUserName;
        public string otherUserImage;

        public bool isOwner;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                view = inflater.Inflate(MyTrap.Droid.Resource.Layout.fragment_trap_notification, container, false);

                InitializeViews(view);

                UpdateInterface();

                UpdateUser();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }

            return view;
        }

        private void InitializeViews(View view)
        {
            try
            {
                img_notification_other_user = (ImageView)view.FindViewById(MyTrap.Droid.Resource.Id.img_notification_other_user);
                lbl_notification_msg = (TextView)view.FindViewById(MyTrap.Droid.Resource.Id.lbl_notification_msg);

                mapFrag = (MapFragment)FragmentManager.FindFragmentById(MyTrap.Droid.Resource.Id.map_notification);

                mapFrag.GetMapAsync(new MyOnMapReadyCallback(this));
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private async void UpdateUser()
        {
            var result = await UserApiService.Get();

            if (!result.Error)
            {
                AppStatus.UserLogged = result;
            }
        }

        private void UpdateInterface()
        {
            img_notification_other_user.SetBackgroundResource(MyTrap.Droid.Resource.Drawable.profile_picture_background);

            Bitmap profileBitmap = null;

            if (!string.IsNullOrEmpty(otherUserImage))
            {
                string base64 = Functions.GetBase64ImageFromUrl(otherUserImage);

                profileBitmap = MyTrapDroidFunctions.GetImageFromBase64(base64);
            }
            else
            {
                profileBitmap = BitmapFactory.DecodeResource(Resources, MyTrap.Droid.Resource.Drawable.user_empty);
            }

            profileBitmap = MyTrapDroidFunctions.GetCroppedBitmap(profileBitmap);

            img_notification_other_user.SetImageBitmap(profileBitmap);

            if (isOwner)
            {
                string msg = Resources.GetString(MyTrap.Droid.Resource.String.you_caught_other_user);

                msg = string.Format(msg, otherUserName, pointsEarned);

                lbl_notification_msg.Text = msg;
            }
            else
            {
                string msg = Resources.GetString(MyTrap.Droid.Resource.String.you_caught_by_user);

                msg = string.Format(msg, otherUserName);

                lbl_notification_msg.Text = msg;
            }
        }

        private void LoadMapExplosion()
        {
            var idIcon = 0;

            if (trapNameKey == ETrap.BEAR.ToString())
            {
                idIcon = MyTrap.Droid.Resource.Drawable.trap_bear_trap_selected_icon;
            }
            else if (trapNameKey == ETrap.MINE.ToString())
            {
                idIcon = MyTrap.Droid.Resource.Drawable.trap_mine_trap_selected_icon;
            }
            else if (trapNameKey == ETrap.PIT.ToString())
            {
                idIcon = MyTrap.Droid.Resource.Drawable.trap_pit_trap_selected_icon;
            }
            else if (trapNameKey == ETrap.DOGS.ToString())
            {
                idIcon = MyTrap.Droid.Resource.Drawable.trap_dogs_trap_selected_icon;
            }
            else
            {
                return;
            }

            var position = new LatLng(latitude, longitude);

            googleMap.AddMarker(new MarkerOptions()
                .SetPosition(position)
                .SetTitle(date.ToString(Resources.GetString(MyTrap.Droid.Resource.String.format_date_hour_minute)))
                .SetIcon(BitmapDescriptorFactory.FromResource(idIcon))
             );

            int fillColor = int.Parse("80f15f4b", System.Globalization.NumberStyles.HexNumber);

            CircleOptions circleOptions = new CircleOptions();

            circleOptions.InvokeCenter(position);
            circleOptions.InvokeFillColor(fillColor);
            circleOptions.InvokeStrokeWidth(3);

            googleMap.AddCircle(circleOptions);

            googleMap.MoveCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(latitude, longitude), 16));
        }

        private class MyOnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
        {
            public TrapNotificationFragment TrapNotificationFragment;

            public MyOnMapReadyCallback(TrapNotificationFragment fragment)
            {
                TrapNotificationFragment = fragment;
            }

            public void OnMapReady(GoogleMap map)
            {
                TrapNotificationFragment.googleMap = map;

                TrapNotificationFragment.googleMap.UiSettings.MyLocationButtonEnabled = false;
                TrapNotificationFragment.googleMap.MyLocationEnabled = false;
                TrapNotificationFragment.googleMap.UiSettings.MapToolbarEnabled = false;

                TrapNotificationFragment.LoadMapExplosion();
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
    }
}