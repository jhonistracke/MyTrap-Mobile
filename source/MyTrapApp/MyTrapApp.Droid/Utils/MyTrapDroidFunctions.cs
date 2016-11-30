using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Util;
using Java.Security;
using Java.Util;
using MyTrapApp.Services;
using System;

namespace MyTrapApp.Droid.Utils
{
    public class MyTrapDroidFunctions
    {
        public static void PrintAppHash(Context context)
        {
            try
            {
                PackageInfo info = context.PackageManager.GetPackageInfo("com.mytrap", PackageInfoFlags.Signatures);

                foreach (Android.Content.PM.Signature signature in info.Signatures)
                {
                    MessageDigest md;
                    md = MessageDigest.GetInstance("SHA");
                    md.Update(signature.ToByteArray());

                    string hash = System.Text.Encoding.Default.GetString(Base64.Encode(md.Digest(), Base64Flags.Default));
                }
            }
            catch (Exception)
            { }
        }

        public static Bitmap GetImageFromBase64(string base64)
        {
            Bitmap image = null;

            try
            {

                byte[] decodedString = Base64.Decode(base64, Android.Util.Base64Flags.Default);
                image = BitmapFactory.DecodeByteArray(decodedString, 0, decodedString.Length);
            }
            catch (Exception)
            {
                image = null;
            }

            return image;
        }

        public static Bitmap GetCroppedBitmap(Bitmap bitmap)
        {
            Bitmap output = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(output);

            Paint paint = new Paint();
            Rect rect = new Rect(0, 0, bitmap.Width, bitmap.Height);
            RectF rectF = new RectF(rect);

            paint.AntiAlias = true;
            canvas.DrawARGB(0, 0, 0, 0);
            paint.Color = Color.Red;
            canvas.DrawOval(rectF, paint);

            paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
            canvas.DrawBitmap(bitmap, rect, rect, paint);

            bitmap.Recycle();

            return output;
        }

        public static void OpenHome(Activity activity)
        {
            try
            {
                PositionService.Start(activity.BaseContext);

                activity.StartActivity(typeof(HomeActivity));

                activity.Finish();
            }
            catch (Exception exception)
            {
                InsightsUtils.LogException(exception);
            }
        }
    }
}