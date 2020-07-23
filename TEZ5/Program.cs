using System;
using System.Windows.Forms;

namespace TEZ5
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OpenForm());
            Application.Run(new MainForm());


        }
    }
}
