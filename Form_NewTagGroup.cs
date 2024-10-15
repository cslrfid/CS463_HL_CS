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
    public partial class Form_NewTagGroup : Form
    {
        CS463_HL_API reader = new CS463_HL_API();

        public Form_NewTagGroup()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void Form_NewTagGroup_Load(object sender, EventArgs e)
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

        private void button_Add_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "Add Tag Group", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (reader.AddTagGroupID(textBox_TagGroupID.Text))
            {
                MessageBox.Show(String.Format("Add Tag Group OK.\n"), "Add TagGroup ID", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Cannot get Reader ID.\n ({0})", reader.error_msg), "Add TagGroup ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
