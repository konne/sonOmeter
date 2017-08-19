using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;

namespace sonOmeter.Controls
{
    public class SonarColorCheckBox : CheckBox
    {
        public SonarColorCheckBox()
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnResize(EventArgs e)
        {
            this.Width = 16;
            this.Height = 16;
        }
        
        protected override void OnPaint(PaintEventArgs pevent)
        {
            Graphics g = pevent.Graphics;

            int b = 3;

            g.Clear(this.BackColor);

            if (this.Checked)
                g.FillRectangle(new SolidBrush(this.ForeColor), b, b, this.Width - 2 * b, this.Height - 2 * b);
        }
    }
}
