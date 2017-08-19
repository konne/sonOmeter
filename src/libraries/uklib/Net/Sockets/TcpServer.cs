using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;

namespace UKLib.Net.Sockets
{
	/// <summary>
	/// This class provides a TCP Server with event handlers for incoming and outgoing messages.
	/// The network stream is handles automatically via an underlying thread. The Clientthreads
	/// created and destroyed automaticaliy by the Server & Reclaim Thread
	/// </summary>
	public class TcpServerThread
	{
		#region Variables
		private bool continueServer = false;
		private bool continueReclaim = false;
		private ArrayList Clients = new ArrayList();
		private TcpListener listener;
		private int nrClients = 0;

		private Thread reclaimThread;
		private Thread serverThread;

		private int port = 1000;
		private int maxNrClients = 2;

		private ISynchronizeInvoke synchronizingObject;
		#endregion

		#region Event
		public event TcpClientThreadEventHandler Connect;
		#endregion

		#region Properties

		public ISynchronizeInvoke SynchronizingObject
		{
			get { return synchronizingObject; }
			set
			{
				synchronizingObject = value;
			}
		}

		public int MaxNrClients
		{
			get { return maxNrClients; }
			set { value = maxNrClients; }
		}

		public bool Alive
		{
			get { return continueServer; }
		}

		public int Port
		{
			get { return port; }
			set
			{
				if ((serverThread != null) && (!serverThread.IsAlive))
					port = value;
			}
		}
		#endregion

		#region Constructor
		public TcpServerThread()
		{
		}
		#endregion

		#region Functions
		public void SendString(string data)
		{
			lock (Clients.SyncRoot)
			{
				for (int i = Clients.Count - 1; i >= 0; i--)
				{
					TcpClientThread Client = Clients[i] as TcpClientThread;
					Client.SendString(data);
				}
			}
		}
		#endregion

		#region Start / Stop
		private void start(IPAddress ipaddr)
		{
			continueReclaim = true;
			continueServer = true;
			listener = new TcpListener(ipaddr, port);
			listener.Start();

			serverThread = new Thread(new ThreadStart(threadServer));
			serverThread.Name = "TCPServer";
			serverThread.Start();
			reclaimThread = new Thread(new ThreadStart(threadReclaim));
			reclaimThread.Start();
		}

		public void Start()
		{
			if (serverThread == null || !serverThread.IsAlive)
				start(IPAddress.Any);
		}

		public void Start(int port)
		{
			if (serverThread == null || !serverThread.IsAlive)
			{
				this.port = port;
				start(IPAddress.Any);
			}
		}

		public void Start(IPAddress ipaddr, int port)
		{
			if (serverThread == null || !serverThread.IsAlive)
			{
				this.port = port;
				start(ipaddr);
			}
		}

		public void Stop()
		{
			try
			{
				continueServer = false;
				continueReclaim = false;                
				listener.Stop();
				if (synchronizingObject != null && serverThread.IsAlive)
					serverThread.Join();
				if (synchronizingObject != null && reclaimThread.IsAlive)
					reclaimThread.Join();

				foreach (TcpClientThread Client in Clients)
				{
					Client.Close();
				}
			}
			catch (Exception ex)
			{
                Console.WriteLine("Server Stop: " + ex.Message);
			}
			Connect = null;
			serverThread = null;
			reclaimThread = null;
		}
		#endregion

		#region Threads
		private void threadServer()
		{
			while (continueServer)
			{
				while ((nrClients < maxNrClients) && (continueServer))
				{                    
					try
					{						
                        if ((listener.Pending()))
                        {
                            Socket socket = listener.AcceptSocket();

                            if (socket != null)
                            {
                                TcpClientThread client = new TcpClientThread();
                                client.SynchronizingObject = synchronizingObject;
                                client.Connect(socket);
                                if (Connect != null)
                                {
                                    TcpClientThreadEventArgs args = new TcpClientThreadEventArgs(client);
                                    if (synchronizingObject != null && synchronizingObject.InvokeRequired)
                                    {
                                        synchronizingObject.BeginInvoke(Connect, new object[2] { this, args });
                                    }
                                    else
                                    {
                                        Connect(this, args);
                                    }
                                }

                                lock (Clients.SyncRoot)
                                {
                                    Clients.Add(client);
                                }
                                nrClients++;
                            }
                            else
                            {
                                break;
                            }
                        }
					}
					catch
					{                        
						continueServer = false;
						break;
					}
					Thread.Sleep(100);
				}
				Thread.Sleep(100);
			}            
		}

		private void threadReclaim()
		{
			while (continueReclaim)
			{
				try
				{
					lock (Clients.SyncRoot)
					{
						for (int i = Clients.Count - 1; i >= 0; i--)
						{
							TcpClientThread Client = Clients[i] as TcpClientThread;
							if (!Client.Running)
							{
								Clients.Remove(Client);
								nrClients--;
							}
						}
					}
					Thread.Sleep(100);
				}
				catch
				{
					continueReclaim = false;
				}
			}
		}
		#endregion

	}
}
