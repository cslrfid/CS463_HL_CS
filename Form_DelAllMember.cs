using CSL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CS463_HL_CS
{
    public partial class Form_DelAllMember : Form
    {
        CS463_HL_API reader = new CS463_HL_API();

        public Form_DelAllMember()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void Form_DelAllMember_Load(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "Add Tag Group Member", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            System.Collections.ArrayList tagGroups = reader.listTagGroup();

            if (tagGroups.Count == 0)
            {
                MessageBox.Show(String.Format("No Tag Group ID exists. \n ({0})", reader.error_msg), "Add Tag Group Member", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                for (int i = 0; i < tagGroups.Count; i++)
                    comboBox_tagGroups.Items.Add(tagGroups[i]);
                comboBox_tagGroups.SelectedIndex = 0;
            }

            reader.logout();
        }

        private void loadUserSettings()
        {
            reader.login_name = (string)Application.UserAppDataRegistry.GetValue("LoginName", "root");
            reader.login_password = (string)Application.UserAppDataRegistry.GetValue("LoginPassword", "csl");
            reader.http_timeout = (int)Application.UserAppDataRegistry.GetValue("HttpTimeout", 30000);
            reader.setURI((string)Application.UserAppDataRegistry.GetValue("URI", "http://192.168.25.208/"));
            reader.api_log_level = reader.LogLevel((string)Application.UserAppDataRegistry.GetValue("LogLevel", "Info"));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "Delete Tag Group All Member", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reader.DelAllMembers(comboBox_tagGroups.Text))
            {
                MessageBox.Show(String.Format("Delet Tag Group All Member OK.\n"), "Delete Tag Group All Member", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Cannot Delet Tag Group All Member.\n ({0})", reader.error_msg), "Delete Tag Group All Member", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }
    }
}