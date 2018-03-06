using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jubilant_Waffle {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        private static System.Threading.Mutex mutex = null;
        [STAThread]
        static void Main() {
            #region Make the application single instance
            // code at http://www.c-sharpcorner.com/UploadFile/f9f215/how-to-restrict-the-application-to-just-one-instance/
            bool createdNew;
            const string appName = "Jubilant Waffle";
            mutex = new System.Threading.Mutex(true, appName, out createdNew);
            if (!createdNew) {
                return;
            }
            #endregion
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


        }
    }
}
