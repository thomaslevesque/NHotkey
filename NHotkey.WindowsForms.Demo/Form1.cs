using System;
using System.Windows.Forms;

namespace NHotkey.WindowsForms.Demo
{
    public partial class Form1 : Form
    {
        private readonly WindowsFormsHotkeyManager _hotkeyManager;
        private int _value;


        public Form1()
        {
            _hotkeyManager = new WindowsFormsHotkeyManager(this);
            _hotkeyManager.Add("Increment", Keys.Control | Keys.Alt | Keys.Add, OnIncrement);
            _hotkeyManager.Add("Decrement", Keys.Control | Keys.Alt | Keys.Subtract, OnDecrement);
            InitializeComponent();
        }

        private void OnIncrement(object sender, HotkeyEventArgs e)
        {
            Value++;
        }

        private void OnDecrement(object sender, HotkeyEventArgs e)
        {
            Value--;
        }

        private int Value
        {
            get { return _value; }
            set
            {
                _value = value;
                lblValue.Text = _value.ToString();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _hotkeyManager.Dispose();
        }
    }
}
