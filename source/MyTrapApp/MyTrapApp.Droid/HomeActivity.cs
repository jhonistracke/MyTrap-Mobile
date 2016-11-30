using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Utils;
using System;
using System.Threading.Tasks;
using Fragment = Android.App.Fragment;

namespace MyTrapApp.Droid
{
    [Activity(Name = "com.mytrap.HomeActivity", ScreenOrientation = ScreenOrientation.Portrait)]
    public class HomeActivity : Activity, NavigationDrawerFragment.NavigationDrawerCallbacks
    {
        private NavigationDrawerFragment mNavigationDrawerFragment;
        private DrawerLayout mDrawerLayout;
        private string mTitle;

        private Fragment homeFragment;
        private Fragment actualFragment;
        private Fragment armFragment;

        private int currentFragmentPosition;

        private bool showNotification = false;

        public const int HOME_MENU_INDEX = 1;
        public const int PLANT_MENU_INDEX = 2;
        public const int BUY_MENU_INDEX = 3;
        public const int ARMED_TRAPS = 4;

        public bool firstOpen = true;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            try
            {
                SetContentView(MyTrap.Droid.Resource.Layout.activity_home);

                mNavigationDrawerFragment = (NavigationDrawerFragment)FragmentManager.FindFragmentById(MyTrap.Droid.Resource.Id.navigation_drawer);

                mTitle = Title;

                mDrawerLayout = FindViewById<DrawerLayout>(MyTrap.Droid.Resource.Id.drawer_layout);

                mNavigationDrawerFragment.SetUp(MyTrap.Droid.Resource.Id.navigation_drawer, mDrawerLayout);

                if (Intent.Extras != null && Intent.Extras.GetBoolean("showNotification"))
                {
                    showNotification = true;
                }

                ActionBar.SetCustomView(MyTrap.Droid.Resource.Layout.action_bar);

                ActionBar.SetDisplayShowTitleEnabled(false);
                ActionBar.SetDisplayShowCustomEnabled(true);

                SendBroadcast(new Intent("RESTART_SERVICE"));
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public void OnNavigationDrawerItemSelected(int position)
        {
            try
            {
                if (!IsFinishing)
                {
                    FragmentManager.BeginTransaction().Replace(MyTrap.Droid.Resource.Id.container, PlaceholderFragment.newInstance(position + 1)).Commit();
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public void OnSectionAttached(int number, bool update = false)
        {
            try
            {
                if (armFragment != null)
                {
                    ((PlantFragment)armFragment).StopMonitoring();

                    armFragment = null;
                }

                currentFragmentPosition = number;

                actualFragment = new Fragment();

                if (showNotification)
                {
                    showNotification = false;

                    var notificationFragment = new TrapNotificationFragment();

                    notificationFragment.pointsEarned = Convert.ToInt32(Intent.Extras.GetString("points"));
                    notificationFragment.trapNameKey = Intent.Extras.GetString("trap");
                    notificationFragment.latitude = float.Parse(Intent.Extras.GetString("lat").Replace(",", "."));
                    notificationFragment.longitude = float.Parse(Intent.Extras.GetString("lng").Replace(",", "."));
                    notificationFragment.date = DateUtils.StringToDate(Intent.Extras.GetString("date"));
                    notificationFragment.date = notificationFragment.date.ToLocalTime();
                    notificationFragment.otherUserName = Intent.Extras.GetString("userName");
                    notificationFragment.otherUserImage = Intent.Extras.GetString("img");

                    var isOwner = Intent.Extras.GetString("owner");

                    notificationFragment.isOwner = !string.IsNullOrEmpty(isOwner);

                    actualFragment = notificationFragment;
                }
                else if (number == HOME_MENU_INDEX)
                {
                    if (homeFragment == null)
                    {
                        homeFragment = new HomeFragment();
                    }
                    else if (homeFragment != null && update)
                    {
                        ((HomeFragment)homeFragment).RequestProfile(true);
                    }

                    actualFragment = homeFragment;
                }
                else if (number == PLANT_MENU_INDEX)
                {
                    if (AppStatus.UserLogged.ContainsTraps())
                    {
                        actualFragment = new PlantFragment();
                        armFragment = actualFragment;
                    }
                    else
                    {
                        actualFragment = new ShopFragment();
                    }
                }
                else if (number == BUY_MENU_INDEX)
                {
                    actualFragment = new ShopFragment();
                }
                else if (number == ARMED_TRAPS)
                {
                    actualFragment = new ArmedTrapsFragment();
                }
                else
                {
                    actualFragment = null;
                }

                if (actualFragment != null)
                {
                    if (!IsFinishing)
                    {
                        FragmentManager.BeginTransaction().Replace(MyTrap.Droid.Resource.Id.container, actualFragment).Commit();
                    }
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        protected override void OnDestroy()
        {
            try
            {
                SendBroadcast(new Intent("RESTART_SERVICE"));

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private class PlaceholderFragment : Fragment
        {
            private static string ARG_SECTION_NUMBER = "section_number";

            public static PlaceholderFragment newInstance(int sectionNumber)
            {
                PlaceholderFragment fragment = new PlaceholderFragment();

                Bundle args = new Bundle();
                args.PutInt(ARG_SECTION_NUMBER, sectionNumber);
                fragment.Arguments = args;

                return fragment;
            }

            public PlaceholderFragment() { }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                View rootView = inflater.Inflate(MyTrap.Droid.Resource.Layout.fragment_home, container, false);

                return rootView;
            }

            public override void OnAttach(Activity activity)
            {
                base.OnAttach(activity);

                ((HomeActivity)activity).OnSectionAttached(Arguments.GetInt(ARG_SECTION_NUMBER));
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (actualFragment != null && data != null)
            {
                actualFragment.OnActivityResult(requestCode, resultCode, data);
            }
        }
    }
}