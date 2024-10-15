namespace CS463_HL_CS
{
    partial class frmOperationProfile
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
            this.label1 = new System.Windows.Forms.Label();
            this.cbProfileID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbModProfile = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.numPopEst = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cbSession = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.numTxPower = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.cbCapMode = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numWindowTime = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.chkEnable = new System.Windows.Forms.CheckBox();
            this.chkAnt1 = new System.Windows.Forms.CheckBox();
            this.chkAnt2 = new System.Windows.Forms.CheckBox();
            this.chkAnt3 = new System.Windows.Forms.CheckBox();
            this.chkAnt4 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cbTrigger = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numPopEst)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTxPower)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWindowTime)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Operation Profile";
            // 
            // cbProfileID
            // 
            this.cbProfileID.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbProfileID.FormattingEnabled = true;
            this.cbProfileID.Location = new System.Drawing.Point(152, 21);
            this.cbProfileID.Name = "cbProfileID";
            this.cbProfileID.Size = new System.Drawing.Size(403, 21);
            this.cbProfileID.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Modulation Profile";
            // 
            // cbModProfile
            // 
            this.cbModProfile.FormattingEnabled = true;
            this.cbModProfile.Items.AddRange(new object[] {
            "Profile0",
            "Profile1",
            "Profile2",
            "Profile3"});
            this.cbModProfile.Location = new System.Drawing.Point(138, 23);
            this.cbModProfile.Name = "cbModProfile";
            this.cbModProfile.Size = new System.Drawing.Size(158, 21);
            this.cbModProfile.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Population Estimation";
            // 
            // numPopEst
            // 
            this.numPopEst.Location = new System.Drawing.Point(138, 60);
            this.numPopEst.Maximum = new decimal(new int[] {
            65000,
            0,
            0,
            0});
            this.numPopEst.Name = "numPopEst";
            this.numPopEst.Size = new System.Drawing.Size(158, 20);
            this.numPopEst.TabIndex = 1;
            this.numPopEst.Value = new decimal(new int[] {
            64,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Session Number";
            // 
            // cbSession
            // 
            this.cbSession.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSession.FormattingEnabled = true;
            this.cbSession.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cbSession.Location = new System.Drawing.Point(138, 93);
            this.cbSession.Name = "cbSession";
            this.cbSession.Size = new System.Drawing.Size(158, 21);
            this.cbSession.TabIndex = 2;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 130);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Transmit Power (dBm)";
            // 
            // numTxPower
            // 
            this.numTxPower.DecimalPlaces = 2;
            this.numTxPower.Increment = new decimal(new int[] {
            25,
            0,
            0,
            131072});
            this.numTxPower.Location = new System.Drawing.Point(138, 128);
            this.numTxPower.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            131072});
            this.numTxPower.Minimum = new decimal(new int[] {
            1500,
            0,
            0,
            131072});
            this.numTxPower.Name = "numTxPower";
            this.numTxPower.Size = new System.Drawing.Size(158, 20);
            this.numTxPower.TabIndex = 3;
            this.numTxPower.Value = new decimal(new int[] {
            3000,
            0,
            0,
            131072});
            this.numTxPower.ValueChanged += new System.EventHandler(this.numTxPower_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 163);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(115, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Capture Mode Prefilter ";
            // 
            // cbCapMode
            // 
            this.cbCapMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCapMode.FormattingEnabled = true;
            this.cbCapMode.Items.AddRange(new object[] {
            "Time Window"});
            this.cbCapMode.Location = new System.Drawing.Point(138, 163);
            this.cbCapMode.Name = "cbCapMode";
            this.cbCapMode.Size = new System.Drawing.Size(158, 21);
            this.cbCapMode.TabIndex = 4;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(17, 234);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Window Time";
            // 
            // numWindowTime
            // 
            this.numWindowTime.Location = new System.Drawing.Point(243, 240);
            this.numWindowTime.Maximum = new decimal(new int[] {
            1800000,
            0,
            0,
            0});
            this.numWindowTime.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numWindowTime.Name = "numWindowTime";
            this.numWindowTime.Size = new System.Drawing.Size(241, 20);
            this.numWindowTime.TabIndex = 6;
            this.numWindowTime.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(17, 247);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(168, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Duplicate Elimination Time  (msec)";
            // 
            // chkEnable
            // 
            this.chkEnable.AutoSize = true;
            this.chkEnable.Location = new System.Drawing.Point(338, 25);
            this.chkEnable.Name = "chkEnable";
            this.chkEnable.Size = new System.Drawing.Size(146, 17);
            this.chkEnable.TabIndex = 7;
            this.chkEnable.Text = "Operation Profile Enabled";
            this.chkEnable.UseVisualStyleBackColor = true;
            // 
            // chkAnt1
            // 
            this.chkAnt1.AutoSize = true;
            this.chkAnt1.Location = new System.Drawing.Point(338, 63);
            this.chkAnt1.Name = "chkAnt1";
            this.chkAnt1.Size = new System.Drawing.Size(72, 17);
            this.chkAnt1.TabIndex = 8;
            this.chkAnt1.Text = "Antenna1";
            this.chkAnt1.UseVisualStyleBackColor = true;
            // 
            // chkAnt2
            // 
            this.chkAnt2.AutoSize = true;
            this.chkAnt2.Location = new System.Drawing.Point(338, 95);
            this.chkAnt2.Name = "chkAnt2";
            this.chkAnt2.Size = new System.Drawing.Size(72, 17);
            this.chkAnt2.TabIndex = 9;
            this.chkAnt2.Text = "Antenna2";
            this.chkAnt2.UseVisualStyleBackColor = true;
            // 
            // chkAnt3
            // 
            this.chkAnt3.AutoSize = true;
            this.chkAnt3.Location = new System.Drawing.Point(338, 129);
            this.chkAnt3.Name = "chkAnt3";
            this.chkAnt3.Size = new System.Drawing.Size(72, 17);
            this.chkAnt3.TabIndex = 10;
            this.chkAnt3.Text = "Antenna3";
            this.chkAnt3.UseVisualStyleBackColor = true;
            // 
            // chkAnt4
            // 
            this.chkAnt4.AutoSize = true;
            this.chkAnt4.Location = new System.Drawing.Point(338, 162);
            this.chkAnt4.Name = "chkAnt4";
            this.chkAnt4.Size = new System.Drawing.Size(72, 17);
            this.chkAnt4.TabIndex = 11;
            this.chkAnt4.Text = "Antenna4";
            this.chkAnt4.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cbTrigger);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.chkAnt4);
            this.groupBox1.Controls.Add(this.chkAnt3);
            this.groupBox1.Controls.Add(this.chkAnt2);
            this.groupBox1.Controls.Add(this.chkAnt1);
            this.groupBox1.Controls.Add(this.chkEnable);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.numWindowTime);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.cbCapMode);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.numTxPower);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.cbSession);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numPopEst);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cbModProfile);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(14, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(540, 280);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operation Profile Details";
            // 
            // cbTrigger
            // 
            this.cbTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbTrigger.FormattingEnabled = true;
            this.cbTrigger.Items.AddRange(new object[] {
            "Autonomous Time Trigger",
            "Polling Trigger by Client"});
            this.cbTrigger.Location = new System.Drawing.Point(243, 199);
            this.cbTrigger.Name = "cbTrigger";
            this.cbTrigger.Size = new System.Drawing.Size(241, 21);
            this.cbTrigger.TabIndex = 5;
            this.cbTrigger.SelectedIndexChanged += new System.EventHandler(this.cbTrigger_SelectedIndexChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 202);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(197, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Duplicate Elimination Triggering Method ";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(13, 348);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(117, 27);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(160, 348);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(117, 27);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(307, 348);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(117, 27);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmOperationProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(574, 388);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cbProfileID);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmOperationProfile";
            this.Text = "Operation Profile";
            this.Load += new System.EventHandler(this.OperationProfile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numPopEst)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTxPower)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numWindowTime)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbProfileID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbModProfile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numPopEst;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbSession;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numTxPower;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbCapMode;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numWindowTime;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox chkEnable;
        private System.Windows.Forms.CheckBox chkAnt1;
        private System.Windows.Forms.CheckBox chkAnt2;
        private System.Windows.Forms.CheckBox chkAnt3;
        private System.Windows.Forms.CheckBox chkAnt4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ComboBox cbTrigger;
        private System.Windows.Forms.Label label9;
    }
}