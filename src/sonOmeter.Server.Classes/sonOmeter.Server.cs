using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using UKLib.Net.Sockets;
using UKLib.Xml;
using System.Xml;
using System.IO;
using System.Diagnostics;
using System.Xml.Serialization;

namespace sonOmeter.Server.Classes
{
	public class sonOmeterServerClass
	{
		#region Variables
		public List<SerialSonarLineSource> serSonar = new List<SerialSonarLineSource>();
		List<SerialPositionSource> serPos = new List<SerialPositionSource>();
	    SonarLineCollector collect;

		TcpServerThread srvData = new TcpServerThread();
		TcpServerThread srvCfg = new TcpServerThread();

		private ISynchronizeInvoke synchronizingObject;		
		#endregion

		#region Properties
        private sonOmeterServerConfig settings = new sonOmeterServerConfig();
        public sonOmeterServerConfig Settings
        {
            get
            {
                return settings;
            }
            set
            {
                if (value != null)
                {
                    settings = value;
                }
            }
        }

        [XmlIgnore]
		public ISynchronizeInvoke SynchronizingObject
		{
			get { return synchronizingObject; }
			set
			{
				synchronizingObject = value;
			}
		}

        [XmlIgnore]
		public bool isRunning
		{
			get { return srvData.Alive; }
		}
      		
		#endregion

		#region Constructor
		public sonOmeterServerClass()
		{

		}
		#endregion

		#region ParseDeviceList
		public void ParseDeviceList()
		{
            if (settings.SonDeviceList != null)
            {
                foreach (SonarDevice sd in settings.SonDeviceList)
                {                
                    SerialSonarLineSource ser = new SerialSonarLineSource(sd);

                    try
                    {
                        ser.Connect();
                        collect.ConnectToSource(ser);
                        serSonar.Add(ser);
                        ser.OnNewLogData += new SerialLogEventHandler(sonOmeterServerClass_OnNewLogData);
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("SD: " + ex.Message);
                    }
                }
            }

            if (settings.PosDeviceList != null)
            {
                foreach (PositionDevice pd in settings.PosDeviceList)
                {
                    SerialPositionSource pos = new SerialPositionSource(pd);

                    try
                    {
                        pos.Connect();
                        collect.ConnectToSource(pos);
                        serPos.Add(pos);
                        pos.OnNewLogData += new SerialLogEventHandler(sonOmeterServerClass_OnNewLogData);                        
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine("PD: " + ex.Message);
                    }                    
                }
            }       
		}
		#endregion
       
		#region Start / Stop DataServer

        StreamWriter logFile = null;

        public void InitDLSB30()
        {
            foreach (var item in serPos)
            {
                item.InitDLSB30();
            }

        }

        public void StartDataServer()
        {
            if (srvData.Alive)
            {
                StopDataServer();
            }

            collect = new SonarLineCollector();            
            collect.TmSon0ID0Use = settings.SonID0Timer;
            collect.FilterType32 = true;
            collect.FilterType33 = true;
            collect.FilterType37 = true;
            collect.OnNewSonarLine += new SonarLineEventHandler(collect_OnNewSonarLine);

            srvData.MaxNrClients = 5;
            srvData.SynchronizingObject = synchronizingObject;
            srvData.Start(settings.DataPort);
            srvData.Connect += new TcpClientThreadEventHandler(server_Connect);

            string fileName = AppDomain.CurrentDomain.BaseDirectory + "server.log";

            if (File.Exists(fileName))
            {
                try
                {
                    logFile = File.AppendText(fileName);
                }
                catch
                {
                    logFile = null;
                }
            }
            ParseDeviceList();
           // Console.WriteLine("Server Started");
        }


		public void StopDataServer()
		{
            if (logFile != null)
            {
                logFile.Close();
                logFile = null;
            }

			foreach (SerialSonarLineSource ser in serSonar)
			{
				ser.Disconnect();
			}
			foreach (SerialPositionSource pos in serPos)
			{
				pos.Disconnect();
			}
			collect.Clear();
			serSonar.Clear();
			serPos.Clear();

			srvData.Stop();
		}

		#endregion

		#region Events Data Server
		void server_Connect(object sender, TcpClientThreadEventArgs e)
		{
			string res = "<devicelist>" + collect.descriptiveXML + "</devicelist>\r\n";
			e.Client.SendString(res);
			e.Client.NewLine += new TcpLineEventHandler(Client_NewLine);
		}

		void Client_NewLine(object sender, TcpLineEventArgs e)
		{
			if (e.S == "quit")
				(sender as TcpClientThread).Close();
		}
		#endregion

		#region Events Collect

		void collect_OnNewSonarLine(object sender, SonarLineEventArgs e)
		{
            string s = e.Line.ToString() + "\r\n";
			srvData.SendString(s);
            if (logFile != null)
            {
                lock (logFile)
                {
                    logFile.Write(s);
                }
            }
		}

		#endregion            
      
        #region Start / Stop ConfigServer

        public void StartConfigServer()
		{
			srvCfg.MaxNrClients = 2;
			srvCfg.SynchronizingObject = synchronizingObject;
			srvCfg.Start(Settings.ConfigPort);
			srvCfg.Connect += new TcpClientThreadEventHandler(srvCfg_Connect);
		}

		public void StopConfigServer()
		{
			if (srvData.Alive) StopDataServer();
			srvCfg.Stop();
		}

		#endregion

		#region Events Config Server
		void srvCfg_Connect(object sender, TcpClientThreadEventArgs e)
		{
			e.Client.NewLine += new TcpLineEventHandler(Client_NewLineCfg);
		}

		void Client_NewLineCfg(object sender, TcpLineEventArgs e)
		{
			try
			{
				if (e.S == "quit")
					(sender as TcpClientThread).Close();
                
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
						#region Start / Stop
						case "start":							
                            StartDataServer();
							break;

						case "stop":                                                       
							if (srvData.Alive)
							{								
								StopDataServer();
							}							
							break;
						#endregion

                        #region Status
                        case "status":                            
                            if (srvData.Alive)
                            {
                                (sender as TcpClientThread).SendString("<started />\r\n");
                            }
                            else
                            {
                                (sender as TcpClientThread).SendString("<stoped />\r\n");
                            }
                            break;
                        #endregion

                        #region GetConfig
                        case "getconfig":  
                            {
                            StringWriter sw = new StringWriter();
                            XmlSerializer xs = new XmlSerializer(typeof(sonOmeterServerConfig));
                            xs.Serialize(sw,settings);
                            string s = "";
                             s = sw.ToString();
                             s = s.Substring(s.IndexOf("<sonOmeterServerConfig"));                            
                            s = s.Replace("\r\n", "");
                                            
							(sender as TcpClientThread).SendString(s  + "\r\n");
                            }
							break;
						#endregion

                        #region Setconfig
                        case "setconfig":
							{
								string s = reader.ReadInnerXml();
                                try
                                {
                                    if (s != null)
                                    {
                                        var sr = new StringReader(s);
                                        var xs = new XmlSerializer(typeof(sonOmeterServerConfig));
                                        sonOmeterServerConfig set = (sonOmeterServerConfig)xs.Deserialize(sr);
                                        sr.Close();
                                        settings.CopyContent(set);                                        
                                    }
                                }
                                catch
                                { 
                                
                                }																
							}
							break;
						#endregion
										
						#region LogSerData
						case "log":

							//string comPort;

                            //comPort = reader.GetAttribute("ComPort");
                            //Debug.WriteLine(comPort);
                            //foreach (ISerialLog log in serPos)
                            //{
                            //    log.EnableLog = false;
                            //    if (log.Port == comPort)
                            //        log.EnableLog = true;
                            //}
                            //foreach (ISerialLog log in serSonar)
                            //{
                            //    log.EnableLog = false;
                            //    if (log.Port == comPort)
                            //        log.EnableLog = true;
                            //}

							break;
						#endregion

						#region Quit
						case "quit":
							(sender as TcpClientThread).Close();
							break;
						#endregion

						default:
							break;
					}
					reader.Read();
				}
			}
			catch
			{
			}
		}

		void sonOmeterServerClass_OnNewLogData(object sender, SerialLogEventArgs e)
		{
			srvCfg.SendString("<log>" + e.Data + "</log>\r\n");
		}
		#endregion
         
    }
}
