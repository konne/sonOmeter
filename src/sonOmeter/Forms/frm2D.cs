using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Globalization;
using UKLib.MathEx;
using UKLib.Survey.Math;
using sonOmeter.Classes;
using sonOmeter.Classes.Sonar2D;
using UKLib.Debug;
using System.Threading;
using System.Collections.Generic;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for frm2D.
    /// </summary>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frm2D : DockDotNET.DockWindow
    {
        #region Create and Dispose
        public frm2D()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            sonar2DView.OnGridChanged += new EventHandler(sonar2DView_OnGridChanged);

            lastSize = Size;

            this.MouseWheel += new MouseEventHandler(frm2D_MouseWheel);
            
            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            GSC.PropertyChanged += new PropertyChangedEventHandler(OnSettingsChanged);
            OnSettingsChanged(this, new PropertyChangedEventArgs("All"));

            UpdateLabDisplay();
            UpdateLabViewMode();
            UpdateLabState();

            globalEventHandler = new GlobalEventHandler(OnGlobalEvent);
            surfaceCreatedEventHandler = new EventHandler(On3DRecordSurfaceCreated);
            GlobalNotifier.SignIn(globalEventHandler, GetFilterList());
        }

        private List<GlobalNotifier.MsgTypes> GetFilterList()
        {
            var filterlist = new List<GlobalNotifier.MsgTypes>();

            filterlist.Add(GlobalNotifier.MsgTypes.Toggle3DRecord);
            filterlist.Add(GlobalNotifier.MsgTypes.ToggleDevice);
            filterlist.Add(GlobalNotifier.MsgTypes.TogglePoints);
            filterlist.Add(GlobalNotifier.MsgTypes.NewCoordinate);
            filterlist.Add(GlobalNotifier.MsgTypes.Interpolate);
            filterlist.Add(GlobalNotifier.MsgTypes.EditBlankLine);
            filterlist.Add(GlobalNotifier.MsgTypes.NewRecord);
            filterlist.Add(GlobalNotifier.MsgTypes.WorkLineChanged);

            return filterlist;
        }
        #endregion

        #region Variables
        private GlobalEventHandler globalEventHandler;
        private EventHandler surfaceCreatedEventHandler;

        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

        SonarProject project = null;

        Size lastSize;

        bool coordsSet = false;

        bool ctrlHold = false;
        bool altHold = false;
        bool shiftHold = false;
        #endregion

        #region Properties
        public SonarProject Project
        {
            set
            {
                project = value;
                sonar2DView.Project = value;

                try
                {					
                   CoordLimitChange(true);
                }
                catch (Exception e)
                {
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "frm2D.Project: " + e.Message);
                }
            }
        }
        #endregion

        #region Key events
        private void frm2D_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            e.Handled = true;

            if ((editBar.EditMode != EditModes.Nothing))
            {
                editBar.Edit(e);
                return;
            }

            switch (e.KeyData)
            {
                // function keys
                case Keys.F5:
                    sonar2DView.PanelType = SonarPanelType.HF;
                    UpdateLabDisplay();
                    break;
                case Keys.F6:
                    sonar2DView.PanelType = SonarPanelType.NF;
                    UpdateLabDisplay();
                    break;
                case Keys.F7:
                    sonar2DView.TopColorMode = (TopColorMode)(((int)sonar2DView.TopColorMode + 1) % 3);
                    UpdateLabDisplay();
                    break;
                case Keys.F8:
                    sonar2DView.ViewMode2D = (Sonar2DView.Mode2D)(((int)sonar2DView.ViewMode2D % (int)Sonar2DView.Mode2D.Both) + 1);
                    UpdateLabViewMode();
                    break;
                case Keys.F8 | Keys.Shift:
                    sonar2DView.ViewMode3D = (Sonar2DView.Mode3D)(((int)sonar2DView.ViewMode3D % (int)Sonar2DView.Mode3D.Both) + 1);
                    UpdateLabViewMode();
                    break;
                case Keys.F10:
                    editBar.EditMode = EditModes.TraceGrid;
                    editBar.EditString = GSC.Settings.TraceSize.ToString(GSC.Settings.NFI);
                    break;
                case Keys.F12:
                    if (sonar2DView.InteractionMode == Sonar2DView.InteractionModes.EditBlankLine)
                        sonar2DView.InteractionMode = Sonar2DView.InteractionModes.None;
                    sonar2DView.ShowBlankLines = !sonar2DView.ShowBlankLines;
                    UpdateLabState();
                    break;
                case Keys.F12 | Keys.Control:
                    // create a new blank line and edit it or stop editting it
                    if (sonar2DView.InteractionMode == Sonar2DView.InteractionModes.EditBlankLine)
                        sonar2DView.InteractionMode = Sonar2DView.InteractionModes.None;
                    else
                    {
                        sonar2DView.ShowBlankLines = true; // switch on blank lines.
                        BlankLine blankLine = new BlankLine() { Name = "New blankline", ShowInTrace = true };
                        project.BlankLines.Add(blankLine);
                        project.EditBlankLine = blankLine;
                        GlobalNotifier.Invoke(this, blankLine, GlobalNotifier.MsgTypes.NewBlankLine);
                        GlobalNotifier.Invoke(this, blankLine, GlobalNotifier.MsgTypes.EditBlankLine);
                    }
                    break;
                case Keys.Back:
                    // clear blankline
                    if (sonar2DView.InteractionMode == Sonar2DView.InteractionModes.EditBlankLine)
                    {
                        project.EditBlankLine.Poly.Clear();
                        sonar2DView.Invalidate();
                    }
                    break;
                case Keys.D4:
                    CoordLimitChange(true);
                    break;
                // alphanumeric keys
                case Keys.F | Keys.Control:
                    sonar2DView.InteractionMode = Sonar2DView.InteractionModes.Find;
                    sonar2DView.Cursor = Cursors.Cross;
                    break;
                case Keys.B | Keys.Control:
                    sonar2DView.InteractionMode = Sonar2DView.InteractionModes.PlaceBuoy;
                    sonar2DView.Cursor = Cursors.Cross;
                    break;
                case Keys.M | Keys.Control:
                    sonar2DView.InteractionMode = Sonar2DView.InteractionModes.MeasureStart;
                    sonar2DView.Cursor = Cursors.Cross;
                    break;
                case Keys.W:
                    sonar2DView.ViewAngle = 270.0;
                    break;
                case Keys.E:
                    sonar2DView.ViewAngle = 90.0;
                    break;
                case Keys.N:
                    sonar2DView.ViewAngle = 0.0;
                    break;
                case Keys.S:
                    sonar2DView.ViewAngle = 180.0;
                    break;
                case Keys.G:
                    // Test for geotagged images.
                    if (GSC.Settings.Lic[Module.Modules.Testing])
                    {
                        frmGeoTag frm = new frmGeoTag();
                        if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            sonar2DView.DrawGeoTaggedImage(frm.FileName, frm.Type, frm.TransparentBG, frm.DrawOverlays);
                        }
                    }
                    break;
                // special keys
                case Keys.Left | Keys.Alt:
                    sonar2DView.MoveWorkLine(true, false, true);
                    break;
                case Keys.Left | Keys.Control:
                    sonar2DView.MoveWorkLine(true, true, true);
                    break;
                case Keys.Right | Keys.Alt:
                    sonar2DView.MoveWorkLine(false, false, true);
                    break;
                case Keys.Right | Keys.Control:
                    sonar2DView.MoveWorkLine(false, true, true);
                    break;
                case Keys.Up | Keys.Alt:
                    sonar2DView.SwitchWorkLineDev(false);
                    break;
                case Keys.Down | Keys.Alt:
                    sonar2DView.SwitchWorkLineDev(true);
                    break;
                case Keys.Escape:
                    sonar2DView.InteractionMode = Sonar2DView.InteractionModes.None;
                    sonar2DView.Cursor = Cursors.Default;
                    sonar2DView.Invalidate();
                    break;
            }
        }

        private void editBar_EditReady(object sender, sonOmeter.EditEventArgs e)
        {
            switch (e.EditMode)
            {
                case EditModes.Grid:
                    GSC.Settings.GridSize = e.ToDouble(nfi);
                    break;

                case EditModes.TraceGrid:
                    GSC.Settings.TraceSize = e.ToDouble(nfi);
                    break;
            }
        }
        #endregion

        #region Mouse
        private void frm2D_MouseWheel(object sender, MouseEventArgs e)
        {
            Point pt = sonar2DView.PointToClient(MousePosition);
            if (sonar2DView.ClientRectangle.Contains(pt))
                sonar2DView.InvokeOnMouseWheel(e);
        }
        #endregion

        #region Functions
        void UpdateLabDisplay()
        {
            labDisplay.Text = sonar2DView.PanelType.ToString() + "\n" + sonar2DView.TopColorMode.ToString();
        }

        void UpdateLabViewMode()
        {
            labViewMode.Text = sonar2DView.ViewMode2D.ToString() + "\n" + sonar2DView.ViewMode3D.ToString();
        }

        void UpdateLabState()
        {
            if (sonar2DView.ShowBlankLines)
                labState.Text = "B";
            else
                labState.Text = "";
        }

        void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            bool all = e.PropertyName == "All";

            if (e.PropertyName.StartsWith("CS.") || all)
                RefreshColors();

            if ((e.PropertyName == "CorridorBeepInterval") || all)
            {
                beepTimer.Interval = GSC.Settings.CorridorBeepInterval;
                beepTimer.Start();
            }

            // Start all user control settings changed notification functions:
            editBar.OnSettingsChanged(sender, e);
            sonar2DView.OnSettingsChanged(sender, e);

            switch (GSC.Settings.ViewMode2D)
            {
                case Sonar2DViewMode.Fixed:
                    cbDoCoordChange.CheckState = CheckState.Unchecked;
                    break;
                case Sonar2DViewMode.Follow:
                    cbDoCoordChange.CheckState = CheckState.Checked;
                    break;
                case Sonar2DViewMode.FollowRotate:
                    cbDoCoordChange.CheckState = CheckState.Indeterminate;
                    break;
            }
        }

        private void RefreshColors()
        {
            labInfo.BackColor = GSC.Settings.CS.BackColor;
            labDisplay.BackColor = GSC.Settings.CS.BackColor;
            labViewMode.BackColor = GSC.Settings.CS.BackColor;
            labCorridor.BackColor = GSC.Settings.CS.BackColor;
            labState.BackColor = GSC.Settings.CS.BackColor;
            labInfo.ForeColor = GSC.Settings.CS.ForeColor;
            labDisplay.ForeColor = GSC.Settings.CS.ForeColor;
            labViewMode.ForeColor = GSC.Settings.CS.ForeColor;
            labCorridor.ForeColor = GSC.Settings.CS.ForeColor;
            labState.ForeColor = GSC.Settings.CS.ForeColor;
        }

        public void RebuildTraceArray()
        {
            sonar2DView.RebuildTraceArray();
            sonar2DView.RedrawSonarBmp();
        }
        #endregion

        #region Coordinates
        private void UpdateViewPort()
        {
            sonar2DView.ViewPort = new RectangleD(sonar2DView.CoordLimits);
        }

        private void sonar2DView_CoordChange(object sender, sonOmeter.CoordEventArgs e)
        {
            Transform tr = GSC.Settings.ForwardTransform;

            Coordinate coord = tr.Run(e.Coord, CoordinateType.Elliptic);

            if (MouseButtons == MouseButtons.Middle)
            {
                if (ctrlHold)
                {
                    // Sonar2DView is in rotation mode
                    editBar.LabelString = sonar2DView.ViewAngle.ToString("0.0", nfi) + "°";
                }
                else
                {
                    // Sonar2DView is in drag mode
                    PointD ptD = sonar2DView.ViewPort.Center - sonar2DView.CoordLimits.Center;

                    editBar.LabelString = "dHV: " + ptD.X.ToString("0.000", nfi) +
                                        "\ndRV: " + ptD.Y.ToString("0.000", nfi);
                }
            }
            else
            {
                editBar.LabelString = "HV: " + e.Coord.HV.ToString("0.000", nfi) +
                                      "   LA: " + Trigonometry.Rad2DMS(coord.LA, nfi, true) +
                                      "\nRV: " + e.Coord.RV.ToString("0.000", nfi) +
                                      "   LO: " + Trigonometry.Rad2DMS(coord.LO, nfi, false);
            }
        }
    
        public void CoordLimitChange(bool recalc)
        {
            try
            {
                if (project == null)
                    return;

                RectangleD rc = project.CoordBounds();
                cbDoCoordChange.BackColor = Color.Gray;

                if ((rc.Left != 0) & (rc.Right != 0) & (rc.Top != 0) & (rc.Bottom != 0))
                {
                    coordsSet = true;
                }

                if ((sonar2DView.CoordLimits.Left > rc.Left) |
                    (sonar2DView.CoordLimits.Right < rc.Right) |
                    (sonar2DView.CoordLimits.Bottom > rc.Bottom) |
                    (sonar2DView.CoordLimits.Top < rc.Top) | recalc)
                {
                    if (cbDoCoordChange.Checked & !recalc)
                    {
                        cbDoCoordChange.BackColor = Color.Red;
                    }
                    else
                    {						
                        if (rc.Width > rc.Height)
                        {
                            double diff = rc.Width - rc.Height;
                            rc.Top += diff / 2;
                            rc.Bottom -= diff / 2;
                        }

                        if (rc.Height > rc.Width)
                        {
                            double diff = rc.Height - rc.Width;
                            rc.Left -= diff / 2;
                            rc.Right += diff / 2;
                        }				
                        
                        rc.Inflate(2, 2);

                        sonar2DView.CoordLimits = rc;
                    }
                }

                UpdateViewPort();
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "frm2D.CoordLimitChange: " + ex.Message);
            }
        }
        #endregion

        #region Global events
        private Coordinate workLineCoord = Coordinate.Empty;

        public void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            switch (state)
            {
                case GlobalNotifier.MsgTypes.WorkLineChanged:
                    var e = args as RecordEventArgs;
                    var line = e.Tag as SonarLine;

                    if (line != null)
                        workLineCoord = line.CoordRvHv;
                    else
                        workLineCoord = Coordinate.Empty;
                    break;

                case GlobalNotifier.MsgTypes.NewRecord:
                case GlobalNotifier.MsgTypes.Toggle3DRecord:
                    Sonar3DRecord rec3D = args as Sonar3DRecord;

                    if (rec3D == null)
                        return;

                    if (rec3D.ShowInTrace)
                        rec3D.SurfaceInterpolated += surfaceCreatedEventHandler;
                    else
                        rec3D.SurfaceInterpolated -= surfaceCreatedEventHandler;
                    sonar2DView.RedrawSonarBmp();
                    break;

                case GlobalNotifier.MsgTypes.ToggleDevice:
                case GlobalNotifier.MsgTypes.TogglePoints:
                case GlobalNotifier.MsgTypes.Interpolate:
                    // CoordLimitChange(true);
                    // -> Removed because of bug report #0000068.
                    RebuildTraceArray();
                    break;

                case GlobalNotifier.MsgTypes.EditBlankLine:
                    sonar2DView.InteractionMode = Sonar2DView.InteractionModes.EditBlankLine;
                    break;

                case GlobalNotifier.MsgTypes.NewCoordinate:
                    try
                    {
                        Coordinate coord = (Coordinate)args;
                        RectangleD viewPort = sonar2DView.ViewPort;

                        if (coord.Type != CoordinateType.TransverseMercator)
                            break;

                        if (!coordsSet)
                        {
                            RectangleD rc = sonar2DView.CoordLimits;
                            PointD pt = new PointD(coord.RV, coord.HV);
                            coordsSet = true;
                        }

                        switch (cbDoCoordChange.CheckState)
                        {
                            case CheckState.Unchecked:
                                {
                                    PointD pcoord = new PointD(coord.RV, coord.HV);
                                    cbDoCoordChange.BackColor = Color.Gray;
                                    if (!viewPort.Contains(pcoord))
                                        cbDoCoordChange.BackColor = Color.Red;
                                }
                                break;
                            case CheckState.Checked:
                                {
                                    PointD pcoord = new PointD(coord.RV, coord.HV);
                                    cbDoCoordChange.BackColor = Color.Gray;
                                    RectangleD v75 = new RectangleD(viewPort);
                                    v75.Inflate(0.75, 0.75);

                                    if (!v75.Contains(pcoord))
                                    {
                                        double width = viewPort.Width;
                                        double height = viewPort.Height;

                                        viewPort.Left = -(viewPort.Width / 2) + coord.RV;
                                        viewPort.Top = -(viewPort.Height / 2) + coord.HV;
                                        viewPort.Right = viewPort.Left + width;
                                        viewPort.Bottom = viewPort.Top + height;
                                        sonar2DView.ViewPort = viewPort;
                                    }
                                }
                                break;
                            case CheckState.Indeterminate:
                                {
                                    cbDoCoordChange.BackColor = Color.Gray;

                                    double width = viewPort.Width;
                                    double height = viewPort.Height;

                                    viewPort.Left = -(viewPort.Width / 2) + coord.RV;
                                    viewPort.Top = -(viewPort.Height / 2) + coord.HV;
                                    viewPort.Right = viewPort.Left + width;
                                    viewPort.Bottom = viewPort.Top + height;
                                    sonar2DView.ViewPort = viewPort;
                                    sonar2DView.ViewAngle = project.LastRotation.Yaw;
                                }
                                break;

                        }
                    }
                    catch (Exception ex)
                    {
                        DebugClass.SendDebugLine(this, DebugLevel.Red, "NewCoordinate @ frm2D : " + ex.Message);
                    }
                    break;                  
            }
        }

        void On3DRecordSurfaceCreated(object sender, EventArgs e)
        {
            sonar2DView.RedrawSonarBmp();
        }
        #endregion

        #region DoCoordChange-Checkbox Event
        private void cbDoCoordChange_CheckStateChanged(object sender, EventArgs e)
        {
            switch (cbDoCoordChange.CheckState)
            {
                case CheckState.Unchecked:
                    GSC.Settings.ViewMode2D = Sonar2DViewMode.Fixed;
                    break;
                case CheckState.Checked:
                    GSC.Settings.ViewMode2D = Sonar2DViewMode.Follow;
                    break;
                case CheckState.Indeterminate:
                    GSC.Settings.ViewMode2D = Sonar2DViewMode.FollowRotate;
                    break;
            }

            cbDoCoordChange.ImageIndex = (int)GSC.Settings.ViewMode2D;
        }
        #endregion

        #region Corridor
        private void labCorridor_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            float w = labCorridor.Width;
            float h = labCorridor.Height;
            float d = 0.0F;
            float r = 0.5F;
            float cw = 0.0F;
            int col = 0;

            float w_p = w / 2.0F - 5.0F - 30.0F;
            float h_p = h / 2.0F - 5.0F;
            float d_p = 0.0F;

            BuoyConnection c;

            if (labCorridor.Tag is CorrDistTag)
            {
                c = ((CorrDistTag)labCorridor.Tag).Corr;
                cw = (float)c.CatchWidth;
                r = (float)c.Corridor / cw;
                d = (float)((CorrDistTag)labCorridor.Tag).RelDist;
                d_p = (float)((CorrDistTag)labCorridor.Tag).RelDistNorm;
                if (Math.Abs(d_p) < r)
                    col = (int)(Math.Abs(d_p) / r * 255.0F);
                else
                    col = 255 + (int)((Math.Abs(d_p) - r) / (1.0F - r) * 255.0F);
                d_p *= w_p;
            }

            if (col < 256)
                g.Clear(Color.FromArgb(col, 255, 0));
            else
                g.Clear(Color.FromArgb(255, 511 - col, 0));

            Font fnt = new Font(this.Font.FontFamily.Name, 18.0F, FontStyle.Regular, GraphicsUnit.Pixel);
            RectangleF rc = new RectangleF(2.0F, 5.0F, 60.0F - 2.0F, h - 10.0F);
            StringFormat sf = new StringFormat(StringFormat.GenericDefault);
            sf.Alignment = StringAlignment.Far;
            g.DrawString((d * cw / 2.0F).ToString("0.0"), fnt, new SolidBrush(Color.Black), rc, sf);

            g.TranslateTransform(w / 2.0F - 1.0F + 30.0F, h / 2.0F - 1.0F);

            g.DrawLine(new Pen(Color.Black, 1.0F), -w_p, 0, w_p, 0);
            g.DrawLine(new Pen(Color.Black, 1.0F), 0, -h_p, 0, h_p);
            g.DrawLine(new Pen(Color.Black, 1.0F), -w_p * r, -h_p * 0.5F, -w_p * r, h_p * 0.5F);
            g.DrawLine(new Pen(Color.Black, 1.0F), w_p * r, -h_p * 0.5F, w_p * r, h_p * 0.5F);
            g.DrawLine(new Pen(Color.Black, 5.0F), d_p, -h_p * 0.8F, d_p, h_p * 0.8F);
        }
        #endregion

        #region BeepTimer / Corridor
        struct CorrDistTag
        {
            /// <summary>
            /// The distance in relation to the catch width.
            /// </summary>
            public double RelDist;
            /// <summary>
            /// The normalized distance in relation to the catch width.
            /// </summary>
            public double RelDistNorm;
            /// <summary>
            /// The <see cref="BuoyConnection"/> corridor.
            /// </summary>
            public BuoyConnection Corr;

            /// <summary>
            /// Creates a new structure.
            /// </summary>
            /// <param name="dist">The distance in relation to the catch width.</param>
            /// <param name="dist">The normalized distance in relation to the catch width.</param>
            /// <param name="corr">The <see cref="BuoyConnection"/> corridor.</param>
            public CorrDistTag(double relDist, double relDistNorm, BuoyConnection corr)
            {
                this.RelDist = relDist;
                this.RelDistNorm = relDistNorm;
                this.Corr = corr;
            }
        }

        private void beepTimer_Tick(object sender, EventArgs e)
        {
            var coord = (sonar2DView.LastCoord.Type == CoordinateType.TransverseMercator) && project.Recording ? sonar2DView.LastCoord : workLineCoord;

            PointD pt = coord.Point + project.LastLockOffset;
            double dNearest = double.MaxValue;
            double rel, rel_norm;
            BuoyConnection cNearest = null;
            int f;

            // Get the highlighted connection.
            if (!coord.IsEmpty)
            {
                try
                {
                    if (sonar2DView.LockedConnection != null)
                    {
                        cNearest = sonar2DView.LockedConnection;
                        dNearest = cNearest.GetCorridorDistance(pt);
                    }
                    else
                    {
                        project.GetNearestConnection(pt, ref cNearest, ref dNearest, true);
                    }
                }
                catch (Exception ex)
                {
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "beepTimer, Buoys " + ex.Message);
                }

                try
                {
                    if ((cNearest == null) || (cNearest.Corridor <= 0.0))
                        return;

                    rel = dNearest / (cNearest.CatchWidth / 2.0);
                    rel_norm = Math.Min(1.0, Math.Max(-1.0, rel));

                    f = Math.Max(4 * (int)Math.Pow(10.0, 2.0 + Math.Abs(rel_norm)), 400);

                    if ((f < 4000) && GSC.Settings.CorridorBeep && project.Recording)
                    {
                        GlobalNotifier.BeepThread.Add((beepTimer.Interval / 2), f);
                    }

                    cNearest.Highlighted = true;
                    project.SelectedCorridor = cNearest;

                    labCorridor.Tag = new CorrDistTag(rel, rel_norm, cNearest);
                    labCorridor.Invalidate();
                }
                catch (Exception ex)
                {
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "beepTimer, Dist " + ex.Message);
                }
            }
            else
            {
                labCorridor.Tag = null;
                labCorridor.Invalidate();
            }
        }
        #endregion

        void sonar2DView_OnGridChanged(object sender, EventArgs e)
        {
            labInfo.Text = sonar2DView.GridSpacing.ToString("F2");
        }
    }
}
