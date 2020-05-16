using Android.OS;
using Android.App;
using Android.Widget;
using Android.Content;
using Android.Content.PM;
using Android.Support.V7.App;
using System.Reflection.Emit;
using Android.Support.Design.Widget;
using System;
using Android.Views;
using Xamarin.Essentials;

namespace Coursework
{
    [Activity(Label = "Ключевая Информация", ScreenOrientation = ScreenOrientation.Portrait, ParentActivity = typeof(MainActivity))]
    public class CallsInfoActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            // Base.
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.call_info);

            string contactName = Intent.Extras.Get("Contact").ToString();
            this.Title = contactName;

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            int position = int.Parse(Intent.Extras.Get("Number").ToString());

            ExpandableListView expandableTextView = FindViewById<ExpandableListView>(Resource.Id.expandbleListView);
            ExpandableTextViewAdapter adapter = new ExpandableTextViewAdapter(this, position);
            expandableTextView.SetAdapter(adapter);
        }

        private async void FabOnClick(object sender, EventArgs e)
        {
            await Permissions.RequestAsync<Permissions.CalendarWrite>();
            var status = await Permissions.CheckStatusAsync<Permissions.CalendarWrite>();
            if (status == PermissionStatus.Granted)
            {
                AddNewEventBottomSheet bottomSheet = new AddNewEventBottomSheet(this, this);
                bottomSheet.SetStyle(Android.Support.V4.App.DialogFragment.StyleNormal, Resource.Style.AppBottomSheetDialogTheme);
                bottomSheet.Show(SupportFragmentManager, "addEventBottomSheet");
            }
            else if (status == PermissionStatus.Denied)
            {
                Toast.MakeText(this, "Сначала предоставьте разрешение на \"запись в календарь\"", ToastLength.Long);
                await Permissions.RequestAsync<Permissions.CalendarWrite>();
            }
        }
    }
}