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
    [Activity(Label = "WorkoutHistory")]
    public class WorkoutHistory : ListActivity
    {

        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "jedzenie.db3");
        SQLiteConnection dbConnection;

        List<DBWorkout> workoutList;
        List<string> stringList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            dbConnection = new SQLiteConnection(dbPath);
            workoutList = dbConnection.Table<DBWorkout>().ToList();

            stringList = workoutList.Select(o => $"Spaliłeś/aś {o.m_kcalBurned} kcal! " + "\r\n" +
                $"Nazwa ćwiczenia: {o.m_workoutName}" + "\r\n" +
                $"Czas: {o.m_time} minut!" + "\r\n" +
                $"Date: {o.m_date.ToString()}"
                 ).ToList();

            ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, stringList);

            ListView.TextFilterEnabled = true;

            ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                Toast.MakeText(Application, ((TextView)args.View).Text, ToastLength.Short).Show();
            };
        }
    }
}