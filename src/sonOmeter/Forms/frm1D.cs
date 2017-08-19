using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using sonOmeter.Classes;
using System.Collections.ObjectModel;
using sonOmeter.Classes.Sonar2D;
using System.Collections.Generic;
using UKLib.MathEx;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for frmRecord.
    /// </summary>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frm1D : DockDotNET.DockWindow
    {
        #region Contruction and dispose
        public frm1D()
        {
            Init();

            OnSettingsChanged(this, new PropertyChangedEventArgs("All"));
        }

        public frm1D(SonarProject prj, SonarRecord rec, int deviceID)
        {
            Init();

            try
            {
                project = prj;

                BindSonarLines(rec, rec.Devices[deviceID]);

                // Try to find a form that already contains one device of the same record.
                // In case of a hit - copy the workline location and break.
                foreach (SonarDevice dev in record.Devices)
                {
                    if ((dev != device) && (dev.IsOpen))
                    {
                        WorkLine = device.SonarLine(Math.Min(device.SonarLines.Count, dev.IndexOf((dev.HostForm as frm1D).workLine)));
                        break;
                    }
                }

                OnSettingsChanged(this, new PropertyChangedEventArgs("All"));
            }
            catch
            {
            }
        }

        private void BindSonarLines(SonarRecord rec, SonarDevice dev)
        {
            // Save previous record for preservation of the workline.
            SonarRecord oldRec = record;
            int workLineID = -1;

            if (device != null)
            {
                // Save previous workline ID.
                workLineID = device.IndexOf(workLine);

                // Set free the old device.
                device.IsOpen = false;
                device.HostForm = null;
            }
            
            // Switch to new record and device and lock device.
            record = rec;
            int deviceID = rec.IndexOf(dev);
            device = dev;
            device.IsOpen = true;
            device.HostForm = this;
            
            // Set cutline sets.
            sonar1DView.ClSetHF = device.ClSetHF;
            sonar1DView.ClSetNF = device.ClSetNF;
            sonar1DView.Recording = record.Recording;
            sonar1DView.RefreshLists(device.SonarLines, project.BuoyList, record.ManualPoints);

            // Set view range.
            sbFile.RegionWidth = device.SonarLines.Count;
            sbFile.SectionStart = 0;

            VideoOut = device.SonarLines.Count;

            tsmiZoom100_Click(this, null);

            // Reset or preserve work line.
            if ((workLineID == -1) || (oldRec != record))
                WorkLine = null;
            else
                WorkLine = device.SonarLine(Math.Min(device.SonarLines.Count, workLineID));
                
            // Set form text.
            this.Text = record.Description + " - Sonar " + (record.IsProfile ? "data" : (deviceID + 1).ToString());

            // Apply arch.
            device.ApplyArchAndVolume(sonarStatusBar.IsCut);
            project.RecalcVolume();

            // Raise event.
            GlobalNotifier.Invoke(this, new RecordEventArgs(rec, dev, rec.GetType()), GlobalNotifier.MsgTypes.DeviceChanged);
        }

        public void Init()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            GSC.PropertyChanged += new PropertyChangedEventHandler(OnSettingsChanged);

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            sonarInfoBar.EditBar.EditReady += new EditEventHandler(EditReady);

            globalEventHandler = new GlobalEventHandler(OnGlobalEvent);
            GlobalNotifier.SignIn(globalEventHandler, GetFilterList());
        }

     
        private List<GlobalNotifier.MsgTypes> GetFilterList()
        {
            var filterlist = new List<GlobalNotifier.MsgTypes>();
            filterlist.Add(GlobalNotifier.MsgTypes.RecordingChanged);
            filterlist.Add(GlobalNotifier.MsgTypes.NewSonarLine);
            filterlist.Add(GlobalNotifier.MsgTypes.NewDeviceList);
            filterlist.Add(GlobalNotifier.MsgTypes.WorkLineChanged);
            filterlist.Add(GlobalNotifier.MsgTypes.PlaceBuoy);
            filterlist.Add(GlobalNotifier.MsgTypes.PlaceManualPoint);
            filterlist.Add(GlobalNotifier.MsgTypes.UpdateCoordinates);
            filterlist.Add(GlobalNotifier.MsgTypes.UpdateProfiles);
            filterlist.Add(GlobalNotifier.MsgTypes.UpdateRecord);

            return filterlist;
        }
        #endregion
    
        #region Variables
        SonarProject project = null;
        SonarRecord record = null;
        SonarDevice device = null;

        CutMode cutMode = CutMode.Nothing;
        
        SonarLine workLine = null;
        
        double depthZoomHi = 0.0;
        double depthZoomLo = -100.0;

        private System.Windows.Forms.CheckBox cbMainWindow;
        private System.Windows.Forms.ImageList imgListMainWnd;
        private sonOmeter.SonarInfoBar sonarInfoBar;

        GlobalEventHandler globalEventHandler;

        private int VideoOut = 0;

        private bool redrawView = true;
        private bool useAbsoluteHeights = false;
        #endregion
        
        #region Properties
        public bool UseAbsoluteHeights
        {
            get { return useAbsoluteHeights; }
            set
            {
                useAbsoluteHeights = value;
                sonar1DView.UseAbsoluteHeights = value;
                UpdateDepthLimits();
            }
        }

        public SonarRecord Record
        {
            get { return record; }
        }

        public SonarDevice Device
        {
            get { return device; }
        }
        
        private double DepthZoomHi
        {
            get { return depthZoomHi; }
            set
            {
                /*if (value <= depthZoomLo)
                    return;*/
                
                depthZoomHi = value;
                panControl.DepthZoomHi = value;
                sonar1DView.ViewPort.Top = value;

                if (redrawView)
                {
                    sonar1DView.RedrawSonarBmp();
                    sonar1DView.Invalidate();
                }
            }
        }

        private double DepthZoomLo
        {
            get { return depthZoomLo; }
            set
            {
                /*if (value >= depthZoomHi)
                    return;*/
                
                depthZoomLo = value;
                panControl.DepthZoomLo = value;
                sonar1DView.ViewPort.Bottom = value;

                if (redrawView)
                {
                    sonar1DView.RedrawSonarBmp();
                    sonar1DView.Invalidate();
                }
            }
        }

        private CutMode CutModeProp
        {
            set
            {
                cutMode = value;
                sonar1DView.CutMode = cutMode;

                if (sonarInfoBar != null)
                {
                    sonarInfoBar.LabelText = "Cut mode switched to: " + cutMode + "    [" + sonar1DView.CutMouseMode + "]";
                }
            }
        }

        [Browsable(false)]
        private SonarLine WorkLine
        {
            set
            {
                workLine = value;

                sonar1DView.WorkLine = workLine;
                workLine = sonar1DView.WorkLine;

                if ((project != null) && (record != null) && (device != null))
                {
                    RecordEventArgs e = new RecordEventArgs(record, device, workLine);
                    GlobalNotifier.Invoke(this, e, GlobalNotifier.MsgTypes.WorkLineChanged);
                }
                if (workLine != null)
                {
                    if (workLine.CoordRvHv.Type == UKLib.Survey.Math.CoordinateType.TransverseMercator)
                        sonarInfoBar.LabelText = "HV: " + workLine.CoordRvHv.HV.ToString("0.00") + " RV: " + workLine.CoordRvHv.RV.ToString("0.00") + " AL: " + workLine.CoordRvHv.AL.ToString("0.00") + " Time: " + workLine.Time.ToString();
                    else
                        sonarInfoBar.LabelText = " Time: " + workLine.Time.ToString();

#if DEBUG
                    sonarInfoBar.LabelText += ", Index: " + device.IndexOf(workLine);
#endif
                }
                else
                {
                    sonarInfoBar.LabelText = "";
                }		                
            }
        }
        #endregion

        #region Zoom events
        private void ZoomDepth(object sender, ZoomEventArgs e)
        {
            double top = GSC.Settings.DepthTop;

            if (useAbsoluteHeights && (device != null))
                top = Math.Ceiling(device.ALMax) + 1;
            
            redrawView = false;

            switch (e.ZoomEventType)
            {
                case ZoomEventArgs.ZoomEventTypes.Start:
                    DepthZoomHi = -e.ZoomValue + top;
                    break;
                case ZoomEventArgs.ZoomEventTypes.End:
                    DepthZoomLo = -e.ZoomValue + top;
                    break;
                case ZoomEventArgs.ZoomEventTypes.Position:
                    double temp = depthZoomHi + e.ZoomValue - top;
                    DepthZoomHi = depthZoomHi - temp;
                    DepthZoomLo = depthZoomLo - temp;
                    break;
            }

            sonar1DView.RedrawSonarBmp();
            sonar1DView.Invalidate();

            redrawView = true;
        }
        
        private void ZoomFile(object sender, ZoomEventArgs e)
        {
            OnResize(new System.EventArgs());

            WorkLine = workLine;
        }

        private void tsmiZoom50_Click(object sender, EventArgs e)
        {
            sbFile.SetWidth(sonar1DView.Width * 2);

            OnResize(new System.EventArgs());
        }

        private void tsmiZoom100_Click(object sender, EventArgs e)
        {
            sbFile.SetWidth(sonar1DView.Width);

            OnResize(new System.EventArgs());
        }

        private void tsmiZoom200_Click(object sender, EventArgs e)
        {
            sbFile.SetWidth(sonar1DView.Width / 2);

            OnResize(new System.EventArgs());
        }

        private void tsmiFitPage_Click(object sender, EventArgs e)
        {
            sbFile.SectionStart = 0;
            sbFile.SectionWidth = sbFile.RegionWidth;

            OnResize(new System.EventArgs());
        }
        
        private void btnTopUp_Click(object sender, System.EventArgs e)
        {
            sbDepth.SlideAnchor = SlideBar.SlideAnchors.End;
            sbDepth.SectionStart -= 5;
            sbDepth.SlideAnchor = SlideBar.SlideAnchors.Width;
            sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
        }

        private void btnTopDown_Click(object sender, System.EventArgs e)
        {
            sbDepth.SlideAnchor = SlideBar.SlideAnchors.End;
            sbDepth.SectionStart += 5;
            sbDepth.SlideAnchor = SlideBar.SlideAnchors.Width;
            sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
        }

        private void btnBottomUp_Click(object sender, System.EventArgs e)
        {
            sbDepth.SectionEnd -= 5;
            sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);
        }

        private void btnBottomDown_Click(object sender, System.EventArgs e)
        {
            sbDepth.SectionEnd += 5;
            sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);
        }
        #endregion

        #region Key events
        /// <summary>
        /// Change the associated sonar device and / or record.
        /// </summary>
        /// <param name="up">Advance in list upwards (lower index).</param>
        /// <param name="jmp">Jump between devices.</param>
        private void ChangeDevice(bool up, bool jmp)
        {
            SonarRecord rec = record;
            SonarDevice dev = device;
            int recID = project.IndexOf(rec);
            int devID = record.IndexOf(dev);
            int recMax = project.RecordCount;

            if (up)
            {
                if (record is SonarProfile)
                {
                    recMax = project.ProfileCount;

                    #region Advance upwards between profiles.
                    while (recID >= 0)
                    {
                        recID--;

                        if (recID >= 0)
                        {
                            rec = project.Profile(recID);
                            dev = rec.Devices[devID];
                            if (!dev.IsOpen)
                                break;
                        }
                    }
                    #endregion
                }
                else if (jmp)
                {
                    #region Advance upwards between devices.
                    while ((devID >= 0) && (recID >= 0))
                    {
                        devID--;

                        if (devID < 0)
                        {
                            recID--;
                            if (recID >= 0)
                            {
                                rec = project.Record(recID);
                                devID = rec.DeviceCount - 1;
                            }
                        }

                        if ((devID >= 0) && (recID >= 0))
                        {
                            dev = rec.Devices[devID];
                            if (!dev.IsOpen)
                                break;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Advance upwards between records.
                    while ((devID >= 0) && (recID >= 0))
                    {
                        recID--;

                        if (recID >= 0)
                        {
                            rec = project.Record(recID);
                            if (rec.DeviceCount <= devID)
                                devID = -1;

                            if (devID >= 0)
                            {
                                dev = rec.Devices[devID];
                                if (!dev.IsOpen)
                                    break;
                            }
                        }
                    }
                    #endregion
                }
            }
            else
            {
                if (record is SonarProfile)
                {
                    recMax = project.ProfileCount;

                    #region Advance downwards between profiles.
                    while (recID < recMax)
                    {
                        recID++;

                        if (recID < recMax)
                        {
                            rec = project.Profile(recID);
                            dev = rec.Devices[devID];
                            if (!dev.IsOpen)
                                break;
                        }
                    }
                    #endregion
                }
                else if (jmp)
                {
                    #region Advance downwards between devices.
                    while ((recID < recMax) && (devID < rec.DeviceCount))
                    {
                        devID++;

                        if (devID >= rec.DeviceCount)
                        {
                            recID++;
                            if (recID < recMax)
                            {
                                rec = project.Record(recID);
                                devID = 0;
                            }
                        }

                        if ((recID < recMax) && (devID < rec.DeviceCount))
                        {
                            dev = rec.Devices[devID];
                            if (!dev.IsOpen)
                                break;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region Advance downwards between records.
                    while ((recID < recMax) && (devID >= 0))
                    {
                        recID++;

                        if (recID < recMax)
                        {
                            rec = project.Record(recID);
                            if (rec.DeviceCount <= devID)
                                devID = -1;

                            if (devID >= 0)
                            {
                                dev = rec.Devices[devID];
                                if (!dev.IsOpen)
                                    break;
                            }
                        }
                    }
                    #endregion
                }
            }

            if ((devID >= 0) && (recID >= 0) && (recID < recMax) && (devID < rec.DeviceCount))
                BindSonarLines(rec, dev);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        
            e.Handled = true;

            if (sonarInfoBar.EditMode != EditModes.Nothing)
            {
                sonarInfoBar.Edit(e);
                return;
            }
            
            switch (e.KeyData)
            {
                    // digits
                case Keys.D1:
                    tsmiZoom50_Click(this, new System.EventArgs());
                    break;
                case Keys.D2:
                    tsmiZoom100_Click(this, new System.EventArgs());
                    break;
                case Keys.D3:
                    tsmiZoom200_Click(this, new System.EventArgs());
                    break;
                case Keys.D4:
                    tsmiFitPage_Click(this, new System.EventArgs());
                    break;
                case Keys.D5:
                    sbDepth.SectionStart = 0;
                    sbDepth.SectionWidth = sbDepth.RegionWidth;
                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);
                    tsmiFitPage_Click(this, new System.EventArgs());
                    break;
                    // letters
                case Keys.C:
                    GSC.Settings.SECL[0].SonarColorVisible = !GSC.Settings.SECL[0].SonarColorVisible;
                    break;
                case Keys.G:
                    GSC.Settings.SECL[1].SonarColorVisible = !GSC.Settings.SECL[1].SonarColorVisible;
                    break;
                case Keys.Y:
                    GSC.Settings.SECL[2].SonarColorVisible = !GSC.Settings.SECL[2].SonarColorVisible;
                    break;
                case Keys.O:
                    GSC.Settings.SECL[3].SonarColorVisible = !GSC.Settings.SECL[3].SonarColorVisible;
                    break;
                case Keys.M:
                    GSC.Settings.SECL[4].SonarColorVisible = !GSC.Settings.SECL[4].SonarColorVisible;
                    break;
                case Keys.R:
                    GSC.Settings.SECL[5].SonarColorVisible = !GSC.Settings.SECL[5].SonarColorVisible;
                    break;
                case Keys.B:
                    GSC.Settings.SECL[6].SonarColorVisible = !GSC.Settings.SECL[6].SonarColorVisible;
                    break;
                case Keys.A:
                    if ((cutMode == CutMode.Top) || (cutMode == CutMode.Bottom))
                    {
                        sonarInfoBar.CutMode = cutMode;
                        sonarInfoBar.EditMode = EditModes.Arch;
                    }
                    else if ((cutMode == CutMode.Nothing) && project.Recording)
                    {
                        sonar1DView.PlaceSomething = PlaceMode.ManualPoint;
                        sonar1DView.PlaceMarker(0.0F, device.GetLastLineWithCoord(), false);
                        sonar1DView.PlaceSomething = PlaceMode.Nothing;
                    }
                    break;
                case Keys.A | Keys.Control:
                case Keys.L | Keys.Control:
                    if (cutMode == CutMode.Nothing)
                        sonar1DView.PlaceSomething = PlaceMode.ManualPoint;
                    break;
                case Keys.L:
                    if (cutMode == CutMode.Nothing)
                        ToggleDepthLines();
                    break;
                case Keys.Q:
                    if ((cutMode == CutMode.Nothing) && project.Recording)
                    {
                        sonar1DView.PlaceSomething = PlaceMode.Buoy;
                        sonar1DView.PlaceMarker(0.0F, device.GetLastLineWithCoord(), false);
                        sonar1DView.PlaceSomething = PlaceMode.Nothing;
                    }
                    break;
                case Keys.Q | Keys.Control:
                case Keys.B | Keys.Control:
                    if (cutMode == CutMode.Nothing)
                        sonar1DView.PlaceSomething = PlaceMode.Buoy;
                    break;
                case Keys.D:
                    if ((cutMode == CutMode.Top) || (cutMode == CutMode.Bottom))
                    {
                        sonarInfoBar.CutMode = cutMode;
                        sonarInfoBar.EditMode = EditModes.Displace;
                    }
                    break;
                case Keys.H:
                    sonarInfoBar.EditMode = EditModes.PitchH;
                    sonarInfoBar.EditString = GSC.Settings.PitchH.ToString(GSC.Settings.NFI);
                    break;
                case Keys.H | Keys.Control:
                    UseAbsoluteHeights = !useAbsoluteHeights;
                    break;
                case Keys.V:
                    sonarInfoBar.EditMode = EditModes.PitchV;
                    sonarInfoBar.EditString = GSC.Settings.PitchV.ToString(GSC.Settings.NFI);
                    break;
                case Keys.P:
                    if ((sonar1DView.CutMouseMode == CutMouseMode.Paint) | ((cutMode != CutMode.Top) & (cutMode != CutMode.Bottom)))
                        sonar1DView.CutMouseMode = CutMouseMode.Normal;
                    else
                        sonar1DView.CutMouseMode = CutMouseMode.Paint;
                    
                    CutModeProp = cutMode;
                    break;
                case Keys.E:
                    if ((sonar1DView.CutMouseMode == CutMouseMode.Disable) | ((cutMode != CutMode.Top) & (cutMode != CutMode.Bottom)))
                        sonar1DView.CutMouseMode = CutMouseMode.Normal;
                    else if (GSC.Settings.Lic[Module.Modules.V22])
                        sonar1DView.CutMouseMode = CutMouseMode.Disable;

                    CutModeProp = cutMode;
                    break;

                case Keys.E | Keys.Alt:
                    if ((sonar1DView.CutMouseMode == CutMouseMode.Remove) | ((cutMode != CutMode.Top) & (cutMode != CutMode.Bottom)))
                        sonar1DView.CutMouseMode = CutMouseMode.Normal;
                    else if (GSC.Settings.Lic[Module.Modules.V22])
                        sonar1DView.CutMouseMode = CutMouseMode.Remove;

                    CutModeProp = cutMode;
                    break;
                case Keys.E | Keys.Control:
                    if ((sonar1DView.CutMouseMode == CutMouseMode.Rubber) | ((cutMode != CutMode.Top) & (cutMode != CutMode.Bottom)))
                        sonar1DView.CutMouseMode = CutMouseMode.Normal;
                    else
                        sonar1DView.CutMouseMode = CutMouseMode.Rubber;

                    CutModeProp = cutMode;
                    break;
                case Keys.S:
                    if ((cutMode == CutMode.Top) || (cutMode == CutMode.Bottom))
                    {
                        sonarInfoBar.CutMode = cutMode;
                        sonarInfoBar.EditMode = EditModes.SetGlobal;
                    }
                    break;
                    // special keys
                case Keys.PageUp:
                    if (sbFile.SectionEnd + sbFile.SectionWidth < sbFile.RegionWidth)
                        sbFile.SectionStart += sbFile.SectionWidth;
                    else
                        sbFile.SectionStart = sbFile.RegionWidth - sbFile.SectionWidth;
                    sbFile.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
                    break;
                case Keys.PageDown:
                    sbFile.SectionStart -= sbFile.SectionWidth;
                    sbFile.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
                    break;
                case Keys.Home:
                    sbFile.SectionStart = 0;
                    sbFile.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
                    break;
                case Keys.End:
                    sbFile.SectionStart = sbFile.RegionWidth-sbFile.SectionWidth;
                    sbFile.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
                    break;
                case Keys.Space:
                    if (cutMode == CutMode.Top)
                        CutModeProp = CutMode.Bottom;
                    else if (cutMode == CutMode.Bottom)
                        CutModeProp = CutMode.Top;
                    break;
                case Keys.Back:
                    if ((cutMode == CutMode.Top) || (cutMode == CutMode.Bottom))
                    {
                        sonarInfoBar.CutMode = cutMode;
                        sonarInfoBar.EditMode = EditModes.ClearCutLine;
                    }
                    break;
                case Keys.Escape:
                    sonar1DView.PlaceSomething = PlaceMode.Nothing;
                    sonar1DView.CutMouseMode = CutMouseMode.Normal;
                    break;
                    // NumPad
                case Keys.NumPad9:
                    sbDepth.SectionEnd -= sbDepth.SectionWidthMin;
                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);
                    break;
                case Keys.NumPad3:
                    sbDepth.SectionEnd += sbDepth.SectionWidthMin;
                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);
                    break;
                case Keys.NumPad7:
                    sbDepth.SlideAnchor = SlideBar.SlideAnchors.End;
                    sbDepth.SectionStart -= sbDepth.SectionWidthMin;
                    sbDepth.SlideAnchor = SlideBar.SlideAnchors.Width;
                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
                    break;
                case Keys.NumPad1:
                    sbDepth.SlideAnchor = SlideBar.SlideAnchors.End;
                    sbDepth.SectionStart += sbDepth.SectionWidthMin;
                    sbDepth.SlideAnchor = SlideBar.SlideAnchors.Width;
                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
                    break;
                case Keys.NumPad8:
                    sbDepth.SectionStart -= sbDepth.SectionWidthMin;
                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Position);
                    break;
                case Keys.NumPad2:
                    sbDepth.SectionStart += sbDepth.SectionWidthMin;
                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Position);
                    break;
                case Keys.NumPad5:
                case Keys.Enter:
                    if (cutMode != CutMode.Nothing)
                    {
                        if (sonar1DView.CutMouseMode == CutMouseMode.Remove || sonar1DView.CutMouseMode == CutMouseMode.Disable)
                        {
                            // Accept disable selection.
                            sonar1DView.DisableRemoveSelectedCut(sonar1DView.CutMouseMode == CutMouseMode.Remove);
                        }

                        else
                        {
                            // Finish cut.
                            cutMode = CutMode.Nothing;
                            sonarStatusBar.IsCut = true;
                            sonar1DView.CutNow();
                            device.ApplyArchAndVolume(true);
                            project.RecalcVolume();
                            GlobalNotifier.Invoke(this, device, GlobalNotifier.MsgTypes.CutEvent);
                            GlobalNotifier.Invoke(this, project, GlobalNotifier.MsgTypes.UpdateProfiles);
                        }

                        sonar1DView.CutMouseMode = CutMouseMode.Normal;
                        CutModeProp = cutMode;
                    }
                    break;
                case Keys.Left | Keys.Alt:
                    if (!record.Recording)
                        MoveWorkLine(true, false);
                    break;
                case Keys.Left | Keys.Control:
                    if (!record.Recording)
                        MoveWorkLine(true, true);
                    break;
                case Keys.Right | Keys.Alt:
                    if (!record.Recording)
                        MoveWorkLine(false, false);
                    break;
                case Keys.Right | Keys.Control:
                    if (!record.Recording)
                        MoveWorkLine(false, true);
                    break;
                case Keys.Up | Keys.Control:
                    ChangeDevice(true, false);
                    break;
                case Keys.Up | Keys.Alt:
                    ChangeDevice(true, true);
                    break;
                case Keys.Down | Keys.Control:
                    ChangeDevice(false, false);
                    break;
                case Keys.Down| Keys.Alt:
                    ChangeDevice(false,true);
                    break;
                    // function keys
                case Keys.Control | Keys.Z:
                    if (! (sonarStatusBar.ShowHF && sonarStatusBar.ShowNF))
                    {
                        if (sonarStatusBar.ShowHF)
                            sonar1DView.GetCutLine(SonarPanelType.HF).Revert();
                        else
                            sonar1DView.GetCutLine(SonarPanelType.NF).Revert();
                    }
                    break;
                case Keys.F4:
                    if (cutMode == CutMode.Top)
                    {
                        sonar1DView.CutNow();
                        sonarStatusBar.IsCut = true;
                    }
                    device.CalcD(sonarStatusBar.IsCut);
                    if (cutMode == CutMode.Top)
                    {
                        sonarStatusBar.IsCut = false;
                        sonar1DView.GetCutLine(SonarPanelType.HF, false, CutMode.Top).Displace(sonar1DView.GetCutLine(SonarPanelType.HF, false, CutMode.CDepth), 0, false);
                        sonar1DView.GetCutLine(SonarPanelType.NF, false, CutMode.Top).Displace(sonar1DView.GetCutLine(SonarPanelType.NF, false, CutMode.CDepth), 0, false);
                    }
                    break;
                case Keys.F5:
                    sonarStatusBar.ShowHF = !sonarStatusBar.ShowHF;
                    break;
                case Keys.F6:
                    sonarStatusBar.ShowNF = !sonarStatusBar.ShowNF;
                    break;
                case Keys.F7:
                    sonarInfoBar.ShowVol = !sonarInfoBar.ShowVol;
                    break;
                case Keys.F8:
                    sonarStatusBar.IsCut = !sonarStatusBar.IsCut;
                    break;
                case Keys.F9:
                    sonarStatusBar.ShowRuler = !sonarStatusBar.ShowRuler;
                    break;
                case Keys.F10:
                    sonarStatusBar.ShowPos = !sonarStatusBar.ShowPos;
                    break;			
                case Keys.F11:
                    if (GSC.Settings.Lic[Module.Modules.VideoRender])
                    {
                        frmRenderVideo form = new frmRenderVideo(this.project,this.record, this.device, 0, 0);
                        form.ShowDialog();
                    }
                    break;
                case Keys.F12:
                    if (!record.Recording && !useAbsoluteHeights && !(record is SonarProfile))
                    {
                        if (cutMode == CutMode.Nothing)
                        {
                            sonarInfoBar.EditBar.InvCutMode = CutMode.Top;
                            sonar1DView.CutMouseMode = CutMouseMode.Normal;
                            CutModeProp = CutMode.Top;
                            sonarStatusBar.IsCut = false;
                        }
                        else
                        {
                            CutModeProp = CutMode.Nothing;
                            sonarInfoBar.EditMode = EditModes.Nothing;
                            SonarDevice.RefreshCutLine(device.ClSetHF, device.SonarLines);
                            SonarDevice.RefreshCutLine(device.ClSetNF, device.SonarLines);
                            sonarStatusBar.IsCut = true;
                        }
                    }
                    break;
            }

            sonar1DView.RedrawSonarBmp();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        
            e.Handled = true;
        }
        #endregion

        #region Main window changes
        private void cbMainWindow_CheckedChanged(object sender, System.EventArgs e)
        {
            try
            {
                if (cbMainWindow.Checked)
                {
                    if (record.Recording)
                        cbMainWindow.BackColor = Color.DarkRed;
                    else
                        cbMainWindow.BackColor = Color.Gray;

                    cbMainWindow.ImageIndex = 1;
                }
                else
                {
                    if (record.Recording)
                        cbMainWindow.BackColor = Color.Red;
                    else
                        cbMainWindow.BackColor = SystemColors.Control;

                    cbMainWindow.ImageIndex = 0;
                }
            }
            catch
            {
            }
        }
        #endregion

        #region Settings
        void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            bool all = e.PropertyName == "All";
            bool inv = false;

            if (e.PropertyName.Contains("SECL[") || e.PropertyName.StartsWith("Arch") || all)
            {
                sonar1DView.RedrawSonarBmp();
                inv = true;
            }
            
            try
            {
                if (e.PropertyName.StartsWith("CS.") || all)
                {
                    labHorZoom.BackColor = GSC.Settings.CS.BackColor;
                    labHorZoom.ForeColor = GSC.Settings.CS.ForeColor;
                    inv = true;
                }

                if ((e.PropertyName == "DepthTop") || (e.PropertyName == "DepthBottom") || all)
                {
                    UpdateDepthLimits();
                    inv = true;
                }

                if (inv)
                    Invalidate();
            }
            catch
            {
            }

            // Start all user control settings changed notification functions:
            panControl.OnSettingsChanged(sender, e);
            sonar1DView.OnSettingsChanged(sender, e);
            sonarInfoBar.OnSettingsChanged(sender, e);
            sonarStatusBar.OnSettingsChanged(sender, e);
        }

        private void UpdateDepthLimits()
        {
            double bottom = GSC.Settings.DepthBottom;
            double top = GSC.Settings.DepthTop;

            if (useAbsoluteHeights && (device != null))
            {
                top = Math.Ceiling(device.ALMax) + 1;
                bottom = Math.Floor(device.ALMin) - 1 + Math.Max(record.MaxDepth, bottom);
            }

            panControl.DepthTop = (float)top;
            panControl.DepthBottom = (float)bottom;

            sbDepth.RegionWidth = top - bottom;
            sbDepth.SectionStart = 0;
            sbDepth.SectionEnd = top - bottom;

            sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
            sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);

            SetViewMode();
        }
        #endregion

        #region Overrides
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            try
            {
                if ((sbFile == null) ||(labHorZoom == null))
                    return;

                double ratio = sbFile.Ratio * 100.0;
                labHorZoom.Text = ratio.ToString("0") + " %";

                sonar1DView.ViewPort.Left = sbFile.SectionStart;
                sonar1DView.ViewPort.Right = sbFile.SectionEnd;
                sonar1DView.RedrawSonarBmp();
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this,UKLib.Debug.DebugLevel.Red,"frmRecord.OnResize: " + ex.Message);
            }
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            cbMainWindow_CheckedChanged(this, null);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if ((project != null) && (record != null))
                GlobalNotifier.Invoke(this, new RecordEventArgs(record, device, record.GetType()), GlobalNotifier.MsgTypes.DeviceChanged);
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);

            GlobalNotifier.Invoke(this, new RecordEventArgs(null, null), GlobalNotifier.MsgTypes.DeviceChanged);
        }
        #endregion

        #region SurfaceLine popup
        private void cmsPopup_Opening(object sender, CancelEventArgs e)
        {
            if (cutMode == CutMode.Nothing)
            {
                tsmiSurfLine.Visible = true;
                tsmiSurfLineOp.Visible = true;
            }
            else
            {
                tsmiSurfLine.Visible = false;
                tsmiSurfLineOp.Visible = false;
            }
        }

        private void tsmiSurfLine_Click(object sender, EventArgs e)
        {
            tsmiSurfLine.Checked = !tsmiSurfLine.Checked;
            sonar1DView.ShowSurf = tsmiSurfLine.Checked;
        }

        private void tsmiSurfLineOp_Click(object sender, EventArgs e)
        {
            tsmiSurfLineOp.Checked = !tsmiSurfLineOp.Checked;
            sonar1DView.ShowSurfOp = tsmiSurfLineOp.Checked;
        }

        private void tsmiCalcLine_Click(object sender, EventArgs e)
        {
            tsmiCalcLine.Checked = !tsmiCalcLine.Checked;
            sonar1DView.ShowCalc = tsmiCalcLine.Checked;
        }

        private void tsmiCalcLineOp_Click(object sender, EventArgs e)
        {
            tsmiCalcLineOp.Checked = !tsmiCalcLineOp.Checked;
            sonar1DView.ShowCalcOp = tsmiCalcLineOp.Checked;
        }
        #endregion

        #region Mouse events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            /*base.OnMouseMove(e);
        
            if ((cutMode != CutMode.Nothing) || sonar1DView.CtrlHold)
                return;

            Rectangle rcSeperator = new Rectangle(sonar1DView.Location, new Size(sonar1DView.Width, 1));
            rcSeperator.Inflate(0, 1);

            if ((e.Button == MouseButtons.Left) && (Cursor == Cursors.SizeNS))
            {
                PanelYRatio = (double)e.Y / (double)panControl.Height;
                return;
            }

            if (sonarStatusBar.ShowHF && sonarStatusBar.ShowNF && rcSeperator.Contains(e.X, e.Y))
                Cursor = Cursors.SizeNS;
            else
                Cursor = Cursors.Default;*/
        }

        private void OnMouseMove1DView(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseEventArgs e2 = new MouseEventArgs(e.Button, e.Clicks, e.X + sonar1DView.Location.X, e.Y + sonar1DView.Location.Y, e.Delta);
            OnMouseMove(e2);
        }
        #endregion

        #region Work line
        public void MoveWorkLine(bool back, bool fast)
        {
            int newLine = sonar1DView.RefToScreen();
            Sonar1DView.PanelProp pp = sonar1DView.GetProp();
            int pitch = Convert.ToInt32(pp.w);
            if (pitch == 0)
                pitch = 1;
            
            if (back)
            {
                if (fast)
                    newLine -= pitch*5;
                else
                    newLine -= pitch;
            }
            else
            {
                if (fast)
                    newLine += pitch*5;
                else
                    newLine += pitch;
            }

            WorkLine = sonar1DView.ScreenToRef(newLine);
        }

        public void OnWorkLineChanged(object sender, object args)
        {
            try
            {
                RecordEventArgs e = args as RecordEventArgs;

                #region RD8000
                // Set antenna depth for RD8000 measurements.
                // The corresponding line will decide whether the marker is drawn or not (@null).
                sonar1DView.AntDepthLine = e.Tag as SonarLine;
                #endregion
                
                if ((sender is frm1D) || (sender is frmRecord))
                    return;

                if (e.Rec != record)
                    return;

                if (e.Dev != device)
                    return;

                WorkLine = e.Tag as SonarLine;
                if (sbFile.SectionWidth > sonar1DView.Width / 2)
                    tsmiZoom200_Click(this, null);
                sbFile.SectionStart = Math.Max(0, device.IndexOf(workLine) - 50);
                sonar1DView.ViewPort.Left = sbFile.SectionStart;
                sonar1DView.ViewPort.Right = sbFile.SectionEnd;
                sonar1DView.RedrawSonarBmp();
            }
            catch
            {
                WorkLine = null;
            }
        }
        #endregion

        #region Misc functions
        public void ToggleDepthLines()
        {
            sonar1DView.ToggleDepthLines();
        }

        public void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            switch (state)
            {
                case GlobalNotifier.MsgTypes.UpdateRecord:
                    if (!(args is Sonar3DRecord))
                    {
                        cbMainWindow_CheckedChanged(this, null);
                        this.Text = "Record [" + project.IndexOf(record) + "] - Sonar [" + record.IndexOf(device) + "]";
                    }
                    break;
                case GlobalNotifier.MsgTypes.UpdateProfiles:
                    if (record is SonarProfile)
                        sonar1DView.RedrawSonarBmp();
                    break;
                case GlobalNotifier.MsgTypes.RecordingChanged:
                    OnRecordingChanged(sender, args);
                    break;
                case GlobalNotifier.MsgTypes.UpdateCoordinates:
                    if (record is SonarProfile)
                    {
                        List<SonarLine> list = args as List<SonarLine>;
                        if ((list != null) && (list.Count != 0))
                            OnNewSonarLine(device, (args as List<SonarLine>)[0]);
                    }
                    break;
                case GlobalNotifier.MsgTypes.NewSonarLine:
                    OnNewSonarLine(sender, args);
                    break;
                case GlobalNotifier.MsgTypes.NewDeviceList:
                    OnNewDeviceList(sender, args);
                    break;
                case GlobalNotifier.MsgTypes.WorkLineChanged:
                    OnWorkLineChanged(sender, args);
                    break;
                case GlobalNotifier.MsgTypes.PlaceBuoy:
                    if ((ModifierKeys & Keys.Control) == Keys.None)
                        sonar1DView.PlaceSomething = PlaceMode.Nothing;
                    
                    Buoy buoy = args as Buoy;

                    if (!device.SonarLines.Contains(buoy.Tag as SonarLine))
                        break;

                    buoy.Description = record.Description;

                    project.BuoyList.Add(buoy);
                    GSC.Settings.Changed = true;

                    sonar1DView.Invalidate();
                    break;
                case GlobalNotifier.MsgTypes.PlaceManualPoint:
                    if (sender is SonarRecord)
                        break;

                    if ((ModifierKeys & Keys.Control) == Keys.None)
                        sonar1DView.PlaceSomething = PlaceMode.Nothing;

                    ManualPoint point = args as ManualPoint;

                    if (!device.SonarLines.Contains(point.Tag as SonarLine))
                        break;

                    record.AddManualPoint(point);
                    GSC.Settings.Changed = true;

                    sonar1DView.Invalidate();
                    break;
            }
        }

        public void OnRecordingChanged(object sender, object args)
        {
            SonarRecord rec = sender as SonarRecord;

            if (rec == record)
            {
                SetViewMode();

                sonar1DView.Recording = record.Recording;
            }
        }

        public void OnNewSonarLine(object sender, object args)
        {
            if (project.Tracking)
                return;

            if (device != sender as SonarDevice)
                return;

            // Set antenna depth for RD8000 measurements.
            // The corresponding line will decide whether the marker is drawn or not (@null).
            if (record.Recording)
                sonar1DView.AntDepthLine = args as SonarLine;
            else if (record is SonarProfile)
                sonar1DView.AntDepthLine = (record as SonarProfile).LastUpdatedLine;
            else
                sonar1DView.AntDepthLine = null;

            // Update viewport.
            SetViewMode();

            // Draw the workline and then refresh.
            int pos = device.IndexOf(args as SonarLine);
            if (pos != -1)
                sonar1DView.DrawWorkLine(pos);
            sonar1DView.RedrawSonarBmp(true);
        }

        public void OnNewDeviceList(object sender, object args)
        {
            SonarRecord rec = sender as SonarRecord;

            if (rec == null)
                return;
            else if (rec != record)
            {
                // Check, if this record is recording now and if it is created directly after the currently chosen one.
                if (rec.Recording && (project.IndexOf(rec) == project.IndexOf(record) + 1))
                {
                    // In this case, switch record and device, if possible.
                    ChangeDevice(false, false);
                }
            }
        }

        public void SetViewMode()
        {
            if ((device == null) || (record == null))
                return;

            try
            {
                SonarViewMode viewMode = GSC.Settings.ViewMode;

                if (!record.Recording)
                    viewMode = SonarViewMode.LeftFixed;

                double d = sbFile.SectionWidth;
                sbFile.RegionWidth = device.SonarLines.Count;
                sbFile.SetWidth(d);

                switch (viewMode)
                {
                    case SonarViewMode.LeftFixed:
                        sbFile.Enabled = true;
                        break;
                    case SonarViewMode.RightFixed:
                        sbFile.Enabled = false;
                        sbFile.SectionStart = sbFile.RegionWidth-d;
                        break;
                    case SonarViewMode.RadarLike:
                        sbFile.Enabled = false;
                        sbFile.SectionStart = sbFile.RegionWidth-d;
                        break;
                }

                sonar1DView.ViewPort.Left = sbFile.SectionStart;
                sonar1DView.ViewPort.Right = sbFile.SectionEnd;
            }
            catch
            {
            }
        }
        #endregion

        #region Event handler
        public void EditReady(object sender, EditEventArgs e)
        {
            switch (e.EditMode)
            {
                case EditModes.Arch:
                    this.CutModeProp = e.CutMode;
                    device.ApplyArchToCutLine(e.SrcPanelType, cutMode);
                    break;

                case EditModes.Displace:
                    this.CutModeProp = e.CutMode;
                    float offset = 0;

                    if (e.EditString != "d")
                        offset = e.ToSingle(GSC.Settings.NFI);

                    if (cutMode == CutMode.Bottom)
                        offset = -offset;

                    if (e.SrcPanelType == SonarPanelType.HF)
                        sonar1DView.GetCutLine(SonarPanelType.HF, e.SrcPanelType != e.DstPanelType, cutMode).Displace(sonar1DView.GetCutLine(SonarPanelType.HF, false, e.InvCutMode), offset, (e.EditString == "d"));
                    else
                        sonar1DView.GetCutLine(SonarPanelType.NF, e.SrcPanelType != e.DstPanelType, cutMode).Displace(sonar1DView.GetCutLine(SonarPanelType.NF, false, e.InvCutMode), offset, (e.EditString == "d"));
                    break;

                case EditModes.PitchH:                    
                    GSC.Settings.PitchH = e.ToDouble(GSC.Settings.NFI);
                    SonarRecord.ApplyScaleMarks(project, GSC.Settings.PitchH, GSC.Settings.PitchV, sonarStatusBar.IsCut);
                    sonarStatusBar.ShowRuler = true;
                    break;

                case EditModes.PitchV:
                    GSC.Settings.PitchV = e.ToDouble(GSC.Settings.NFI);
                    SonarRecord.ApplyScaleMarks(project, GSC.Settings.PitchH, GSC.Settings.PitchV, sonarStatusBar.IsCut);
                    sonarStatusBar.ShowRuler = true;
                    break;

                case EditModes.SetGlobal:
                    this.CutModeProp = e.CutMode;
                    
                    LineData data;
                    float cut = e.ToSingle(GSC.Settings.NFI);
                    int max = device.SonarLines.Count;

                    for (int i = 0; i < max; i++)
                    {
                        if (e.SrcPanelType == SonarPanelType.HF)
                            data = device.SonarLine(i).HF;
                        else
                            data = device.SonarLine(i).NF;

                        if (cutMode == CutMode.Top)
                            data.TCut = cut;
                        else if (cutMode == CutMode.Bottom)
                            data.BCut = cut;
                    }

                    device.RefreshCutLines();
                    sonarStatusBar.IsCut = true;
                    break;

                case EditModes.ClearCutLine:
                    this.CutModeProp = e.CutMode;
                    
                    if (cutMode == CutMode.Top)
                    {
                        if (e.SrcPanelType == SonarPanelType.HF)
                            device.ClSetHF.CutTop.Clear(0, device.SonarLines.Count - 1, 0);
                        else
                            device.ClSetNF.CutTop.Clear(0, device.SonarLines.Count - 1, 0);
                    }
                    else if (cutMode == CutMode.Bottom)
                    {
                        if (e.SrcPanelType == SonarPanelType.HF)
                            device.ClSetHF.CutBottom.Clear(0, device.SonarLines.Count - 1, -100);
                        else
                            device.ClSetNF.CutBottom.Clear(0, device.SonarLines.Count - 1, -100);
                    }
                    break;
            }

            sonar1DView.RedrawSonarBmp();
        }

        private void sonar1DView_ViewPortChanged(object sender, EventArgs e)
        {
            sbFile.SlideAnchor = SlideBar.SlideAnchors.End;
            sbFile.SectionStart = sonar1DView.ViewPort.Left;
            sbFile.SlideAnchor = SlideBar.SlideAnchors.Width;
            sbFile.SectionEnd = sonar1DView.ViewPort.Right;

            sbDepth.SlideAnchor = SlideBar.SlideAnchors.End;
            sbDepth.SectionStart = sonar1DView.ViewPort.Top;
            sbDepth.SlideAnchor = SlideBar.SlideAnchors.Width;
            sbDepth.SectionEnd = sonar1DView.ViewPort.Bottom;

            sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
            sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);
        }
        
        private void ToggleHF(object sender, System.EventArgs e)
        {
            panControl.ShowHF = sonarStatusBar.ShowHF;
            sonar1DView.ShowHF = sonarStatusBar.ShowHF;
        }

        private void ToggleNF(object sender, System.EventArgs e)
        {
            panControl.ShowNF = sonarStatusBar.ShowNF;
            sonar1DView.ShowNF = sonarStatusBar.ShowNF;
        }

        private void ToggleCUT(object sender, System.EventArgs e)
        {
            sonar1DView.IsCut = sonarStatusBar.IsCut;

            device.ApplyArchAndVolume(sonarStatusBar.IsCut);
            project.RecalcVolume();
        }

        private void TogglePos(object sender, System.EventArgs e)
        {
            sonar1DView.ShowPos = sonarStatusBar.ShowPos;
        }

        private void ToggleBottom()
        {
            sonar1DView.ShowVol = sonarInfoBar.ShowVol;
            sonar1DView.ShowRul = sonarStatusBar.ShowRuler;
            panControl.BottomWidth = (sonar1DView.ShowVol || sonar1DView.ShowRul) ? sonar1DView.InfoBarHeight : 0;
        }

        private void ToggleRuler(object sender, System.EventArgs e)
        {
            ToggleBottom();
        }

        private void ToggleVol(object sender, System.EventArgs e)
        {
            ToggleBottom();
        }

        private void ToggleColor(object sender, System.EventArgs e)
        {
        }

        private void frmRecord_Closed(object sender, System.EventArgs e)
        {
            if (device != null)
            {
                device.HostForm = null;
                device.IsOpen = false;
            }

            if (ParentForm != null)
                ParentForm.RemoveOwnedForm(this);

            if (globalEventHandler != null)
                GlobalNotifier.SignOut(globalEventHandler, GetFilterList());

            GSC.PropertyChanged -= new PropertyChangedEventHandler(OnSettingsChanged);
        }

        private void frmRecord_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (device != null)
            {
                device.HostForm = null;
                device.IsOpen = false;
            }

            if (ParentForm != null)
                ParentForm.RemoveOwnedForm(this);

            if (globalEventHandler != null)
                GlobalNotifier.SignOut(globalEventHandler, GetFilterList());

            GSC.PropertyChanged -= new PropertyChangedEventHandler(OnSettingsChanged);
        }

        private void frmRecord_FormClosing(object sender, FormClosingEventArgs e)
        {
            GlobalNotifier.Invoke(this, new RecordEventArgs(null, null), GlobalNotifier.MsgTypes.DeviceChanged);
        }

        private void frmRecord_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            GlobalNotifier.Invoke(this, new RecordEventArgs(null, null), GlobalNotifier.MsgTypes.DeviceChanged);
        }

        private void frmRecord_VisibleChanged(object sender, System.EventArgs e)
        {
        }
        #endregion
    }
}
