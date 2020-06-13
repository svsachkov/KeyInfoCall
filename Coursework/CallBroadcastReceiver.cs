using System;

using Android.OS;
using Android.App;
using Android.Widget;
using Android.Content;
using Android.Provider;
using Android.Telephony;
using static Android.Provider.Settings;

using Xamarin.Essentials;
using Android.Support.V4.Content;
using Android;
using Android.Content.PM;
using System.Threading;

namespace Coursework
{
    [BroadcastReceiver(Name = "com.test.OutgoingCallBroadcastReceiver", Enabled =true, Exported =true)]
    [IntentFilter(new[] { TelephonyManager.ActionPhoneStateChanged })]
    public class CallBroadcastReceiver : BroadcastReceiver
    {
        /// <summary>
        /// Detecting incomig and outgoing calls.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="intent"></param>
        public override void OnReceive(Context context, Intent intent)
        {
            try
            {
                Intent mIntent = null;
                try
                {
                    mIntent = new Intent(context, typeof(SpeechRecognitionService));
                }
                catch (Exception) { }
                string state = null;

                try
                {
                    state = intent.GetStringExtra(TelephonyManager.ExtraState);
                }
                catch (Exception) { }

                // Phone off hook.
                if (state == TelephonyManager.ExtraStateOffhook)
                {
                    Toast.MakeText(context, "\"KeyInfoCall\" начинает работу", ToastLength.Long).Show();
                    try
                    {
                        if (int.Parse(Build.VERSION.Release) >= 10 && (Secure.GetString(context.ContentResolver, Secure.EnabledAccessibilityServices) == null ||
                                   !Secure.GetString(context.ContentResolver, Secure.EnabledAccessibilityServices).Contains("KeyInfoCallAccessibilityService")))
                        {
                            Toast.MakeText(context, "Разрешение \"Специальные возможности\" не предоставлено", ToastLength.Long);
                            return;
                        }
                    }
                    catch (Exception) { }
                    if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.RecordAudio) == (int)Permission.Granted)
                        context.StartService(mIntent);
                    else
                        Toast.MakeText(context, "У приложения \"KeyInfoCall\" нет доступа к микрофону", ToastLength.Long).Show();
                }
                // Call is ended.
                else if (state == TelephonyManager.ExtraStateIdle)
                {
                    try
                    {
                        if (int.Parse(Build.VERSION.Release) >= 10 && (Secure.GetString(context.ContentResolver, Secure.EnabledAccessibilityServices) == null ||
                                   !Secure.GetString(context.ContentResolver, Secure.EnabledAccessibilityServices).Contains("KeyInfoCallAccessibilityService")))
                        {
                            Toast.MakeText(context, "Разрешение \"Специальные возможности\" не предоставлено", ToastLength.Long);
                            return;
                        }
                    }
                    catch (Exception) { }

                    string outboundPhoneNumber;
                    string callName;
                    try
                    {
                        var phones = Application.Context.ContentResolver.Query(CallLog.Calls.ContentUri, null, null, null, String.Format("{0} desc ", CallLog.Calls.Date));
                        phones.MoveToNext();
                        callName = phones.GetString(phones.GetColumnIndex(CallLog.Calls.CachedName));
                        outboundPhoneNumber = phones.GetString(phones.GetColumnIndex(CallLog.Calls.Number));
                    }
                    catch (Exception)
                    {
                        Toast.MakeText(context, "Не удалось определить номер собеседника...\nВозможно у приложения отсутствует доступ к списку вызовов", ToastLength.Long).Show();
                        callName = "Неизвестно";
                        outboundPhoneNumber = "0";
                    }
                    RecentCall recentCall = new RecentCall()
                    {
                        Number = outboundPhoneNumber,
                        PhoneNumber = callName == null || callName == "" ? outboundPhoneNumber : callName,
                        DateAndTime = DateTime.Now
                    };

                    if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.RecordAudio) == (int)Permission.Granted &&
                        ContextCompat.CheckSelfPermission(context, Manifest.Permission.WriteExternalStorage) == (int)Permission.Granted)
                    {
                        SqlData.SaveRecentCall(recentCall);
                        context.StopService(mIntent);
                        Thread.Sleep(2000);
                        MainActivity.UpdateData();
                    }
                    else
                    {
                        Toast.MakeText(context, "Ошибка в работе приложения \"KeyInfoCall\"\nПроверьте все необходимые разрешения", ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception)
            {
                Toast.MakeText(context, "Возникла ошибка ", ToastLength.Long).Show();
            }
        }
    }
}