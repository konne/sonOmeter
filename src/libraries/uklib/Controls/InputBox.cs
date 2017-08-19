using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace UKLib.Controls
{
    #region InputBoxResult
    public class InputBoxResult
    {
        #region Variables
        private string inputText = "";
        private DialogResult dialogResult = DialogResult.None;
        #endregion

        #region Properties
        public string InputText
        {
            get { return inputText; }
        }

        public DialogResult DialogResult
        {
            get { return dialogResult; }
        }
        #endregion

        #region Constructor
        public InputBoxResult(DialogResult dialogResult, string inputText)
        {
            this.inputText = inputText;
            this.dialogResult = dialogResult;
        }
        #endregion

    }
    #endregion

    public static class InputBox
    {
        public static InputBoxResult Show(string text)
        {
            frmInputBox frm = new frmInputBox(text,"InputBox",'\0');
            return new InputBoxResult(frm.ShowDialog(), frm.InputText);
        }

        public static InputBoxResult Show(string text, string caption)
        {
            frmInputBox frm = new frmInputBox(text, caption, '\0');
            return new InputBoxResult(frm.ShowDialog(), frm.InputText);
        }

        public static InputBoxResult Show(string input, string text, string caption)
        {
            frmInputBox frm = new frmInputBox(input, text, caption);
            return new InputBoxResult(frm.ShowDialog(), frm.InputText);
        }

        public static InputBoxResult Show(string text, string caption, char passwordchar)
        {
            frmInputBox frm = new frmInputBox(text, caption, passwordchar);
            return new InputBoxResult(frm.ShowDialog(), frm.InputText);
        }
    }
}
