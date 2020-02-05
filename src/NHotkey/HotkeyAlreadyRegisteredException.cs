using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace NHotkey
{
    [Serializable]
    public class HotkeyAlreadyRegisteredException : Exception
    {
        private readonly string _name;

        public HotkeyAlreadyRegisteredException(string name, Exception inner) : base(inner.Message, inner)
        {
            _name = name;
            HResult = Marshal.GetHRForException(inner);
        }

        protected HotkeyAlreadyRegisteredException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            _name = (string) info.GetValue("_name", typeof (string));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_name", _name);
        }

        public string Name
        {
            get { return _name; }
        }
    }
}
