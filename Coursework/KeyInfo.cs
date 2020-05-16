namespace Coursework
{
    public class KeyInfo
    {
        /// <summary>
        /// Events from the speech.
        /// </summary>
        public string Events { get; set; }

        /// <summary>
        /// Locations from the speech.
        /// </summary>
        public string Locations { get; set; }

        /// <summary>
        /// Date and time from the speech.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Other information from the speech.
        /// </summary>
        public string OtherInfo { get; set; }

        /// <summary>
        /// Whole conversation as text.
        /// </summary>
        public string TextFromSpeech { get; set; }
    }
}