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
using System.IO;
using SQLite;
using System.Globalization;

namespace ApkaUG
{
    [Activity(Label = "AddMealActivity")]
    public class AddMealActivity : Activity
    {
        // DB
        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "jedzenie.db3");
        SQLiteConnection dbConnection;
        DBMeal meal;

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

            kcalInput_view.Text = Intent.GetStringExtra("kcalInput");
            carbInput_view.Text = Intent.GetStringExtra("carbInput");
            fatInput_view.Text = Intent.GetStringExtra("fatInput");
            proteinInput_view.Text = Intent.GetStringExtra("proteinInput");

        }

        void OnButtonScanClicked()
        {
            StartActivity(typeof(ScannerActivity));
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
         
        }
        void OnButtonClicked()
        {
            // Values
            if (!string.IsNullOrWhiteSpace(gramsInput_view.Text))
                gramsInput = float.Parse(gramsInput_view.Text, CultureInfo.InvariantCulture.NumberFormat);
            else
                gramsInput = 0;
            if (!string.IsNullOrWhiteSpace(kcalInput_view.Text))           
                kcalInput = float.Parse(kcalInput_view.Text, CultureInfo.InvariantCulture.NumberFormat);
            else
                kcalInput = 0;
            if (!string.IsNullOrWhiteSpace(carbInput_view.Text))             
                carbInput = float.Parse(carbInput_view.Text, CultureInfo.InvariantCulture.NumberFormat);
            else
                carbInput = 0;
            if (!string.IsNullOrWhiteSpace(fatInput_view.Text))
                fatInput = float.Parse(fatInput_view.Text, CultureInfo.InvariantCulture.NumberFormat);
            else
                fatInput = 0;
            if (!string.IsNullOrWhiteSpace(proteinInput_view.Text))
                proteinInput = float.Parse(proteinInput_view.Text, CultureInfo.InvariantCulture.NumberFormat);
            else
                proteinInput = 0;

            multiplier = gramsInput / 100;

            dbConnection = new SQLiteConnection(dbPath);
            dbConnection.CreateTable<DBMeal>();

            meal = new DBMeal(gramsInput, kcalInput, carbInput, fatInput, proteinInput, DateTime.Now.ToString());
            dbConnection.Insert(meal);

            // For getting table:
            // var table = dbConnection.Table<DBMeal>();

            AlertDialog alert = dialog.Create();
            alert.SetTitle("INFO");
            alert.SetMessage($"Zjadłeś {multiplier * kcalInput} kcal, " +
                $"{multiplier * carbInput} węgli, " +
                $"{multiplier * proteinInput} białka, " +
                $"{multiplier * fatInput} tłuszczy!"
                );
            alert.SetButton("OK", (c, ev) =>
            {
                // OK button clicked 
            });

            alert.Show();
        }
    }
}