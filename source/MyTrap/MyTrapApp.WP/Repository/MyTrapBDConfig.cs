using Windows.Storage;

namespace MyTrapApp.WP.Repository
{
    public class MyTrapBDConfig
    {
        public static void Initialize()
        {
            AppStatus.DirectoryBD = ApplicationData.Current.LocalFolder.Path;
            AppStatus.SQLitePlatform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
        }
    }
}