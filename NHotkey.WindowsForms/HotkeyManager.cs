using System;
using System.Windows.Forms;

namespace NHotkey.WindowsForms
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design",
        "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
        Justification = "This is a singleton; disposing it would break it")]
    public class HotkeyManager : HotkeyManagerBase
    {
        #region Singleton implementation

        public static HotkeyManager Current { get { return LazyInitializer.Instance; } }

        private static class LazyInitializer
        {
            static LazyInitializer() { }
            public static readonly HotkeyManager Instance = new HotkeyManager();
        }

        #endregion

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly MessageWindow _messageWindow;

        private HotkeyManager()
        {
            _messageWindow = new MessageWindow(this);
            SetHwnd(_messageWindow.Handle);
        }

        public void AddOrReplace(string name, Keys keys, bool noRepeat, EventHandler<HotkeyEventArgs> handler)
        {
            var flags = GetFlags(keys, noRepeat);
            var vk = unchecked((uint)(keys & ~Keys.Modifiers));
            AddOrReplace(name, vk, flags, handler);
        }

        public void AddOrReplace(string name, Keys keys, EventHandler<HotkeyEventArgs> handler)
        {
            AddOrReplace(name, keys, false, handler);
        }

        private static HotkeyFlags GetFlags(Keys hotkey, bool noRepeat)
        {
            var noMod = hotkey & ~Keys.Modifiers;
            var flags = HotkeyFlags.None;
            if (hotkey.HasFlag(Keys.Alt))
                flags |= HotkeyFlags.Alt;
            if (hotkey.HasFlag(Keys.Control))
                flags |= HotkeyFlags.Control;
            if (hotkey.HasFlag(Keys.Shift))
                flags |= HotkeyFlags.Shift;
            if (noMod == Keys.LWin || noMod == Keys.RWin)
                flags |= HotkeyFlags.Windows;
            if (noRepeat)
                flags |= HotkeyFlags.NoRepeat;
            return flags;
        }

        class MessageWindow : ContainerControl
        {
            private readonly HotkeyManager _hotkeyManager;

            public MessageWindow(HotkeyManager hotkeyManager)
            {
                _hotkeyManager = hotkeyManager;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    var parameters = base.CreateParams;
                    parameters.Parent = HwndMessage;
                    return parameters;
                }
            }

            protected override void WndProc(ref Message m)
            {
                bool handled = false;
                Hotkey hotkey;
                m.Result = _hotkeyManager.HandleHotkeyMessage(Handle, m.Msg, m.WParam, m.LParam, ref handled, out hotkey);
                if (!handled)
                    base.WndProc(ref m);
            }
        }
    }
}
