using MyTrapApp.Models.App;
using SQLite.Net;
using System.Linq;

namespace MyTrapApp.Repository
{
    public class PreferenceRepository
    {
        public static void Save(string key, string value)
        {
            Preference preference = GetByKey(key);

            using (SQLiteConnection con = new MyTrapDB().GetConnection())
            {
                if (preference != null)
                {
                    preference.Value = value;

                    con.Update(preference);
                }
                else
                {
                    preference = new Preference() { Key = key, Value = value };
                    con.Insert(preference);
                }
            }
        }

        public static Preference GetByKey(string key)
        {
            Preference result;

            using (SQLiteConnection con = new MyTrapDB().GetConnection())
            {
                result = con.Table<Preference>().FirstOrDefault(obj => obj.Key == key);
            }

            return result;
        }
    }
}