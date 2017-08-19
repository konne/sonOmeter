namespace sonOmeter
{	
	 partial class frmRecord
	{
        #region Windows Form Designer generated code

        private System.ComponentModel.IContainer components  = null;
        private System.Windows.Forms.Button btnTopUp;
        private System.Windows.Forms.Button btnTopDown;
        private System.Windows.Forms.Button btnBottomDown;
		 private System.Windows.Forms.Button btnBottomUp;
        private System.Windows.Forms.Timer changeTimer;
        private sonOmeter.SonarDepthMeter panControl;
        private sonOmeter.SonarPanel panNF;
        private sonOmeter.SonarPanel panHF;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;        
		private sonOmeter.SonarStatusBar sonarStatusBar;
		private sonOmeter.SlideBar sbFile;
		 private sonOmeter.SlideBar sbDepth;
		private System.Windows.Forms.Label labHorZoom;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRecord));
            this.labHorZoom = new System.Windows.Forms.Label();
            this.cmsHorZoom = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiZoom50 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiZoom100 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiZoom200 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiFitPage = new System.Windows.Forms.ToolStripMenuItem();
            this.cbMainWindow = new System.Windows.Forms.CheckBox();
            this.imgListMainWnd = new System.Windows.Forms.ImageList(this.components);
            this.btnTopUp = new System.Windows.Forms.Button();
            this.btnTopDown = new System.Windows.Forms.Button();
            this.btnBottomDown = new System.Windows.Forms.Button();
            this.btnBottomUp = new System.Windows.Forms.Button();
            this.changeTimer = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.cmsPopup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSurfLine = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSurfLineOp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCalcLine = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiCalcLineOp = new System.Windows.Forms.ToolStripMenuItem();
            this.panNF = new sonOmeter.SonarPanel();
            this.panControl = new sonOmeter.SonarDepthMeter();
            this.sbDepth = new sonOmeter.SlideBar();
            this.sbFile = new sonOmeter.SlideBar();
            this.panHF = new sonOmeter.SonarPanel();
            this.sonarInfoBar = new sonOmeter.SonarInfoBar();
            this.sonarStatusBar = new sonOmeter.SonarStatusBar();
            this.cmsHorZoom.SuspendLayout();
            this.cmsPopup.SuspendLayout();
            this.SuspendLayout();
            // 
            // labHorZoom
            // 
            this.labHorZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labHorZoom.BackColor = System.Drawing.Color.Blue;
            this.labHorZoom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labHorZoom.ContextMenuStrip = this.cmsHorZoom;
            this.labHorZoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labHorZoom.ForeColor = System.Drawing.Color.White;
            this.labHorZoom.Location = new System.Drawing.Point(0, 514);
            this.labHorZoom.Name = "labHorZoom";
            this.labHorZoom.Size = new System.Drawing.Size(48, 24);
            this.labHorZoom.TabIndex = 6;
            this.labHorZoom.Text = "100 %";
            this.labHorZoom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmsHorZoom
            // 
            this.cmsHorZoom.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiZoom50,
            this.tsmiZoom100,
            this.tsmiZoom200,
            this.toolStripSeparator1,
            this.tsmiFitPage});
            this.cmsHorZoom.Name = "cmsHorZoom";
            this.cmsHorZoom.Size = new System.Drawing.Size(117, 98);
            // 
            // tsmiZoom50
            // 
            this.tsmiZoom50.Name = "tsmiZoom50";
            this.tsmiZoom50.Size = new System.Drawing.Size(116, 22);
            this.tsmiZoom50.Text = "50%";
            this.tsmiZoom50.Click += new System.EventHandler(this.tsmiZoom50_Click);
            // 
            // tsmiZoom100
            // 
            this.tsmiZoom100.Name = "tsmiZoom100";
            this.tsmiZoom100.Size = new System.Drawing.Size(116, 22);
            this.tsmiZoom100.Text = "100%";
            this.tsmiZoom100.Click += new System.EventHandler(this.tsmiZoom100_Click);
            // 
            // tsmiZoom200
            // 
            this.tsmiZoom200.Name = "tsmiZoom200";
            this.tsmiZoom200.Size = new System.Drawing.Size(116, 22);
            this.tsmiZoom200.Text = "200%";
            this.tsmiZoom200.Click += new System.EventHandler(this.tsmiZoom200_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(113, 6);
            // 
            // tsmiFitPage
            // 
            this.tsmiFitPage.Name = "tsmiFitPage";
            this.tsmiFitPage.Size = new System.Drawing.Size(116, 22);
            this.tsmiFitPage.Text = "Fit page";
            this.tsmiFitPage.Click += new System.EventHandler(this.tsmiFitPage_Click);
            // 
            // cbMainWindow
            // 
            this.cbMainWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMainWindow.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbMainWindow.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbMainWindow.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbMainWindow.ImageIndex = 0;
            this.cbMainWindow.ImageList = this.imgListMainWnd;
            this.cbMainWindow.Location = new System.Drawing.Point(537, 514);
            this.cbMainWindow.Name = "cbMainWindow";
            this.cbMainWindow.Size = new System.Drawing.Size(24, 24);
            this.cbMainWindow.TabIndex = 8;
            this.cbMainWindow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbMainWindow.CheckedChanged += new System.EventHandler(this.cbMainWindow_CheckedChanged);
            // 
            // imgListMainWnd
            // 
            this.imgListMainWnd.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgListMainWnd.ImageStream")));
            this.imgListMainWnd.TransparentColor = System.Drawing.Color.White;
            this.imgListMainWnd.Images.SetKeyName(0, "");
            this.imgListMainWnd.Images.SetKeyName(1, "");
            // 
            // btnTopUp
            // 
            this.btnTopUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTopUp.Location = new System.Drawing.Point(0, 0);
            this.btnTopUp.Name = "btnTopUp";
            this.btnTopUp.Size = new System.Drawing.Size(24, 24);
            this.btnTopUp.TabIndex = 9;
            this.btnTopUp.Text = "+";
            this.btnTopUp.Click += new System.EventHandler(this.btnTopUp_Click);
            // 
            // btnTopDown
            // 
            this.btnTopDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTopDown.Location = new System.Drawing.Point(0, 24);
            this.btnTopDown.Name = "btnTopDown";
            this.btnTopDown.Size = new System.Drawing.Size(24, 24);
            this.btnTopDown.TabIndex = 9;
            this.btnTopDown.Text = "-";
            this.btnTopDown.Click += new System.EventHandler(this.btnTopDown_Click);
            // 
            // btnBottomDown
            // 
            this.btnBottomDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBottomDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBottomDown.Location = new System.Drawing.Point(0, 466);
            this.btnBottomDown.Name = "btnBottomDown";
            this.btnBottomDown.Size = new System.Drawing.Size(24, 24);
            this.btnBottomDown.TabIndex = 9;
            this.btnBottomDown.Text = "-";
            this.btnBottomDown.Click += new System.EventHandler(this.btnBottomDown_Click);
            // 
            // btnBottomUp
            // 
            this.btnBottomUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBottomUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBottomUp.Location = new System.Drawing.Point(0, 442);
            this.btnBottomUp.Name = "btnBottomUp";
            this.btnBottomUp.Size = new System.Drawing.Size(24, 24);
            this.btnBottomUp.TabIndex = 9;
            this.btnBottomUp.Text = "+";
            this.btnBottomUp.Click += new System.EventHandler(this.btnBottomUp_Click);
            // 
            // changeTimer
            // 
            this.changeTimer.Enabled = true;
            this.changeTimer.Interval = 500;
            this.changeTimer.Tick += new System.EventHandler(this.OnChangeTimer);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Title = "Save sceen shot";
            // 
            // cmsPopup
            // 
            this.cmsPopup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSurfLine,
            this.tsmiSurfLineOp,
            this.tsmiCalcLine,
            this.tsmiCalcLineOp});
            this.cmsPopup.Name = "cmsPopup";
            this.cmsPopup.Size = new System.Drawing.Size(173, 92);
            this.cmsPopup.Opening += new System.ComponentModel.CancelEventHandler(this.cmsPopup_Opening);
            // 
            // tsmiSurfLine
            // 
            this.tsmiSurfLine.Name = "tsmiSurfLine";
            this.tsmiSurfLine.Size = new System.Drawing.Size(172, 22);
            this.tsmiSurfLine.Text = "Surface line";
            this.tsmiSurfLine.Click += new System.EventHandler(this.tsmiSurfLine_Click);
            // 
            // tsmiSurfLineOp
            // 
            this.tsmiSurfLineOp.Name = "tsmiSurfLineOp";
            this.tsmiSurfLineOp.Size = new System.Drawing.Size(172, 22);
            this.tsmiSurfLineOp.Text = "Surface line op.";
            this.tsmiSurfLineOp.Click += new System.EventHandler(this.tsmiSurfLineOp_Click);
            // 
            // tsmiCalcLine
            // 
            this.tsmiCalcLine.Name = "tsmiCalcLine";
            this.tsmiCalcLine.Size = new System.Drawing.Size(172, 22);
            this.tsmiCalcLine.Text = "Calculated line";
            this.tsmiCalcLine.Click += new System.EventHandler(this.tsmiCalcLine_Click);
            // 
            // tsmiCalcLineOp
            // 
            this.tsmiCalcLineOp.Name = "tsmiCalcLineOp";
            this.tsmiCalcLineOp.Size = new System.Drawing.Size(172, 22);
            this.tsmiCalcLineOp.Text = "Calculated line op.";
            this.tsmiCalcLineOp.Click += new System.EventHandler(this.tsmiCalcLineOp_Click);
            // 
            // panNF
            // 
            this.panNF.AltHold = false;
            this.panNF.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panNF.BackColor = System.Drawing.Color.Blue;
            this.panNF.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panNF.ContextMenuStrip = this.cmsPopup;
            this.panNF.CtrlHold = false;
            this.panNF.Cursor = System.Windows.Forms.Cursors.Default;
            this.panNF.CutMouseMode = sonOmeter.CutMouseMode.Normal;
            this.panNF.DepthMeter = this.panControl;
            this.panNF.DepthSlideBar = this.sbDepth;
            this.panNF.FileSlideBar = this.sbFile;
            this.panNF.ForeColor = System.Drawing.Color.White;
            this.panNF.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panNF.IsCut = true;
            this.panNF.Location = new System.Drawing.Point(120, 208);
            this.panNF.Margin = new System.Windows.Forms.Padding(0);
            this.panNF.Name = "panNF";
            this.panNF.PanelType = sonOmeter.Classes.SonarPanelType.NF;
            this.panNF.PlaceSomething = sonOmeter.PlaceMode.Nothing;
            this.panNF.ShiftHold = false;
            this.panNF.ShowCalcLHF = false;
            this.panNF.ShowCalcLNF = false;
            this.panNF.ShowPos = false;
            this.panNF.ShowRul = false;
            this.panNF.ShowSurfLHF = false;
            this.panNF.ShowSurfLNF = false;
            this.panNF.ShowVol = false;
            this.panNF.Size = new System.Drawing.Size(441, 282);
            this.panNF.TabIndex = 12;
            this.panNF.WorkLine = null;
            this.panNF.WorkLinePos = 32;
            this.panNF.WorkLineWidth = 20;
            this.panNF.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMoveNF);
            // 
            // panControl
            // 
            this.panControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.panControl.BackColor = System.Drawing.Color.Blue;
            this.panControl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panControl.BorderX = 5;
            this.panControl.BorderYl = 50;
            this.panControl.BorderYr = 1;
            this.panControl.BottomWidth = 0;
            this.panControl.DepthZoomHi = 0;
            this.panControl.DepthZoomLo = -100;
            this.panControl.ForeColor = System.Drawing.Color.White;
            this.panControl.Location = new System.Drawing.Point(24, 0);
            this.panControl.Margin = new System.Windows.Forms.Padding(0);
            this.panControl.Name = "panControl";
            this.panControl.PanelYRatio = 0.5;
            this.panControl.ShowHF = true;
            this.panControl.ShowNF = true;
            this.panControl.Size = new System.Drawing.Size(95, 490);
            this.panControl.TabIndex = 10;
            // 
            // sbDepth
            // 
            this.sbDepth.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.sbDepth.BackColor = System.Drawing.SystemColors.Control;
            this.sbDepth.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sbDepth.BorderX = 2;
            this.sbDepth.BorderY = 2;
            this.sbDepth.Cursor = System.Windows.Forms.Cursors.Default;
            this.sbDepth.DragColor = System.Drawing.Color.DimGray;
            this.sbDepth.ForeColor = System.Drawing.Color.Gray;
            this.sbDepth.HoverColor = System.Drawing.Color.DarkGray;
            this.sbDepth.KeepRatio = false;
            this.sbDepth.Location = new System.Drawing.Point(0, 48);
            this.sbDepth.Name = "sbDepth";
            this.sbDepth.Pitch = 0.01;
            this.sbDepth.RegionWidth = 100;
            this.sbDepth.SectionEnd = 100;
            this.sbDepth.SectionStart = 0;
            this.sbDepth.SectionWidth = 100;
            this.sbDepth.SectionWidthMin = 1;
            this.sbDepth.Size = new System.Drawing.Size(24, 394);
            this.sbDepth.SlideAnchor = sonOmeter.SlideBar.SlideAnchors.Width;
            this.sbDepth.TabIndex = 5;
            this.sbDepth.Vertical = true;
            this.sbDepth.WidthOffset = 0;
            this.sbDepth.OnZoom += new sonOmeter.ZoomEventHandler(this.ZoomDepth);
            // 
            // sbFile
            // 
            this.sbFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sbFile.BackColor = System.Drawing.SystemColors.Control;
            this.sbFile.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sbFile.BorderX = 2;
            this.sbFile.BorderY = 2;
            this.sbFile.Cursor = System.Windows.Forms.Cursors.Default;
            this.sbFile.DragColor = System.Drawing.Color.DimGray;
            this.sbFile.ForeColor = System.Drawing.Color.Gray;
            this.sbFile.HoverColor = System.Drawing.Color.DarkGray;
            this.sbFile.KeepRatio = true;
            this.sbFile.Location = new System.Drawing.Point(168, 490);
            this.sbFile.Name = "sbFile";
            this.sbFile.Pitch = 1;
            this.sbFile.RegionWidth = 100;
            this.sbFile.SectionEnd = 213.58695652173913;
            this.sbFile.SectionStart = 0;
            this.sbFile.SectionWidth = 213.58695652173913;
            this.sbFile.SectionWidthMin = 50;
            this.sbFile.Size = new System.Drawing.Size(393, 24);
            this.sbFile.SlideAnchor = sonOmeter.SlideBar.SlideAnchors.Width;
            this.sbFile.TabIndex = 1;
            this.sbFile.Vertical = false;
            this.sbFile.WidthOffset = 49;
            // 
            // panHF
            // 
            this.panHF.AltHold = false;
            this.panHF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panHF.BackColor = System.Drawing.Color.Blue;
            this.panHF.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panHF.ContextMenuStrip = this.cmsPopup;
            this.panHF.CtrlHold = false;
            this.panHF.Cursor = System.Windows.Forms.Cursors.Default;
            this.panHF.CutMouseMode = sonOmeter.CutMouseMode.Normal;
            this.panHF.DepthMeter = this.panControl;
            this.panHF.DepthSlideBar = this.sbDepth;
            this.panHF.FileSlideBar = this.sbFile;
            this.panHF.ForeColor = System.Drawing.Color.White;
            this.panHF.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.panHF.IsCut = true;
            this.panHF.Location = new System.Drawing.Point(120, 0);
            this.panHF.Margin = new System.Windows.Forms.Padding(0);
            this.panHF.Name = "panHF";
            this.panHF.PanelType = sonOmeter.Classes.SonarPanelType.HF;
            this.panHF.PlaceSomething = sonOmeter.PlaceMode.Nothing;
            this.panHF.ShiftHold = false;
            this.panHF.ShowCalcLHF = false;
            this.panHF.ShowCalcLNF = false;
            this.panHF.ShowPos = false;
            this.panHF.ShowRul = false;
            this.panHF.ShowSurfLHF = false;
            this.panHF.ShowSurfLNF = false;
            this.panHF.ShowVol = false;
            this.panHF.Size = new System.Drawing.Size(441, 208);
            this.panHF.TabIndex = 11;
            this.panHF.WorkLine = null;
            this.panHF.WorkLinePos = 32;
            this.panHF.WorkLineWidth = 20;
            this.panHF.MouseMove += new System.Windows.Forms.MouseEventHandler(this.OnMouseMoveHF);
            // 
            // sonarInfoBar
            // 
            this.sonarInfoBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sonarInfoBar.BackColor = System.Drawing.Color.Blue;
            this.sonarInfoBar.ColorOff = System.Drawing.Color.Gray;
            this.sonarInfoBar.ColorOn = System.Drawing.Color.White;
            this.sonarInfoBar.CutMode = sonOmeter.Classes.CutMode.Nothing;
            this.sonarInfoBar.EditMode = sonOmeter.EditModes.Nothing;
            this.sonarInfoBar.ForeColor = System.Drawing.Color.White;
            this.sonarInfoBar.LabelText = "";
            this.sonarInfoBar.Location = new System.Drawing.Point(48, 514);
            this.sonarInfoBar.Name = "sonarInfoBar";
            this.sonarInfoBar.ShowVol = false;
            this.sonarInfoBar.Size = new System.Drawing.Size(489, 24);
            this.sonarInfoBar.TabIndex = 7;
            this.sonarInfoBar.ToggleVol += new System.EventHandler(this.ToggleVol);
            // 
            // sonarStatusBar
            // 
            this.sonarStatusBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sonarStatusBar.BackColor = System.Drawing.Color.Blue;
            this.sonarStatusBar.ColorOff = System.Drawing.Color.Gray;
            this.sonarStatusBar.ColorOn = System.Drawing.Color.White;
            this.sonarStatusBar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sonarStatusBar.ForeColor = System.Drawing.Color.White;
            this.sonarStatusBar.IsCut = true;
            this.sonarStatusBar.Location = new System.Drawing.Point(0, 490);
            this.sonarStatusBar.Name = "sonarStatusBar";
            this.sonarStatusBar.ShowHF = true;
            this.sonarStatusBar.ShowNF = true;
            this.sonarStatusBar.ShowPos = false;
            this.sonarStatusBar.ShowRuler = false;
            this.sonarStatusBar.Size = new System.Drawing.Size(168, 24);
            this.sonarStatusBar.TabIndex = 4;
            this.sonarStatusBar.ToggleHF += new System.EventHandler(this.ToggleHF);
            this.sonarStatusBar.ToggleColor += new System.EventHandler(this.ToggleColor);
            this.sonarStatusBar.ToggleCUT += new System.EventHandler(this.ToggleCUT);
            this.sonarStatusBar.TogglePos += new System.EventHandler(this.TogglePos);
            this.sonarStatusBar.ToggleNF += new System.EventHandler(this.ToggleNF);
            this.sonarStatusBar.ToggleRuler += new System.EventHandler(this.ToggleRuler);
            // 
            // frmRecord
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(561, 538);
            this.Controls.Add(this.panNF);
            this.Controls.Add(this.panHF);
            this.Controls.Add(this.panControl);
            this.Controls.Add(this.btnTopUp);
            this.Controls.Add(this.sbFile);
            this.Controls.Add(this.sbDepth);
            this.Controls.Add(this.sonarInfoBar);
            this.Controls.Add(this.sonarStatusBar);
            this.Controls.Add(this.labHorZoom);
            this.Controls.Add(this.cbMainWindow);
            this.Controls.Add(this.btnTopDown);
            this.Controls.Add(this.btnBottomDown);
            this.Controls.Add(this.btnBottomUp);
            this.DockType = DockDotNET.DockContainerType.Document;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(456, 488);
            this.Name = "frmRecord";
            this.Text = "Record";            
            this.Closed += new System.EventHandler(this.frmRecord_Closed);
            this.VisibleChanged += new System.EventHandler(this.frmRecord_VisibleChanged);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmRecord_FormClosed);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmRecord_Closing);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmRecord_FormClosing);
            this.cmsHorZoom.ResumeLayout(false);
            this.cmsPopup.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion


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

		 private System.Windows.Forms.ContextMenuStrip cmsHorZoom;
		 private System.Windows.Forms.ToolStripMenuItem tsmiZoom50;
		 private System.Windows.Forms.ToolStripMenuItem tsmiZoom100;
		 private System.Windows.Forms.ToolStripMenuItem tsmiZoom200;
		 private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		 private System.Windows.Forms.ToolStripMenuItem tsmiFitPage;
		 private System.Windows.Forms.ContextMenuStrip cmsPopup;
		 private System.Windows.Forms.ToolStripMenuItem tsmiSurfLine;
		 private System.Windows.Forms.ToolStripMenuItem tsmiSurfLineOp;
		 private System.Windows.Forms.ToolStripMenuItem tsmiCalcLine;
		 private System.Windows.Forms.ToolStripMenuItem tsmiCalcLineOp;
	
	}
}
