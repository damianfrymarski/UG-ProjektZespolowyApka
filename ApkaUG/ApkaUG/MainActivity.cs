using System;
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

using System.IO;
using SQLite;
using Xamarin.Essentials;
using System.Collections.Generic;

namespace ApkaUG
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "jedzenie.db3");
        SQLiteConnection dbConnection;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            UpdateLabels();
        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateLabels();
        }

        void UpdateLabels()
        {
            FindViewById<TextView>(Resource.Id.KcalText).Text = "Twoje Dzienne Kcal:" + Preferences.Get("kcal", 0).ToString();
            FindViewById<TextView>(Resource.Id.WeglowodanyText).Text = "Twoje Dzienne Węglowodany:" + Preferences.Get("carb", 0).ToString();
            FindViewById<TextView>(Resource.Id.BialkoText).Text = "Twoje Dzienne Białka:" + Preferences.Get("prot", 0).ToString();
            FindViewById<TextView>(Resource.Id.TluszczText).Text = "Twoje Dzienne Tłuszcze:" + Preferences.Get("fat", 0).ToString();

            float workoutSum = 0;
            float workoutCarb = 0;
            float workoutProt = 0;
            float workoutFat = 0;

            float foodSum = 0;
            float foodCarb = 0;
            float foodProt = 0;
            float foodFat = 0;

            dbConnection = new SQLiteConnection(dbPath);
            try
            {
                List<DBWorkout> workoutList = dbConnection.Table<DBWorkout>().ToList();
                foreach (var a in workoutList)
                {
                    workoutSum += a.m_kcalBurned;
                }
            }
            catch (SQLiteException e)
            {

            }
            if (Preferences.Get("kcal", 0) > 0)
            {
                float carbPercent = (float)Preferences.Get("carb", 0) / Preferences.Get("kcal", 0);
                float protPercent = (float)Preferences.Get("prot", 0) / Preferences.Get("kcal", 0);
                float fatPercent = (float)Preferences.Get("fat", 0) / Preferences.Get("kcal", 0);

                workoutCarb = workoutSum * carbPercent;
                workoutProt = workoutSum * protPercent;
                workoutFat = workoutSum * fatPercent;
            }
            else
            {
                workoutCarb = workoutSum * 0.6f;
                workoutProt = workoutSum * 0.2f;
                workoutFat = workoutSum * 0.2f;
            }



            try
            {
                List<DBMeal> mealList = dbConnection.Table<DBMeal>().ToList();
                foreach (var a in mealList)
                {
                    foodSum += a.m_kcal * a.m_grams / 100;
                    foodCarb += a.m_carb * a.m_grams / 100;
                    foodProt += a.m_protein * a.m_grams / 100;
                    foodFat += a.m_fat * a.m_grams / 100;
                }
            }
            catch (SQLiteException e)
            {

            }

            int todayKcal = (int)(Preferences.Get("kcal", 0) + workoutSum - foodSum);
            int todayCarb = (int)(Preferences.Get("carb", 0) + workoutCarb - foodCarb);
            int todayProt = (int)(Preferences.Get("prot", 0) + workoutProt - foodProt);
            int todayFat = (int)(Preferences.Get("fat", 0) + workoutFat - foodFat);

            FindViewById<TextView>(Resource.Id.KcalLeftText).Text = "Pozostałe Kcal Dzisiaj:" + todayKcal.ToString();
            FindViewById<TextView>(Resource.Id.WeglowodanyLeftText).Text = "Pozostałe Węglowodany Dzisiaj:" + todayCarb.ToString();
            FindViewById<TextView>(Resource.Id.BialkoLeftText).Text = "Pozostałe Białka Dzisiaj:" + todayProt.ToString();
            FindViewById<TextView>(Resource.Id.TluszczLeftText).Text = "Pozostałe Tłuszcze Dzisiaj:" + todayFat.ToString();
        }
        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_add)
            {
                StartActivity(typeof(AddMealActivity));
            }
            else if (id == Resource.Id.nav_history)
            {
                StartActivity(typeof(HistoryActivity));
            }
            else if (id == Resource.Id.nav_excercise)
            {
                StartActivity(typeof(WorkoutActivity));
            }
            else if (id == Resource.Id.nav_calc)
            {
                StartActivity(typeof(CalculatorActivity));
            }
            else if (id == Resource.Id.nav_workout_history)
            {
                StartActivity(typeof(WorkoutHistory));
            }
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}

