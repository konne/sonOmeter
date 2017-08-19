using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using sonOmeter.Classes;
using System.Collections.Generic;
using sonOmeter.Controls;

namespace sonOmeter
{
    /// <summary>
    /// Summary description for frmVolWeight.
    /// </summary>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frmVolWeight : DockDotNET.DockWindow
    {
        class ArchDepthFixedPoint : INotifyPropertyChanged
        {
            public ArchDepthFixedPoint()
            {
                GSC.Settings.PropertyChanged += new PropertyChangedEventHandler(Settings_PropertyChanged);
            }

            void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
            {
                if ((e.PropertyName == "ArchDepth") && !blockEvent)
                {
                    archDepthFP = (int)(GSC.Settings.ArchDepth * 10.0F);
                    OnPropertyChanged("ArchDepthFP");
                }
                else if ((e.PropertyName == "ArchTopColorDepth") && !blockEvent)
                {
                    archTopColorDepthFP = (int)(GSC.Settings.ArchTopColorDepth * 10.0F);
                    OnPropertyChanged("ArchTopColorDepthFP");
                }
            }

            bool blockEvent = false;
            int archDepthFP = 1;
            public int ArchDepthFP
            {
                get { return archDepthFP; }
                set
                {
                    if ((value >= 0) & (value != archDepthFP))
                    {
                        archDepthFP = value;
                        blockEvent = true;
                        GSC.Settings.ArchDepth = (float)archDepthFP / 10.0F;
                        blockEvent = false;
                        OnPropertyChanged("ArchDepthFP");
                    }
                }
            }

            int archTopColorDepthFP = 1;
            public int ArchTopColorDepthFP
            {
                get { return archTopColorDepthFP; }
                set
                {
                    if ((value >= 0) & (value != archTopColorDepthFP))
                    {
                        archTopColorDepthFP = value;
                        blockEvent = true;
                        GSC.Settings.ArchTopColorDepth = (float)archTopColorDepthFP / 10.0F;
                        blockEvent = false;
                        OnPropertyChanged("ArchTopColorDepthFP");
                    }
                }
            }

            private void OnPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

       

        public frmVolWeight()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            foreach (Control item in Controls)
            {
                if (item.Name.StartsWith("hsValue"))
                    sonarColorScrollbars.Add(new Tuple<HScrollBar, int>((HScrollBar)item, int.Parse(item.Name.Replace("hsValue", ""))));
                else if (item.Name.StartsWith("lbValue"))
                    sonarColorLabels.Add(new Tuple<Label, int>((Label)item, int.Parse(item.Name.Replace("lbValue", ""))));
                else if (item.Name.StartsWith("sonarCCBH"))
                    sonarColorCheckboxesH.Add(new Tuple<SonarColorCheckBox, int>((SonarColorCheckBox)item, int.Parse(item.Name.Replace("sonarCCBH", ""))));
                else if (item.Name.StartsWith("sonarCCBC"))
                    sonarColorCheckboxesC.Add(new Tuple<SonarColorCheckBox, int>((SonarColorCheckBox)item, int.Parse(item.Name.Replace("sonarCCBC", ""))));
            }

            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), GlobalNotifier.MsgTypes.SwitchProperties);
        }

        List<Tuple<HScrollBar,int>> sonarColorScrollbars = new List<Tuple<HScrollBar,int>>();
        List<Tuple<Label, int>> sonarColorLabels = new List<Tuple<Label, int>>();
        List<Tuple<SonarColorCheckBox, int>> sonarColorCheckboxesH = new List<Tuple<SonarColorCheckBox, int>>();
        List<Tuple<SonarColorCheckBox, int>> sonarColorCheckboxesC = new List<Tuple<SonarColorCheckBox, int>>();

        ArchDepthFixedPoint archDepthFP;

        private void frmVolWeight_Load(object sender, EventArgs e)
        {
            archDepthFP = new ArchDepthFixedPoint();
            UpdateDataBindings();
        }

        public void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            UpdateDataBindings();
        }
   
        private void UpdateDataBindings()
        {
            foreach (Tuple<HScrollBar, int> item in sonarColorScrollbars)
            {
                item.Item1.DataBindings.Clear();
                item.Item1.DataBindings.Add("Value", GSC.Settings.SECL[item.Item2], "VolumeWeight", false, DataSourceUpdateMode.OnPropertyChanged);
            }

            foreach (Tuple<Label, int> item in sonarColorLabels)
            {
                item.Item1.DataBindings.Clear();
                item.Item1.DataBindings.Add("Text", GSC.Settings.SECL[item.Item2], "VolumeWeight");
            }

            foreach (Tuple<SonarColorCheckBox, int> item in sonarColorCheckboxesH)
            {
                item.Item1.DataBindings.Clear();
                item.Item1.DataBindings.Add("ForeColor", GSC.Settings.CS, "BackColor");
                item.Item1.DataBindings.Add("BackColor", GSC.Settings.SECL[item.Item2], "SonarColor");
                item.Item1.DataBindings.Add("Checked", GSC.Settings.SECL[item.Item2], "ArchStop", false, DataSourceUpdateMode.OnPropertyChanged);
            }

            foreach (Tuple<SonarColorCheckBox, int> item in sonarColorCheckboxesC)
            {
                item.Item1.DataBindings.Clear();
                item.Item1.DataBindings.Add("ForeColor", GSC.Settings.CS, "BackColor");
                item.Item1.DataBindings.Add("BackColor", GSC.Settings.SECL[item.Item2], "SonarColor");
                item.Item1.DataBindings.Add("Visible", GSC.Settings, "ArchDepthsIndependent");
                item.Item1.DataBindings.Add("Checked", GSC.Settings.SECL[item.Item2], "ArchStopColor", false, DataSourceUpdateMode.OnPropertyChanged);
            }

            cbStopStrongLayer.DataBindings.Clear(); cbStopStrongLayer.DataBindings.Add("Checked", GSC.Settings, "ArchStopAtStrongLayer", false, DataSourceUpdateMode.OnPropertyChanged);
            checkBoxActivate.DataBindings.Clear(); checkBoxActivate.DataBindings.Add("Checked", GSC.Settings, "ArchActive", false, DataSourceUpdateMode.OnPropertyChanged);

            trackDepth.DataBindings.Clear(); trackDepth.DataBindings.Add("Value", GSC.Settings, "ArchDepth", true, DataSourceUpdateMode.OnPropertyChanged);
            tbDepth.DataBindings.Clear(); tbDepth.DataBindings.Add("Text", GSC.Settings, "ArchDepth", true, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownDepth.DataBindings.Clear(); numericUpDownDepth.DataBindings.Add("Value", archDepthFP, "ArchDepthFP", false, DataSourceUpdateMode.OnPropertyChanged);

            cb3D.DataBindings.Clear();
            cb3D.DataBindings.Add("Checked", GSC.Settings, "ArchDepthsIndependent", false, DataSourceUpdateMode.OnPropertyChanged);
            if (GSC.Settings.Lic[Module.Modules.ThreeD])
                cb3D.DataBindings.Add("Visible", GSC.Settings, "Extended3DArch", false, DataSourceUpdateMode.OnPropertyChanged);
            else
                cb3D.Visible = false;

            trackSurface.DataBindings.Clear(); trackSurface.DataBindings.Add("Value", GSC.Settings, "ArchTopColorDepth", true, DataSourceUpdateMode.OnPropertyChanged);
            trackSurface.DataBindings.Add("Visible", GSC.Settings, "ArchDepthsIndependent");
            tbSurface.DataBindings.Clear(); tbSurface.DataBindings.Add("Text", GSC.Settings, "ArchTopColorDepth", true, DataSourceUpdateMode.OnPropertyChanged);
            tbSurface.DataBindings.Add("Visible", GSC.Settings, "ArchDepthsIndependent");
            numericUpDownSurface.DataBindings.Clear(); numericUpDownSurface.DataBindings.Add("Value", archDepthFP, "ArchTopColorDepthFP", false, DataSourceUpdateMode.OnPropertyChanged);
            numericUpDownSurface.DataBindings.Add("Visible", GSC.Settings, "ArchDepthsIndependent");
            
            label3D.DataBindings.Clear();
            label3D.DataBindings.Add("Visible", GSC.Settings, "ArchDepthsIndependent");
            label3D2.DataBindings.Clear();
            label3D2.DataBindings.Add("Visible", GSC.Settings, "ArchDepthsIndependent");

            if (!cb3D.Visible)
                cb3D.Checked = false;
        }

        private void checkBoxActivate_CheckedChanged(object sender, System.EventArgs e)
        {
            if (checkBoxActivate.Checked)
                checkBoxActivate.BackColor = SystemColors.ControlDark;
            else
                checkBoxActivate.BackColor = SystemColors.Control;
        }

        private void cbStopStrongLayer_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStopStrongLayer.Checked)
                cbStopStrongLayer.BackColor = SystemColors.ControlDark;
            else
                cbStopStrongLayer.BackColor = SystemColors.Control;
        }

        private void cb3D_CheckedChanged(object sender, EventArgs e)
        {
            if (cb3D.Checked)
                this.Size = this.MaximumSize;
            else
                this.Size = this.MinimumSize;
        }
    }
}
