using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;

//using System.Runtime.CompilerServices; chg: not necessary?

namespace UKLib.Net.Sockets
{
	/// <summary>
	/// This class provides a simple TCP client with event handlers for incoming and outgoing messages.
	/// The network stream is handled automatically via an underlying thread.
	/// </summary>
    public class TcpClientThread
    {
        #region Variables
        Thread thread;
      
        string line = "";      
        bool running = false;
            
        bool tryReConnect = false;
        bool firstConnect = false;
       
        StringCollection txSBuf = new StringCollection();

        Queue<byte> txPipe = new Queue<byte>();

        Socket sck = null;
        IPEndPoint ipe = null;
        #endregion

        #region Properties
        public ISynchronizeInvoke SynchronizingObject { get; set; }

        public bool TryReConnect
        {
            get { return (tryReConnect | (ReconnectMaxIdleSeconds > 0)) & (ipe != null); }
            set
            {
                tryReConnect = value;
            }
        }

        /// <summary>
        /// Gets or sets the priority of the polling thread.
        /// Careless use of this feature may cause unwanted effects on other applications!
        /// </summary>
        public ThreadPriority Priority
        {
            get { return thread.Priority; }
            set { thread.Priority = value; }
        }

        /// <summary>
        /// Gets or sets the EOL tag.
        /// </summary>
        public string EOLTag { get; set; }        

        /// <summary>
        /// Gets the connection state of the client.
        /// </summary>
        public bool Connected
        {
            get { return running; }
        }

        /// <summary>
        /// Gets the state of the underlying thread.
        /// </summary>
        public bool Running
        {
            get { return thread.IsAlive; }
        }

        /// <summary>
        /// Gets or sets the tag that is sent on a disconnect call. Set string to empty, if this feature is not used.
        /// </summary>
        public string DisconnectTag { get; set; }

        /// <summary>
        /// Gets or sets the start connection timeout
        /// </summary>
        public int ConnectTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the max idle time in seconds before try reconnect (disable value <= 0)
        /// </summary>
        public int ReconnectMaxIdleSeconds { get; set; }

        #endregion

        #region Events
        public event TcpLineEventHandler NewLine;
        public event TcpByteEventHandler NewByte;
        public event TcpDataEventHandler NewData;
        public event EventHandler<EventArgs> OnConnect;
        public event EventHandler<EventArgs> OnDisConnect;
        
        #endregion

        #region Constructors
        void Init()
        {
            EOLTag = "\r\n";
            DisconnectTag = "";
            thread = new Thread(new ThreadStart(ThreadProc));
            thread.IsBackground = true;
            thread.Name = "tcpClientThread";

            ReconnectMaxIdleSeconds = 0;
            ConnectTimeOut = 500;
        }

        /// <summary>
        /// Standard constructor.
        /// </summary>
        public TcpClientThread()
        {
            Init();
        }       

        public TcpClientThread(Socket socket)
        {
            Init();
            Connect(socket);
        }
        #endregion

        #region TCP functions
        /// <summary>
        /// Opens a connection to the given TCP server.
        /// </summary>
        /// <param name="hostname">The host name.</param>
        /// <param name="port">The port of the server.</param>
        public void Connect(string hostname, int port)
        {
            try
            {               
                firstConnect = true;                          

                IPAddress[] addresses = Dns.GetHostAddresses(hostname);
                IPAddress add = null;

                foreach (IPAddress ip in addresses)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        add = ip;
                        break;
                    }
                }

                if (add != null)
                {
                    ipe = new IPEndPoint(add, port);
                  
                    thread.Start();
                }
            }
            catch
            {                                
            }
        }

        public void Connect(Socket socket)
        {
            sck = socket;
            SetSocketDefaults(sck);
            thread.Start();            
        }

        private void SetSocketDefaults(Socket socket)
        {
            socket.ReceiveBufferSize = 65536;
            socket.SendBufferSize = 65536;
            socket.ReceiveTimeout = 200;
            socket.SendTimeout = 200;
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {           
            try
            {
                if (DisconnectTag != "")
                {      
                    if (sck != null)
                        sck.Send(System.Text.Encoding.Default.GetBytes(DisconnectTag));           
                }
                running = false;

                if (SynchronizingObject != null && thread.IsAlive)
                    thread.Join();
            }
            catch
            {
            }
        }
        #endregion
      
        #region Thread functions
        /// <summary>
        /// The main thread procedure.
        /// </summary>        
        void ThreadProc()
        {                   
            running = true;

            int bufferSize = 4096*2;

            int rxCount = 0;
            byte[] rxbuf = new byte[bufferSize];
            SocketError sckError = SocketError.Success;
          
            byte[] txbuf = null;

            DateTime refTime = DateTime.Now;

            while (running)
            {
                #region Fill TXBuffer
                lock (txSBuf)
                {
                    if (txSBuf.Count > 0)
                    {
                        txbuf = System.Text.Encoding.Default.GetBytes(txSBuf[0]);
                        txSBuf.RemoveAt(0);
                    }
                }
                lock (txPipe)
                {
                    if ((txbuf == null) & (txPipe.Count > 0))
                    {
                        txbuf = txPipe.ToArray();
                        txPipe.Clear();                       
                    }
                }
                #endregion

                #region Send / Receive Data
                try
                {
                    rxCount = 0;
                    sckError = SocketError.NotConnected;
                    if (sck != null)
                    {                        
                        sckError = SocketError.Success;
                        if (txbuf != null)
                        {
                            int txCount = sck.Send(txbuf, 0, txbuf.Length, SocketFlags.None, out sckError);
                            txbuf = null;
                            // reset refTime for ReconnectMaxIdleSeconds
                            if (sckError == SocketError.Success) refTime = DateTime.Now;
                        }                        
                        if ((sck.Available > 0) & (sckError == SocketError.Success))
                        {                                            
                            rxCount = sck.Receive(rxbuf, 0, bufferSize, SocketFlags.None, out sckError);
                            // reset refTime for ReconnectMaxIdleSeconds
                            if (sckError == SocketError.Success) refTime = DateTime.Now;
                        }
                    }                                   
                }
                catch
                {
                }
                #endregion

                #region (Re)/Connect
                // do a reconnect than no data since MaxIdleSeconds
                if ((ReconnectMaxIdleSeconds > 0) & ((DateTime.Now-refTime).TotalSeconds > ReconnectMaxIdleSeconds))
                {
                    sckError = SocketError.TimedOut;
                }

                // if socket null or any error try reconnect
                if ((sck == null) || (sckError != SocketError.Success))
                {

                    // if socket available try to disconnect and close
                    if (sck != null)
                    {                        
                        try
                        {
                            sck.Disconnect(false);
                            sck.Shutdown(SocketShutdown.Both);
                            sck.Close();                           
                            
                        }
                        catch
                        {
                        }
                        if (OnDisConnect != null)
                        {
                            if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
                            {
                                this.SynchronizingObject.BeginInvoke(OnDisConnect,
                                    new object[2] { this, EventArgs.Empty });
                            }
                            else
                            {
                                OnDisConnect(this, EventArgs.Empty);
                            }
                        }

                        sck = null;
                        Thread.Sleep(1000);
                    }

                    // try make a new connection
                    try
                    {
                        if (TryReConnect | firstConnect) 
                        {
                            firstConnect = false;
                            sck = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                            SetSocketDefaults(sck);

                            IAsyncResult iar = sck.BeginConnect(ipe, null, null);
                            bool succ = iar.AsyncWaitHandle.WaitOne(ConnectTimeOut, true);                           
                            if (succ)
                            {
                                if (OnConnect != null)
                                {
                                    if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
                                    {
                                        this.SynchronizingObject.BeginInvoke(OnConnect,
                                            new object[2] { this, EventArgs.Empty });
                                    }
                                    else
                                    {
                                        OnConnect(this, EventArgs.Empty);
                                    }
                                }
                                sck.EndConnect(iar);                                
                            }
                            else
                            {
                                // Connection no sucsessful wait 1 second and try again
                                iar.AsyncWaitHandle.Close();
                                sck.Close();
                                sck = null;
                                Thread.Sleep(1000);
                            }
                            refTime = DateTime.Now;
                        }
                        else
                            running = false;
                    }
                    catch
                    {                        
                    }

                }
                #endregion

                #region Call NewData Events
                try
                {                   
                    if (rxCount > 0)
                    {                       
                        if (NewByte != null)
                        {
                            for (int i = 0; i < rxCount; i++)
                            {
                                byte b = rxbuf[i];
                                TcpByteEventArgs args = new TcpByteEventArgs(b);

                                if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
                                {
                                    this.SynchronizingObject.BeginInvoke(NewByte,
                                        new object[2] { this, args });
                                }
                                else
                                {
                                    NewByte(this, args);
                                }
                            }
                        }
                        if (NewData != null)
                        {
                            if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
                            {
                                this.SynchronizingObject.BeginInvoke(NewData,
                                    new object[2] { rxbuf, rxCount });
                            }
                            else
                            {
                                IAsyncResult asr = NewData.BeginInvoke(rxbuf, rxCount, null, null);
                                Thread.Sleep(1);
                                NewData.EndInvoke(asr);
                            }

                        }

                        if (NewLine != null)
                        {
                            line += System.Text.Encoding.Default.GetString(rxbuf, 0, rxCount);
                            
                            string s = "";
                            int count = line.IndexOf(EOLTag, 0);
                            while (count > -1)
                            {
                                s = line.Substring(0, count);

                                if (s.Length > 0)
                                {
                                    TcpLineEventArgs args = new TcpLineEventArgs(s);

                                    if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
                                    {
                                        this.SynchronizingObject.BeginInvoke(NewLine,
                                            new object[2] { this, args });
                                    }
                                    else
                                    {
                                        NewLine(this, args);
                                    }
                                }

                                line = line.Substring(count + EOLTag.Length);

                                count = line.IndexOf(EOLTag, 0);
                            }
                        }
                    }

                    if (sck != null)
                    {                        
                       
                    }

                }
                catch (Exception e)
                {
                    //  UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, e.Message);
                    Console.WriteLine(ipe + " " + e.Message);
                    Thread.Sleep(1000);
                    if (!TryReConnect)
                        running = false;
                }
                #endregion

                Thread.Sleep(1); //Reduce CPU Load down to almost nothing
            }
            if (sck != null)
            {
                try
                {
                    sck.Disconnect(false);
                    sck.Shutdown(SocketShutdown.Both);
                    sck.Close();
                    sck = null;
                }
                catch
                {
                }
            }
            Console.WriteLine(ipe + " finnished ");
        }
        #endregion

        #region TX functions
        public void SendString(string s)
        {
            lock (txSBuf)
            {
                if (txSBuf.Count < 8000)
                    txSBuf.Add(s);
            }
        }

        public void SendByte(byte b)
        {
            lock (txPipe)
            {
                if (txPipe.Count < 8000)
                    txPipe.Enqueue(b);
            }
        }

        public void SendBytes(byte[] buf)
        {
            lock (txPipe)
            {
                for (int i = 0; i < buf.Length; i++)
                    txPipe.Enqueue(buf[i]);
            }
        }
        #endregion
    }
}
