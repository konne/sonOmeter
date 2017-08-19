using System;
namespace sonOmeter.Server.Classes
{
    interface ISerialSource
    {
        void Connect();
        void Disconnect();
    }
}
