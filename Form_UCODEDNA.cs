using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSL;

namespace CS463_HL_CS
{
    public partial class Form_UCODEDNA : Form
    {
        CS463_HL_API reader = new CS463_HL_API();

        public Form_UCODEDNA()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void Form_UCODEDNA_Load(object sender, EventArgs e)
        {

        }

        private void loadUserSettings()
        {
            reader.login_name = (string)Application.UserAppDataRegistry.GetValue("LoginName", "root");
            reader.login_password = (string)Application.UserAppDataRegistry.GetValue("LoginPassword", "csl");
            reader.http_timeout = (int)Application.UserAppDataRegistry.GetValue("HttpTimeout", 30000);
            reader.setURI((string)Application.UserAppDataRegistry.GetValue("URI", "http://192.168.25.208/"));
            reader.api_log_level = reader.LogLevel((string)Application.UserAppDataRegistry.GetValue("LogLevel", "Info"));
        }

        private void button_ReadKey0_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string key = reader.ucodeDNAReadKey(textBox_EPC.Text, 0);
            if (key != null)
            {
                textBox_Key0.Text = key;
                MessageBox.Show(String.Format("Read Key 0 OK.\n"), "UCODE DNA", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Read Key 0 Fail.\n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void button_ReadKey1_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string key = reader.ucodeDNAReadKey(textBox_EPC.Text, 1);
            if (key != null)
            {
                textBox_Key1.Text = key;
                MessageBox.Show(String.Format("Read Key 1 OK.\n"), "UCODE DNA", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Read Key 1 Fail.\n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void button_WriteKey0_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reader.ucodeDNAWriteKey(textBox_EPC.Text, 0, textBox_Key0.Text))
            {
                MessageBox.Show(String.Format("Write Key 0 OK.\n"), "UCODE DNA", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Write Key 0 Fail.\n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void button_WriteKey1_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reader.ucodeDNAWriteKey(textBox_EPC.Text, 1, textBox_Key1.Text))
            {
                MessageBox.Show(String.Format("Write Key 1 OK.\n"), "UCODE DNA", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Write Key 1 Fail.\n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void button_ActivateKey0_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reader.ucodeDNAActivateKey(textBox_EPC.Text, 0))
            {
                MessageBox.Show(String.Format("Activate Key 0 OK.\n"), "UCODE DNA", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Activate Key 0 Fail.\n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void button_ActivateKey1_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reader.ucodeDNAActivateKey(textBox_EPC.Text, 1))
            {
                MessageBox.Show(String.Format("Activate Key 1 OK.\n"), "UCODE DNA", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Activate Key 1 Fail.\n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void button_TAM1_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string response = reader.ucodeDNAAuthenticate(textBox_EPC.Text, "TAM1", textBox_Challenge.Text);
            if (response != null)
            {
                label_TagResponse.Text = response;
                MessageBox.Show(String.Format("Authenticate TAM1 OK.\n"), "UCODE DNA", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Authenticate TAM1 Fail.\n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void button_TAM2_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string response = reader.ucodeDNAAuthenticate(textBox_EPC.Text, "TAM2", textBox_Challenge.Text);
            if (response != null)
            {
                label_TagResponse.Text = response;
                MessageBox.Show(String.Format("Authenticate TAM2 OK.\n"), "UCODE DNA", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Authenticate TAM2 Fail.\n ({0})", reader.error_msg), "UCODE DNA", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }
    }
}
