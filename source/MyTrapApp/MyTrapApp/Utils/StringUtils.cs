namespace MyTrapApp.Utils
{
    public class StringUtils
    {
        public static string EMPTY = "";

        public static bool IsNullOrEmpty(string value)
        {
            if (value == null || value == EMPTY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}