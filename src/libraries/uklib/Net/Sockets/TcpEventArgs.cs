using System;
using System.Net.Sockets;

namespace UKLib.Net.Sockets
{
    #region TcpLineEventArgs
    /// <summary>
	/// This object is used for incoming lines.
	/// </summary>
	public class TcpLineEventArgs : EventArgs
	{
		#region Variables
		string s = "";
		DateTime time;
		#endregion

        #region Properties
        /// <summary>
		/// The transmitted line without the EOL tag.
		/// </summary>
		public string S
		{
			get { return s; }
		}

		/// <summary>
		/// The time stamp of the event.
		/// </summary>
		public DateTime Time
		{
			get { return time; }
		}
		#endregion

		public TcpLineEventArgs(string s)
		{
			this.s = s;
			this.time = DateTime.Now;
		}
    }
    #endregion

    #region TcpByteEventArgs
    /// <summary>
    /// This object is used for incoming bytes.
    /// </summary>
    public class TcpByteEventArgs : EventArgs
    {
        #region Variables
        byte b = 0;
        DateTime time;
        #endregion

        #region Properties
        /// <summary>
        /// The transmitted line without the EOL tag.
        /// </summary>
        public byte B
        {
            get { return b; }
        }

        /// <summary>
        /// The time stamp of the event.
        /// </summary>
        public DateTime Time
        {
            get { return time; }
        }
        #endregion

        public TcpByteEventArgs(byte b)
        {
            this.b = b;
            this.time = DateTime.Now;
        }
    }
    #endregion

    #region TcpClientThreadEventArgs
    public class TcpClientThreadEventArgs : EventArgs
    {

        private TcpClientThread client;
        public TcpClientThread Client
        {
            get { return client; }
        }
	
        public TcpClientThreadEventArgs(TcpClientThread client)
        {
            this.client = client;
        }
    }
    #endregion

    #region Delegates
    public delegate void TcpClientThreadEventHandler(object sender, TcpClientThreadEventArgs e);
	public delegate void TcpLineEventHandler(object sender, TcpLineEventArgs e);
    public delegate void TcpByteEventHandler(object sender, TcpByteEventArgs e);
    public delegate void TcpDataEventHandler(byte[] buf, int len);
    #endregion
}
