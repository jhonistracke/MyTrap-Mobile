using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using HockeyApp;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Request;
using MyTrapApp.Models.Result;
using MyTrapApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.InAppBilling;

namespace MyTrapApp.Droid
{
    [Activity(Name = "com.mytrap.ShopFragment", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, ScreenOrientation = ScreenOrientation.Portrait)]
    public class ShopFragment : Fragment
    {
        const string APP_LICENSE_PUBLIC_KEY = "";

        private InAppBillingServiceConnection _serviceConnection;

        private IList<Product> products;

        List<AvailableTrapApiResult> availableTraps;

        TextView lbl_title_my_traps;

        ListView list_traps_to_buy;
        LinearLayout group_my_traps;

        BuyIntentApiRequest pendingBuyIntent;

        ProgressDialog progressDialog;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = null;

            try
            {
                view = inflater.Inflate(MyTrap.Droid.Resource.Layout.fragment_shop, container, false);

                InitializeViews(view);

                progressDialog = ProgressDialog.Show(Activity, Resources.GetString(MyTrap.Droid.Resource.String.loading), Resources.GetString(MyTrap.Droid.Resource.String.loading_inventory));

                LoadTraps();

                InitializeGooglePlayBilling();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }

            return view;
        }

        private void InitializeGooglePlayBilling()
        {
            _serviceConnection = new Xamarin.InAppBilling.InAppBillingServiceConnection(Activity, APP_LICENSE_PUBLIC_KEY);
        }

        private void InitializeViews(View view)
        {
            list_traps_to_buy = view.FindViewById<ListView>(MyTrap.Droid.Resource.Id.list_traps_to_buy);
            group_my_traps = view.FindViewById<LinearLayout>(MyTrap.Droid.Resource.Id.group_my_traps);

            lbl_title_my_traps = view.FindViewById<TextView>(MyTrap.Droid.Resource.Id.lbl_title_my_traps);

            DrawMyTraps();
        }

        private async void LoadTraps()
        {
            try
            {
                var response = await PurchaseApiService.ListAvailableTraps();

                if (response != null)
                {
                    availableTraps = response;

                    LoadGooglePrices();
                }
                else
                {
                    progressDialog.Cancel();

                    HomeActivity homeActivity = (HomeActivity)Activity;
                    homeActivity.OnSectionAttached(HomeActivity.HOME_MENU_INDEX, true);
                }

                if (!AppStatus.UserLogged.ContainsTraps())
                {
                    lbl_title_my_traps.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void LoadGooglePrices()
        {
            try
            {
                var ids = new List<string>();

                foreach (var item in availableTraps)
                {
                    ids.Add(item.KeyGoogle);
                }

                _serviceConnection.OnConnected += async () =>
                {
                    products = await _serviceConnection.BillingHandler.QueryInventoryAsync(ids, Xamarin.InAppBilling.ItemType.Product);

                    _serviceConnection.BillingHandler.OnProductPurchased += BillingHandler_OnProductPurchased;

                    _serviceConnection.BillingHandler.OnProductPurchasedError += BillingHandler_OnProductPurchasedError;

                    _serviceConnection.BillingHandler.OnUserCanceled += BillingHandler_OnUserCanceled;

                    LoadProductPrices(products.ToList());

                    var purchases = _serviceConnection.BillingHandler.GetPurchases(ItemType.Product);

                    if (purchases != null)
                    {
                        foreach (var purchase in purchases)
                        {
                            var intent = await InsertIntent(purchase.ProductId);

                            var result = await RegisterPurchase(intent);

                            if (result)
                            {
                                _serviceConnection.BillingHandler.ConsumePurchase(purchase);
                            }
                            else
                            {
                                InvalidateBuyIntent(intent);
                            }
                        }
                    }

                    progressDialog.Cancel();
                };

                _serviceConnection.Connect();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        public void ShowInfo(AvailableTrapApiResult availableTrap)
        {
            //string msgDistance = Resources.GetString(MyTrap.Droid.Resource.String.trap_buy_info_distance);

            //msgDistance = string.Format(msgDistance, availableTrap.Meters);

            //new AlertDialog.Builder(Activity)
            //    .SetIcon(Android.Resource.Drawable.IcDialogInfo)
            //    .SetMessage(msgDistance)
            //    .Show();
        }

        private void BillingHandler_OnUserCanceled()
        {
            InvalidateBuyIntent(pendingBuyIntent);
        }

        private void BillingHandler_OnProductPurchasedError(int responseCode, string sku)
        {
            InvalidateBuyIntent(pendingBuyIntent);
        }

        private async void BillingHandler_OnProductPurchased(int response, Purchase purchase, string purchaseData, string purchaseSignature)
        {
            var result = await RegisterPurchase(pendingBuyIntent);

            if (result)
            {
                _serviceConnection.BillingHandler.ConsumePurchase(purchase);
            }

            HomeActivity homeActivity = (HomeActivity)Activity;
            homeActivity.OnSectionAttached(HomeActivity.HOME_MENU_INDEX, true);

            progressDialog.Cancel();
        }

        private void LoadProductPrices(List<Product> productsLoaded)
        {
            try
            {
                foreach (var itemGoogle in productsLoaded)
                {
                    foreach (var item in availableTraps)
                    {
                        if (item.KeyGoogle == itemGoogle.ProductId)
                        {
                            item.Price = itemGoogle.Price;
                            break;
                        }
                    }
                }

                TrapItemShopListAdapter adapter = new TrapItemShopListAdapter(Activity.ApplicationContext, availableTraps, this);

                list_traps_to_buy.SetAdapter(adapter);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }

        private void DrawMyTraps()
        {
            if (AppStatus.UserLogged.ContainsTraps())
            {
                foreach (var trap in AppStatus.UserLogged.Traps)
                {
                    if (trap.Amount > 0)
                    {
                        var my_trap_layout = Activity.LayoutInflater.Inflate(MyTrap.Droid.Resource.Layout.my_trap_template, null);

                        var img_my_trap = my_trap_layout.FindViewById<ImageView>(MyTrap.Droid.Resource.Id.img_my_trap);
                        var lbl_amount_my_trap = my_trap_layout.FindViewById<TextView>(MyTrap.Droid.Resource.Id.lbl_qtd_my_trap);

                        if (trap.NameKey.Equals(ETrap.BEAR))
                        {
                            img_my_trap.SetImageDrawable(Resources.GetDrawable(MyTrap.Droid.Resource.Drawable.trap_bear_trap_selected_icon));
                        }
                        else if (trap.NameKey.Equals(ETrap.MINE))
                        {
                            img_my_trap.SetImageDrawable(Resources.GetDrawable(MyTrap.Droid.Resource.Drawable.trap_mine_trap_selected_icon));
                        }
                        else if (trap.NameKey.Equals(ETrap.PIT))
                        {
                            img_my_trap.SetImageDrawable(Resources.GetDrawable(MyTrap.Droid.Resource.Drawable.trap_pit_trap_selected_icon));
                        }
                        else if (trap.NameKey.Equals(ETrap.DOGS))
                        {
                            img_my_trap.SetImageDrawable(Resources.GetDrawable(MyTrap.Droid.Resource.Drawable.trap_dogs_trap_selected_icon));
                        }
                        else
                        {
                            continue;
                        }

                        string qty = trap.Amount + "x";

                        lbl_amount_my_trap.Text = qty;

                        group_my_traps.AddView(my_trap_layout);
                    }
                }
            }
        }

        public async Task<BuyIntentApiRequest> InsertIntent(string productId)
        {
            BuyIntentApiRequest intent = new BuyIntentApiRequest() { StoreKey = productId };

            await PurchaseApiService.InsertBuyIntent(intent);

            return intent;
        }

        public async void TryBuy(AvailableTrapApiResult availableTrap)
        {
            progressDialog = ProgressDialog.Show(Activity, Resources.GetString(MyTrap.Droid.Resource.String.loading), Resources.GetString(MyTrap.Droid.Resource.String.registering_purchase));

            try
            {
                BuyIntentApiRequest intent = new BuyIntentApiRequest() { AvailableTrapId = availableTrap.Id, StoreKey = availableTrap.KeyGoogle };

                await PurchaseApiService.InsertBuyIntent(intent);

                pendingBuyIntent = intent;

                Product product = null;

                foreach (var itemGoogle in products)
                {
                    if (itemGoogle.ProductId == availableTrap.KeyGoogle)
                    {
                        product = itemGoogle;
                        break;
                    }
                }

                if (product != null)
                {
                    //product.ProductId = "android.test.purchased";
                    //product.ProductId = "android.test.canceled";
                    //product.ProductId = "android.test.refunded";

                    //await RegisterPurchase(intent);

                    _serviceConnection.BillingHandler.BuyProduct(product);
                }
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }

            progressDialog.Cancel();
        }

        public async void InvalidateBuyIntent(BuyIntentApiRequest intent)
        {
            await PurchaseApiService.InvalidateBuyIntent(intent);
        }

        public async Task<bool> RegisterPurchase(BuyIntentApiRequest intent)
        {
            bool result = false;

            try
            {
                result = await PurchaseApiService.RegisterPurchase(intent);
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }

            return result;
        }

        public override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (data != null)
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (_serviceConnection != null && _serviceConnection.BillingHandler != null)
                {
                    _serviceConnection.BillingHandler.HandleActivityResult(requestCode, resultCode, data);
                }
            }
        }

        public override void OnDestroy()
        {
            if (_serviceConnection != null && _serviceConnection.Connected)
            {
                _serviceConnection.Disconnect();
            }

            base.OnDestroy();
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