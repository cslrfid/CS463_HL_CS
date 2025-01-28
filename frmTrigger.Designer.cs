namespace CS463_HL_CS
{
    partial class frmTrigger
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
            this.cbID = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cbMode = new System.Windows.Forms.ComboBox();
            this.chkAnt1 = new System.Windows.Forms.CheckBox();
            this.chkAnt2 = new System.Windows.Forms.CheckBox();
            this.chkAnt3 = new System.Windows.Forms.CheckBox();
            this.chkAnt4 = new System.Windows.Forms.CheckBox();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.gAntenna = new System.Windows.Forms.GroupBox();
            this.gTimeout = new System.Windows.Forms.GroupBox();
            this.numTimeout = new System.Windows.Forms.NumericUpDown();
            this.gSensor = new System.Windows.Forms.GroupBox();
            this.cbLevel = new System.Windows.Forms.ComboBox();
            this.cbSensor = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.gAntenna.SuspendLayout();
            this.gTimeout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).BeginInit();
            this.gSensor.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "ID";
            // 
            // cbID
            // 
            this.cbID.FormattingEnabled = true;
            this.cbID.Location = new System.Drawing.Point(97, 29);
            this.cbID.Name = "cbID";
            this.cbID.Size = new System.Drawing.Size(321, 21);
            this.cbID.TabIndex = 1;
            this.cbID.SelectionChangeCommitted += new System.EventHandler(this.cbID_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Description";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(97, 61);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(321, 20);
            this.txtDesc.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Mode";
            // 
            // cbMode
            // 
            this.cbMode.FormattingEnabled = true;
            this.cbMode.Items.AddRange(new object[] {
            "Read Any Tags (any ID, 1 trigger per tag)",
            "Input Sensor State",
            "No Tag Read in Specified Time Span",
            "No Tag Read during Inventory Enabling Cycle",
            "Read Any Tags (any ID, 1 trigger on first tag of inventory enabling cycle)",
            "Read Any Tags (any ID, 1 trigger at the end of inventory enabling cycle)"});
            this.cbMode.Location = new System.Drawing.Point(98, 92);
            this.cbMode.Name = "cbMode";
            this.cbMode.Size = new System.Drawing.Size(320, 21);
            this.cbMode.TabIndex = 5;
            this.cbMode.SelectedIndexChanged += new System.EventHandler(this.cbMode_SelectedIndexChanged);
            // 
            // chkAnt1
            // 
            this.chkAnt1.AutoSize = true;
            this.chkAnt1.Location = new System.Drawing.Point(28, 26);
            this.chkAnt1.Name = "chkAnt1";
            this.chkAnt1.Size = new System.Drawing.Size(72, 17);
            this.chkAnt1.TabIndex = 6;
            this.chkAnt1.Text = "Antenna1";
            this.chkAnt1.UseVisualStyleBackColor = true;
            // 
            // chkAnt2
            // 
            this.chkAnt2.AutoSize = true;
            this.chkAnt2.Location = new System.Drawing.Point(28, 49);
            this.chkAnt2.Name = "chkAnt2";
            this.chkAnt2.Size = new System.Drawing.Size(72, 17);
            this.chkAnt2.TabIndex = 7;
            this.chkAnt2.Text = "Antenna2";
            this.chkAnt2.UseVisualStyleBackColor = true;
            // 
            // chkAnt3
            // 
            this.chkAnt3.AutoSize = true;
            this.chkAnt3.Location = new System.Drawing.Point(28, 72);
            this.chkAnt3.Name = "chkAnt3";
            this.chkAnt3.Size = new System.Drawing.Size(72, 17);
            this.chkAnt3.TabIndex = 8;
            this.chkAnt3.Text = "Antenna3";
            this.chkAnt3.UseVisualStyleBackColor = true;
            // 
            // chkAnt4
            // 
            this.chkAnt4.AutoSize = true;
            this.chkAnt4.Location = new System.Drawing.Point(28, 95);
            this.chkAnt4.Name = "chkAnt4";
            this.chkAnt4.Size = new System.Drawing.Size(72, 17);
            this.chkAnt4.TabIndex = 9;
            this.chkAnt4.Text = "Antenna4";
            this.chkAnt4.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(25, 396);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(90, 28);
            this.btnUpdate.TabIndex = 10;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(126, 396);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(90, 28);
            this.btnRemove.TabIndex = 11;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(227, 396);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 28);
            this.btnRefresh.TabIndex = 12;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(328, 396);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(90, 28);
            this.btnClose.TabIndex = 13;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Time (second):";
            // 
            // gAntenna
            // 
            this.gAntenna.Controls.Add(this.chkAnt1);
            this.gAntenna.Controls.Add(this.chkAnt2);
            this.gAntenna.Controls.Add(this.chkAnt3);
            this.gAntenna.Controls.Add(this.chkAnt4);
            this.gAntenna.Location = new System.Drawing.Point(25, 119);
            this.gAntenna.Name = "gAntenna";
            this.gAntenna.Size = new System.Drawing.Size(393, 124);
            this.gAntenna.TabIndex = 15;
            this.gAntenna.TabStop = false;
            this.gAntenna.Text = "Select Antenna";
            // 
            // gTimeout
            // 
            this.gTimeout.Controls.Add(this.numTimeout);
            this.gTimeout.Controls.Add(this.label4);
            this.gTimeout.Location = new System.Drawing.Point(25, 314);
            this.gTimeout.Name = "gTimeout";
            this.gTimeout.Size = new System.Drawing.Size(393, 65);
            this.gTimeout.TabIndex = 16;
            this.gTimeout.TabStop = false;
            this.gTimeout.Text = "Time span";
            // 
            // numTimeout
            // 
            this.numTimeout.Location = new System.Drawing.Point(106, 25);
            this.numTimeout.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numTimeout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTimeout.Name = "numTimeout";
            this.numTimeout.Size = new System.Drawing.Size(122, 20);
            this.numTimeout.TabIndex = 15;
            this.numTimeout.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // gSensor
            // 
            this.gSensor.Controls.Add(this.cbLevel);
            this.gSensor.Controls.Add(this.cbSensor);
            this.gSensor.Controls.Add(this.label6);
            this.gSensor.Controls.Add(this.label5);
            this.gSensor.Location = new System.Drawing.Point(25, 249);
            this.gSensor.Name = "gSensor";
            this.gSensor.Size = new System.Drawing.Size(393, 59);
            this.gSensor.TabIndex = 17;
            this.gSensor.TabStop = false;
            this.gSensor.Text = "Input Sensors";
            // 
            // cbLevel
            // 
            this.cbLevel.FormattingEnabled = true;
            this.cbLevel.Items.AddRange(new object[] {
            "Low",
            "High"});
            this.cbLevel.Location = new System.Drawing.Point(260, 19);
            this.cbLevel.Name = "cbLevel";
            this.cbLevel.Size = new System.Drawing.Size(98, 21);
            this.cbLevel.TabIndex = 3;
            // 
            // cbSensor
            // 
            this.cbSensor.FormattingEnabled = true;
            this.cbSensor.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4"});
            this.cbSensor.Location = new System.Drawing.Point(80, 21);
            this.cbSensor.Name = "cbSensor";
            this.cbSensor.Size = new System.Drawing.Size(98, 21);
            this.cbSensor.TabIndex = 2;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(207, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Level:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "Sensor:";
            // 
            // frmTrigger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 461);
            this.Controls.Add(this.gSensor);
            this.Controls.Add(this.gTimeout);
            this.Controls.Add(this.gAntenna);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.cbMode);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbID);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmTrigger";
            this.Text = "Trigger";
            this.Load += new System.EventHandler(this.frmTrigger_Load);
            this.gAntenna.ResumeLayout(false);
            this.gAntenna.PerformLayout();
            this.gTimeout.ResumeLayout(false);
            this.gTimeout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeout)).EndInit();
            this.gSensor.ResumeLayout(false);
            this.gSensor.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbID;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDesc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbMode;
        private System.Windows.Forms.CheckBox chkAnt1;
        private System.Windows.Forms.CheckBox chkAnt2;
        private System.Windows.Forms.CheckBox chkAnt3;
        private System.Windows.Forms.CheckBox chkAnt4;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gAntenna;
        private System.Windows.Forms.GroupBox gTimeout;
        private System.Windows.Forms.NumericUpDown numTimeout;
        private System.Windows.Forms.GroupBox gSensor;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cbLevel;
        private System.Windows.Forms.ComboBox cbSensor;
    }
}