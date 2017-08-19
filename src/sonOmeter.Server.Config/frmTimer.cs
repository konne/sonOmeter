using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace sonOmeter.Server.Config
{
	public partial class frmTimer : Form
    {
        #region Variables & Properties
        /// <summary>
		/// The time in milliseconds this control will stay open.
		/// </summary>
		public int Time { get; set; }
	
		/// <summary>
		/// The text of the label control above the progress bar.
		/// </summary>
		public string Label
		{
			get { return labText.Text; }
			set { labText.Text = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
		/// Standard constructor.
		/// </summary>
		public frmTimer()
		{
			InitializeComponent();
        }
        #endregion

        #region FormLoad
        private void frmTimer_Load(object sender, EventArgs e)
		{
			pg.Maximum = Time / timer.Interval;

			timer.Enabled = true;
			timer.Start();
        }
        #endregion

        #region Timer
        private void timer_Tick(object sender, EventArgs e)
		{
			pg.PerformStep();

			if (pg.Value >= pg.Maximum)
			{
				timer.Stop();
				Close();
			}
        }
        #endregion

        #region FormClose
        private void frmTimer_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (pg.Value < pg.Maximum)
				this.DialogResult = DialogResult.Cancel;
			else
				this.DialogResult = DialogResult.OK;
        }
        #endregion

        #region Events
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion
    }
}