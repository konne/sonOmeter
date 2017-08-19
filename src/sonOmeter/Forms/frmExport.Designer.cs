namespace sonOmeter
{
    public partial class frmExport
    {
            #region Designer Variables
        private System.ComponentModel.IContainer components;

        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private UKLib.Fireball.Syntax.SyntaxDocument synDoc;

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnOpen;
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmExport));
            this.btnCompile = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.synDoc = new UKLib.Fireball.Syntax.SyntaxDocument(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnOpenSettings = new System.Windows.Forms.Button();
            this.propGrid = new System.Windows.Forms.PropertyGrid();
            this.codeEditor = new UKLib.Fireball.Windows.Forms.CodeEditorControl();
            this.errorList1 = new UKLib.ErrorList.ErrorList();
            this.dlgOpen = new System.Windows.Forms.OpenFileDialog();
            this.dlgSave = new System.Windows.Forms.SaveFileDialog();
            this.dlgOpenExportSettings = new System.Windows.Forms.OpenFileDialog();
            this.dlgSaveExportSettings = new System.Windows.Forms.SaveFileDialog();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCompile
            // 
            this.btnCompile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCompile.Location = new System.Drawing.Point(8, 544);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(88, 23);
            this.btnCompile.TabIndex = 1;
            this.btnCompile.Text = "Compile";
            this.btnCompile.Click += new System.EventHandler(this.OnCompile);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(416, 544);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(88, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Ok";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(520, 544);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // synDoc
            // 
            this.synDoc.Lines = new string[] {
        ""};
            this.synDoc.MaxUndoBufferSize = 1000;
            this.synDoc.Modified = false;
            this.synDoc.UndoStep = 0;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(196, 544);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(88, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpen.Location = new System.Drawing.Point(102, 544);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(88, 23);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(8, 6);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.errorList1);
            this.splitContainer1.Size = new System.Drawing.Size(600, 532);
            this.splitContainer1.SplitterDistance = 380;
            this.splitContainer1.TabIndex = 16;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btnSaveSettings);
            this.splitContainer2.Panel1.Controls.Add(this.btnOpenSettings);
            this.splitContainer2.Panel1.Controls.Add(this.propGrid);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.codeEditor);
            this.splitContainer2.Size = new System.Drawing.Size(600, 380);
            this.splitContainer2.SplitterDistance = 191;
            this.splitContainer2.TabIndex = 0;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveSettings.Location = new System.Drawing.Point(116, 357);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSaveSettings.TabIndex = 1;
            this.btnSaveSettings.Text = "Save";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // btnOpenSettings
            // 
            this.btnOpenSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOpenSettings.Location = new System.Drawing.Point(0, 357);
            this.btnOpenSettings.Name = "btnOpenSettings";
            this.btnOpenSettings.Size = new System.Drawing.Size(75, 23);
            this.btnOpenSettings.TabIndex = 1;
            this.btnOpenSettings.Text = "Open";
            this.btnOpenSettings.UseVisualStyleBackColor = true;
            this.btnOpenSettings.Click += new System.EventHandler(this.btnOpenSettings_Click);
            // 
            // propGrid
            // 
            this.propGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.propGrid.Location = new System.Drawing.Point(0, 0);
            this.propGrid.Name = "propGrid";
            this.propGrid.Size = new System.Drawing.Size(191, 351);
            this.propGrid.TabIndex = 0;
            // 
            // codeEditor
            // 
            this.codeEditor.ActiveView = UKLib.Fireball.Windows.Forms.CodeEditor.ActiveView.BottomRight;
            this.codeEditor.AllowBreakPoints = false;
            this.codeEditor.AutoListPosition = null;
            this.codeEditor.AutoListSelectedText = "a123";
            this.codeEditor.AutoListVisible = false;
            this.codeEditor.CopyAsRTF = false;
            this.codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEditor.Document = this.synDoc;
            this.codeEditor.InfoTipCount = 1;
            this.codeEditor.InfoTipPosition = null;
            this.codeEditor.InfoTipSelectedIndex = 1;
            this.codeEditor.InfoTipVisible = false;
            this.codeEditor.Location = new System.Drawing.Point(0, 0);
            this.codeEditor.LockCursorUpdate = false;
            this.codeEditor.Name = "codeEditor";
            this.codeEditor.Saved = false;
            this.codeEditor.Size = new System.Drawing.Size(405, 380);
            this.codeEditor.SmoothScroll = false;
            this.codeEditor.SplitView = false;
            this.codeEditor.SplitviewH = -4;
            this.codeEditor.SplitviewV = -4;
            this.codeEditor.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(219)))), ((int)(((byte)(214)))));
            this.codeEditor.TabIndex = 0;
            this.codeEditor.Text = "codeEditorControl1";
            this.codeEditor.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            this.codeEditor.TextChanged += new System.EventHandler(this.codeEditor_TextChanged);
            this.codeEditor.KeyDown += new System.Windows.Forms.KeyEventHandler(this.codeEditor_KeyDown);
            // 
            // errorList1
            // 
            this.errorList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorList1.ErrorsVisible = true;
            this.errorList1.Location = new System.Drawing.Point(0, 0);
            this.errorList1.MessagesVisible = true;
            this.errorList1.Name = "errorList1";
            this.errorList1.Size = new System.Drawing.Size(600, 148);
            this.errorList1.TabIndex = 0;
            this.errorList1.WarningsVisible = true;
            this.errorList1.ItemDblClick += new UKLib.ErrorList.ErrorListEventHandler(this.errorList1_ItemDblClick);
            // 
            // dlgOpen
            // 
            this.dlgOpen.DefaultExt = "*.cs";
            this.dlgOpen.Filter = "Code files (*.cs)|*.cs|All files (*.*)|*.*";
            // 
            // dlgSave
            // 
            this.dlgSave.DefaultExt = "*.cs";
            this.dlgSave.Filter = "Code files (*.cs)|*.cs|All files (*.*)|*.*";
            // 
            // dlgOpenExportSettings
            // 
            this.dlgOpenExportSettings.DefaultExt = "*.xml";
            this.dlgOpenExportSettings.Filter = "Export settings (*.xml)|*.xml|All files (*.*)|*.*";
            // 
            // dlgSaveExportSettings
            // 
            this.dlgSaveExportSettings.DefaultExt = "*.xml";
            this.dlgSaveExportSettings.Filter = "Export settings (*.xml)|*.xml|All files (*.*)|*.*";
            // 
            // frmExport
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(618, 575);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCompile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmExport";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export";
            this.Load += new System.EventHandler(this.frmExport_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Button btnOpenSettings;
        private System.Windows.Forms.OpenFileDialog dlgOpenExportSettings;
        private System.Windows.Forms.SaveFileDialog dlgSaveExportSettings;

    }
}
