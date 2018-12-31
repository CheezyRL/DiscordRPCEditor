using DiscordRPC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DiscordRPCClient
{
    public partial class CustomRPC : Form
    {
        public CustomRPC()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            if (System.IO.File.Exists("last-good-clientid")) textBox1.Text = System.IO.File.ReadAllText("last-good-clientid");
        }

        private void label1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Opening up your web browser. Follow steps 1 & 2, then paste your Client ID in the text field.", "Application Creation Instructions");
            Process.Start("https://www.reddit.com/r/discordapp/comments/a2c2un/how_to_setup_a_custom_discord_rich_presence_for/");
        }

        DiscordRpcClient client;
        private void checkCID_Click(object sender, EventArgs e)
        {
            label2.Text = "Client ID - Checking";
            checkCID.Enabled = false;
            textBox1.Enabled = false;
            client = new DiscordRpcClient(textBox1.Text);
            System.IO.File.WriteAllBytes("discord-rpc.log", new byte[0]);
            client.Logger = new DiscordRPC.Logging.FileLogger("discord-rpc.log") { Level = DiscordRPC.Logging.LogLevel.Warning };
            client.Initialize();
            new Thread(new ThreadStart(wait)).Start();
        }

        public void wait()
        {
            Thread.Sleep(5000);
            if (File.ReadAllText("discord-rpc.log").Contains("Invalid Client ID")) label2.Text = "Client ID - Invalid"; else { label2.Text = "Client ID - Valid"; System.IO.File.WriteAllText("last-good-clientid", textBox1.Text);
            }
            System.IO.File.Delete("discord-rpc.log");
            client.Dispose();
            checkCID.Enabled = true;
            textBox1.Enabled = true;
        }
    }
}
