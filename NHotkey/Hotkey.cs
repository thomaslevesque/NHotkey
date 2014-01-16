using System;
using System.Runtime.InteropServices;

namespace NHotkey
{
    internal class Hotkey
    {
        private static int _nextId;

        private readonly int _id;
        private readonly uint _virtualKey;
        private readonly HotkeyFlags _flags;
        private readonly EventHandler<HotkeyEventArgs> _handler;

        public Hotkey(uint virtualKey, HotkeyFlags flags, EventHandler<HotkeyEventArgs> handler)
        {
            _id = ++_nextId;
            _virtualKey = virtualKey;
            _flags = flags;
            _handler = handler;
        }

        public int Id
        {
            get { return _id; }
        }

        public uint VirtualKey
        {
            get { return _virtualKey; }
        }

        public HotkeyFlags Flags
        {
            get { return _flags; }
        }

        public EventHandler<HotkeyEventArgs> Handler
        {
            get { return _handler; }
        }

        private IntPtr _hwnd;

        public void Register(IntPtr hwnd)
        {
            if (!RegisterHotKey(hwnd, _id, _flags, _virtualKey))
            {
                var hr = Marshal.GetHRForLastWin32Error();
                throw Marshal.GetExceptionForHR(hr);
            }
            _hwnd = hwnd;
        }

        public void Unregister()
        {
            if (_hwnd != IntPtr.Zero)
            {
                if (!UnregisterHotKey(_hwnd, _id))
                {
                    var hr = Marshal.GetHRForLastWin32Error();
                    throw Marshal.GetExceptionForHR(hr);
                }
                _hwnd = IntPtr.Zero;
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, HotkeyFlags fsModifiers, uint vk);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
            
    }
}