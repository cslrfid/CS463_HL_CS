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
    public partial class frmEvent : Form
    {
        System.Collections.ArrayList profileList;
        System.Collections.ArrayList triggerList;
        System.Collections.ArrayList actionList;
        System.Collections.ArrayList eventList;

        CS463_HL_API reader = new CS463_HL_API();

        public frmEvent()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void frmEvent_Load(object sender, EventArgs e)
        {
            this.btnRefresh.PerformClick();
        }

        private void loadUserSettings()
        {
            reader.login_name = (string)Application.UserAppDataRegistry.GetValue("LoginName", "root");
            reader.login_password = (string)Application.UserAppDataRegistry.GetValue("LoginPassword", "csl");
            reader.http_timeout = (int)Application.UserAppDataRegistry.GetValue("HttpTimeout", 30000);
            reader.setURI((string)Application.UserAppDataRegistry.GetValue("URI", "http://192.168.25.208/"));
            reader.api_log_level = reader.LogLevel((string)Application.UserAppDataRegistry.GetValue("LogLevel", "Info"));
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Get Event", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            profileList = reader.getOperProfile();
            cbProfile.Items.Clear();
            if (profileList != null)
            {
                foreach (OPERATION_PROFILE o in profileList)
                {
                    cbProfile.Items.Add(o.profile_id);
                }
            }

            triggerList = reader.listTriggeringLogic();
            cbTrigger.Items.Clear();
            cbEnabling.Items.Clear();
            cbDisabling.Items.Clear();
            cbEnabling.Items.Add("Always On");
            cbDisabling.Items.Add("Never Stop");
            if (triggerList != null)
            {
                foreach (TRIGGER_INFO t in triggerList)
                {
                    if (t.mode.Equals("Input Sensor State", StringComparison.OrdinalIgnoreCase))
                    {
                        cbTrigger.Items.Add(t.id);
                        cbEnabling.Items.Add(t.id);
                        cbDisabling.Items.Add(t.id);
                    }
                    else if (t.mode.Equals("No Tag Read in Specified Time Span", StringComparison.OrdinalIgnoreCase))
                    {
                        cbDisabling.Items.Add(t.id);
                    }
                    else if (t.mode.Equals("Read Any Tags (any ID, 1 trigger per tag)", StringComparison.OrdinalIgnoreCase))
                    {
                        cbTrigger.Items.Add(t.id);
                    }
                    else if (t.mode.Equals("No Tag Read during Inventory Enabling Cycle", StringComparison.OrdinalIgnoreCase))
                    {
                        cbTrigger.Items.Add(t.id);
                    }
                    else if (t.mode.Equals("Read Any Tags (any ID, 1 trigger on first tag of inventory enabling cycle)", StringComparison.OrdinalIgnoreCase))
                    {
                        cbTrigger.Items.Add(t.id);
                    }
                    else if (t.mode.Equals("Read Any Tags (any ID, 1 trigger at the end of inventory enabling cycle)", StringComparison.OrdinalIgnoreCase))
                    {
                        cbTrigger.Items.Add(t.id);
                    }
                    else if (t.mode.Equals("Read Any Tags", StringComparison.OrdinalIgnoreCase))
                    {
                        cbTrigger.Items.Add(t.id);
                    }
                }
            }

            actionList = reader.listResultantAction();
            cbAction.Items.Clear();
            cbAction.Items.Add("NONE");
            if (actionList != null)
            {
                foreach (RESULTANT_ACTION_INFO r in actionList)
                {
                    cbAction.Items.Add(r.action_id);
                }
            }

            eventList = reader.listEvent();

            cbID.Items.Clear();
            if (eventList == null)
            {
                cbID.Text = "";
                txtDesc.Text = "";
                cbProfile.Text = "";
                cbTrigger.Text = "";
                cbAction.Text = "";
                cbEnabling.SelectedIndex = 0;
                cbDisabling.SelectedIndex = 0;
                chkEnable.Checked = false;
                chkLog.Checked = false;
                return;
            }

            foreach (EVENT_INFO ei in eventList)
            {
                cbID.Items.Add(ei.event_id);
            }

            EVENT_INFO info = (EVENT_INFO)eventList[0];
            cbID.Text = info.event_id;
            txtDesc.Text = info.desc;
            cbProfile.Text = info.operProfile_id;
            cbTrigger.Text = info.triggering_logic;
            cbAction.Text = info.resultant_action;
            chkEnable.Checked = info.enable;
            chkLog.Checked = info.event_log;
            cbEnabling.Text = info.inventoryEnablingTrigger;
            cbDisabling.Text = info.inventoryDisablingTrigger;

            reader.logout();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (cbID.Text == "")
            {
                MessageBox.Show("Event ID cannot be empty.", "Set Event", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reader.connect() == false)
            {
                MessageBox.Show("Cannot connect to reader.", "Set Event", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            EVENT_INFO info = new EVENT_INFO();
            info.event_id = cbID.Text;
            info.desc = txtDesc.Text;
            info.operProfile_id = cbProfile.Text;
            info.triggering_logic = cbTrigger.Text;
            info.resultant_action = cbAction.Text;
            info.event_log = chkLog.Checked;
            info.enable = chkEnable.Checked;
            info.inventoryEnablingTrigger = cbEnabling.Text;
            info.inventoryDisablingTrigger = cbDisabling.Text;

            if (cbID.SelectedIndex > -1)
            {
                if (reader.modEvent(info))
                {
                    MessageBox.Show("Event Modified", "Modify Event", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(String.Format("Fail to modify Event.\n ({0})", reader.error_msg), "Modify Event", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {

                if (reader.addEvent(info))
                {
                    MessageBox.Show("Event Updated", "Set Event", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(String.Format("Fail to update Event.\n ({0})", reader.error_msg), "Set Event", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            this.btnRefresh.PerformClick();
        }

        private void cbID_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int index = cbID.SelectedIndex;

            if (index < 0)
                return;

            EVENT_INFO info = (EVENT_INFO)eventList[index];
            cbID.Text = info.event_id;
            txtDesc.Text = info.desc;
            cbProfile.Text = info.operProfile_id;
            cbTrigger.Text = info.triggering_logic;
            cbAction.Text = info.resultant_action;
            chkEnable.Checked = info.enable;
            chkLog.Checked = info.event_log;
            cbEnabling.Text = info.inventoryEnablingTrigger;
            cbDisabling.Text = info.inventoryDisablingTrigger;

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (cbID.SelectedIndex == -1)
                return;

            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Cannot connect to reader.\n ({0})", reader.error_msg), "Remove Event", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reader.delEvent(cbID.Text))
            {
                MessageBox.Show("Event Removed.", "Remove Event", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(String.Format("Cannot remove Event.\n ({0})", reader.error_msg), "Remove Event", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            this.btnRefresh.PerformClick();

        }

    }
}