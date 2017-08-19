using System;
using System.Collections.Generic;
using System.Text;

namespace sonOmeter.Server.Classes
{
    /// <summary>
    /// Position Data informing about GPS, Compass or anything else
    /// </summary>
    public struct PositionData
    { 
        #region Var / Properties
        internal DateTime timeStamp;
        public DateTime TimeStamp
        {
            get { return timeStamp; }
        }
        
        internal int type;
        public int Type
        {
            get { return type; }
        }

        internal string content;
        public string Content
        {
            get { return content; }
        }
        #endregion

        #region Constructor
        public PositionData(int type, byte[] content)
        {
            this.timeStamp = DateTime.Now;
            this.type = type;
            this.content = Encoding.ASCII.GetString(content);
        }

        public PositionData(int type, string content)
        {
            this.timeStamp = DateTime.Now;
            this.type = type;
            this.content = content;
        }

        public PositionData(DateTime timeStamp, int type, byte[] content)
        {
            this.timeStamp = timeStamp;
            this.type = type;
            this.content = Encoding.ASCII.GetString(content);
        }
        #endregion

        #region ToString()
        public override string ToString()
        {
            string res = "<pos time=\"" + this.TimeStamp.ToString("HH:mm:ss.fff") + "\" ";
            res += string.Format("type=\"{0}\"", this.Type);
            res += ">" + Content + "</pos>";
            return (res);
        }
        #endregion
    }
    
}
