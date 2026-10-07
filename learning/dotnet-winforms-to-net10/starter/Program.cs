using System;
using System.Windows.Forms;

namespace ClickCounter
{
    internal static class Program
    {
        // This is the classic .NET Framework startup recipe: three lines.
        // Newer .NET replaces them with one line (you'll do that in the lesson).
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
