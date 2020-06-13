using System.Linq;
using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Graphics;

using Plugin.Connectivity;
using Android.Net;

namespace Coursework
{
    public class ExpandableTextViewAdapter : BaseExpandableListAdapter
    {
        readonly Context context;

        readonly string[] faqs =
        {
            "События",
            "Места",
            "Даты и время",
            "Другая информация"
        };

        readonly List<string> noInfo = new List<string>() { "Нет информации . . ." };

        readonly List<List<string>> answers;

        public ExpandableTextViewAdapter(Context context, int position)
        {
            this.context = context;

            KeyInfo keyInfo = SqlData.GetKeyInfo(position);

            if (keyInfo != null && keyInfo.TextFromSpeech != "")
            {
                ConnectivityManager cm = (ConnectivityManager)context.GetSystemService(Context.ConnectivityService);

                if (CrossConnectivity.Current.IsConnected && cm.ActiveNetworkInfo != null && cm.ActiveNetworkInfo.IsConnected)
                {
                    //try
                    //{
                    //    EntityRecognition.RecognizeEntities(keyInfo.TextFromSpeech, position);

                    //    Toast.MakeText(context, "Информация обработана", ToastLength.Long).Show();
                    //}
                    //catch (System.Exception)
                    //{
                    //    Toast.MakeText(context, "Ошибка при обработке информации...\nПожалуйста, проверьте свою подписку на Azure", ToastLength.Long).Show();
                    //}
                    //keyInfo = SqlData.GetKeyInfo(position);
                    EntityRecognition.RecognizeEntities(keyInfo.TextFromSpeech, position);
                }
                else
                {
                    Toast.MakeText(context, "Ошибка при обработке информации...\nПожалуйста, проверьте Интернет соединение", ToastLength.Long).Show();
                }
            }

            List<string> Events = keyInfo == null ? new List<string>() { "" } : keyInfo.Events.Split(',').Distinct().ToList();
            List<string> Locations = keyInfo == null ? new List<string>() { "" } : keyInfo.Locations.Split(',').Distinct().ToList();
            List<string> Time = keyInfo == null ? new List<string>() { "" } : keyInfo.Time.Split(',').Distinct().ToList();
            List<string> Otherinfo = keyInfo == null ? new List<string>() { "" } : keyInfo.OtherInfo.Split(',').Distinct().ToList();

            answers = new List<List<string>>
            {
                Events[0] == "" ? noInfo : Events.Where(x => x.ToString() != "").ToList(),
                Locations[0] == "" ? noInfo : Locations.Where(x => x.ToString() != "").ToList(),
                Time[0] == "" ? noInfo : Time.Where(x => x.ToString() != "").ToList(),
                Otherinfo[0] == "" ? noInfo : Otherinfo.Where(x => x.ToString() != "").ToList()
            };
        }

        public override int GroupCount => faqs.Length;

        public override bool HasStableIds => false;

        public override Java.Lang.Object GetChild(int groupPosition, int childPosition) => answers[groupPosition][childPosition];

        public override long GetChildId(int groupPosition, int childPosition) => childPosition;

        public override int GetChildrenCount(int groupPosition) => answers[groupPosition].Count;

        public override View GetChildView(int groupPosition, int childPosition, bool isLastChild, View convertView, ViewGroup parent)
        {

            string answerFaq = GetChild(groupPosition, childPosition).ToString();

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)this.context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.answer, null);
            }

            TextView textView = convertView.FindViewById<TextView>(Resource.Id.information);
            textView.Text = answerFaq;

            return convertView;
        }

        public override Java.Lang.Object GetGroup(int groupPosition) => faqs[groupPosition];

        public override long GetGroupId(int groupPosition) => groupPosition;

        public override View GetGroupView(int groupPosition, bool isExpanded, View convertView, ViewGroup parent)
        {
            string questionFaq = GetGroup(groupPosition).ToString();

            if (convertView == null)
            {
                LayoutInflater inflater = (LayoutInflater)this.context.GetSystemService(Context.LayoutInflaterService);
                convertView = inflater.Inflate(Resource.Layout.title, null);
            }

            TextView textView = convertView.FindViewById<TextView>(Resource.Id.title);
            textView.SetTypeface(null, TypefaceStyle.Bold);
            textView.Text = questionFaq;

            switch (groupPosition)
            {
                case 0:
                    ImageView eventImage = convertView.FindViewById<ImageView>(Resource.Id.icon);
                    eventImage.SetImageResource(Resource.Drawable.events);
                    break;
                case 1:
                    ImageView locationImage = convertView.FindViewById<ImageView>(Resource.Id.icon);
                    locationImage.SetImageResource(Resource.Drawable.location);
                    break;
                case 2:
                    ImageView timeImage = convertView.FindViewById<ImageView>(Resource.Id.icon);
                    timeImage.SetImageResource(Resource.Drawable.time);
                    break;
                case 3:
                    ImageView otherImage = convertView.FindViewById<ImageView>(Resource.Id.icon);
                    otherImage.SetImageResource(Resource.Drawable.other);
                    break;
                default:
                    break;
            }

            return convertView;
        }

        public override bool IsChildSelectable(int groupPosition, int childPosition) => false;
    }
}