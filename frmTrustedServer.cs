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
    public partial class frmTrustedServer : Form
    {
        System.Collections.ArrayList list;

        CS463_HL_API reader = new CS463_HL_API();

        public frmTrustedServer()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void loadUserSettings()
        {
            lock (this)
            {
                reader.login_name = (string)Application.UserAppDataRegistry.GetValue("LoginName", "root");
                reader.login_password = (string)Application.UserAppDataRegistry.GetValue("LoginPassword", "csl");
                reader.http_timeout = (int)Application.UserAppDataRegistry.GetValue("HttpTimeout", 30000);
                reader.setURI((string)Application.UserAppDataRegistry.GetValue("URI", "http://192.168.25.208/"));
                reader.api_log_level = reader.LogLevel((string)Application.UserAppDataRegistry.GetValue("LogLevel", "Info"));
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmTrustedServer_Load(object sender, EventArgs e)
        {
            this.btnRefresh.PerformClick();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Get Trusted Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            list = reader.listServer();

            cbServerID.Items.Clear();
            if (list == null)
            {
                txtDesc.Text = "";
                txtIP.Text = "";
                txtPort.Text = "";
                cbMode.Text = "";
                return;
            }

            foreach (SERVER_INFO si in list)
            {
                cbServerID.Items.Add(si.id);
            }

            SERVER_INFO svr = (SERVER_INFO)list[0];
            cbServerID.Text = svr.id;
            txtDesc.Text = svr.desc;
            txtIP.Text = svr.ip;
            cbMode.Text = svr.mode;
            if (svr.mode.Equals("Listening Port on Reader Side"))
            {
                txtPort.Text = svr.reader_port;
            }
            else
            {
                txtPort.Text = svr.server_port;
            }


            reader.logout();
        }

        private void cbServerID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int index = cbServerID.SelectedIndex;

            if (index < 0)
                return;

            SERVER_INFO svr = (SERVER_INFO)list[index];

            txtDesc.Text = svr.desc;
            txtIP.Text = svr.ip;
            cbMode.Text = svr.mode;
            if (cbMode.Text.Equals("Listening Port on Reader Side"))
            {
                txtPort.Text = svr.reader_port;
            }
            else
            {
                txtPort.Text = svr.server_port;
            }

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Get Trusted Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SERVER_INFO svr = new SERVER_INFO();
            svr.id = cbServerID.Text;
            svr.desc = txtDesc.Text;
            svr.ip = txtIP.Text;
            svr.mode = cbMode.Text;
            if (cbMode.Text.Equals("Listening Port on Reader Side"))
            {
                svr.server_port = "";
                svr.reader_port = txtPort.Text;
            }
            else
            {
                svr.server_port = txtPort.Text;
                svr.reader_port = "";
            }

            if (reader.setServerID(svr) == false)
            {
                if (reader.modServerID(svr) == false)
                {
                    MessageBox.Show(String.Format("Fail to update Trusted Server.\n ({0})", reader.error_msg), "Set Trusted Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Trusted Server Updated", "Set Trusted Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Trusted Server Updated", "Set Trusted Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.btnRefresh.PerformClick();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (cbServerID.SelectedIndex > -1)
            {
                if (reader.connect() == false)
                {
                    MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Get Trusted Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (reader.delServerID(cbServerID.Text))
                    MessageBox.Show("Server Removed.", "Remove Trusted Server", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(String.Format("Fail to remove server.\n ({0})", reader.error_msg), "Remove Trusted Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.btnRefresh.PerformClick();
            }
        }

    }
}