namespace My_EMGU_Program
{
    partial class Form1
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
            this.image_PCBX = new System.Windows.Forms.PictureBox();
            this.Load_BTN = new System.Windows.Forms.Button();
            this.X_pos_LBL = new System.Windows.Forms.Label();
            this.Y_pos_LBL = new System.Windows.Forms.Label();
            this.Val_LBL = new System.Windows.Forms.Label();
            this.Convert_btn = new System.Windows.Forms.Button();
            this.Red_Spectrum_CHCK = new System.Windows.Forms.CheckBox();
            this.Green_Spectrum_CHCK = new System.Windows.Forms.CheckBox();
            this.Blue_Spectrum_CHCK = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.image_PCBX)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // image_PCBX
            // 
            this.image_PCBX.Location = new System.Drawing.Point(3, 3);
            this.image_PCBX.Name = "image_PCBX";
            this.image_PCBX.Size = new System.Drawing.Size(1033, 445);
            this.image_PCBX.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.image_PCBX.TabIndex = 0;
            this.image_PCBX.TabStop = false;
            this.image_PCBX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.image_PCBX_MouseMove);
            // 
            // Load_BTN
            // 
            this.Load_BTN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Load_BTN.Location = new System.Drawing.Point(605, 462);
            this.Load_BTN.Name = "Load_BTN";
            this.Load_BTN.Size = new System.Drawing.Size(82, 23);
            this.Load_BTN.TabIndex = 1;
            this.Load_BTN.Text = "Load";
            this.Load_BTN.UseVisualStyleBackColor = true;
            this.Load_BTN.Click += new System.EventHandler(this.Load_BTN_Click);
            // 
            // X_pos_LBL
            // 
            this.X_pos_LBL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.X_pos_LBL.AutoSize = true;
            this.X_pos_LBL.Location = new System.Drawing.Point(12, 466);
            this.X_pos_LBL.Name = "X_pos_LBL";
            this.X_pos_LBL.Size = new System.Drawing.Size(17, 13);
            this.X_pos_LBL.TabIndex = 2;
            this.X_pos_LBL.Text = "X:";
            // 
            // Y_pos_LBL
            // 
            this.Y_pos_LBL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Y_pos_LBL.AutoSize = true;
            this.Y_pos_LBL.Location = new System.Drawing.Point(84, 466);
            this.Y_pos_LBL.Name = "Y_pos_LBL";
            this.Y_pos_LBL.Size = new System.Drawing.Size(17, 13);
            this.Y_pos_LBL.TabIndex = 3;
            this.Y_pos_LBL.Text = "Y:";
            // 
            // Val_LBL
            // 
            this.Val_LBL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Val_LBL.AutoSize = true;
            this.Val_LBL.Location = new System.Drawing.Point(168, 466);
            this.Val_LBL.Name = "Val_LBL";
            this.Val_LBL.Size = new System.Drawing.Size(37, 13);
            this.Val_LBL.TabIndex = 4;
            this.Val_LBL.Text = "Value:";
            // 
            // Convert_btn
            // 
            this.Convert_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Convert_btn.Location = new System.Drawing.Point(693, 462);
            this.Convert_btn.Name = "Convert_btn";
            this.Convert_btn.Size = new System.Drawing.Size(103, 23);
            this.Convert_btn.TabIndex = 5;
            this.Convert_btn.Text = "Convert to Gray";
            this.Convert_btn.UseVisualStyleBackColor = true;
            this.Convert_btn.Click += new System.EventHandler(this.Convert_btn_Click);
            // 
            // Red_Spectrum_CHCK
            // 
            this.Red_Spectrum_CHCK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Red_Spectrum_CHCK.AutoSize = true;
            this.Red_Spectrum_CHCK.Enabled = false;
            this.Red_Spectrum_CHCK.Location = new System.Drawing.Point(802, 465);
            this.Red_Spectrum_CHCK.Name = "Red_Spectrum_CHCK";
            this.Red_Spectrum_CHCK.Size = new System.Drawing.Size(76, 17);
            this.Red_Spectrum_CHCK.TabIndex = 6;
            this.Red_Spectrum_CHCK.Text = "Show Red";
            this.Red_Spectrum_CHCK.UseVisualStyleBackColor = true;
            this.Red_Spectrum_CHCK.CheckedChanged += new System.EventHandler(this.Red_Spectrum_CHCK_CheckedChanged);
            // 
            // Green_Spectrum_CHCK
            // 
            this.Green_Spectrum_CHCK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Green_Spectrum_CHCK.AutoSize = true;
            this.Green_Spectrum_CHCK.Enabled = false;
            this.Green_Spectrum_CHCK.Location = new System.Drawing.Point(884, 465);
            this.Green_Spectrum_CHCK.Name = "Green_Spectrum_CHCK";
            this.Green_Spectrum_CHCK.Size = new System.Drawing.Size(85, 17);
            this.Green_Spectrum_CHCK.TabIndex = 7;
            this.Green_Spectrum_CHCK.Text = "Show Green";
            this.Green_Spectrum_CHCK.UseVisualStyleBackColor = true;
            this.Green_Spectrum_CHCK.CheckedChanged += new System.EventHandler(this.Green_Spectrum_CHCK_CheckedChanged);
            // 
            // Blue_Spectrum_CHCK
            // 
            this.Blue_Spectrum_CHCK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Blue_Spectrum_CHCK.AutoSize = true;
            this.Blue_Spectrum_CHCK.Enabled = false;
            this.Blue_Spectrum_CHCK.Location = new System.Drawing.Point(975, 465);
            this.Blue_Spectrum_CHCK.Name = "Blue_Spectrum_CHCK";
            this.Blue_Spectrum_CHCK.Size = new System.Drawing.Size(77, 17);
            this.Blue_Spectrum_CHCK.TabIndex = 8;
            this.Blue_Spectrum_CHCK.Text = "Show Blue";
            this.Blue_Spectrum_CHCK.UseVisualStyleBackColor = true;
            this.Blue_Spectrum_CHCK.CheckedChanged += new System.EventHandler(this.Blue_Spectrum_CHCK_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.image_PCBX);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1039, 451);
            this.panel1.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1063, 496);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.Blue_Spectrum_CHCK);
            this.Controls.Add(this.Green_Spectrum_CHCK);
            this.Controls.Add(this.Red_Spectrum_CHCK);
            this.Controls.Add(this.Convert_btn);
            this.Controls.Add(this.Val_LBL);
            this.Controls.Add(this.Y_pos_LBL);
            this.Controls.Add(this.X_pos_LBL);
            this.Controls.Add(this.Load_BTN);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.image_PCBX)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox image_PCBX;
        private System.Windows.Forms.Button Load_BTN;
        private System.Windows.Forms.Label X_pos_LBL;
        private System.Windows.Forms.Label Y_pos_LBL;
        private System.Windows.Forms.Label Val_LBL;
        private System.Windows.Forms.Button Convert_btn;
        private System.Windows.Forms.CheckBox Red_Spectrum_CHCK;
        private System.Windows.Forms.CheckBox Green_Spectrum_CHCK;
        private System.Windows.Forms.CheckBox Blue_Spectrum_CHCK;
        private System.Windows.Forms.Panel panel1;
    }
}

