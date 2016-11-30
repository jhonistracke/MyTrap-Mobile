using MyTrapApp.Models.Enums;
using MyTrapApp.Models.Result;
using SQLite.Net.Interop;

namespace MyTrapApp
{
    public class AppStatus
    {
        public static UserApiResult UserLogged = null;

        public static EPlatform Platform { get; set; }

        public static string DirectoryBD { get; set; }

        public static ISQLitePlatform SQLitePlatform { get; set; }

        public static string Language { get; set; }

        public static string AppRegistration { get; set; }
    }
}