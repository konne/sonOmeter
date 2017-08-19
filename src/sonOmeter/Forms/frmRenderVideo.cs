namespace sonOmeter
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using sonOmeter.Classes;
    using UKLib.QuickTime;
    using UKLib.Survey.Controls;
    using System.IO;
    using UKLib.Controls;
    using System.Drawing.Imaging;
    //using AForge.Video;
    //using AForge.Video.FFMPEG;    
    using sonOmeter.Classes.Sonar2D;
    using UKLib.MathEx;
    using UKLib.Survey.Math;
    using UKLib.Survey.Parse;
    
    #endregion

    public partial class frmRenderVideo : Form
    {
        public frmRenderVideo()
        {
            InitializeComponent();
        }

        SonarDevice device = null;

        QTMovFile mvFile;
        int videoIn; 
        int videoOut;

        SonarProject project;
        SonarRecord record;
        public frmRenderVideo(SonarProject project, SonarRecord record, SonarDevice device, int videoIn, int videoOut)
        {
            InitializeComponent();
            this.device = device;
            this.record = record;
            this.project = project;
            this.videoIn = videoIn;
            this.videoOut = videoOut;

            //if (!File.Exists(record.LinkedVideoFile))
            //    return;
            
            //mvFile = new QTMovFile(record.LinkedVideoFile);


            pgbRenderVideo.Minimum = 0;
            pgbRenderVideo.Maximum = (int)(record.TimeEnd - record.TimeStart).TotalMilliseconds;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            //dlgSave.FileName = "test.mp4";

            //if (dlgSave.ShowDialog() != DialogResult.OK)
            //    return;
       
            ////QTMovFile mvNew = QTMovFile.EmptyMPJEGVideoFile(dlgSave.FileName);

            //var vfw = new VideoFileWriter();
            //vfw.Open(dlgSave.FileName, 1900, 1080, 1, VideoCodec.MPEG4);

            //CompassControl ctCompass = new CompassControl();
            //ctCompass.Smooth = false;
            //ctCompass.ShowFrame = true;
            //ctCompass.ShowSpeed = false;
            //ctCompass.BackColor = Color.Blue;
            //HorizonControl ctHorizon = new HorizonControl();
            //ctHorizon.Smooth = false;
            //ctHorizon.ShowFrame = true;
            //ctHorizon.BackColor = Color.Blue;

            //InstrumentControlPaintVars icpv;
           
            //var fnt = new Font(FontFamily.GenericMonospace, 30);
            //var white = new SolidBrush(Color.White);
            //var black = new SolidBrush(Color.Black);
            //pgbRenderVideo.Value = pgbRenderVideo.Minimum;
            //double? lastvalidAzimuth = null;
            //double? lastDinstance = null;
            //try
            //{
            //    foreach (SonarLine line in record.Device(0).SonarLines)
            //    {
            //        pgbRenderVideo.Value = (int)(line.Time-record.TimeStart).TotalMilliseconds;
            //        double? Distance=null;
            //        double? Station=null;
            //        PointD? ptCoord = null;
            //        double? al = null;
            //        #region Check Coord
            //        if (line.CoordRvHv.Type != CoordinateType.Empty)
            //        {
            //            double dNearest = 0;
            //            BuoyConnection cNearest = null;
            //            PointD pt = line.CoordRvHv.Point + line.LockOffset;
            //            al = line.CoordRvHv.AL;
            //            ptCoord = pt;

            //            if (project.GetNearestConnection(pt, ref cNearest, ref dNearest, false))
            //            {
            //                Distance = cNearest.GetCorridorDistance(pt);
            //                Station = cNearest.GetStation(pt);
            //            }
            //        } 
            //        #endregion

            //        double? azimuth = null;
            //        double? distance = null;

            //        #region Parse Positions
            //        foreach (SonarPos pos in line.PosList)
            //        {

            //            switch (pos.type)
            //            {
            //                case PosType.DLSB30:
            //                    DLSB30 dls = new DLSB30(pos.strPos);
            //                    distance = dls.Distance;
            //                    break;

            //                case PosType.Compass:
            //                    var cmp = new Compass(pos.strPos) - GSC.Settings.CompassZero;
            //                    azimuth = cmp.Yaw;
            //                    break;
            //                case PosType.HMR3300:
            //                    var cmp2 = new HMR3300(pos.strPos) - GSC.Settings.CompassZero;
            //                    azimuth = cmp2.Yaw;
            //                    break;

            //                case PosType.NMEA:
            //                    NMEA nmea = new NMEA(pos.strPos, GSC.Settings.NFI);
            //                    //if (nmea.Type == "$GPVTG")
            //                    //    compassControl.Speed = nmea.Speed;

            //                    if (nmea.Type == "$SBG01")
            //                    {
            //                        var rotation = new RotD(nmea.Yaw, nmea.Pitch, nmea.Roll) - GSC.Settings.CompassZero;
            //                        azimuth = rotation.Yaw;
            //                    }
            //                    break;
            //                default:
            //                    break;
            //            }
            //        } 
            //        #endregion

            //        Bitmap bmp = new Bitmap(1900, 1080);
            //        Graphics g = Graphics.FromImage(bmp);

            //        g.FillRectangle(new SolidBrush(Color.Blue), 0, 1900, 880, 1080);
            //        //g.DrawRectangle(new Pen(Color.White), 0, 0, 160, 160);
            //        //g.DrawRectangle(new Pen(Color.White), 0, 160, 160, 160);
            //        //g.DrawRectangle(new Pen(Color.White), 0, 320, 160, 160);


            //        if (ptCoord.HasValue)
            //        {
            //           g.DrawString("RV:" + ptCoord.Value.X.ToString("0.000").PadLeft(12, ' ') + "m", fnt, white, new PointF(400, 950));
            //           g.DrawString("HV:" + ptCoord.Value.Y.ToString("0.000").PadLeft(12, ' ') + "m", fnt, white, new PointF(400, 1000));                     
            //        }
            //        if (al.HasValue)   g.DrawString("AL:" + al.Value.ToString("0.000").PadLeft(6, ' ') + "m", fnt, white, new PointF(1400, 1000));

            //        if (!distance.HasValue && lastDinstance.HasValue)
            //            distance = lastDinstance;

            //        if (distance.HasValue)
            //        {
            //            g.DrawString("DT:" + distance.Value.ToString("0.000").PadLeft(6, ' ') + "m", fnt, white, new PointF(1400, 950));
            //            lastDinstance = distance;
            //        }

            //        if (Station.HasValue) g.DrawString("Station:  " + Station.Value.ToString("0.00").PadLeft(7, ' ') + "m", fnt, white, new PointF(850, 950));
            //        if (Distance.HasValue) g.DrawString("Distance: " + Distance.Value.ToString("0.00").PadLeft(7, ' ') + "m", fnt, white, new PointF(850, 1000));
            //        g.DrawString("Time: " + line.Time.ToLongTimeString(), fnt, white, new PointF(20,950));
            //        g.DrawString("Date: " + line.Time.ToString("dd.MM.yy"), fnt, white, new PointF(20,1000));
                   
            //        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //        icpv = new InstrumentControlPaintVars(160, 160, Color.White, Color.White);
                 
            //        if (!azimuth.HasValue && lastvalidAzimuth.HasValue)
            //            azimuth = lastvalidAzimuth;

            //        if (azimuth.HasValue)
            //        {
            //            g.TranslateTransform(1900-160-20, 900);
            //            ctCompass.Azimuth = azimuth.Value;
            //            ctCompass.PaintInstrument(g, ref icpv);
            //            lastvalidAzimuth = azimuth;
            //            g.ResetTransform();
            //        }

            //        //g.TranslateTransform(0, 160);
            //        //icpv = new InstrumentControlPaintVars(160, 160, Color.White, Color.White);

            //        //ctHorizon.PaintInstrument(g, ref icpv);
            //        //g.ResetTransform();
            //        vfw.WriteVideoFrame(bmp, line.Time - record.TimeStart);

            //        g.Dispose();
            //        bmp.Dispose();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            //vfw.Close();

            //Close();
            ////
            ////mvNew.Close(true);
        }

        private void frmRenderVideo_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (mvFile != null)
                mvFile.Close();
        }
    }
}
