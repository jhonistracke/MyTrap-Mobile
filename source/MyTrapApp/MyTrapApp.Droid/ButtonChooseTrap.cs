using Android.Content;
using Android.Widget;

namespace MyTrapApp.Droid
{
    public class ButtonChooseTrap : Button
    {
        public string TrapKey { get; set; }

        public ButtonChooseTrap(Context context)
            : base(context)
        {

        }
    }
}