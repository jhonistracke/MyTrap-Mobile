using Android.Content;
using Android.Views;
using Android.Widget;
using MyTrap.Droid;
using MyTrapApp.Droid.Utils;
using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Result;
using System;
using System.Collections.Generic;

namespace MyTrapApp.Droid
{
    public class TrapItemShopListAdapter : BaseAdapter
    {
        private LayoutInflater mInflater;
        private ShopFragment shopFragment;
        private List<AvailableTrapApiResult> itens = new List<AvailableTrapApiResult>();

        public TrapItemShopListAdapter(Context context, List<AvailableTrapApiResult> itens, ShopFragment fragment)
        {
            this.itens = itens;
            shopFragment = fragment;
            mInflater = LayoutInflater.From(context);
        }

        public override int Count
        {
            get { if (itens != null) { return itens.Count; } else { return 0; } }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return null;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View view, ViewGroup parent)
        {
            try
            {
                if (view == null)
                {
                    view = mInflater.Inflate(Resource.Layout.trap_to_buy_list_item, null);
                }

                AvailableTrapApiResult item = itens[position];

                TextView txt_qty_item_buy = view.FindViewById<TextView>(Resource.Id.txt_qty_item_buy);
                TextView txt_value_item_buy = view.FindViewById<TextView>(Resource.Id.txt_value_item_buy);

                Button btn_trap_item_buy = view.FindViewById<Button>(Resource.Id.btn_trap_item_buy);

                ImageView img_trap_item_buy = view.FindViewById<ImageView>(Resource.Id.img_trap_item_buy);

                if (item.NameKey.Equals(ETrap.BEAR))
                {
                    img_trap_item_buy.SetImageDrawable(view.Context.Resources.GetDrawable(Resource.Drawable.trap_bear_trap_selected_icon));
                }
                else if (item.NameKey.Equals(ETrap.MINE))
                {
                    img_trap_item_buy.SetImageDrawable(view.Context.Resources.GetDrawable(Resource.Drawable.trap_mine_trap_selected_icon));
                }
                else if (item.NameKey.Equals(ETrap.PIT))
                {
                    img_trap_item_buy.SetImageDrawable(view.Context.Resources.GetDrawable(Resource.Drawable.trap_pit_trap_selected_icon));
                }
                else if (item.NameKey.Equals(ETrap.DOGS))
                {
                    img_trap_item_buy.SetImageDrawable(view.Context.Resources.GetDrawable(Resource.Drawable.trap_dogs_trap_selected_icon));
                }
                else
                {
                    return null;
                }

                btn_trap_item_buy.SetOnClickListener(new MyImageBuyClickListener(item, shopFragment));

                txt_qty_item_buy.Text = item.Amount.ToString() + "x";
                txt_value_item_buy.Text = item.Price;
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }

            return view;
        }

        public class MyImageBuyClickListener : Java.Lang.Object, View.IOnClickListener
        {
            AvailableTrapApiResult availableTrap;
            ShopFragment shopFragment;

            public MyImageBuyClickListener(AvailableTrapApiResult availableTrap, ShopFragment shopFragment)
            {
                this.availableTrap = availableTrap;
                this.shopFragment = shopFragment;
            }

            public void OnClick(View v)
            {
                if (v.Id == Resource.Id.btn_trap_item_buy)
                {
                    shopFragment.TryBuy(availableTrap);
                }
            }
        }
    }
}