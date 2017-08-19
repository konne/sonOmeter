namespace sonOmeter
{
	public partial class frmProject
	{
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
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

		private UKLib.TreeView.CoolTreeView tvProject;
		private System.Windows.Forms.SaveFileDialog dlgSave;
		private System.Windows.Forms.ContextMenuStrip cms;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripMenuItem tsmiInterpolate;
        private System.Windows.Forms.ToolStripMenuItem tsmiExport;
        private System.Windows.Forms.ToolStripSeparator tsmiSep;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpen;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenConfig;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenBlankLineXml;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenBuoys;
        private System.Windows.Forms.ToolStripMenuItem tsmiSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveConfig;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveBlankLineXml;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveBuoys;
        private System.Windows.Forms.OpenFileDialog openDlg;
        private System.Windows.Forms.SaveFileDialog saveDlg;
        private System.Windows.Forms.ToolStripSeparator tsmiOpenSep;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenBlankLineSurfer;
        private System.Windows.Forms.ToolStripSeparator tsmiSaveSep;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveBlankLineSurfer;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveBlankLineXmlLALO;
        private System.Windows.Forms.ToolStripMenuItem tsmiSaveBlankLineSurferLALO;
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProject));
            this.tvProject = new UKLib.TreeView.CoolTreeView();
            this.cms = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiSnapshot = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEmpty = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiInterpolate = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExport = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiExportSurfer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiProperties = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSep = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenBuoys = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenSep = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiOpenBlankLineXml = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenBlankLineSurfer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiOpenDA040 = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenPKT = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveBuoys = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveSep = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSaveBlankLineXml = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveBlankLineXmlLALO = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveBlankLineSurfer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSaveBlankLineSurferLALO = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenDirect = new System.Windows.Forms.ToolStripMenuItem();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.openDlg = new System.Windows.Forms.OpenFileDialog();
            this.saveDlg = new System.Windows.Forms.SaveFileDialog();
            this.tsmiEnableAll = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDisableAll = new System.Windows.Forms.ToolStripMenuItem();
            this.cms.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvProject
            // 
            this.tvProject.ContextMenuStrip = this.cms;
            resources.ApplyResources(this.tvProject, "tvProject");
            this.tvProject.ImageIndex = 0;
            this.tvProject.Name = "tvProject";
            this.tvProject.SelectedImageIndex = 0;
            this.tvProject.SelectedNode = null;
            this.tvProject.NodeClick += new System.Windows.Forms.TreeViewEventHandler(this.tvProject_NodeClick);
            this.tvProject.NodeDoubleClick += new System.Windows.Forms.TreeViewEventHandler(this.tvProject_NodeDoubleClick);
            this.tvProject.NodeStateChanged += new System.Windows.Forms.TreeViewEventHandler(this.tvProject_NodeStateChanged);
            this.tvProject.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tvProject_KeyDown);
            // 
            // cms
            // 
            this.cms.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiEnableAll,
            this.tsmiDisableAll,
            this.tsmiSnapshot,
            this.tsmiDelete,
            this.tsmiEmpty,
            this.tsmiInterpolate,
            this.tsmiExport,
            this.tsmiExportSurfer,
            this.tsmiProperties,
            this.tsmiEdit,
            this.tsmiSep,
            this.tsmiOpen,
            this.tsmiSave,
            this.tsmiOpenDirect});
            this.cms.Name = "cms";
            resources.ApplyResources(this.cms, "cms");
            this.cms.Opening += new System.ComponentModel.CancelEventHandler(this.cms_Opening);
            // 
            // tsmiSnapshot
            // 
            this.tsmiSnapshot.Name = "tsmiSnapshot";
            resources.ApplyResources(this.tsmiSnapshot, "tsmiSnapshot");
            this.tsmiSnapshot.Click += new System.EventHandler(this.tsmiSnapshot_Click);
            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            resources.ApplyResources(this.tsmiDelete, "tsmiDelete");
            this.tsmiDelete.Click += new System.EventHandler(this.tsmiDelete_Click);
            // 
            // tsmiEmpty
            // 
            this.tsmiEmpty.Name = "tsmiEmpty";
            resources.ApplyResources(this.tsmiEmpty, "tsmiEmpty");
            this.tsmiEmpty.Click += new System.EventHandler(this.tsmiEmpty_Click);
            // 
            // tsmiInterpolate
            // 
            this.tsmiInterpolate.Name = "tsmiInterpolate";
            resources.ApplyResources(this.tsmiInterpolate, "tsmiInterpolate");
            this.tsmiInterpolate.Click += new System.EventHandler(this.tsmiInterpolate_Click);
            // 
            // tsmiExport
            // 
            this.tsmiExport.Name = "tsmiExport";
            resources.ApplyResources(this.tsmiExport, "tsmiExport");
            this.tsmiExport.Click += new System.EventHandler(this.tsmiExport_Click);
            // 
            // tsmiExportSurfer
            // 
            this.tsmiExportSurfer.Name = "tsmiExportSurfer";
            resources.ApplyResources(this.tsmiExportSurfer, "tsmiExportSurfer");
            this.tsmiExportSurfer.Click += new System.EventHandler(this.tsmiExportSurfer_Click);
            // 
            // tsmiProperties
            // 
            this.tsmiProperties.Name = "tsmiProperties";
            resources.ApplyResources(this.tsmiProperties, "tsmiProperties");
            this.tsmiProperties.Click += new System.EventHandler(this.tsmiProperties_Click);
            // 
            // tsmiEdit
            // 
            this.tsmiEdit.Name = "tsmiEdit";
            resources.ApplyResources(this.tsmiEdit, "tsmiEdit");
            this.tsmiEdit.Click += new System.EventHandler(this.tsmiEdit_Click);
            // 
            // tsmiSep
            // 
            this.tsmiSep.Name = "tsmiSep";
            resources.ApplyResources(this.tsmiSep, "tsmiSep");
            // 
            // tsmiOpen
            // 
            this.tsmiOpen.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpenConfig,
            this.tsmiOpenBuoys,
            this.tsmiOpenSep,
            this.tsmiOpenBlankLineXml,
            this.tsmiOpenBlankLineSurfer,
            this.tsmiOpenSep2,
            this.tsmiOpenDA040,
            this.tsmiOpenPKT});
            this.tsmiOpen.Name = "tsmiOpen";
            resources.ApplyResources(this.tsmiOpen, "tsmiOpen");
            // 
            // tsmiOpenConfig
            // 
            this.tsmiOpenConfig.Name = "tsmiOpenConfig";
            resources.ApplyResources(this.tsmiOpenConfig, "tsmiOpenConfig");
            this.tsmiOpenConfig.Click += new System.EventHandler(this.tsmiOpenConfig_Click);
            // 
            // tsmiOpenBuoys
            // 
            this.tsmiOpenBuoys.Name = "tsmiOpenBuoys";
            resources.ApplyResources(this.tsmiOpenBuoys, "tsmiOpenBuoys");
            this.tsmiOpenBuoys.Click += new System.EventHandler(this.tsmiOpenBuoys_Click);
            // 
            // tsmiOpenSep
            // 
            this.tsmiOpenSep.Name = "tsmiOpenSep";
            resources.ApplyResources(this.tsmiOpenSep, "tsmiOpenSep");
            // 
            // tsmiOpenBlankLineXml
            // 
            this.tsmiOpenBlankLineXml.Name = "tsmiOpenBlankLineXml";
            resources.ApplyResources(this.tsmiOpenBlankLineXml, "tsmiOpenBlankLineXml");
            this.tsmiOpenBlankLineXml.Click += new System.EventHandler(this.tsmiOpenBlankLineXml_Click);
            // 
            // tsmiOpenBlankLineSurfer
            // 
            this.tsmiOpenBlankLineSurfer.Name = "tsmiOpenBlankLineSurfer";
            resources.ApplyResources(this.tsmiOpenBlankLineSurfer, "tsmiOpenBlankLineSurfer");
            this.tsmiOpenBlankLineSurfer.Click += new System.EventHandler(this.tsmiOpenBlankLineSurfer_Click);
            // 
            // tsmiOpenSep2
            // 
            this.tsmiOpenSep2.Name = "tsmiOpenSep2";
            resources.ApplyResources(this.tsmiOpenSep2, "tsmiOpenSep2");
            // 
            // tsmiOpenDA040
            // 
            this.tsmiOpenDA040.Name = "tsmiOpenDA040";
            resources.ApplyResources(this.tsmiOpenDA040, "tsmiOpenDA040");
            this.tsmiOpenDA040.Click += new System.EventHandler(this.tsmiOpenDA040_Click);
            // 
            // tsmiOpenPKT
            // 
            this.tsmiOpenPKT.Name = "tsmiOpenPKT";
            resources.ApplyResources(this.tsmiOpenPKT, "tsmiOpenPKT");
            this.tsmiOpenPKT.Click += new System.EventHandler(this.tsmiOpenPKT_Click);
            // 
            // tsmiSave
            // 
            this.tsmiSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSaveConfig,
            this.tsmiSaveBuoys,
            this.tsmiSaveSep,
            this.tsmiSaveBlankLineXml,
            this.tsmiSaveBlankLineXmlLALO,
            this.tsmiSaveBlankLineSurfer,
            this.tsmiSaveBlankLineSurferLALO});
            this.tsmiSave.Name = "tsmiSave";
            resources.ApplyResources(this.tsmiSave, "tsmiSave");
            // 
            // tsmiSaveConfig
            // 
            this.tsmiSaveConfig.Name = "tsmiSaveConfig";
            resources.ApplyResources(this.tsmiSaveConfig, "tsmiSaveConfig");
            this.tsmiSaveConfig.Click += new System.EventHandler(this.tsmiSaveConfig_Click);
            // 
            // tsmiSaveBuoys
            // 
            this.tsmiSaveBuoys.Name = "tsmiSaveBuoys";
            resources.ApplyResources(this.tsmiSaveBuoys, "tsmiSaveBuoys");
            this.tsmiSaveBuoys.Click += new System.EventHandler(this.tsmiSaveBuoys_Click);
            // 
            // tsmiSaveSep
            // 
            this.tsmiSaveSep.Name = "tsmiSaveSep";
            resources.ApplyResources(this.tsmiSaveSep, "tsmiSaveSep");
            // 
            // tsmiSaveBlankLineXml
            // 
            this.tsmiSaveBlankLineXml.Name = "tsmiSaveBlankLineXml";
            resources.ApplyResources(this.tsmiSaveBlankLineXml, "tsmiSaveBlankLineXml");
            this.tsmiSaveBlankLineXml.Click += new System.EventHandler(this.tsmiSaveBlankLineXml_Click);
            // 
            // tsmiSaveBlankLineXmlLALO
            // 
            this.tsmiSaveBlankLineXmlLALO.Name = "tsmiSaveBlankLineXmlLALO";
            resources.ApplyResources(this.tsmiSaveBlankLineXmlLALO, "tsmiSaveBlankLineXmlLALO");
            this.tsmiSaveBlankLineXmlLALO.Click += new System.EventHandler(this.blanklineXMLLALOToolStripMenuItem_Click);
            // 
            // tsmiSaveBlankLineSurfer
            // 
            this.tsmiSaveBlankLineSurfer.Name = "tsmiSaveBlankLineSurfer";
            resources.ApplyResources(this.tsmiSaveBlankLineSurfer, "tsmiSaveBlankLineSurfer");
            this.tsmiSaveBlankLineSurfer.Click += new System.EventHandler(this.tsmiSaveBlankLineSurfer_Click);
            // 
            // tsmiSaveBlankLineSurferLALO
            // 
            this.tsmiSaveBlankLineSurferLALO.Name = "tsmiSaveBlankLineSurferLALO";
            resources.ApplyResources(this.tsmiSaveBlankLineSurferLALO, "tsmiSaveBlankLineSurferLALO");
            this.tsmiSaveBlankLineSurferLALO.Click += new System.EventHandler(this.tsmiSaveBlankLineSurferLALO_Click);
            // 
            // tsmiOpenDirect
            // 
            this.tsmiOpenDirect.Name = "tsmiOpenDirect";
            resources.ApplyResources(this.tsmiOpenDirect, "tsmiOpenDirect");
            this.tsmiOpenDirect.Click += new System.EventHandler(this.tsmiOpenDirect_Click);
            // 
            // dlgSave
            // 
            this.dlgSave.DefaultExt = "*.txt";
            resources.ApplyResources(this.dlgSave, "dlgSave");
            // 
            // openDlg
            // 
            this.openDlg.DefaultExt = "xml";
            resources.ApplyResources(this.openDlg, "openDlg");
            // 
            // saveDlg
            // 
            this.saveDlg.DefaultExt = "xml";
            resources.ApplyResources(this.saveDlg, "saveDlg");
            // 
            // tsmiEnableAll
            // 
            this.tsmiEnableAll.Name = "tsmiEnableAll";
            resources.ApplyResources(this.tsmiEnableAll, "tsmiEnableAll");
            this.tsmiEnableAll.Click += new System.EventHandler(this.tsmiEnableAll_Click);
            // 
            // tsmiDisableAll
            // 
            this.tsmiDisableAll.Name = "tsmiDisableAll";
            resources.ApplyResources(this.tsmiDisableAll, "tsmiDisableAll");
            this.tsmiDisableAll.Click += new System.EventHandler(this.tsmiDisableAll_Click);
            // 
            // frmProject
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tvProject);
            this.DockType = DockDotNET.DockContainerType.ToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmProject";
            this.cms.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

        private System.Windows.Forms.ToolStripSeparator tsmiOpenSep2;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenDA040;
        private System.Windows.Forms.ToolStripMenuItem tsmiProperties;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenDirect;
        private System.Windows.Forms.ToolStripMenuItem tsmiSnapshot;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenPKT;
        private System.Windows.Forms.ToolStripMenuItem tsmiEdit;
        private System.Windows.Forms.ToolStripMenuItem tsmiExportSurfer;
        private System.Windows.Forms.ToolStripMenuItem tsmiEmpty;
        private System.Windows.Forms.ToolStripMenuItem tsmiEnableAll;
        private System.Windows.Forms.ToolStripMenuItem tsmiDisableAll;
    }
}