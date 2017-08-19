namespace sonOmeter
{
	public partial class frmVolWeight
	{
		#region Form Variables
		private System.Windows.Forms.HScrollBar hsValue0;
        private System.Windows.Forms.Label lbValue0;
		private System.Windows.Forms.Label lbValue1;
        private System.Windows.Forms.HScrollBar hsValue1;
		private System.Windows.Forms.Label lbValue2;
        private System.Windows.Forms.HScrollBar hsValue2;
		private System.Windows.Forms.Label lbValue3;
        private System.Windows.Forms.HScrollBar hsValue3;
		private System.Windows.Forms.Label lbValue4;
        private System.Windows.Forms.HScrollBar hsValue4;
		private System.Windows.Forms.Label lbValue5;
        private System.Windows.Forms.HScrollBar hsValue5;
		private System.Windows.Forms.Label lbValue6;
        private System.Windows.Forms.HScrollBar hsValue6;
		private System.Windows.Forms.CheckBox checkBoxActivate;
        private System.Windows.Forms.TrackBar trackDepth;
		#endregion
        private System.Windows.Forms.CheckBox cbStopStrongLayer;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmVolWeight));
            this.hsValue0 = new System.Windows.Forms.HScrollBar();
            this.lbValue0 = new System.Windows.Forms.Label();
            this.lbValue1 = new System.Windows.Forms.Label();
            this.hsValue1 = new System.Windows.Forms.HScrollBar();
            this.lbValue2 = new System.Windows.Forms.Label();
            this.hsValue2 = new System.Windows.Forms.HScrollBar();
            this.lbValue3 = new System.Windows.Forms.Label();
            this.hsValue3 = new System.Windows.Forms.HScrollBar();
            this.lbValue4 = new System.Windows.Forms.Label();
            this.hsValue4 = new System.Windows.Forms.HScrollBar();
            this.lbValue5 = new System.Windows.Forms.Label();
            this.hsValue5 = new System.Windows.Forms.HScrollBar();
            this.lbValue6 = new System.Windows.Forms.Label();
            this.hsValue6 = new System.Windows.Forms.HScrollBar();
            this.checkBoxActivate = new System.Windows.Forms.CheckBox();
            this.trackDepth = new System.Windows.Forms.TrackBar();
            this.cbStopStrongLayer = new System.Windows.Forms.CheckBox();
            this.tbDepth = new System.Windows.Forms.TextBox();
            this.numericUpDownDepth = new System.Windows.Forms.NumericUpDown();
            this.cb3D = new System.Windows.Forms.CheckBox();
            this.trackSurface = new System.Windows.Forms.TrackBar();
            this.numericUpDownSurface = new System.Windows.Forms.NumericUpDown();
            this.tbSurface = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3D2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label3D = new System.Windows.Forms.Label();
            this.sonarCCBC6 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBH6 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBC5 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBH5 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBC4 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBH4 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBC3 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBH3 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBC2 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBH2 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBC1 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBH1 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBC0 = new sonOmeter.Controls.SonarColorCheckBox();
            this.sonarCCBH0 = new sonOmeter.Controls.SonarColorCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDepth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSurface)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSurface)).BeginInit();
            this.SuspendLayout();
            // 
            // hsValue0
            // 
            this.hsValue0.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hsValue0.Location = new System.Drawing.Point(78, 32);
            this.hsValue0.Maximum = 109;
            this.hsValue0.Name = "hsValue0";
            this.hsValue0.Size = new System.Drawing.Size(215, 17);
            this.hsValue0.TabIndex = 1;
            // 
            // lbValue0
            // 
            this.lbValue0.Location = new System.Drawing.Point(50, 32);
            this.lbValue0.Name = "lbValue0";
            this.lbValue0.Size = new System.Drawing.Size(25, 17);
            this.lbValue0.TabIndex = 2;
            this.lbValue0.Text = "100";
            this.lbValue0.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbValue1
            // 
            this.lbValue1.Location = new System.Drawing.Point(50, 56);
            this.lbValue1.Name = "lbValue1";
            this.lbValue1.Size = new System.Drawing.Size(25, 16);
            this.lbValue1.TabIndex = 6;
            this.lbValue1.Text = "100";
            this.lbValue1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // hsValue1
            // 
            this.hsValue1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hsValue1.Location = new System.Drawing.Point(78, 56);
            this.hsValue1.Maximum = 109;
            this.hsValue1.Name = "hsValue1";
            this.hsValue1.Size = new System.Drawing.Size(215, 17);
            this.hsValue1.TabIndex = 5;
            // 
            // lbValue2
            // 
            this.lbValue2.Location = new System.Drawing.Point(50, 80);
            this.lbValue2.Name = "lbValue2";
            this.lbValue2.Size = new System.Drawing.Size(25, 16);
            this.lbValue2.TabIndex = 9;
            this.lbValue2.Text = "100";
            this.lbValue2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // hsValue2
            // 
            this.hsValue2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hsValue2.Location = new System.Drawing.Point(78, 80);
            this.hsValue2.Maximum = 109;
            this.hsValue2.Name = "hsValue2";
            this.hsValue2.Size = new System.Drawing.Size(215, 17);
            this.hsValue2.TabIndex = 8;
            // 
            // lbValue3
            // 
            this.lbValue3.Location = new System.Drawing.Point(50, 104);
            this.lbValue3.Name = "lbValue3";
            this.lbValue3.Size = new System.Drawing.Size(25, 16);
            this.lbValue3.TabIndex = 12;
            this.lbValue3.Text = "100";
            this.lbValue3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // hsValue3
            // 
            this.hsValue3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hsValue3.Location = new System.Drawing.Point(78, 104);
            this.hsValue3.Maximum = 109;
            this.hsValue3.Name = "hsValue3";
            this.hsValue3.Size = new System.Drawing.Size(215, 17);
            this.hsValue3.TabIndex = 11;
            // 
            // lbValue4
            // 
            this.lbValue4.Location = new System.Drawing.Point(50, 128);
            this.lbValue4.Name = "lbValue4";
            this.lbValue4.Size = new System.Drawing.Size(25, 16);
            this.lbValue4.TabIndex = 15;
            this.lbValue4.Text = "100";
            this.lbValue4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // hsValue4
            // 
            this.hsValue4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hsValue4.Location = new System.Drawing.Point(78, 128);
            this.hsValue4.Maximum = 109;
            this.hsValue4.Name = "hsValue4";
            this.hsValue4.Size = new System.Drawing.Size(215, 17);
            this.hsValue4.TabIndex = 14;
            // 
            // lbValue5
            // 
            this.lbValue5.Location = new System.Drawing.Point(50, 152);
            this.lbValue5.Name = "lbValue5";
            this.lbValue5.Size = new System.Drawing.Size(25, 16);
            this.lbValue5.TabIndex = 18;
            this.lbValue5.Text = "100";
            this.lbValue5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // hsValue5
            // 
            this.hsValue5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hsValue5.Location = new System.Drawing.Point(78, 152);
            this.hsValue5.Maximum = 109;
            this.hsValue5.Name = "hsValue5";
            this.hsValue5.Size = new System.Drawing.Size(215, 17);
            this.hsValue5.TabIndex = 17;
            // 
            // lbValue6
            // 
            this.lbValue6.Location = new System.Drawing.Point(50, 176);
            this.lbValue6.Name = "lbValue6";
            this.lbValue6.Size = new System.Drawing.Size(25, 16);
            this.lbValue6.TabIndex = 21;
            this.lbValue6.Text = "100";
            this.lbValue6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // hsValue6
            // 
            this.hsValue6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.hsValue6.Location = new System.Drawing.Point(78, 176);
            this.hsValue6.Maximum = 109;
            this.hsValue6.Name = "hsValue6";
            this.hsValue6.Size = new System.Drawing.Size(215, 17);
            this.hsValue6.TabIndex = 20;
            // 
            // checkBoxActivate
            // 
            this.checkBoxActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxActivate.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBoxActivate.BackColor = System.Drawing.SystemColors.Control;
            this.checkBoxActivate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBoxActivate.Location = new System.Drawing.Point(180, 5);
            this.checkBoxActivate.Name = "checkBoxActivate";
            this.checkBoxActivate.Size = new System.Drawing.Size(54, 24);
            this.checkBoxActivate.TabIndex = 33;
            this.checkBoxActivate.Text = "ARCH";
            this.checkBoxActivate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxActivate.UseVisualStyleBackColor = false;
            this.checkBoxActivate.CheckedChanged += new System.EventHandler(this.checkBoxActivate_CheckedChanged);
            // 
            // trackDepth
            // 
            this.trackDepth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackDepth.LargeChange = 10;
            this.trackDepth.Location = new System.Drawing.Point(78, 196);
            this.trackDepth.Maximum = 30;
            this.trackDepth.Name = "trackDepth";
            this.trackDepth.Size = new System.Drawing.Size(215, 45);
            this.trackDepth.TabIndex = 34;
            this.trackDepth.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // cbStopStrongLayer
            // 
            this.cbStopStrongLayer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbStopStrongLayer.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbStopStrongLayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbStopStrongLayer.Location = new System.Drawing.Point(239, 5);
            this.cbStopStrongLayer.Name = "cbStopStrongLayer";
            this.cbStopStrongLayer.Size = new System.Drawing.Size(54, 24);
            this.cbStopStrongLayer.TabIndex = 39;
            this.cbStopStrongLayer.Text = "SATL";
            this.cbStopStrongLayer.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbStopStrongLayer.UseVisualStyleBackColor = false;
            this.cbStopStrongLayer.CheckedChanged += new System.EventHandler(this.cbStopStrongLayer_CheckedChanged);
            // 
            // tbDepth
            // 
            this.tbDepth.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbDepth.Location = new System.Drawing.Point(30, 211);
            this.tbDepth.Name = "tbDepth";
            this.tbDepth.Size = new System.Drawing.Size(29, 13);
            this.tbDepth.TabIndex = 32;
            this.tbDepth.Text = "0.1";
            // 
            // numericUpDownDepth
            // 
            this.numericUpDownDepth.Location = new System.Drawing.Point(28, 208);
            this.numericUpDownDepth.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownDepth.Name = "numericUpDownDepth";
            this.numericUpDownDepth.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownDepth.TabIndex = 40;
            // 
            // cb3D
            // 
            this.cb3D.AutoSize = true;
            this.cb3D.Location = new System.Drawing.Point(77, 10);
            this.cb3D.Name = "cb3D";
            this.cb3D.Size = new System.Drawing.Size(40, 17);
            this.cb3D.TabIndex = 41;
            this.cb3D.Text = "3D";
            this.cb3D.UseVisualStyleBackColor = true;
            this.cb3D.CheckedChanged += new System.EventHandler(this.cb3D_CheckedChanged);
            // 
            // trackSurface
            // 
            this.trackSurface.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackSurface.LargeChange = 10;
            this.trackSurface.Location = new System.Drawing.Point(78, 247);
            this.trackSurface.Maximum = 30;
            this.trackSurface.Name = "trackSurface";
            this.trackSurface.Size = new System.Drawing.Size(215, 45);
            this.trackSurface.TabIndex = 34;
            this.trackSurface.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // numericUpDownSurface
            // 
            this.numericUpDownSurface.Location = new System.Drawing.Point(28, 259);
            this.numericUpDownSurface.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDownSurface.Name = "numericUpDownSurface";
            this.numericUpDownSurface.Size = new System.Drawing.Size(47, 20);
            this.numericUpDownSurface.TabIndex = 40;
            // 
            // tbSurface
            // 
            this.tbSurface.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbSurface.Location = new System.Drawing.Point(30, 262);
            this.tbSurface.Name = "tbSurface";
            this.tbSurface.Size = new System.Drawing.Size(29, 13);
            this.tbSurface.TabIndex = 32;
            this.tbSurface.Text = "0.1";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "H";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3D2
            // 
            this.label3D2.Location = new System.Drawing.Point(28, 9);
            this.label3D2.Name = "label3D2";
            this.label3D2.Size = new System.Drawing.Size(16, 16);
            this.label3D2.TabIndex = 2;
            this.label3D2.Text = "C";
            this.label3D2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "H";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3D
            // 
            this.label3D.Location = new System.Drawing.Point(6, 259);
            this.label3D.Name = "label3D";
            this.label3D.Size = new System.Drawing.Size(16, 16);
            this.label3D.TabIndex = 2;
            this.label3D.Text = "C";
            this.label3D.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonarCCBC6
            // 
            this.sonarCCBC6.AutoSize = true;
            this.sonarCCBC6.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBC6.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBC6.Location = new System.Drawing.Point(28, 176);
            this.sonarCCBC6.Name = "sonarCCBC6";
            this.sonarCCBC6.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBC6.TabIndex = 43;
            this.sonarCCBC6.Text = "sonarCCBC7";
            this.sonarCCBC6.UseVisualStyleBackColor = false;
            // 
            // sonarCCBH6
            // 
            this.sonarCCBH6.AutoSize = true;
            this.sonarCCBH6.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBH6.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBH6.Location = new System.Drawing.Point(6, 176);
            this.sonarCCBH6.Name = "sonarCCBH6";
            this.sonarCCBH6.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBH6.TabIndex = 43;
            this.sonarCCBH6.Text = "sonarColorCheckBox1";
            this.sonarCCBH6.UseVisualStyleBackColor = false;
            // 
            // sonarCCBC5
            // 
            this.sonarCCBC5.AutoSize = true;
            this.sonarCCBC5.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBC5.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBC5.Location = new System.Drawing.Point(28, 152);
            this.sonarCCBC5.Name = "sonarCCBC5";
            this.sonarCCBC5.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBC5.TabIndex = 43;
            this.sonarCCBC5.Text = "sonarCCBC6";
            this.sonarCCBC5.UseVisualStyleBackColor = false;
            // 
            // sonarCCBH5
            // 
            this.sonarCCBH5.AutoSize = true;
            this.sonarCCBH5.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBH5.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBH5.Location = new System.Drawing.Point(6, 152);
            this.sonarCCBH5.Name = "sonarCCBH5";
            this.sonarCCBH5.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBH5.TabIndex = 43;
            this.sonarCCBH5.Text = "sonarColorCheckBox1";
            this.sonarCCBH5.UseVisualStyleBackColor = false;
            // 
            // sonarCCBC4
            // 
            this.sonarCCBC4.AutoSize = true;
            this.sonarCCBC4.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBC4.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBC4.Location = new System.Drawing.Point(28, 128);
            this.sonarCCBC4.Name = "sonarCCBC4";
            this.sonarCCBC4.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBC4.TabIndex = 43;
            this.sonarCCBC4.Text = "sonarCCBC5";
            this.sonarCCBC4.UseVisualStyleBackColor = false;
            // 
            // sonarCCBH4
            // 
            this.sonarCCBH4.AutoSize = true;
            this.sonarCCBH4.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBH4.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBH4.Location = new System.Drawing.Point(6, 128);
            this.sonarCCBH4.Name = "sonarCCBH4";
            this.sonarCCBH4.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBH4.TabIndex = 43;
            this.sonarCCBH4.Text = "sonarColorCheckBox1";
            this.sonarCCBH4.UseVisualStyleBackColor = false;
            // 
            // sonarCCBC3
            // 
            this.sonarCCBC3.AutoSize = true;
            this.sonarCCBC3.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBC3.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBC3.Location = new System.Drawing.Point(28, 104);
            this.sonarCCBC3.Name = "sonarCCBC3";
            this.sonarCCBC3.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBC3.TabIndex = 43;
            this.sonarCCBC3.Text = "sonarCCBC4";
            this.sonarCCBC3.UseVisualStyleBackColor = false;
            // 
            // sonarCCBH3
            // 
            this.sonarCCBH3.AutoSize = true;
            this.sonarCCBH3.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBH3.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBH3.Location = new System.Drawing.Point(6, 104);
            this.sonarCCBH3.Name = "sonarCCBH3";
            this.sonarCCBH3.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBH3.TabIndex = 43;
            this.sonarCCBH3.Text = "sonarColorCheckBox1";
            this.sonarCCBH3.UseVisualStyleBackColor = false;
            // 
            // sonarCCBC2
            // 
            this.sonarCCBC2.AutoSize = true;
            this.sonarCCBC2.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBC2.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBC2.Location = new System.Drawing.Point(28, 80);
            this.sonarCCBC2.Name = "sonarCCBC2";
            this.sonarCCBC2.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBC2.TabIndex = 43;
            this.sonarCCBC2.Text = "sonarCCBC3";
            this.sonarCCBC2.UseVisualStyleBackColor = false;
            // 
            // sonarCCBH2
            // 
            this.sonarCCBH2.AutoSize = true;
            this.sonarCCBH2.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBH2.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBH2.Location = new System.Drawing.Point(6, 80);
            this.sonarCCBH2.Name = "sonarCCBH2";
            this.sonarCCBH2.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBH2.TabIndex = 43;
            this.sonarCCBH2.Text = "sonarColorCheckBox1";
            this.sonarCCBH2.UseVisualStyleBackColor = false;
            // 
            // sonarCCBC1
            // 
            this.sonarCCBC1.AutoSize = true;
            this.sonarCCBC1.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBC1.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBC1.Location = new System.Drawing.Point(28, 56);
            this.sonarCCBC1.Name = "sonarCCBC1";
            this.sonarCCBC1.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBC1.TabIndex = 43;
            this.sonarCCBC1.Text = "sonarCCBC2";
            this.sonarCCBC1.UseVisualStyleBackColor = false;
            // 
            // sonarCCBH1
            // 
            this.sonarCCBH1.AutoSize = true;
            this.sonarCCBH1.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBH1.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBH1.Location = new System.Drawing.Point(6, 56);
            this.sonarCCBH1.Name = "sonarCCBH1";
            this.sonarCCBH1.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBH1.TabIndex = 43;
            this.sonarCCBH1.Text = "sonarColorCheckBox1";
            this.sonarCCBH1.UseVisualStyleBackColor = false;
            // 
            // sonarCCBC0
            // 
            this.sonarCCBC0.AutoSize = true;
            this.sonarCCBC0.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBC0.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBC0.Location = new System.Drawing.Point(28, 32);
            this.sonarCCBC0.Name = "sonarCCBC0";
            this.sonarCCBC0.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBC0.TabIndex = 43;
            this.sonarCCBC0.Text = "sonarCCBC1";
            this.sonarCCBC0.UseVisualStyleBackColor = false;
            // 
            // sonarCCBH0
            // 
            this.sonarCCBH0.AutoSize = true;
            this.sonarCCBH0.BackColor = System.Drawing.Color.Lime;
            this.sonarCCBH0.ForeColor = System.Drawing.Color.Black;
            this.sonarCCBH0.Location = new System.Drawing.Point(6, 32);
            this.sonarCCBH0.Name = "sonarCCBH0";
            this.sonarCCBH0.Size = new System.Drawing.Size(16, 16);
            this.sonarCCBH0.TabIndex = 43;
            this.sonarCCBH0.Text = "sonarColorCheckBox1";
            this.sonarCCBH0.UseVisualStyleBackColor = false;
            // 
            // frmVolWeight
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(299, 299);
            this.Controls.Add(this.sonarCCBC6);
            this.Controls.Add(this.sonarCCBH6);
            this.Controls.Add(this.sonarCCBC5);
            this.Controls.Add(this.sonarCCBH5);
            this.Controls.Add(this.sonarCCBC4);
            this.Controls.Add(this.sonarCCBH4);
            this.Controls.Add(this.sonarCCBC3);
            this.Controls.Add(this.sonarCCBH3);
            this.Controls.Add(this.sonarCCBC2);
            this.Controls.Add(this.sonarCCBH2);
            this.Controls.Add(this.sonarCCBC1);
            this.Controls.Add(this.sonarCCBH1);
            this.Controls.Add(this.sonarCCBC0);
            this.Controls.Add(this.sonarCCBH0);
            this.Controls.Add(this.cb3D);
            this.Controls.Add(this.tbSurface);
            this.Controls.Add(this.numericUpDownSurface);
            this.Controls.Add(this.tbDepth);
            this.Controls.Add(this.numericUpDownDepth);
            this.Controls.Add(this.trackSurface);
            this.Controls.Add(this.cbStopStrongLayer);
            this.Controls.Add(this.trackDepth);
            this.Controls.Add(this.checkBoxActivate);
            this.Controls.Add(this.lbValue6);
            this.Controls.Add(this.hsValue6);
            this.Controls.Add(this.lbValue5);
            this.Controls.Add(this.hsValue5);
            this.Controls.Add(this.lbValue4);
            this.Controls.Add(this.hsValue4);
            this.Controls.Add(this.lbValue3);
            this.Controls.Add(this.hsValue3);
            this.Controls.Add(this.lbValue2);
            this.Controls.Add(this.hsValue2);
            this.Controls.Add(this.lbValue1);
            this.Controls.Add(this.hsValue1);
            this.Controls.Add(this.label3D);
            this.Controls.Add(this.label3D2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbValue0);
            this.Controls.Add(this.hsValue0);
            this.DockType = DockDotNET.DockContainerType.ToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(315, 386);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(315, 288);
            this.Name = "frmVolWeight";
            this.Text = "Volume Weights & Virtual Archaeology";
            this.Load += new System.EventHandler(this.frmVolWeight_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDepth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackSurface)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownSurface)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private System.Windows.Forms.TextBox tbDepth;
        private System.Windows.Forms.NumericUpDown numericUpDownDepth;
        private System.Windows.Forms.CheckBox cb3D;
        private System.Windows.Forms.TrackBar trackSurface;
        private System.Windows.Forms.NumericUpDown numericUpDownSurface;
        private System.Windows.Forms.TextBox tbSurface;
        private Controls.SonarColorCheckBox sonarCCBH0;
        private Controls.SonarColorCheckBox sonarCCBH1;
        private Controls.SonarColorCheckBox sonarCCBH2;
        private Controls.SonarColorCheckBox sonarCCBH3;
        private Controls.SonarColorCheckBox sonarCCBH4;
        private Controls.SonarColorCheckBox sonarCCBH5;
        private Controls.SonarColorCheckBox sonarCCBH6;
        private Controls.SonarColorCheckBox sonarCCBC0;
        private Controls.SonarColorCheckBox sonarCCBC1;
        private Controls.SonarColorCheckBox sonarCCBC2;
        private Controls.SonarColorCheckBox sonarCCBC3;
        private Controls.SonarColorCheckBox sonarCCBC4;
        private Controls.SonarColorCheckBox sonarCCBC5;
        private Controls.SonarColorCheckBox sonarCCBC6;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3D2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label3D;


    }
}
