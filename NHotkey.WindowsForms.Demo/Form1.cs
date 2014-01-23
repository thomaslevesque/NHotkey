using System;
using System.Windows.Forms;

namespace NHotkey.WindowsForms.Demo
{
    public partial class Form1 : Form
    {
        private int _value;

        public Form1()
        {
            InitializeComponent();

            HotkeyManager.Current.AddOrReplace("Increment", Keys.Control | Keys.Alt | Keys.Add, OnIncrement);
            HotkeyManager.Current.AddOrReplace("Decrement", Keys.Control | Keys.Alt | Keys.Subtract, OnDecrement);
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

        private void btnChangeBindings_Click(object sender, EventArgs e)
        {
            HotkeyManager.Current.AddOrReplace("Increment", Keys.Control | Keys.Alt | Keys.Up, OnIncrement);
            HotkeyManager.Current.AddOrReplace("Decrement", Keys.Control | Keys.Alt | Keys.Down, OnDecrement);
        }
    }
}
