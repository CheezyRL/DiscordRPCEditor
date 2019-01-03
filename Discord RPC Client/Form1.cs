using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Discord_RPC_Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord RPC Editor")))
            {
                DirectoryInfo info = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord RPC Editor"));
                progressBar1.Value = 20;

                FileStream fs1 = new FileStream(Path.Combine(info.FullName, "DiscordRPC.dll"), FileMode.Create, FileAccess.Write);
                fs1.Write(Properties.Resources.DiscordRPC, 0, Properties.Resources.DiscordRPC.Length);
                fs1.Close();

                progressBar1.Value = 40;

                fs1 = new FileStream(Path.Combine(info.FullName, "Newtonsoft.Json.dll"), FileMode.Create, FileAccess.Write);
                fs1.Write(Properties.Resources.Newtonsoft_Json, 0, Properties.Resources.Newtonsoft_Json.Length);
                fs1.Close();

                progressBar1.Value = 50;

                fs1 = new FileStream(Path.Combine(info.FullName, "Discord RPC Editor.exe"), FileMode.Create, FileAccess.Write);
                fs1.Write(Properties.Resources.Discord_RPC_Editor, 0, Properties.Resources.Discord_RPC_Editor.Length);
                fs1.Close();

                progressBar1.Value = 70;

                fs1 = new FileStream(Path.Combine(info.FullName, "Updater.exe"), FileMode.Create, FileAccess.Write);
                fs1.Write(Properties.Resources.Updater, 0, Properties.Resources.Updater.Length);
                fs1.Close();

                progressBar1.Value = 80;

                fs1 = new FileStream(Path.Combine(info.FullName, "DRPCE_Updater.dll"), FileMode.Create, FileAccess.Write);
                fs1.Write(Properties.Resources.DRPCE_Updater, 0, Properties.Resources.DRPCE_Updater.Length);
                fs1.Close();

                progressBar1.Value = 90;

                fs1 = new FileStream(Path.Combine(info.FullName, "MySql.Data.dll"), FileMode.Create, FileAccess.Write);
                fs1.Write(Properties.Resources.MySql_Data, 0, Properties.Resources.MySql_Data.Length);
                fs1.Close();

                progressBar1.Value = 100;

                FileStream outStream = System.IO.File.Create(Path.Combine(info.FullName, "discord.ico"));
                Properties.Resources.app.Save(outStream);
                outStream.Flush();
                outStream.Close();

                string shortcutLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Discord RPC Editor.lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                shortcut.Description = "Points to the Discord RPC Editor.";
                shortcut.IconLocation = Path.Combine(info.FullName, "discord.ico");
                shortcut.TargetPath = Path.Combine(info.FullName, "Discord RPC Editor.exe");
                shortcut.WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord RPC Editor");
                shortcut.Save();
            }
            new Thread(new ThreadStart(() =>
            {
                Process.Start(new ProcessStartInfo()
                {
                    FileName = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord RPC Editor"), "Discord RPC Editor.exe"),
                    WorkingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord RPC Editor"),
                    Arguments = "\"" + Assembly.GetEntryAssembly().Location + "\""
                });
            })).Start();
            this.Close();
        }
    }
}
