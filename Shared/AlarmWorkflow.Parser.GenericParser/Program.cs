using System;
using System.Windows.Forms;

namespace AlarmWorkflow.Parser.GenericParser
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
            Application.Run(new AlarmWorkflow.Parser.GenericParser.Forms.MainForm());
        }
    }
}
