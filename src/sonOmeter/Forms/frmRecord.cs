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

namespace sonOmeter
{
    /// <summary>
    /// Summary description for frmRecord.
    /// </summary>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frmRecord : DockDotNET.DockWindow
    {
        #region Contruction and dispose
        public frmRecord()
        {
            Init();
        }

        public frmRecord(SonarProject prj, SonarRecord rec, int deviceID)
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
                        WorkLine = device.SonarLine(Math.Min(device.SonarLines.Count, dev.IndexOf((dev.HostForm as frmRecord).workLine)));
                        break;
                    }
                }
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
            panHF.ClSetHF = device.ClSetHF;
            panHF.ClSetNF = device.ClSetNF;
            panHF.Recording = record.Recording;
            panHF.RefreshLists(device.SonarLines, project.BuoyList, record.ManualPoints);
            panNF.ClSetHF = device.ClSetHF;
            panNF.ClSetNF = device.ClSetNF;
            panNF.Recording = record.Recording;
            panNF.RefreshLists(device.SonarLines, project.BuoyList, record.ManualPoints);

            // Set view range.
            sbFile.RegionWidth = device.SonarLines.Count;
            sbFile.SectionStart = 0;

            VideoOut = device.SonarLines.Count;

            ResizePanels();
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
            OnSettingsChanged(this, new PropertyChangedEventArgs("All"));

            SetStyle(ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            sonarInfoBar.EditBar.EditReady += new EditEventHandler(EditReady);

            globalEventHandler = new GlobalEventHandler(OnGlobalEvent);
            GlobalNotifier.SignIn(globalEventHandler, GetFilterList());

            sbFile.OnZoom += new ZoomEventHandler(ZoomFile);
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
        double panelYRatio = 0.5;

        private System.Windows.Forms.CheckBox cbMainWindow;
        private System.Windows.Forms.ImageList imgListMainWnd;
        private sonOmeter.SonarInfoBar sonarInfoBar;

        GlobalEventHandler globalEventHandler;

        private int VideoOut = 0;
        #endregion
        
        #region Properties
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
                if (value <= depthZoomLo)
                    return;
                if (value > GSC.Settings.DepthTop)
                    value = GSC.Settings.DepthTop;
                depthZoomHi = value;
                panControl.DepthZoomHi = value;
                panHF.DepthZoomHi = (float)value;
                panNF.DepthZoomHi = (float)value;
                ResetChangeTimer();
            }
        }

        private double DepthZoomLo
        {
            get { return depthZoomLo; }
            set
            {
                if (value >= depthZoomHi)
                    return;
                if (value < GSC.Settings.DepthBottom)
                    value = GSC.Settings.DepthBottom;
                depthZoomLo = value;
                panControl.DepthZoomLo = value;
                panHF.DepthZoomLo = (float)value;
                panNF.DepthZoomLo = (float)value;
                ResetChangeTimer();
            }
        }

        private double PanelYRatio
        {
            set
            {
                if (value*Height < 40)
                    value = 40/(double)Height;
                if (Height-value*Height < 40)
                    value = 1 - 40/(double)Height;
                panelYRatio = value;
                panControl.PanelYRatio = value;
                ResizePanels();
            }
        }
        
        private CutMode CutModeProp
        {
            set
            {
                cutMode = value;
                panHF.CutMode = cutMode;
                panNF.CutMode = cutMode;

                if (sonarInfoBar != null)
                {
                    sonarInfoBar.LabelText = "Cut mode switched to: " + cutMode + "    [" + panHF.CutMouseMode + "]";
                }
            }
        }

        [Browsable(false)]
        private SonarLine WorkLine
        {
            set
            {
                workLine = value;

                if (sonarStatusBar.ShowHF)
                {
                    panHF.DrawWorkLine(true, null);
                    panHF.WorkLine = workLine;
                    panHF.DrawWorkLine(false, null);
                }
                else
                    panHF.WorkLine = workLine;

                workLine = panHF.WorkLine;

                if (sonarStatusBar.ShowNF)
                {
                    panNF.DrawWorkLine(true, null);
                    panNF.WorkLine = workLine;
                    panNF.DrawWorkLine(false, null);
                }
                else
                    panNF.WorkLine = workLine;

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
            switch (e.ZoomEventType)
            {
                case ZoomEventArgs.ZoomEventTypes.Start:
                    DepthZoomHi = -e.ZoomValue + GSC.Settings.DepthTop; ;
                    break;
                case ZoomEventArgs.ZoomEventTypes.End:
                    DepthZoomLo = -e.ZoomValue + GSC.Settings.DepthTop;
                    break;
                case ZoomEventArgs.ZoomEventTypes.Position:
                    double temp = depthZoomHi + e.ZoomValue - GSC.Settings.DepthTop;
                    DepthZoomHi = depthZoomHi - temp;
                    DepthZoomLo = depthZoomLo - temp;
                    break;
            }
        }
        
        private void ZoomFile(object sender, ZoomEventArgs e)
        {
            OnResize(new System.EventArgs());

            WorkLine = workLine;
            ResetChangeTimer();
        }

        private void tsmiZoom50_Click(object sender, EventArgs e)
        {
            sbFile.SetWidth(panHF.Width * 2);

            OnResize(new System.EventArgs());
        }

        private void tsmiZoom100_Click(object sender, EventArgs e)
        {
            sbFile.SetWidth(panHF.Width);

            OnResize(new System.EventArgs());
        }

        private void tsmiZoom200_Click(object sender, EventArgs e)
        {
            sbFile.SetWidth(panHF.Width / 2);

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

            panHF.CtrlHold = e.Control;
            panNF.CtrlHold = e.Control;
            panHF.AltHold = e.Alt;
            panNF.AltHold = e.Alt;
            panHF.ShiftHold = e.Shift;
            panNF.ShiftHold = e.Shift;
            
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
                        panNF.PlaceSomething = PlaceMode.ManualPoint;
                        panNF.PlaceMarker(0.0F, device.GetLastLineWithCoord(), false);
                        panNF.PlaceSomething = PlaceMode.Nothing;
                    }
                    break;
                case Keys.A | Keys.Control:
                case Keys.L | Keys.Control:
                    if (cutMode == CutMode.Nothing)
                        panNF.PlaceSomething = panHF.PlaceSomething = PlaceMode.ManualPoint;
                    break;
                case Keys.L:
                    if (cutMode == CutMode.Nothing)
                        ToggleDepthLines();
                    break;
                case Keys.Q:
                    if ((cutMode == CutMode.Nothing) && project.Recording)
                    {
                        panNF.PlaceSomething = PlaceMode.Buoy;
                        panNF.PlaceMarker(0.0F, device.GetLastLineWithCoord(), false);
                        panNF.PlaceSomething = PlaceMode.Nothing;
                    }
                    break;
                case Keys.Q | Keys.Control:
                case Keys.B | Keys.Control:
                    if (cutMode == CutMode.Nothing)
                        panNF.PlaceSomething = panHF.PlaceSomething = PlaceMode.Buoy;
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
                case Keys.V:
                    sonarInfoBar.EditMode = EditModes.PitchV;
                    sonarInfoBar.EditString = GSC.Settings.PitchV.ToString(GSC.Settings.NFI);
                    break;
                case Keys.P:
                    if ((panHF.CutMouseMode == CutMouseMode.Paint) | ((cutMode != CutMode.Top) & (cutMode != CutMode.Bottom)))
                    {
                        panHF.CutMouseMode = CutMouseMode.Normal;
                        panNF.CutMouseMode = CutMouseMode.Normal;
                    }
                    else
                    {
                        panHF.CutMouseMode = CutMouseMode.Paint;
                        panNF.CutMouseMode = CutMouseMode.Paint;
                    }

                    CutModeProp = cutMode;
                    break;
                case Keys.E:
                    if ((panHF.CutMouseMode == CutMouseMode.Rubber) | ((cutMode != CutMode.Top) & (cutMode != CutMode.Bottom)))
                    {
                        panHF.CutMouseMode = CutMouseMode.Normal;
                        panNF.CutMouseMode = CutMouseMode.Normal;
                    }
                    else
                    {
                        panHF.CutMouseMode = CutMouseMode.Rubber;
                        panNF.CutMouseMode = CutMouseMode.Rubber;
                    }

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
                    break;
                case Keys.PageDown:
                    sbFile.SectionStart -= sbFile.SectionWidth;
                    break;
                case Keys.Home:
                    sbFile.SectionStart = 0;
                    break;
                case Keys.End:
                    sbFile.SectionStart = sbFile.RegionWidth-sbFile.SectionWidth;
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
                    panNF.PlaceSomething = panHF.PlaceSomething = PlaceMode.Nothing;
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
                        CutModeProp = CutMode.Nothing;
                        sonarStatusBar.IsCut = true;
                        panHF.CutNow();
                        panNF.CutNow();
                        device.ApplyArchAndVolume(true);
                        project.RecalcVolume();
                        GlobalNotifier.Invoke(this, project, GlobalNotifier.MsgTypes.CutEvent);
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
                case Keys.F4:
                    if (cutMode == CutMode.Top)
                    {
                        panHF.CutNow();
                        panNF.CutNow();
                        sonarStatusBar.IsCut = true;
                    }
                    RecalcDepth();
                    if (cutMode == CutMode.Top)
                    {
                        sonarStatusBar.IsCut = false;
                        panHF.GetCutLine(false, CutMode.Top).Displace(panHF.GetCutLine(false, CutMode.CDepth), 0, false);
                        panNF.GetCutLine(false, CutMode.Top).Displace(panNF.GetCutLine(false, CutMode.CDepth), 0, false);
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
                        frmRenderVideo form = new frmRenderVideo(this.project, this.record, this.device, 0, 0);
                        form.ShowDialog();
                    }
                    break;
                case Keys.F12:
                    if (!record.Recording && !(record is SonarProfile))
                    {
                        if (cutMode == CutMode.Nothing)
                        {
                            sonarInfoBar.EditBar.InvCutMode = CutMode.Top;
                            panHF.CutMouseMode = CutMouseMode.Normal;
                            panNF.CutMouseMode = CutMouseMode.Normal;
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

            ResetChangeTimer();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
        
            e.Handled = true;

            panHF.CtrlHold = e.Control;
            panNF.CtrlHold = e.Control;
            panHF.AltHold = e.Alt;
            panNF.AltHold = e.Alt;
            panHF.ShiftHold = e.Shift;
            panNF.ShiftHold = e.Shift;
        }
        #endregion

        #region CalcDeep
        private float RecalcHFNF(LineData data, float lastdepth, float subDepth)
        {
            float max1 = float.NaN;
            float max1c = -1;

            if (data.Entries != null)
            {
                for (int j = 0; j < data.Entries.Length; j++)
                {
                    if ((sonarStatusBar.IsCut & data.Entries[j].visible) | !sonarStatusBar.IsCut)
                    {
                        float high = ((sonarStatusBar.IsCut) ? data.Entries[j].high : data.Entries[j].uncutHigh) - subDepth;
                        float low = data.Entries[j].low - subDepth;
                        float dl = data.Entries[j].colorID;

                        if (j + 1 < data.Entries.Length)
                        {
                            float high1 = ((sonarStatusBar.IsCut) ? data.Entries[j + 1].high : data.Entries[j + 1].uncutHigh) - subDepth;

                            if (low - high1 > GSC.Settings.CalcdMaxSpace)
                                dl = -1;
                        }
                        if (!float.IsNaN(lastdepth))
                        {
                            if (Math.Abs(lastdepth - high) > GSC.Settings.CalcdMaxChange)
                            {
                                dl = -1;
                            }
                        }

                        if (dl > max1c)
                        {
                            max1c = dl;
                            max1 = high;
                        }
                    }
                }
                data.CDepth = max1;
                if (max1 <= 0)
                    lastdepth = max1;
            }
            return lastdepth;
        }

        private void Average()
        {
            int cnt = device.SonarLines.Count;
            int avg = GSC.Settings.CalcdAverage;

            for (int i = 0; i < cnt; i++)
            {
                float averageHF = 0F;
                float averageNF = 0F;
                int anzHF = 0;
                int anzNF = 0;
                for (int j = -avg; j <= avg; j++)
                {
                    if ((i + j >= 0) & (i+j < cnt))
                    {
                        if (!float.IsNaN(device.SonarLines[i + j].HF.CDepth))
                        {
                            anzHF++;
                            averageHF += device.SonarLines[i + j].HF.CDepth;
                        }
                        if (!float.IsNaN(device.SonarLines[i + j].NF.CDepth))
                        {
                            anzNF++;
                            averageNF += device.SonarLines[i + j].NF.CDepth;
                        }
                    }
                }
                averageHF = averageHF / anzHF;
                averageNF = averageNF / anzNF;
                device.SonarLines[i].HF.CDepth = averageHF;
            }  
        }

        private void RecalcDepth()
        {
            if (GSC.Settings.Lic[Module.Modules.CalcDepth])
            {

                float lastdepthHF = float.NaN;
                float lastdepthNF = float.NaN;
                int i;
                for (i = 0; i < device.SonarLines.Count; i++)
                {
                    SonarLine line = device.SonarLines[i];

                    lastdepthHF = RecalcHFNF(line.HF, lastdepthHF, line.SubDepth);
                    lastdepthNF = RecalcHFNF(line.NF, lastdepthNF, line.SubDepth);
                }
                Average();

                SonarDevice.RefreshCutLine(device.ClSetHF, CutMode.CDepth, device.SonarLines, GSC.Settings.CalcdLinearisation);
                SonarDevice.RefreshCutLine(device.ClSetNF, CutMode.CDepth, device.SonarLines, GSC.Settings.CalcdLinearisation);
            }
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
                ResetChangeTimer();
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
                    sbDepth.RegionWidth = GSC.Settings.DepthTop - GSC.Settings.DepthBottom;
                    sbDepth.SectionStart = -depthZoomHi - GSC.Settings.DepthTop;
                    sbDepth.SectionEnd = -depthZoomLo - GSC.Settings.DepthTop;

                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.Start);
                    sbDepth.InvokeZoom(ZoomEventArgs.ZoomEventTypes.End);

                    SetViewMode();
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
            panHF.OnSettingsChanged(sender, e);
            panNF.OnSettingsChanged(sender, e);
            sonarInfoBar.OnSettingsChanged(sender, e);
            sonarStatusBar.OnSettingsChanged(sender, e);
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

                ResizePanels();

                double ratio = sbFile.Ratio * 100.0;
                labHorZoom.Text = ratio.ToString("0") + " %";              
                ResetChangeTimer();
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

            panHF.CtrlHold = false;
            panNF.CtrlHold = false;
            panHF.AltHold = false;
            panNF.AltHold = false;
        }
        #endregion

        #region ChangeTimer
        private void OnChangeTimer(object sender, System.EventArgs e)
        {
            changeTimer.Stop();

            if (sonarStatusBar.ShowHF)
                panHF.RedrawSonarBmp();
            if (sonarStatusBar.ShowNF)
                panNF.RedrawSonarBmp();			
        }

        public void ResetChangeTimer()
        {
            changeTimer.Stop(); // sollte das Problem mit den nicht zu sehenden Sonardaten sein
            changeTimer.Start();
        }

        public void StartChangeTimer()
        {
            changeTimer.Start();
        }
      

        public void RefreshChangeTimer()
        {
            if (sonarStatusBar.ShowHF)
                panHF.RedrawSonarBmp();
            if (sonarStatusBar.ShowNF)
                panNF.RedrawSonarBmp();
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
            panHF.ShowSurfLHF = panNF.ShowSurfLNF = tsmiSurfLine.Checked;
        }

        private void tsmiSurfLineOp_Click(object sender, EventArgs e)
        {
            tsmiSurfLineOp.Checked = !tsmiSurfLineOp.Checked;
            panHF.ShowSurfLNF = panNF.ShowSurfLHF = tsmiSurfLineOp.Checked;
        }

        private void tsmiCalcLine_Click(object sender, EventArgs e)
        {
            tsmiCalcLine.Checked = !tsmiCalcLine.Checked;
            panHF.ShowCalcLHF = panNF.ShowCalcLNF = tsmiCalcLine.Checked;
        }

        private void tsmiCalcLineOp_Click(object sender, EventArgs e)
        {
            tsmiCalcLineOp.Checked = !tsmiCalcLineOp.Checked;
            panHF.ShowCalcLNF = panNF.ShowCalcLHF = tsmiCalcLineOp.Checked;
        }
        #endregion

        #region Mouse events
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
        
            if ((cutMode != CutMode.Nothing) || panHF.CtrlHold)
                return;

            Rectangle rcSeperator = new Rectangle(panNF.Location, new Size(panNF.Width, 1));
            rcSeperator.Inflate(0, 1);

            if ((e.Button == MouseButtons.Left) && (Cursor == Cursors.SizeNS))
            {
                PanelYRatio = (double)e.Y / (double)panControl.Height;
                return;
            }

            if (sonarStatusBar.ShowHF && sonarStatusBar.ShowNF && rcSeperator.Contains(e.X, e.Y))
                Cursor = Cursors.SizeNS;
            else
                Cursor = Cursors.Default;
        }

        #region panHF
        private void OnMouseMoveHF(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseEventArgs e2 = new MouseEventArgs(e.Button, e.Clicks, e.X+panHF.Location.X, e.Y+panHF.Location.Y, e.Delta);
            OnMouseMove(e2);
        }
        #endregion

        #region panNF
        private void OnMouseMoveNF(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseEventArgs e2 = new MouseEventArgs(e.Button, e.Clicks, e.X+panNF.Location.X, e.Y+panNF.Location.Y, e.Delta);
            OnMouseMove(e2);
        }
        #endregion
        #endregion

        #region Work line
        public void MoveWorkLine(bool back, bool fast)
        {
            int newLine = panHF.RefToScreen();
            int pitch = Convert.ToInt32(panHF.Width/sbFile.SectionWidth);
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

            WorkLine = panHF.ScreenToRef(newLine);
        }

        public void OnWorkLineChanged(object sender, object args)
        {
            try
            {
                RecordEventArgs e = args as RecordEventArgs;

                if ((sender is frm1D) || (sender is frmRecord))
                    return;

                if (e.Rec != record)
                    return;

                if (e.Dev != device)
                    return;

                WorkLine = e.Tag as SonarLine;
                if (sbFile.SectionWidth > panHF.Width / 2)
                    tsmiZoom200_Click(this, null);
                sbFile.SectionStart = Math.Max(0, device.IndexOf(workLine) - 50);
            }
            catch
            {
                WorkLine = null;
            }

            RefreshChangeTimer();
        }
        #endregion

        #region Misc functions
        public void ToggleDepthLines()
        {
            panHF.ToggleDepthLines();
            panNF.ToggleDepthLines();
        }

        private void ResizePanels()
        {
            try
            {
                if ((panControl == null) || (panHF == null) || (panNF == null))
                    return;

                if (panHF.Parent == null)
                    return;

                int x = panControl.Location.X + panControl.Width + 1;
                int y = (int)((double)panControl.Height*panelYRatio);
                int w = panHF.Parent.Width;

                if (sonarStatusBar.ShowHF && sonarStatusBar.ShowNF)
                {
                    panHF.Size = new Size(w-x, y);
                    panHF.Location = new Point(x, 0);
                    panHF.Visible = true;

                    panNF.Size = new Size(w-x, panControl.Height-y);
                    panNF.Location = new Point(x, y);
                    panNF.Visible = true;
                }
                else if (sonarStatusBar.ShowHF)
                {
                    panHF.Size = new Size(w-x, panControl.Height);
                    panHF.Location = new Point(x, 0);
                    panHF.Visible = true;

                    panNF.Visible = false;
                }
                else
                {
                    panHF.Visible = false;

                    panNF.Size = new Size(w-x, panControl.Height);
                    panNF.Location = new Point(x, 0);
                    panNF.Visible = true;
                }

                ResetChangeTimer();
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this,UKLib.Debug.DebugLevel.Red,"frmRecord.ResizePanels: " + ex.Message);
            }
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
                case GlobalNotifier.MsgTypes.RecordingChanged:
                    OnRecordingChanged(sender, args);
                    break;
                case GlobalNotifier.MsgTypes.UpdateCoordinates:
                    if (record is SonarProfile)
                        OnNewSonarLine(device, (args as List<SonarLine>)[0]);
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
                    if (!panHF.ShiftHold)
                        panNF.PlaceSomething = panHF.PlaceSomething = PlaceMode.Nothing;
                    
                    Buoy buoy = args as Buoy;

                    if (!device.SonarLines.Contains(buoy.Tag as SonarLine))
                        break;

                    buoy.Description = record.Description;

                    project.BuoyList.Add(buoy);
                    GSC.Settings.Changed = true;

                    panHF.AddMarker(buoy);
                    panNF.AddMarker(buoy);

                    panHF.Invalidate();
                    panNF.Invalidate();
                    break;
                case GlobalNotifier.MsgTypes.PlaceManualPoint:
                    if (sender is SonarRecord)
                        break;

                    if (!panHF.ShiftHold)
                        panNF.PlaceSomething = panHF.PlaceSomething = PlaceMode.Nothing;

                    ManualPoint point = args as ManualPoint;

                    if (!device.SonarLines.Contains(point.Tag as SonarLine))
                        break;

                    record.AddManualPoint(point);
                    GSC.Settings.Changed = true;

                    panHF.AddMarker(point);
                    panNF.AddMarker(point);

                    panHF.Invalidate();
                    panNF.Invalidate();
                    break;
            }
        }

        public void OnRecordingChanged(object sender, object args)
        {
            SonarRecord rec = sender as SonarRecord;

            if (rec == record)
            {
                SetViewMode();

                panHF.Recording = record.Recording;
                panNF.Recording = record.Recording;

                RefreshChangeTimer();
            }
        }

        public void OnNewSonarLine(object sender, object args)
        {
            if (project.Tracking)
                return;

            if (device != sender as SonarDevice)
                return;

            SetViewMode();

            int n = device.SonarLines.Count-1;

            if (sonarStatusBar.ShowHF)
                panHF.DrawWorkLine(n);
            if (sonarStatusBar.ShowNF)
                panNF.DrawWorkLine(n);

            if (sonarStatusBar.ShowHF)
                panHF.RedrawSonarBmp(true);
            if (sonarStatusBar.ShowNF)
                panNF.RedrawSonarBmp(true);
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
                    
                    if (e.SrcPanelType == SonarPanelType.HF)
                        panHF.Invalidate();
                    else
                        panHF.Invalidate();
                    break;

                case EditModes.Displace:
                    this.CutModeProp = e.CutMode;
                    float offset = 0;

                    if (e.EditString != "d")
                        offset = e.ToSingle(GSC.Settings.NFI);

                    if (cutMode == CutMode.Bottom)
                        offset = -offset;

                    if (e.SrcPanelType == SonarPanelType.HF)
                        panHF.GetCutLine(e.SrcPanelType != e.DstPanelType, cutMode).Displace(panHF.GetCutLine(false, e.InvCutMode), offset, (e.EditString == "d"));
                    else
                        panNF.GetCutLine(e.SrcPanelType != e.DstPanelType, cutMode).Displace(panNF.GetCutLine(false, e.InvCutMode), offset, (e.EditString == "d"));
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
            
            ResetChangeTimer();
        }
        
        private void ToggleHF(object sender, System.EventArgs e)
        {
            panControl.ShowHF = sonarStatusBar.ShowHF;
            ResizePanels();
        }

        private void ToggleNF(object sender, System.EventArgs e)
        {
            panControl.ShowNF = sonarStatusBar.ShowNF;
            ResizePanels();
        }

        private void ToggleCUT(object sender, System.EventArgs e)
        {
            panHF.IsCut = sonarStatusBar.IsCut;
            panNF.IsCut = sonarStatusBar.IsCut;

            device.ApplyArchAndVolume(sonarStatusBar.IsCut);
            project.RecalcVolume();

            ResizePanels();
        }

        private void TogglePos(object sender, System.EventArgs e)
        {
            panHF.ShowPos = sonarStatusBar.ShowPos;
            panNF.ShowPos = sonarStatusBar.ShowPos;
            ResizePanels();
        }

        private void ToggleBottom()
        {
            if (sonarStatusBar.ShowRuler || sonarInfoBar.ShowVol)
            {
                int w = 16;
                panHF.BottomWidth = w;
                panNF.BottomWidth = w;
                panControl.BottomWidth = w;
            }
            else
            {
                panHF.BottomWidth = 0;
                panNF.BottomWidth = 0;
                panControl.BottomWidth = 0;
            }

            panHF.ShowVol = sonarInfoBar.ShowVol;
            panNF.ShowVol = sonarInfoBar.ShowVol;
            panHF.ShowRul = sonarStatusBar.ShowRuler;
            panNF.ShowRul = sonarStatusBar.ShowRuler;

            ResizePanels();
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
            ResizePanels();
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
            ResizePanels();
        }
        #endregion       
    }
}
