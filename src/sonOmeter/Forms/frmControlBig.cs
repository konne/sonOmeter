using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace sonOmeter
{
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frmControlBig : DockDotNET.DockWindow
    {
        #region Variables

        public event BigControlerHandler OnBigControlClick;
        #endregion

        #region Create and Dispose
        public frmControlBig()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();

            if (this.ControlContainer != null)
                this.ControlContainer.Resize += new EventHandler(ControlContainer_Resize);
        }

        #endregion


        #region Events
        private void btnPrjSave_Click(object sender, System.EventArgs e)
        {
            if (OnBigControlClick != null)
                OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.ProjectSave));
        }

        private void btnStartStopRec_Click(object sender, System.EventArgs e)
        {
            if (OnBigControlClick != null)
            {
                if (btnStartStopRec.Text == "Start")
                    OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RecordStart));
                else
                    OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RecordStop));
            }
        }

        private void btnNextRecord_Click(object sender, System.EventArgs e)
        {
            if (OnBigControlClick != null)
            {
                OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RecordStop));
                OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RecordStart));
            }
        }

        private void btnTracking_Click(object sender, EventArgs e)
        {
            if (OnBigControlClick != null)
            {
                if (btnTracking.Text == "Start Track")
                    OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.TrackingStart));
                else
                    OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.TrackingStop));
            }
        }

        public void RefreshButtons(BigControlEventType type)
        {
            switch (type)
            {
                case BigControlEventType.RecordStart:
                    btnStartStopRec.Text = "Stop";
                    btnNextRecord.Enabled = true;
                    btnSim.Enabled = false;
                    break;
                case BigControlEventType.RecordStop:
                    btnStartStopRec.Text = "Start";
                    btnNextRecord.Enabled = false;
                    btnSim.Enabled = true;
                    break;
                case BigControlEventType.TrackingStart:
                    btnTracking.Text = "Stop Track";
                    break;
                case BigControlEventType.TrackingStop:
                    btnTracking.Text = "Start Track";
                    break;
                case BigControlEventType.Simulate:
                    btnStartStopRec.Text = "Stop";
                    btnNextRecord.Enabled = false;
                    btnSim.Enabled = false;
                    break;
            }
        }

        private void btnConfigServer_Click(object sender, EventArgs e)
        {
            if (OnBigControlClick != null)
            {
                OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.RestartServer));
            }
        }

        private void btnManualInput_Click(object sender, EventArgs e)
        {
            if (OnBigControlClick != null)
            {
                OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.ManualInput));
            }
        }

        private void btnSim_Click(object sender, EventArgs e)
        {
            if (OnBigControlClick != null)
            {
                OnBigControlClick(this, new BigControlEventArgs(BigControlEventType.Simulate));
            }
        }
        #endregion

        void ControlContainer_Resize(object sender, EventArgs e)
        {
            int x = 5;
            int y = 5;

            int size = Math.Min(ControlContainer.Width, ControlContainer.Height) - 15;

            foreach (Control c in this.ControlContainer.Controls)
            {

                if (c is Button)
                {
                    Button b = c as Button;

                    b.Height = size;
                    b.Width = size;
                    b.Location = new Point(x, y);

                    x += size + 5;

                    if ((x + 88) >= this.ControlContainer.Width)
                    {
                        y += size + 5;
                        x = 5;
                    }
                }
            }

        }
    }
}

