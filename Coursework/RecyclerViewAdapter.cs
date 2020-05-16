using System;
using System.Collections.Generic;

using Android.App;
using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Support.V7.Widget;
using Xamarin.Essentials;

namespace Coursework
{
    public class RecyclerViewAdapter : RecyclerView.Adapter, IItemClickListener
    {
        private RecyclerViewHolder viewHolder;
        private LayoutInflater inflater;
        private Intent intent;
        private View itemView;
        readonly private ProgressDialog progressDialog;
        readonly private List<RecentCall> recentCalls;
        readonly private Context mContext;

        public RecyclerViewAdapter(Context mContext, List<RecentCall> recentCalls, ProgressDialog progressDialog)
        {
            this.mContext = mContext;
            this.recentCalls = recentCalls;
            this.progressDialog = progressDialog;
        }

        public override int ItemCount => recentCalls == null ? 0 : recentCalls.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            viewHolder = holder as RecyclerViewHolder;
            viewHolder.PhoneNumber.Text = recentCalls[position].PhoneNumber;
            viewHolder.DateAndTime.Text = recentCalls[position].DateAndTime.ToString();
            viewHolder.SetItemClickListener(this);
        }

        public void OnClick(View itemView, int position)
        {
            intent = new Intent(mContext, typeof(CallsInfoActivity));
            intent.PutExtra(recentCalls[position].PhoneNumber, false);
            intent.PutExtra("Number", position);
            intent.PutExtra("Contact", recentCalls[position].PhoneNumber);
            progressDialog.Show();
            progressDialog.SetContentView(Resource.Layout.custom_dialog);
            progressDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            progressDialog.SetCancelable(true);
            mContext.StartActivity(intent);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            inflater = LayoutInflater.From(parent.Context);
            itemView = inflater.Inflate(Resource.Layout.recycler, parent, false);
            return new RecyclerViewHolder(itemView, recentCalls);
        }

        internal class RecyclerViewHolder : RecyclerView.ViewHolder, View.IOnClickListener
        {
            private IItemClickListener itemClickListener;

            readonly private ImageView deleteImage;

            public ImageView ImageView { get; set; }

            public TextView PhoneNumber { get; set; }

            public TextView DateAndTime { get; set; }

            public RecyclerViewHolder(View itemView, List<RecentCall> recentCalls) : base(itemView)
            {
                ImageView = itemView.FindViewById<ImageView>(Resource.Id.imageView) as ImageView;
                PhoneNumber = itemView.FindViewById<TextView>(Resource.Id.phoneNumber) as TextView;
                DateAndTime = itemView.FindViewById<TextView>(Resource.Id.dateAndTime) as TextView;
                deleteImage = itemView.FindViewById<ImageView>(Resource.Id.delete);
                itemView.SetOnClickListener(this);
                ImageView.Click += (object sender, EventArgs e) =>
                {
                    PhoneDialer.Open(recentCalls[AdapterPosition].Number);
                };
                deleteImage.Click += (object sender, EventArgs e) =>
                {
                    SqlData.RemoveRecentCall(AdapterPosition);
                    MainActivity.UpdateData();
                };
            }

            public void SetItemClickListener(IItemClickListener itemClickListener) => this.itemClickListener = itemClickListener;

            public void OnClick(View v) => itemClickListener.OnClick(v, AdapterPosition);
        }
    }
}