using SQLite.Net.Attributes;

namespace MyTrapApp.Models.App
{
    public class Preference
    {
        public static string USER_LOGGED_JSON = "USER_LOGGED_JSON";

        [PrimaryKey]
        public string Key { get; set; }
        public string Value { get; set; }
    }
}