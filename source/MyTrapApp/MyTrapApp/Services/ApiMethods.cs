namespace MyTrapApp.Services
{
    public class ApiMethods
    {
        private static string CONTROLLER_TAG = "#CONTROLLER#";

        private static string SERVER_API = "http://localhost:1313/api/" + CONTROLLER_TAG;

        public static string LoginUrl()
        {
            return GetUrlBase().Replace(CONTROLLER_TAG, "User/Login");
        }

        public static string GetUser()
        {
            return GetUrlBase().Replace(CONTROLLER_TAG, "User");
        }

        public static string SendPositionUrl()
        {
            return GetUrlBase().Replace(CONTROLLER_TAG, "Position/Send");
        }

        public static string ArmTrap()
        {
            return GetUrlBase().Replace(CONTROLLER_TAG, "Trap/Arm");
        }

        public static string GetArmedTraps()
        {
            return GetUrlBase().Replace(CONTROLLER_TAG, "Trap/Armed");
        }

        public static string AvailableTraps()
        {
            return GetUrlBase().Replace(CONTROLLER_TAG, "Purchase/AvailableTraps");
        }

        public static string InsertBuyIntent()
        {
            return GetUrlBase().Replace(CONTROLLER_TAG, "Purchase/InsertBuyIntent");
        }

        public static string InvalidateBuyIntent()
        {
            return GetUrlBase().Replace(CONTROLLER_TAG, "Purchase/InvalidateBuyIntent");
        }

        public static string RegisterPurchase()
        {
            return GetUrlBase().Replace(CONTROLLER_TAG, "Purchase/RegisterPurchase");
        }

        private static string GetUrlBase()
        {
            return SERVER_API;
        }
    }
}