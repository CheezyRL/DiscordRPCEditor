using DRPCE_Updater;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GUIUpdater
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            Updater updater = new Updater();

            new Thread(new ThreadStart(() =>
            {
                updater.Update();
            })).Start();
            Thread th = new Thread(new ThreadStart(() =>
            {
                while (!updater.DownloadComplete)
                {
                    progressBar1.Increment(1);
                    Thread.Sleep(25);
                }

                // Apply update here
                label2.Text = "Extracting...";
                if (Process.GetProcessesByName("Discord RPC Editor").Count() >= 1)
                {
                    foreach (Process pc in Process.GetProcessesByName("Discord RPC Editor"))
                    {
                        pc.Kill();
                    }
                }

                Thread.Sleep(5000);

                using (ZipArchive archive = ZipFile.Open(updater.DownloadedFilePath, ZipArchiveMode.Read))
                {
                    try
                    {
                        foreach(ZipArchiveEntry entry in archive.Entries)
                        {
                            if (!entry.FullName.Contains(Assembly.GetExecutingAssembly().GetName().Name) && !entry.FullName.Contains("MySql.Data.dll"))
                                entry.ExtractToFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord RPC Editor/" + entry.FullName), true);
                        }
                    } catch(Exception ex)
                    {
                    }
                }

                Thread.Sleep(2500);

                new Thread(new ThreadStart(() => {
                    Thread.Sleep(2000);
                    Process.Start(new ProcessStartInfo()
                    {
                        FileName = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord RPC Editor"), "Discord RPC Editor.exe"),
                        WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord RPC Editor")
                    });
                })).Start();

                this.Close();
            }));
            th.Start();
        }
    }
}
