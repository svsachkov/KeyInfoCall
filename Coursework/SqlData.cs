using System;
using System.IO;
using System.Collections.Generic;

using SQLite;
using Android.Widget;
using Android.Content;
using System.Runtime.CompilerServices;

namespace Coursework
{
    /// <summary>
    /// Class for working with the local database.
    /// </summary>
    public class SqlData
    {
        static Context contex1 = null;
        // Path to the file, where information about recent calls is saved.
        readonly static string pathRecentCalls = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "PhoneCalls.db3");

        // Path to the file, where key information from calls is saved.
        readonly static string pathKeyInfo = Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "HighlightedInformation.db3");

        /// <summary>
        /// Method to save information about recent call.
        /// </summary>
        /// <param name="recentCall">Recent call (number + time)</param>
        public static void SaveRecentCall(RecentCall recentCall)
        {
            var db = new SQLiteConnection(pathRecentCalls);
            List<RecentCall> recentCalls = GetRecentCalls();
            if (recentCalls == null || (recentCalls != null && recentCalls.Count > 0 && (recentCall.DateAndTime - recentCalls[0].DateAndTime) > new TimeSpan(0, 0, 10)))
            {
                db.CreateTable<RecentCall>();
                db.Insert(recentCall);
            }
        }

        /// <summary>
        /// Method to get information about recent calls.
        /// </summary>
        /// <returns>
        /// List with recent calls (number + time).
        /// </returns>
        public static List<RecentCall> GetRecentCalls()
        {
            List<RecentCall> recentCalls = null;
            try
            {
                var db = new SQLiteConnection(pathRecentCalls);
                recentCalls = db.Table<RecentCall>().OrderByDescending(x => x.DateAndTime).ToList();
                return db.Table<RecentCall>().OrderByDescending(x => x.DateAndTime).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void RemoveRecentCall(int position)
        {
            try
            {
                var db = new SQLiteConnection(pathRecentCalls);
                List<RecentCall> recentCalls = GetRecentCalls();
                db.DeleteAll<RecentCall>();
                recentCalls.RemoveAt(position);
                if (recentCalls.Count > 0)
                    db.InsertAll(recentCalls);
                else
                    File.Delete(pathRecentCalls);

                var db2 = new SQLiteConnection(pathKeyInfo);
                List<KeyInfo> keyInfo = db2.Table<KeyInfo>().ToList();
                keyInfo.Reverse();
                keyInfo.RemoveAt(position);
                db2.DeleteAll<KeyInfo>();
                keyInfo.Reverse();
                if (keyInfo.Count > 0)
                    db2.InsertAll(keyInfo);
                else
                    File.Delete(pathKeyInfo);
            }
            catch (Exception) { }
        }

        public static void SaveKeyInfo(KeyInfo keyInfo, Context context)
        {
            contex1 = context;
            List<RecentCall> recentCalls = GetRecentCalls();
            List<KeyInfo> oldKeyInfo;
            try
            {
                var db = new SQLiteConnection(pathKeyInfo);
                try
                {
                    oldKeyInfo = db.Table<KeyInfo>().ToList();
                }
                catch (Exception)
                {
                    oldKeyInfo = null;
                }
                if (oldKeyInfo == null || recentCalls.Count - oldKeyInfo.Count == 1)
                {
                    db.CreateTable<KeyInfo>();
                    db.Insert(keyInfo);
                }
            }
            catch (Exception) 
            {
                Toast.MakeText(context,$"Что-то пошло не так при сохранении данных", ToastLength.Long).Show();
            }
        }

        public static KeyInfo GetKeyInfo(int position)
        {
            try
            {
                var db = new SQLiteConnection(pathKeyInfo);
                List<KeyInfo> keyInfo = db.Table<KeyInfo>().ToList();
                keyInfo.Reverse();
                return keyInfo[position];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void ProccessKeyInfo(int position, KeyInfo newKeyInfo)
        {
            try
            {
                var db = new SQLiteConnection(pathKeyInfo);
                List<KeyInfo> keyInfo = db.Table<KeyInfo>().ToList();
                keyInfo.Reverse();
                keyInfo.RemoveAt(position);
                keyInfo.Insert(position, newKeyInfo);
                keyInfo.Reverse();
                File.Delete(pathKeyInfo);
                var db2 = new SQLiteConnection(pathKeyInfo);
                db2.CreateTable<KeyInfo>();
                db2.InsertAll(keyInfo);
            }
            catch (Exception) 
            {
                Toast.MakeText(contex1, "Ошибка обработки информации", ToastLength.Long).Show();
            }
        }
    }
}