using System;
using System.Collections.Generic;
using System.Windows.Forms;
using StockMonitor.Forms;

namespace StockMonitor
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
            string[] args = Environment.GetCommandLineArgs();

            if(args.Length < 3){
                args = new string[3];
                args[1] = "600000";
                args[2] = "浦发银行";
            }
            Application.Run(new StockForm(args[1], args[2]));
        }
    }
}