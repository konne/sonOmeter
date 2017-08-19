using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;
using System.Drawing;
using UKLib.MathEx;

namespace sonOmeter
{
    public partial class frm3D : DockDotNET.DockWindow
    {
        private GlobalEventHandler globalEventHandler;
        private SonarProject project;
        private SonarPanelType type = SonarPanelType.HF;
        private TopColorMode mode = TopColorMode.Top;

        private EventHandler meshingCompletedEventHandler;
        
        #region Constructor, Creation and Dispose
        public frm3D(SonarProject project)
        {
            this.project = project;			
            GSC.PropertyChanged += new PropertyChangedEventHandler(OnSettingsChanged);
            
            InitializeComponent();

            if (!pan3D.InitializeGraphics())
            {
                MessageBox.Show("Could not initialize Direct3D.");
                return;
            }

            pan3D.OnResetDevice(this, EventArgs.Empty);
            pan3D.OnRender += new RenderEventHandler(OnRender);
            pan3D.OnDisposeDevice += new RenderEventHandler(OnDisposeDevice);

            globalEventHandler = new GlobalEventHandler(OnGlobalEvent);
            GlobalNotifier.SignIn(globalEventHandler, GlobalNotifier.MsgTypes.Toggle3DRecord);
            GlobalNotifier.SignIn(globalEventHandler, GlobalNotifier.MsgTypes.NewRecord);

            meshingCompletedEventHandler = new EventHandler(OnMeshingCompleted);
        }

        void OnDisposeDevice(object sender, RenderEventArgs e)
        {
            foreach (Sonar3DRecord rec3D in project.Records3D)
                rec3D.Dispose();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (globalEventHandler != null)
            {				
                GlobalNotifier.SignOut(globalEventHandler, GlobalNotifier.MsgTypes.Toggle3DRecord);
            }

            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
        #endregion

        public void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            switch (state)
            {
                case GlobalNotifier.MsgTypes.NewRecord:
                case GlobalNotifier.MsgTypes.Toggle3DRecord:
                    Sonar3DRecord rec3D = args as Sonar3DRecord;

                    if (rec3D == null)
                        return;

                    if (rec3D.ShowInTrace)
                        rec3D.MeshBuilt += meshingCompletedEventHandler;
                    else
                        rec3D.MeshBuilt -= meshingCompletedEventHandler;

                    RectangleD rc = project.CoordBounds3D();
                    pan3D.ViewDistance = (float)rc.Diag;
                    pan3D.Update3D();
                    break;
            }
        }

        public void OnMeshingCompleted(object sender, EventArgs e)
        {
            bool updateNow = true;

            foreach (Sonar3DRecord rec3D in project.Records3D)
                if (rec3D.IsMeshing)
                    updateNow = false;

            if (updateNow)
                pan3D.Update3D();
        }

        #region Settings Changed
        void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            /*if (this.Visible)
            {
                Interpolate3D();
            }*/
        }
        #endregion

        public void Interpolate3D()
        {
            project.Build3DMesh(type, mode);
        }

        void OnRender(object sender, RenderEventArgs e)
        {
            try
            {
                for (int i = 0; i < project.Records3D.Count; i++)
                {
                    Sonar3DRecord rec = project.Records3D[i];
                    if (rec.ShowInTrace)
                        if (e.BoundingBox)
                        {
                            if (!rec.DrawBoundingBox(e.Device))
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                        else
                        {
                            if (!rec.Draw3DCells(e.Device))
                            {
                                e.Cancel = true;
                                return;
                            }
                        }
                }
            }
            catch { }
        }

        #region Key events
        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                // function keys
                case Keys.F5:
                    type = SonarPanelType.HF;
                    Interpolate3D();
                    break;
                case Keys.F6:
                    type = SonarPanelType.NF;
                    Interpolate3D();
                    break;
                case Keys.F7:
                    mode = (TopColorMode)(((int)mode + 1) % 3);
                    Interpolate3D();
                    break;
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
        }
        #endregion

        #region Mouse
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            Point pt = pan3D.PointToClient(MousePosition);
            if (pan3D.ClientRectangle.Contains(pt))
                pan3D.InvokeOnMouseWheel(e);
        }
        #endregion
    }
}

