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
    public partial class frmResultantAction : Form
    {
        System.Collections.ArrayList list;

        CS463_HL_API reader = new CS463_HL_API();

        public frmResultantAction()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void frmResultantAction_Load(object sender, EventArgs e)
        {
            this.btnRefresh.PerformClick();
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

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Get Resultant Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Collections.ArrayList svrList = reader.listServer();
            cbServerID.Items.Clear();
            if (svrList != null)
            {
                foreach (SERVER_INFO s in svrList)
                {
                    cbServerID.Items.Add(s.id);
                }
            }

            list = reader.listResultantAction();

            cbID.Items.Clear();
            if (list == null)
            {
                cbID.Text = "";
                txtDesc.Text = "";
                cbMode.Text = "";
                cbReportID.Text = "";
                cbServerID.Text = "";
                return;
            }

            foreach (RESULTANT_ACTION_INFO r in list)
            {
                cbID.Items.Add(r.action_id);
            }

            RESULTANT_ACTION_INFO info = (RESULTANT_ACTION_INFO)list[0];

            cbID.Text = info.action_id;
            txtDesc.Text = info.desc;
            cbMode.Text = info.action_mode;
            cbServerID.Text = info.server_id;
            cbReportID.Text = info.report_id;

            reader.logout();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbID.Text == "")
                return;

            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Set Resultant Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RESULTANT_ACTION_INFO info = new RESULTANT_ACTION_INFO();
            info.action_id = cbID.Text;
            info.desc = txtDesc.Text;
            info.action_mode = cbMode.Text;
            info.server_id = cbServerID.Text;
            info.report_id = cbReportID.Text;

            if (cbID.SelectedIndex > -1)
            {
                if (reader.modResultantAction(info))
                {
                    MessageBox.Show("Resultant Action Modified.", "Modify Resultant Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(String.Format("Cannot Modify Resultant Action.\n ({0})", reader.error_msg), "Modify Resultant Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {

                if (reader.addResultantAction(info))
                {
                    MessageBox.Show("Resultant Action Updated.", "Set Resultant Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(String.Format("Cannot update Resultant Action.\n ({0})", reader.error_msg), "Set Resultant Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.btnRefresh.PerformClick();
        }

        private void cbID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int index = cbID.SelectedIndex;

            if (index < 0)
                return;

            RESULTANT_ACTION_INFO info = (RESULTANT_ACTION_INFO)list[index];

            cbID.Text = info.action_id;
            txtDesc.Text = info.desc;
            cbMode.Text = info.action_mode;
            cbServerID.Text = info.server_id;
            cbReportID.Text = info.report_id;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (cbID.SelectedIndex > -1)
            {
                if (reader.connect() == false)
                {
                    MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Remove Resultant Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (reader.delResultantAction(cbID.Text))
                    MessageBox.Show("Resultant Action Removed.", "Remove Resultant Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show(String.Format("Fail to Resultant Action.\n ({0})", reader.error_msg), "Remove Resultant Action", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.btnRefresh.PerformClick();
            }

            this.btnRefresh.PerformClick();
        }

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMode.Text.Equals("Do Nothing (Only Show on Screen)", StringComparison.OrdinalIgnoreCase))
            {
                cbServerID.Enabled = false;
                cbReportID.Enabled = false;
            }
            else if (cbMode.Text.Equals("Batch Alert to Server", StringComparison.OrdinalIgnoreCase) ||
                cbMode.Text.Equals("Instant Alert to Server", StringComparison.OrdinalIgnoreCase))
            {
                cbServerID.Enabled = true;
                cbReportID.Enabled = true;
            }
        }

    }
}