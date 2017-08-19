using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;

namespace sonOmeter
{
    public partial class frmSmallToolWindow : DockDotNET.DockWindow
    {
        #region Variables
        protected SonarProject project = null;

        protected GlobalEventHandler globalEventHandler;
        #endregion

        #region Constructor & InitializeTool
        public frmSmallToolWindow()
        {
            InitializeComponent();
        }
        
        protected virtual void InitializeToolWindow(SonarProject prj)
        {
            try
            {
                project = prj;

                globalEventHandler = new GlobalEventHandler(OnGlobalEvent);

                var filterlist = new List<GlobalNotifier.MsgTypes>();
                filterlist.Add(GlobalNotifier.MsgTypes.NewSonarLine);
                filterlist.Add(GlobalNotifier.MsgTypes.WorkLineChanged);

                GlobalNotifier.SignIn(globalEventHandler, filterlist);	
                
                GSC.PropertyChanged += new PropertyChangedEventHandler(OnSettingsChanged);
                OnSettingsChanged(this, new PropertyChangedEventArgs("All"));
            }
            catch
            {
            }
        }
        #endregion

        #region Events
        protected virtual void OnSettingsChanged(object sender, PropertyChangedEventArgs e)        
        {
            if (e.PropertyName.StartsWith("CS.") || (e.PropertyName == "All"))
            {
                this.BackColor = GSC.Settings.CS.BackColor;
                this.ForeColor = GSC.Settings.CS.ForeColor;

                Invalidate();
            }
        }

        protected virtual void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            switch (state)
            {
                case GlobalNotifier.MsgTypes.NewSonarLine:
                    SelectSonarLine(args as SonarLine, state);
                    break;
                case GlobalNotifier.MsgTypes.WorkLineChanged:
                    RecordEventArgs e = args as RecordEventArgs;
                    if (e.Tag != null)
                        SelectSonarLine(e.Tag as SonarLine, state);
                    break;
            }
        }

        protected virtual void SelectSonarLine(SonarLine line, GlobalNotifier.MsgTypes state)
        {
            if (line == null)
                return;

            // Overwrite for special functionality.
        }
        #endregion
    }
}
