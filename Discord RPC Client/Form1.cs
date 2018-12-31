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

                progressBar1.Value = 78;

                fs1 = new FileStream(Path.Combine(info.FullName, "Newtonsoft.Json.dll"), FileMode.Create, FileAccess.Write);
                fs1.Write(Properties.Resources.Newtonsoft_Json, 0, Properties.Resources.Newtonsoft_Json.Length);
                fs1.Close();

                progressBar1.Value = 90;

                fs1 = new FileStream(Path.Combine(info.FullName, "Discord RPC Editor.exe"), FileMode.Create, FileAccess.Write);
                fs1.Write(Properties.Resources.Discord_RPC_Editor, 0, Properties.Resources.Discord_RPC_Editor.Length);
                fs1.Close();

                progressBar1.Value = 100;

                FileStream outStream = System.IO.File.Create(Path.Combine(info.FullName, "discord.ico"));
                Properties.Resources.app.Save(outStream);
                outStream.Flush();
                outStream.Close();

                string shortcutLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Discord RPC Editor.lnk");
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                shortcut.Description = "Points to the Discord RPC Editor.";   // The description of the shortcut
                shortcut.IconLocation = Path.Combine(info.FullName, "discord.ico");           // The icon of the shortcut
                shortcut.TargetPath = Path.Combine(info.FullName, "Discord RPC Editor.exe");                 // The path of the file that will launch when the shortcut is run
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
