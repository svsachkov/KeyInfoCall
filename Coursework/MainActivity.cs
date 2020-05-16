using System;
using System.Collections.Generic;

using Android.OS;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Runtime;
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Support.V7.Widget;

using Xamarin.Essentials;

using static Android.Provider.Settings;

namespace Coursework
{
    [Activity(Label = "@string/main_activity_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        private Dialog dialog = null;
        private static RecyclerView recycler;
        private List<RecentCall> RecentCalls;
        private static RecyclerViewAdapter adapter;
        private static TextView noRecentCallsTextView;
        private Android.Support.V7.Widget.Toolbar toolbar;
        private static ProgressDialog progressDialog = null;
        private static List<RecentCall> newRecentCalls = null;
        private static RecyclerView.LayoutManager layoutManager;

        public static Context AndroidContext { get; private set; }


        protected override void OnCreate(Bundle savedInstanceState)
        {
            // Base.
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Title = "Журнал звонков";

            // Toolbar.
            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            // Ask for permissions.
            RequestPermissions();

            AndroidContext = this;

            if (progressDialog == null)
                progressDialog = new ProgressDialog(this);
            else
            {
                progressDialog.Dismiss();
                progressDialog = new ProgressDialog(this);
            }

            // TextView, which is visible when there are no recent calls in the list.
            noRecentCallsTextView = FindViewById<TextView>(Resource.Id.noRecentCallsTextView);

            // RecyclerView.
            recycler = FindViewById<RecyclerView>(Resource.Id.recycler);

            layoutManager = new LinearLayoutManager(this);

            // Get data with recent calls.
            RecentCalls = SqlData.GetRecentCalls();

            // If there is saved call data.
            if (RecentCalls != null)
            {
                noRecentCallsTextView.Visibility = ViewStates.Gone;
                adapter = new RecyclerViewAdapter(this, RecentCalls, progressDialog);
                AddDataToRecyclerView(adapter);
            }

        }

        /// <summary>
        /// Method for requesting permissions.
        /// </summary>
        public async void RequestPermissions()
        {
            await Permissions.RequestAsync<Permissions.Phone>();
            await Permissions.RequestAsync<Permissions.Microphone>();
            await Permissions.RequestAsync<Permissions.StorageRead>();
            await Permissions.RequestAsync<Permissions.StorageWrite>();
            
            if (Secure.GetString(ContentResolver, Secure.EnabledAccessibilityServices) == null || 
                !Secure.GetString(ContentResolver, Secure.EnabledAccessibilityServices).Contains("KeyInfoCallAccessibilityService"))
            {
                dialog = new Dialog(this);
                dialog.SetContentView(Resource.Layout.permission_dialog);
                Button settingsButton = dialog.FindViewById<Button>(Resource.Id.go_to_settings);
                Button permissionGrantedButton = dialog.FindViewById<Button>(Resource.Id.permission_is_granted);
               
                settingsButton.Click += (object sender, EventArgs e) =>
                {
                    StartActivity(new Intent(ActionAccessibilitySettings));
                };
                
                permissionGrantedButton.Click += (object sender, EventArgs e) =>
                {
                    if ((Secure.GetString(this.ContentResolver, Secure.EnabledAccessibilityServices) != null && 
                    Secure.GetString(ContentResolver, Secure.EnabledAccessibilityServices).Contains("KeyInfoCallAccessibilityService")))
                        dialog.Dismiss();
                };

                dialog.SetCancelable(false);
                dialog.Show();
            }
        }

        public static void AddDataToRecyclerView(RecyclerViewAdapter mAdapter)
        {
            recycler.HasFixedSize = true;
            recycler.SetLayoutManager(layoutManager);
            recycler.SetAdapter(mAdapter);
        }

        public static void UpdateData()
        {
            newRecentCalls = SqlData.GetRecentCalls();

            noRecentCallsTextView.Visibility = ViewStates.Gone;
            if (newRecentCalls == null || newRecentCalls.Count <= 0)
                noRecentCallsTextView.Visibility = ViewStates.Visible;

            AddDataToRecyclerView(new RecyclerViewAdapter(AndroidContext, newRecentCalls, progressDialog));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}