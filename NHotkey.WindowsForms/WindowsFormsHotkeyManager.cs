using System;
using System.Windows.Forms;

namespace NHotkey.WindowsForms
{
    public class WindowsFormsHotkeyManager : HotkeyManager
    {
        private readonly Form _form;
        private readonly DelegateMessageFilter _filter;

        public WindowsFormsHotkeyManager(Form form)
        {
            _form = form;
            _filter = new DelegateMessageFilter(HandleMessage);

            _form.HandleCreated += HandleCreated;
            _form.HandleDestroyed += HandleDestroyed;
            
        }

        public void Add(string name, Keys keys, bool noRepeat, EventHandler<HotkeyEventArgs> handler)
        {
            var flags = GetFlags(keys, noRepeat);
            var vk = unchecked((uint)(keys & ~Keys.Modifiers));
            Add(name, vk, flags, handler);
        }

        public void Add(string name, Keys keys, EventHandler<HotkeyEventArgs> handler)
        {
            Add(name, keys, false, handler);
        }

        private void HandleCreated(object sender, EventArgs e)
        {
            Application.AddMessageFilter(_filter);
            Register(_form.Handle);
        }

        private void HandleDestroyed(object sender, EventArgs e)
        {
            Application.RemoveMessageFilter(_filter);
            Unregister();
        }

        public override void Dispose()
        {
            base.Dispose();
            _form.HandleCreated -= HandleCreated;
            _form.HandleDestroyed -= HandleDestroyed;
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

        private bool HandleMessage(ref Message m)
        {
            bool handled = false;
            Hotkey hotkey;
            m.Result = HandleHotkeyMessage(_form.Handle, m.Msg, m.WParam, m.LParam, ref handled, out hotkey);
            return handled;
        }

        private delegate bool MessageHandler(ref Message m);
        private class DelegateMessageFilter : IMessageFilter
        {
            private readonly MessageHandler _handler;

            public DelegateMessageFilter(MessageHandler handler)
            {
                _handler = handler;
            }

            public bool PreFilterMessage(ref Message m)
            {
                return _handler(ref m);
            }
        }
    }
}
