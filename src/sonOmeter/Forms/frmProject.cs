using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using sonOmeter.Classes;
using UKLib.Controls;
using sonOmeter.Classes.Sonar2D;
using UKLib.Survey.Parse;
using UKLib.DXF;
using UKLib.Debug;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for Form2.
    /// </summary>
    public partial class frmProject : DockDotNET.DockWindow
    {
        #region TreeNodeType Enum
        public enum TreeNodeType
        {
            Void,
            nDevice,
            nRecords,
            nRecord,
            nRecord3D,
            nProfile,
            nDXFFile,
            nProject,
            nPositions,
            nTracks,
            n2D,
            nBlankLine,
            n3D,
            nDXF,
            nBlanklines,
            nBuoylist,
            nPoints
        }
        #endregion

        #region Create and Dispose
        public frmProject()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            tsmiOpenDA040.Visible = GSC.Settings.Lic[Module.Modules.V22];
            tsmiOpenSep2.Visible = GSC.Settings.Lic[Module.Modules.V22];

            nodeProj = tvProject.Nodes.Add("Project");
            nodeRecords = nodeProj.Nodes.Add("Records");
            nodeProfiles = nodeProj.Nodes.Add("Profiles");
            node3DRecords = nodeProj.Nodes.Add("3D Records");
            nodeDXF = nodeProj.Nodes.Add("DXF Files");
            nodeBlanklines = nodeProj.Nodes.Add("Blanklines");
            nodeTracks = nodeProj.Nodes.Add("Tracks");
            node2D = nodeTracks.Nodes.Add("2D");
            node3D = nodeTracks.Nodes.Add("3D");
            nodeTracks.Nodes.Add("Buoylist");

            tvProject.ExpandAll();

            node3DRecords.ForeColor = node3D.ForeColor = GSC.Settings.Lic[Module.Modules.ThreeD] ? this.ForeColor : disableColor;
            nodeDXF.ForeColor = GSC.Settings.Lic[Module.Modules.DXF] ? this.ForeColor : disableColor;

            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), GlobalNotifier.MsgTypes.DeviceChanged);
            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), GlobalNotifier.MsgTypes.NewDeviceList);
            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), GlobalNotifier.MsgTypes.NewBlankLine);
            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), GlobalNotifier.MsgTypes.ToggleDXFFile);
            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), GlobalNotifier.MsgTypes.UpdateRecord);
            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), GlobalNotifier.MsgTypes.NewRecord);
            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), GlobalNotifier.MsgTypes.Close);
        }
        #endregion

        #region Variables and properties
        private int selRec = 0;
        private int selDev = 0;
        private Type selRecType = null;

        private TreeNode nodeProj = null;
        private TreeNode nodeTracks = null;
        private TreeNode nodeRecords = null;
        private TreeNode nodeProfiles = null;
        private TreeNode node3DRecords = null;
        private TreeNode node2D = null;
        private TreeNode node3D = null;
        private TreeNode nodeDXF = null;
        private TreeNode nodeBlanklines = null;

        private Color disableColor = Color.Gray;

        private SonarProject project = null;
        public SonarProject Project
        {
            set
            {
                project = value;
                nodeProj.Tag = value;
            }
        }
        #endregion

        #region Events
        public event ProjectEventHandler ShowSonar;
        public event ProjectEventHandler ShowManualPoints;
        public event ProjectEventHandler ShowPositions;
        public event ProjectEventHandler Show2D;
        public event ProjectEventHandler Show3D;
        #endregion

        #region Functions
        private string GetRecordText(int i, SonarRecord record)
        {
            // No zero-based record count...
            i++;

            if (record.Description == "") record.Description = "Record " + i;
            return "[" + i + "] - " + record.ToString();
        }

        private TreeNodeType GetNodeType(TreeNode node)
        {
            if (node == null)
                return TreeNodeType.Void;

            if (node.Text.StartsWith("Position"))
                return TreeNodeType.nPositions;

            if (node.Text.StartsWith("Records"))
                return TreeNodeType.nRecords;

            if (node.Text.StartsWith("2D"))
                return TreeNodeType.n2D;

            if (node.Text.StartsWith("3D") && !(node.Tag is Sonar3DRecord))
                return TreeNodeType.n3D;

            if (node.Text.StartsWith("DXF"))
                return TreeNodeType.nDXF;

            if (node.Text.StartsWith("Blanklines"))
                return TreeNodeType.nBlanklines;

            if (node.Text.StartsWith("Tracks"))
                return TreeNodeType.nTracks;

            if (node.Text.StartsWith("Sonar "))
                return TreeNodeType.nDevice;

            if (node.Text.StartsWith("Project"))
                return TreeNodeType.nProject;

            if (node.Text.StartsWith("Buoylist"))
                return TreeNodeType.nBuoylist;

            if (node.Text.StartsWith("Points"))
                return TreeNodeType.nPoints;

            try
            {
                if (node.Tag is DXFFile)
                    return TreeNodeType.nDXFFile;
                if (node.Tag is BlankLine)
                    return TreeNodeType.nBlankLine;
                if (node.Tag is Sonar3DRecord)
                    return TreeNodeType.nRecord3D;
                else if (node.Tag is SonarProfile)
                    return TreeNodeType.nProfile;
                else if (node.Tag is SonarRecord)
                    return TreeNodeType.nRecord;
                else
                    return TreeNodeType.Void;
            }
            catch
            {
                return TreeNodeType.Void;
            }

        }
        #endregion

        #region Event handler
        public void AddRecord(int i, SonarRecord record)
        {
            TreeNode node = new TreeNode(GetRecordText(i, record));
            node.Tag = record;
            node.Checked = true;
            record.NodeRef = node;

            if (record is Sonar3DRecord)
            {
                if (node3DRecords != null)
                {
                    Sonar3DRecord rec3D = record as Sonar3DRecord;
                    if (rec3D.IsInterpolating || rec3D.IsMeshing)
                    {
                        node.ForeColor = disableColor;
                    }
                    rec3D.MeshBuilt += new EventHandler(Rec3DReady);
                    rec3D.StartingInterpolation += new CancelEventHandler(Rec3DStartingInterpolation);
                    node3DRecords.Nodes.Add(node);
                }
            }
            else if (record is SonarProfile)
            {
                record.BuildDeviceTree(node);
                if (nodeProfiles != null)
                    nodeProfiles.Nodes.Add(node);
                foreach (TreeNode cnode in node.Nodes)
                {
                    if (!cnode.Text.StartsWith("Position"))
                    {
                        //cnode.Checked = true;
                        tvProject.SetNodeType(cnode, true);
                    }
                }
            }
            else if (record is SonarRecord)
            {
                record.BuildDeviceTree(node);
                if (nodeRecords != null)
                    nodeRecords.Nodes.Add(node);
                foreach (TreeNode cnode in node.Nodes)
                {
                    if (!cnode.Text.StartsWith("Position"))
                    {
                        //cnode.Checked = true;
                        tvProject.SetNodeType(cnode, true);
                    }
                }
            }

            tvProject.SetNodeType(node, true);
            node.Checked = true;
            tvProject.UpdateTree();

            tvProject.ExpandAll();
        }

        void Rec3DStartingInterpolation(object sender, CancelEventArgs e)
        {
            // Disable record entry by changing its color.
            if (node3DRecords != null)
            {
                foreach (TreeNode node in node3DRecords.Nodes)
                {
                    if (node.Tag == sender)
                    {
                        node.ForeColor = disableColor;
                        break;
                    }
                }
            }
        }

        void Rec3DReady(object sender, EventArgs e)
        {
            // Enable record entry by setting its color to default.
            if (node3DRecords != null)
            {
                foreach (TreeNode node in node3DRecords.Nodes)
                {
                    if (node.Tag == sender)
                    {
                        node.ForeColor = this.ForeColor;
                        break;
                    }
                }
            }
        }

        protected void UpdateRecord(int i, SonarRecord record)
        {
            if (nodeRecords == null)
                return;

            foreach (TreeNode node in nodeRecords.Nodes)
            {
                if (node.Tag == record)
                {
                    node.Text = GetRecordText(i, record);
                    record.BuildDeviceTree(node);
                    foreach (TreeNode cnode in node.Nodes)
                    {
                        if (!cnode.Text.StartsWith("Position"))
                        {
                            //cnode.Checked = true;
                            tvProject.SetNodeType(cnode, true);
                        }
                    }
                    tvProject.UpdateTree();
                }
            }
        }
        #endregion

        #region Tree view
        private void tvProject_NodeDoubleClick(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            TreeNodeType type = GetNodeType(e.Node);

            switch (type)
            {
                case TreeNodeType.nDevice:
                    int id = 0;
                    SonarRecord rec = (SonarRecord)e.Node.Parent.Tag;
                    if (!rec.IsProfile)
                        id = rec.Devices.IndexOf(e.Node.Tag as SonarDevice);
                    if (ShowSonar != null)
                        ShowSonar(this, new ProjectEventArgs(id, rec, ProjectEventType.ShowSonar));
                    break;
                case TreeNodeType.nPoints:
                    if (ShowManualPoints != null)
                        ShowManualPoints(this, new ProjectEventArgs(-1, (SonarRecord)e.Node.Parent.Tag, ProjectEventType.ShowPositions));
                    break;
                case TreeNodeType.nPositions:
                    if (ShowPositions != null)
                        ShowPositions(this, new ProjectEventArgs(-1, (SonarRecord)e.Node.Parent.Tag, ProjectEventType.ShowPositions));
                    break;
                case TreeNodeType.nDXF:
                case TreeNodeType.nBlankLine:
                case TreeNodeType.n2D:
                    if (Show2D != null)
                        Show2D(this, new ProjectEventArgs(-1, null, ProjectEventType.Show2D));
                    break;
                case TreeNodeType.n3D:
                case TreeNodeType.nRecord3D:
                    if (GSC.Settings.Lic[Module.Modules.ThreeD] && (Show3D != null) && (e.Node.ForeColor != disableColor))
                        Show3D(this, new ProjectEventArgs(-1, null, ProjectEventType.Show3D));
                    break;
                case TreeNodeType.nBuoylist:
                    frmBuoyEditor frm = new frmBuoyEditor();
                    frm.BuoyList = project.BuoyList;
                    frm.BuoyConnectionsList = project.BuoyConnectionList;
                    frm.ShowDialog();
                    GSC.Settings.Changed = true;
                    GlobalNotifier.Invoke(this, project.BuoyList, GlobalNotifier.MsgTypes.BuoyListChanged);
                    break;
            }
        }

        private void tvProject_NodeStateChanged(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            bool updateProfiles = false;

            if (e.Node.Tag is SonarDevice)
            {
                SonarDevice device = e.Node.Tag as SonarDevice;
                if (device.ShowInTrace != e.Node.Checked)
                {
                    device.ShowInTrace = e.Node.Checked;
                    GlobalNotifier.Invoke(this, device, GlobalNotifier.MsgTypes.ToggleDevice);
                    updateProfiles = !(e.Node.Parent.Tag is SonarProfile);
                }
            }
            else if ((e.Node.Tag is Sonar3DRecord) && (e.Node.ForeColor != disableColor))
            {
                Sonar3DRecord rec3D = e.Node.Tag as Sonar3DRecord;
                if (rec3D.ShowInTrace != e.Node.Checked)
                {
                    rec3D.ShowInTrace = e.Node.Checked;
                    GlobalNotifier.Invoke(this, rec3D, GlobalNotifier.MsgTypes.Toggle3DRecord);
                    updateProfiles = true;
                }
            }
            else if (e.Node.Tag is DXFFile)
            {
                DXFFile dxfFile = e.Node.Tag as DXFFile;
                if (dxfFile.ShowInTrace != e.Node.Checked)
                {
                    dxfFile.ShowInTrace = e.Node.Checked;
                    GlobalNotifier.Invoke(this, dxfFile, GlobalNotifier.MsgTypes.ToggleDXFFile);
                    updateProfiles = true;
                }
            }
            else if (e.Node.Tag is BlankLine)
            {
                BlankLine blankLine = e.Node.Tag as BlankLine;
                if (blankLine.ShowInTrace != e.Node.Checked)
                {
                    blankLine.ShowInTrace = e.Node.Checked;
                    GlobalNotifier.Invoke(this, blankLine, GlobalNotifier.MsgTypes.ToggleBlankLine);
                    updateProfiles = true;
                }
            }
            else if (e.Node.Text.StartsWith("Points"))
            {
                SonarRecord rec = e.Node.Tag as SonarRecord;
                if (rec.ShowManualPoints != e.Node.Checked)
                {
                    rec.ShowManualPoints = e.Node.Checked;
                    GlobalNotifier.Invoke(this, rec, GlobalNotifier.MsgTypes.TogglePoints);
                    updateProfiles = !(e.Node.Parent.Tag is SonarProfile);
                }
            }

            if (updateProfiles)
                GlobalNotifier.Invoke(this, project, GlobalNotifier.MsgTypes.UpdateProfiles);
        }

        private void tvProject_NodeClick(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
        }

        public void DeSelect()
        {
            try
            {
                TreeNode node = nodeRecords;
                if (selRecType == typeof(SonarProfile))
                    node = nodeProfiles;

                if (node.Nodes.Count <= selRec)
                    return;

                node.Nodes[selRec].NodeFont = null;
                node.Nodes[selRec].Text = node.Nodes[selRec].Text.TrimEnd();
                node.Nodes[selRec].Nodes[selDev].NodeFont = null;
                node.Nodes[selRec].Nodes[selDev].Text = node.Nodes[selRec].Nodes[selDev].Text.TrimEnd();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "frmProject.DeSelect: " + e.Message);
            }
        }

        /// <summary>
        /// Selects the record, profile or 3D record and its corresponding device in the tree view.
        /// </summary>
        /// <param name="rec">The record, profile or 3D record ID.</param>
        /// <param name="dev">The device ID.</param>
        /// <param name="recType">The type of rec.</param>
        public void Select(int rec, int dev, Type recType)
        {
            try
            {
                Font font = tvProject.Font;

                DeSelect();

                if ((rec < 0) || (dev < 0))
                    return;

                selRec = rec;
                selDev = dev;
                selRecType = recType;

                TreeNode node = nodeRecords;
                if (selRecType == typeof(SonarProfile))
                    node = nodeProfiles;

                node.Nodes[selRec].NodeFont = new Font(font, FontStyle.Bold);
                node.Nodes[selRec].Text = node.Nodes[selRec].Text + "          ";
                node.Nodes[selRec].Nodes[selDev].NodeFont = new Font(font, FontStyle.Bold);
                node.Nodes[selRec].Nodes[selDev].Text = node.Nodes[selRec].Nodes[selDev].Text + "    ";
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "frmProject.Select: " + e.Message);
            }
        }

        private void tvProject_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2)
            {
                frmInputBox input = new frmInputBox("", "Input");
                TreeNodeType type = GetNodeType(tvProject.SelectedNode);

                switch (type)
                {
                    case TreeNodeType.nProject:
                        input.InputText = project.Name;
                        if (input.ShowDialog() == DialogResult.OK)
                        {
                            project.Name = input.InputText;
                            Invalidate();
                        }
                        break;
                    case TreeNodeType.nRecord:
                        SonarRecord record = tvProject.SelectedNode.Tag as SonarRecord;
                        input.InputText = record.Description;
                        if (input.ShowDialog() == DialogResult.OK)
                        {
                            record.Description = input.InputText;
                            tvProject.SelectedNode.Text = GetRecordText(project.IndexOf(record), record);
                            GlobalNotifier.Invoke(this, record, GlobalNotifier.MsgTypes.SwitchProperties);
                            Invalidate();
                        }
                        break;
                }
            }
        }
        #endregion

        #region Global events
        private void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            SonarRecord rec;

            switch (state)
            {
                case GlobalNotifier.MsgTypes.NewRecord:
                    rec = args as SonarRecord;
                    AddRecord(project.IndexOf(rec), rec);
                    break;

                case GlobalNotifier.MsgTypes.UpdateRecord:
                    rec = args as SonarRecord;
                    UpdateRecord(project.IndexOf(rec), rec);
                    break;

                case GlobalNotifier.MsgTypes.Close:
                    nodeRecords.Collapse();
                    nodeRecords.Nodes.Clear();
                    nodeProfiles.Collapse();
                    nodeProfiles.Nodes.Clear();
                    node3DRecords.Collapse();
                    node3DRecords.Nodes.Clear();
                    nodeDXF.Collapse();
                    nodeDXF.Nodes.Clear();
                    nodeBlanklines.Collapse();
                    nodeBlanklines.Nodes.Clear();
                    tvProject.UpdateTree();
                    break;

                case GlobalNotifier.MsgTypes.DeviceChanged:
                    OnDeviceChanged(sender, args);
                    break;

                case GlobalNotifier.MsgTypes.NewDeviceList:
                    rec = sender as SonarRecord;
                    UpdateRecord(project.IndexOf(rec), rec);
                    break;

                case GlobalNotifier.MsgTypes.NewBlankLine:
                    BlankLine blankLine = args as BlankLine;

                    if ((blankLine != null) && (nodeBlanklines != null))
                    {
                        if (string.IsNullOrEmpty(blankLine.Name))
                            blankLine.Name = "Blankline " + project.BlankLines.IndexOf(blankLine);

                        TreeNode node = new TreeNode(blankLine.Name);
                        blankLine.NodeRef = node;
                        node.Tag = blankLine;
                        node.Checked = blankLine.ShowInTrace;

                        nodeBlanklines.Nodes.Add(node);
                        tvProject.SetNodeType(node, true);
                        tvProject.UpdateTree();
                    }
                    break;

                case GlobalNotifier.MsgTypes.ToggleDXFFile:
                    DXFFile dxfFile = args as DXFFile;

                    if ((sender != this) && (dxfFile != null) && (nodeDXF != null))
                    {
                        bool match = false;

                        foreach (TreeNode node in nodeDXF.Nodes)
                            if (node.Tag == args)
                            {
                                match = true;
                                break;
                            }

                        if (!match)
                        {
                            TreeNode node = new TreeNode(dxfFile.FileName);
                            node.Tag = dxfFile;
                            node.Checked = true;

                            nodeDXF.Nodes.Add(node);

                            tvProject.SetNodeType(node, true);
                            node.Checked = true;
                            tvProject.UpdateTree();

                            tvProject.ExpandAll();
                        }
                    }
                    break;
            }
        }

        private void OnDeviceChanged(object sender, object args)
        {
            try
            {
                RecordEventArgs e = args as RecordEventArgs;
                int rec = e.Rec != null ? project.IndexOf(e.Rec) : -1;
                int dev = e.Rec != null ? e.Rec.IndexOf(e.Dev) : -1;

                Select(rec, dev, e.Tag as Type);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "frmProject.OnDeviceChanged: " + e.Message);
            }
        }
        #endregion

        #region Popup menu
        #region Opening event
        private void cms_Opening(object sender, CancelEventArgs e)
        {
            TreeNodeType type = GetNodeType(tvProject.SelectedNode);

            // Standard values.
            tsmiDisableAll.Visible = false;
            tsmiEnableAll.Visible = false;

            tsmiOpenBlankLineSurfer.Visible = false;
            tsmiOpenBlankLineXml.Visible = false;
            tsmiSaveBlankLineSurfer.Visible = false;
            tsmiSaveBlankLineSurferLALO.Visible = false;
            tsmiSaveBlankLineXml.Visible = false;
            tsmiSaveBlankLineXmlLALO.Visible = false;

            tsmiOpenBuoys.Visible = true;
            tsmiOpenConfig.Visible = true;
            tsmiOpenSep2.Visible = GSC.Settings.Lic[Module.Modules.V22];
            tsmiOpenDA040.Visible = GSC.Settings.Lic[Module.Modules.V22];
            tsmiOpenDirect.Visible = false;
            tsmiOpenPKT.Visible = false;
            tsmiOpenSep.Visible = true;
            tsmiOpenSep2.Visible = false;
            tsmiSaveSep.Visible = false;
            tsmiInterpolate.Text = "Interpolate";
            tsmiEdit.Visible = false;
            tsmiExportSurfer.Visible = false;
            tsmiEmpty.Visible = false;

            // Values depending on the selected item.
            switch (type)
            {
                case TreeNodeType.nRecords:
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = false;
                    tsmiInterpolate.Visible = false;
                    tsmiExport.Visible = false;
                    tsmiProperties.Visible = false;
                    tsmiSep.Visible = false;
                    tsmiOpen.Visible = false;
                    tsmiSave.Visible = false;
                    tsmiEmpty.Visible = false;

                    tsmiDisableAll.Visible = true;
                    tsmiEnableAll.Visible = true;
                    break;

                case TreeNodeType.nDevice:
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = !(tvProject.SelectedNode.Parent.Tag as SonarRecord).IsProfile;
                    tsmiInterpolate.Visible = false;
                    tsmiExport.Visible = true;
                    tsmiProperties.Visible = true;
                    tsmiSep.Visible = false;
                    tsmiOpen.Visible = false;
                    tsmiSave.Visible = false;
                    tsmiEmpty.Visible = true;
                    break;
                case TreeNodeType.nRecord:
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = true;
                    tsmiInterpolate.Visible = true;
                    tsmiExport.Visible = true;
                    tsmiProperties.Visible = true;
                    tsmiSep.Visible = false;
                    tsmiOpen.Visible = true;
                    tsmiOpenBuoys.Visible = false;
                    tsmiOpenConfig.Visible = false;
                    tsmiOpenDA040.Visible = false;
                    tsmiOpenDirect.Visible = false;
                    tsmiOpenPKT.Visible = GSC.Settings.Lic[Module.Modules.RadioDetection];
                    tsmiOpenSep.Visible = false;
                    tsmiOpenSep2.Visible = false;
                    tsmiSave.Visible = false;
                    break;
                case TreeNodeType.nProfile:
                    tsmiSnapshot.Visible = true;
                    tsmiDelete.Visible = true;
                    tsmiInterpolate.Visible = true;
                    tsmiInterpolate.Text = "Rebuild";
                    tsmiExport.Visible = true;
                    tsmiProperties.Visible = true;
                    tsmiSep.Visible = false;
                    tsmiOpen.Visible = false;
                    tsmiSave.Visible = false;
                    break;
                case TreeNodeType.nRecord3D:
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = true;
                    tsmiInterpolate.Visible = false;
                    tsmiExport.Visible = true;
                    tsmiExportSurfer.Visible = true;
                    tsmiProperties.Visible = true;
                    tsmiSep.Visible = false;
                    tsmiOpen.Visible = false;
                    tsmiSave.Visible = false;

                    e.Cancel = (tvProject.SelectedNode.ForeColor == disableColor);
                    break;
                case TreeNodeType.nProject:
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = false;
                    tsmiInterpolate.Visible = true;
                    tsmiExport.Visible = true;
                    tsmiProperties.Visible = true;
                    tsmiSep.Visible = true;
                    tsmiOpen.Visible = true;
                    tsmiSave.Visible = true;
                    tsmiOpenPKT.Visible = GSC.Settings.Lic[Module.Modules.RadioDetection];
                    break;
                case TreeNodeType.nDXF:
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = false;
                    tsmiInterpolate.Visible = false;
                    tsmiExport.Visible = false;
                    tsmiProperties.Visible = false;
                    tsmiSep.Visible = false;
                    tsmiOpen.Visible = false;
                    tsmiSave.Visible = false;
                    tsmiOpenDirect.Visible = GSC.Settings.Lic[Module.Modules.DXF];
                    break;
                case TreeNodeType.nDXFFile:
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = true;
                    tsmiInterpolate.Visible = false;
                    tsmiExport.Visible = false;
                    tsmiProperties.Visible = true;
                    tsmiSep.Visible = false;
                    tsmiOpen.Visible = false;
                    tsmiSave.Visible = false;
                    break;
                case TreeNodeType.nBlankLine:
                    tsmiOpenBuoys.Visible = false;
                    tsmiOpenConfig.Visible = false;
                    tsmiSaveBuoys.Visible = false;
                    tsmiSaveConfig.Visible = false;
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = true;
                    tsmiInterpolate.Visible = false;
                    tsmiExport.Visible = false;
                    tsmiProperties.Visible = true;
                    tsmiOpenSep.Visible = false;
                    tsmiSep.Visible = true;
                    tsmiOpen.Visible = true;
                    tsmiSave.Visible = true;
                    tsmiEdit.Visible = true;

                    tsmiOpenBlankLineSurfer.Visible = true;
                    tsmiOpenBlankLineXml.Visible = true;
                    tsmiSaveBlankLineSurfer.Visible = true;
                    tsmiSaveBlankLineSurferLALO.Visible = true;
                    tsmiSaveBlankLineXml.Visible = true;
                    tsmiSaveBlankLineXmlLALO.Visible = true;
                    break;
                case TreeNodeType.nBlanklines:
                    tsmiOpenBuoys.Visible = false;
                    tsmiOpenConfig.Visible = false;
                    tsmiSaveBuoys.Visible = false;
                    tsmiSaveConfig.Visible = false;
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = false;
                    tsmiInterpolate.Visible = false;
                    tsmiExport.Visible = false;
                    tsmiProperties.Visible = false;
                    tsmiOpenSep.Visible = false;
                    tsmiSep.Visible = false;
                    tsmiOpen.Visible = true;
                    tsmiSave.Visible = false;

                    tsmiOpenBlankLineSurfer.Visible = true;
                    tsmiOpenBlankLineXml.Visible = true;
                    tsmiOpenDA040.Visible = false;
                    break;
                case TreeNodeType.n2D:
                    tsmiOpenBuoys.Visible = false;
                    tsmiOpenConfig.Visible = false;
                    tsmiSaveBuoys.Visible = false;
                    tsmiSaveConfig.Visible = false;
                    tsmiSnapshot.Visible = false;
                    tsmiDelete.Visible = false;
                    tsmiInterpolate.Visible = false;
                    tsmiExport.Visible = false;
                    tsmiProperties.Visible = false;
                    tsmiOpenSep.Visible = false;
                    tsmiSep.Visible = false;
                    tsmiOpen.Visible = true;
                    tsmiSave.Visible = false;

                    tsmiOpenBlankLineSurfer.Visible = false;
                    tsmiOpenBlankLineXml.Visible = false;
                    tsmiOpenDA040.Visible = true;
                    break;
                default:
                    e.Cancel = true;
                    break;
            }
        }
        #endregion

        #region Misc
        private void tsmiProperties_Click(object sender, EventArgs e)
        {
            TreeNodeType type = GetNodeType(tvProject.SelectedNode);

            switch (type)
            {
                case TreeNodeType.nDevice:
                case TreeNodeType.nRecord:
                case TreeNodeType.nProfile:
                case TreeNodeType.nRecord3D:
                case TreeNodeType.nBlankLine:
                    GlobalNotifier.Invoke(this, tvProject.SelectedNode.Tag, GlobalNotifier.MsgTypes.SwitchProperties);
                    break;
                case TreeNodeType.nDXFFile:
                    DXFFile dxf = tvProject.SelectedNode.Tag as DXFFile;
                    DXFLayerForm form = new DXFLayerForm(dxf.DXF);
                    form.ShowDialog();
                    dxf.DXF.OnUpdate();
                    GlobalNotifier.Invoke(this, tvProject.SelectedNode.Tag, GlobalNotifier.MsgTypes.ToggleDXFFile);
                    break;
                default:
                    GlobalNotifier.Invoke(this, GSC.Settings, GlobalNotifier.MsgTypes.SwitchProperties);
                    break;
            }
        }

        private void tsmiSnapshot_Click(object sender, EventArgs e)
        {
            SonarProfile profile = tvProject.SelectedNode.Tag as SonarProfile;

            project.AddRecord(profile.CreateSnapshot());
        }

        private void tsmiEdit_Click(object sender, EventArgs e)
        {
            BlankLine blankLine = tvProject.SelectedNode.Tag as BlankLine;

            if (blankLine != null)
            {
                project.EditBlankLine = blankLine;
                GlobalNotifier.Invoke(this, blankLine, GlobalNotifier.MsgTypes.EditBlankLine);
            }
        }
        #endregion

        #region Delete, Interpolate & Export
        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            SonarDevice device = null;
            SonarRecord record = null;
            BlankLine blankLine = null;

            switch (GetNodeType(tvProject.SelectedNode))
            {
                case TreeNodeType.nRecord:
                case TreeNodeType.nProfile:
                case TreeNodeType.nRecord3D:
                    record = tvProject.SelectedNode.Tag as SonarRecord;
                    project.RemoveRecord(record);
                    tvProject.SelectedNode.Remove();
                    record.Dispose();
                    record = null;
                    break;

                case TreeNodeType.nDevice:
                    device = tvProject.SelectedNode.Tag as SonarDevice;
                    record = tvProject.SelectedNode.Parent.Tag as SonarRecord;

                    if (device.SonID != 0)
                    {
                        record.DeleteDevice(device.SonID);
                        tvProject.Nodes.Remove(tvProject.SelectedNode);
                    }
                    break;

                case TreeNodeType.nBlankLine:
                    blankLine = tvProject.SelectedNode.Tag as BlankLine;

                    if (blankLine != null)
                    {
                        project.BlankLines.Remove(blankLine);
                        tvProject.Nodes.Remove(tvProject.SelectedNode);
                    }
                    break;

                case TreeNodeType.nDXFFile:
                    DXFFile dxfFile = tvProject.SelectedNode.Tag as DXFFile;

                    if (dxfFile != null)
                    {
                        project.DXFFiles.Remove(dxfFile);
                        tvProject.Nodes.Remove(tvProject.SelectedNode);
                    }

                    GlobalNotifier.Invoke(this, dxfFile, GlobalNotifier.MsgTypes.ToggleDXFFile);

                    dxfFile.Dispose();
                    break;

                default:
                    return;
            }

            tvProject.UpdateTree();
            GlobalNotifier.Invoke(this, project, GlobalNotifier.MsgTypes.UpdateProfiles);
        }

        private void tsmiExport_Click(object sender, EventArgs e)
        {
            TreeNodeType type = GetNodeType(tvProject.SelectedNode);

            // Follow normal export steps...
            StringCollection st = new StringCollection();
            var form = new frmExport();
            DialogResult tf = form.ShowDialog();
            if (tf == DialogResult.OK)
            {
                SonarExport exp = form.exp;
                form.Dispose();
                SonarRecord record;
                SonarDevice device;
                String recname = "";
                string sRecordLine;

                switch (type)
                {
                    case TreeNodeType.nDevice:
                        device = tvProject.SelectedNode.Tag as SonarDevice;

                        record = tvProject.SelectedNode.Parent.Tag as SonarRecord;
                        sRecordLine = exp.ExportBeginRecord(record);
                        if (sRecordLine != "") st.Add(sRecordLine);
                        record.Export(st, exp, device.SonID, project.IndexOf(record));
                        sRecordLine = exp.ExportEndRecord(record);
                        if (sRecordLine != "") st.Add(sRecordLine);
                        if (record.Description == "")
                        {
                            recname = tvProject.SelectedNode.Parent.Text;
                            recname = recname.Substring(1, recname.IndexOf("- (") - 1);
                        }
                        else
                            recname = record.Description;
                        recname = "." + recname + ".Sonar" + device.SonID.ToString();
                        break;
                    case TreeNodeType.nRecord:
                    case TreeNodeType.nProfile:
                    case TreeNodeType.nRecord3D:
                        record = tvProject.SelectedNode.Tag as SonarRecord;
                        sRecordLine = exp.ExportBeginRecord(record);
                        if (sRecordLine != "") st.Add(sRecordLine);
                        record.Export(st, exp, project.IndexOf(record));
                        sRecordLine = exp.ExportEndRecord(record);
                        if (sRecordLine != "") st.Add(sRecordLine);
                        project.AppendBuoysToExport(st, exp);

                        if (record.Description == "")
                        {
                            recname = tvProject.SelectedNode.Parent.Text;
                            recname = recname.Substring(1, recname.IndexOf("- (") - 1);
                        }
                        else
                            recname = record.Description;

                        break;
                    case TreeNodeType.nProject:
                        for (int i = 0; i < project.RecordCount; i++)
                        {
                            if (project.Record(i).ShowInTrace)
                            {
                                sRecordLine = exp.ExportBeginRecord(project.Record(i));
                                if (sRecordLine != "") st.Add(sRecordLine);
                                project.Record(i).Export(st, exp, i);
                                sRecordLine = exp.ExportEndRecord(project.Record(i));
                                if (sRecordLine != "") st.Add(sRecordLine);
                            }
                        }
                        project.AppendBuoysToExport(st, exp);
                        break;
                }

                if (st.Count > 0)
                {
                    dlgSave.FileName = Path.ChangeExtension(project.FileName, recname + ".txt");
                    if (dlgSave.ShowDialog() == DialogResult.OK)
                    {
                        StreamWriter sw = new StreamWriter(dlgSave.FileName);
                        foreach (string s in st)
                            sw.WriteLine(s);
                        sw.Close();
                        if (exp.ExportLog != "")
                        {
                            dlgSave.FileName = Path.ChangeExtension(dlgSave.FileName, ".log");
                            sw = new StreamWriter(dlgSave.FileName);
                            sw.WriteLine(exp.ExportLog);
                            sw.Close();
                        }
                    }
                }
            }
        }

        private void tsmiExportSurfer_Click(object sender, EventArgs e)
        {
            TreeNodeType type = GetNodeType(tvProject.SelectedNode);

            // 3D records have an extra export window...
            if (type == TreeNodeType.nRecord3D)
            {
                Sonar3DRecord rec3D = tvProject.SelectedNode.Tag as Sonar3DRecord;

                if (rec3D != null)
                {
                    frmExport3D form3D = new frmExport3D() { Rec3D = rec3D };
                    form3D.ShowDialog();
                }
            }
        }

        private void tsmiInterpolate_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNodeType type = GetNodeType(tvProject.SelectedNode);

                switch (type)
                {
                    case TreeNodeType.nRecord:
                        SonarRecord record = tvProject.SelectedNode.Tag as SonarRecord;
                        record.UpdateAllCoordinates();
                        GlobalNotifier.Invoke(this, record, GlobalNotifier.MsgTypes.Interpolate);
                        break;
                    case TreeNodeType.nProfile:
                        SonarProfile profile = tvProject.SelectedNode.Tag as SonarProfile;
                        profile.CreateProfile(project);
                        GlobalNotifier.Invoke(this, profile, GlobalNotifier.MsgTypes.Interpolate);
                        break;
                    case TreeNodeType.nProject:
                        for (int i = 0; i < project.RecordCount; i++)
                            project.Record(i).UpdateAllCoordinates();
                        GlobalNotifier.Invoke(this, project, GlobalNotifier.MsgTypes.Interpolate);
                        break;
                }
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "frmProject.mnuInterpolate_Click: " + ex.Message);
            }
        }
        #endregion

        #region Open
        private void tsmiOpenConfig_Click(object sender, EventArgs e)
        {
            openDlg.Filter = "Config files (*.cfgx)|*.cfgx|All files (*.*)|*.*";
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                GSC.ReadXmlFile(openDlg.FileName);
            }
        }

        private void tsmiOpenBuoys_Click(object sender, EventArgs e)
        {
            if (openDlg.ShowDialog() == DialogResult.OK)
                Buoy.ReadXml(openDlg.FileName, project.BuoyList, project.BuoyConnectionList);
        }

        private void tsmiOpenBlankLineXml_Click(object sender, EventArgs e)
        {
            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                BlankLine blankline = new BlankLine();
                blankline.ReadFromFileXml(openDlg.FileName);
                project.BlankLines.Add(blankline);
                GlobalNotifier.Invoke(this, blankline, GlobalNotifier.MsgTypes.NewBlankLine);
            }
        }

        private void tsmiOpenBlankLineSurfer_Click(object sender, EventArgs e)
        {
            string filter = openDlg.Filter;
            openDlg.Filter = "BLN files (*.bln)|*.bln|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                BlankLine blankline = new BlankLine();
                blankline.ReadFromFileSurfer(openDlg.FileName);
                project.BlankLines.Add(blankline);
                GlobalNotifier.Invoke(this, blankline, GlobalNotifier.MsgTypes.NewBlankLine);
            }

            openDlg.Filter = filter;
        }

        private void tsmiOpenDA040_Click(object sender, EventArgs e)
        {
            string filter = openDlg.Filter;
            openDlg.Filter = "DA040 files (*.d40)|*.d40|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                Buoy bTemplate = new Buoy();
                BuoyConnection cTemplate = new BuoyConnection();

                if (frmPropertyViewer.ShowDialog(bTemplate, "Set buoy template...") != DialogResult.OK)
                    return;

                if (frmPropertyViewer.ShowDialog(cTemplate, "Set connection template...") != DialogResult.OK)
                    return;

                Buoy.ReadDA040(openDlg.FileName, project.BuoyList, project.BuoyConnectionList, bTemplate, cTemplate);
            }

            openDlg.Filter = filter;
        }

        private void tsmiOpenDirect_Click(object sender, EventArgs e)
        {
            TreeNodeType type = GetNodeType(tvProject.SelectedNode);

            switch (type)
            {
                case TreeNodeType.nDXF:
                    string filter = openDlg.Filter;
                    openDlg.Filter = "DXF files (*.dxf)|*.dxf|All files (*.*)|*.*";

                    if (openDlg.ShowDialog() == DialogResult.OK)
                    {
                        string file = openDlg.FileName;

                        // Create a new object and read the file.
                        DXFFile dxfFile = new DXFFile(project, file, true);

                        TreeNode node = new TreeNode(dxfFile.FileName);
                        node.Tag = dxfFile;
                        node.Checked = true;

                        if (nodeDXF != null)
                            nodeDXF.Nodes.Add(node);

                        tvProject.SetNodeType(node, true);
                        node.Checked = true;
                        tvProject.UpdateTree();

                        tvProject.ExpandAll();

                        GlobalNotifier.Invoke(this, dxfFile, GlobalNotifier.MsgTypes.ToggleDXFFile);
                    }

                    openDlg.Filter = filter;
                    break;
            }
        }

        private void tsmiOpenPKT_Click(object sender, EventArgs e)
        {
            openDlg.Filter = "PKT files (*.pkt)|*.pkt|All files (*.*)|*.*";

            if (openDlg.ShowDialog() == DialogResult.OK)
            {
                // Create a record according to the node type.
                TreeNodeType type = GetNodeType(tvProject.SelectedNode);
                SonarRecord record;

                switch (type)
                {
                    case TreeNodeType.nRecord:
                        record = tvProject.SelectedNode.Tag as SonarRecord;
                        break;

                    case TreeNodeType.nProject:
                        record = new SonarRecord();
                        record.Description = Path.GetFileNameWithoutExtension(openDlg.FileName);
                        record.ShowInTrace = true;
                        record.ShowManualPoints = true;
                        project.AddRecord(record);
                        break;

                    default:
                        return;
                }

                // Open the PKT import form.
                frmPKTImport frm = new frmPKTImport(openDlg.FileName);

                if (frm.ShowDialog() != DialogResult.OK)
                    return;

                // Add the PKT list to the record.
                record.AddPKTList(frm.List);

                // Add the connection to the record.
                if (frm.Connection != null)
                {
                    project.BuoyList.Add(frm.Connection.StartBuoy);
                    project.BuoyList.Add(frm.Connection.EndBuoy);
                    project.BuoyConnectionList.Add(frm.Connection);
                }

                // Refresh project tree.
                GlobalNotifier.Invoke(this, project, GlobalNotifier.MsgTypes.TogglePoints);
            }
        }
        #endregion

        #region Save
        private void tsmiSaveConfig_Click(object sender, EventArgs e)
        {
            string tempDir = System.IO.Directory.GetCurrentDirectory();
            saveDlg.InitialDirectory = Application.StartupPath;
            if (saveDlg.ShowDialog() == DialogResult.OK)
                GSC.WriteXml(saveDlg.FileName);
            System.IO.Directory.SetCurrentDirectory(tempDir);
        }

        private void tsmiSaveBuoys_Click(object sender, EventArgs e)
        {
            if (saveDlg.ShowDialog() == DialogResult.OK)
                Buoy.WriteXml(saveDlg.FileName, project.BuoyList, project.BuoyConnectionList, GSC.Settings.NFI);
        }

        private void tsmiSaveBlankLineXml_Click(object sender, EventArgs e)
        {
            BlankLine blankLine = tvProject.SelectedNode.Tag as BlankLine;

            if (saveDlg.ShowDialog() == DialogResult.OK)
                blankLine.WriteToFileXml(saveDlg.FileName, false);
        }

        private void blanklineXMLLALOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BlankLine blankLine = tvProject.SelectedNode.Tag as BlankLine;

            if (saveDlg.ShowDialog() == DialogResult.OK)
                blankLine.WriteToFileXml(saveDlg.FileName, true);
        }

        private void tsmiSaveBlankLineSurfer_Click(object sender, EventArgs e)
        {
            BlankLine blankLine = tvProject.SelectedNode.Tag as BlankLine;

            string filter = saveDlg.Filter;
            saveDlg.Filter = "BLN files (*.bln)|*.bln|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveDlg.ShowDialog() == DialogResult.OK)
                blankLine.WriteToFileSurfer(saveDlg.FileName, false);

            saveDlg.Filter = filter;
        }

        private void tsmiSaveBlankLineSurferLALO_Click(object sender, EventArgs e)
        {
            BlankLine blankLine = tvProject.SelectedNode.Tag as BlankLine;

            string filter = saveDlg.Filter;
            saveDlg.Filter = "BLN files (*.bln)|*.bln|Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (saveDlg.ShowDialog() == DialogResult.OK)
                blankLine.WriteToFileSurfer(saveDlg.FileName, true);

            saveDlg.Filter = filter;
        }
        #endregion

        #region Empty
        private void tsmiEmpty_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNodeType type = GetNodeType(tvProject.SelectedNode);

                switch (type)
                {
                    case TreeNodeType.nDevice:
                        var device = tvProject.SelectedNode.Tag as SonarDevice;
                        //  var record = tvProject.SelectedNode.Parent.Tag as SonarRecord;

                        if (device != null)
                        {
                            device.SonarLines.Clear();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "frmProject.mnuInterpolate_Click: " + ex.Message);
            }
        }
        #endregion

        #region Enable Disable ALL
        private void tsmiEnableAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < project.RecordCount; i++)
                {

                    var rec = project.Record(i);
                    rec.ShowInTrace = true;
                    rec.ShowManualPoints = true;

                    UpdateRecord(project.IndexOf(rec), rec);
                }
            }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "tsmiEnableAll_Click: " + ex.Message + "\n" + ex.StackTrace);
            }


        }

        private void tsmiDisableAll_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < project.RecordCount; i++)
                {

                    var rec = project.Record(i);
                    rec.ShowInTrace = false;
                    rec.ShowManualPoints = false;

                    UpdateRecord(project.IndexOf(rec), rec);
                }
            }
            catch (Exception ex)
            {
                DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "tsmiDisableAll_Click: " + ex.Message + "\n" + ex.StackTrace);

            }
        }
        #endregion
        #endregion


    }
}
