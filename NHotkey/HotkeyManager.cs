using System;
using System.Collections.Generic;

namespace NHotkey
{
    public abstract class HotkeyManager : IDisposable
    {
        private readonly Dictionary<int, string> _hotkeyNames = new Dictionary<int, string>();
        private readonly Dictionary<string, Hotkey> _hotkeys = new Dictionary<string, Hotkey>();


        internal void Add(string name, uint virtualKey, HotkeyFlags flags, EventHandler<HotkeyEventArgs> handler)
        {
            var hotkey = new Hotkey(virtualKey, flags, handler);
            lock (_hotkeys)
            {
                _hotkeys.Add(name, hotkey);
                _hotkeyNames.Add(hotkey.Id, name);
            }
        }

        public void Remove(string name)
        {
            lock (_hotkeys)
            {
                Hotkey hotkey;
                if (_hotkeys.TryGetValue(name, out hotkey))
                {
                    _hotkeys.Remove(name);
                    _hotkeyNames.Remove(hotkey.Id);
                }
            }
        }

        internal void Register(IntPtr hwnd)
        {
            lock (_hotkeys)
            {
                foreach (var hotkey in _hotkeys.Values)
                {
                    hotkey.Register(hwnd);
                }
            }
        }

        internal void Unregister()
        {
            lock (_hotkeys)
            {
                foreach (var hotkey in _hotkeys.Values)
                {
                    hotkey.Unregister();
                }
            }
        }

        public virtual void Dispose()
        {
            Unregister();
        }

        private const int WmHotkey = 0x0312;

        internal IntPtr HandleHotkeyMessage(
            IntPtr hwnd,
            int msg,
            IntPtr wParam,
            IntPtr lParam,
            ref bool handled,
            out Hotkey hotkey)
        {
            hotkey = null;
            if (msg == WmHotkey)
            {
                int id = wParam.ToInt32();
                string name;
                if (_hotkeyNames.TryGetValue(id, out name))
                {
                    hotkey = _hotkeys[name];
                    var handler = hotkey.Handler;
                    if (handler != null)
                    {
                        var e = new HotkeyEventArgs(name);
                        handler(this, e);
                        handled = e.Handled;
                    }
                }
            }
            return IntPtr.Zero;
        }
    }
}
