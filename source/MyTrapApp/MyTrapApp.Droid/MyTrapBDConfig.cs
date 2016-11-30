using SQLite.Net.Platform.XamarinAndroid;
using System;

namespace MyTrapApp.Droid
{
    public class MyTrapBDConfig
    {
        public static void Initialize()
        {
            AppStatus.DirectoryBD = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            AppStatus.SQLitePlatform = new SQLitePlatformAndroid();
        }
    }
}