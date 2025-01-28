namespace CS463_HL_CS
{
    partial class frmReaderID
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
            this.txtReaderID = new System.Windows.Forms.TextBox();
            this.btnSetReaderID = new System.Windows.Forms.Button();
            this.btnGet = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Reader ID";
            // 
            // txtReaderID
            // 
            this.txtReaderID.Location = new System.Drawing.Point(140, 22);
            this.txtReaderID.Name = "txtReaderID";
            this.txtReaderID.Size = new System.Drawing.Size(378, 20);
            this.txtReaderID.TabIndex = 1;
            // 
            // btnSetReaderID
            // 
            this.btnSetReaderID.Location = new System.Drawing.Point(211, 91);
            this.btnSetReaderID.Name = "btnSetReaderID";
            this.btnSetReaderID.Size = new System.Drawing.Size(141, 29);
            this.btnSetReaderID.TabIndex = 2;
            this.btnSetReaderID.Text = "Update Reader ID";
            this.btnSetReaderID.UseVisualStyleBackColor = true;
            this.btnSetReaderID.Click += new System.EventHandler(this.btnSetReaderID_Click);
            // 
            // btnGet
            // 
            this.btnGet.Location = new System.Drawing.Point(377, 91);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(141, 29);
            this.btnGet.TabIndex = 3;
            this.btnGet.Text = "Get Reader ID";
            this.btnGet.UseVisualStyleBackColor = true;
            this.btnGet.Click += new System.EventHandler(this.btnGet_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Reader Description";
            // 
            // txtDesc
            // 
            this.txtDesc.Location = new System.Drawing.Point(140, 53);
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.Size = new System.Drawing.Size(378, 20);
            this.txtDesc.TabIndex = 5;
            // 
            // frmReaderID
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 147);
            this.Controls.Add(this.txtDesc);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnGet);
            this.Controls.Add(this.btnSetReaderID);
            this.Controls.Add(this.txtReaderID);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "frmReaderID";
            this.Text = "ReaderID";
            this.Load += new System.EventHandler(this.frmReaderID_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtReaderID;
        private System.Windows.Forms.Button btnSetReaderID;
        private System.Windows.Forms.Button btnGet;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDesc;
    }
}