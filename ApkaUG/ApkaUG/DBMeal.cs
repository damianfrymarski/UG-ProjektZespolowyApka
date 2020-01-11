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
    class DBMeal
    {
        public float m_grams { get; set; }
        public float m_kcal { get; set; }
        public float m_carb { get; set; }
        public float m_fat { get; set; }
        public float m_protein { get; set; }
        public string m_date { get; set; }

        public DBMeal() {}

        public DBMeal(float grams, float kcal, float carb, float fat, float protein, string date)
        {
            m_grams = grams;
            m_kcal = kcal;
            m_carb = carb;
            m_fat = fat;
            m_protein = protein;
            m_date = date;
        }

    }
}