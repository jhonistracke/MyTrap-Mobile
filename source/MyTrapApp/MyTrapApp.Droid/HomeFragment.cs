using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using HockeyApp;
using HockeyApp.Metrics;
using MyTrap.Droid;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Models.Result;
using MyTrapApp.Services;
using System;

namespace MyTrapApp.Droid
{
    [Activity(Name = "com.mytrap.HomeFragment", ScreenOrientation = ScreenOrientation.Portrait)]
    public class HomeFragment : Fragment
    {
        private ImageView img_home_profile;
        private TextView lbl_home_name;
        private TextView lbl_home_points;
        private TextView lbl_home_points_value;
        private TextView lbl_home_traps;
        private TextView lbl_home_traps_value;
        private TextView lbl_home_other_msg;
        private Button btn_home_arm;
        private Button btn_home_buy;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                view = inflater.Inflate(Resource.Layout.fragment_home, container, false);

                InitializeViews(view);

                CrashManager.Register(Activity);
                MetricsManager.Register(Activity, Activity.Application);

                HomeActivity homeActivity = (HomeActivity)Activity;

                if (homeActivity.firstOpen)
                {
                    RequestProfile(true);
                    homeActivity.firstOpen = false;
                }
                else
                {
                    RequestProfile(false);
                }
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
                img_home_profile = (ImageView)view.FindViewById(Resource.Id.img_home_profile);
                lbl_home_name = (TextView)view.FindViewById(Resource.Id.lbl_home_name);
                lbl_home_points = (TextView)view.FindViewById(Resource.Id.lbl_home_points);
                lbl_home_points_value = (TextView)view.FindViewById(Resource.Id.lbl_home_points_value);
                lbl_home_traps = (TextView)view.FindViewById(Resource.Id.lbl_home_traps);
                lbl_home_traps_value = (TextView)view.FindViewById(Resource.Id.lbl_home_traps_value);
                lbl_home_other_msg = (TextView)view.FindViewById(Resource.Id.lbl_home_other_msg);
                btn_home_arm = (Button)view.FindViewById(Resource.Id.btn_home_arm);
                btn_home_buy = (Button)view.FindViewById(Resource.Id.btn_home_buy);

                btn_home_arm.Click += Btn_home_arm_Click;
                btn_home_buy.Click += Btn_home_buy_Click;
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void Btn_home_buy_Click(object sender, EventArgs e)
        {
            HomeActivity homeActivity = (HomeActivity)Activity;
            homeActivity.OnSectionAttached(HomeActivity.BUY_MENU_INDEX);
        }

        private void Btn_home_arm_Click(object sender, EventArgs e)
        {
            HomeActivity homeActivity = (HomeActivity)Activity;
            homeActivity.OnSectionAttached(HomeActivity.PLANT_MENU_INDEX);
        }

        public async void RequestProfile(bool updateUser)
        {
            try
            {
                UpdateInfoProfile();

                if (updateUser)
                {
                    var response = await UserApiService.Get();

                    if (response != null && !response.Error)
                    {
                        AppStatus.UserLogged = response;

                        UpdateInfoProfile();
                    }
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void UpdateInfoProfile()
        {
            try
            {
                img_home_profile.SetBackgroundResource(Resource.Drawable.profile_picture_background);

                Bitmap profileBitmap = null;

                if (AppStatus.UserLogged.ProfilePicture != null && AppStatus.UserLogged.ContainsProfilePictureBase64())
                {
                    profileBitmap = MyTrapDroidFunctions.GetImageFromBase64(AppStatus.UserLogged.ProfilePicture.Base64);
                }
                else
                {
                    profileBitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.user_empty);
                }

                profileBitmap = MyTrapDroidFunctions.GetCroppedBitmap(profileBitmap);

                img_home_profile.SetImageBitmap(profileBitmap);

                lbl_home_name.SetText(AppStatus.UserLogged.Name, TextView.BufferType.Normal);
                lbl_home_points_value.SetText(AppStatus.UserLogged.Points.ToString(), TextView.BufferType.Normal);
                lbl_home_traps_value.SetText(AppStatus.UserLogged.GetAmountTraps().ToString(), TextView.BufferType.Normal);

                lbl_home_other_msg.Visibility = ViewStates.Visible;

                if (AppStatus.UserLogged.GetAmountTraps() == 0)
                {
                    lbl_home_other_msg.SetText(Resource.String.buy_a_trap_msg);
                }
                else
                {
                    lbl_home_other_msg.SetText(Resource.String.arm_a_trap_msg);
                }

                ShowHideButtons(AppStatus.UserLogged);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void ShowHideButtons(UserApiResult user)
        {
            try
            {
                if (user.ContainsTraps())
                {
                    btn_home_arm.Visibility = ViewStates.Visible;
                }
                else
                {
                    btn_home_buy.Visibility = ViewStates.Visible;
                }
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
        }

        public override void OnPause()
        {
            base.OnPause();

            Tracking.StopUsage(Activity);
        }
    }
}