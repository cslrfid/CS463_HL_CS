using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;

using CSL;

namespace CS463_HL_CS
{
    public partial class frmMain : Form
    {
        CS463_HL_API reader1 = new CS463_HL_API();
        TrustedServer server = new TrustedServer();

        System.Collections.ArrayList InstantTags = new System.Collections.ArrayList();

        DataTable tagsTable = new DataTable("Tags");
        Int32 totalTags = 0;
        Int32 countDown = 0;

        public frmMain()
        {
            reader1.api_log_level = LOG_LEVEL.Info;
            reader1.saveToLogInfo(String.Format("CS463 Demo Program ({0}) starting up ...", Application.ProductVersion));
            reader1.api_log_level = LOG_LEVEL.Disabled;

            InitializeComponent();
            this.Text = String.Format("CS463 Demo (High Level API) - {0}", Application.ProductVersion);
            tmrClock.Enabled = true;
            tsslClock.Text = String.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());

            loadUserSettings();

            //Create data table
            DataColumn col = new DataColumn();
            tagsTable.Columns.Add("ROW ID", typeof(UInt32));
            tagsTable.Columns.Add("Tag ID", typeof(String));
            tagsTable.Columns.Add("Capture Point", typeof(String));
            tagsTable.Columns.Add("Event", typeof(String));
            tagsTable.Columns.Add("RSSI", typeof(String));
            tagsTable.Columns.Add("Time", typeof(String));
            tagsTable.Columns.Add("Reader IP", typeof(String));
            tagsTable.Columns.Add("TimeStamp (UTC)", typeof(DateTime));
            tagsTable.Columns.Add("Frequency", typeof(String));
            tagsTable.Columns.Add("Session", typeof(String));
            tagsTable.Columns.Add("Index", typeof(String));
            tagsTable.Columns.Add("Antenna", typeof(String));
            tagsTable.Columns.Add("Count", typeof(UInt32));
            tagsTable.Columns.Add("Bank0", typeof(String));
            tagsTable.Columns.Add("Bank2", typeof(String));
            tagsTable.Columns.Add("Bank3", typeof(String));
            tagsTable.Columns.Add("PC", typeof(String));

            //            tagsTable.PrimaryKey = new DataColumn[] { tagsTable.Columns["Tag ID"], tagsTable.Columns["Capture Point"], tagsTable.Columns["Event"] };
            tagsTable.PrimaryKey = new DataColumn[] { tagsTable.Columns["Tag ID"] };

            dgvTags.DataSource = tagsTable;

            dgvTags.Columns[0].Width = 100;
            dgvTags.Columns[1].Width = 170;
            dgvTags.Columns[2].Width = 140;
            dgvTags.Columns[3].Width = 60;
            dgvTags.Columns[4].Width = 60;
            dgvTags.Columns[5].Width = 60;
            dgvTags.Columns[6].Width = 80;
            dgvTags.Columns[7].Width = 120;

            //Setup event handler
            server.BatchEnd += new BatchEndEventHandler(this.reader1_BatchEnd);
            server.AntennaEvent += new AntennaEventHandler(this.reader1_AntennaEvent);
            server.BufferOverflowEvent += new BufferOverflowEventHandler(this.reader1_BufferOverflowEvent);
            server.TagReceiveEvent += new TagReceiveEventHandler(this.reader1_TagReceiveEvent);
            server.TagListEvent += new TagListEventHandler(this.reader1_TagListEvent);
            server.InventoryEvent += new InventoryEventHandler(this.reader1_InventoryEvent);

            updateLayout();

        }

        private void loadUserSettings()
        {
            reader1.login_name = (string)Application.UserAppDataRegistry.GetValue("LoginName", "root");
            reader1.login_password = (string)Application.UserAppDataRegistry.GetValue("LoginPassword", "csl");
            reader1.http_timeout = (int)Application.UserAppDataRegistry.GetValue("HttpTimeout", 30000);

            reader1.setURI((string)Application.UserAppDataRegistry.GetValue("URI", "http://192.168.25.208/"));
            txtURI.Text = reader1.getURI();
            reader1.api_log_level = reader1.LogLevel((string)Application.UserAppDataRegistry.GetValue("LogLevel", "Info"));
            chkAutoPollingEnable.Checked = ((int)Application.UserAppDataRegistry.GetValue("AutoPollingEnable", 0) == 1) ? true : false;
            numPollingTmr.Value = (int)Application.UserAppDataRegistry.GetValue("PollingPeriod", 1);
            cbLogLevel.Text = (string)Application.UserAppDataRegistry.GetValue("LogLevel", "Info");

            server.tcp_port = (int)Application.UserAppDataRegistry.GetValue("TcpPort", 9090);
            txtAlertPort.Text = server.tcp_port.ToString();
            txtPollingPort.Text = server.tcp_port.ToString();
            server.api_log_level = reader1.api_log_level;
        }

        private void tmrClock_Tick(object sender, EventArgs e)
        {
            tsslClock.Text = String.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmrClock.Enabled = false;
        }

        private void btnReadTags_Click(object sender, EventArgs e)
        {
            if (reader1.connect() == false)
            {
                MessageBox.Show("Cannot connect to reader");
                return;
            }

            string mb = "";
            mb += (cbBank0.Checked == true) ? "0" : "";
            mb += (cbBank1.Checked == true) ? "1" : "";
            mb += (cbBank2.Checked == true) ? "2" : "";
            mb += (cbBank3.Checked == true) ? "3" : "";

            if (mb == "")
            {
                MessageBox.Show("Please select at least one memory bank");
                return;
            }

            System.Collections.ArrayList tagslist = reader1.getTagDataAllBanks(mb, (ushort)numDuration.Value);

            update_dgvTagList(tagslist);

        }

        delegate void addTextTo_txtMsg_Delegate(string str);

        private void addTextTo_txtMsg(string str)
        {
            if (txtMsg.InvokeRequired)
            {
                addTextTo_txtMsg_Delegate task = new addTextTo_txtMsg_Delegate(addTextTo_txtMsg);
                BeginInvoke(task, new object[] { String.Format("Invoke: {0}", str) });
            }
            else
            {
                txtMsg.Text += String.Format("{0} | {1}", DateTime.Now.ToString(), str);
                txtMsg.Select(txtMsg.TextLength, 0);
                txtMsg.ScrollToCaret();
            }
        }

        delegate void addTextTo_tsddbAntenna_Delegate(string str);

        private void addTextTo_tsddbAntenna(string str)
        {
            if (InvokeRequired)
            {
                addTextTo_tsddbAntenna_Delegate task = new addTextTo_tsddbAntenna_Delegate(addTextTo_tsddbAntenna);
                BeginInvoke(task, new object[] { str });
            }
            else
            {
                tsddbAntenna.DropDownItems.Add(str);
                tsddbAntenna.Text = str;
            }
        }

        delegate void update_dgvTagList_Delegate(System.Collections.ArrayList list);

        private void update_dgvTagList(System.Collections.ArrayList list)
        {
            DataRow dr;

            if (list != null)
            {
                Monitor.Enter(list);
                {
                    tagsTable.BeginLoadData();
                    try
                    {
                        //Add new tags to tagList
                        foreach (TAG tag in list)
                        {
                            dr = tagsTable.Rows.Find(tag.TagOrigId);

                            totalTags++;
                            if (dr == null)
                            {
                                int oIndex = 0;
                                object[] o = new object[17];
                                o[oIndex++] = tagsTable.Rows.Count + 1;
                                o[oIndex++] = (tag.TagOrigId == null) ? " " : tag.TagOrigId;
                                o[oIndex++] = (tag.CapturePointId == null) ? "None" : tag.CapturePointId;
                                o[oIndex++] = (tag.EventId == null) ? "None" : tag.EventId;
                                o[oIndex++] = tag.RSSI.ToString("0.00");
                                o[oIndex++] = tag.Time.ToString("0"); ;
                                o[oIndex++] = tag.ServerIp;
                                o[oIndex++] = tag.ApiTimeStampUTC;
                                o[oIndex++] = tag.Frequency.ToString("0.00");
                                o[oIndex++] = tag.session;
                                o[oIndex++] = tag.Index;
                                o[oIndex++] = tag.Antenna;
                                o[oIndex++] = (tag.count == 0) ? 1 : tag.count;
                                o[oIndex++] = tag.Bank0;
                                o[oIndex++] = tag.Bank2;
                                o[oIndex++] = tag.Bank3;
                                o[oIndex++] = tag.PC;
                                tagsTable.LoadDataRow(o, LoadOption.OverwriteChanges);
                            }
                            else
                            {
                                dr.BeginEdit();
                                int oIndex = 1;
                                dr[oIndex++] = (tag.TagOrigId == null) ? " " : tag.TagOrigId;
                                dr[oIndex++] = (tag.CapturePointId == null) ? "None" : tag.CapturePointId;
                                dr[oIndex++] = (tag.EventId == null) ? "None" : tag.EventId;
                                dr[oIndex++] = tag.RSSI.ToString("0.00");
                                dr[oIndex++] = tag.Time.ToString("0"); ;
                                dr[oIndex++] = tag.ServerIp;
                                dr[oIndex++] = tag.ApiTimeStampUTC;
                                dr[oIndex++] = tag.Frequency.ToString("0.00");
                                dr[oIndex++] = tag.session;
                                dr[oIndex++] = tag.Index;
                                dr[oIndex++] = tag.Antenna;
                                {
                                    UInt32 cnt = (UInt32)dr[oIndex];
                                    dr[oIndex++] = (tag.count == 0) ? cnt + 1 : cnt + tag.count;
                                }
                                dr[oIndex++] = tag.Bank0;
                                dr[oIndex++] = tag.Bank2;
                                dr[oIndex++] = tag.Bank3;
                                dr[oIndex++] = tag.PC;
                                dr.EndEdit();

                            }

                        }
                    }
                    catch
                    { }

                    tagsTable.EndLoadData();
                    tsslCount.Text = String.Format("{0:d}/{1:d}", tagsTable.Rows.Count, totalTags);
                }
                Monitor.Exit(list);
            }
        }

        delegate void update_dgvTag_Delegate(object tag);

        private void update_dgvTag(object intag)
        {
            DataRow dr;

            if (intag != null)
            {
                Monitor.Enter(intag);
                {
                    TAG tag = (TAG)intag;
                    dr = tagsTable.Rows.Find(tag.TagOrigId);

                    try
                    {
                        totalTags++;
                        if (dr == null)
                        {
                            int oIndex = 0;
                            object[] o = new object[17];
                            o[oIndex++] = tagsTable.Rows.Count + 1;
                            o[oIndex++] = (tag.TagOrigId == null) ? " " : tag.TagOrigId;
                            o[oIndex++] = (tag.CapturePointId == null) ? "None" : tag.CapturePointId;
                            o[oIndex++] = (tag.EventId == null) ? "None" : tag.EventId;
                            o[oIndex++] = tag.RSSI.ToString("0.00");
                            o[oIndex++] = tag.Time.ToString("0"); ;
                            o[oIndex++] = tag.ServerIp;
                            o[oIndex++] = tag.ApiTimeStampUTC;
                            o[oIndex++] = tag.Frequency.ToString("0.00");
                            o[oIndex++] = tag.session;
                            o[oIndex++] = tag.Index;
                            o[oIndex++] = tag.Antenna;
                            o[oIndex++] = (tag.count == 0) ? 1 : tag.count;
                            o[oIndex++] = tag.Bank0;
                            o[oIndex++] = tag.Bank2;
                            o[oIndex++] = tag.Bank3;
                            o[oIndex++] = tag.PC;
                            tagsTable.LoadDataRow(o, LoadOption.OverwriteChanges);
                        }
                        else
                        {
                            dr.BeginEdit();
                            int oIndex = 1;
                            dr[oIndex++] = (tag.TagOrigId == null) ? " " : tag.TagOrigId;
                            dr[oIndex++] = (tag.CapturePointId == null) ? "None" : tag.CapturePointId;
                            dr[oIndex++] = (tag.EventId == null) ? "None" : tag.EventId;
                            dr[oIndex++] = tag.RSSI.ToString("0.00");
                            dr[oIndex++] = tag.Time.ToString("0"); ;
                            dr[oIndex++] = tag.ServerIp;
                            dr[oIndex++] = tag.ApiTimeStampUTC;
                            dr[oIndex++] = tag.Frequency.ToString("0.00");
                            dr[oIndex++] = tag.session;
                            dr[oIndex++] = tag.Index;
                            dr[oIndex++] = tag.Antenna;
                            {
                                UInt32 cnt = (UInt32)dr[oIndex];
                                dr[oIndex++] = (tag.count == 0) ? cnt + 1 : cnt + tag.count;
                            }
                            dr[oIndex++] = tag.Bank0;
                            dr[oIndex++] = tag.Bank2;
                            dr[oIndex++] = tag.Bank3;
                            dr[oIndex++] = tag.PC;
                            dr.EndEdit();

                        }
                    }
                    catch
                    { }

                    tagsTable.EndLoadData();
                    tsslCount.Text = String.Format("{0:d}/{1:d}", tagsTable.Rows.Count, totalTags);
                }
                Monitor.Exit(intag);
            }
        }


        private void btnClearTable_Click(object sender, EventArgs e)
        {
            tagsTable.Clear();
            DataColumn c = tagsTable.Columns[0];
            c.AutoIncrementSeed = 1;

            tsslCount.Text = "0/0";
            totalTags = 0;
            Application.DoEvents();
        }

        private void readerIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReaderID frm = new frmReaderID();
            frm.ShowDialog();
        }

        private void operationProfileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOperationProfile frm = new frmOperationProfile();
            frm.ShowDialog();
        }

        private void captureNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCapturePointName frm = new frmCapturePointName();
            frm.ShowDialog();
        }

        private void trustedServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTrustedServer frm = new frmTrustedServer();
            frm.ShowDialog();
        }

        private void triggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmTrigger frm = new frmTrigger();
            frm.ShowDialog();
        }

        private void resultantActionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmResultantAction frm = new frmResultantAction();
            frm.ShowDialog();
        }

        private void eventToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEvent frm = new frmEvent();
            frm.ShowDialog();
        }

        private void btnAlert_Click(object sender, EventArgs e)
        {
            if (btnAlert.Text.Equals("Start Read Alert", StringComparison.OrdinalIgnoreCase))
            {
                //start reading alerts
                btnAlert.Text = "Stop Read Alert";

                server.Start();
                tsslStatus.Text = String.Format("Start reading alert at port: {0} ...\r\n", server.tcp_port);
                addTextTo_txtMsg(tsslStatus.Text);
                tsslAlert.BackColor = Color.Green;
                Application.DoEvents();

                rbBatchAlert.Enabled = false;
                rbInstantAlert.Enabled = false;
                numRefresh.Enabled = false;

                btnPolling.Enabled = false;
                btnPollingReadTags.Enabled = false;
                txtPollingPort.Enabled = false;
                txtAlertPort.Enabled = false;
                numPollingTmr.Enabled = false;
                chkAutoPollingEnable.Enabled = false;
            }
            else
            {
                //stop reading alerts
                btnAlert.Text = "Start Read Alert";

                server.Stop();
                tsslStatus.Text = "Alert listener stopped.";
                addTextTo_txtMsg("Stop reading alert.\r\n");
                tsslAlert.BackColor = Color.Red;
                Application.DoEvents();

                rbBatchAlert.Enabled = true;
                rbInstantAlert.Enabled = true;
                if (rbInstantAlert.Checked == true)
                    numRefresh.Enabled = true;

                btnPolling.Enabled = true;
                btnPollingReadTags.Enabled = true;
                txtPollingPort.Enabled = true;
                txtAlertPort.Enabled = true;
                numPollingTmr.Enabled = true;
                chkAutoPollingEnable.Enabled = true;
            }
        }

        private void txtURI_TextChanged(object sender, EventArgs e)
        {
            try
            {
                Uri u = new Uri(txtURI.Text);
                reader1.setURI(txtURI.Text);
                txtURI.ForeColor = Color.Black;
                Application.UserAppDataRegistry.SetValue("URI", txtURI.Text, Microsoft.Win32.RegistryValueKind.String);
            }
            catch (UriFormatException)
            {
                txtURI.ForeColor = Color.Red;
            }
        }

        private void txtAlertPort_TextChanged(object sender, EventArgs e)
        {
            Int32 d;

            if (Int32.TryParse(txtAlertPort.Text, out d))
            {
                if ((d > -1) && (d < 65536))
                {
                    server.tcp_port = d;
                    txtAlertPort.ForeColor = Color.Black;
                    Application.UserAppDataRegistry.SetValue("TcpPort", d, Microsoft.Win32.RegistryValueKind.DWord);
                    if (txtPollingPort.Text != txtAlertPort.Text)
                        txtPollingPort.Text = txtAlertPort.Text;
                    return;
                }
            }
            txtAlertPort.ForeColor = Color.Red;
        }

        private void saveTagsToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog of = new SaveFileDialog();
            of.Filter = "*Comma Separated Values (*.CSV)|*.CSV";
            of.AddExtension = true;
            of.CheckFileExists = false;
            of.CheckPathExists = true;
            of.DefaultExt = "csv";
            of.CreatePrompt = true;
            of.OverwritePrompt = true;
            of.Title = "Please select which file to store the Tag information";
            if (of.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sw = new StreamWriter(of.FileName, false);
                Int32 h = tagsTable.Columns.Count;
                StringBuilder sb = new StringBuilder();

                for (int col = 0; col < tagsTable.Columns.Count; col++)
                {
                    sb.Append(String.Format("\"{0}\"", tagsTable.Columns[col].Caption));
                    if (col != (tagsTable.Columns.Count - 1))
                    {
                        sb.Append(", ");
                    }
                }
                sw.WriteLine(sb.ToString());

                foreach (DataRow row in tagsTable.Rows)
                {
                    sb = new StringBuilder();
                    for (int col = 0; col < tagsTable.Columns.Count; col++)
                    {
                        sb.Append(String.Format("\"{0}\"", row[col]));
                        if (col != (tagsTable.Columns.Count - 1))
                        {
                            sb.Append(", ");
                        }
                    }
                    sw.WriteLine(sb.ToString());
                }
                sw.Close();
            }
        }

        private void btnPolling_Click(object sender, EventArgs e)
        {
            if (btnPolling.Text.Equals("Start Polling Trigger", StringComparison.OrdinalIgnoreCase))
            {
                //start reading alerts
                btnPolling.Text = "Stop Polling Trigger";

                server.Start();
                reader1.startInventory();

                addTextTo_txtMsg(String.Format("Start reading alert at port: {0} ...\r\n", server.tcp_port));
                tsslAlert.BackColor = Color.Green;
                Application.DoEvents();

                if (chkAutoPollingEnable.Checked)
                {
                    countDown = (int)numPollingTmr.Value;
                    txtCountDown.Text = countDown.ToString("0");
                    txtCountDown.BackColor = Color.Orange;
                    tmrPolling.Interval = 1000; //1sec
                    tmrPolling.Enabled = true;
                    btnPollingReadTags.Enabled = false;
                }
                else
                {
                    btnPollingReadTags.Enabled = true;
                    txtCountDown.Text = "";
                }
                chkAutoPollingEnable.Enabled = false;
                numPollingTmr.Enabled = false;
                btnAlert.Enabled = false;
                txtAlertPort.Enabled = false;
                txtPollingPort.Enabled = false;
            }
            else
            {
                //stop reading alerts
                btnPolling.Text = "Start Polling Trigger";

                server.Stop();

                addTextTo_txtMsg("Stop reading alert.\r\n");
                tsslAlert.BackColor = Color.Red;
                Application.DoEvents();

                tmrPolling.Enabled = false;
                btnPollingReadTags.Enabled = false;
                chkAutoPollingEnable.Enabled = true;
                numPollingTmr.Enabled = true;
                btnAlert.Enabled = true;
                txtAlertPort.Enabled = true;
                txtPollingPort.Enabled = true;
                txtCountDown.Text = "";
                txtCountDown.BackColor = Color.Empty;
            }
        }

        private void btnPollingReadTags_Click(object sender, EventArgs e)
        {
            if (reader1.connect() == false)
            {
                addTextTo_txtMsg("Fail to connect to reader");
                return;
            }

            if (reader1.startInventory() == false)
            {
                MessageBox.Show("Fail to start Inventory using Polling Triggering Method\n Please check the Operation Profile is correct.");
            }
            reader1.logout();
        }

        private void txtPollingPort_TextChanged(object sender, EventArgs e)
        {
            Int32 d;

            if (Int32.TryParse(txtPollingPort.Text, out d))
            {
                if ((d > -1) && (d < 65536))
                {
                    server.tcp_port = d;
                    txtPollingPort.ForeColor = Color.Black;
                    Application.UserAppDataRegistry.SetValue("TcpPort", d, Microsoft.Win32.RegistryValueKind.DWord);
                    if (txtPollingPort.Text != txtAlertPort.Text)
                        txtAlertPort.Text = txtPollingPort.Text;
                    return;
                }
            }
            txtPollingPort.ForeColor = Color.Red;
        }

        private void tmrPolling_Tick(object sender, EventArgs e)
        {
            if (btnPolling.Text.Equals("Start Polling Trigger", StringComparison.OrdinalIgnoreCase)) return;

            countDown--;
            txtCountDown.Text = countDown.ToString("0"); ;
            if (countDown == 0)
            {
                if (reader1.connect() == false)
                {
                    addTextTo_txtMsg("Fail to connect to reader");
                    return;
                }

                reader1.startInventory();
                addTextTo_txtMsg("Auto Polling\r\n");
                countDown = (int)numPollingTmr.Value;
                txtCountDown.Text = countDown.ToString("0");
                if (txtCountDown.BackColor == Color.Orange)
                {
                    txtCountDown.BackColor = Color.Yellow;
                }
                else
                {
                    txtCountDown.BackColor = Color.Orange;
                }
            }
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            updateLayout();
        }

        private void updateLayout()
        {
            int space = 5;

            pbLogo.Location = new Point(ClientRectangle.Right - pbLogo.Width - space, statusStrip1.Location.Y + statusStrip1.Height + space);

            btnClearTable.Location = new Point(pbLogo.Location.X, pbLogo.Location.Y + pbLogo.Height + space);

            tabControl1.Location = new Point(ClientRectangle.Left + space, statusStrip1.Location.Y + statusStrip1.Height + space);
            tabControl1.Width = ClientSize.Width - pbLogo.Width - (space * 3);

            dgvTags.Location = new Point(ClientRectangle.Left + space, tabControl1.Location.Y + tabControl1.Height + (space * 2));
            dgvTags.Width = ClientSize.Width - (space * 2);
            dgvTags.Height = ClientSize.Height - dgvTags.Location.Y - space;
        }

        private void chkAutoPollingEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoPollingEnable.Checked)
                Application.UserAppDataRegistry.SetValue("AutoPollingEnable", 1, Microsoft.Win32.RegistryValueKind.DWord);
            else
                Application.UserAppDataRegistry.SetValue("AutoPollingEnable", 0, Microsoft.Win32.RegistryValueKind.DWord);
        }

        private void numPollingTmr_ValueChanged(object sender, EventArgs e)
        {
            Application.UserAppDataRegistry.SetValue("PollingPeriod", numPollingTmr.Value, Microsoft.Win32.RegistryValueKind.DWord);
        }

        #region event handler
        public void reader1_BatchEnd(object sender, BatchEndEventArgs e)
        {
            reader1.saveToLogInfo(String.Format("Batch End Event received: {0}", e.TimeStamp.ToString()));
        }

        public void reader1_AntennaEvent(object sender, AntennaEventArgs e)
        {
            reader1.saveToLogInfo(String.Format("Antenna Event received: {0}, {1}", e.Ports, e.TimeStamp.ToString()));
            addTextTo_tsddbAntenna(String.Format("{0}, Antenna mismatched: {1}", DateTime.Now.ToShortTimeString(), e.Ports));
        }

        public void reader1_BufferOverflowEvent(object sender, BufferOverflowEventArgs e)
        {
            reader1.saveToLogInfo(String.Format("Buffer Overflow Event received: {0}", e.TimeStamp.ToString()));
        }

        public void reader1_TagReceiveEvent(object sender, TagReceiveEventArgs e)
        {
            if (e.rxTag != null)
            {
                Monitor.Enter(e);
                TAG tag = (TAG)e.rxTag;
                if (rbInstantAlert.Checked)
                {
                    update_dgvTag_Delegate udd = new update_dgvTag_Delegate(update_dgvTag);
                    this.BeginInvoke(udd, tag);
                    if (chkBeep.Checked == true)
                        System.Media.SystemSounds.Beep.Play();
                }
                reader1.saveToLogInfo(String.Format("Tag Receive Event received: {0}", tag.TagOrigId));
                Monitor.Exit(e);

            }
            else
            {
                reader1.saveToLogInfo("Tag Receive Event received: None");
            }
        }

        public void reader1_TagListEvent(object sender, TagListEventArgs e)
        {
            if (e.TagsList != null)
            {
                Monitor.Enter(e);
                if (rbBatchAlert.Checked)
                {
                    update_dgvTagList_Delegate udd = new update_dgvTagList_Delegate(update_dgvTagList);
                    System.Collections.ArrayList list = new System.Collections.ArrayList(e.TagsList);
                    this.BeginInvoke(udd, list);
                    if (chkBeep.Checked == true)
                        System.Media.SystemSounds.Beep.Play();
                }
                reader1.saveToLogInfo(String.Format("Tag List Event received: {0}", e.TagsList.Count));
                Monitor.Exit(e);
            }
            else
            {
                reader1.saveToLogInfo("Tag List Event received: None");
            }
        }

        public void reader1_InventoryEvent(object sender, InventoryEventArgs e)
        {
            if (e.InventoryNtf != null)
            {
                INVENTORY_NTF_INFO o = (INVENTORY_NTF_INFO)e.InventoryNtf;
                reader1.saveToLogInfo(String.Format("Inventory Event received: {0}, {1}", o.id, o.msg));
                addTextTo_tsddbAntenna(String.Format("Event:{0} {1}", o.id, o.msg));
            }
        }
        #endregion

        private void cbLogLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            Application.UserAppDataRegistry.SetValue("LogLevel", cbLogLevel.Text, Microsoft.Win32.RegistryValueKind.String);
            reader1.api_log_level = reader1.LogLevel(cbLogLevel.Text);
        }

        private void dgvTags_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex < 0) return;

            object[] row = tagsTable.Rows[e.RowIndex].ItemArray;
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("Bank0 = {0}\r\n", row[13]));
            sb.Append(String.Format("Protocol Control (PC) = {0}\r\n", row[16]));
            sb.Append(String.Format("Bank1 (EPC) = {0}\r\n", row[1]));
            sb.Append(String.Format("Bank2 (TID) = {0}\r\n", row[14]));
            sb.Append(String.Format("Bank3 (User Memory) = {0}\r\n", row[15]));
            sb.Append(String.Format("RSSI = {0}\r\n", row[4]));
            sb.Append(String.Format("Antenna = {0}\r\n", row[11]));
            sb.Append(String.Format("Reader IP = {0}\r\n", row[6]));
            sb.Append(String.Format("Event = {0}\r\n", row[3]));
            sb.Append(String.Format("Frequency = {0}\r\n", row[8]));
            sb.Append(String.Format("Time = {0}\r\n", row[5]));
            sb.Append(String.Format("TimeStamp (UTC) = {0}\r\n", row[7]));
            sb.Append(String.Format("Session = {0}\r\n", row[9]));
            sb.Append(String.Format("Index= {0}", row[10]));

            e.ToolTipText = sb.ToString();
        }

        private void rbInstantAlert_CheckedChanged(object sender, EventArgs e)
        {
            if (rbInstantAlert.Checked == true)
            {
                numRefresh.Enabled = true;
            }
            else
            {
                numRefresh.Enabled = false;
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabGetCaptureTags);
            tabControl1.TabPages.Remove(tabPollingTriggerByClient);
        }

        private void newTagGroupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_NewTagGroup frm = new Form_NewTagGroup();
            frm.ShowDialog();
        }

        private void addMemberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_AddMember frm = new Form_AddMember();
            frm.ShowDialog();
        }

        private void deleteMemberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_DelMember frm = new Form_DelMember();
            frm.ShowDialog();
        }

        private void deleteAllMemberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_DelAllMember frm = new Form_DelAllMember();
            frm.ShowDialog();
        }

        private void importMembersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form_ImportMembers frm = new Form_ImportMembers();
            frm.ShowDialog();
        }
    }
}