using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordRPCClient
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if(args.Count() >= 1)
            {
                try
                {
                    new Thread(new ThreadStart(() =>
                    {
                        Thread.Sleep(2000);
                        File.Delete(args[0]);
                    })).Start();
                } catch(Exception ex) { }
            }
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            } catch (Exception ex) { }
        }
    }
}
