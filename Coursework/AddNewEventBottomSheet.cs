using System;
using System.Collections.Generic;
//using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Telephony;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;

using Xamarin.Essentials;

namespace Coursework
{
    class AddNewEventBottomSheet : BottomSheetDialogFragment, DatePickerDialog.IOnDateSetListener, TimePickerDialog.IOnTimeSetListener
    {
        readonly CallsInfoActivity activity;
        readonly Context context;

        EditText title;
        EditText location;
        TextView start;
        TextView end;
        EditText description;
        Button button;

        readonly Calendar startTime = Calendar.Instance;
        readonly Calendar endTime = Calendar.Instance;

        bool flag = true;

        public AddNewEventBottomSheet(CallsInfoActivity activity, Context context)
        {
            this.activity = activity;
            this.context = context;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.bottom_sheet_layout, container, false);

            View dialogView = View.Inflate(Activity, Resource.Layout.bottom_sheet_layout, null);
            AlertDialog alertDialog = new AlertDialog.Builder(Activity).Create();

            title = v.FindViewById<EditText>(Resource.Id.title);
            location = v.FindViewById<EditText>(Resource.Id.location);
            start = v.FindViewById<TextView>(Resource.Id.start);
            end = v.FindViewById<TextView>(Resource.Id.end);
            description = v.FindViewById<EditText>(Resource.Id.description);
            start.Text = $"Начало:\t{DateTime.Now}";
            end.Text = $"Конец:\t\t{DateTime.Now.AddHours(1)}";

            if (Preferences.ContainsKey("Title"))
                title.Text = Preferences.Get("Title", "");
            if (Preferences.ContainsKey("Location"))
                location.Text = Preferences.Get("Location", "");
            if (Preferences.ContainsKey("Description"))
                description.Text = Preferences.Get("Description", "");

            title.TextChanged += TitleTextChange;
            location.TextChanged += LocationTextChange;
            description.TextChanged += DescriptionTextChange;

            start.Click += ChooseStartTime;
            end.Click += ChooseEndTime;

            button = v.FindViewById<Button>(Resource.Id.save);
            button.Click += AddEvent;

            if (title.Text != "")
                button.Enabled = true;

            return v;
        }

        private void ChooseEndTime(object sender, EventArgs e)
        {
            flag = false;
            DatePickerDialog datePickerDialog = new DatePickerDialog(context);
            datePickerDialog.SetOnDateSetListener(this);
            datePickerDialog.Show();
        }

        private void DescriptionTextChange(object sender, TextChangedEventArgs e) =>
            Preferences.Set("Description", description.Text);

        private void LocationTextChange(object sender, TextChangedEventArgs e) =>
            Preferences.Set("Location", location.Text);

        public void AddEvent(object sender, EventArgs e)
        {
            ContentResolver cr = activity.ContentResolver;
            ContentValues cv = new ContentValues();
            cv.Put(CalendarContract.EventsColumns.Title, title.Text);
            cv.Put(CalendarContract.EventsColumns.EventLocation, location.Text);
            cv.Put(CalendarContract.EventsColumns.Dtstart, startTime.TimeInMillis);
            cv.Put(CalendarContract.EventsColumns.Dtend, endTime.TimeInMillis);
            cv.Put(CalendarContract.EventsColumns.Description, description.Text);
            cv.Put(CalendarContract.EventsColumns.CalendarId, 1);
            cv.Put(CalendarContract.EventsColumns.EventTimezone, Calendar.Instance.TimeZone.ID);
            cr.Insert(CalendarContract.Events.ContentUri, cv);

            Dismiss();
            Preferences.Clear();
            Toast.MakeText(context, "Событие успешно добавлено в Календарь", ToastLength.Short).Show();
        }

        public void TitleTextChange<TEventArgs>(object sender, TEventArgs e)
        {
            Preferences.Set("Title", title.Text);
            if (title.Text != "")
                button.Enabled = true;
            else
                button.Enabled = false;
        }

        public void ChooseStartTime(object sender, EventArgs e)
        {
            DatePickerDialog datePickerDialog = new DatePickerDialog(context);
            datePickerDialog.SetOnDateSetListener(this);
            datePickerDialog.Show();
        }

        [Obsolete]
        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            if (flag)
            {
                if (month >= 9 && dayOfMonth >= 10)
                {
                    start.Text = $"Начало:\t{dayOfMonth}.{month + 1}.{year} ";
                    end.Text = $"Конец:\t\t{dayOfMonth}.{month + 1}.{year} ";
                }
                else if (month >= 9 && dayOfMonth < 10)
                {
                    start.Text = $"Начало:\t0{dayOfMonth}.{month + 1}.{year} ";
                    end.Text = $"Конец:\t\t0{dayOfMonth}.{month + 1}.{year} ";
                }
                else if (month < 9 && dayOfMonth >= 10)
                {
                    start.Text = $"Начало:\t{dayOfMonth}.0{month + 1}.{year} ";
                    end.Text = $"Конец:\t\t{dayOfMonth}.0{month + 1}.{year} ";
                }
                else
                {
                    start.Text = $"Начало:\t0{dayOfMonth}.0{month + 1}.{year} ";
                    end.Text = $"Конец:\t\t0{dayOfMonth}.0{month + 1}.{year} ";
                }

                startTime.Set(Calendar.Year, year);
                startTime.Set(Calendar.Month, month);
                startTime.Set(Calendar.DayOfMonth, dayOfMonth);

                endTime.Set(Calendar.Year, year);
                endTime.Set(Calendar.Month, month);
                endTime.Set(Calendar.DayOfMonth, dayOfMonth);
            }
            else
            {
                if (month >= 9 && dayOfMonth >= 10)
                    end.Text = $"Конец:\t\t{dayOfMonth}.{month + 1}.{year} ";
                else if (month < 9 && dayOfMonth >= 10)
                    end.Text = $"Конец:\t\t{dayOfMonth}.0{month + 1}.{year} ";
                else if (month >= 9 && dayOfMonth < 10)
                    end.Text = $"Конец:\t\t0{dayOfMonth}.{month + 1}.{year} ";
                else
                    end.Text = $"Конец:\t\t0{dayOfMonth}.0{month + 1}.{year} ";

                endTime.Set(Calendar.Year, year);
                endTime.Set(Calendar.Month, month);
                endTime.Set(Calendar.DayOfMonth, dayOfMonth);
            }

            TimePickerDialog timePickerDialog = new TimePickerDialog(context, this, DateTime.Now.Hour, DateTime.Now.Minute, true);
            timePickerDialog.Show();
        }

        [Obsolete]
        public void OnTimeSet(TimePicker view, int hourOfDay, int minute)
        {
            if (flag)
            {
                if (hourOfDay >= 10 && minute >= 10)
                {
                    start.Text += $"{hourOfDay}:{minute}";
                    end.Text += $"{hourOfDay + 1}:{minute}";
                }
                else if (hourOfDay < 10 && minute >= 10)
                {
                    start.Text += $"0{hourOfDay}:{minute}";
                    end.Text += $"0{hourOfDay + 1}:{minute}";
                }
                else if (hourOfDay >= 10 && minute < 10)
                {
                    start.Text += $"{hourOfDay}:0{minute}";
                    end.Text += $"{hourOfDay + 1}:0{minute}";
                }
                else
                {
                    start.Text += $"0{hourOfDay}:0{minute}";
                    end.Text += $"0{hourOfDay + 1}:0{minute}";
                }

                startTime.Set(Calendar.HourOfDay, hourOfDay);
                startTime.Set(Calendar.Minute, minute);

                endTime.Set(Calendar.HourOfDay, hourOfDay + 1);
                endTime.Set(Calendar.Minute, minute);
            }
            else
            {
                flag = true;
                if (hourOfDay >= 10 && minute >= 10)
                    end.Text += $"{hourOfDay}:{minute}";
                else if (hourOfDay < 10 && minute >= 10)
                    end.Text += $"0{hourOfDay}:{minute}";
                else if (hourOfDay >= 10 && minute < 10)
                    end.Text += $"{hourOfDay}:0{minute}";
                else
                    end.Text += $"0{hourOfDay}:0{minute}";

                endTime.Set(Calendar.HourOfDay, hourOfDay);
                endTime.Set(Calendar.Minute, minute);
            }
        }
    }
}