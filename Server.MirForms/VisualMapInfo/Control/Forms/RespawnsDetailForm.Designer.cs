﻿namespace Server.MirForms.VisualMapInfo.Control.Forms
{
    partial class RespawnsDetailForm
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
            this.Spread = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Y = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.DoneButton = new System.Windows.Forms.Button();
            this.X = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Delay = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Count = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.RoutePath = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Direction = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.RDelay = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Spread
            // 
            this.Spread.Location = new System.Drawing.Point(71, 59);
            this.Spread.Name = "Spread";
            this.Spread.Size = new System.Drawing.Size(53, 21);
            this.Spread.TabIndex = 13;
            this.Spread.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Chk);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 12;
            this.label3.Text = "拓展:";
            // 
            // Y
            // 
            this.Y.Location = new System.Drawing.Point(71, 35);
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(53, 21);
            this.Y.TabIndex = 11;
            this.Y.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Chk);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "Y:";
            // 
            // DoneButton
            // 
            this.DoneButton.Location = new System.Drawing.Point(179, 120);
            this.DoneButton.Name = "DoneButton";
            this.DoneButton.Size = new System.Drawing.Size(105, 22);
            this.DoneButton.TabIndex = 9;
            this.DoneButton.Text = "完成";
            this.DoneButton.UseVisualStyleBackColor = true;
            this.DoneButton.Click += new System.EventHandler(this.DoneButton_Click);
            // 
            // X
            // 
            this.X.Location = new System.Drawing.Point(71, 11);
            this.X.Name = "X";
            this.X.Size = new System.Drawing.Size(53, 21);
            this.X.TabIndex = 8;
            this.X.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Chk);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "X:";
            // 
            // Delay
            // 
            this.Delay.Location = new System.Drawing.Point(184, 35);
            this.Delay.Name = "Delay";
            this.Delay.Size = new System.Drawing.Size(53, 21);
            this.Delay.TabIndex = 17;
            this.Delay.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Chk);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(141, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 12);
            this.label4.TabIndex = 16;
            this.label4.Text = "延迟:";
            // 
            // Count
            // 
            this.Count.Location = new System.Drawing.Point(184, 11);
            this.Count.Name = "Count";
            this.Count.Size = new System.Drawing.Size(53, 21);
            this.Count.TabIndex = 15;
            this.Count.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Chk);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(140, 14);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 12);
            this.label5.TabIndex = 14;
            this.label5.Text = "数量:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(243, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 12);
            this.label6.TabIndex = 18;
            this.label6.Text = "(分)";
            // 
            // RoutePath
            // 
            this.RoutePath.Location = new System.Drawing.Point(184, 83);
            this.RoutePath.Name = "RoutePath";
            this.RoutePath.Size = new System.Drawing.Size(100, 21);
            this.RoutePath.TabIndex = 19;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(141, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 12);
            this.label7.TabIndex = 20;
            this.label7.Text = "路线:";
            // 
            // Direction
            // 
            this.Direction.Location = new System.Drawing.Point(71, 83);
            this.Direction.Name = "Direction";
            this.Direction.Size = new System.Drawing.Size(53, 21);
            this.Direction.TabIndex = 21;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(35, 86);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 12);
            this.label8.TabIndex = 22;
            this.label8.Text = "显示:";
            // 
            // RDelay
            // 
            this.RDelay.Location = new System.Drawing.Point(184, 59);
            this.RDelay.Name = "RDelay";
            this.RDelay.Size = new System.Drawing.Size(53, 21);
            this.RDelay.TabIndex = 23;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(133, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(41, 12);
            this.label9.TabIndex = 24;
            this.label9.Text = "R延迟:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(243, 62);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 12);
            this.label10.TabIndex = 25;
            this.label10.Text = "(分)";
            // 
            // RespawnsDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 153);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.RDelay);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.Direction);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.RoutePath);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.Delay);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Count);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.Spread);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.Y);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DoneButton);
            this.Controls.Add(this.X);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "RespawnsDetailForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Respawns";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox Spread;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.TextBox Y;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button DoneButton;
        public System.Windows.Forms.TextBox X;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox Delay;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.TextBox Count;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        public System.Windows.Forms.TextBox RoutePath;
        private System.Windows.Forms.Label label7;
        public System.Windows.Forms.TextBox Direction;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.TextBox RDelay;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
    }
}