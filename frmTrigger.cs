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
    public partial class frmTrigger : Form
    {
        System.Collections.ArrayList list;

        CS463_HL_API reader = new CS463_HL_API();

        public frmTrigger()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void frmTrigger_Load(object sender, EventArgs e)
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
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Get Trigger Logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            list = reader.listTriggeringLogic();

            cbID.Items.Clear();
            if (list == null)
            {
                cbID.Text = "";
                txtDesc.Text = "";
                cbMode.SelectedIndex = 0;
                chkAnt1.Checked = false;
                chkAnt2.Checked = false;
                chkAnt3.Checked = false;
                chkAnt4.Checked = false;
            }

            if (list == null)
            {
                reader.logout();
                return;
            }

            foreach (TRIGGER_INFO i in list)
            {
                cbID.Items.Add(i.id);
            }

            TRIGGER_INFO info = (TRIGGER_INFO) list[0];

            cbID.Text = info.id;
            txtDesc.Text = info.desc;
            cbMode.Text = info.mode;

            if (cbMode.SelectedIndex != 1)
            {
                if (info.capture_point.Contains("1"))
                    chkAnt1.Checked = true;
                else
                    chkAnt1.Checked = false;

                if (info.capture_point.Contains("2"))
                    chkAnt2.Checked = true;
                else
                    chkAnt2.Checked = false;

                if (info.capture_point.Contains("3"))
                    chkAnt3.Checked = true;
                else
                    chkAnt3.Checked = false;

                if (info.capture_point.Contains("4"))
                    chkAnt4.Checked = true;
                else
                    chkAnt4.Checked = false;
            }

            if (cbMode.SelectedIndex == 1)
            {
                try
                {
                    string temp = null;

                    if (info.logic.StartsWith("sensor", StringComparison.OrdinalIgnoreCase))
                    {
                        temp = info.logic.Substring(8, 1);
                        cbLevel.SelectedIndex = int.Parse(temp);
                        temp = info.logic.Substring(6, 1);
                        cbSensor.SelectedIndex = int.Parse(temp) - 1;
                    }
                }
                catch
                {
                    cbSensor.SelectedIndex = -1;
                    cbLevel.SelectedIndex = -1;
                }
            }
            
            if (cbMode.SelectedIndex == 2)
            {
                try
                {
                    numTimeout.Value = decimal.Parse(info.logic);
                }
                catch
                {
                    numTimeout.Value = 30;
                }
            }

            reader.logout();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbID.Text == "")
                return;

            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Set Trigger Logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            //if (cbID.SelectedIndex > -1)
            //{
            //    reader.delTriggeringLogic(cbID.Text);
            //}

            TRIGGER_INFO info = new TRIGGER_INFO();
            info.id = cbID.Text;
            info.desc = txtDesc.Text;
            info.mode = cbMode.Text;

            info.capture_point = "";
            info.logic = "";
            if (cbMode.SelectedIndex != 1)
            {
                if (chkAnt1.Checked)
                    info.capture_point += "1";
                if (chkAnt2.Checked)
                    info.capture_point += "2";
                if (chkAnt3.Checked)
                    info.capture_point += "3";
                if (chkAnt4.Checked)
                    info.capture_point += "4";
            }

            if (cbMode.SelectedIndex == 1)
            {
                info.logic = String.Format("Sensor{0}:{1}", cbSensor.SelectedIndex + 1, cbLevel.SelectedIndex);
            }
            
            if (cbMode.SelectedIndex == 2)
            {
                info.logic = numTimeout.Value.ToString();
            }


            if (cbID.SelectedIndex > -1)
            {
                if (reader.modTriggeringLogic(info))
                {
                    MessageBox.Show("Trigger Modified.", "Modify Trigger Logic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(String.Format("Fail to modify Trigger.\n ({0})", reader.error_msg), "Modify Trigger Logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {

                if (reader.addTriggeringLogic(info))
                {
                    MessageBox.Show("Trigger Updated.", "Set Trigger Logic", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(String.Format("Fail to update Trigger.\n ({0})", reader.error_msg), "Set Trigger Logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.btnRefresh.PerformClick();
        }

        private void cbID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int index = cbID.SelectedIndex;

            if (index < 0)
                return;

            TRIGGER_INFO info = (TRIGGER_INFO)list[index];
            cbID.Text = info.id;
            txtDesc.Text = info.desc;
            cbMode.Text = info.mode;

            if (cbMode.SelectedIndex != 1)
            {
                if (info.capture_point.Contains("1"))
                    chkAnt1.Checked = true;
                else
                    chkAnt1.Checked = false;

                if (info.capture_point.Contains("2"))
                    chkAnt2.Checked = true;
                else
                    chkAnt2.Checked = false;

                if (info.capture_point.Contains("3"))
                    chkAnt3.Checked = true;
                else
                    chkAnt3.Checked = false;

                if (info.capture_point.Contains("4"))
                    chkAnt4.Checked = true;
                else
                    chkAnt4.Checked = false;
            }
            else if (cbMode.SelectedIndex == 1)
            {
                try
                {
                    string temp = null;

                    if (info.logic.StartsWith("sensor", StringComparison.OrdinalIgnoreCase))
                    {
                        temp = info.logic.Substring(8, 1);
                        cbLevel.SelectedIndex = int.Parse(temp);
                        temp = info.logic.Substring(6, 1);
                        cbSensor.SelectedIndex = int.Parse(temp) - 1;
                    }
                }
                catch
                {
                    cbSensor.SelectedIndex = -1;
                    cbLevel.SelectedIndex = -1;
                }
            }
            else if (cbMode.SelectedIndex == 2)
            {
                try
                {
                    numTimeout.Value = decimal.Parse(info.logic);
                }
                catch
                {
                    numTimeout.Value = 30;
                }
            }

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (cbID.SelectedIndex == -1)
                return;

            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Remove Trigger Logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reader.delTriggeringLogic(cbID.Text))
            {
                MessageBox.Show("Trigger Removed.", "Remove Trigger Logic", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(String.Format("Cannot remove Trigger.\n ({0})", reader.error_msg), "Remove Trigger Logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.btnRefresh.PerformClick();
        }

        private void cbMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbMode.SelectedIndex != 1)
            {
                gAntenna.Enabled = true;
                gSensor.Enabled = false;
                gTimeout.Enabled = false;
            }
            
            if (cbMode.SelectedIndex == 1)
            {
                gAntenna.Enabled = false;
                gSensor.Enabled = true;
                gTimeout.Enabled = false;
            }
            
            if (cbMode.SelectedIndex == 2)
            {
                gAntenna.Enabled = true;
                gSensor.Enabled = false;
                gTimeout.Enabled = true;
            }
        }

    }
}