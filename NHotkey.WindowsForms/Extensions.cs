using System.Windows.Forms;

namespace NHotkey.WindowsForms
{
    static class Extensions
    {
        public static bool HasFlag(this Keys keys, Keys flag)
        {
            return (keys & flag) == flag;
        }
    }
}
