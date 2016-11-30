using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MyTrapApp.Droid.Utils;
using Fragment = Android.App.Fragment;

namespace MyTrapApp.Droid
{
    [Activity(Name = "com.mytrap.NavigationDrawerFragment")]
    public class NavigationDrawerFragment : Fragment
    {
        private static string STATE_SELECTED_POSITION = "selected_navigation_drawer_position";

        private NavigationDrawerCallbacks mCallbacks;

        private ActionBarDrawerToggle mDrawerToggle;

        private DrawerLayout mDrawerLayout;
        private ListView mDrawerListView;
        private View mFragmentContainerView;

        private int mCurrentSelectedPosition = 0;
        private bool mFromSavedInstanceState;

        public NavigationDrawerFragment()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                if (savedInstanceState != null)
                {
                    mCurrentSelectedPosition = savedInstanceState.GetInt(STATE_SELECTED_POSITION);
                    mFromSavedInstanceState = true;
                }

                SelectItem(mCurrentSelectedPosition);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            try
            {
                base.OnActivityCreated(savedInstanceState);

                SetHasOptionsMenu(true);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                mDrawerListView = (ListView)inflater.Inflate(MyTrap.Droid.Resource.Layout.fragment_navigation_drawer, container, false);

                var myAdapterOnClickListener = new MyViewOnClickListener();
                myAdapterOnClickListener.owner = this;

                mDrawerListView.OnItemClickListener = myAdapterOnClickListener;

                ArrayAdapter adapter = new ArrayAdapter(GetActionBar().ThemedContext, Android.Resource.Layout.SimpleListItemActivated1, new string[] {
                Resources.GetString(MyTrap.Droid.Resource.String.home_left_menu),
                Resources.GetString(MyTrap.Droid.Resource.String.arm_left_menu),
                Resources.GetString(MyTrap.Droid.Resource.String.shop_left_menu),
                Resources.GetString(MyTrap.Droid.Resource.String.traps_armed_left_menu),
            });

                mDrawerListView.SetAdapter(adapter);

                mDrawerListView.SetItemChecked(mCurrentSelectedPosition, true);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }

            return mDrawerListView;
        }

        public bool IsDrawerOpen()
        {
            return mDrawerLayout != null && mDrawerLayout.IsDrawerOpen(mFragmentContainerView);
        }

        public void SetUp(int fragmentId, DrawerLayout drawerLayout)
        {
            try
            {
                mFragmentContainerView = Activity.FindViewById(fragmentId);

                mDrawerLayout = drawerLayout;

                mDrawerLayout.SetDrawerShadow(MyTrap.Droid.Resource.Drawable.drawer_shadow, GravityCompat.Start);

                ActionBar actionBar = GetActionBar();

                actionBar.SetDisplayHomeAsUpEnabled(true);
                actionBar.SetHomeButtonEnabled(true);

                mDrawerToggle = new MyActionBarDrawerToggle(Activity, mDrawerLayout, MyTrap.Droid.Resource.Drawable.side_menu, MyTrap.Droid.Resource.String.navigation_drawer_open, MyTrap.Droid.Resource.String.navigation_drawer_close);

                ((MyActionBarDrawerToggle)mDrawerToggle).owner = this;

                var myRunnable = new MyRunnablePost();
                myRunnable.owner = this;

                mDrawerLayout.Post(myRunnable);

                mDrawerLayout.SetDrawerListener(mDrawerToggle);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void SelectItem(int position)
        {
            try
            {
                mCurrentSelectedPosition = position;

                if (mDrawerListView != null)
                {
                    mDrawerListView.SetItemChecked(position, true);
                }

                if (mDrawerLayout != null)
                {
                    mDrawerLayout.CloseDrawer(mFragmentContainerView);
                }

                if (mCallbacks != null)
                {
                    mCallbacks.OnNavigationDrawerItemSelected(position);
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override void OnAttach(Activity activity)
        {
            try
            {
                base.OnAttach(activity);

                mCallbacks = (NavigationDrawerCallbacks)activity;
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override void OnDetach()
        {
            try
            {
                base.OnDetach();

                mCallbacks = null;
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            try
            {
                base.OnSaveInstanceState(outState);

                outState.PutInt(STATE_SELECTED_POSITION, mCurrentSelectedPosition);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override void OnConfigurationChanged(Android.Content.Res.Configuration newConfig)
        {
            try
            {
                base.OnConfigurationChanged(newConfig);

                mDrawerToggle.OnConfigurationChanged(newConfig);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            try
            {
                if (mDrawerLayout != null && IsDrawerOpen())
                {
                    inflater.Inflate(MyTrap.Droid.Resource.Layout.menu_global, menu);
                    ShowGlobalContextActionBar();
                }

                base.OnCreateOptionsMenu(menu, inflater);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            try
            {
                if (mDrawerToggle.OnOptionsItemSelected(item))
                {
                    return true;
                }

                return base.OnOptionsItemSelected(item);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }

            return false;
        }

        private void ShowGlobalContextActionBar()
        {
            try
            {
                ActionBar actionBar = GetActionBar();

                actionBar.SetDisplayShowTitleEnabled(true);
                actionBar.NavigationMode = ActionBarNavigationMode.Standard;
                actionBar.SetTitle(MyTrap.Droid.Resource.String.app_name);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private ActionBar GetActionBar()
        {
            return Activity.ActionBar;
        }

        public interface NavigationDrawerCallbacks
        {
            void OnNavigationDrawerItemSelected(int position);
        }

        internal class MyViewOnClickListener : Java.Lang.Object, AdapterView.IOnItemClickListener
        {
            public NavigationDrawerFragment owner { get; set; }

            public void OnItemClick(AdapterView parent, View view, int position, long id)
            {
                owner.SelectItem(position);
            }
        }

        internal class MyRunnablePost : Java.Lang.Object, IRunnable
        {
            public NavigationDrawerFragment owner { get; set; }

            public void Run()
            {
                owner.mDrawerToggle.SyncState();
            }
        }

        internal class MyActionBarDrawerToggle : ActionBarDrawerToggle
        {
            public NavigationDrawerFragment owner { get; set; }

            public MyActionBarDrawerToggle(Activity activity, DrawerLayout layout, int imgRes, int openRes, int closeRes)
                : base(activity, layout, imgRes, openRes, closeRes)
            {

            }

            public override void OnDrawerClosed(View drawerView)
            {
                base.OnDrawerClosed(drawerView);

                if (!owner.IsAdded)
                {
                    return;
                }

                owner.Activity.InvalidateOptionsMenu();
            }

            public override void OnDrawerOpened(View drawerView)
            {
                base.OnDrawerOpened(drawerView);

                if (!owner.IsAdded)
                {
                    return;
                }

                owner.Activity.InvalidateOptionsMenu();
            }
        }
    }
}