using Android.App;
using Android.Content;
using MyTrapApp.Models.Base;
using System;

namespace MyTrapApp.Droid
{
    public class ResponseValidator
    {
        public static bool Validate(BaseApiResult response, Context context)
        {
            bool result = false;

            try
            {
                if (context != null)
                {
                    if (response != null)
                    {
                        if (response.Error)
                        {
                            //if (response.Status == ResponseStatus.ERROR_LOCAL)
                            //{
                            //    var msg = context.Resources.GetString(Resource.String.general_error);

                            //    ShowAlertError(msg, context);
                            //}
                            if (response.Message != null && !string.IsNullOrEmpty(response.Message))
                            {
                                ShowAlertError(response.Message, context);
                            }
                            else
                            {
                                ShowAlertUnknownError(context);
                            }
                        }
                        else
                        {
                            result = true;
                        }
                    }
                    else
                    {
                        ShowAlertUnknownError(context);
                    }
                }
            }
            catch (Exception)
            {
                ShowAlertUnknownError(context);
            }

            return result;
        }

        private static void ShowAlertError(string content, Context context)
        {
            if (context != null)
            {
                new AlertDialog.Builder(context)
                    .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                    .SetTitle(MyTrap.Droid.Resource.String.attention)
                    .SetMessage(content)
                    .Show();
            }
        }

        private static void ShowAlertUnknownError(Context context)
        {
            if (context != null)
            {
                new AlertDialog.Builder(context)
                   .SetIcon(Android.Resource.Drawable.IcDialogAlert)
                   .SetTitle(MyTrap.Droid.Resource.String.attention)
                   .SetMessage(MyTrap.Droid.Resource.String.general_error)
                   .Show();
            }
        }
    }
}