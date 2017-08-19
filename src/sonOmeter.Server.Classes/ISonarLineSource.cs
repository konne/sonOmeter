using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO.Ports;
using System.Globalization;
using System.ComponentModel;


namespace sonOmeter.Server.Classes
{
    public interface ISonarLineSource
    {
        event SonarLineEventHandler OnNewSonarLine;
        string descriptiveXML { get;}
    }

    public delegate void SonarLineEventHandler(object sender, SonarLineEventArgs e);

    public class SonarLineEventArgs : EventArgs
    {
        SonarLine line;
        public SonarLine Line { get { return (line); } }
        public SonarLineEventArgs(SonarLine l)
        {
            line = l;
        }
    }
}
