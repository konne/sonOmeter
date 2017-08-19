using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using DockDotNET;
using SingleInstance;
using sonOmeter.Classes;
using sonOmeter.Properties;
using sonOmeter.Server.Classes;
using sonOmeter.Server.Config;
using sonOmeter.SonarFlashLoader;
using UKLib.Debug;


namespace sonOmeter
{
    /// <summary>
    /// This is the Mainform
    /// </summary>
    public partial class frmMain : System.Windows.Forms.Form
    {
        #region Variables
        private SonarProject prjMain;

        private frmProject formPrj;
        private frmVolWeight formVolWeight;

        private frm2D form2D;
        private frm3D form3D;
        private frmCompass formCompass;
        private frmHorizon formHorizon;
        private frmSat formSat;
        private frmControlBig formControl;
        private frmSettings formSettings;
        private frmDebug formDebug;
        private frmManualInput formManualInput;
        private frmCamera formCamera;
        private frmUbootControl formUbootControl;
        private frmPositioning formPositioning;

        private frmRDMeasure formRDMeasure;

        private bool blink = false;
        private ToolStripMenuItem miViewUbootControl;
        private StatusBarPanel sbpBattery4;
        private StatusBarPanel sbpBattery3;

        private sonOmeterServerClass serv = new sonOmeterServerClass();

        private bool no3Dlib = false;

        string Revision = "";
        #endregion

        #region SVNRevision
        private void SetSVNRevision()
        {
           
        }
        #endregion

        #region Create, Load and Dispose
        public frmMain()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            GSC.Settings = new GlobalSettings();

            GSC.PropertyChanged += new PropertyChangedEventHandler(GSC_PropertyChanged);

            DockManager.FastMoveDraw = false;
            DockManager.FormLoad += new FormLoadEventHandler(DockManager_FormLoad);

//            testToolStripMenuItem.Visible = true;
//            exportToolStripMenuItem.Visible = true;

            miToolProfile3D.Enabled = GSC.Settings.Lic[Module.Modules.ThreeD];

            miViewOpenLayout.Enabled = GSC.Settings.Lic[Module.Modules.V21];
            miViewSaveLayout.Enabled = GSC.Settings.Lic[Module.Modules.V21];
            miToolProfile2D.Enabled = GSC.Settings.Lic[Module.Modules.V21];

            miViewPositioning.Enabled = GSC.Settings.Lic[Module.Modules.V22];
            flashSonarToolStripMenuItem.Enabled = GSC.Settings.Lic[Module.Modules.V22];
            if (GSC.Settings.Lic[Module.Modules.V22])
            {
                this.dlgSaveRecord.Filter = "Record files|*.xml|Compressed binary project|*.sonx|All files|*.*";
            }

            miViewUbootControl.Enabled = GSC.Settings.Lic[Module.Modules.Submarine];
            miViewCamera.Enabled = GSC.Settings.Lic[Module.Modules.Submarine];

            miViewRDMeassure.Enabled = GSC.Settings.Lic[Module.Modules.RadioDetection];

            miViewNewRecordWindows.Checked = Settings.Default.UseNewRecordWindows;

            this.Closing += new CancelEventHandler(mruManager.OnOwnerClosing);
        }

        void GSC_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                if (e.PropertyName == "Settings")
                {
                    serv.Settings.PosDeviceList = GSC.Settings.PosDeviceList;
                    serv.Settings.SonDeviceList = GSC.Settings.SonDeviceList;
                }
            }
            catch
            {
            }
        }

        private void DockManager_FormLoad(object sender, FormLoadEventArgs e)
        {
            if (e.Type == formPrj.GetType())
                e.Form = formPrj;
            else if (e.Type == formVolWeight.GetType())
                e.Form = formVolWeight;
            else if (e.Type == form2D.GetType())
                e.Form = form2D;
            else if (e.Type == form3D.GetType())
                e.Form = form3D;
            else if ((e.Type == formManualInput.GetType()) && GSC.Settings.Lic[Module.Modules.ThreeD])
                e.Form = formManualInput;
            else if (e.Type == formControl.GetType())
                e.Form = formControl;
            else if (e.Type == formDebug.GetType())
                e.Form = formDebug;
            else if (e.Type == formCompass.GetType())
                e.Form = formCompass;
            else if (e.Type == formHorizon.GetType())
                e.Form = formHorizon;
            else if (e.Type == formSat.GetType())
                e.Form = formSat;
            else if (e.Type == formSettings.GetType())
                e.Form = formSettings;
            else if (e.Type == formRDMeasure.GetType() && GSC.Settings.Lic[Module.Modules.RadioDetection])
                e.Form = formRDMeasure;
            else if ((e.Type == formUbootControl.GetType()) && GSC.Settings.Lic[Module.Modules.Submarine])
                e.Form = formUbootControl;
            else if ((e.Type == formCamera.GetType()) && GSC.Settings.Lic[Module.Modules.Submarine])
                e.Form = formCamera;
            else if ((e.Type == formPositioning.GetType()) && GSC.Settings.Lic[Module.Modules.V22])
                e.Form = formPositioning;
            else
            {
                e.Cancel = true;
                e.Form = null;
            }
        }

        private void frmMain_Load(object sender, System.EventArgs e)
        {
            SetSVNRevision();

            this.Text = "sonOmeter - " + Revision + " [new File]";

            try
            {
                if (sonOmeter.Properties.Settings.Default.Setting != null)
                {
                    GSC.ReadXmlString(sonOmeter.Properties.Settings.Default.Setting);
                }
                GSC.PropertyChanged += new PropertyChangedEventHandler(OnSettingsChanged);

                formDebug = new frmDebug(this);
                formDebug.HideOnClose = true;
                Application.DoEvents();

                prjMain = new SonarProject();
                prjMain.SynchronizingObject = this;
                Application.DoEvents();

                formPrj = new frmProject();
                formPrj.Project = prjMain;
                formPrj.HideOnClose = true;
                Application.DoEvents();

                formVolWeight = new frmVolWeight();
                formVolWeight.HideOnClose = true;
                Application.DoEvents();

                form2D = new frm2D();
                form2D.Project = prjMain;
                form2D.HideOnClose = true;
                form2D.Visible = false;
                Application.DoEvents();

                try
                {
                    form3D = new frm3D(prjMain);
                    form3D.HideOnClose = true;
                    form3D.Visible = false;
                }
                catch (Exception ex)
                {
                    if (GSC.Settings.Lic[Module.Modules.ThreeD])
                    {
                        DebugClass.SendDebugLine(this, DebugLevel.Red, "form3D " + ex.Message);

                        if (ex is System.IO.FileNotFoundException)
                        {
                            if (MessageBox.Show("The SlimDX library is not installed!\nThe 3D module needs this library and is therefore deactivated.", "Warning", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                                Application.Exit();
                            no3Dlib = true;
                            miToolProfile3D.Enabled = false;
                        }
                        else
                        {
                            if (MessageBox.Show("The 3D module failed on initialization!\nPlease report the error in the debug window.", "3D Initialization Error", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.Cancel)
                                Application.Exit();
                        }
                    }
                }
                Application.DoEvents();

                formManualInput = new frmManualInput();
                formManualInput.HideOnClose = true;
                formManualInput.Project = prjMain;
                Application.DoEvents();

                formControl = new frmControlBig();
                formControl.HideOnClose = true;
                Application.DoEvents();

                formCompass = new frmCompass(prjMain);
                formCompass.HideOnClose = true;
                Application.DoEvents();

                formHorizon = new frmHorizon(prjMain);
                formHorizon.HideOnClose = true;
                Application.DoEvents();

                formSat = new frmSat(prjMain);
                formSat.HideOnClose = true;
                Application.DoEvents();

                formPositioning = new frmPositioning(prjMain);
                formPositioning.HideOnClose = true;
                Application.DoEvents();

                formSettings = new frmSettings();
                formSettings.SelectedObject = GSC.Settings;
                formSettings.HideOnClose = true;
                Application.DoEvents();

                formControl.OnBigControlClick += new BigControlerHandler(formControl_OnBigControlClick);
                formPrj.ShowSonar += new ProjectEventHandler(formPrj_ShowSonar);
                formPrj.ShowManualPoints += new ProjectEventHandler(formPrj_ShowManualPoints);
                formPrj.ShowPositions += new ProjectEventHandler(formPrj_ShowPositions);
                formPrj.Show2D += new ProjectEventHandler(formPrj_Show2D);
                formPrj.Show3D += new ProjectEventHandler(formPrj_Show3D);
                Application.DoEvents();

                formUbootControl = new frmUbootControl(this, prjMain);
                formUbootControl.HideOnClose = true;
                //formUbootControl.Visible = false;
                formUbootControl.UbootBatteryChanged += new frmUbootControl.UbootBatteryEventHandler(formUbootControl_UbootBatteryChanged);


                formCamera = new frmCamera(prjMain);
                //formCamera.Visible = false;
                formCamera.HideOnClose = true;

                formRDMeasure = new frmRDMeasure(prjMain);
                formRDMeasure.HideOnClose = true;

                Application.DoEvents();

                dockManager.DockWindow(formPrj, DockStyle.Left);
                formPrj.HostContainer.DockWindow(formCompass, DockStyle.Bottom);
                formCompass.HostContainer.DockWindow(formHorizon, DockStyle.Fill);
                formCompass.HostContainer.DockWindow(formSat, DockStyle.Fill);
                if (GSC.Settings.Lic[Module.Modules.V22])
                    formCompass.HostContainer.DockWindow(formPositioning, DockStyle.Fill);
                formCompass.HostContainer.DockWindow(formManualInput, DockStyle.Fill);
                formCompass.HostContainer.DockWindow(formVolWeight, DockStyle.Fill);
                dockManager.DockWindow(formDebug, DockStyle.Bottom);
                formDebug.HostContainer.DockWindow(formControl, DockStyle.Fill);
                formPrj.HostContainer.DockWindow(formSettings, DockStyle.Fill);
                Application.DoEvents();

                formPrj.HostContainer.SelectTab(0);

                GlobalNotifier.SignIn(new GlobalEventHandler(OnLineFound), GlobalNotifier.MsgTypes.LineFound);
                GlobalNotifier.SignIn(new GlobalEventHandler(OnNewSonarLine), GlobalNotifier.MsgTypes.NewSonarLine);

                if (GSC.Settings.ServerAutoStart) StartServer();

                // Load autosave or temporary data if existing and if the user wants it. Otherwise, delete it.
                if (!prjMain.ReadAutoSave())
                    SonarProject.ClearTempPath();
            }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message);
                MessageBox.Show(ex.Message);
            }
        }


        // tbd in den formen lösen
        //void SECL_ListChanged(object sender, ListChangedEventArgs e)
        //{
        //    if (e.ListChangedType == ListChangedType.ItemChanged)
        //    {
        //        prjMain.RecalcVolume();

        //        if (form3D != null)
        //            form3D.Update3DPanel();
        //    }
        //}      

        void formUbootControl_UbootBatteryChanged(int battery, double voltage)
        {
            switch (battery + 1)
            {
                case 1:
                    sbpBattery1.Text = "Bat1: " + voltage.ToString("0.0") + "V";
                    break;
                case 2:
                    sbpBattery2.Text = "Bat2: " + voltage.ToString("0.0") + "V";
                    break;
                case 3:
                    sbpBattery3.Text = "Bat3: " + voltage.ToString("0.0") + "V";
                    break;
                case 4:
                    sbpBattery4.Text = "Bat4: " + voltage.ToString("0.0") + "V";
                    break;

            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            StopServer();
        }

        #endregion

        #region Start/Stop Server
        private void StartServer()
        {
            try
            {
                serv.Settings.SonDeviceList = GSC.Settings.SonDeviceList;
                serv.Settings.PosDeviceList = GSC.Settings.PosDeviceList;
                serv.Settings.SonID0Timer = GSC.Settings.ServerSonID0Timer;
                serv.StartConfigServer();
                serv.StartDataServer();
            }
            catch
            {

            }
        }

        private void StopServer()
        {
            try
            {
                serv.StopDataServer();
                serv.StopConfigServer();
            }
            catch
            {
            }
        }
        #endregion

        #region MainThread and SplashThread

        static Thread splashThread = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        private static frmSplashScreen formSplash = new frmSplashScreen(false);
        static frmMain formMain = new frmMain();

        static void DoSplash()
        {            
            formSplash.lbRegisteredText = "";
            formSplash.Show();
            formSplash.StartSplash();
            while (!formSplash.isReady())
            {
                formSplash.DoSplash();
                formSplash.Invalidate();
                Application.DoEvents();
                Thread.Sleep(80);
            }
            formMain.Startup();
        }

        delegate void StartupCallback();

        public void Startup()
        {
            if (this.InvokeRequired)
            {
                StartupCallback d = new StartupCallback(Startup);
                this.Invoke(d, null);
            }
            else
            {
                formMain.BringToFront();
                formMain.Focus();
            }
        }

        [STAThread]
        static void Main()
        {
            GlobalSettings settings = new GlobalSettings();

            splashThread = new Thread(new ThreadStart(DoSplash));
            splashThread.IsBackground = true;
            splashThread.TrySetApartmentState(ApartmentState.STA);
            splashThread.Name = "Splash";
            splashThread.Start();
            SingleApplication.Run(formMain);

            GlobalNotifier.BeepThread.Stop();
        }
        #endregion

        #region General functions
        private void SetFilenameCaption(string filename)
        {
            this.Text = "sonOmeter - " + Revision + " [" + filename + "]";
        }

        private void frmMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                // Request save, if something changed.
                if (GSC.Settings.Changed)
                {
                    DialogResult ret = MessageBox.Show("Do you want to save changes?", "The project changed since last save.", MessageBoxButtons.YesNoCancel);

                    if (ret == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }
                    else if (ret == DialogResult.Yes)
                        miFileSave_Click(this, EventArgs.Empty);

                    string fileName = AppDomain.CurrentDomain.BaseDirectory + "AutoSave.xml";
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                    }
                }

                // Dispose.
                prjMain.Dispose();
                SonarProject.ClearTempPath();

                // Close all windows.
                formPrj.HideOnClose = false;
                formVolWeight.HideOnClose = false;
                formControl.HideOnClose = false;
                form2D.HideOnClose = false;
                if (form3D != null)
                    form3D.HideOnClose = false;
                formControl.HideOnClose = false;

                formPrj.Close();
                formPrj.Dispose();
                formPrj = null;

                try
                {
                    Sonar3DCell.DisposeStatic();
                    Sonar3DWall.DisposeStatic();
                }
                catch
                {
                    // Drop these errors silently...
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "An error occured during closing.");
            }
        }
        #endregion

        #region Project window
        private void formPrj_ShowSonar(object sender, ProjectEventArgs e)
        {
            // If window already open, then do nothing, else set IsOpen flag.
            if (e.Record.Devices[e.Id].IsOpen)
                return;
            else
                e.Record.Devices[e.Id].IsOpen = true;

            // Create and show new record form.
            DockWindow form;

            if (miViewNewRecordWindows.Checked)
                form = new frm1D(prjMain, e.Record, Convert.ToInt16(e.Id));
            else
                form = new frmRecord(prjMain, e.Record, Convert.ToInt16(e.Id));

            e.Record.Devices[e.Id].HostForm = form;

            DockContainer cont = dockManager.GetNextChild(DockContainerType.Document, null);
            if (cont == null)
                cont = dockManager;
            cont.DockWindow(form, DockStyle.Fill);
        }

        void formPrj_ShowManualPoints(object sender, ProjectEventArgs e)
        {
            frmManualPoints form = new frmManualPoints(prjMain, e.Record);
            dockManager.DockWindow(form, DockStyle.Fill);
        }

        private void formPrj_ShowPositions(object sender, ProjectEventArgs e)
        {
            frmPositions form = new frmPositions(prjMain, e.Record);
            dockManager.DockWindow(form, DockStyle.Fill);
        }

        private void formPrj_Show2D(object sender, ProjectEventArgs e)
        {
            // Create and show new 2D form.

            DockContainer cont = dockManager.GetNextChild(DockContainerType.Document, null);
            if (cont == null)
                cont = dockManager;
            cont.DockWindow(form2D, DockStyle.Fill);
        }

        private void formPrj_Show3D(object sender, ProjectEventArgs e)
        {
            // Create and show new 3D form.
            if ((form3D != null) && !no3Dlib)
            {
                DockContainer cont = dockManager.GetNextChild(DockContainerType.Document, null);
                if (cont == null)
                    cont = dockManager;
                cont.DockWindow(form3D, DockStyle.Fill);
            }
        }
        #endregion

        #region Arch and Volume Weight window
        public void OpenVolWeightWindow()
        {
            // tbd
            formVolWeight.Show();
        }
        #endregion

        #region File menu
        private void miFileNewProject_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show("Do you want to create a new project?", "Please confirm", MessageBoxButtons.YesNo) != System.Windows.Forms.DialogResult.Yes)
                return;

            CloseDocuments(this, null);
            prjMain.Clear();
            SonarProject.ClearTempPath();
            SetFilenameCaption("");
        }

        private void miFileOpen_Click(object sender, System.EventArgs e)
        {
            var dlg = dlgOpenRecord;

            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            mruManager_OpenMRUFile(dlg.FileName);
        }

        private void mruManager_OpenMRUFile(string fileName)
        {
            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            CloseDocuments(this, null);
            prjMain.Clear();

            try
            {
                prjMain.Read(fileName);
                FileInfo fileInfo = new FileInfo(fileName);
                mruManager.CurrentDir = fileInfo.DirectoryName;
                mruManager.Add(fileName);
            }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message);
                mruManager.Remove(fileName);
            }

            GlobalNotifier.Invoke(this, GSC.Settings, GlobalNotifier.MsgTypes.SwitchProperties);

            form2D.RebuildTraceArray();
            form2D.CoordLimitChange(true);
            formManualInput.RebuildList();
            SetFilenameCaption(fileName);
            this.Cursor = Cursors.Default;
        }

        private void miFileImportProject_Click(object sender, System.EventArgs e)
        {
            var dlg = dlgOpenRecord;

            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            this.Cursor = Cursors.WaitCursor;
            Application.DoEvents();

            prjMain.Read(dlg.FileName);

            form2D.CoordLimitChange(true);

            this.Cursor = Cursors.Default;
        }

        private void miFileImportRecord_Click(object sender, System.EventArgs e)
        {
            var dlg =  dlgOpenRecord;

            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            this.Cursor = Cursors.WaitCursor;

            frmImport form = new frmImport();
            form.FileName = dlg.FileName;
            form.MainProject = prjMain;
            form.ShowDialog();

            form2D.CoordLimitChange(true);

            this.Cursor = Cursors.Default;
        }

        private void miFileImportDOS_Click(object sender, EventArgs e)
        {
            frmSON2XML form = new frmSON2XML(prjMain);

            form.ShowDialog();
        }

        private void miFileSave_Click(object sender, System.EventArgs e)
        {
            if (prjMain.NewFile)
            {
                miFileSaveAs_Click(sender, e);
                return;
            }           

            // Save the existing record object
            this.Cursor = Cursors.WaitCursor;
            prjMain.IsBinary = prjMain.FileName.EndsWith("sonx");
            prjMain.Write();
            this.Cursor = Cursors.Default;
        }

        private void miFileSaveAs_Click(object sender, System.EventArgs e)
        {
            var dlg =  dlgSaveRecord;

            dlg.FileName = prjMain.FileName;
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            // Save the existing record object
            this.Cursor = Cursors.WaitCursor;

            prjMain.IsBinary = dlg.FileName.EndsWith("sonx");

            if (!prjMain.IsBinary && (prjMain.DXFFiles.Count > 0))
            {
                if (MessageBox.Show("All DXF file links will be lost! Continue?", "Warning", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
                {
                    this.Cursor = Cursors.Default;
                    return;
                }
            }

            prjMain.Write(dlg.FileName);
            SetFilenameCaption(dlg.FileName);
            FileInfo fileInfo = new FileInfo(dlg.FileName);
            mruManager.CurrentDir = fileInfo.DirectoryName;
            mruManager.Add(dlg.FileName);

            this.Cursor = Cursors.Default;
        }

        private void miFileSaveAsCut_Click(object sender, EventArgs e)
        {
            var dlg = dlgSaveRecord;

            dlg.FileName = prjMain.FileName;
            if (dlg.ShowDialog(this) != DialogResult.OK)
                return;

            // Check if file is used.
            if (File.Exists(dlg.FileName))
            {
                MessageBox.Show("It is not allowed to overwrite the currently opened file with this function. A severe loss of data may be caused.", "Command blocked", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }

            // Save the existing record object
            this.Cursor = Cursors.WaitCursor;

            prjMain.DropInvisibleData();
            prjMain.IsBinary = GSC.Settings.Lic[Module.Modules.V22] && dlg.FileName.EndsWith("sonx");
            prjMain.Write(dlg.FileName);
            SetFilenameCaption(dlg.FileName);
            FileInfo fileInfo = new FileInfo(dlg.FileName);
            mruManager.CurrentDir = fileInfo.DirectoryName;
            mruManager.Add(dlg.FileName);

            this.Cursor = Cursors.Default;
        }

        private void miFileExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void miFileExportSonOmeter2012_Click(object sender, EventArgs e)
        {
            //var dlg = new sonOmeter.Export.Export2sonOmeter2012(prjMain);

            //dlg.ShowDialog();
        }
        #endregion

        #region Record menu
        private void miRecordStartStop_Click(object sender, EventArgs e)
        {
            if (miRecordStartStop.Text == "Start")
                formControl_OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RecordStart));
            else
                formControl_OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RecordStop));
        }

        private void miRecordNext_Click(object sender, EventArgs e)
        {
            formControl_OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RecordStop));
            formControl_OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RecordStart));
        }

        private void miRecordStartStopTracking_Click(object sender, EventArgs e)
        {
            if (miRecordStartStopTracking.Text == "Start Tracking")
                formControl_OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.TrackingStart));
            else
                formControl_OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.TrackingStop));
        }
        #endregion

        #region View menu
        private void miViewSaveLayout_Click(object sender, EventArgs e)
        {
            if (dlgSaveLayout.ShowDialog() != DialogResult.OK)
                return;

            DockManager.WriteXml(dlgSaveLayout.FileName);
        }

        private void miViewOpenLayout_Click(object sender, EventArgs e)
        {
            if (dlgOpenLayout.ShowDialog() != DialogResult.OK)
                return;

            DockManager.CloseAll();
            DockManager.ReadXml(dlgOpenLayout.FileName);
        }

        private void miViewPositioning_Click(object sender, EventArgs e)
        {
            if (formPositioning != null)
                formPositioning.Visible = !formPositioning.Visible;
        }

        private void CloseDocuments(object sender, System.EventArgs e)
        {
            try
            {
                DockManager.CloseDocuments();
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "frmMain.CloseDocuments: " + ex.Message);
            }
        }

        private void miViewNewRecordWindows_Click(object sender, EventArgs e)
        {
            miViewNewRecordWindows.Checked = !miViewNewRecordWindows.Checked;
            Settings.Default.UseNewRecordWindows = miViewNewRecordWindows.Checked;
        }

        private void miView_Popup(object sender, System.EventArgs e)
        {
            if (formPrj != null)
                miViewProject.Checked = formPrj.Visible;
            if (formControl != null)
                miViewControls.Checked = formControl.Visible;
            if (formCompass != null)
                miViewCompass.Checked = formCompass.Visible;
            if (formVolWeight != null)
                miViewVolumeArch.Checked = formVolWeight.Visible;
            if (formSettings != null)
                miViewSettings.Checked = formSettings.Visible;
            if (formSat != null)
                miViewSatellites.Checked = formSat.Visible;
            if (formHorizon != null)
                miViewHorizon.Checked = formHorizon.Visible;
            if (formManualInput != null)
                miViewManualInput.Checked = formManualInput.Visible;
            if (formUbootControl != null)
                miViewUbootControl.Checked = formUbootControl.Visible;
            if (formCamera != null)
                miViewCamera.Checked = formCamera.Visible;
            if (formPositioning != null)
                miViewPositioning.Checked = formPositioning.Visible;
        }

        private void miViewManualInput_Click(object sender, EventArgs e)
        {
            if (formManualInput != null)
                formManualInput.Visible = !formManualInput.Visible;
        }

        private void miViewProject_Click(object sender, System.EventArgs e)
        {
            if (formPrj != null)
                formPrj.Visible = !formPrj.Visible;
        }

        private void miViewSettings_Click(object sender, System.EventArgs e)
        {
            if (formSettings != null)
                formSettings.Visible = !formSettings.Visible;
        }

        private void miViewControls_Click(object sender, System.EventArgs e)
        {
            if (formControl != null)
                formControl.Visible = !formControl.Visible;
        }

        private void miViewCompass_Click(object sender, System.EventArgs e)
        {
            if (formCompass != null)
                formCompass.Visible = !formCompass.Visible;
        }

        private void miViewVolume_Click(object sender, System.EventArgs e)
        {
            if (formVolWeight != null)
                formVolWeight.Visible = !formVolWeight.Visible;
        }

        private void miViewDocuments_Click(object sender, System.EventArgs e)
        {
            frmDocList form = new frmDocList();
            form.Manager = dockManager;
            form.ShowDialog();
        }

        private void miViewSatellites_Click(object sender, System.EventArgs e)
        {
            if (formSat != null)
                formSat.Visible = !formSat.Visible;
        }

        private void miViewHorizon_Click(object sender, System.EventArgs e)
        {
            if (formHorizon != null)
                formHorizon.Visible = !formHorizon.Visible;
        }

        private void miViewCamera_Click(object sender, EventArgs e)
        {
            if (formCamera.Visible)
            {
                formCamera.Visible = false;
            }
            else
            {
                DockContainer cont = dockManager.GetNextChild(DockContainerType.Document, null);
                if (cont == null)
                    cont = dockManager;
                cont.DockWindow(formCamera, DockStyle.Fill);
            }
        }

        private void miViewUbootControl_Click(object sender, EventArgs e)
        {
            if (formUbootControl != null)
                formUbootControl.Visible = !formUbootControl.Visible;
        }

        private void miViewRDMeassure_Click(object sender, EventArgs e)
        {
            if (formRDMeasure != null)
                formRDMeasure.Visible = !formRDMeasure.Visible;
        }
        #endregion

        #region Tools Menu
        private void miToolConfigServer_Click(object sender, EventArgs e)
        {
            frmSonarConfig frmConf = new frmSonarConfig(serv);
            frmConf.ShowDialog();
        }

        private void flashSonarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            serv.StopDataServer();
            frmFlashSonar frmFlash = new frmFlashSonar();
            frmFlash.ShowDialog();
            serv.StartDataServer();
        }

        private void miToolProfile2D_Click(object sender, EventArgs e)
        {
            frmProfile form = new frmProfile(prjMain);
            form.ShowDialog();
        }

        private void miToolProfile3D_Click(object sender, EventArgs e)
        {
            frmPrepare3D form = new frmPrepare3D(prjMain);
            form.ShowDialog();
        }

        private void miToolInitDLSB30_Click(object sender, EventArgs e)
        {
            serv.InitDLSB30();
        }
        #endregion

        #region Settings ApplyChanges
        void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("All") | e.PropertyName.StartsWith("SECL[") | e.PropertyName.StartsWith("Arch"))
            {
                prjMain.ApplyArchAndVolume();
                prjMain.RecalcVolume();
            }

            if (GSC.Settings.AutoSave > 0)
            {
                autoSaveTimer.Interval = (int)(GSC.Settings.AutoSave * 60000.0);
                autoSaveTimer.Enabled = true;
                autoSaveTimer.Start();
            }
            else
            {
                autoSaveTimer.Stop();
                autoSaveTimer.Enabled = false;
            }

            if (e.PropertyName.Equals("ServerDeviceList"))
            {

            }
        }
        #endregion

        #region Help menu
        private void miHelpContents_Click(object sender, EventArgs e)
        {
            Help.ShowHelp(this, Path.GetDirectoryName(Application.ExecutablePath) + "\\Chm\\sonOmeter.chm");
        }

        private void miHelpIndex_Click(object sender, EventArgs e)
        {
            Help.ShowHelpIndex(this, Path.GetDirectoryName(Application.ExecutablePath) + "\\Chm\\sonOmeter.chm");
        }

        private void miHelpAbout_Click(object sender, System.EventArgs e)
        {
            frmSplashScreen frm = new frmSplashScreen(true);
            frm.lbRegisteredText = "";
            AddOwnedForm(frm);
            frm.ShowDialog();
        }
        #endregion

        #region TestForm
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //var dt2Pos = new Dictionary<DateTime, SonarPos>();

            //var tr = File.OpenText(@"C:\Users\konne\Desktop\Messdaten\10121201.pkZ");
            //while (!tr.EndOfStream)
            //{
            //    var line = tr.ReadLine();
            //    var datas = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //    if (datas.Length > 3)
            //    {
            //        var dt = DateTime.Parse("28.10.2010 " + datas[0]);
            //        var rw = Double.Parse(datas[1], CultureInfo.InvariantCulture);
            //        var hw = Double.Parse(datas[2], CultureInfo.InvariantCulture);
            //        var al = Double.Parse(datas[3], CultureInfo.InvariantCulture);

            //        string posString = "37=" + hw.ToString("0.000", CultureInfo.InvariantCulture) + ";38=" + rw.ToString("0.000", CultureInfo.InvariantCulture) + ";39=" + al.ToString("0.000", CultureInfo.InvariantCulture) + ";";
            //        var sp = new SonarPos(dt, PosType.Geodimeter, false, posString);

            //        if (!dt2Pos.ContainsKey(dt))
            //            dt2Pos.Add(dt, sp);
            //        else
            //            dt2Pos[dt] = sp;
            //    }
            //}
            //Console.WriteLine("j,h");
            //for (int i = 0; i < prjMain.RecordCount; i++)
            //{
            //    var rec = prjMain.Record(i);
            //    var dev0 = rec.Devices[0];
            //    for (int i2 = 0; i2 < dev0.SonarLines.Count; i2++)
            //    {
            //        var line = dev0.SonarLines[i2];
            //        var dt = line.Time;
            //        dt = dt.AddMilliseconds(-dt.Millisecond);
            //        if (dt2Pos.ContainsKey(dt))
            //        {

            //            line.PosList.Add(dt2Pos[dt]);
            //        }
            //    }

            //    //rec.Devices[0].SonarLines[0].
            //}

            //tr.Close();
            //Console.WriteLine("j,h");

            //line.PosList.Add(new SonarPos(DateTime.Now, PosType.Fixed, false, posString));

            frmTest form = new frmTest(serv);
            form.Show();
        }
        #endregion

        #region AutoSave
        private void autoSaveTimer_Tick(object sender, System.EventArgs e)
        {
            try
            {
                autoSaveTimer.Enabled = false;
                prjMain.WriteAutoSave();
                autoSaveTimer.Enabled = true;
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "frmMain.autoSaveTimer_Tick: " + ex.Message);
            }
        }
        #endregion

        #region Paint Dockmanager
        private void dockManager_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
          
        }
        #endregion

        #region Statusbar
        private void statusBar_DrawItem(object sender, System.Windows.Forms.StatusBarDrawItemEventArgs sbdevent)
        {
            try
            {
                if ((sbdevent.Panel == sbpRecordMarker) && (prjMain != null))
                {
                    Graphics g = sbdevent.Graphics;

                    if (blink && prjMain.Recording)
                        g.Clear(SystemColors.ControlDark);
                    else
                        g.Clear(SystemColors.Control);

                    if (prjMain.Recording)
                    {
                        RectangleF rc = new RectangleF(0, 0, (float)sbpRecordMarker.Width, (float)statusBar.Height);
                        StringFormat sf = new StringFormat(StringFormat.GenericDefault);
                        sf.LineAlignment = sf.Alignment = StringAlignment.Center;
                        Font font = new Font(sbdevent.Font, FontStyle.Bold);
                        g.DrawString("R", font, new SolidBrush(Color.Black), rc, sf);
                    }
                }
            }
            catch
            {
            }
        }
        #endregion

        #region NewSonarLine
        private void OnNewSonarLine(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            blink = !blink;
            statusBar.Invalidate();
        }
        #endregion

        #region ExitTimer
        private void tmExit_Tick(object sender, System.EventArgs e)
        {
            frmSplashScreen from = new frmSplashScreen(true);
            //frmSplashScreen.
            Close();
        }
        #endregion

        #region BigControls
        private void formControl_OnBigControlClick(object sender, BigControlEventArgs e)
        {
            switch (e.BigControlEvent)
            {
                case BigControlEventType.RecordStart:
                    if (prjMain.Tracking)
                        formControl_OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.TrackingStop));

                    miRecordStartStop.Text = "Stop";
                    miRecordNext.Enabled = true;

                    if (GSC.Settings.AutoSave > 0)
                    {
                        autoSaveTimer.Interval = (int)(GSC.Settings.AutoSave * 60000.0);
                        autoSaveTimer.Enabled = true;
                        autoSaveTimer.Start();
                    }
                    prjMain.StartRec(false, false);
                    statusBar.Invalidate();
                    break;
                case BigControlEventType.RecordStop:
                    miRecordStartStop.Text = "Start";
                    miRecordNext.Enabled = false;

                    autoSaveTimer.Stop();
                    prjMain.StopRec();
                    statusBar.Invalidate();
                    break;
                case BigControlEventType.ProjectSave:
                    miFileSave_Click(this, EventArgs.Empty);
                    break;
                case BigControlEventType.TrackingStart:
                    if (prjMain.Recording)
                        formControl_OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RecordStop));

                    miRecordStartStopTracking.Text = "Stop Track";
                    prjMain.StartRec(true, false);
                    statusBar.Invalidate();
                    break;
                case BigControlEventType.TrackingStop:
                    miRecordStartStopTracking.Text = "Start Track";
                    prjMain.StopRec();
                    statusBar.Invalidate();
                    break;
                case BigControlEventType.RestartServer:
                    StopServer();
                    Application.DoEvents();
                    Application.DoEvents();
                    Application.DoEvents();
                    Application.DoEvents();
                    StartServer();
                    break;
                case BigControlEventType.ManualInput:
                    if (formManualInput.Visible)
                        formManualInput.Focus();
                    else
                        formManualInput.Visible = true;
                    break;
                case BigControlEventType.Simulate:
                    miRecordStartStop.Text = "Stop";

                    if (GSC.Settings.AutoSave > 0)
                    {
                        autoSaveTimer.Interval = (int)(GSC.Settings.AutoSave * 60000.0);
                        autoSaveTimer.Enabled = true;
                        autoSaveTimer.Start();
                    }
                    prjMain.StartRec(false, true);
                    statusBar.Invalidate();
                    break;
            }

            formControl.RefreshButtons(e.BigControlEvent);
        }
        #endregion

        #region OnLineFound

        private void OnLineFound(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            try
            {
                RecordEventArgs e = args as RecordEventArgs;

                if (e.Dev.HostForm != null)
                {
                    DockWindow form = e.Dev.HostForm as DockWindow;
                    if (form.IsDocked)
                        form.HostContainer.SelectTab(form.ControlContainer);
                    else
                        form.BringToFront();
                }
                else
                {
                    ProjectEventArgs e2 = new ProjectEventArgs(e.Rec.IndexOf(e.Dev), e.Rec, ProjectEventType.ShowSonar);
                    formPrj_ShowSonar(this, e2);
                }

                GlobalNotifier.Invoke(this, args, GlobalNotifier.MsgTypes.WorkLineChanged);
            }
            catch
            {
            }
        }
        #endregion
    }
}