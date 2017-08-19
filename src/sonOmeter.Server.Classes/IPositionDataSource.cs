using System;
using System.Collections.Generic;
using System.Text;

namespace sonOmeter.Server.Classes
{
    public interface IPositionDataSource
    {
        event PositionDataEventHandler OnNewPositionData;
        string descriptiveXML { get;}
    }

    public delegate void PositionDataEventHandler(object sender, PositionDataEventArgs e);

    public class PositionDataEventArgs : EventArgs
    {
        PositionData data;
        public PositionData Data { get { return (data); } }
        public PositionDataEventArgs(PositionData d)
        {
            data = d;
        }
    }
}
