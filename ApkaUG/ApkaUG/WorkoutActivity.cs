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
using System.IO;
using SQLite;

namespace ApkaUG
{
    [Activity(Label = "WorkoutActivity")]
    public class WorkoutActivity : Activity
    {
        // DB
        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "jedzenie.db3");
        SQLiteConnection dbConnection;
        DBWorkout workout;

        // Layout variables
        EditText kcal_burned_view;
        EditText minutes_view;
        Button apply_button;
        Spinner excercise_spinner;

        List<string> excercises = new List<string>() { "Bieganie (19 kcal/min)", "Skakanie (25 kcal/min)", "Wyciskanie (32 kcal/min)", "Chodzenie (6 kcal/min)" };
        ArrayAdapter excercise_adapter;

        float kcal_burned;
        float kcal_burned_ex;
        float excercise_time;
        string workout_name = "Bez Nazwy";

        Android.App.AlertDialog.Builder dialog;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.addExcercise);

            // View Getters
            kcal_burned_view = FindViewById<EditText>(Resource.Id.kcal_burned_ET);
            minutes_view = FindViewById<EditText>(Resource.Id.minutes_ET);
            apply_button = FindViewById<Button>(Resource.Id.apply_BT);
            excercise_spinner = FindViewById<Spinner>(Resource.Id.excercise_spinner);

            excercise_spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            excercise_adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, excercises.ToArray());
            excercise_adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            excercise_spinner.Adapter = excercise_adapter;

            // On Click delegate
            apply_button.Click += delegate
            {
                // Case gdzie poda od razu kcal spalone
                if (!string.IsNullOrWhiteSpace(kcal_burned_view.Text))
                    kcal_burned = (float)Convert.ToDouble(kcal_burned_view.Text);
                else
                    kcal_burned = 0;

                if (!string.IsNullOrWhiteSpace(minutes_view.Text))
                    excercise_time = (float)Convert.ToDouble(minutes_view.Text);

                dbConnection = new SQLiteConnection(dbPath);
                dbConnection.CreateTable<DBWorkout>();

                workout = new DBWorkout(workout_name, kcal_burned, excercise_time, DateTime.Now.ToString());
                dbConnection.Insert(workout);

                // Pop-up info
                dialog = new AlertDialog.Builder(this);
                AlertDialog alert = dialog.Create();
                alert.SetTitle("INFO");
                alert.SetMessage($"Spaliłeś/aś {kcal_burned + kcal_burned_ex * excercise_time} kcal! " +
                    "Brawo!!!"
                    );
                alert.SetButton("OK", (c, ev) =>
                {
                    // OK button clicked 
                });

                OnButtonClicked(alert);
            };
        }
        void OnButtonClicked(AlertDialog a_alert)
        {
            a_alert.Show();
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            string toast = string.Format("Excercise: {0}", spinner.GetItemAtPosition(e.Position));
            dbConnection = new SQLiteConnection(dbPath);
            dbConnection.CreateTable<DBWorkout>();

            switch (e.Position)
            {
                case 0:
                    kcal_burned_ex = 19;
                    workout_name = "Bieganie";
                    break;
                case 1:
                    kcal_burned_ex = 25;
                    workout_name = "Skakanie";
                    break;
                case 2:
                    kcal_burned_ex = 32;
                    workout_name = "Wyciskanie";
                    break;
                case 3:
                    kcal_burned_ex = 6;
                    workout_name = "Chodzenie";
                    break;
            }
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }

    }
}