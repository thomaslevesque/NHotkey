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

        public void Register(IntPtr hwnd, string name)
        {
            if (!NativeMethods.RegisterHotKey(hwnd, _id, _flags, _virtualKey))
            {
                var hr = Marshal.GetHRForLastWin32Error();
                var ex = Marshal.GetExceptionForHR(hr);
                if ((uint) hr == 0x80070581)
                    throw new HotkeyAlreadyRegisteredException(name, ex);
                throw ex;
            }
            _hwnd = hwnd;
        }

        public void Unregister()
        {
            if (_hwnd != IntPtr.Zero)
            {
                if (!NativeMethods.UnregisterHotKey(_hwnd, _id))
                {
                    var hr = Marshal.GetHRForLastWin32Error();
                    throw Marshal.GetExceptionForHR(hr);
                }
                _hwnd = IntPtr.Zero;
            }
        }
    }
}