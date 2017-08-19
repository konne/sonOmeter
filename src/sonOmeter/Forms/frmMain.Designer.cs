namespace sonOmeter
{
	public partial class frmMain
	{
			#region Form variables
        private System.ComponentModel.IContainer components = null;

		private System.Windows.Forms.ToolStripMenuItem miViewProject;
		private System.Windows.Forms.MenuStrip mnuMain;
		private System.Windows.Forms.ToolStripMenuItem miFile;
		private System.Windows.Forms.ToolStripMenuItem miFileOpen;
		private System.Windows.Forms.ToolStripMenuItem miFileSave;
		private System.Windows.Forms.ToolStripMenuItem miFileSaveAs;
		private System.Windows.Forms.ToolStripMenuItem miFileExit;
		private System.Windows.Forms.ToolStripMenuItem miHelp;
		private System.Windows.Forms.ToolStripMenuItem miHelpAbout;
		private System.Windows.Forms.ToolStripMenuItem miHelpContents;
		private System.Windows.Forms.ToolStripMenuItem miHelpIndex;
		private System.Windows.Forms.ToolStripMenuItem miHelpSearch;
		private System.Windows.Forms.ToolStripMenuItem miView;
		private System.Windows.Forms.OpenFileDialog dlgOpenRecord;
		private System.Windows.Forms.SaveFileDialog dlgSaveRecord;
		private DockDotNET.DockManager dockManager;
		private System.Windows.Forms.ToolStripMenuItem miViewControls;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel sbpBattery1;
        private System.Windows.Forms.StatusBarPanel sbpBattery2;
		private System.Windows.Forms.ToolStripMenuItem miFileImport;
		private System.Windows.Forms.ToolStripMenuItem miFileImportProject;
		private System.Windows.Forms.ToolStripMenuItem miFileImportRecord;
		private System.Windows.Forms.ToolStripMenuItem miViewCompass;
		private System.Windows.Forms.Timer autoSaveTimer;
		private System.Windows.Forms.StatusBarPanel sbpRecordMarker;

        private System.Windows.Forms.ToolStripMenuItem miViewVolumeArch;
        private System.Windows.Forms.ToolStripMenuItem miFileNew;
        private System.Windows.Forms.ToolStripMenuItem miViewDocuments;
        private System.Windows.Forms.ToolStripMenuItem miViewCloseAllDocuments;
        private System.Windows.Forms.ToolStripMenuItem miViewSettings;
        private System.Windows.Forms.ToolStripMenuItem miTools;
        private System.Windows.Forms.ToolStripMenuItem miViewHorizon;
        private System.Windows.Forms.ToolStripMenuItem miViewSatellites;
        private System.Windows.Forms.ToolStripSeparator miS2;
        private System.Windows.Forms.ToolStripSeparator miS10;
        private System.Windows.Forms.ToolStripSeparator miS9;
        private System.Windows.Forms.Timer tmExit;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miRecord;
        private System.Windows.Forms.ToolStripMenuItem miRecordStartStop;
        private System.Windows.Forms.ToolStripMenuItem miRecordNext;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem miRecordStartStopTracking;
        private System.Windows.Forms.ToolStripSeparator miS1;
        private System.Windows.Forms.ToolStripMenuItem miFileRecentFiles;
        private sonOmeter.Classes.MRUManager mruManager;
        private System.Windows.Forms.ToolStripMenuItem miFileSaveAsCut;
        private System.Windows.Forms.StatusBarPanel spbEnd;
        private System.Windows.Forms.ToolStripMenuItem miToolConfigServer;

        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem miViewManualInput;
        private System.Windows.Forms.ToolStripMenuItem miToolProfile2D;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem miFileImportDOS;
        private System.Windows.Forms.ToolStripMenuItem miToolProfile3D;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem miViewCamera;

		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.miFile = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileNew = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileImport = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileImportProject = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileImportRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miFileImportDOS = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileSave = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileSaveAsCut = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miFileExportSonOmeter2012 = new System.Windows.Forms.ToolStripMenuItem();
            this.miS1 = new System.Windows.Forms.ToolStripSeparator();
            this.miFileRecentFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.miS2 = new System.Windows.Forms.ToolStripSeparator();
            this.miFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.miRecord = new System.Windows.Forms.ToolStripMenuItem();
            this.miRecordStartStop = new System.Windows.Forms.ToolStripMenuItem();
            this.miRecordNext = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miRecordStartStopTracking = new System.Windows.Forms.ToolStripMenuItem();
            this.miView = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewCompass = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewControls = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewHorizon = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewSatellites = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewVolumeArch = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewPositioning = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewRDMeassure = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.miViewManualInput = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.miViewCamera = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewUbootControl = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.miViewSaveLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewOpenLayout = new System.Windows.Forms.ToolStripMenuItem();
            this.miS10 = new System.Windows.Forms.ToolStripSeparator();
            this.miViewDocuments = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewCloseAllDocuments = new System.Windows.Forms.ToolStripMenuItem();
            this.miViewNewRecordWindows = new System.Windows.Forms.ToolStripMenuItem();
            this.miTools = new System.Windows.Forms.ToolStripMenuItem();
            this.miToolConfigServer = new System.Windows.Forms.ToolStripMenuItem();
            this.flashSonarToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miToolInitDLSB30 = new System.Windows.Forms.ToolStripMenuItem();
            this.miToolsProfileSep = new System.Windows.Forms.ToolStripSeparator();
            this.miToolProfile2D = new System.Windows.Forms.ToolStripMenuItem();
            this.miToolProfile3D = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelpContents = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelpIndex = new System.Windows.Forms.ToolStripMenuItem();
            this.miHelpSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.miS9 = new System.Windows.Forms.ToolStripSeparator();
            this.miHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgOpenRecord = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveRecord = new System.Windows.Forms.SaveFileDialog();
            this.dockManager = new DockDotNET.DockManager(this.components);
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.sbpRecordMarker = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.sbpBattery1 = new System.Windows.Forms.StatusBarPanel();
            this.sbpBattery2 = new System.Windows.Forms.StatusBarPanel();
            this.sbpBattery3 = new System.Windows.Forms.StatusBarPanel();
            this.sbpBattery4 = new System.Windows.Forms.StatusBarPanel();
            this.spbEnd = new System.Windows.Forms.StatusBarPanel();
            this.autoSaveTimer = new System.Windows.Forms.Timer(this.components);
            this.tmExit = new System.Windows.Forms.Timer(this.components);
            this.mruManager = new sonOmeter.Classes.MRUManager();
            this.dlgOpenLayout = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveLayout = new System.Windows.Forms.SaveFileDialog();
            this.dlgOpenRecordDemo = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveRecordDemo = new System.Windows.Forms.SaveFileDialog();
            this.mnuMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sbpRecordMarker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpBattery1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpBattery2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpBattery3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpBattery4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spbEnd)).BeginInit();
            this.SuspendLayout();
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFile,
            this.miRecord,
            this.miView,
            this.miTools,
            this.miHelp,
            this.testToolStripMenuItem});
            this.mnuMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(792, 24);
            this.mnuMain.TabIndex = 0;
            this.mnuMain.Text = "mnuMain";
            // 
            // miFile
            // 
            this.miFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileNew,
            this.miFileOpen,
            this.miFileImport,
            this.miFileSave,
            this.miFileSaveAs,
            this.miFileSaveAsCut,
            this.exportToolStripMenuItem,
            this.miS1,
            this.miFileRecentFiles,
            this.miS2,
            this.miFileExit});
            this.miFile.Name = "miFile";
            this.miFile.Size = new System.Drawing.Size(37, 20);
            this.miFile.Text = "&File";
            // 
            // miFileNew
            // 
            this.miFileNew.Name = "miFileNew";
            this.miFileNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.miFileNew.Size = new System.Drawing.Size(208, 22);
            this.miFileNew.Text = "&New";
            this.miFileNew.Click += new System.EventHandler(this.miFileNewProject_Click);
            // 
            // miFileOpen
            // 
            this.miFileOpen.Name = "miFileOpen";
            this.miFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.miFileOpen.Size = new System.Drawing.Size(208, 22);
            this.miFileOpen.Text = "&Open...";
            this.miFileOpen.Click += new System.EventHandler(this.miFileOpen_Click);
            // 
            // miFileImport
            // 
            this.miFileImport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileImportProject,
            this.miFileImportRecord,
            this.toolStripSeparator3,
            this.miFileImportDOS});
            this.miFileImport.Name = "miFileImport";
            this.miFileImport.Size = new System.Drawing.Size(208, 22);
            this.miFileImport.Text = "&Import";
            // 
            // miFileImportProject
            // 
            this.miFileImportProject.Name = "miFileImportProject";
            this.miFileImportProject.Size = new System.Drawing.Size(147, 22);
            this.miFileImportProject.Text = "Project...";
            this.miFileImportProject.Click += new System.EventHandler(this.miFileImportProject_Click);
            // 
            // miFileImportRecord
            // 
            this.miFileImportRecord.Name = "miFileImportRecord";
            this.miFileImportRecord.Size = new System.Drawing.Size(147, 22);
            this.miFileImportRecord.Text = "&Record(s)...";
            this.miFileImportRecord.Click += new System.EventHandler(this.miFileImportRecord_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(144, 6);
            // 
            // miFileImportDOS
            // 
            this.miFileImportDOS.Name = "miFileImportDOS";
            this.miFileImportDOS.Size = new System.Drawing.Size(147, 22);
            this.miFileImportDOS.Text = "DOS Format...";
            this.miFileImportDOS.Click += new System.EventHandler(this.miFileImportDOS_Click);
            // 
            // miFileSave
            // 
            this.miFileSave.Name = "miFileSave";
            this.miFileSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.miFileSave.Size = new System.Drawing.Size(208, 22);
            this.miFileSave.Text = "&Save";
            this.miFileSave.Click += new System.EventHandler(this.miFileSave_Click);
            // 
            // miFileSaveAs
            // 
            this.miFileSaveAs.Name = "miFileSaveAs";
            this.miFileSaveAs.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.miFileSaveAs.Size = new System.Drawing.Size(208, 22);
            this.miFileSaveAs.Text = "Save &As...";
            this.miFileSaveAs.Click += new System.EventHandler(this.miFileSaveAs_Click);
            // 
            // miFileSaveAsCut
            // 
            this.miFileSaveAsCut.Name = "miFileSaveAsCut";
            this.miFileSaveAsCut.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.S)));
            this.miFileSaveAsCut.Size = new System.Drawing.Size(208, 22);
            this.miFileSaveAsCut.Text = "Save As &Cut...";
            this.miFileSaveAsCut.Click += new System.EventHandler(this.miFileSaveAsCut_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miFileExportSonOmeter2012});
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(208, 22);
            this.exportToolStripMenuItem.Text = "&Export";
            this.exportToolStripMenuItem.Visible = false;
            // 
            // miFileExportSonOmeter2012
            // 
            this.miFileExportSonOmeter2012.Name = "miFileExportSonOmeter2012";
            this.miFileExportSonOmeter2012.Size = new System.Drawing.Size(160, 22);
            this.miFileExportSonOmeter2012.Text = "son&Ometer 2012";
            this.miFileExportSonOmeter2012.Click += new System.EventHandler(this.miFileExportSonOmeter2012_Click);
            // 
            // miS1
            // 
            this.miS1.Name = "miS1";
            this.miS1.Size = new System.Drawing.Size(205, 6);
            // 
            // miFileRecentFiles
            // 
            this.miFileRecentFiles.Enabled = false;
            this.miFileRecentFiles.Name = "miFileRecentFiles";
            this.miFileRecentFiles.Size = new System.Drawing.Size(208, 22);
            this.miFileRecentFiles.Text = "&Recent Files";
            // 
            // miS2
            // 
            this.miS2.Name = "miS2";
            this.miS2.Size = new System.Drawing.Size(205, 6);
            // 
            // miFileExit
            // 
            this.miFileExit.Name = "miFileExit";
            this.miFileExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.miFileExit.Size = new System.Drawing.Size(208, 22);
            this.miFileExit.Text = "E&xit";
            this.miFileExit.Click += new System.EventHandler(this.miFileExit_Click);
            // 
            // miRecord
            // 
            this.miRecord.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miRecordStartStop,
            this.miRecordNext,
            this.toolStripSeparator1,
            this.miRecordStartStopTracking});
            this.miRecord.Name = "miRecord";
            this.miRecord.Size = new System.Drawing.Size(56, 20);
            this.miRecord.Text = "&Record";
            // 
            // miRecordStartStop
            // 
            this.miRecordStartStop.Name = "miRecordStartStop";
            this.miRecordStartStop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F1)));
            this.miRecordStartStop.Size = new System.Drawing.Size(189, 22);
            this.miRecordStartStop.Text = "Start";
            this.miRecordStartStop.Click += new System.EventHandler(this.miRecordStartStop_Click);
            // 
            // miRecordNext
            // 
            this.miRecordNext.Enabled = false;
            this.miRecordNext.Name = "miRecordNext";
            this.miRecordNext.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F2)));
            this.miRecordNext.Size = new System.Drawing.Size(189, 22);
            this.miRecordNext.Text = "Next";
            this.miRecordNext.Click += new System.EventHandler(this.miRecordNext_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(186, 6);
            // 
            // miRecordStartStopTracking
            // 
            this.miRecordStartStopTracking.Name = "miRecordStartStopTracking";
            this.miRecordStartStopTracking.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F3)));
            this.miRecordStartStopTracking.Size = new System.Drawing.Size(189, 22);
            this.miRecordStartStopTracking.Text = "Start Tracking";
            this.miRecordStartStopTracking.Click += new System.EventHandler(this.miRecordStartStopTracking_Click);
            // 
            // miView
            // 
            this.miView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miViewCompass,
            this.miViewControls,
            this.miViewHorizon,
            this.miViewProject,
            this.miViewSatellites,
            this.miViewSettings,
            this.miViewVolumeArch,
            this.miViewPositioning,
            this.miViewRDMeassure,
            this.toolStripSeparator2,
            this.miViewManualInput,
            this.toolStripMenuItem1,
            this.miViewCamera,
            this.miViewUbootControl,
            this.toolStripSeparator4,
            this.miViewSaveLayout,
            this.miViewOpenLayout,
            this.miS10,
            this.miViewDocuments,
            this.miViewCloseAllDocuments,
            this.miViewNewRecordWindows});
            this.miView.Name = "miView";
            this.miView.Size = new System.Drawing.Size(44, 20);
            this.miView.Text = "&View";
            this.miView.DropDownOpened += new System.EventHandler(this.miView_Popup);
            // 
            // miViewCompass
            // 
            this.miViewCompass.Name = "miViewCompass";
            this.miViewCompass.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F1)));
            this.miViewCompass.Size = new System.Drawing.Size(275, 22);
            this.miViewCompass.Text = "Compass";
            this.miViewCompass.Click += new System.EventHandler(this.miViewCompass_Click);
            // 
            // miViewControls
            // 
            this.miViewControls.Name = "miViewControls";
            this.miViewControls.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F2)));
            this.miViewControls.Size = new System.Drawing.Size(275, 22);
            this.miViewControls.Text = "Controls";
            this.miViewControls.Click += new System.EventHandler(this.miViewControls_Click);
            // 
            // miViewHorizon
            // 
            this.miViewHorizon.Name = "miViewHorizon";
            this.miViewHorizon.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F3)));
            this.miViewHorizon.Size = new System.Drawing.Size(275, 22);
            this.miViewHorizon.Text = "Horizon";
            this.miViewHorizon.Click += new System.EventHandler(this.miViewHorizon_Click);
            // 
            // miViewProject
            // 
            this.miViewProject.Name = "miViewProject";
            this.miViewProject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F4)));
            this.miViewProject.Size = new System.Drawing.Size(275, 22);
            this.miViewProject.Text = "Project Tree";
            this.miViewProject.Click += new System.EventHandler(this.miViewProject_Click);
            // 
            // miViewSatellites
            // 
            this.miViewSatellites.Name = "miViewSatellites";
            this.miViewSatellites.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F5)));
            this.miViewSatellites.Size = new System.Drawing.Size(275, 22);
            this.miViewSatellites.Text = "Sat Finder";
            this.miViewSatellites.Click += new System.EventHandler(this.miViewSatellites_Click);
            // 
            // miViewSettings
            // 
            this.miViewSettings.Name = "miViewSettings";
            this.miViewSettings.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F6)));
            this.miViewSettings.Size = new System.Drawing.Size(275, 22);
            this.miViewSettings.Text = "Settings";
            this.miViewSettings.Click += new System.EventHandler(this.miViewSettings_Click);
            // 
            // miViewVolumeArch
            // 
            this.miViewVolumeArch.Name = "miViewVolumeArch";
            this.miViewVolumeArch.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F7)));
            this.miViewVolumeArch.Size = new System.Drawing.Size(275, 22);
            this.miViewVolumeArch.Text = "Volume && Arch";
            this.miViewVolumeArch.Click += new System.EventHandler(this.miViewVolume_Click);
            // 
            // miViewPositioning
            // 
            this.miViewPositioning.Name = "miViewPositioning";
            this.miViewPositioning.Size = new System.Drawing.Size(275, 22);
            this.miViewPositioning.Text = "Positioning";
            this.miViewPositioning.Click += new System.EventHandler(this.miViewPositioning_Click);
            // 
            // miViewRDMeassure
            // 
            this.miViewRDMeassure.Name = "miViewRDMeassure";
            this.miViewRDMeassure.Size = new System.Drawing.Size(275, 22);
            this.miViewRDMeassure.Text = "RDMeassure";
            this.miViewRDMeassure.Click += new System.EventHandler(this.miViewRDMeassure_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(272, 6);
            // 
            // miViewManualInput
            // 
            this.miViewManualInput.Name = "miViewManualInput";
            this.miViewManualInput.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F8)));
            this.miViewManualInput.Size = new System.Drawing.Size(275, 22);
            this.miViewManualInput.Text = "Manual Input";
            this.miViewManualInput.Click += new System.EventHandler(this.miViewManualInput_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(272, 6);
            // 
            // miViewCamera
            // 
            this.miViewCamera.Name = "miViewCamera";
            this.miViewCamera.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F9)));
            this.miViewCamera.Size = new System.Drawing.Size(275, 22);
            this.miViewCamera.Text = "Camera";
            this.miViewCamera.Click += new System.EventHandler(this.miViewCamera_Click);
            // 
            // miViewUbootControl
            // 
            this.miViewUbootControl.Name = "miViewUbootControl";
            this.miViewUbootControl.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F10)));
            this.miViewUbootControl.Size = new System.Drawing.Size(275, 22);
            this.miViewUbootControl.Text = "Submarine Control";
            this.miViewUbootControl.Click += new System.EventHandler(this.miViewUbootControl_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(272, 6);
            // 
            // miViewSaveLayout
            // 
            this.miViewSaveLayout.Name = "miViewSaveLayout";
            this.miViewSaveLayout.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.miViewSaveLayout.Size = new System.Drawing.Size(275, 22);
            this.miViewSaveLayout.Text = "Save Window Layout...";
            this.miViewSaveLayout.Click += new System.EventHandler(this.miViewSaveLayout_Click);
            // 
            // miViewOpenLayout
            // 
            this.miViewOpenLayout.Name = "miViewOpenLayout";
            this.miViewOpenLayout.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.W)));
            this.miViewOpenLayout.Size = new System.Drawing.Size(275, 22);
            this.miViewOpenLayout.Text = "Open Window Layout...";
            this.miViewOpenLayout.Click += new System.EventHandler(this.miViewOpenLayout_Click);
            // 
            // miS10
            // 
            this.miS10.Name = "miS10";
            this.miS10.Size = new System.Drawing.Size(272, 6);
            // 
            // miViewDocuments
            // 
            this.miViewDocuments.Name = "miViewDocuments";
            this.miViewDocuments.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.miViewDocuments.Size = new System.Drawing.Size(275, 22);
            this.miViewDocuments.Text = "&Documents...";
            this.miViewDocuments.Click += new System.EventHandler(this.miViewDocuments_Click);
            // 
            // miViewCloseAllDocuments
            // 
            this.miViewCloseAllDocuments.Name = "miViewCloseAllDocuments";
            this.miViewCloseAllDocuments.Size = new System.Drawing.Size(275, 22);
            this.miViewCloseAllDocuments.Text = "&Close All Documents";
            this.miViewCloseAllDocuments.Click += new System.EventHandler(this.CloseDocuments);
            // 
            // miViewNewRecordWindows
            // 
            this.miViewNewRecordWindows.Name = "miViewNewRecordWindows";
            this.miViewNewRecordWindows.Size = new System.Drawing.Size(275, 22);
            this.miViewNewRecordWindows.Text = "New Record Windows";
            this.miViewNewRecordWindows.Click += new System.EventHandler(this.miViewNewRecordWindows_Click);
            // 
            // miTools
            // 
            this.miTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miToolConfigServer,
            this.flashSonarToolStripMenuItem,
            this.miToolInitDLSB30,
            this.miToolsProfileSep,
            this.miToolProfile2D,
            this.miToolProfile3D});
            this.miTools.Name = "miTools";
            this.miTools.Size = new System.Drawing.Size(48, 20);
            this.miTools.Text = "&Tools";
            // 
            // miToolConfigServer
            // 
            this.miToolConfigServer.Name = "miToolConfigServer";
            this.miToolConfigServer.Size = new System.Drawing.Size(145, 22);
            this.miToolConfigServer.Text = "Config Server";
            this.miToolConfigServer.Click += new System.EventHandler(this.miToolConfigServer_Click);
            // 
            // flashSonarToolStripMenuItem
            // 
            this.flashSonarToolStripMenuItem.Name = "flashSonarToolStripMenuItem";
            this.flashSonarToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
            this.flashSonarToolStripMenuItem.Text = "Flash Sonar";
            this.flashSonarToolStripMenuItem.Click += new System.EventHandler(this.flashSonarToolStripMenuItem_Click);
            // 
            // miToolInitDLSB30
            // 
            this.miToolInitDLSB30.Name = "miToolInitDLSB30";
            this.miToolInitDLSB30.Size = new System.Drawing.Size(145, 22);
            this.miToolInitDLSB30.Text = "Init DLSB30";
            this.miToolInitDLSB30.Click += new System.EventHandler(this.miToolInitDLSB30_Click);
            // 
            // miToolsProfileSep
            // 
            this.miToolsProfileSep.Name = "miToolsProfileSep";
            this.miToolsProfileSep.Size = new System.Drawing.Size(142, 6);
            // 
            // miToolProfile2D
            // 
            this.miToolProfile2D.Name = "miToolProfile2D";
            this.miToolProfile2D.Size = new System.Drawing.Size(145, 22);
            this.miToolProfile2D.Text = "2D Profile";
            this.miToolProfile2D.Click += new System.EventHandler(this.miToolProfile2D_Click);
            // 
            // miToolProfile3D
            // 
            this.miToolProfile3D.Name = "miToolProfile3D";
            this.miToolProfile3D.Size = new System.Drawing.Size(145, 22);
            this.miToolProfile3D.Text = "3D Profile";
            this.miToolProfile3D.Click += new System.EventHandler(this.miToolProfile3D_Click);
            // 
            // miHelp
            // 
            this.miHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miHelpContents,
            this.miHelpIndex,
            this.miHelpSearch,
            this.miS9,
            this.miHelpAbout});
            this.miHelp.Name = "miHelp";
            this.miHelp.Size = new System.Drawing.Size(44, 20);
            this.miHelp.Text = "&Help";
            // 
            // miHelpContents
            // 
            this.miHelpContents.Name = "miHelpContents";
            this.miHelpContents.Size = new System.Drawing.Size(169, 22);
            this.miHelpContents.Text = "Contents";
            this.miHelpContents.Click += new System.EventHandler(this.miHelpContents_Click);
            // 
            // miHelpIndex
            // 
            this.miHelpIndex.Name = "miHelpIndex";
            this.miHelpIndex.Size = new System.Drawing.Size(169, 22);
            this.miHelpIndex.Text = "Index";
            this.miHelpIndex.Click += new System.EventHandler(this.miHelpIndex_Click);
            // 
            // miHelpSearch
            // 
            this.miHelpSearch.Enabled = false;
            this.miHelpSearch.Name = "miHelpSearch";
            this.miHelpSearch.Size = new System.Drawing.Size(169, 22);
            this.miHelpSearch.Text = "Search";
            // 
            // miS9
            // 
            this.miS9.Name = "miS9";
            this.miS9.Size = new System.Drawing.Size(166, 6);
            // 
            // miHelpAbout
            // 
            this.miHelpAbout.Name = "miHelpAbout";
            this.miHelpAbout.Size = new System.Drawing.Size(169, 22);
            this.miHelpAbout.Text = "About sonOmeter";
            this.miHelpAbout.Click += new System.EventHandler(this.miHelpAbout_Click);
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.testToolStripMenuItem.Text = "Test";
            this.testToolStripMenuItem.Visible = false;
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // dlgOpenRecord
            // 
            this.dlgOpenRecord.DefaultExt = "xml";
            this.dlgOpenRecord.Filter = "Record files|*.xml;*.sonx|All files|*.*";
            this.dlgOpenRecord.Title = "Open existing record";
            // 
            // dlgSaveRecord
            // 
            this.dlgSaveRecord.DefaultExt = "xml";
            this.dlgSaveRecord.Filter = "Record files|*.xml|All files|*.*";
            this.dlgSaveRecord.Title = "Save current record";
            // 
            // dockManager
            // 
            this.dockManager.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dockManager.BackColor = System.Drawing.Color.Transparent;
            this.dockManager.DockType = DockDotNET.DockContainerType.Document;
            this.dockManager.FastDrawing = true;
            this.dockManager.Location = new System.Drawing.Point(0, 27);
            this.dockManager.Name = "dockManager";
            this.dockManager.Padding = new System.Windows.Forms.Padding(2, 23, 2, 2);
            this.dockManager.Size = new System.Drawing.Size(792, 422);
            this.dockManager.TabIndex = 7;
            this.dockManager.Paint += new System.Windows.Forms.PaintEventHandler(this.dockManager_Paint);
            // 
            // statusBar
            // 
            this.statusBar.Font = new System.Drawing.Font("Lucida Console", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusBar.Location = new System.Drawing.Point(0, 449);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.sbpRecordMarker,
            this.statusBarPanel1,
            this.sbpBattery1,
            this.sbpBattery2,
            this.sbpBattery3,
            this.sbpBattery4,
            this.spbEnd});
            this.statusBar.ShowPanels = true;
            this.statusBar.Size = new System.Drawing.Size(792, 24);
            this.statusBar.TabIndex = 5;
            this.statusBar.DrawItem += new System.Windows.Forms.StatusBarDrawItemEventHandler(this.statusBar_DrawItem);
            // 
            // sbpRecordMarker
            // 
            this.sbpRecordMarker.Name = "sbpRecordMarker";
            this.sbpRecordMarker.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
            this.sbpRecordMarker.Text = "R";
            this.sbpRecordMarker.Width = 24;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Width = 50;
            // 
            // sbpBattery1
            // 
            this.sbpBattery1.Name = "sbpBattery1";
            this.sbpBattery1.Width = 90;
            // 
            // sbpBattery2
            // 
            this.sbpBattery2.Name = "sbpBattery2";
            this.sbpBattery2.Width = 90;
            // 
            // sbpBattery3
            // 
            this.sbpBattery3.Name = "sbpBattery3";
            this.sbpBattery3.Width = 90;
            // 
            // sbpBattery4
            // 
            this.sbpBattery4.Name = "sbpBattery4";
            this.sbpBattery4.Width = 90;
            // 
            // spbEnd
            // 
            this.spbEnd.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.spbEnd.Name = "spbEnd";
            this.spbEnd.Width = 341;
            // 
            // autoSaveTimer
            // 
            this.autoSaveTimer.Tick += new System.EventHandler(this.autoSaveTimer_Tick);
            // 
            // tmExit
            // 
            this.tmExit.Interval = 1000;
            this.tmExit.Tick += new System.EventHandler(this.tmExit_Tick);
            // 
            // mruManager
            // 
            this.mruManager.CurrentDir = "";
            this.mruManager.MaxDisplayNameLength = 40;
            this.mruManager.MaxMRULength = 5;
            this.mruManager.MenuItemMRU = this.miFileRecentFiles;
            this.mruManager.OpenMRUFile += new sonOmeter.Classes.OpenMRUFileHandler(this.mruManager_OpenMRUFile);
            // 
            // dlgOpenLayout
            // 
            this.dlgOpenLayout.DefaultExt = "xml";
            this.dlgOpenLayout.Filter = "Window layout files|*.xml|All files|*.*";
            this.dlgOpenLayout.Title = "Open existing window layout";
            // 
            // dlgSaveLayout
            // 
            this.dlgSaveLayout.DefaultExt = "xml";
            this.dlgSaveLayout.Filter = "Window layout files|*.xml|All files|*.*";
            this.dlgSaveLayout.Title = "Save current window layout";
            // 
            // dlgOpenRecordDemo
            // 
            this.dlgOpenRecordDemo.DefaultExt = "xml";
            this.dlgOpenRecordDemo.Filter = "Record files|*.xml|All files|*.*";
            this.dlgOpenRecordDemo.Title = "Open existing record";
            // 
            // dlgSaveRecordDemo
            // 
            this.dlgSaveRecordDemo.DefaultExt = "xml";
            this.dlgSaveRecordDemo.Filter = "Record files|*.xml|All files|*.*";
            this.dlgSaveRecordDemo.Title = "Save current record";
            // 
            // frmMain
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(792, 473);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.dockManager);
            this.Controls.Add(this.mnuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnuMain;
            this.MinimumSize = new System.Drawing.Size(800, 45);
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "sonOmeter";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Closing += new System.ComponentModel.CancelEventHandler(this.frmMain_Closing);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sbpRecordMarker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpBattery1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpBattery2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpBattery3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpBattery4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spbEnd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );

        }
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem miViewSaveLayout;
        private System.Windows.Forms.ToolStripMenuItem miViewOpenLayout;
        private System.Windows.Forms.OpenFileDialog dlgOpenLayout;
        private System.Windows.Forms.SaveFileDialog dlgSaveLayout;
        private System.Windows.Forms.ToolStripSeparator miToolsProfileSep;
        private System.Windows.Forms.ToolStripMenuItem miViewPositioning;
        private System.Windows.Forms.ToolStripMenuItem flashSonarToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miViewRDMeassure;
        private System.Windows.Forms.ToolStripMenuItem miViewNewRecordWindows;
        private System.Windows.Forms.ToolStripMenuItem miToolInitDLSB30;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miFileExportSonOmeter2012;
        private System.Windows.Forms.OpenFileDialog dlgOpenRecordDemo;
        private System.Windows.Forms.SaveFileDialog dlgSaveRecordDemo;

	}
}