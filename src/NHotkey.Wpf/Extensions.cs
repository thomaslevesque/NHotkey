using System.Windows.Input;

namespace NHotkey.Wpf
{
    static class Extensions
    {
        public static bool HasFlag(this ModifierKeys modifiers, ModifierKeys flag)
        {
            return (modifiers & flag) == flag;
        }

        public static bool HasFlag(this HotkeyFlags flags, HotkeyFlags flag)
        {
            return (flags & flag) == flag;
        }
    }
}
