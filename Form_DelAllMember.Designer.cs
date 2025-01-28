namespace CS463_HL_CS
{
    partial class Form_DelAllMember
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
            this.button_DelAllMember = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox_tagGroups = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_DelAllMember
            // 
            this.button_DelAllMember.Location = new System.Drawing.Point(187, 117);
            this.button_DelAllMember.Name = "button_DelAllMember";
            this.button_DelAllMember.Size = new System.Drawing.Size(75, 23);
            this.button_DelAllMember.TabIndex = 1;
            this.button_DelAllMember.Text = "Delete All";
            this.button_DelAllMember.UseVisualStyleBackColor = true;
            this.button_DelAllMember.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(316, 117);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Close";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox_tagGroups
            // 
            this.comboBox_tagGroups.FormattingEnabled = true;
            this.comboBox_tagGroups.Location = new System.Drawing.Point(278, 49);
            this.comboBox_tagGroups.Name = "comboBox_tagGroups";
            this.comboBox_tagGroups.Size = new System.Drawing.Size(121, 21);
            this.comboBox_tagGroups.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(174, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "TagGroup ID";
            // 
            // Form_DelAllMember
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(605, 208);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_tagGroups);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button_DelAllMember);
            this.Name = "Form_DelAllMember";
            this.Text = "Delete All Member";
            this.Load += new System.EventHandler(this.Form_DelAllMember_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button_DelAllMember;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ComboBox comboBox_tagGroups;
        private System.Windows.Forms.Label label1;
    }
}