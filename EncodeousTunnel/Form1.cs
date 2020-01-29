using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EncodeousTunnel
{
    public partial class Form1 : Form
    {
        private ChiselTunnel ct = null;
        private CloudflareManager cm = null;
        private ChiselConnector cc = null;
        public static Form instance;
        public Form1()
        {
            instance = this;
            InitializeComponent();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            ct?.Stop();
            cm?.Stop();
            cc?.Stop();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Client
            this.label2.Text = "Status: Connecting...";
            this.button2.Enabled = false;
            this.textBox1.Enabled = false;
            this.address.ReadOnly = true;
            this.button1.Enabled = false;
            if (address.Text == "" || textBox1.Text == "")
            {
                MessageBox.Show("Please specify a Tunnel ID and/or ports to connect to!");
                this.button2.Enabled = true;
                this.textBox1.Enabled = true;
                this.address.ReadOnly = false;
                this.button1.Enabled = true;
                button4.Enabled = false;
                button3.Enabled = false;
                StatusUpdate.SetStatus("Status: Offline.");
                return;
            }
            cc = new ChiselConnector();
            cc.Start(address.Text,textBox1.Text);
            button3.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Host
            this.label2.Text = "Status: Starting Server...";
            this.button2.Enabled = false;
            this.textBox1.Enabled = false;
            this.address.ReadOnly = true;
            this.address.Text = "";
            this.textBox1.Text = "";
            this.button1.Enabled = false;
            button4.Enabled = true;
            ct = new ChiselTunnel();
            ct.Start();
            cm = new CloudflareManager();
            cm.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            StatusUpdate.DebugConsoleUpdater = DebugConsoleUpdater;
            StatusUpdate.StatusUpdater = StatusUpdater;
            StatusUpdate.Error = Error;
            StatusUpdate.Address = Address;
        }

        private void Address(string line)
        {
            address.Text = line;
        }

        private void Error(string line)
        {
            StatusUpdate.SetStatus("Status: Offline.");
            this.button2.Enabled = true;
            this.textBox1.Enabled = true;
            this.address.ReadOnly = false;
            this.button1.Enabled = true;
            button4.Enabled = false;
            button3.Enabled = false;
            ct?.Stop();
            cm?.Stop();
            cc?.Stop();
        }

        private void StatusUpdater(string line)
        {
            label2.Text = line;
        }

        private void DebugConsoleUpdater(string line)
        {
            richTextBox1.Text += line;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            cc.Stop();
            StatusUpdate.SetStatus("Status: Offline.");
            this.button2.Enabled = true;
            this.textBox1.Enabled = true;
            this.address.ReadOnly = false;
            this.button1.Enabled = true;
            button4.Enabled = false;
            button3.Enabled = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            cm.Stop();
            ct.Stop();
            StatusUpdate.SetStatus("Status: Server Stopped!");
            this.button2.Enabled = true;
            this.textBox1.Enabled = true;
            this.address.ReadOnly = false;
            this.address.Text = "";
            this.textBox1.Text = "";
            this.button1.Enabled = true;
            button4.Enabled = false;
            button3.Enabled = false;
        }
    }
}
