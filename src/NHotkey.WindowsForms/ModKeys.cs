using System.Windows.Forms;

namespace NHotkey.WindowsForms
{
    public static class ModKeys
    {
        public const Keys Shift = Keys.Shift;
        public const Keys Control = Keys.Control;
        public const Keys Alt = Keys.Alt;

        // This doesn't correspond to a real Keys value, and will be mapped to MOD_WIN when calling RegisterHotKey.
        // Use a value far from the existing ones, in case Microsoft decides to add new ones.
        public const Keys Windows = (Keys)0x80_0000;
    }
}
