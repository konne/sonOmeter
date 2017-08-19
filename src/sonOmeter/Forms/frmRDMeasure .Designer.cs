namespace sonOmeter
{
    partial class frmRDMeasure
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
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDiscard = new System.Windows.Forms.Button();
            this.tbPipelineDiameter = new System.Windows.Forms.TextBox();
            this.tbSeconds = new System.Windows.Forms.TextBox();
            this.tbStation = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbDepth = new System.Windows.Forms.TextBox();
            this.tbDistance = new System.Windows.Forms.TextBox();
            this.tbTimeDiff = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tbAltitude = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.tbAntDepth = new System.Windows.Forms.TextBox();
            this.tbAntLength = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAdd
            // 
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(12, 227);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "Add";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDiscard
            // 
            this.btnDiscard.Location = new System.Drawing.Point(124, 227);
            this.btnDiscard.Name = "btnDiscard";
            this.btnDiscard.Size = new System.Drawing.Size(75, 23);
            this.btnDiscard.TabIndex = 1;
            this.btnDiscard.Text = "Discard";
            this.btnDiscard.UseVisualStyleBackColor = true;
            this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
            // 
            // tbPipelineDiameter
            // 
            this.tbPipelineDiameter.Location = new System.Drawing.Point(73, 6);
            this.tbPipelineDiameter.Name = "tbPipelineDiameter";
            this.tbPipelineDiameter.Size = new System.Drawing.Size(42, 20);
            this.tbPipelineDiameter.TabIndex = 2;
            this.tbPipelineDiameter.Text = "0";
            // 
            // tbSeconds
            // 
            this.tbSeconds.Location = new System.Drawing.Point(179, 6);
            this.tbSeconds.Name = "tbSeconds";
            this.tbSeconds.Size = new System.Drawing.Size(45, 20);
            this.tbSeconds.TabIndex = 3;
            this.tbSeconds.Text = "9.2";
            this.tbSeconds.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbStation
            // 
            this.tbStation.Enabled = false;
            this.tbStation.Location = new System.Drawing.Point(73, 94);
            this.tbStation.Name = "tbStation";
            this.tbStation.Size = new System.Drawing.Size(100, 20);
            this.tbStation.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Diameter:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(121, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Seconds:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 97);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Station:";
            // 
            // tbDepth
            // 
            this.tbDepth.Enabled = false;
            this.tbDepth.Location = new System.Drawing.Point(73, 149);
            this.tbDepth.Name = "tbDepth";
            this.tbDepth.Size = new System.Drawing.Size(100, 20);
            this.tbDepth.TabIndex = 8;
            // 
            // tbDistance
            // 
            this.tbDistance.Enabled = false;
            this.tbDistance.Location = new System.Drawing.Point(73, 120);
            this.tbDistance.Name = "tbDistance";
            this.tbDistance.Size = new System.Drawing.Size(100, 20);
            this.tbDistance.TabIndex = 9;
            // 
            // tbTimeDiff
            // 
            this.tbTimeDiff.Enabled = false;
            this.tbTimeDiff.Location = new System.Drawing.Point(73, 201);
            this.tbTimeDiff.Name = "tbTimeDiff";
            this.tbTimeDiff.Size = new System.Drawing.Size(100, 20);
            this.tbTimeDiff.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Distance:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 152);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(39, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Depth:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 204);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(50, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Timediff.:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 178);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(45, 13);
            this.label7.TabIndex = 15;
            this.label7.Text = "Altitude:";
            // 
            // tbAltitude
            // 
            this.tbAltitude.Enabled = false;
            this.tbAltitude.Location = new System.Drawing.Point(73, 175);
            this.tbAltitude.Name = "tbAltitude";
            this.tbAltitude.Size = new System.Drawing.Size(100, 20);
            this.tbAltitude.TabIndex = 14;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(108, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "Ant. Depth to Master:";
            // 
            // tbAntDepth
            // 
            this.tbAntDepth.BackColor = System.Drawing.SystemColors.Window;
            this.tbAntDepth.Enabled = false;
            this.tbAntDepth.Location = new System.Drawing.Point(115, 68);
            this.tbAntDepth.Name = "tbAntDepth";
            this.tbAntDepth.ReadOnly = true;
            this.tbAntDepth.Size = new System.Drawing.Size(58, 20);
            this.tbAntDepth.TabIndex = 17;
            this.tbAntDepth.Text = "0.0";
            this.tbAntDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // tbAntLength
            // 
            this.tbAntLength.Location = new System.Drawing.Point(179, 32);
            this.tbAntLength.Name = "tbAntLength";
            this.tbAntLength.Size = new System.Drawing.Size(45, 20);
            this.tbAntLength.TabIndex = 18;
            this.tbAntLength.Text = "4.66";
            this.tbAntLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(130, 38);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(43, 13);
            this.label9.TabIndex = 19;
            this.label9.Text = "Length:";
            // 
            // frmRDMeasure
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 256);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.tbAntLength);
            this.Controls.Add(this.tbAntDepth);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.tbAltitude);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbTimeDiff);
            this.Controls.Add(this.tbDistance);
            this.Controls.Add(this.tbDepth);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbStation);
            this.Controls.Add(this.tbSeconds);
            this.Controls.Add(this.tbPipelineDiameter);
            this.Controls.Add(this.btnDiscard);
            this.Controls.Add(this.btnAdd);
            this.DockType = DockDotNET.DockContainerType.ToolWindow;
            this.MinimumSize = new System.Drawing.Size(280, 290);
            this.Name = "frmRDMeasure";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "RDMeasure";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDiscard;
        private System.Windows.Forms.TextBox tbPipelineDiameter;
        private System.Windows.Forms.TextBox tbSeconds;
        private System.Windows.Forms.TextBox tbStation;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbDepth;
        private System.Windows.Forms.TextBox tbDistance;
        private System.Windows.Forms.TextBox tbTimeDiff;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox tbAltitude;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox tbAntDepth;
        private System.Windows.Forms.TextBox tbAntLength;
        private System.Windows.Forms.Label label9;
    }
}