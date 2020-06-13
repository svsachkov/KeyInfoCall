using System;

using Azure;
using Azure.AI.TextAnalytics;
using Google.Cloud.Translation.V2;

namespace Coursework
{
    class EntityRecognition
    {
        private static readonly AzureKeyCredential credentials = new AzureKeyCredential("3e0ef0b048a14aa697c54deffe7ca34e");
        private static readonly Uri endpoint = new Uri("https://stepan.cognitiveservices.azure.com/");

        public static void RecognizeEntities(string text, int position)
        {
            string events = "";
            string locations = "";
            string time = "";
            string others = "";

            var googleClient = TranslationClient.CreateFromApiKey("AIzaSyCVpDRvqpr4XLV7xb2W9Cn8wycDz89k9wQ");
            var googleResponse = googleClient.TranslateText(text, LanguageCodes.English, LanguageCodes.Russian);
            text = googleResponse.TranslatedText;

            for (int i = 0; i <= text.Length / 5120; i++)
            {
                int length = text.Substring(i * 5120).Length >= 5120 ? 5120 : text.Substring(i * 5120).Length;

                var client = new TextAnalyticsClient(endpoint, credentials);
                var response = client.RecognizeEntities(text.Substring(i * 5120, length));
                foreach (var entity in response.Value)
                {
                    switch (entity.Category.ToString())
                    {
                        case "Event":
                            events += entity.Text + ",";
                            break;
                        case "Location":
                            locations += entity.Text + ",";
                            break;
                        case "DateTime":
                            time += entity.Text + ",";
                            break;
                        default:
                            others += entity.Text + ",";
                            break;
                    }
                }
                var response2 = client.RecognizeLinkedEntities(text);
                foreach (var entity in response2.Value)
                {
                    others += entity.Name + ",";
                }
            }

            events = googleClient.TranslateText(events, LanguageCodes.Russian, LanguageCodes.English).TranslatedText;
            locations = googleClient.TranslateText(locations, LanguageCodes.Russian, LanguageCodes.English).TranslatedText;
            time = googleClient.TranslateText(time, LanguageCodes.Russian, LanguageCodes.English).TranslatedText;
            others = googleClient.TranslateText(others, LanguageCodes.Russian, LanguageCodes.English).TranslatedText;
            events = events.Replace(", ", ",");
            events = events.Replace(" ,", ",");
            locations = locations.Replace(", ", ",");
            locations = locations.Replace(" ,", ",");
            time = time.Replace(", ", ",");
            time = time.Replace(" ,", ",");
            others = others.Replace(", ", ",");
            others = others.Replace(" ,", ",");

            SqlData.ProccessKeyInfo(position, new KeyInfo()
            {
                Events = events,
                Locations = locations,
                Time = time,
                OtherInfo = others,
                TextFromSpeech = ""
            });
        }
    }
}