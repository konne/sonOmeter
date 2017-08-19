using System;
using System.Windows.Forms;
using sonOmeter.Classes;
using UKLib.ErrorList;
using UKLib.Fireball.Syntax;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for frmExport.
    /// </summary>
    public partial class frmExport : System.Windows.Forms.Form
    {

        #region Variables
        public SonarExport exp;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private PropertyGrid propGrid;
        private UKLib.Fireball.Windows.Forms.CodeEditorControl codeEditor;
        private ErrorList errorList1;
        private OpenFileDialog dlgOpen;
        private SaveFileDialog dlgSave;

        private bool codeTextChanged = false;

        #endregion

        #region Constr., Load & Destructor
        public frmExport()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            exp = new SonarExport();
            exp.ExpSettings.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ExpSettings_PropertyChanged);
            codeEditor.Document.Lines = GSC.Settings.ExportFunction;

           
            propGrid.SelectedObject = exp.ExpSettings;

            SynFileLoader.SetSyntax(codeEditor.Document, SyntaxLanguage.CSharp);

            OnCompile(null, null);
        }

        void ExpSettings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            propGrid.Refresh();
            propGrid.ExpandAllGridItems();
        }

        private void frmExport_Load(object sender, System.EventArgs e)
        {

        }

        #endregion

        #region Events

        private void OnCompile(object sender, System.EventArgs e)
        {
            GSC.Settings.ExportFunction = synDoc.Lines;
            foreach (Row row in codeEditor.Document)
                foreach (Word word in row)
                    word.HasError = false;
            exp.CompileExportFunc(errorList1.Items);
            foreach (ErrorListItem itm in errorList1.Items)
            {
                try
                {
                    codeEditor.Document.GetWordFromPos(new TextPoint(itm.Column - 1, itm.Line - 1)).HasError = true;
                }
                catch
                {
                }
            }
            errorList1.RefreshView();
            codeEditor.Refresh();
            btnOk.Enabled = exp.CompileOK;
            propGrid.Enabled = exp.CompileOK;
            propGrid.SelectedObject = exp.CompileOK ? exp.ExpSettings : null;
            codeTextChanged = false;
            SetCustomer();
        }


        private void btnOpen_Click(object sender, EventArgs e)
        {
            string tempDir = System.IO.Directory.GetCurrentDirectory();

            dlgOpen.InitialDirectory = Application.StartupPath;
            if (dlgOpen.ShowDialog() == DialogResult.OK)
            {
                codeEditor.Open(dlgOpen.FileName);
                codeTextChanged = false;
                OnCompile(this, EventArgs.Empty);
            }
            System.IO.Directory.SetCurrentDirectory(tempDir);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string tempDir = System.IO.Directory.GetCurrentDirectory();

            dlgSave.InitialDirectory = Application.StartupPath;
            if (dlgSave.ShowDialog() == DialogResult.OK)
            {
                codeEditor.Save(dlgSave.FileName);
                codeTextChanged = false;
            }
            System.IO.Directory.SetCurrentDirectory(tempDir);
        }

        private void codeEditor_TextChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = false;
            codeTextChanged = true;
        }

        private void errorList1_ItemDblClick(object sender, ErrorListArgs e)
        {
            codeEditor.Document[e.Item.Line].EnsureVisible();
            codeEditor.Caret.Position.Y = e.Item.Line - 1;
            codeEditor.Caret.Position.X = e.Item.Column - 1;
            codeEditor.ScrollIntoView();
            codeEditor.Refresh();
            codeEditor.Selection.Bounds.FirstColumn = e.Item.Column - 1;
            codeEditor.Selection.Bounds.FirstRow = e.Item.Line - 1;
            codeEditor.Selection.SelLength = codeEditor.Caret.CurrentWord.Text.Length;
            codeEditor.Refresh();
            codeEditor.Focus();
        }


        private void codeEditor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F6)
                OnCompile(null, null);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            bool close = true;
            if (codeTextChanged)
            {
                if (MessageBox.Show("Code changed. Exit without save?", "Question", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                    close = false;
            }
            if (close)
            {
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        #endregion

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;           
            Close();
        }
      
        private void SetCustomer()
        {            
            foreach (var item in exp.ExpSettings.AvExportCustomer)
            {               
                    exp.ExpSettings.Customer = item;
                    break;
            }
        }

        private void btnOpenSettings_Click(object sender, EventArgs e)
        {
            if (dlgOpenExportSettings.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            exp.Deserialize(dlgOpenExportSettings.FileName);
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (dlgSaveExportSettings.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;

            exp.Serialize(dlgSaveExportSettings.FileName);
        }
    }
}
