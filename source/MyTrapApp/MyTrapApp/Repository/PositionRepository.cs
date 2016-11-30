using MyTrapApp.Models.Request;
using SQLite.Net;
using System.Collections.Generic;
using System.Linq;

namespace MyTrapApp.Repository
{
    public class PositionRepository
    {
        public static void Save(PositionApiRequest position)
        {
            using (SQLiteConnection con = new MyTrapDB().GetConnection())
            {
                con.Insert(position);
            }
        }

        public static List<PositionApiRequest> GetAll()
        {
            List<PositionApiRequest> positions;

            using (SQLiteConnection con = new MyTrapDB().GetConnection())
            {
                positions = con.Table<PositionApiRequest>().ToList();
            }

            return positions;
        }

        public static void Delete(int idPosition)
        {
            using (SQLiteConnection con = new MyTrapDB().GetConnection())
            {
                PositionApiRequest position = con.Table<PositionApiRequest>().FirstOrDefault(obj => obj.Id == idPosition);

                con.Delete(position);
            }
        }
    }
}