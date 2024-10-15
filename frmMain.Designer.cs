namespace CS463_HL_CS
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsddbAntenna = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsslAlert = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslClock = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrClock = new System.Windows.Forms.Timer(this.components);
            this.btnReadTags = new System.Windows.Forms.Button();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.dgvTags = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.txtURI = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numTagLiveTime = new System.Windows.Forms.NumericUpDown();
            this.btnClearTable = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTagsToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readerIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.operationProfileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.captureNameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trustedServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triggerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resultantActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miscToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newTagGroupToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMemberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMemberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteAllMemberToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importMembersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAlert = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAlertPort = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.cbLogLevel = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tabGetCaptureTags = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.numDuration = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.cbBank3 = new System.Windows.Forms.CheckBox();
            this.cbBank2 = new System.Windows.Forms.CheckBox();
            this.cbBank1 = new System.Windows.Forms.CheckBox();
            this.cbBank0 = new System.Windows.Forms.CheckBox();
            this.tabAutonomousTimeTrigger = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkBeep = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numRefresh = new System.Windows.Forms.NumericUpDown();
            this.rbInstantAlert = new System.Windows.Forms.RadioButton();
            this.rbBatchAlert = new System.Windows.Forms.RadioButton();
            this.tabPollingTriggerByClient = new System.Windows.Forms.TabPage();
            this.txtCountDown = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numPollingTmr = new System.Windows.Forms.NumericUpDown();
            this.chkAutoPollingEnable = new System.Windows.Forms.CheckBox();
            this.btnPollingReadTags = new System.Windows.Forms.Button();
            this.txtPollingPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnPolling = new System.Windows.Forms.Button();
            this.tabSystemMessages = new System.Windows.Forms.TabPage();
            this.pbLogo = new System.Windows.Forms.PictureBox();
            this.tmrPolling = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTags)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTagLiveTime)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabGetCaptureTags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).BeginInit();
            this.tabAutonomousTimeTrigger.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRefresh)).BeginInit();
            this.tabPollingTriggerByClient.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPollingTmr)).BeginInit();
            this.tabSystemMessages.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslStatus,
            this.tsddbAntenna,
            this.tsslAlert,
            this.tsslCount,
            this.tsslClock});
            this.statusStrip1.Location = new System.Drawing.Point(0, 28);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(792, 27);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsslStatus
            // 
            this.tsslStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslStatus.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsslStatus.Name = "tsslStatus";
            this.tsslStatus.Size = new System.Drawing.Size(288, 22);
            this.tsslStatus.Spring = true;
            this.tsslStatus.Text = "Ready...";
            this.tsslStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsddbAntenna
            // 
            this.tsddbAntenna.AutoSize = false;
            this.tsddbAntenna.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsddbAntenna.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsddbAntenna.ForeColor = System.Drawing.Color.Red;
            this.tsddbAntenna.Image = ((System.Drawing.Image)(resources.GetObject("tsddbAntenna.Image")));
            this.tsddbAntenna.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsddbAntenna.Name = "tsddbAntenna";
            this.tsddbAntenna.Size = new System.Drawing.Size(270, 25);
            this.tsddbAntenna.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslAlert
            // 
            this.tsslAlert.BackColor = System.Drawing.Color.Red;
            this.tsslAlert.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslAlert.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.tsslAlert.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsslAlert.Name = "tsslAlert";
            this.tsslAlert.Size = new System.Drawing.Size(111, 22);
            this.tsslAlert.Text = "Alert Listening";
            // 
            // tsslCount
            // 
            this.tsslCount.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsslCount.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenInner;
            this.tsslCount.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsslCount.Name = "tsslCount";
            this.tsslCount.Size = new System.Drawing.Size(34, 22);
            this.tsslCount.Text = "0/0";
            // 
            // tsslClock
            // 
            this.tsslClock.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)));
            this.tsslClock.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsslClock.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslClock.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsslClock.Name = "tsslClock";
            this.tsslClock.Size = new System.Drawing.Size(74, 22);
            this.tsslClock.Text = "00:00:00";
            this.tsslClock.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tmrClock
            // 
            this.tmrClock.Interval = 30000;
            this.tmrClock.Tick += new System.EventHandler(this.tmrClock_Tick);
            // 
            // btnReadTags
            // 
            this.btnReadTags.Location = new System.Drawing.Point(258, 34);
            this.btnReadTags.Name = "btnReadTags";
            this.btnReadTags.Size = new System.Drawing.Size(184, 31);
            this.btnReadTags.TabIndex = 0;
            this.btnReadTags.Text = "&Read Memory Banks";
            this.btnReadTags.UseVisualStyleBackColor = true;
            this.btnReadTags.Click += new System.EventHandler(this.btnReadTags_Click);
            // 
            // txtMsg
            // 
            this.txtMsg.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMsg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMsg.Location = new System.Drawing.Point(0, 0);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtMsg.Size = new System.Drawing.Size(502, 103);
            this.txtMsg.TabIndex = 14;
            // 
            // dgvTags
            // 
            this.dgvTags.AllowUserToAddRows = false;
            this.dgvTags.AllowUserToDeleteRows = false;
            this.dgvTags.Location = new System.Drawing.Point(0, 180);
            this.dgvTags.MultiSelect = false;
            this.dgvTags.Name = "dgvTags";
            this.dgvTags.ReadOnly = true;
            this.dgvTags.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvTags.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTags.Size = new System.Drawing.Size(792, 420);
            this.dgvTags.TabIndex = 13;
            this.dgvTags.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.dgvTags_CellToolTipTextNeeded);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "&URI";
            // 
            // txtURI
            // 
            this.txtURI.Location = new System.Drawing.Point(116, 23);
            this.txtURI.Name = "txtURI";
            this.txtURI.Size = new System.Drawing.Size(368, 20);
            this.txtURI.TabIndex = 4;
            this.txtURI.Text = "http://192.168.25.208/";
            this.txtURI.TextChanged += new System.EventHandler(this.txtURI_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Tag &Live Time (sec)";
            // 
            // numTagLiveTime
            // 
            this.numTagLiveTime.Enabled = false;
            this.numTagLiveTime.Location = new System.Drawing.Point(116, 60);
            this.numTagLiveTime.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numTagLiveTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTagLiveTime.Name = "numTagLiveTime";
            this.numTagLiveTime.Size = new System.Drawing.Size(128, 20);
            this.numTagLiveTime.TabIndex = 8;
            this.numTagLiveTime.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // btnClearTable
            // 
            this.btnClearTable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearTable.Location = new System.Drawing.Point(515, 155);
            this.btnClearTable.Name = "btnClearTable";
            this.btnClearTable.Size = new System.Drawing.Size(272, 23);
            this.btnClearTable.TabIndex = 2;
            this.btnClearTable.Text = "&Clear Table";
            this.btnClearTable.UseVisualStyleBackColor = true;
            this.btnClearTable.Click += new System.EventHandler(this.btnClearTable_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.miscToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(792, 28);
            this.menuStrip1.TabIndex = 12;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveTagsToFileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveTagsToFileToolStripMenuItem
            // 
            this.saveTagsToFileToolStripMenuItem.Name = "saveTagsToFileToolStripMenuItem";
            this.saveTagsToFileToolStripMenuItem.Size = new System.Drawing.Size(184, 24);
            this.saveTagsToFileToolStripMenuItem.Text = "Save tags to file";
            this.saveTagsToFileToolStripMenuItem.Click += new System.EventHandler(this.saveTagsToFileToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readerIDToolStripMenuItem,
            this.operationProfileToolStripMenuItem,
            this.captureNameToolStripMenuItem,
            this.trustedServerToolStripMenuItem,
            this.triggerToolStripMenuItem,
            this.resultantActionToolStripMenuItem,
            this.eventToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(74, 24);
            this.settingsToolStripMenuItem.Text = "&Settings";
            // 
            // readerIDToolStripMenuItem
            // 
            this.readerIDToolStripMenuItem.Name = "readerIDToolStripMenuItem";
            this.readerIDToolStripMenuItem.Size = new System.Drawing.Size(192, 24);
            this.readerIDToolStripMenuItem.Text = "Reader ID";
            this.readerIDToolStripMenuItem.Click += new System.EventHandler(this.readerIDToolStripMenuItem_Click);
            // 
            // operationProfileToolStripMenuItem
            // 
            this.operationProfileToolStripMenuItem.Name = "operationProfileToolStripMenuItem";
            this.operationProfileToolStripMenuItem.Size = new System.Drawing.Size(192, 24);
            this.operationProfileToolStripMenuItem.Text = "Operation Profile";
            this.operationProfileToolStripMenuItem.Click += new System.EventHandler(this.operationProfileToolStripMenuItem_Click);
            // 
            // captureNameToolStripMenuItem
            // 
            this.captureNameToolStripMenuItem.Name = "captureNameToolStripMenuItem";
            this.captureNameToolStripMenuItem.Size = new System.Drawing.Size(192, 24);
            this.captureNameToolStripMenuItem.Text = "Capture Name";
            this.captureNameToolStripMenuItem.Click += new System.EventHandler(this.captureNameToolStripMenuItem_Click);
            // 
            // trustedServerToolStripMenuItem
            // 
            this.trustedServerToolStripMenuItem.Name = "trustedServerToolStripMenuItem";
            this.trustedServerToolStripMenuItem.Size = new System.Drawing.Size(192, 24);
            this.trustedServerToolStripMenuItem.Text = "Trusted Server";
            this.trustedServerToolStripMenuItem.Click += new System.EventHandler(this.trustedServerToolStripMenuItem_Click);
            // 
            // triggerToolStripMenuItem
            // 
            this.triggerToolStripMenuItem.Name = "triggerToolStripMenuItem";
            this.triggerToolStripMenuItem.Size = new System.Drawing.Size(192, 24);
            this.triggerToolStripMenuItem.Text = "Trigger";
            this.triggerToolStripMenuItem.Click += new System.EventHandler(this.triggerToolStripMenuItem_Click);
            // 
            // resultantActionToolStripMenuItem
            // 
            this.resultantActionToolStripMenuItem.Name = "resultantActionToolStripMenuItem";
            this.resultantActionToolStripMenuItem.Size = new System.Drawing.Size(192, 24);
            this.resultantActionToolStripMenuItem.Text = "Resultant Action";
            this.resultantActionToolStripMenuItem.Click += new System.EventHandler(this.resultantActionToolStripMenuItem_Click);
            // 
            // eventToolStripMenuItem
            // 
            this.eventToolStripMenuItem.Name = "eventToolStripMenuItem";
            this.eventToolStripMenuItem.Size = new System.Drawing.Size(192, 24);
            this.eventToolStripMenuItem.Text = "Event";
            this.eventToolStripMenuItem.Click += new System.EventHandler(this.eventToolStripMenuItem_Click);
            // 
            // miscToolStripMenuItem
            // 
            this.miscToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newTagGroupToolStripMenuItem,
            this.addMemberToolStripMenuItem,
            this.deleteMemberToolStripMenuItem,
            this.deleteAllMemberToolStripMenuItem,
            this.importMembersToolStripMenuItem});
            this.miscToolStripMenuItem.Name = "miscToolStripMenuItem";
            this.miscToolStripMenuItem.Size = new System.Drawing.Size(85, 24);
            this.miscToolStripMenuItem.Text = "TagGroup";
            // 
            // newTagGroupToolStripMenuItem
            // 
            this.newTagGroupToolStripMenuItem.Name = "newTagGroupToolStripMenuItem";
            this.newTagGroupToolStripMenuItem.Size = new System.Drawing.Size(204, 24);
            this.newTagGroupToolStripMenuItem.Text = "New TagGroup";
            this.newTagGroupToolStripMenuItem.Click += new System.EventHandler(this.newTagGroupToolStripMenuItem_Click);
            // 
            // addMemberToolStripMenuItem
            // 
            this.addMemberToolStripMenuItem.Name = "addMemberToolStripMenuItem";
            this.addMemberToolStripMenuItem.Size = new System.Drawing.Size(204, 24);
            this.addMemberToolStripMenuItem.Text = "Add Member";
            this.addMemberToolStripMenuItem.Click += new System.EventHandler(this.addMemberToolStripMenuItem_Click);
            // 
            // deleteMemberToolStripMenuItem
            // 
            this.deleteMemberToolStripMenuItem.Name = "deleteMemberToolStripMenuItem";
            this.deleteMemberToolStripMenuItem.Size = new System.Drawing.Size(204, 24);
            this.deleteMemberToolStripMenuItem.Text = "Delete Member";
            this.deleteMemberToolStripMenuItem.Click += new System.EventHandler(this.deleteMemberToolStripMenuItem_Click);
            // 
            // deleteAllMemberToolStripMenuItem
            // 
            this.deleteAllMemberToolStripMenuItem.Name = "deleteAllMemberToolStripMenuItem";
            this.deleteAllMemberToolStripMenuItem.Size = new System.Drawing.Size(204, 24);
            this.deleteAllMemberToolStripMenuItem.Text = "Delete All Member";
            this.deleteAllMemberToolStripMenuItem.Click += new System.EventHandler(this.deleteAllMemberToolStripMenuItem_Click);
            // 
            // importMembersToolStripMenuItem
            // 
            this.importMembersToolStripMenuItem.Name = "importMembersToolStripMenuItem";
            this.importMembersToolStripMenuItem.Size = new System.Drawing.Size(204, 24);
            this.importMembersToolStripMenuItem.Text = "Import Members";
            this.importMembersToolStripMenuItem.Click += new System.EventHandler(this.importMembersToolStripMenuItem_Click);
            // 
            // btnAlert
            // 
            this.btnAlert.Location = new System.Drawing.Point(22, 58);
            this.btnAlert.Name = "btnAlert";
            this.btnAlert.Size = new System.Drawing.Size(122, 24);
            this.btnAlert.TabIndex = 11;
            this.btnAlert.Text = "Start Read Alert";
            this.btnAlert.UseVisualStyleBackColor = true;
            this.btnAlert.Click += new System.EventHandler(this.btnAlert_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(50, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Alert Port";
            // 
            // txtAlertPort
            // 
            this.txtAlertPort.Location = new System.Drawing.Point(80, 15);
            this.txtAlertPort.Name = "txtAlertPort";
            this.txtAlertPort.Size = new System.Drawing.Size(54, 20);
            this.txtAlertPort.TabIndex = 10;
            this.txtAlertPort.Text = "9090";
            this.txtAlertPort.TextChanged += new System.EventHandler(this.txtAlertPort_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabGetCaptureTags);
            this.tabControl1.Controls.Add(this.tabAutonomousTimeTrigger);
            this.tabControl1.Controls.Add(this.tabPollingTriggerByClient);
            this.tabControl1.Controls.Add(this.tabSystemMessages);
            this.tabControl1.Location = new System.Drawing.Point(0, 51);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(510, 129);
            this.tabControl1.TabIndex = 19;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.label10);
            this.tabGeneral.Controls.Add(this.cbLogLevel);
            this.tabGeneral.Controls.Add(this.label7);
            this.tabGeneral.Controls.Add(this.txtURI);
            this.tabGeneral.Controls.Add(this.label1);
            this.tabGeneral.Controls.Add(this.label3);
            this.tabGeneral.Controls.Add(this.numTagLiveTime);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabGeneral.Size = new System.Drawing.Size(502, 103);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(113, 7);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(101, 13);
            this.label10.TabIndex = 11;
            this.label10.Text = "( http:// or https:// )";
            // 
            // cbLogLevel
            // 
            this.cbLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbLogLevel.FormattingEnabled = true;
            this.cbLogLevel.Items.AddRange(new object[] {
            "Disable",
            "Info",
            "Verbose"});
            this.cbLogLevel.Location = new System.Drawing.Point(350, 59);
            this.cbLogLevel.Name = "cbLogLevel";
            this.cbLogLevel.Size = new System.Drawing.Size(134, 21);
            this.cbLogLevel.TabIndex = 10;
            this.cbLogLevel.SelectedIndexChanged += new System.EventHandler(this.cbLogLevel_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(278, 62);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "Log Level:";
            // 
            // tabGetCaptureTags
            // 
            this.tabGetCaptureTags.Controls.Add(this.label9);
            this.tabGetCaptureTags.Controls.Add(this.numDuration);
            this.tabGetCaptureTags.Controls.Add(this.label8);
            this.tabGetCaptureTags.Controls.Add(this.cbBank3);
            this.tabGetCaptureTags.Controls.Add(this.cbBank2);
            this.tabGetCaptureTags.Controls.Add(this.cbBank1);
            this.tabGetCaptureTags.Controls.Add(this.cbBank0);
            this.tabGetCaptureTags.Controls.Add(this.btnReadTags);
            this.tabGetCaptureTags.Location = new System.Drawing.Point(4, 22);
            this.tabGetCaptureTags.Name = "tabGetCaptureTags";
            this.tabGetCaptureTags.Padding = new System.Windows.Forms.Padding(3);
            this.tabGetCaptureTags.Size = new System.Drawing.Size(502, 103);
            this.tabGetCaptureTags.TabIndex = 1;
            this.tabGetCaptureTags.Text = "GetCaptureTags";
            this.tabGetCaptureTags.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Red;
            this.label9.Location = new System.Drawing.Point(170, 76);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(326, 13);
            this.label9.TabIndex = 7;
            this.label9.Text = "Note: Duplicate Elimination Method must be Polling Trigger by Client";
            // 
            // numDuration
            // 
            this.numDuration.Location = new System.Drawing.Point(322, 8);
            this.numDuration.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numDuration.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.numDuration.Name = "numDuration";
            this.numDuration.Size = new System.Drawing.Size(120, 20);
            this.numDuration.TabIndex = 6;
            this.numDuration.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(255, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Duration";
            // 
            // cbBank3
            // 
            this.cbBank3.AutoSize = true;
            this.cbBank3.Location = new System.Drawing.Point(21, 75);
            this.cbBank3.Name = "cbBank3";
            this.cbBank3.Size = new System.Drawing.Size(127, 17);
            this.cbBank3.TabIndex = 4;
            this.cbBank3.Text = "Bank3 (User memory)";
            this.cbBank3.UseVisualStyleBackColor = true;
            // 
            // cbBank2
            // 
            this.cbBank2.AutoSize = true;
            this.cbBank2.Location = new System.Drawing.Point(21, 52);
            this.cbBank2.Name = "cbBank2";
            this.cbBank2.Size = new System.Drawing.Size(114, 17);
            this.cbBank2.TabIndex = 3;
            this.cbBank2.Text = "Bank2 (TID / UID)";
            this.cbBank2.UseVisualStyleBackColor = true;
            // 
            // cbBank1
            // 
            this.cbBank1.AutoSize = true;
            this.cbBank1.Checked = true;
            this.cbBank1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbBank1.Enabled = false;
            this.cbBank1.Location = new System.Drawing.Point(21, 29);
            this.cbBank1.Name = "cbBank1";
            this.cbBank1.Size = new System.Drawing.Size(87, 17);
            this.cbBank1.TabIndex = 2;
            this.cbBank1.Text = "Bank1 (EPC)";
            this.cbBank1.UseVisualStyleBackColor = true;
            // 
            // cbBank0
            // 
            this.cbBank0.AutoSize = true;
            this.cbBank0.Location = new System.Drawing.Point(21, 6);
            this.cbBank0.Name = "cbBank0";
            this.cbBank0.Size = new System.Drawing.Size(117, 17);
            this.cbBank0.TabIndex = 1;
            this.cbBank0.Text = "Bank0 (Passwords)";
            this.cbBank0.UseVisualStyleBackColor = true;
            // 
            // tabAutonomousTimeTrigger
            // 
            this.tabAutonomousTimeTrigger.Controls.Add(this.groupBox1);
            this.tabAutonomousTimeTrigger.Controls.Add(this.label4);
            this.tabAutonomousTimeTrigger.Controls.Add(this.txtAlertPort);
            this.tabAutonomousTimeTrigger.Controls.Add(this.btnAlert);
            this.tabAutonomousTimeTrigger.Location = new System.Drawing.Point(4, 22);
            this.tabAutonomousTimeTrigger.Name = "tabAutonomousTimeTrigger";
            this.tabAutonomousTimeTrigger.Size = new System.Drawing.Size(502, 103);
            this.tabAutonomousTimeTrigger.TabIndex = 2;
            this.tabAutonomousTimeTrigger.Text = "Autonomous Time Trigger";
            this.tabAutonomousTimeTrigger.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkBeep);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.numRefresh);
            this.groupBox1.Controls.Add(this.rbInstantAlert);
            this.groupBox1.Controls.Add(this.rbBatchAlert);
            this.groupBox1.Location = new System.Drawing.Point(163, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(314, 71);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trusted Server Operating Mode";
            // 
            // chkBeep
            // 
            this.chkBeep.AutoSize = true;
            this.chkBeep.Location = new System.Drawing.Point(201, 46);
            this.chkBeep.Name = "chkBeep";
            this.chkBeep.Size = new System.Drawing.Size(51, 17);
            this.chkBeep.TabIndex = 4;
            this.chkBeep.Text = "Beep";
            this.chkBeep.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Refresh Time (sec)";
            // 
            // numRefresh
            // 
            this.numRefresh.Enabled = false;
            this.numRefresh.Location = new System.Drawing.Point(108, 43);
            this.numRefresh.Name = "numRefresh";
            this.numRefresh.Size = new System.Drawing.Size(66, 20);
            this.numRefresh.TabIndex = 2;
            this.numRefresh.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // rbInstantAlert
            // 
            this.rbInstantAlert.AutoSize = true;
            this.rbInstantAlert.Location = new System.Drawing.Point(6, 19);
            this.rbInstantAlert.Name = "rbInstantAlert";
            this.rbInstantAlert.Size = new System.Drawing.Size(111, 17);
            this.rbInstantAlert.TabIndex = 1;
            this.rbInstantAlert.Text = "Instant Alert Mode";
            this.rbInstantAlert.UseVisualStyleBackColor = true;
            this.rbInstantAlert.CheckedChanged += new System.EventHandler(this.rbInstantAlert_CheckedChanged);
            // 
            // rbBatchAlert
            // 
            this.rbBatchAlert.AutoSize = true;
            this.rbBatchAlert.Checked = true;
            this.rbBatchAlert.Location = new System.Drawing.Point(201, 20);
            this.rbBatchAlert.Name = "rbBatchAlert";
            this.rbBatchAlert.Size = new System.Drawing.Size(107, 17);
            this.rbBatchAlert.TabIndex = 0;
            this.rbBatchAlert.TabStop = true;
            this.rbBatchAlert.Text = "Batch Alert Mode";
            this.rbBatchAlert.UseVisualStyleBackColor = true;
            // 
            // tabPollingTriggerByClient
            // 
            this.tabPollingTriggerByClient.Controls.Add(this.txtCountDown);
            this.tabPollingTriggerByClient.Controls.Add(this.label6);
            this.tabPollingTriggerByClient.Controls.Add(this.numPollingTmr);
            this.tabPollingTriggerByClient.Controls.Add(this.chkAutoPollingEnable);
            this.tabPollingTriggerByClient.Controls.Add(this.btnPollingReadTags);
            this.tabPollingTriggerByClient.Controls.Add(this.txtPollingPort);
            this.tabPollingTriggerByClient.Controls.Add(this.label5);
            this.tabPollingTriggerByClient.Controls.Add(this.btnPolling);
            this.tabPollingTriggerByClient.Location = new System.Drawing.Point(4, 22);
            this.tabPollingTriggerByClient.Name = "tabPollingTriggerByClient";
            this.tabPollingTriggerByClient.Size = new System.Drawing.Size(502, 103);
            this.tabPollingTriggerByClient.TabIndex = 3;
            this.tabPollingTriggerByClient.Text = "Polling Trigger by Client";
            this.tabPollingTriggerByClient.UseVisualStyleBackColor = true;
            // 
            // txtCountDown
            // 
            this.txtCountDown.Location = new System.Drawing.Point(375, 53);
            this.txtCountDown.Name = "txtCountDown";
            this.txtCountDown.ReadOnly = true;
            this.txtCountDown.Size = new System.Drawing.Size(95, 20);
            this.txtCountDown.TabIndex = 23;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(303, 56);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 13);
            this.label6.TabIndex = 22;
            this.label6.Text = "Count Down";
            // 
            // numPollingTmr
            // 
            this.numPollingTmr.Location = new System.Drawing.Point(217, 54);
            this.numPollingTmr.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numPollingTmr.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPollingTmr.Name = "numPollingTmr";
            this.numPollingTmr.Size = new System.Drawing.Size(65, 20);
            this.numPollingTmr.TabIndex = 21;
            this.numPollingTmr.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numPollingTmr.ValueChanged += new System.EventHandler(this.numPollingTmr_ValueChanged);
            // 
            // chkAutoPollingEnable
            // 
            this.chkAutoPollingEnable.AutoSize = true;
            this.chkAutoPollingEnable.Location = new System.Drawing.Point(31, 57);
            this.chkAutoPollingEnable.Name = "chkAutoPollingEnable";
            this.chkAutoPollingEnable.Size = new System.Drawing.Size(166, 17);
            this.chkAutoPollingEnable.TabIndex = 20;
            this.chkAutoPollingEnable.Text = "Auto-refresh interval (Second)";
            this.chkAutoPollingEnable.UseVisualStyleBackColor = true;
            this.chkAutoPollingEnable.CheckedChanged += new System.EventHandler(this.chkAutoPollingEnable_CheckedChanged);
            // 
            // btnPollingReadTags
            // 
            this.btnPollingReadTags.Enabled = false;
            this.btnPollingReadTags.Location = new System.Drawing.Point(360, 17);
            this.btnPollingReadTags.Name = "btnPollingReadTags";
            this.btnPollingReadTags.Size = new System.Drawing.Size(110, 23);
            this.btnPollingReadTags.TabIndex = 19;
            this.btnPollingReadTags.Text = "Read Tags";
            this.btnPollingReadTags.UseVisualStyleBackColor = true;
            this.btnPollingReadTags.Click += new System.EventHandler(this.btnPollingReadTags_Click);
            // 
            // txtPollingPort
            // 
            this.txtPollingPort.Location = new System.Drawing.Point(95, 20);
            this.txtPollingPort.Name = "txtPollingPort";
            this.txtPollingPort.Size = new System.Drawing.Size(100, 20);
            this.txtPollingPort.TabIndex = 18;
            this.txtPollingPort.Text = "9090";
            this.txtPollingPort.TextChanged += new System.EventHandler(this.txtPollingPort_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(50, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Alert Port";
            // 
            // btnPolling
            // 
            this.btnPolling.Location = new System.Drawing.Point(217, 17);
            this.btnPolling.Name = "btnPolling";
            this.btnPolling.Size = new System.Drawing.Size(118, 24);
            this.btnPolling.TabIndex = 16;
            this.btnPolling.Text = "Start Polling Trigger";
            this.btnPolling.UseVisualStyleBackColor = true;
            this.btnPolling.Click += new System.EventHandler(this.btnPolling_Click);
            // 
            // tabSystemMessages
            // 
            this.tabSystemMessages.Controls.Add(this.txtMsg);
            this.tabSystemMessages.Location = new System.Drawing.Point(4, 22);
            this.tabSystemMessages.Name = "tabSystemMessages";
            this.tabSystemMessages.Size = new System.Drawing.Size(502, 103);
            this.tabSystemMessages.TabIndex = 4;
            this.tabSystemMessages.Text = "System Messages";
            this.tabSystemMessages.UseVisualStyleBackColor = true;
            // 
            // pbLogo
            // 
            this.pbLogo.Image = ((System.Drawing.Image)(resources.GetObject("pbLogo.Image")));
            this.pbLogo.Location = new System.Drawing.Point(515, 51);
            this.pbLogo.Name = "pbLogo";
            this.pbLogo.Padding = new System.Windows.Forms.Padding(3);
            this.pbLogo.Size = new System.Drawing.Size(272, 106);
            this.pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbLogo.TabIndex = 20;
            this.pbLogo.TabStop = false;
            // 
            // tmrPolling
            // 
            this.tmrPolling.Tick += new System.EventHandler(this.tmrPolling_Tick);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 600);
            this.Controls.Add(this.btnClearTable);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.pbLogo);
            this.Controls.Add(this.dgvTags);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "frmMain";
            this.Text = "CS463 Demo (High Level API)";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTags)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTagLiveTime)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            this.tabGetCaptureTags.ResumeLayout(false);
            this.tabGetCaptureTags.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numDuration)).EndInit();
            this.tabAutonomousTimeTrigger.ResumeLayout(false);
            this.tabAutonomousTimeTrigger.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numRefresh)).EndInit();
            this.tabPollingTriggerByClient.ResumeLayout(false);
            this.tabPollingTriggerByClient.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPollingTmr)).EndInit();
            this.tabSystemMessages.ResumeLayout(false);
            this.tabSystemMessages.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;
        private System.Windows.Forms.ToolStripStatusLabel tsslClock;
        private System.Windows.Forms.Timer tmrClock;
        private System.Windows.Forms.Button btnReadTags;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.DataGridView dgvTags;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtURI;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numTagLiveTime;
        private System.Windows.Forms.Button btnClearTable;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem readerIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem operationProfileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem captureNameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trustedServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triggerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resultantActionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem eventToolStripMenuItem;
        private System.Windows.Forms.Button btnAlert;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAlertPort;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTagsToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel tsslAlert;
        private System.Windows.Forms.ToolStripStatusLabel tsslCount;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.TabPage tabGetCaptureTags;
        private System.Windows.Forms.TabPage tabAutonomousTimeTrigger;
        private System.Windows.Forms.TabPage tabPollingTriggerByClient;
        private System.Windows.Forms.TabPage tabSystemMessages;
        private System.Windows.Forms.PictureBox pbLogo;
        private System.Windows.Forms.Button btnPolling;
        private System.Windows.Forms.TextBox txtPollingPort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnPollingReadTags;
        private System.Windows.Forms.Timer tmrPolling;
        private System.Windows.Forms.CheckBox chkAutoPollingEnable;
        private System.Windows.Forms.NumericUpDown numPollingTmr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCountDown;
        private System.Windows.Forms.ComboBox cbLogLevel;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ToolStripDropDownButton tsddbAntenna;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbInstantAlert;
        private System.Windows.Forms.RadioButton rbBatchAlert;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numRefresh;
        private System.Windows.Forms.CheckBox chkBeep;
        private System.Windows.Forms.CheckBox cbBank2;
        private System.Windows.Forms.CheckBox cbBank1;
        private System.Windows.Forms.CheckBox cbBank0;
        private System.Windows.Forms.CheckBox cbBank3;
        private System.Windows.Forms.NumericUpDown numDuration;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ToolStripMenuItem miscToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newTagGroupToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addMemberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMemberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteAllMemberToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importMembersToolStripMenuItem;
    }
}

