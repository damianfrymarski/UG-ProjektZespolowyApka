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
    class DBWorkout
    {
        public string m_workoutName { get; set; }
        public float m_kcalBurned { get; set; }
        public float m_time { get; set; }
        public string m_date { get; set; }

        public DBWorkout() { }
        public DBWorkout(string workoutName, float kcalBurned, float time, string date)
        {
            m_workoutName = workoutName;
            m_kcalBurned = kcalBurned;
            m_time = time;
            m_date = date;
        }
    }
}