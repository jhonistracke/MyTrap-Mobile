using Android.App;
using System;
using Xamarin;

namespace MyTrapApp.Droid.Utils
{
    public class InsightsUtils
    {
        private static bool _initialized = false;

        public static void LogException(Exception exception)
        {
            if (!_initialized)
            {
                Insights.Initialize("db181fc2d697b48cb96dbcf463fe90ce7f3db682", Application.Context);

                _initialized = true;
            }

            Insights.Report(exception);
        }
    }
}