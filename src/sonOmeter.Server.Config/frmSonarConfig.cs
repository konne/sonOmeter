using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UKLib.Net.Sockets;
using sonOmeter.Server.Classes;
using System.IO;
using System.Xml;
using UKLib.Xml;
using System.Globalization;
using System.Diagnostics;
using System.Xml.Serialization;
using System.Threading;

namespace sonOmeter.Server.Config
{
	public partial class frmSonarConfig : Form
    {       
        #region Constructor & Load
        public frmSonarConfig()
		{
            InitializeComponent();           
		}

        public frmSonarConfig(sonOmeterServerClass server)
        {
            InitializeComponent();

            this.server = server;
        }


        frmTimer formTimer = new frmTimer();

        sonOmeterServerConfig serverConfig = null;


        private void frmSonarConfig_Load(object sender, EventArgs e)
        {
            if (server != null)
            {
                serverConfig = server.Settings;
            }
            else
            {
                configThread = new TcpClientThread();
                configThread.EOLTag = "\r\n";
                configThread.DisconnectTag = "<quit />" + configThread.EOLTag;
                configThread.SynchronizingObject = this;
                configThread.NewLine += new TcpLineEventHandler(configThread_NewLine);
                configThread.Connect("localhost", HostPort);
                configThread.ConnectTimeOut = 500;
                configThread.TryReConnect = false;
                
                Application.DoEvents();
                configThread.SendString("<getconfig />" + configThread.EOLTag);

                formTimer.Label = "Waiting for Connection to Server";
                formTimer.Time = 3000;
                formTimer.ShowDialog();
                
                if (serverConfig == null)
                {
                    MessageBox.Show("Could not connect. Press ok to close.");
                    Close();
                }
            }
            pgServerConfig.SelectedObject = serverConfig;
            RebuildActiveSonars();
        }


        #endregion

        #region Variables
        private TcpClientThread configThread = new TcpClientThread();
        NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        sonOmeterServerClass server = null;
		#endregion

		#region Properties

        int hostPort = 9100;
		public int HostPort
		{
            get { return hostPort; }
            set { hostPort = value; }
		}

        private bool connected
        {
            get
            {
                return true;
                /*if (configThread == null) return false;
                return configThread.Connected;*/
            }
        }
		#endregion
             
		#region NewLine event
		private void configThread_NewLine(object sender, TcpLineEventArgs e)
		{
            try
            {              
                XmlTextReader reader = new XmlTextReader(new StringReader(e.S));
                reader.WhitespaceHandling = WhitespaceHandling.None;

                reader.Read();

                while (!reader.EOF)
                {
                    if (reader.NodeType != XmlNodeType.Element)
                    {
                        reader.Read();
                        continue;
                    }

                    switch (reader.Name)
                    {
                        #region sonOmeterServerConfig
                        case "sonOmeterServerConfig":
                            try
                            {                                
                                var sr = new StringReader(e.S);
                                var xs = new XmlSerializer(typeof(sonOmeterServerConfig));
                                serverConfig = (sonOmeterServerConfig)xs.Deserialize(sr);
                                if (formTimer.Visible) formTimer.Close();
                            }
                            catch
                            {

                            }
                            break;
                        #endregion

                        #region Log
                        case "log":
                            string s = reader.ReadInnerXml();
                            try
                            {
                               // lbDataView.AddHexString(s);
                            }
                            catch
                            {
                            }
                            break;
                        #endregion
                     
                        default:
                            break;
                    }
                    reader.Read();
                }
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
            }

		}
		#endregion

        #region (Re)Start
        private void btnReStart_Click(object sender, EventArgs e)
        {
            if (server != null)
            {
                server.StartDataServer();
            }
            else
            {
                var sw = new StringWriter();
                var xs = new XmlSerializer(typeof(sonOmeterServerConfig));
                xs.Serialize(sw, serverConfig);
                string s = "";
                s = sw.ToString();
                s = s.Substring(s.IndexOf("<sonOmeterServerConfig"));
                s = s.Replace("\r\n", "");

                ConfigThreadSend("<setconfig>"+s+"</setconfig>");
                ConfigThreadSend("<start />");               
            }
            RebuildActiveSonars();
        }
        #endregion


        private void RebuildActiveSonars()
        {
            lbSonars.Items.Clear();
            if (server != null)
            {
                foreach (var item in server.serSonar)
                {
                    lbSonars.Items.Add(item);
                }
            }
            else
            {
            }
        }

        private void ConfigThreadSend(string s)
        {
            if (configThread.Connected)
            {
                configThread.SendString(s+configThread.EOLTag);
            }
        }
		
        #region OnClose
        private void frmSonarConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (configThread.Connected)
            {
                configThread.SendString("<quit />");
                configThread.Close();
            }
        }
        #endregion    

        private void lbSonars_SelectedIndexChanged(object sender, EventArgs e)
        {            
            TryGetSonConf();
        }

        private void TryGetSonConf()
        {
            pgSonarConfig.SelectedObject = null;
            if (lbSonars.SelectedItem != null)
            {
                SerialSonarLineSource ssls = lbSonars.SelectedItem as SerialSonarLineSource;

                if (server != null)
                {                   
                    if (ssls.SonDeviceReadConfig != null)
                    {
                        pgSonarConfig.SelectedObject = ssls.SonDeviceReadConfig;
                    }
                }
                else
                {
                    // tbd
                }
            }

            if (pgSonarConfig.SelectedObject == null)
            {
                lbTryRead.Visible = true;
                tmTryGetSonConf.Enabled = true;
            }
            else
            {
                tmTryGetSonConf.Enabled = false;
                lbTryRead.Visible = false;
            }
        }

        private void tmTryGetSonConf_Tick(object sender, EventArgs e)
        {           
            tmTryGetSonConf.Enabled = false;
            TryGetSonConf();
        }

        private void btnWriteSonConf_Click(object sender, EventArgs e)
        {
            if ((lbSonars.SelectedItem != null) & (pgSonarConfig.SelectedObject != null))
            {
                SerialSonarLineSource ssls = lbSonars.SelectedItem as SerialSonarLineSource;
                Nullable<SonarDeviceConfig> sd = (pgSonarConfig.SelectedObject as Nullable<SonarDeviceConfig>);

                if (server != null)
                {
                    ssls.SonDeviceWriteConfig = sd;
                    ssls.SonDeviceReadConfig = null;
                }
                else
                {
                    // tbd Send Config with TCPIP
                }
                TryGetSonConf();          
            }
        }
       
    }
}