using System;

namespace NHotkey
{
    public class HotkeyEventArgs : EventArgs
    {
        private readonly string _name;

        internal HotkeyEventArgs(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }

        public bool Handled { get; set; }
    }
}