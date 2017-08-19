using System;
using System.Collections.Generic;
using System.Text;

namespace sonOmeter.Server.Classes
{

    public delegate void SerialLogEventHandler(object sender, SerialLogEventArgs e);

    public class SerialLogEventArgs : EventArgs
    {
        string data;

        public string Data { get { return (data); } }
        public SerialLogEventArgs(string data)
        {
            this.data = data;
        }
    }

    public interface ISerialLog
    {
        event SerialLogEventHandler OnNewLogData;
        bool EnableLog { get; set;}
        string Port { get;}
    }
}
