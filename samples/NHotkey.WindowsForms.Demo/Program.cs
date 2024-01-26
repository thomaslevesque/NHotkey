using System;
using System.Windows.Forms;

namespace NHotkey.WindowsForms.Demo
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if !NETFRAMEWORK
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#endif
            Application.Run(new Form1());
        }
    }
}
