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
    [Activity(Label = "HistoryActivity")]
    public class HistoryActivity : ListActivity
    {
        // DB
        string dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "jedzenie.db3");
        SQLiteConnection dbConnection;

        List<DBMeal> mealList;
        List<string> stringList;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            dbConnection = new SQLiteConnection(dbPath);
            mealList = dbConnection.Table<DBMeal>().ToList();

            stringList  = mealList.Select(o => $"Zjadłeś {o.m_grams/100f * o.m_kcal} kcal, " +
                $"{o.m_grams / 100f * o.m_carb} węgli, " +
                $"{o.m_grams / 100f * o.m_protein} białka, " +
                $"{o.m_grams / 100f * o.m_fat} tłuszczy!" + "\r\n" + 
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