using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Android.Util;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using MySql.Data.MySqlClient;
using Android.Support.Design.Widget;
using Android.Widget;
using Android.Support.V7.App;
using Android.Support.V4.App;
using static Android.Gms.Vision.Detector;
using Android.Graphics;
using System.Globalization;

namespace ApkaUG
{
    [Activity(Label = "ScannerActivity")]
    public class ScannerActivity :  AppCompatActivity,  IProcessor, ISurfaceHolderCallback
    {

        string sServer;
        string query;
        MySqlConnection conn;
        BarcodeDetector barcodeDetector;
        CameraSource cameraSource;
        SurfaceView cameraPreview;
        ListView ListDataPos;
        TextView txtResult;
        List<string> logList;
        Vibrator vib;
        Button btnApproved;
        public static string kcalInput;
        public static string carbInput;
        public static string fatInput;
        public static string proteinInput;

        const int RequestCameraPermissionID = 1001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.scanner);



            //mysql-------------------------------------
         
            sServer = "Server=projektowanie.vetrex.eu;Port=3306;database=UG;User Id=UG;Password=WinterIsComing0;charset=utf8";
            SetMysqlConn(sServer);
            //------------------------------------------


            cameraPreview = FindViewById<SurfaceView>(Resource.Id.cameraPreview);
            txtResult = FindViewById<TextView>(Resource.Id.txtResult);
            ListDataPos = FindViewById<ListView>(Resource.Id.lvListLog);
            btnApproved = FindViewById<Button>(Resource.Id.btnApproved);


            logList = new List<string>();

            vib = (Vibrator)GetSystemService(Context.VibratorService);



            barcodeDetector = new BarcodeDetector.Builder(this)
               .SetBarcodeFormats(BarcodeFormat.Ean13)
               .Build();

            cameraSource = new CameraSource
               .Builder(this, barcodeDetector)
               .SetRequestedPreviewSize(640, 480)
               .SetAutoFocusEnabled(true)
               .Build();



            cameraPreview.Holder.AddCallback(this);
            barcodeDetector.SetProcessor(this);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);


            // On Click delegate
            btnApproved.Click += delegate
            {
                OnButtonApprovedClicked();
            };

            void OnButtonApprovedClicked()
            {
              
                var intent = new Intent(this, typeof(AddMealActivity));
                intent.PutExtra("kcalInput", kcalInput);
                intent.PutExtra("carbInput", carbInput);
                intent.PutExtra("fatInput", fatInput);
                intent.PutExtra("proteinInput", proteinInput);
                StartActivity(intent);
                Finish();
  
            }


        }


        private void SetMysqlConn(string sServer)
        {
            try
            {
                conn = new MySqlConnection(sServer);
                Toast.MakeText(this, "Connect to database", ToastLength.Long).Show();

            }
            catch (Exception ex)
            {

                Toast.MakeText(this, "Problem z połączeniem do bazy : "+ex.Message.ToString(), ToastLength.Long).Show();
            }


        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            
        }



        private void Scanning()
        {
            try
            {
                cameraPreview.LayoutParameters.Height = 800;
                cameraPreview.RequestLayout();
                cameraSource.Start(cameraPreview.Holder);
                Toast.MakeText(Application.Context, txtResult.Text, ToastLength.Short).Show();


            }
            catch (InvalidOperationException)
            {
                Toast.MakeText(this, "Problem", ToastLength.Long).Show();
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                //request permission
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Manifest.Permission.Camera
                }, RequestCameraPermissionID);

                return;
            }

            try
            {
                cameraSource.Start(cameraPreview.Holder);
                Scanning();
            }
            catch (InvalidOperationException)
            {
                Toast.MakeText(this, "Problem z aparatem", ToastLength.Long).Show();
            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
        }

        public void ReceiveDetections(Detections detections)
        {

            SparseArray qrcodes = detections.DetectedItems;
            if (qrcodes.Size() != 0)
            {
                txtResult.Post(() =>
                {

                    vib.Vibrate(100);
                    Toast.MakeText(Application.Context, ((Barcode)qrcodes.ValueAt(0)).RawValue, ToastLength.Short).Show();
                    logList.Clear();
                    txtResult.Text = ((Barcode)qrcodes.ValueAt(0)).RawValue;

           
                    cameraPreview.LayoutParameters.Height = 0;
                    cameraSource.Stop();
                    GetDataFromDB(txtResult.Text);

                    btnApproved.Visibility = Android.Views.ViewStates.Visible;

                });

            }
        }

        public void Release()
        {
            
        }


        private void GetDataFromDB(string ItemCode)
        {
            try
            {
               
                logList.Clear();
                DataSet dsLogList = new DataSet();
                

                query = "Call GetItemData('" + ItemCode + "')";

           
                MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                adapter.Fill(dsLogList, "product_name");

             
                
                DataTable dtProductValues = dsLogList.Tables["product_name"];

                if (dtProductValues.Rows.Count == 0)
                {
                    Toast.MakeText(this, "brak itemu w bazie  " , ToastLength.Long).Show();
                    return;
                }

                foreach (DataColumn col in dsLogList.Tables["product_name"].Columns)
                {
                    logList.Add(col.ColumnName);
                }


                foreach (DataRow row in dsLogList.Tables["product_name"].Rows)
                {
                    for (int i = 0; i < logList.Count; i++)
                        logList[i] += " : " + row[i].ToString();
                }

                double Kcal;
                // KJ to KCAL
                Kcal = float.Parse( dtProductValues.Rows[0]["energia"].ToString(), CultureInfo.InvariantCulture.NumberFormat) * 0.2388;

                kcalInput = Kcal.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                carbInput = dtProductValues.Rows[0]["weglowodany"].ToString();
                fatInput = dtProductValues.Rows[0]["tluszcz"].ToString();
                proteinInput = dtProductValues.Rows[0]["bialko"].ToString();

                ArrayAdapter<string> adapterArray = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSelectableListItem, logList);
                ListDataPos.Adapter = adapterArray;

            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error : " + ex.ToString(), ToastLength.Long).Show();
            }


        }


    }
}