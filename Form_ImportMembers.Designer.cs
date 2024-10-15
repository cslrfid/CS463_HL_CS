namespace CS463_HL_CS
{
    partial class Form_ImportMembers
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
            this.button_AddAll = new System.Windows.Forms.Button();
            this.button_Close = new System.Windows.Forms.Button();
            this.comboBox_tagGroups = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_EPCID = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_AddAll
            // 
            this.button_AddAll.Location = new System.Drawing.Point(158, 303);
            this.button_AddAll.Name = "button_AddAll";
            this.button_AddAll.Size = new System.Drawing.Size(75, 23);
            this.button_AddAll.TabIndex = 1;
            this.button_AddAll.Text = "Add All";
            this.button_AddAll.UseVisualStyleBackColor = true;
            this.button_AddAll.Click += new System.EventHandler(this.button_AddAll_Click);
            // 
            // button_Close
            // 
            this.button_Close.Location = new System.Drawing.Point(309, 303);
            this.button_Close.Name = "button_Close";
            this.button_Close.Size = new System.Drawing.Size(75, 23);
            this.button_Close.TabIndex = 2;
            this.button_Close.Text = "Close";
            this.button_Close.UseVisualStyleBackColor = true;
            this.button_Close.Click += new System.EventHandler(this.button_Close_Click);
            // 
            // comboBox_tagGroups
            // 
            this.comboBox_tagGroups.FormattingEnabled = true;
            this.comboBox_tagGroups.Location = new System.Drawing.Point(220, 37);
            this.comboBox_tagGroups.Name = "comboBox_tagGroups";
            this.comboBox_tagGroups.Size = new System.Drawing.Size(121, 21);
            this.comboBox_tagGroups.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(116, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "TagGroup ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(132, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "EPC ID";
            // 
            // textBox_EPCID
            // 
            this.textBox_EPCID.Location = new System.Drawing.Point(221, 82);
            this.textBox_EPCID.Multiline = true;
            this.textBox_EPCID.Name = "textBox_EPCID";
            this.textBox_EPCID.Size = new System.Drawing.Size(206, 192);
            this.textBox_EPCID.TabIndex = 6;
            // 
            // Form_ImportMembers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 364);
            this.Controls.Add(this.textBox_EPCID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_tagGroups);
            this.Controls.Add(this.button_Close);
            this.Controls.Add(this.button_AddAll);
            this.Name = "Form_ImportMembers";
            this.Text = "Add TagGroup Member";
            this.Load += new System.EventHandler(this.Form_ImportMembers_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_AddAll;
        private System.Windows.Forms.Button button_Close;
        private System.Windows.Forms.ComboBox comboBox_tagGroups;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_EPCID;
    }
}