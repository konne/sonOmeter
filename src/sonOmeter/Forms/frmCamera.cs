using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;
using System.Net;
using System.IO;
using UKLib.Net;
using UKLib.QuickTime;

namespace sonOmeter
{
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frmCamera : DockDotNET.DockWindow
    {     
        #region Variables & Properties
        SonarProject project = null;
        MJPEGStream mjpeg = new MJPEGStream();
        QTMovFile mvFile = null;
        string mvFileSelected = "";

        bool tryRecording = false;
        #endregion

        #region Settings
        private void SetSettings(bool changed)
        {
            mjpeg.VideoSource = "http://" + GSC.Settings.CameraHostname + "/axis-cgi/mjpg/video.cgi";
            mjpeg.Login = GSC.Settings.CameraUsername;
            mjpeg.Password = GSC.Settings.CameraPassword;            
        }

        void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            SetSettings(false);
        }
        #endregion       

        #region Constructor
        public frmCamera()
        {
            InitializeComponent();
        }
        
        public frmCamera(SonarProject project)
        {
            InitializeComponent();

            mjpeg.SynchronizingObject = this;
            mjpeg.NewFrame += new NewJPEGEventHandler(mjpeg_NewFrame);

            GSC.PropertyChanged += new PropertyChangedEventHandler(OnSettingsChanged);

            this.project = project;
            project.StartRecording += new EventHandler(project_StartRecording);
            project.StopRecoding += new EventHandler(project_StopRecoding);

            var filterlist = new List<GlobalNotifier.MsgTypes>();
            filterlist.Add(GlobalNotifier.MsgTypes.NewSonarLine);
            filterlist.Add(GlobalNotifier.MsgTypes.WorkLineChanged);

            GlobalNotifier.SignIn(new GlobalEventHandler(OnGlobalEvent), filterlist);
        }
        #endregion

        #region Workline changed
        protected virtual void OnGlobalEvent(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            SonarLine line = null;

            switch (state)
            {
                case GlobalNotifier.MsgTypes.NewSonarLine:
                    line = args as SonarLine;
                    if ((line.SonID == 0) && (mvFile != null))
                    {
                        int frameCount = mvFile.FrameCount;
                        if (frameCount > 0)
                            line.PosList.Add(new SonarPos(DateTime.Now, "40", true, frameCount.ToString()));
                    }
                    break;

                case GlobalNotifier.MsgTypes.WorkLineChanged:
                    RecordEventArgs e = args as RecordEventArgs;
                    line = e.Tag as SonarLine;

                    if (line == null)
                        return;

                    SonarRecord rec = e.Rec;

                    if ((rec == null) && (rec.LinkedVideoFile == ""))
                        return;

                    if (mvFileSelected != rec.LinkedVideoFile)
                    {
                        if (!File.Exists(rec.LinkedVideoFile))
                            return;

                        if (mvFile != null) mvFile.Close(false);
                        mvFile = new QTMovFile(rec.LinkedVideoFile);
                        mvFileSelected = rec.LinkedVideoFile;
                    }

                    if (mvFile == null)
                        return;

                    // Search in inverted order as the VideoMarker is expected to be added as one of the last position entries.
                    int max = line.PosList.Count - 1;

                    for (int i = max; i >= 0; i--)
                    {
                        if (line.PosList[i].type == PosType.VideoMarker)
                        {
                            int pos = 0;

                            if (!int.TryParse(line.PosList[i].strPos, out pos))
                                continue;

                            if (mvFile.FrameCount < pos)
                                continue;

                            lastBMP = new Bitmap(new MemoryStream(mvFile.GetFrame(pos), false));
                            ShowBMP(lastBMP);                           
                            break;
                        }
                    }
                    break;
            }
        }
        #endregion

        #region NewFrames

        Brush br = new SolidBrush(Color.Black);

        Bitmap lastBMP = null;

        bool dcheck = false;        

        void ShowBMP(Bitmap bmp1)
        {
            dcheck = true;
            try
            {
                Bitmap orgBmp = (Bitmap)bmp1.Clone();

                int iHeight = orgBmp.Height;
                int iWidth = orgBmp.Width;

                int width = pbCamera.Width;
                int height = pbCamera.Height;

                Bitmap bmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bmp);
                g.FillRectangle(br, 0, 0, width, height);

                double scale = (double)iHeight / (double)height;

                int nwidth = (int)(iWidth / scale);

                if (nwidth > width)
                {
                    scale = (double)iWidth / (double)width;
                    height = (int)(iHeight / scale);
                }
                else
                {
                    width = nwidth;
                }

                int x = (pbCamera.Width - width) / 2;
                int y = (pbCamera.Height - height) / 2;

                g.DrawImage(orgBmp, x, y, width, height);
               // g.DrawString(DateTime.Now.ToLongTimeString(), new Font(FontFamily.GenericSerif, 9), new SolidBrush(Color.Red), x + 200, y);
                g.Dispose();

                pbCamera.Image = bmp;
                pbCamera.Invalidate();               
            }
            catch
            { }
            dcheck = false;
        }

        DateTime dtLastFrame = DateTime.Now;

        void mjpeg_NewFrame(object sender, byte[] buf)
        {
            if (tryRecording & (mvFile == null))
            {
                try
                {
                    string dir = GSC.Settings.CameraVideoDir;
                    if (!Directory.Exists(dir)) dir = "";
                    string filename = dir + "record_" + DateTime.Now.ToString("ddMMyy_HHmm") + ".mov";
                    mvFile = QTMovFile.EmptyMPJEGVideoFile(filename);
                    project.NewRecord.LinkedVideoFile = filename;
                }
                catch
                {
                }
            }

            if (mvFile != null)
            {
                try
                {
                    TimeSpan ts = DateTime.Now - dtLastFrame;
                    double secs = 0;
                    if (ts.TotalSeconds < 10)
                        secs = ts.TotalSeconds;

                    mvFile.AppendFrame(buf,secs);                    
                    dtLastFrame = DateTime.Now;
                }
                catch
                {
                }

            }
            if (!dcheck)
            {
                lastBMP = new Bitmap(new MemoryStream(buf, false));
                ShowBMP(lastBMP);
            }
        }        

        #endregion

        #region StartStopRecordingEvents
        
        void project_StopRecoding(object sender, EventArgs e)
        {
            tryRecording = false;
            try
            {
                if (mvFile != null)
                {
                    mvFile.Close(true);
                    mvFile = null;
                }
            }
            catch
            {
            }            
        }

        void project_StartRecording(object sender, EventArgs e)
        {
            project_StopRecoding(sender, e);
            tryRecording = true;
        }
        #endregion
      
        #region Visible Changed
        private void frmCamera_VisibleChanged(object sender, EventArgs e)
        {
            SetSettings(true);           
            if (Visible)
            {
                mjpeg.Start();
            }
            else
            {
                try
                {
                    project_StopRecoding(this, EventArgs.Empty);
                    mjpeg.SignalToStop();
                    Application.DoEvents();
                    mjpeg.Stop();
                }
                catch
                {
                }
               
            }
        }
        #endregion

        #region onResize
        private void frmCamera_Resize(object sender, EventArgs e)
        {         
            if (lastBMP != null)
            {
                if (!dcheck) 
                ShowBMP(lastBMP);
            }
        }
        #endregion
    }
}