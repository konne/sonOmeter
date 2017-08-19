using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sonOmeter
{
    public partial class frmAppError : Form
    {
        public frmAppError(Exception ex)
        {
            InitializeComponent();

            tbError.Text = ex.Message + "\n\n" + ex.StackTrace;
        }
    }
}
