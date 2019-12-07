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

                Toast.MakeText(this, "Problem z połączeniem do bazy : ", ToastLength.Long).Show();
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

                foreach (DataColumn col in dsLogList.Tables["product_name"].Columns)
                {
                    logList.Add(col.ColumnName);
                }


                foreach (DataRow row in dsLogList.Tables["product_name"].Rows)
                {
                    for (int i = 0; i < logList.Count; i++)
                        logList[i] += " : " + row[i].ToString();
                }



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