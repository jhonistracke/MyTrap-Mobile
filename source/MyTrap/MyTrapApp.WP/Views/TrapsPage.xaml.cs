using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Result;
using MyTrapApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.ApplicationModel.Store;
using Windows.UI.Xaml.Controls;

namespace MyTrapApp.WP.Views
{
    public sealed partial class TrapsPage : Page
    {
        List<AvailableTrapApiResult> availableTraps;

        public TrapsPage()
        {
            this.InitializeComponent();

            lblBearAmount.Text = "0x";
            lblDogsAmount.Text = "0x";
            lblMineAmount.Text = "0x";
            lblPitAmount.Text = "0x";

            foreach (var trap in AppStatus.UserLogged.Traps)
            {
                if (trap.NameKey == ETrap.BEAR)
                {
                    lblBearAmount.Text = trap.Amount + "x";
                }
                else if (trap.NameKey == ETrap.DOGS)
                {
                    lblDogsAmount.Text = trap.Amount + "x";
                }
                else if (trap.NameKey == ETrap.MINE)
                {
                    lblMineAmount.Text = trap.Amount + "x";
                }
                else if (trap.NameKey == ETrap.PIT)
                {
                    lblPitAmount.Text = trap.Amount + "x";
                }
            }

            LoadAvailableTraps();
        }

        private async void LoadAvailableTraps()
        {
            var response = await PurchaseApiService.ListAvailableTraps();

            if (response != null)
            {
                availableTraps = response;

                LoadWindowsPrices();
            }
        }

        private async void LoadWindowsPrices()
        {
            try
            {
                List<string> productsIds = new List<string>();

                availableTraps.ForEach(a =>
                {
                    productsIds.Add(a.KeyWindows);
                });

                ListingInformation products = await CurrentApp.LoadListingInformationByProductIdsAsync(productsIds);

                if (products != null)
                {

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}