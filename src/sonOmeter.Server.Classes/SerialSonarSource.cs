using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO.Ports;
using UKLib.Debug;
using UKLib.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace sonOmeter.Server.Classes
{
	public class SerialSonarLineSource : ISonarLineSource, ISerialSource, ISerialLog
	{
		#region ISonarLineSource Members

		public event SonarLineEventHandler OnNewSonarLine;

		public string descriptiveXML
		{
            get
            {
                System.Globalization.NumberFormatInfo nfi = new System.Globalization.CultureInfo("en-US", false).NumberFormat;
                string conf = "";
                if (sonDeviceReadConfig != null)
                    conf = sonDeviceReadConfig.Value.ToXMLString();
                return (string.Format(nfi, "<sondevice sonid=\"{0}\" dx=\"{1}\" dy=\"{2}\" dz=\"{3}\" "
                    + "dp=\"{4}\" dr=\"{5}\" desc=\"{6}\" {7} />",
                        SonDevice.ID, SonDevice.DX, SonDevice.DY, SonDevice.DZ, SonDevice.DP, SonDevice.DR, SonDevice.Description, conf));
            }
		}

		#endregion

		#region ISerialLog Members

		public event SerialLogEventHandler OnNewLogData;

		private bool enabelLog = false;

		public bool EnableLog
		{
			get
			{
				return enabelLog;
			}
			set
			{
				enabelLog = value;
			}
		}

		public string Port
		{
			get { return SonDevice.Port; }
		}
		#endregion

		#region Variables

		private SonarLine tempSonarLine;
		private SerialPort com;
        private TcpClientThread tcp;

		private int readConfCount = 15;

		private float calcDepth = float.NaN;

		Queue<byte> InputBuffer = new Queue<byte>();

        private SonarDevice SonDevice;
		#endregion

		#region Properties
		public int SonID
		{
			get { return SonDevice.ID; }
		}

        private Nullable<SonarDeviceConfig> sonDeviceReadConfig = null;
        public Nullable<SonarDeviceConfig> SonDeviceReadConfig
        {
            get
            {
                return sonDeviceReadConfig;
            }
            set
            {
                if (value == null)
                    sonDeviceReadConfig = null;
            }
        }

        private Nullable<SonarDeviceConfig> sonDeviceWriteConfig = null;
        public Nullable<SonarDeviceConfig> SonDeviceWriteConfig
        {
            get
            {
                return sonDeviceWriteConfig;
            }
            set
            {
                if (value != null)
                    sonDeviceWriteConfig = value;
            }
        }

		#endregion

		#region Constructor
		public SerialSonarLineSource(SonarDevice sonDevice)
		{
            SonDevice = (SonarDevice)sonDevice.Clone();
           
            com = new SerialPort();
            com.ReceivedBytesThreshold = 10;
            com.DataReceived += new SerialDataReceivedEventHandler(com_DataReceived);
            tcp = new TcpClientThread();
            tcp.NewData += new TcpDataEventHandler(tcp_NewData);
            tcp.ReconnectMaxIdleSeconds = 20;
		}     
		#endregion

		#region (Dis-)Connect

		public void Connect()
		{
			try
			{
                if (tcp.Connected)
                    tcp.Close();                   

				if (com.IsOpen)
					com.Close();					

                if (SonDevice.Port.IndexOf("COM") != 0)
                {
                    tcp.Connect(SonDevice.Port, SonDevice.Parameter);
                }
                else
                {
                    com.PortName = SonDevice.Port;
                    com.BaudRate = SonDevice.Parameter;
                    com.Handshake = Handshake.None;
                    com.Open();
                }
				InputBuffer.Clear();
			}
			catch (Exception ex)
			{
				DebugClass.SendDebugLine(this, DebugLevel.Red, "SerialSonarSource.Connect(): " + ex.Message);
			}
		}

		public void Disconnect()
		{
			try
			{
                tcp.Close();
				com.Close();
				InputBuffer.Clear();
			}
			catch (Exception ex)
			{
				DebugClass.SendDebugLine(this, DebugLevel.Red, "SerialSonarSource.Disconnect(): " + ex.Message);
			}
		}
		#endregion

		#region Events

		private void SendSonarLine(SonarLine tempSonarLine)
		{		
			if (OnNewSonarLine != null)
				OnNewSonarLine(this, new SonarLineEventArgs(tempSonarLine));
		}

        void tcp_NewData(byte[] buf, int len)
        {
            try
            {               
                ProcessData(len, buf);               
            }
            catch (Exception ex)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message);
            }
        }

		void com_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{			
				int cnt = com.BytesToRead;
				byte[] data = new byte[cnt];
				com.Read(data, 0, cnt);

                ProcessData(cnt, data);
			}
			catch (Exception ex)
			{
				UKLib.Debug.DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message);
			}
		}

        private void ProcessData(int cnt, byte[] data)
        {
            string log = "";
            for (int i = 0; i < cnt; i++)
            {
                if (data[i] == 0xff)
                {
                    ProcessLine(InputBuffer);
                    InputBuffer.Clear();
                }
                else
                {
                    if (InputBuffer.Count < 1500)
                        InputBuffer.Enqueue(data[i]);
                    else
                        InputBuffer.Clear();
                }
                if (enabelLog) log += data[i].ToString("X");
            }
            if (enabelLog && (log != "") && (OnNewLogData != null))
                OnNewLogData(this, new SerialLogEventArgs(log));
        }
		#endregion

		#region ProcessLine
		private void ProcessLine(Queue<byte> buf)
		{
			string content;
			byte reserved1, reserved2;
                         
			#region SonConf
            try
            {
                if (SonarDeviceConfig.IsProgrammable(SonDevice.Format))
                {
                    if (sonDeviceReadConfig == null)
                    {
                        if (readConfCount > 20)
                        {
                            readConfCount = 0;
                            if (com.IsOpen) com.Write("%o\r");
                            if (tcp.Connected) tcp.SendString("%o\r");
                        }
                        else
                        {
                            readConfCount++;
                        }
                    }
                }                
                if (sonDeviceWriteConfig != null)
                {
                    string[] conflist = sonDeviceWriteConfig.Value.Write().Split(new char[1] { '\r' });
                    foreach (string s in conflist)
                    {                       
                        if (com.IsOpen) com.Write(s+"\r");
                        if (tcp.Connected) tcp.SendString(s+"\r");
                        Thread.Sleep(100);
                    }
                                     
                    sonDeviceReadConfig = null;
                    sonDeviceWriteConfig = null;
                }
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, DebugLevel.Red, e.Message);
            }
            
			#endregion
            
			if (tempSonarLine == null) tempSonarLine = new SonarLine(SonID);
			byte tag = 0x00;
			if (buf.Count > 0)
				tag = buf.Dequeue();

			try
			{
				if ((buf.Count >= 2) | (tag == 0x2F) | (tag == 0x31) | (tag == 0x41))
				{
					switch (tag)
					{
						case 0x04:
							#region HF Line
							reserved1 = buf.Dequeue();
							reserved2 = buf.Dequeue();

							if ((tempSonarLine.HF.Entries.Count > 0) | (tempSonarLine.NF.Entries.Count > 0))
							{
								SendSonarLine(tempSonarLine);
								tempSonarLine = new SonarLine(SonID);
							}
							tempSonarLine.dataHF = DecodeOldSonarLineData(buf);							
							break;
							#endregion
						case 0x08:
							#region NF Line
							reserved1 = buf.Dequeue();
							reserved2 = buf.Dequeue();

							if (tempSonarLine.NF.Entries.Count > 0)
							{
								SendSonarLine(tempSonarLine);
								tempSonarLine = new SonarLine(SonID);
							}
							tempSonarLine.dataNF = DecodeOldSonarLineData(buf);
							break;
							#endregion
						case 0x11: 
							#region New CalcDepth
							if (buf.Count > 0)
							{
								int pg = buf.Dequeue() == 1 ? 256 : 0;
								switch (buf.Count)
								{
									case 0:
										calcDepth = DecodeDepth((255 + pg) * 8);
										break;
									case 1:
										calcDepth = DecodeDepth((buf.Dequeue() + pg) * 8);
										break;
									default:
										DebugClass.SendDebugLine(this, DebugLevel.Red, "Incorrect Calculated Depth");
										break;
								}
							}
							else
							{
								DebugClass.SendDebugLine(this, DebugLevel.Red, "Incorrect Calculated Depth");
							}
							break;
							#endregion
						case 0x23: //Pos Line type 35d -> cDepth      
							#region Old calcdeep
							//string content = Encoding.ASCII.GetString(buf.ToArray());
							//tempSonarLine.posList.Add(new PositionData(33, content));
							break;
							#endregion
						case 0x24: //Pos Line type 32d -> GPS
							#region NMEA / GPS
							content = "$" + Encoding.ASCII.GetString(buf.ToArray());
							content = content.Replace("\n", "").Replace("\r", "");
							tempSonarLine.posList.Add(new PositionData(4, content));
							break;
							#endregion
						case 0x25:
							#region SonarConfig
                            Encoding t = Encoding.GetEncoding("ibm850");
                            byte[] bu = buf.ToArray();
                            
                            //Stream sw = File.Create(@"C:\log.txt");
                            //BinaryWriter bw = new BinaryWriter(sw);
                            //bw.Write(bu);
                            //bw.Close();                            
                            //Console.WriteLine(content);

                            content = t.GetString(bu);
                            content = content.Replace("\r", "");                           
                            try
                            {
                                sonDeviceReadConfig = new SonarDeviceConfig(content);
                            }
                            catch
                            {
                                sonDeviceReadConfig = null;
                            }
							break;
							#endregion
						case 0x26: //Pos Line type 04d -> Compass
							#region Compass
							content = Encoding.ASCII.GetString(buf.ToArray());
							if (content.Length >= 14)
							{
								content = content.Substring(0, 14);
								tempSonarLine.posList.Add(new PositionData(32, content));
							}
							else
								DebugClass.SendDebugLine(this, DebugLevel.Yellow, string.Format("Tag 0x26 too short {0} instead of 14 bytes", content.Length));
							break;
							#endregion
						case 0x2F:
							#region NewFormat EndTag
							// EndTag new LineFormat
							break;
							#endregion
						case 0x31:
							#region NewFormat First Page
							if ((tempSonarLine.dataHF.Entries.Count > 0) | (tempSonarLine.dataNF.Entries.Count > 0))
							{
								SendSonarLine(tempSonarLine);
								tempSonarLine = new SonarLine(SonID);
							}
							DecodeNewSonarLineData(tempSonarLine, buf, 0);
							break;
							#endregion
						case 0x41:
                            #region NewFormat Second Page
                            DecodeNewSonarLineData(tempSonarLine, buf, 1);

							SendSonarLine(tempSonarLine);
							tempSonarLine = new SonarLine(SonID);
							break;
							#endregion
						default:
							//Unknown Tag -> should not happen    
							DebugClass.SendDebugLine(this, DebugLevel.Yellow, string.Format("Unknown Sonar Line Tag 0x{0:x2}", tag));
							break;
					}
				}
				else
				{ //SonarLine shorter than 3 bytes -> should not happen
					DebugClass.SendDebugLine(this, DebugLevel.Yellow, "SonarLine shorter than 3 bytes " + tag.ToString() + " " + buf.Count.ToString());
				}
			}
			catch (Exception e)
			{
				UKLib.Debug.DebugClass.SendDebugLine(this, DebugLevel.Red, e.Message);
			}
		}
		#endregion

		#region Decode New Sonar Line Data

		private void DecodeNewSonarLineByte(byte inb, int Depth, int[] LineBuffer)
		{
			int Probability = ((inb >> 3) & 0x03)*2+1;
			int ColorCode = ((inb >> 5) & 0x07) + 1;
			if (ColorCode == 8)
			{
				DebugClass.SendDebugLine(this, DebugLevel.Yellow, "Wrong Color Code: " + ColorCode.ToString());
				ColorCode = 7;
			}

			for (int i = (7 - Probability); i < 8; i++)
				LineBuffer[(Depth) * 8 + i] = ColorCode;
		}

		private void DecodeNewSonarLineData(SonarLine line, Queue<byte> InBuffer, int page)
		{
			#region Initialization
			int[] LineBufferHF = new int[8 * 513];
			for (int i = 0; i < LineBufferHF.Length; i++)
				LineBufferHF[i] = 0;

			int[] LineBufferNF = new int[8 * 513];
			for (int i = 0; i < LineBufferNF.Length; i++)
				LineBufferNF[i] = 0;
			#endregion

			#region Sorting in Sonar Echoes
			// int LastDepth = 0;
			int DepthOffs = 0;
			if (page == 1) DepthOffs = 255;
            try
            {
                int Depth = DepthOffs + InBuffer.Dequeue();
                while (InBuffer.Count >= 1)
                {
                    byte FirstByte = InBuffer.Dequeue();
                    switch (FirstByte & 0x02)
                    {
                        case 0x00:
                            DecodeNewSonarLineByte(FirstByte, Depth, LineBufferNF);
                            
                            if ((FirstByte & 0x04) == 0x04)
                                Depth++;
                            else
                                Depth = DepthOffs + InBuffer.Dequeue();

                            break;

                        case 0x02:
                            DecodeNewSonarLineByte(FirstByte, Depth, LineBufferHF);

                            if ((FirstByte & 0x01) == 0x01) // NF Byte folgt
                                break;

                            if ((FirstByte & 0x04) == 0x04) // kein NF Byte, folgende Tiefe?
                                Depth++;
                            else
                                Depth = DepthOffs + InBuffer.Dequeue();
                                
                            break;
                    }
                }
            }
            catch
            {
                Debug.WriteLine("Try Catch Error DecodeNewSonarLineData" + page.ToString());
            }
			#endregion

			LengthEncodeData(LineBufferHF, tempSonarLine.dataHF.Entries, 0.0F);
			LengthEncodeData(LineBufferNF, tempSonarLine.dataNF.Entries, 0.0F);

			tempSonarLine.dataHF.cDepth = calcDepth; //Will be changed in Future
			tempSonarLine.dataNF.cDepth = calcDepth; //Will be changed in Future
			calcDepth = float.NaN;

			InBuffer.Clear(); //Just to be safe :-)
		}
		#endregion

		#region Decode Old Sonar Line Data
		/// <summary>
		/// Decodes an "old style" sonar line
		/// </summary>
		/// <param name="InBuffer">The input buffer containing Byte couples</param>
		/// <returns></returns>
		private LineData DecodeOldSonarLineData(Queue<byte> InBuffer)
		{
			#region Initialization
			int[] LineBuffer = new int[8 * 513];
			for (int i = 0; i < LineBuffer.Length; i++)
				LineBuffer[i] = 0;

			#region Old Byte couple Sonar format
			/*sz = record  
             * kopf:tsk;  
             *t:array[0..512,0..1] of byte;  
             *  {b0.0-b0.7 Tiefe}  
             *  {b1.0 Tiefenerweiterung}  
             * {b1.1 Cut or not cut ?}  
             * {b1.2-b1.4 H„ufigkeit}  
             * {b1.5-b1.7 Farbe}  
             */
			#endregion
			#endregion

			#region Sorting in Sonar Echoes
			int LastDepth = 0;
			while (InBuffer.Count >= 2)
			{
				byte FirstByte = InBuffer.Dequeue();
				byte SecondByte = InBuffer.Dequeue();
				int Depth = (((SecondByte & 0x01) == 1) ? 255 : 0) + FirstByte;
				//  bool DoCut = (SecondByte & 0x02) != 0;
				int Probability = ((SecondByte >> 2) & 0x07);
				int ColorCode = ((SecondByte >> 5) & 0x07) + 1;
				if (ColorCode == 8)
				{
					DebugClass.SendDebugLine(this, DebugLevel.Yellow, "Wrong Color Code: " + ColorCode.ToString());
					ColorCode = 7;
				}
				if (Depth >= LastDepth)
				{
					for (int i = (7 - Probability); i < 8; i++)
						LineBuffer[(Depth) * 8 + i] = ColorCode;
					LastDepth = Depth;
				}
				else
				{   //Wrong depth order. Depths have to be decreasing
					DebugClass.SendDebugLine(this, DebugLevel.Yellow, "Wrong depth order: got  " + Depth.ToString() + " after " + LastDepth.ToString() + ".");
					InBuffer.Clear();
				}
			}
			#endregion

			LineData Line = new LineData();
			Line.Entries = new Queue<DataEntry>();

			LengthEncodeData(LineBuffer, Line.Entries, +0.1F);

			Line.cDepth = float.NaN; //Will be changed in Future

			InBuffer.Clear(); //Just to be safe :-)
			return (Line);
		}
		#endregion

		#region Length Encode of Sonar Data
		/// <summary>
		/// Length Encoding of the Sonar Echoes
		/// </summary>
		/// <param name="LineBuffer">Input Buffer</param>
		/// <param name="Entries">Length Encoded Sonard Entries</param>
		private void LengthEncodeData(int[] LineBuffer, Queue<DataEntry> Entries, float offs)
		{
			int OldColor = 0;
			int StartDepth = 0;
			for (int i = 0; i < LineBuffer.Length; i++)
			{
				int CurrentColor = LineBuffer[i];
				int CurrentDepth = i;

				if (CurrentColor != 0)
				{
					if (OldColor == 0)
					{
						OldColor = CurrentColor;
						StartDepth = CurrentDepth;
					}
					else
					{
						if (OldColor != CurrentColor)
						{
							DataEntry Entry;
							int EndDepth = CurrentDepth;
							float StartDepthDecoded = DecodeDepth(StartDepth) + offs;
							float EndDepthDecoded = DecodeDepth(EndDepth) + offs;

							Entry.low = StartDepthDecoded;
							Entry.high = EndDepthDecoded;
							Entry.colorID = OldColor;
							Entries.Enqueue(Entry);

							OldColor = CurrentColor;
							StartDepth = CurrentDepth;
						}
					}
				}
				else //(CurrentColor==0) 
					if (OldColor != 0)
					{
						DataEntry Entry;
						int EndDepth = CurrentDepth;
						float StartDepthDecoded = DecodeDepth(StartDepth) + offs;
						float EndDepthDecoded = DecodeDepth(EndDepth) + offs;

						Entry.low = StartDepthDecoded;
						Entry.high = EndDepthDecoded;
						Entry.colorID = OldColor;
						Entries.Enqueue(Entry);

						OldColor = CurrentColor; //0
						//StartDepth = StartDepth; -> nonsense!
					}
			}
		}
		#endregion

		#region DeepEndoding
		/// <summary>
		/// Decodes non linear encoded depth
		/// </summary>
		/// <param name="Depth">non linear Depth (*8)</param>
		/// <returns>Depth in centimeters</returns>
		private float DecodeDepth(int Depth)
		{
			float result = 0;
			if (Depth <= 799)
				result = (Depth * 10 / 8F);
			else
				if (Depth <= 1399)
					result = (Depth * 5 / 2F - 1000);
				else
					if (Depth <= 2039)
						result = (Depth * 15 / 4F - 2750);
					else
						if (Depth <= 2855)
							result = (Depth * 25 / 4F - 7850);
						else
							if (Depth <= 4072)
								result = (Depth * 25 / 2F - 34700) * 10F;
							else
								DebugClass.SendDebugLine(this, DebugLevel.Yellow, "Illegal Depth: " + Depth.ToString());
			//Meters to centimeters
			return (result / 100F);
		}
		#endregion

        public override string ToString()
        {
            return "ID"+SonDevice.ID+":"+SonDevice.Port;
        }

	}
}
