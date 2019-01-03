using DiscordRPC;
using DRPCE_Updater;
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

namespace DiscordRPCClient
{
    public partial class Form1 : Form
    {
        public bool enablePartyText;
        public int validPartySize, validMaxPartySize;

        public bool updatedClientID;

        public Form1()
        {
            File.WriteAllText("discord-rpc.txt", "");
            updatedClientID = false;
            running = true;
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            validPartySize = 1; validMaxPartySize = 1;
            if(File.Exists("user-data-save")) LoadPreviousState();
            pictureBox2.MouseEnter += PictureBox2_MouseEnter;
            pictureBox2.MouseLeave += PictureBox2_MouseLeave;
            pictureBox3.MouseEnter += PictureBox3_MouseEnter;
            pictureBox3.MouseLeave += PictureBox3_MouseLeave;
            FormClosing += Form1_FormClosing;
            Resize += Form1_Resize;
            notifyIcon1.MouseDoubleClick += NotifyIcon1_MouseDoubleClick;

            ContextMenu menu = new ContextMenu();
            menu.MenuItems.Add("Exit", (s, e) => { notifyIcon1.Dispose(); Application.Exit(); });

            notifyIcon1.ContextMenu = menu;

            detailsI.TextChanged += (s, e) => { details.Text = detailsI.Text; };
            stateI.TextChanged += (s, e) => { if (enablePartyText) { state.Text = stateI.Text + " (" + validPartySize + " of " + validMaxPartySize + ")"; } else state.Text = stateI.Text; };
            largeimagetextI.TextChanged += (s, e) => { LItooltip.Text = largeimagetextI.Text; };
            smallimagetextI.TextChanged += (s, e) => { SItooltip.Text = smallimagetextI.Text; };
            enableParty.CheckedChanged += (s, e) => {
                if (enableParty.Checked) { partysize.Enabled = true; partymax.Enabled = true; enablePartyText = true; state.Text = stateI.Text + " ("+validPartySize+" of "+validMaxPartySize+")"; }
                else { partysize.Enabled = false; partymax.Enabled = false; enablePartyText = false; state.Text = stateI.Text;  }
            };

            partysize.TextChanged += (s, e) => {
                if (partysize.Text != "")
                {
                    int ps = 1;
                    if (int.TryParse(partysize.Text, out ps))
                    {
                        if(ps >= 1) {
                            validPartySize = ps;
                            state.Text = stateI.Text + " (" + partysize.Text + " of " + GetMaxPartySize() + ")";
                        }
                        else
                        {
                            state.Text = stateI.Text + " (1 of " + validMaxPartySize + ")";
                            validPartySize = 1;
                            partysize.Text = "1";
                        }
                    }
                    else
                    {
                        state.Text = stateI.Text + " (1 of " + partymax + ")";
                        validPartySize = 1;
                        partysize.Text = "1";
                    }
                }
            };
            restart = false;
            partymax.TextChanged += (s, e) => {
                if (partymax.Text != "")
                {
                    int pm = 1;
                    if (int.TryParse(partymax.Text, out pm))
                    {
                        if (pm >= 1)
                        {
                            validMaxPartySize = pm;
                            state.Text = stateI.Text + " (" + GetPartySize() + " of " + partymax.Text + ")";
                        } else
                        {
                            state.Text = stateI.Text + " (" + validPartySize + " of " + validPartySize + ")";
                            validMaxPartySize = validPartySize;
                            partymax.Text = partysize.Text;
                        }
                    }
                    else
                    {
                        state.Text = stateI.Text + " (" + partysize.Text + " of " + partysize.Text + ")";
                        validMaxPartySize = validPartySize;
                        partymax.Text = partysize.Text;
                    }
                }
            };

            if (File.Exists("last-good-clientid"))
            {
                RPname.Text = "Application Name";
                label9.Text = "YES";
                label9.ForeColor = Color.ForestGreen;
            }
            else
            {
                RPname.Text = "Discord RPC Editor";
                label9.Text = "NO";
                label9.ForeColor = Color.Crimson;
            }

            Paint += (o, e) =>
            {
                System.Drawing.SolidBrush myBrush = new System.Drawing.SolidBrush(Color.FromArgb(43, 43, 43));
                e.Graphics.FillRectangle(myBrush, new Rectangle(0, 610, 1382, 100));
                myBrush.Dispose();
            };
        }

        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(5000, "Discord RPC Editor", "The RPC Editor has been minimized to the task tray.", ToolTipIcon.Info);
            }
        }

        public int GetPartySize()
        {
            if (validPartySize > validMaxPartySize)
            {
                validPartySize = validMaxPartySize;
                partysize.Text = validPartySize + "";
                return validPartySize;
            }
            return validPartySize;
        }

        public Thread updatethread;

        public int GetMaxPartySize()
        {
            if(validPartySize > validMaxPartySize)
            {
                validMaxPartySize = validPartySize;
                partymax.Text = validMaxPartySize + "";
                return validMaxPartySize;
            }
            return validMaxPartySize;
        }

        public void LoadPreviousState()
        {
            string[] save = File.ReadAllLines("user-data-save");
            try
            {
                detailsI.Text = save[0];
                largeimagetextI.Text = save[2];
                smallimagetextI.Text = save[3];
                largeimagekeyI.Text = save[4];
                smallimagekeyI.Text = save[5];
                enableParty.Checked = bool.Parse(save[6]);
                validPartySize = int.Parse(save[7]);
                validMaxPartySize = int.Parse(save[8]);
                partysize.Text = save[7];
                partymax.Text = save[8];
                checkBox1.Checked = bool.Parse(save[9]);

                details.Text = save[0];
                stateI.Text = save[1];
                LItooltip.Text = save[2];
                SItooltip.Text = save[3];
                partysize.Enabled = bool.Parse(save[6]);
                partymax.Enabled = bool.Parse(save[6]);
                state.Text = save[1] + (enableParty.Checked ? " (" + partysize.Text + " of " + partymax.Text + ")" : "");
                enablePartyText = bool.Parse(save[6]);
            } catch(Exception ex)
            {
                MessageBox.Show("The data file was modified outside of the application and broke.\r\nRestoring default values.", "Error", MessageBoxButtons.OK);

                detailsI.Text = "using Discord RPC Editor";
                largeimagetextI.Text = "Discord RPC Editor";
                smallimagetextI.Text = "Developed by Cheezy#0001";
                largeimagekeyI.Text = "Default";
                smallimagekeyI.Text = "Default1";
                enableParty.Checked = false;
                validPartySize = 1;
                validMaxPartySize = 1;
                partysize.Text = "1";
                partymax.Text = "1";
                checkBox1.Checked = true;

                details.Text = "using Discord RPC Editor";
                stateI.Text = "In Editor";
                LItooltip.Text = "Discord RPC Editor";
                SItooltip.Text = "Developed by Cheezy#0001";
                partysize.Enabled = false;
                partymax.Enabled = false;
                state.Text = "In Editor";
                enablePartyText = false;
            }
        }

        bool running;

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null) client.Dispose();
            // Save user data for next run
            /*
             * Lines
             * 
             * Details
             * State
             * Large Image Text
             * Small Image Text
             * Large Image Key
             * Small Image Key
             * Enable Party
             * Party Size
             * Party Max Size
             * Persist Start Time
             * 
             */
            running = false;
            List<string> lines = new List<string>();
            lines.Add(detailsI.Text);
            lines.Add(stateI.Text);
            lines.Add(largeimagetextI.Text);
            lines.Add(smallimagetextI.Text);
            lines.Add(largeimagekeyI.Text);
            lines.Add(smallimagekeyI.Text);
            lines.Add(enableParty.Checked + "");
            lines.Add(partysize.Text);
            lines.Add(partymax.Text);
            lines.Add(checkBox1.Checked + "");
            File.WriteAllBytes("user-data-save", new byte[0]);
            File.WriteAllLines("user-data-save", lines);
            try
            {
                File.WriteAllBytes("discord-rpc.txt", new byte[0]);
                File.Delete("discord-rpc.txt");
            } catch(Exception ex) { }
        }

        private void PictureBox2_MouseLeave(object sender, EventArgs e)
        {
            LItooltip.Visible = false;
        }

        private void PictureBox2_MouseEnter(object sender, EventArgs e)
        {
            LItooltip.Text = largeimagetextI.Text;
            LItooltip.Visible = true;
        }

        private void PictureBox3_MouseLeave(object sender, EventArgs e)
        {
            SItooltip.Visible = false;
        }

        private void PictureBox3_MouseEnter(object sender, EventArgs e)
        {
            SItooltip.Text = smallimagetextI.Text;
            SItooltip.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string lastid = "";
            if (File.Exists("last-good-clientid")) lastid = File.ReadAllText("last-good-clientid");
            CustomRPC rpc = new CustomRPC();
            rpc.ShowDialog();
            if (File.Exists("last-good-clientid"))
            {
                RPname.Text = "Application Name";
                label9.Text = "YES" + (lastid == File.ReadAllText("last-good-clientid") ? "" : " - PENDING RPC RESTART");
                label9.ForeColor = Color.ForestGreen;
            }
            else
            {
                RPname.Text = "Discord RPC Editor";
                label9.Text = "NO";
                label9.ForeColor = Color.Crimson;
            }
        }

        DiscordRpcClient client;

        public string GetClientID()
        {
            if (File.Exists("last-good-clientid"))
            {
                return File.ReadAllText("last-good-clientid");
            }
            return "529111491501359135";
        }
        int hours, minutes, seconds;

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            Process.Start("https://twitch.tv/cheezyrl");
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.twitter.com/bnylund19");
        }
        public System.Timers.Timer timer;
        private void label13_Click(object sender, EventArgs e)
        {
            label9.Text = label9.Text.Replace(" - PENDING RPC RESTART", "");
            if (timer != null) { timer.Stop(); timer.Dispose(); }
            if (client != null) client.Dispose();
            client = new DiscordRpcClient(GetClientID());

            timer = new System.Timers.Timer(150);
            timer.Elapsed += (s, evt) => { client.Invoke(); };
            timer.Start();

            client.Initialize();

            if (enablePartyText)
            {
                client.SetPresence(new RichPresence()
                {
                    State = stateI.Text,
                    Details = detailsI.Text,
                    Timestamps = new Timestamps()
                    {
                        Start = DateTime.UtcNow
                    },
                    Assets = new Assets()
                    {
                        LargeImageText = largeimagetextI.Text,
                        SmallImageText = smallimagetextI.Text,
                        LargeImageKey = largeimagekeyI.Text.ToLower(),
                        SmallImageKey = smallimagekeyI.Text.ToLower()
                    },
                    Party = new Party()
                    {
                        Size = validPartySize,
                        Max = validMaxPartySize,
                        ID = "CheezyRPCEditor"
                    }
                });
            }
            else
            {
                client.SetPresence(new RichPresence()
                {
                    State = stateI.Text,
                    Details = detailsI.Text,
                    Timestamps = new Timestamps()
                    {
                        Start = DateTime.UtcNow
                    },
                    Assets = new Assets()
                    {
                        LargeImageText = largeimagetextI.Text,
                        SmallImageText = smallimagetextI.Text,
                        LargeImageKey = largeimagekeyI.Text.ToLower(),
                        SmallImageKey = smallimagekeyI.Text.ToLower()
                    }
                });
            }
            if(updatethread != null) { updatethread.Abort(); }
            updatethread = new Thread(new ThreadStart(() => {
                hours = 0; minutes = 0; seconds = 0;
                while (running && !restart)
                {
                    Thread.Sleep(1000);
                    if (!restart)
                    {
                        if (minutes == 59 && seconds == 59) { hours++; minutes = 0; seconds = 0; }
                        if (seconds == 59) { minutes++; seconds = 0; } else seconds++;
                        timestamp.Text = (hours > 0 ? hours + ":" : "") + (minutes < 10 && hours > 0 ? "0" + minutes + ":" : minutes + ":") + (seconds < 10 ? "0" + seconds + "" : seconds + "") + " elapsed";
                    }
                }
                restart = false;
            }));
            updatethread.Start();
        }

        public bool restart;

        private void label14_Click_1(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(Environment.CurrentDirectory, "discord-rpc.txt"));
        }

        private void label15_Click(object sender, EventArgs e)
        {
            string currentversion = Assembly.GetEntryAssembly().GetName().Version.ToString().Substring(0, Assembly.GetEntryAssembly().GetName().Version.ToString().Length - 2);
            if (Updater.UpdateAvailable("v" + currentversion))
            {
                DialogResult dr = MessageBox.Show("There is an update available!\r\nCurrent Version: v" +
                    " " + currentversion + "\r\nLatest Version: " + Updater.GetLatestVersion() + "\r\n" +
                    "Update?", "Updater", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    Process.Start("Updater.exe", "");
                }
            }
            else MessageBox.Show("You are on the latest version.");
        }

        private void label1_Click(object sender, EventArgs e)
        {
            string currentversion = Assembly.GetEntryAssembly().GetName().Version.ToString().Substring(0, Assembly.GetEntryAssembly().GetName().Version.ToString().Length - 2);
            MessageBox.Show("Current Version: v" + currentversion + "\r\n\r\nNeed help? Go to https://discord.gg/H9vQYzU and let me know in the help channel.", "Discord Rich Presence Editor", MessageBoxButtons.OK);
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.youtube.com/channel/UCb9ZZcFkeb6FjDnn8HthevA");
        }

        private void pictureBox7_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/H9vQYzU");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Apply rich presence
            if(client == null)
            {
                client = new DiscordRpcClient(GetClientID());

                timer = new System.Timers.Timer(150);
                timer.Elapsed += (s, evt) => { client.Invoke(); };
                timer.Start();
                File.WriteAllBytes("discord-rpc.txt", new byte[0]);
                client.Logger = new DiscordRPC.Logging.FileLogger("discord-rpc.txt");
                client.Initialize();
                if (enablePartyText)
                {
                    client.SetPresence(new RichPresence()
                    {
                        State = stateI.Text,
                        Details = detailsI.Text,
                        Timestamps = new Timestamps()
                        {
                            Start = DateTime.UtcNow
                        },
                        Assets = new Assets()
                        {
                            LargeImageText = largeimagetextI.Text,
                            SmallImageText = smallimagetextI.Text,
                            LargeImageKey = largeimagekeyI.Text.ToLower(),
                            SmallImageKey = smallimagekeyI.Text.ToLower()
                        },
                        Party = new Party()
                        {
                            Size = validPartySize,
                            Max = validMaxPartySize,
                            ID = "CheezyRPCEditor"
                        }
                    });
                }
                else
                {
                    client.SetPresence(new RichPresence()
                    {
                        State = stateI.Text,
                        Details = detailsI.Text,
                        Timestamps = new Timestamps()
                        {
                            Start = DateTime.UtcNow
                        },
                        Assets = new Assets()
                        {
                            LargeImageText = largeimagetextI.Text,
                            SmallImageText = smallimagetextI.Text,
                            LargeImageKey = largeimagekeyI.Text.ToLower(),
                            SmallImageKey = smallimagekeyI.Text.ToLower()
                        }
                    });
                }
                if (updatethread != null) { updatethread.Abort(); }
                updatethread = new Thread(new ThreadStart(() =>
                {
                    hours = 0; minutes = 0; seconds = 0;
                    while (running && !restart)
                    {
                        Thread.Sleep(1000);
                        if (minutes == 59 && seconds == 59) { hours++; minutes = 0; seconds = 0; }
                        if (seconds == 59) { minutes++; seconds = 0; } else seconds++;
                        timestamp.Text = (hours > 0 ? hours + ":" : "") + (minutes < 10 && hours > 0 ? "0" + minutes + ":" : minutes + ":") + (seconds < 10 ? "0" + seconds + "" : seconds + "") + " elapsed";
                    }
                    restart = false;
                }));
                updatethread.Start();
            } else
            {
                if (checkBox1.Checked && enablePartyText)
                {
                    client.SetPresence(new RichPresence()
                    {
                        State = stateI.Text,
                        Details = detailsI.Text,
                        Timestamps = new Timestamps()
                        {
                            Start = client.CurrentPresence.Timestamps.Start
                        },
                        Assets = new Assets()
                        {
                            LargeImageText = largeimagetextI.Text,
                            SmallImageText = smallimagetextI.Text,
                            LargeImageKey = largeimagekeyI.Text.ToLower(),
                            SmallImageKey = smallimagekeyI.Text.ToLower()
                        },
                        Party = new Party()
                        {
                            Size = validPartySize,
                            Max = validMaxPartySize,
                            ID = "CheezyRPCEditor"
                        }
                    });
                } else if (checkBox1.Checked && !enablePartyText)
                {
                    client.SetPresence(new RichPresence()
                    {
                        State = stateI.Text,
                        Details = detailsI.Text,
                        Timestamps = new Timestamps()
                        {
                            Start = client.CurrentPresence.Timestamps.Start
                        },
                        Assets = new Assets()
                        {
                            LargeImageText = largeimagetextI.Text,
                            SmallImageText = smallimagetextI.Text,
                            LargeImageKey = largeimagekeyI.Text.ToLower(),
                            SmallImageKey = smallimagekeyI.Text.ToLower()
                        },
                    });
                } else if(!checkBox1.Checked && enablePartyText)
                {
                    hours = 0; minutes = 0; seconds = -1;
                    client.SetPresence(new RichPresence()
                    {
                        State = stateI.Text,
                        Details = detailsI.Text,
                        Timestamps = new Timestamps()
                        {
                            Start = DateTime.UtcNow
                        },
                        Assets = new Assets()
                        {
                            LargeImageText = largeimagetextI.Text,
                            SmallImageText = smallimagetextI.Text,
                            LargeImageKey = largeimagekeyI.Text.ToLower(),
                            SmallImageKey = smallimagekeyI.Text.ToLower()
                        },
                        Party = new Party()
                        {
                            Size = validPartySize,
                            Max = validMaxPartySize,
                            ID = "CheezyRPCEditor"
                        }
                    });
                } else
                {
                    hours = 0; minutes = 0; seconds = -1;
                    client.SetPresence(new RichPresence()
                    {
                        State = stateI.Text,
                        Details = detailsI.Text,
                        Timestamps = new Timestamps()
                        {
                            Start = DateTime.UtcNow
                        },
                        Assets = new Assets()
                        {
                            LargeImageText = largeimagetextI.Text,
                            SmallImageText = smallimagetextI.Text,
                            LargeImageKey = largeimagekeyI.Text.ToLower(),
                            SmallImageKey = smallimagekeyI.Text.ToLower()
                        }
                    });
                }
            }
        }
    }
}
