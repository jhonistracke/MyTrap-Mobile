using MyTrapApp.Models.App;
using MyTrapApp.Models.Request;
using SQLite.Net;
using System;
using System.IO;

namespace MyTrapApp.Repository
{
    public class MyTrapDB : IDisposable
    {
        private static string DatabaseName = "mytrap_app.db3";

        private SQLiteConnection Connection { get; set; }

        public SQLiteConnection GetConnection()
        {
            Connection = new SQLiteConnection(AppStatus.SQLitePlatform, Path.Combine(AppStatus.DirectoryBD, DatabaseName));

            Connection.CreateTable<Preference>();
            Connection.CreateTable<PositionApiRequest>();

            return Connection;
        }

        public void Dispose()
        {
            try
            {
                if (Connection != null)
                {
                    Connection.Close();
                }
            }
            catch (Exception)
            {
                Connection = null;
            }
        }
    }
}