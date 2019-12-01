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

namespace ApkaUG
{
    [Activity(Label = "WorkoutActivity")]
    public class WorkoutActivity : Activity
    {
        // Layout variables
        EditText kcal_burned_view;
        EditText minutes_view;
        Button apply_button;

        float kcal_burned;

        Android.App.AlertDialog.Builder dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addExcercise);

            // View Getters
            kcal_burned_view = FindViewById<EditText>(Resource.Id.kcal_burned_ET);
            minutes_view = FindViewById<EditText>(Resource.Id.minutes_ET);
            apply_button = FindViewById<Button>(Resource.Id.apply_BT);


            // Pop-up info
            dialog = new AlertDialog.Builder(this);
            AlertDialog alert = dialog.Create();
            alert.SetTitle("INFO");
            alert.SetMessage("Dodano ćwiczenie pomyślnie!");
            alert.SetButton("OK", (c, ev) =>
            {
                // OK button clicked 
            });

            // On Click delegate
            apply_button.Click += delegate
            {
                OnButtonClicked(alert);
            };
        }
        void OnButtonClicked(AlertDialog a_alert)
        {
            a_alert.Show();
        }

    }
}