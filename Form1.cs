using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;


namespace Props
{
    public partial class Form1 : Form
    {
        CServer server = new CServer();
        delegate void SetConnStatusDel();
        delegate void SetErrorMesageDel(string message);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread connectionThread = new Thread(new ThreadStart(connectThread));
            connectionThread.Start();
        }

        private void SetConnectionStatus()
        {
            if(statusStrip1.InvokeRequired)
            {
                SetConnStatusDel c = new SetConnStatusDel(SetConnectionStatus);
                Invoke(c, null);

            }
            else
            {
                switch(server.connectionStatus)
                {
                    case ConnectionStatus.Connected:
                        toolStripStatusLabel2.Text = "Connected";
                        toolStripStatusLabel2.ForeColor = Color.Green;

                        foreach(Control c in Controls)
                        {
                            c.Enabled = true;
                        }
                        break;

                    case ConnectionStatus.Connecting:
                        toolStripStatusLabel2.Text = "Connecting...";
                        toolStripStatusLabel2.ForeColor = Color.Orange;

                        foreach (Control c in Controls)
                        {
                            if (c.Equals(statusStrip1)) continue;
                            c.Enabled = false;
                        }
                        break;

                    case ConnectionStatus.Disconnected:
                        toolStripStatusLabel2.Text = "Disconnected";
                        toolStripStatusLabel2.ForeColor = Color.Red;

                        foreach (Control c in Controls)
                        {
                            if (c.Equals(statusStrip1)) continue;
                            c.Enabled = false;
                        }
                        break;

                    case ConnectionStatus.Error:
                        toolStripStatusLabel2.Text = "Error:";
                        toolStripStatusLabel2.ForeColor = Color.Red;

                        foreach (Control c in Controls)
                        {
                            if (c.Equals(statusStrip1)) continue;
                            c.Enabled = false;
                        }
                        break;
                }
            }
        }

        public void SetErrorText(string message)
        {
            if (statusStrip1.InvokeRequired)
            {
                SetErrorMesageDel c = new SetErrorMesageDel(SetErrorText);
                Invoke(c, message);

            }
            else
            {
                errorLabel.Text = message;
            }
        }

        private void connectThread()
        {
            server.connectionStatus = ConnectionStatus.Connecting;
            SetConnectionStatus();
            server.InitalizeConnection();
            SetConnectionStatus();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if(textBox2.Text.Length < 1)
            {
                MessageBox.Show("You must specify a Map", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            { 
                List<Prop> props = server.GetPropsByMap(textBox2.Text);                
                dataGridView1.DataSource = props.ToArray();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Prop p = (Prop)dataGridView1.CurrentRow.DataBoundItem;
            server.DeleteProp(p);
        }
    }
}
