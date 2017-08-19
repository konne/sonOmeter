using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;

namespace UKLib.Net.Sockets
{
	/// <summary>
	/// This class provides a simple TCP client with event handlers for incoming and outgoing messages.
	/// The network stream is handles automatically via an underlying thread.
	/// </summary>
    [Obsolete("Use TcpClientThread instead", true)]
	public class TcpThread
	{
		#region Variables
		TcpClient client;
		Thread thread;

		string eolTag = "\r\n";
		string line = "";
		string disconnectTag = "";
		bool connected = false;

		StringCollection rxBuf = new StringCollection();
		StringCollection txBuf = new StringCollection();

        Queue<byte> txPipe=new Queue<byte>();

		private ISynchronizeInvoke synchronizingObject;
		#endregion

		#region Properties
		public ISynchronizeInvoke SynchronizingObject
		{
			get { return synchronizingObject;} 
			set 
			{
				synchronizingObject = value;
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
		public string EOLTag
		{
			get { return eolTag; }
			set { eolTag = value; }
		}

		/// <summary>
		/// Gets the connection state of the client.
		/// </summary>
		public bool Connected
		{
			get { return connected; }
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
		public string DisconnectTag
		{
			get { return disconnectTag; }
			set { disconnectTag = value; }
		}
		#endregion

		#region Events
		public event TcpLineEventHandler NewLine;
        public event TcpByteEventHandler NewByte;
		#endregion

		#region Constructors
       
		void Init()
		{		
			thread = new Thread(new ThreadStart(ThreadProc));
			thread.IsBackground = true;
			thread.Name = "tcpThread";
		}

		/// <summary>
		/// Standard constructor.
		/// </summary>
        
		public TcpThread()
		{
			Init();
		}

		/// <summary>
		/// Initialize and connect to server.
		/// </summary>
		/// <param name="hostname">The host name.</param>
		/// <param name="port">The port of the server.</param>
		public TcpThread(string hostname, int port)
		{
			try
			{
				Init();
				Connect(hostname, port);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}

		/// <summary>
		/// Initialize and connect to server.
		/// </summary>
		/// <param name="hostname">The host IP.</param>
		/// <param name="port">The port of the server.</param>
		/*public TcpThread(IPAddress address, int port)
		{
			try
			{
				Init();
				Connect(address, port);
			}
			catch (Exception e)
			{
				throw new Exception(e.Message);
			}
		}*/
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
				client = new TcpClient();
				client.Connect(hostname, port);
				connected = true;
			}
			catch (Exception e)
			{
				connected = false;
				throw new Exception("Connection failed with return code: " + e.Message);
			}
		}

		/// <summary>
		/// Opens a connection to the given TCP server.
		/// </summary>
		/// <param name="address">The host IP.</param>
		/// <param name="port">The port of the server.</param>
		/*public void Connect(IPAddress address, int port)
		{
			try
			{
				client = new TcpClient();
				client.Connect(address, port);
				connected = true;
			}
			catch (Exception e)
			{
				connected = false;
				throw new Exception("Connection failed with return code: " + e.Message);
			}
		}*/

		/// <summary>
		/// Closes the connection.
		/// </summary>
		public void Close()
		{
			connected = false;

			try
			{
				if (disconnectTag != "")
				{
					char[] cArray = disconnectTag.ToCharArray();

					foreach (char c in cArray)
					{
						client.GetStream().WriteByte((byte)c);
					}
				}

				client.Close();
			}
			catch
			{
			}
		}
		#endregion

		#region Thread functions
		/// <summary>
		/// Starts or resumes the thread.
		/// </summary>
		public void Start()
		{
			if (thread.IsAlive)
				thread.Resume();
			else
				thread.Start();
		}

		/// <summary>
		/// Stops the thread.
		/// </summary>
		public void Stop()
		{
			thread.Suspend();
		}

		/// <summary>
		/// The main thread procedure.
		/// </summary>
        void ThreadProc()
        {
            while (connected)
            {
                NetworkStream stream = client.GetStream();

                if (stream.DataAvailable)
                {
                    byte[] buf = new byte[256];
                    int n = stream.Read(buf, 0, buf.Length);
                    line += System.Text.Encoding.Default.GetString(buf, 0, n);

                    if (NewByte != null) 
                    {
                        for (int i = 0; i < n; i++)
                        {
                            byte b = buf[i];
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
                    if (NewLine != null)
                    {
                        string s = "";
                        n = line.IndexOf(eolTag, 0);
                        while (n > -1)
                        {
                            s = line.Substring(0, n);

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

                            line = line.Substring(n + eolTag.Length);

                            n = line.IndexOf(eolTag, 0);
                        }
                    }
                }

				if (txBuf.Count > 0)
				{
					byte[] buf = System.Text.Encoding.Default.GetBytes(txBuf[0]);
					stream.Write(buf, 0, buf.Length);
					txBuf.RemoveAt(0);
				}

                if (txPipe.Count > 0)
                {
                    stream.WriteByte(txPipe.Dequeue());
                }

                Thread.Sleep(1); //Reduce CPU Load down to almost nothing
            }
        }
		#endregion

		#region RX/TX functions
		public void SendString(string s)
		{
			txBuf.Add(s);
		}

        public void SendByte(byte b)
        {
            txPipe.Enqueue(b);
        }
		#endregion
	}
}
