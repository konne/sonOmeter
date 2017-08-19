using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Contronix.Classes
{
    public enum DebugLevel { Red, Yellow, Green,White }
 
    public class DebugLineEventArgs : EventArgs
    {	
        private string debugline;
        public string DebugLine
        {
            get { return debugline; }
        }
        private DebugLevel level;
        public DebugLevel Level
        {
            get { return level; }
        }
	
        internal DebugLineEventArgs(DebugLevel l,string s)
        {            
            level = l;
            debugline = s;
        }
    }

    public delegate void DebugLineEventHandler(object sender, DebugLineEventArgs e);

    public static class DebugClass
    {
        private static ISynchronizeInvoke synchronizingObject;
        public static ISynchronizeInvoke SynchronizingObject
        {
            get { return synchronizingObject; }
            set
            {
                synchronizingObject = value;
            }
        }

        public static event DebugLineEventHandler OnDebugLines;
       
        /// <summary>
        /// Sends a less or more important debug Line to the host application
        /// </summary>
        /// <param name="s">The message to be sent</param>
        internal static void SendDebugLine(object sender, DebugLevel level,string s)
        {
            if (OnDebugLines != null)
            {
                DebugLineEventArgs args = new DebugLineEventArgs(level,s);

                if (synchronizingObject != null && SynchronizingObject.InvokeRequired)
                {
                    SynchronizingObject.BeginInvoke(OnDebugLines,
                        new object[2] { sender,  args });
                }
                else
                {
                    OnDebugLines(sender, args);
                }
            }
        }

    }
}
