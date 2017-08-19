using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using sonOmeter.Classes;
using UKLib.Net.Sockets;
using System.Net;
using UKLib.Survey.Parse;

namespace sonOmeter
{
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public partial class frmUbootControl : DockDotNET.DockWindow
    {
        #region Enums
        enum BtnJoy
        {
            Pump = 0,
            Valve = 1,
            QuerL = 4,
            QuerR = 5,
            LightL = 6,
            LightR = 7,
            ZoomOut = 8,
            ZoomIn = 9,
            AutoPilot = 10,
            SpeedMode = 11
        }
        #endregion

        #region Variables
        TcpClientThread tcp = null;
        Joystick joystick = null;
        ISynchronizeInvoke SychroObject;

        private int ZoomLevel = 1;

        private bool QuerL = false;
        private bool QuerR = false;
        #endregion

        #region Constructor
        public frmUbootControl()
        {
            InitializeComponent();
        }

        public frmUbootControl(ISynchronizeInvoke SychroObject, SonarProject prj)
        {
            InitializeComponent();
            this.SychroObject = SychroObject;
            
            GlobalNotifier.SignIn(new GlobalEventHandler(OnNewSonarLine), GlobalNotifier.MsgTypes.NewSonarLine);

            this.Load += new EventHandler(frmUbootControl_Load);
            this.FormClosing += new FormClosingEventHandler(frmUbootControl_FormClosing);
        }

        void frmUbootControl_Load(object sender, EventArgs e)
        {
            Reconnect();
        }

        void frmUbootControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (joystick != null) joystick.Close();
            joystick = null;

            if (tcp != null) tcp.Close();
            tcp = null;
        }
        #endregion

        public delegate void UbootBatteryEventHandler(int battery, double voltage);

        public event UbootBatteryEventHandler UbootBatteryChanged;

        #region New SonarLine
        DateTime dtLastDeep = DateTime.Now;


        private void OnNewSonarLine(object sender, object args, GlobalNotifier.MsgTypes state)
        {
            SonarLine line = args as SonarLine;

            foreach (SonarPos pos in line.PosList)
            {
                switch (pos.type)
                {
                    case PosType.Depth:
                        Nullable<float> deep = SosoDepthGauge.ToFloat(pos.strPos);
                        if (deep.HasValue)
                        {                          
                            DateTime dtNow = DateTime.Now;
                            if ((Math.Abs(deep.Value -  ubootControl1.Deep) >= 0.1F) | ((dtNow - dtLastDeep).TotalSeconds > 3))
                            {
                              
                                double deepChange = (ubootControl1.Deep - deep.Value) / ((dtLastDeep - dtNow).TotalSeconds);
                                dtLastDeep = dtNow;                                                
                                ubootControl1.DeepChange = (float)deepChange;
                                ubootControl1.Deep = deep.Value;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Functions

        private void SetTilPanZoom(int Tilt, int Pan, int Zoom)
        {
            HttpWebRequest req;
                   
            string strURL = "http://" + GSC.Settings.CameraHostname + "/axis-cgi/com/ptz.cgi?camera=1";
    
            strURL += "&pan=" + Pan.ToString();
            strURL += "&tilt=" + Tilt.ToString();
            strURL += "&zoom=" + Zoom.ToString();    
            try
            {
                req = (HttpWebRequest)WebRequest.Create(strURL);
                req.KeepAlive = false;
                req.ContentType = "text/html";
                req.AllowAutoRedirect = false;
                req.Timeout = 10;
                WebResponse  webresponse =  req.GetResponse();
                webresponse.Close();                                
            }
            catch (Exception ex)
            {
                
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
            }
        }

        private void SendCommand(string cmd)
        {
            try
            {
                int checksum = 0;
                for (int i = 0; i < cmd.Length; i++)
                {
                    checksum += Convert.ToByte(cmd[i]);
                    if (checksum > 255) checksum -= 256;
                }
                checksum &= 0xff;
                byte chk = (byte)checksum;
                cmd += chk.ToString("X2") + "\r\n";
                if (tcp != null)
                    tcp.SendString(cmd);
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message + "com=" + cmd);
            }
        }
        #endregion

        #region TCP New Line
        void tcp_NewLine(object sender, TcpLineEventArgs e)
        {            
            if (e.S.Substring(0, 2) == "B0")
            {
                string s = e.S.Substring(2, 4);
                try
                {
                    int Sensor = Int16.Parse(s.Substring(0, 1));
                    double Volt = Int16.Parse(s.Substring(1), System.Globalization.NumberStyles.HexNumber) * 0.026;
                    if (UbootBatteryChanged != null)
                        UbootBatteryChanged(Sensor, Volt);
                }
                catch
                {
                }

            }
        }

        #endregion

        #region Joystick Events
        void joystick_JoystickXYZChanged(int X, int Y, int Z)
        {
            ubootControl1.UpDown = (Y - 32191) / 3200F;
            ubootControl1.LeftRight = (X - 32511) / 3200F;
            if (ubootControl1.SpeedMode)
                ubootControl1.Speed += (Z - 32703) / 20000F;
            else
                ubootControl1.Speed = (Z - 32703) / 3100F;
        }

        DateTime dtValvepres = DateTime.Now;
        void joystick_JoystickButtonChanged(int Button, bool State)
        {
            if (Button == (int)BtnJoy.Valve)
            {
                if (State)
                    dtValvepres = DateTime.Now;
                else
                {
                    double time = Math.Abs((dtValvepres - DateTime.Now).TotalSeconds);
                    if (time > 4)
                        State = true;
                }
                ubootControl1.Valve = State;
            }

            if (Button == (int)BtnJoy.Pump)
            {
                ubootControl1.Pump = State;
            }


            if (Button == (int)BtnJoy.QuerL)
            {
                QuerL = State;
            }
            if (Button == (int)BtnJoy.QuerR)
            {
                QuerR = State;
            }

            if ((Button == (int)BtnJoy.LightL) & State)
            {
                ubootControl1.LightL = !ubootControl1.LightL;
            }
            if ((Button == (int)BtnJoy.LightR) & State) ubootControl1.LightR = !ubootControl1.LightR;

            if ((Button == (int)BtnJoy.SpeedMode) & State)
            {
                ubootControl1.SpeedMode = !ubootControl1.SpeedMode;
                if (!ubootControl1.SpeedMode)
                    ubootControl1.Speed = 0;
            }

            #region Zoom
            if ((Button == (int)BtnJoy.ZoomIn) & State)
            {
                switch (ZoomLevel)
                {
                    case 1:
                        ZoomLevel = 3000;
                        break;
                    case 3000:
                        ZoomLevel = 6000;
                        break;
                    case 6000:
                        ZoomLevel = 9999;
                        break;
                    case 9999:
                        ZoomLevel = 9999;
                        break;
                    default:
                        ZoomLevel = 1;
                        break;
                }

                SetTilPanZoom(0, 0, ZoomLevel);
            }
            if ((Button == (int)BtnJoy.ZoomOut) & State)
            {
                switch (ZoomLevel)
                {
                    case 1:
                        ZoomLevel = 1;
                        break;
                    case 3000:
                        ZoomLevel = 1;
                        break;
                    case 6000:
                        ZoomLevel = 3000;
                        break;
                    case 9999:
                        ZoomLevel = 6000;
                        break;
                    default:
                        ZoomLevel = 1;
                        break;
                }

                SetTilPanZoom(0, 0, ZoomLevel);
            }
            #endregion
        }
        #endregion

        #region SendStatusTimer
        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((tcp != null) && (tcp.Connected))
            {
                byte num = 0;
                if (QuerL) num = (byte)(num + 0x40);
                if (QuerR) num = (byte)(num + 0x10);

                if (ubootControl1.LightR) num = (byte)(num + 0x80);
                if (ubootControl1.LightL) num = (byte)(num + 0x20);
                if (ubootControl1.Valve) num = (byte)(num + 8);
                if (ubootControl1.Pump) num = (byte)(num + 4);

                int motorR = 0;
                int motorL = 0;

                try
                {
                    motorR = (int)(ubootControl1.Speed * 25.5F);
                    motorL = motorR;

                    float minC = 0.3F;

                    if (Math.Abs(ubootControl1.Speed) > 2)
                    {
                        if (ubootControl1.LeftRight > minC)
                        {
                            motorR = 0;
                            motorL += (int)(ubootControl1.LeftRight * 10F);
                        }
                        else
                            if (ubootControl1.LeftRight < -minC)
                            {
                                motorR += (int)(-ubootControl1.LeftRight * 10F);
                                motorL = 0;
                            }
                    }
                    else
                    {
                        motorR += (int)(-ubootControl1.LeftRight * 10F);
                        motorL += (int)(ubootControl1.LeftRight * 10F);
                    }

                    if (motorR >= 0) num = (byte)(num + 2);
                    if (motorL >= 0) num = (byte)(num + 1);
                }
                catch (Exception ex)
                {
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
                }

                SendCommand("A" + Math.Abs(motorR).ToString("X2") + Math.Abs(motorL).ToString("X2") + num.ToString("X2"));
                SendCommand("B0");
                SendCommand("B1");
                SendCommand("B2");
                SendCommand("B3");
            }
            else
            {
                btnReConnect.Visible = true;
                btnReConnect.BringToFront();
            }
        }
        #endregion

        #region Reconnect
        private void Reconnect()
        {
            if (joystick != null) joystick.Close();
            joystick = new Joystick();
            joystick.SynchronizingObject = SychroObject;
            joystick.JoystickButtonChanged += new JoystickButtonChangedEventHandler(joystick_JoystickButtonChanged);
            joystick.JoystickXYZChanged += new JoystickXYZChangedEventHandler(joystick_JoystickXYZChanged);

            if (tcp != null) tcp.Close();
            tcp = new TcpClientThread();
            tcp.SynchronizingObject = SychroObject;
            tcp.Connect(GSC.Settings.SubmarineHostname, 10001);
            tcp.NewLine += new TcpLineEventHandler(tcp_NewLine);
            tmSendStatus.Enabled = true;
        }

        private void btnReConnect_Click(object sender, EventArgs e)
        {
            Reconnect();
            btnReConnect.Visible = false;
        }
        #endregion
    }
}