namespace PiceaWindowsFormsApp
{
    partial class Config
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Config));
            this.button1 = new System.Windows.Forms.Button();
            this.b_pairing = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_ip = new System.Windows.Forms.TextBox();
            this.tb_port = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_pairingcode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(99, 292);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(12, 12);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // b_pairing
            // 
            this.b_pairing.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.b_pairing.Location = new System.Drawing.Point(0, 110);
            this.b_pairing.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.b_pairing.Name = "b_pairing";
            this.b_pairing.Size = new System.Drawing.Size(680, 35);
            this.b_pairing.TabIndex = 1;
            this.b_pairing.Text = "Connect This App";
            this.b_pairing.UseVisualStyleBackColor = true;
            this.b_pairing.Click += new System.EventHandler(this.b_pairing_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(93, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(28, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "IP:";
            // 
            // tb_ip
            // 
            this.tb_ip.Location = new System.Drawing.Point(132, 18);
            this.tb_ip.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tb_ip.Name = "tb_ip";
            this.tb_ip.Size = new System.Drawing.Size(298, 26);
            this.tb_ip.TabIndex = 3;
            this.tb_ip.Text = "picea-P00000.local";
            // 
            // tb_port
            // 
            this.tb_port.Location = new System.Drawing.Point(512, 18);
            this.tb_port.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tb_port.Name = "tb_port";
            this.tb_port.Size = new System.Drawing.Size(148, 26);
            this.tb_port.TabIndex = 4;
            this.tb_port.Text = "8080";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(459, 23);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Port:";
            // 
            // tb_pairingcode
            // 
            this.tb_pairingcode.Location = new System.Drawing.Point(132, 58);
            this.tb_pairingcode.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tb_pairingcode.Name = "tb_pairingcode";
            this.tb_pairingcode.Size = new System.Drawing.Size(528, 26);
            this.tb_pairingcode.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 63);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 20);
            this.label3.TabIndex = 7;
            this.label3.Text = "Pairing Code:";
            // 
            // Config
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 145);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_pairingcode);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_port);
            this.Controls.Add(this.tb_ip);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.b_pairing);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Config";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Config";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button b_pairing;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_ip;
        private System.Windows.Forms.TextBox tb_port;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_pairingcode;
        private System.Windows.Forms.Label label3;
    }
}