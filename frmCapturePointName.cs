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
    public partial class frmCapturePointName : Form
    {
        CS463_HL_API reader = new CS463_HL_API();

        public frmCapturePointName()
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

        private void frmCapturePointName_Load(object sender, EventArgs e)
        {
            this.btnRefresh.PerformClick();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Get Capture Point Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string[] name = reader.getCapturePointName();

            txtAnt1.Text = name[0];
            txtAnt2.Text = name[1];
            txtAnt3.Text = name[2];
            txtAnt4.Text = name[3];

            reader.logout();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Set Capture Point Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bool status = true;

            status &= reader.setCapturePointName("Antenna1", txtAnt1.Text);
            status &= reader.setCapturePointName("Antenna2", txtAnt2.Text);
            status &= reader.setCapturePointName("Antenna3", txtAnt3.Text);
            status &= reader.setCapturePointName("Antenna4", txtAnt4.Text);
            if (status == false)
                MessageBox.Show(String.Format("Fail to update capture point name.\n ({0})", reader.error_msg), "Set Capture Point Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                MessageBox.Show("Capture Point Name updated.", "Set Capture Point Name", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.btnRefresh.PerformClick();
        }
    }
}