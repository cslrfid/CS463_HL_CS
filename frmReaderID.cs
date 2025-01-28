using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using CSL;

namespace CS463_HL_CS
{
    public partial class frmReaderID : Form
    {
        CS463_HL_API reader = new CS463_HL_API();

        public frmReaderID()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void frmReaderID_Load(object sender, EventArgs e)
        {
            this.btnGet.PerformClick();
        }

        private void loadUserSettings()
        {
                reader.login_name = (string)Application.UserAppDataRegistry.GetValue("LoginName", "root");
                reader.login_password = (string)Application.UserAppDataRegistry.GetValue("LoginPassword", "csl");
                reader.http_timeout = (int)Application.UserAppDataRegistry.GetValue("HttpTimeout", 30000);
                reader.setURI((string)Application.UserAppDataRegistry.GetValue("URI", "http://192.168.25.208/"));
                reader.api_log_level = reader.LogLevel((string)Application.UserAppDataRegistry.GetValue("LogLevel", "Info"));
        }

        private void btnGet_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "Get Reader ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            READER_ID rdr = new READER_ID();

            if (reader.getReaderID(out rdr))
            {
                txtReaderID.Text = rdr.id;
                txtDesc.Text = rdr.desc;
            }
            else
            {
                MessageBox.Show(String.Format("Cannot get Reader ID.\n ({0})", reader.error_msg), "Get Reader ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void btnSetReaderID_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed.\n ({0})", reader.error_msg), "Set Reader ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            READER_ID rdr = new READER_ID();
            rdr.desc = txtDesc.Text;
            rdr.id = txtReaderID.Text;

            if (reader.setReaderID(rdr))
            {
                MessageBox.Show("Reader ID Updated.", "Set Reader ID", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(String.Format("Cannot update Reader ID.\n ({0})", reader.error_msg), "Set Reader ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.btnGet.PerformClick();
        }

    }
}