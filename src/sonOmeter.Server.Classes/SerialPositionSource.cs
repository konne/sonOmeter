using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using UKLib.Debug;
using UKLib.Net.Sockets;
using UKLib.Arrays;
using UKLib.Hex;
using System.Text.RegularExpressions;
using UKLib.Survey.Parse;

namespace sonOmeter.Server.Classes
{
    class SerialPositionSource : IPositionDataSource, ISerialSource, ISerialLog
    {
        #region Variables
        SerialPort com;
        TcpClientThread tcp;
        Queue<byte> InputBuffer = new Queue<byte>();
        private char SeparationCharacter = '\r';
        private PositionDevice PosDevice = new PositionDevice();
        #endregion

      
       
        #region IPositionDataSource Members

        public event PositionDataEventHandler OnNewPositionData;

        public string descriptiveXML
        {
            get { return (""); }
        }

        #endregion


        public void InitDLSB30()
        {
            if (PosDevice.Format == PositionDeviceFormat.DLSB30)
            {
                string buf = "s0uh\r\n";
                if (com.IsOpen) com.Write(buf);
                if (tcp.Connected) tcp.SendString(buf);
            }
        }

        #region ISerialSource Members

        public void Connect()
        {
            if (com.IsOpen) com.Close();
            if (tcp.Connected) tcp.Close();

            if (PosDevice.Port.IndexOf("COM") != 0)
            {
                tcp.Connect(PosDevice.Port, PosDevice.Parameter);
            }
            else
            {
                com.PortName = PosDevice.Port;
                com.BaudRate = PosDevice.Parameter;
                com.Handshake = Handshake.None;
                com.Open();
            }           
          
            InputBuffer.Clear();
        }

        public void Disconnect()
        {
            tcp.Close();
            com.Close();
            InputBuffer.Clear();
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
            get { return PosDevice.Port; }
        }

        #endregion

        #region Constructor
        public SerialPositionSource(PositionDevice posDevice)
        {
            PosDevice = (PositionDevice)posDevice.Clone();
            com = new SerialPort();
            com.ReceivedBytesThreshold = 3;
            com.DataReceived += new SerialDataReceivedEventHandler(com_DataReceived);
            tcp = new TcpClientThread();
            tcp.TryReConnect = true;
            tcp.NewData += new TcpDataEventHandler(tcp_NewData);          
        }      
        #endregion

        #region Events

        void tcp_NewData(byte[] buf, int len)
        {
            try
            {
                InitSeperationChar();            
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
                InitSeperationChar();
               
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
                byte b = data[i];
                log += b.ToString("X");
                if ((b == SeparationCharacter) && (PosDevice.Format != PositionDeviceFormat.RD8000))
                {
                    ProcessLine(InputBuffer);
                    InputBuffer.Clear();
                    InputBuffer.Enqueue((byte)SeparationCharacter);
                }
                else
                {
                    if (InputBuffer.Count < 1500)
                        InputBuffer.Enqueue(b);
                    else
                        InputBuffer.Clear();
                }
            }

            #region RD8000 Parse to remove ppp header and send ACK
            if (PosDevice.Format == PositionDeviceFormat.RD8000)
            {
                byte[] bb = InputBuffer.ToArray();
                InputBuffer.Clear();            
                
                string buffer = HexStrings.ToHexString(bb);
               
                Regex regex = new Regex("7EFF030021[A-F,0-9]{100}7E");
                if (regex.IsMatch(buffer))
                {
                    MatchCollection mac = regex.Matches(buffer);
                    Match lastMa = null;
                    foreach (Match ma in mac)
                    {
                        SendPositionIfNeccesary(34, ma.Value.Substring(2,54*2));
                        
                        byte[] buf = RD8000Message.RD8000ACKMessage();
                        if (com.IsOpen) com.Write(buf,0,buf.Length);
                        if (tcp.Connected) tcp.SendBytes(buf);                       
                        lastMa = ma;
                    }
                    buffer = buffer.Substring(lastMa.Index + lastMa.Length);
                }

                int index = buffer.LastIndexOf("7E");
                if (index != -1)
                    buffer = buffer.Substring(index);
                else
                    buffer = "";
                
                for (int i = 0; i < buffer.Length / 2; i++)
                {
                    InputBuffer.Enqueue(Byte.Parse(buffer.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber));
                }

            }
            #endregion

            if (enabelLog && (log != "") && (OnNewLogData != null))
                OnNewLogData(this, new SerialLogEventArgs(log));
        }
        #endregion

        #region Functions
        private void InitSeperationChar()
        {
            switch (PosDevice.Format)
            {
                case PositionDeviceFormat.Geodimeter:
                    SeparationCharacter = '>';
                    break;              
                default:
                    SeparationCharacter = '\r';
                    break;
            }
        }

        private void ProcessLine(Queue<byte> InputBuffer)
        {
            string posline = Encoding.ASCII.GetString(InputBuffer.ToArray());                   

            if (posline.Length > 0)
            {
                try
                {
                    switch (PosDevice.Format)
                    {
                        case PositionDeviceFormat.Compass:
                            posline = posline.Remove(0, 1);
                            if (posline[0] == 'S')
                            {
                                // Kompassdaten
                                SendPositionIfNeccesary(32, posline.Remove(0, 1));
                            }
                            if (posline[0] == '!')
                            {
                                //Tiefendaten
                                SendPositionIfNeccesary(33, posline.Remove(0, 1));
                            }
                            break;
                            
                        case PositionDeviceFormat.HMR3300: // Honeywell Compass
                            posline = RemoveCRLF(posline);
                            SendPositionIfNeccesary(31, posline);
                            break;

                        case PositionDeviceFormat.Geodimeter:
                            posline = posline.Replace('\r', ';').Replace('\n', ';');
                            if (posline[0] == '>')
                                SendPositionIfNeccesary(5, posline.Remove(0, 1));
                            break;
                        case PositionDeviceFormat.NMEA:
                            posline = RemoveCRLF(posline);                            
                            if (posline[0] == '$')
                                SendPositionIfNeccesary(4, posline);
                            break;
                        case PositionDeviceFormat.LeicaTPS:
                            posline = RemoveCRLF(posline);                             
                            SendPositionIfNeccesary(7, posline);                         
                            break;
                        case PositionDeviceFormat.RD4000:
                            posline = RemoveCRLF(posline);
                            SendPositionIfNeccesary(35, posline);
                            break;

                        case PositionDeviceFormat.DLSB30:
                             posline = RemoveCRLF(posline);
                            SendPositionIfNeccesary(37, posline);
                            break;                          
                        default:
                            DebugClass.SendDebugLine(this, DebugLevel.Red, "Unknown Position Type declared");
                            break;
                    }
                }
                catch
                {
                    DebugClass.SendDebugLine(this, DebugLevel.Yellow,"Error in " + PosDevice.Format.ToString());
                }
            }
        }

        private void SendPositionIfNeccesary(int tag, string linecontent)
        {
            if (OnNewPositionData != null)
            {
                linecontent = RemoveWrongChars(linecontent);
                OnNewPositionData(this, new PositionDataEventArgs(new PositionData(tag, linecontent)));
            }                
        }

        private string RemoveWrongChars(string si)
        {
            string s = "";
            byte[] bi = Encoding.ASCII.GetBytes(si);
            for (int i = 0; i < si.Length; i++)
            {
                if ((si[i] != '<') & (si[i] != '>') & (bi[i] > 0x1F))
                {
                    s += si[i];
                }
            }
            return s;
        }

        /// <summary>
        /// Just Removes CR and LF out of strings
        /// </summary>
        /// <param name="linecontent">the dirty string</param>
        /// <returns>the clean string</returns>
        private static string RemoveCRLF(string linecontent)
        {
            linecontent = linecontent.Replace("\r", "").Replace("\n", "");
            return linecontent;
        }
        #endregion
    }
}
