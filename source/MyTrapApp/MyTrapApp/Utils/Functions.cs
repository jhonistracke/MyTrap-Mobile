using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MyTrapApp.Utils
{
    public class Functions
    {
        public static string GetBase64ImageFromUrl(string url)
        {
            try
            {
                WebRequest req = WebRequest.Create(url);
                var task = req.GetResponseAsync();

                Stream stream = task.Result.GetResponseStream();

                byte[] bytes;

                using (MemoryStream ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    bytes = ms.ToArray();
                }

                string base64 = Convert.ToBase64String(bytes);

                return base64;
            }
            catch (Exception)
            {
                return StringUtils.EMPTY;
            }
        }

        public static async Task<byte[]> GetBytesFromImage(string imageUrl)
        {
            Stream stream = null;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                try
                {
                    WebRequest request = HttpWebRequest.Create(imageUrl);

                    var response = await request.GetResponseAsync();

                    stream = response.GetResponseStream();
                }
                catch (Exception exception)
                {
                    string tst = exception.Message;
                }
            }

            if (stream != null)
            {
                return await GetByteArrayFromStream(stream);
            }
            else
            {
                return null;
            }
        }

        public static async Task<byte[]> GetByteArrayFromStream(Stream stream)
        {
            if (stream != null)
            {
                byte[] buffer = new byte[16 * 1024];

                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    return ms.ToArray();
                }
            }
            else
            {
                return null;
            }
        }
    }
}