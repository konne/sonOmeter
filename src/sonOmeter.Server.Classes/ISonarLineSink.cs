using System;
namespace sonOmeter.Server.Classes
{
    interface ISonarLineSink
    {
        void ConnectToSource(ISonarLineSource source);
    }
}
