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

using Xamarin.Essentials;

namespace ApkaUG
{

    [Activity(Label = "CalculatorActivity")]
    public class CalculatorActivity : Activity
    {
        EditText weightInput_view;
        EditText ageInput_view;
        EditText heightInput_view;
        RadioButton woman_radioButton;
        RadioButton man_radioButton;
        Button lossButton;
        Button gainButton;

        float weight;
        float height;
        int age;
        int kcal;
        bool woman;

        int carbCount;
        int protCount;
        int fatCount;

        Android.App.AlertDialog.Builder dialog;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.calculator);

            weightInput_view = FindViewById<EditText>(Resource.Id.weightInput);
            ageInput_view = FindViewById<EditText>(Resource.Id.ageInput);
            heightInput_view = FindViewById<EditText>(Resource.Id.heightInput);
            woman_radioButton = FindViewById<RadioButton>(Resource.Id.woman);
            man_radioButton = FindViewById<RadioButton>(Resource.Id.man);
            lossButton = FindViewById<Button>(Resource.Id.loss);
            gainButton = FindViewById<Button>(Resource.Id.gain);

            // Pop-up info
            dialog = new AlertDialog.Builder(this);

            // On Click delegate
            gainButton.Click += delegate
            {
                WeightGainCalculate();
                Preferences.Set("kcal", kcal);
                Preferences.Set("carb", carbCount);
                Preferences.Set("prot", protCount);
                Preferences.Set("fat", fatCount);
            };

            // On Click delegate
            lossButton.Click += delegate
            {
                WeightLossCalculate();
                Preferences.Set("kcal", kcal);
                Preferences.Set("carb", carbCount);
                Preferences.Set("prot", protCount);
                Preferences.Set("fat", fatCount);
            };
        }


        bool UpdateVariables()
        {
            if (!string.IsNullOrEmpty(weightInput_view.Text))
            {
                weight = float.Parse(weightInput_view.Text);
            }
            else
            {
                return false;
            }
            if (!string.IsNullOrEmpty(heightInput_view.Text))
            {
                height = float.Parse(heightInput_view.Text);
            }
            else
            {
                return false;
            }
            if (!string.IsNullOrEmpty(ageInput_view.Text))
            {
                age = int.Parse(ageInput_view.Text);
            }
            else
            {
                return false;
            }
            if (woman_radioButton.Checked)
                woman = true;
            else if (man_radioButton.Checked)
                woman = false;
            else
                return false;
            return true;
        }


        double BaseCalculate()
        {
            if (woman)
                return (9.247 * weight + 3.098 * height - 4.330 * age + 447.593) * 1.3;
            else
                return (13.397 * weight + 3.779 * height - 5.677 * age + 88.362) * 1.3;
        }

        void ShowFinishPopup()
        {
            AlertDialog alert = dialog.Create();
            alert.SetTitle("INFO");
            alert.SetMessage($"Potrzebujesz {kcal} kcal, " +
                $"{carbCount} węgli, " +
                $"{protCount} białka, " +
                $"{fatCount} tłuszczy!"
                );
            alert.SetButton("OK", (c, ev) =>
            {
                // OK button clicked 
            });
            alert.Show();
        }

        void ShowErrorPopup()
        {
            AlertDialog alert = dialog.Create();
            alert.SetTitle("Błąd");
            alert.SetMessage("Złe dane");
            alert.SetButton("OK", (c, ev) =>
            {
                // OK button clicked 
            });
            alert.Show();
        }

        void WeightLossCalculate()
        {
            if (!UpdateVariables())
            {
                ShowErrorPopup();
                return;
            }
            kcal = (int)(BaseCalculate() - 500);
            protCount = (int)(1.3 * weight);
            fatCount = (int)(0.25 * (kcal / 8));
            carbCount = (int)((kcal - fatCount * 8 - protCount * 4) / 4);

            ShowFinishPopup();
        }

        void WeightGainCalculate()
        {
            if (!UpdateVariables())
            {
                ShowErrorPopup();
                return;
            }
            kcal = (int)(BaseCalculate() + 500);
            protCount = (int)(1.5 * weight);
            fatCount = (int)(0.20 * (kcal / 8));
            carbCount = (int)((kcal - fatCount * 8 - protCount * 4) / 4);

            ShowFinishPopup();
        }
    }
}