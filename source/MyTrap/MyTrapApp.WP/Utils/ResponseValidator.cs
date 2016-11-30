using MyTrapApp.Models.Base;
using System;
using Windows.UI.Popups;

namespace MyTrapApp.WP.Utils
{
    public class ResponseValidator
    {
        public static bool Validate(BaseApiResult response)
        {
            bool result = false;

            try
            {
                if (response != null)
                {
                    if (response.Error)
                    {
                        if (response.Message != null && !string.IsNullOrEmpty(response.Message))
                        {
                            ShowAlertError(response.Message);
                        }
                        else
                        {
                            ShowAlertUnknownError();
                        }
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    ShowAlertUnknownError();
                }
            }
            catch (Exception)
            {
                ShowAlertUnknownError();
            }

            return result;
        }

        private static async void ShowAlertError(string content)
        {
            var dialog = new MessageDialog(content);

            dialog.Title = "Warning";

            dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });

            var res = await dialog.ShowAsync();
        }

        private static async void ShowAlertUnknownError()
        {
            var dialog = new MessageDialog("Unable to perform the operation. Try again later");

            dialog.Title = "Warning";

            dialog.Commands.Add(new UICommand { Label = "Ok", Id = 0 });

            var res = await dialog.ShowAsync();
        }
    }
}