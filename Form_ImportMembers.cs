﻿using CSL;
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
    public partial class Form_ImportMembers : Form
    {
        CS463_HL_API reader = new CS463_HL_API();

        public Form_ImportMembers()
        {
            InitializeComponent();
            loadUserSettings();
        }

        private void Form_ImportMembers_Load(object sender, EventArgs e)
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

        private void button_AddAll_Click(object sender, EventArgs e)
        {
            if (reader.connect() == false)
            {
                MessageBox.Show(String.Format("Connection failed. \n ({0})", reader.error_msg), "Add Tag Group", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> epcidlist = new List<string>(textBox_EPCID.Text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None));

            if (reader.ImportTagGroupMembers(comboBox_tagGroups.Text, epcidlist))
            {
                MessageBox.Show(String.Format("Add Tag Group Member OK.\n"), "Add Tag Group Member", MessageBoxButtons.OK);
            }
            else
            {
                MessageBox.Show(String.Format("Cannot Add Tag Group Member.\n ({0})", reader.error_msg), "Add Tag Group Member", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            reader.logout();
        }

        private void button_Close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}