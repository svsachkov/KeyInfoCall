using Android.Views;

namespace Coursework
{
    public interface IItemClickListener
    {
        void OnClick(View itemView, int position) { }
    }
}