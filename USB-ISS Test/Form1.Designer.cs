namespace USB_ISS_Test
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
            this.components = new System.ComponentModel.Container();
            this.USBISS_comboBox = new System.Windows.Forms.ComboBox();
            this.Mode_comboBox = new System.Windows.Forms.ComboBox();
            this.USBISStextBox = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // USBISS_comboBox
            // 
            this.USBISS_comboBox.FormattingEnabled = true;
            this.USBISS_comboBox.Location = new System.Drawing.Point(10, 11);
            this.USBISS_comboBox.Name = "USBISS_comboBox";
            this.USBISS_comboBox.Size = new System.Drawing.Size(121, 20);
            this.USBISS_comboBox.TabIndex = 0;
            this.USBISS_comboBox.SelectedIndexChanged += new System.EventHandler(this.USBISS_comboBox_SelectedIndexChanged);
            // 
            // Mode_comboBox
            // 
            this.Mode_comboBox.FormattingEnabled = true;
            this.Mode_comboBox.Location = new System.Drawing.Point(139, 11);
            this.Mode_comboBox.Name = "Mode_comboBox";
            this.Mode_comboBox.Size = new System.Drawing.Size(141, 20);
            this.Mode_comboBox.TabIndex = 1;
            this.Mode_comboBox.SelectedIndexChanged += new System.EventHandler(this.Mode_comboBox_SelectedIndexChanged);
            // 
            // USBISStextBox
            // 
            this.USBISStextBox.Location = new System.Drawing.Point(12, 81);
            this.USBISStextBox.Multiline = true;
            this.USBISStextBox.Name = "USBISStextBox";
            this.USBISStextBox.Size = new System.Drawing.Size(337, 184);
            this.USBISStextBox.TabIndex = 2;
            // 
            // timer1
            // 
            this.timer1.Enabled = false;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick_1);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Out 0",
            "Out 1",
            "Input",
            "Analog"});
            this.comboBox1.Location = new System.Drawing.Point(217, 45);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(63, 20);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "Out 0",
            "Out 1",
            "Input",
            "Analog"});
            this.comboBox2.Location = new System.Drawing.Point(148, 45);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(63, 20);
            this.comboBox2.TabIndex = 4;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // comboBox3
            // 
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "Out 0",
            "Out 1",
            "Input",
            "Analog"});
            this.comboBox3.Location = new System.Drawing.Point(79, 45);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(63, 20);
            this.comboBox3.TabIndex = 5;
            this.comboBox3.SelectedIndexChanged += new System.EventHandler(this.comboBox3_SelectedIndexChanged);
            // 
            // comboBox4
            // 
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "Out 0",
            "Out 1",
            "Input",
            "Analog"});
            this.comboBox4.Location = new System.Drawing.Point(10, 45);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(63, 20);
            this.comboBox4.TabIndex = 6;
            this.comboBox4.SelectedIndexChanged += new System.EventHandler(this.comboBox4_SelectedIndexChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(286, 42);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 285);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBox4);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.USBISStextBox);
            this.Controls.Add(this.Mode_comboBox);
            this.Controls.Add(this.USBISS_comboBox);
            this.Name = "Form1";
            this.Text = "USB-ISS Test";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox USBISS_comboBox;
        private System.Windows.Forms.ComboBox Mode_comboBox;
        private System.Windows.Forms.TextBox USBISStextBox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.Button button1;
    }
}

