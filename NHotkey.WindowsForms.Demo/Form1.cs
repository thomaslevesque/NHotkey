using System;
using System.Windows.Forms;

namespace NHotkey.WindowsForms.Demo
{
    public partial class Form1 : Form
    {
        private readonly HotkeyManager _hotkeyManager;
        private int _value;


        public Form1()
        {
            InitializeComponent();

            _hotkeyManager = new HotkeyManager(this);
            _hotkeyManager.AddOrReplace("Increment", Keys.Control | Keys.Alt | Keys.Add, OnIncrement);
            _hotkeyManager.AddOrReplace("Decrement", Keys.Control | Keys.Alt | Keys.Subtract, OnDecrement);
        }

        private void OnIncrement(object sender, HotkeyEventArgs e)
        {
            Value++;
            e.Handled = true;
        }

        private void OnDecrement(object sender, HotkeyEventArgs e)
        {
            Value--;
            e.Handled = true;
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

        private void btnChangeBindings_Click(object sender, EventArgs e)
        {
            _hotkeyManager.AddOrReplace("Increment", Keys.Control | Keys.Alt | Keys.Up, OnIncrement);
            _hotkeyManager.AddOrReplace("Decrement", Keys.Control | Keys.Alt | Keys.Down, OnDecrement);
        }
    }
}
