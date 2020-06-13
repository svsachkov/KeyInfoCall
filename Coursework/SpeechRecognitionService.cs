using Android.OS;
using Android.App;
using Android.Media;
using Android.Speech;
using Android.Content;
using Android.Runtime;

using Java.Util;
using Java.Lang;
using Android.Widget;

namespace Coursework
{
    [Service]
    public class SpeechRecognitionService : Service, IRecognitionListener
    {
        private bool isServiceRunning = true;
        private static string voiceInput = "";
        SpeechRecognizer speechRecognizer;
        Intent voiceIntent;

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            // Preparing.
            Thread.Sleep(3000);
            StartSpeechRecognition();
            return base.OnStartCommand(intent, flags, startId);
        }

        public void StartSpeechRecognition()
        {
            // Unmute Google beep sound.
            try { MuteSound(false); }
            catch (Exception) { }
            
            // Intent to listen to user vocal input and return the result to the same activity.
            voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 5000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 10000);
            voiceIntent.PutExtra(RecognizerIntent.ActionVoiceSearchHandsFree, true);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSecure, false);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, "ru-Ru");
            voiceIntent.PutExtra(RecognizerIntent.ExtraCallingPackage, PackageName);

            // Add custom listeners.
            speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(this);
            speechRecognizer.SetRecognitionListener(this);
            speechRecognizer.StartListening(voiceIntent);
        }

        public void OnBeginningOfSpeech() { }

        public override IBinder OnBind(Intent intent) => null;

        public void OnBufferReceived(byte[] buffer) { }

        public void OnEndOfSpeech()
        {
            // Mute Google beep sound.
            //try { MuteSound(true); }
            //catch (Exception) { }

            Thread.Sleep(500);

            StartSpeechRecognition();
        }

        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
            // Mute Google beep sound.
            //try { MuteSound(true); }
            //catch (Exception) { }

            if (isServiceRunning)
            {
                Thread.Sleep(500);

                speechRecognizer.StartListening(voiceIntent);
            }
        }

        public void OnEvent(int eventType, Bundle @params) { }

        public void OnPartialResults(Bundle partialResults) { }

        public void OnReadyForSpeech(Bundle @params) { }

        public void OnResults(Bundle results)
        {
            var matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);

            if (matches.Count != 0)
            {
                voiceInput += " " + matches[0];
            }
        }

        public void OnRmsChanged(float rmsdB) { }

        public override void OnDestroy()
        {
            SqlData.SaveKeyInfo(new KeyInfo()
            {
                Events = "",
                Locations = "",
                Time = "",
                OtherInfo = "",
                TextFromSpeech = voiceInput
            }, this);

            voiceInput = "";
            isServiceRunning = false;

            base.OnDestroy();
        }

        /// <summary>
        /// Mute/Unmute Google beep sound.
        /// </summary>
        /// <param name="flag">Mute flag.</param>
        public void MuteSound(bool flag)
        {
            AudioManager amanager = (AudioManager)GetSystemService(Context.AudioService);
            amanager.SetStreamMute(Stream.Notification, flag);
            amanager.SetStreamMute(Stream.Alarm, flag);
            amanager.SetStreamMute(Stream.Music, flag);
            amanager.SetStreamMute(Stream.Ring, flag);
            amanager.SetStreamMute(Stream.System, flag);
        }
    }
}