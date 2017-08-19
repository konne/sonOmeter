using System;

namespace sonOmeter.Server.Classes
{
    interface IPositionDataSink
    {
        void ConnectToSource(IPositionDataSource source);
    }
}

