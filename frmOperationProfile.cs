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
    public partial class frmOperationProfile : Form
    {
        CS463_HL_API reader = new CS463_HL_API();

        public frmOperationProfile()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void loadUserSettings()
        {
                reader.login_name = (string)Application.UserAppDataRegistry.GetValue("LoginName", "root");
                reader.login_password = (string)Application.UserAppDataRegistry.GetValue("LoginPassword", "csl");
                reader.http_timeout = (int)Application.UserAppDataRegistry.GetValue("HttpTimeout", 30000);
                reader.setURI((string)Application.UserAppDataRegistry.GetValue("URI", "http://192.168.25.208/"));
                reader.api_log_level = reader.LogLevel((string)Application.UserAppDataRegistry.GetValue("LogLevel", "Info"));
        }

        private void OperationProfile_Load(object sender, EventArgs e)
        {
            this.btnRefresh.PerformClick();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string[] sCapMode = new string[] {"Window Mode"};

            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Get Operation Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Collections.ArrayList list = reader.getOperProfile();
            if (list == null)
            {
                cbProfileID.Items.Clear();
                cbSession.SelectedIndex = 0;
                cbModProfile.SelectedIndex = 0;
                cbCapMode.SelectedIndex = 0;
                cbSession.SelectedIndex = 0;
                chkAnt1.Checked = false;
                chkAnt2.Checked = false;
                chkAnt3.Checked = false;
                chkAnt4.Checked = false;
                numPopEst.Value = 64;
                numTxPower.Value = 30;
                numWindowTime.Value = 5000;
                chkEnable.Checked = false;
                cbTrigger.SelectedIndex = 0;
                return;
            }

            //Add profile id to combobox.
            cbProfileID.Items.Clear();
            foreach (OPERATION_PROFILE profile in list)
            {
                cbProfileID.Items.Add(profile.profile_id);
            }
            
            OPERATION_PROFILE op = (OPERATION_PROFILE) list[0];

            cbProfileID.Text = op.profile_id;
            chkEnable.Checked = op.profile_enable;
            cbSession.SelectedIndex = op.session_no-1;
            cbModProfile.Text = op.modulation_profile;
            cbCapMode.Text = op.capture_mode;
            numPopEst.Value = op.population;
            numTxPower.Value = decimal.Parse(op.transmit_power);
            numWindowTime.Value = op.window_time;
            chkAnt1.Text = String.Format("Antenna1: {0}", op.ant1_name);
            chkAnt2.Text = String.Format("Antenna2: {0}", op.ant2_name);
            chkAnt3.Text = String.Format("Antenna3: {0}", op.ant3_name);
            chkAnt4.Text = String.Format("Antenna4: {0}", op.ant4_name);
            chkAnt1.Checked = op.ant1_enable;
            chkAnt2.Checked = op.ant2_enable;
            chkAnt3.Checked = op.ant3_enable;
            chkAnt4.Checked = op.ant4_enable;
            cbTrigger.Text = op.trigger;

            reader.logout();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Get Operation Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OPERATION_PROFILE profile = new OPERATION_PROFILE();

            profile.profile_id = cbProfileID.Text;
            profile.profile_enable = chkEnable.Checked;
            profile.modulation_profile = cbModProfile.Text;
            profile.population = (int) numPopEst.Value;
            profile.session_no = cbSession.SelectedIndex+1;
            profile.transmit_power = numTxPower.Value.ToString();
            profile.window_time = (int) numWindowTime.Value;
            profile.capture_mode = cbCapMode.Text;
            profile.ant1_enable = chkAnt1.Checked;
            profile.ant2_enable = chkAnt2.Checked;
            profile.ant3_enable = chkAnt3.Checked;
            profile.ant4_enable = chkAnt4.Checked;
            profile.trigger = cbTrigger.Text;

            if (reader.setOperProfile(profile))
            {
                this.btnRefresh.PerformClick();
                MessageBox.Show("Profile updated.", "Set Operation Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(String.Format("Cannot update profile.\n ({0})", reader.error_msg), "Set Operation Profile", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.btnRefresh.PerformClick();
        }

        private void numTxPower_ValueChanged(object sender, EventArgs e)
        {
            double value = (double) numTxPower.Value;

            double i = (double)((int)value);
            double d = value - (double)i;

            if ((d == 0) || (d==0.25) || (d==0.5) || (d==0.75))
            {
                return;
            }
            else if (d <= 0.25)
            {
                value = i + 0.25;
            }
            else if (d <= 0.5)
            {
                value = i + 0.5;
            }
            else if (d <= 0.75)
            {
                value = i + 0.75;
            }
            else
            {
                value = i + 1.0;
            }
            numTxPower.Value = (decimal)value;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbTrigger_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTrigger.SelectedIndex == 0)
            {
                numWindowTime.Enabled = true;
            }
            else
            {
                numWindowTime.Enabled = false;
            }
        }

    }
}