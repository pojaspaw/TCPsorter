using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SorterTCP
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static Mutex single = new Mutex(true, "SorterTCP");
        [STAThread]

       

        static void Main()
        {

            if (!single.WaitOne(0, false))
            {
                //there is already another instance running!
                MessageBox.Show("Program jest już otwarty!","!",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (System.Environment.MachineName != "DESKTOP-M2UCG3N")
            {
                Application.Exit();
                MessageBox.Show("Nieprawidłowa nazwa komputera");
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainApp());
        }
    }
}
