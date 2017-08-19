using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UKLib.Controls
{
    public partial class frmInputBox : Form
    {
        public string InputText
        {
            get { return tbInput.Text; }
            set { tbInput.Text = value; }
        }

        private void Init(string text, string caption)
        {
            InitializeComponent();           
            lbText.Text = text;
            this.Width = lbText.Width + 110;
            this.Text = caption;
        }

        public frmInputBox(string text, string caption, char passwordchar)
        {
            Init(text, caption);
            tbInput.PasswordChar = passwordchar;            
        }

        public frmInputBox(string text, string caption)
        {
           Init(text, caption);
        }

        public frmInputBox(string input, string text, string caption)
        {
            Init(text, caption);
            tbInput.Text = input;
        }
    }
}