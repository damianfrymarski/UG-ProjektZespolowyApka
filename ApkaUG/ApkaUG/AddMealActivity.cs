using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;

namespace ApkaUG
{
    [Activity(Label = "AddMealActivity")]
    public class AddMealActivity : Activity
    {
        EditText gramsInput_view;
        EditText kcalInput_view;
        EditText carbInput_view;
        EditText fatInput_view;
        EditText proteinInput_view;
        Button enter_button;
        Button scan_button;

        float gramsInput;
        float multiplier;
        float kcalInput;
        float carbInput;
        float fatInput;
        float proteinInput;

        Android.App.AlertDialog.Builder dialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addFood);

            // Views
            gramsInput_view = FindViewById<EditText>(Resource.Id.gramsInput);
            kcalInput_view = FindViewById<EditText>(Resource.Id.kcalInput);
            carbInput_view = FindViewById<EditText>(Resource.Id.carbInput);
            fatInput_view = FindViewById<EditText>(Resource.Id.fatInput);
            proteinInput_view = FindViewById<EditText>(Resource.Id.proteinInput);
            enter_button = FindViewById<Button>(Resource.Id.enter);
            scan_button = FindViewById<Button>(Resource.Id.scan);

            // Pop-up info
            dialog = new AlertDialog.Builder(this);          

            // On Click delegate
            enter_button.Click += delegate
            {
                OnButtonClicked();
            };

            // On Click delegate
            scan_button.Click += delegate
            {
                OnButtonScanClicked();
            };

        }

        void OnButtonScanClicked()
        {
            StartActivity(typeof(ScannerActivity));
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            //drawer.CloseDrawer(GravityCompat.Start);
        }
        void OnButtonClicked()
        {
            // Values
            gramsInput = (float)Convert.ToDouble(gramsInput_view.Text);
            kcalInput = (float)Convert.ToDouble(kcalInput_view.Text);
            carbInput = (float)Convert.ToDouble(carbInput_view.Text);
            fatInput = (float)Convert.ToDouble(fatInput_view.Text); ;
            proteinInput = (float)Convert.ToDouble(proteinInput_view.Text);
            multiplier = gramsInput / 100;

            AlertDialog alert = dialog.Create();
            alert.SetTitle("INFO");
            alert.SetMessage($"Zjadłeś {multiplier * kcalInput} kcal, " +
                $"{multiplier * carbInput} węgli, " +
                $"{multiplier * proteinInput} białka, " +
                $"{multiplier * proteinInput} tłuszczy!"
                );
            alert.SetButton("OK", (c, ev) =>
            {
                // OK button clicked 
            });

            alert.Show();
        }
    }
}