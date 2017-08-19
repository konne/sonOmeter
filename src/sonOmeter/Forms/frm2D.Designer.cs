namespace sonOmeter
{
	public partial class frm2D
	{
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (globalEventHandler != null)
			{				
                sonOmeter.Classes.GlobalNotifier.SignOut(globalEventHandler, sonOmeter.Classes.GlobalNotifier.MsgTypes.NewCoordinate);
			}

			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

        private sonOmeter.Sonar2DView sonar2DView;
		private sonOmeter.EditBar editBar;
		private System.Windows.Forms.ImageList ilPin;
		private System.Windows.Forms.CheckBox cbDoCoordChange;
        private System.Windows.Forms.Label labDisplay;
        private System.Windows.Forms.Label labViewMode;
        private System.Windows.Forms.Label labCorridor;
		private System.Windows.Forms.Timer beepTimer;
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm2D));
            UKLib.MathEx.RectangleD rectangleD1 = new UKLib.MathEx.RectangleD();
            UKLib.MathEx.RectangleD rectangleD2 = new UKLib.MathEx.RectangleD();
            this.cbDoCoordChange = new System.Windows.Forms.CheckBox();
            this.ilPin = new System.Windows.Forms.ImageList(this.components);
            this.labDisplay = new System.Windows.Forms.Label();
            this.labViewMode = new System.Windows.Forms.Label();
            this.labCorridor = new System.Windows.Forms.Label();
            this.beepTimer = new System.Windows.Forms.Timer(this.components);
            this.labInfo = new System.Windows.Forms.Label();
            this.labState = new System.Windows.Forms.Label();
            this.sonar2DView = new sonOmeter.Sonar2DView();
            this.editBar = new sonOmeter.EditBar();
            this.SuspendLayout();
            // 
            // cbDoCoordChange
            // 
            this.cbDoCoordChange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbDoCoordChange.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbDoCoordChange.BackColor = System.Drawing.SystemColors.Control;
            this.cbDoCoordChange.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cbDoCoordChange.ForeColor = System.Drawing.Color.White;
            this.cbDoCoordChange.ImageIndex = 0;
            this.cbDoCoordChange.ImageList = this.ilPin;
            this.cbDoCoordChange.Location = new System.Drawing.Point(590, 448);
            this.cbDoCoordChange.Name = "cbDoCoordChange";
            this.cbDoCoordChange.Size = new System.Drawing.Size(24, 24);
            this.cbDoCoordChange.TabIndex = 4;
            this.cbDoCoordChange.ThreeState = true;
            this.cbDoCoordChange.UseVisualStyleBackColor = false;
            this.cbDoCoordChange.CheckStateChanged += new System.EventHandler(this.cbDoCoordChange_CheckStateChanged);
            // 
            // ilPin
            // 
            this.ilPin.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilPin.ImageStream")));
            this.ilPin.TransparentColor = System.Drawing.Color.White;
            this.ilPin.Images.SetKeyName(0, "GridFixed.bmp");
            this.ilPin.Images.SetKeyName(1, "GridMove.bmp");
            this.ilPin.Images.SetKeyName(2, "GridMoveRotate.bmp");
            // 
            // labDisplay
            // 
            this.labDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labDisplay.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labDisplay.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labDisplay.Location = new System.Drawing.Point(124, 444);
            this.labDisplay.Margin = new System.Windows.Forms.Padding(0);
            this.labDisplay.Name = "labDisplay";
            this.labDisplay.Size = new System.Drawing.Size(32, 32);
            this.labDisplay.TabIndex = 2;
            this.labDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labViewMode
            // 
            this.labViewMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labViewMode.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labViewMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labViewMode.Location = new System.Drawing.Point(156, 444);
            this.labViewMode.Margin = new System.Windows.Forms.Padding(0);
            this.labViewMode.Name = "labViewMode";
            this.labViewMode.Size = new System.Drawing.Size(52, 32);
            this.labViewMode.TabIndex = 2;
            this.labViewMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labCorridor
            // 
            this.labCorridor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.labCorridor.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labCorridor.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labCorridor.Location = new System.Drawing.Point(470, 444);
            this.labCorridor.Margin = new System.Windows.Forms.Padding(0);
            this.labCorridor.Name = "labCorridor";
            this.labCorridor.Size = new System.Drawing.Size(115, 32);
            this.labCorridor.TabIndex = 2;
            this.labCorridor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labCorridor.Paint += new System.Windows.Forms.PaintEventHandler(this.labCorridor_Paint);
            // 
            // beepTimer
            // 
            this.beepTimer.Enabled = true;
            this.beepTimer.Interval = 500;
            this.beepTimer.Tick += new System.EventHandler(this.beepTimer_Tick);
            // 
            // labInfo
            // 
            this.labInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labInfo.Location = new System.Drawing.Point(0, 444);
            this.labInfo.Margin = new System.Windows.Forms.Padding(0);
            this.labInfo.Name = "labInfo";
            this.labInfo.Size = new System.Drawing.Size(124, 32);
            this.labInfo.TabIndex = 2;
            this.labInfo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // labState
            // 
            this.labState.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labState.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labState.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labState.Location = new System.Drawing.Point(208, 444);
            this.labState.Margin = new System.Windows.Forms.Padding(0);
            this.labState.Name = "labState";
            this.labState.Size = new System.Drawing.Size(32, 32);
            this.labState.TabIndex = 2;
            this.labState.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // sonar2DView
            // 
            this.sonar2DView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sonar2DView.BackColor = System.Drawing.Color.Blue;
            this.sonar2DView.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.sonar2DView.BorderX = 1;
            this.sonar2DView.BorderY = 1;
            rectangleD1.Bottom = 0D;
            rectangleD1.Left = 0D;
            rectangleD1.Right = 0D;
            rectangleD1.Top = 0D;
            this.sonar2DView.CoordLimits = rectangleD1;
            this.sonar2DView.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.sonar2DView.ForeColor = System.Drawing.Color.White;
            this.sonar2DView.InteractionMode = sonOmeter.Sonar2DView.InteractionModes.None;
            this.sonar2DView.Location = new System.Drawing.Point(0, 0);
            this.sonar2DView.Name = "sonar2DView";
            this.sonar2DView.PanelType = sonOmeter.Classes.SonarPanelType.HF;
            this.sonar2DView.ShowBlankLines = true;
            this.sonar2DView.ShowGrid = true;
            this.sonar2DView.Size = new System.Drawing.Size(619, 444);
            this.sonar2DView.TabIndex = 1;
            this.sonar2DView.TopColorMode = sonOmeter.Classes.TopColorMode.Top;
            this.sonar2DView.ViewAngle = 0D;
            this.sonar2DView.ViewMode2D = sonOmeter.Sonar2DView.Mode2D.Trace;
            this.sonar2DView.ViewMode3D = sonOmeter.Sonar2DView.Mode3D.TwoD;
            rectangleD2.Bottom = 0D;
            rectangleD2.Left = 0D;
            rectangleD2.Right = 0D;
            rectangleD2.Top = 0D;
            this.sonar2DView.ViewPort = rectangleD2;
            this.sonar2DView.WheelSpeed = 10D;
            this.sonar2DView.CoordChange += new sonOmeter.CoordEventHandler(this.sonar2DView_CoordChange);
            // 
            // editBar
            // 
            this.editBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.editBar.BackColor = System.Drawing.Color.Blue;
            this.editBar.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.editBar.CutMode = sonOmeter.Classes.CutMode.Nothing;
            this.editBar.DisplaceMove = false;
            this.editBar.EditMode = sonOmeter.EditModes.Nothing;
            this.editBar.ForeColor = System.Drawing.Color.White;
            this.editBar.InvCutMode = sonOmeter.Classes.CutMode.Nothing;
            this.editBar.Location = new System.Drawing.Point(240, 444);
            this.editBar.Margin = new System.Windows.Forms.Padding(0);
            this.editBar.Name = "editBar";
            this.editBar.Size = new System.Drawing.Size(230, 32);
            this.editBar.TabIndex = 3;
            this.editBar.EditReady += new sonOmeter.EditEventHandler(this.editBar_EditReady);
            // 
            // frm2D
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Blue;
            this.ClientSize = new System.Drawing.Size(619, 476);
            this.Controls.Add(this.labInfo);
            this.Controls.Add(this.cbDoCoordChange);
            this.Controls.Add(this.sonar2DView);
            this.Controls.Add(this.labViewMode);
            this.Controls.Add(this.labState);
            this.Controls.Add(this.labDisplay);
            this.Controls.Add(this.labCorridor);
            this.Controls.Add(this.editBar);
            this.DockType = DockDotNET.DockContainerType.Document;
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 500);
            this.Name = "frm2D";
            this.Text = "2D track";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frm2D_KeyDown);
            this.ResumeLayout(false);

		}
		#endregion

        private System.Windows.Forms.Label labInfo;
        private System.Windows.Forms.Label labState;

	}
}