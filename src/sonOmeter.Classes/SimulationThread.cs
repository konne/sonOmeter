using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections.Specialized;
using System.ComponentModel;
using UKLib.Net.Sockets;
using System.IO;
using System.Xml;

namespace sonOmeter.Classes
{
    public class SimulationThread
    {
        #region Variables
        Thread thread;

        XmlTextReader reader = null;
        string line = "";
        string deviceList = "";
        bool running = false;

        SortedList<DateTime, string> entries = new SortedList<DateTime, string>();
        #endregion

        #region Properties
        public ISynchronizeInvoke SynchronizingObject { get; set; }

        /// <summary>
        /// Gets or sets the priority of the polling thread.
        /// Careless use of this feature may cause unwanted effects on other applications!
        /// </summary>
        public ThreadPriority Priority
        {
            get { return thread.Priority; }
            set { thread.Priority = value; }
        }

        /// <summary>
        /// Gets the connection state of the client.
        /// </summary>
        public bool Connected
        {
            get { return running; }
        }

        /// <summary>
        /// Gets the state of the underlying thread.
        /// </summary>
        public bool Running
        {
            get { return thread.IsAlive; }
        }
        #endregion

        #region Events
        public event TcpLineEventHandler NewLine;
        #endregion

        #region Constructors
        /// <summary>
        /// Standard constructor.
        /// </summary>
        public SimulationThread()
        {
            thread = new Thread(new ThreadStart(ThreadProc));
            thread.IsBackground = true;
            thread.Name = "simulationThread";
        }
        #endregion

        #region Open/Close functions
        /// <summary>
        /// Opens a connection to the given TCP server.
        /// </summary>
        /// <param name="hostname">The source file name.</param>
        public void Open(string filename)
        {
            try
            {
                if (!File.Exists(filename))
                    return;

                reader = new XmlTextReader(filename);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                string att = "";
                int sonid = 0;
                DateTime time = DateTime.Now;
                DateTime refTime = DateTime.Now;

                reader.Read();
                while (!reader.EOF)
                {

                    if (reader.NodeType != XmlNodeType.Element)
                    {
                        reader.Read();
                        continue;
                    }

                    switch (reader.Name)
                    {
                        case "record":
                            att = reader.GetAttribute("date");
                            if (att != null)
                                DateTime.TryParse(att, out refTime);
                            reader.Read();
                            break;

                        case "devicelist":
                            deviceList = reader.ReadOuterXml();
                            break;

                        case "line":
                            att = reader.GetAttribute("sonid");
                            if (att != null)
                                int.TryParse(att, out sonid);
                            att = reader.GetAttribute("time");
                            if ((att != null) && DateTime.TryParse(att, out time))
                                time = refTime.Add(time.TimeOfDay);

                            line = "<line sonid=\"" + sonid + "\" time=\"" + time + "\">" + reader.ReadInnerXml() + "</line>";
                            
                            while (entries.ContainsKey(time))
                                time = time.AddMilliseconds(1);
                            entries.Add(time, line);
                            break;

                        case "manualpoint":
                            att = reader.GetAttribute("date");
                            if (att != null)
                                DateTime.TryParse(att, out refTime);
                            att = reader.GetAttribute("time");
                            if ((att != null) && DateTime.TryParse(att, out time))
                                time = refTime.Add(time.TimeOfDay);

                            while (entries.ContainsKey(time))
                                time = time.AddMilliseconds(1);
                            entries.Add(time, reader.ReadOuterXml());
                            break;

                        default:
                            reader.Read();
                            continue;
                    }
                }


                thread.Start();
            }
            catch
            {
            }
        }

        /// <summary>
        /// Closes the connection.
        /// </summary>
        public void Close()
        {
            try
            {
                running = false;

                if (SynchronizingObject != null && thread.IsAlive)
                    thread.Join();
            }
            catch
            {
            }
        }
        #endregion

        /// <summary>
        /// The main thread procedure.
        /// </summary>        
        void ThreadProc()
        {
            running = true;
            bool firstRun = true;
            int index = 0;
            DateTime time = DateTime.Now;
            DateTime nextTime = DateTime.Now;

            while (running)
            {
                try
                {
                    if (firstRun)
                        line = deviceList;
                    else
                    {
                        line = entries.Values[index];
                        time = entries.Keys[index];
                        index++;

                        if (index <= entries.Count - 1)
                            nextTime = entries.Keys[index];
                    }

                    if (NewLine != null)
                    {
                        TcpLineEventArgs args = new TcpLineEventArgs(line);

                        if (this.SynchronizingObject != null && this.SynchronizingObject.InvokeRequired)
                        {
                            this.SynchronizingObject.BeginInvoke(NewLine,
                                new object[2] { this, args });
                        }
                        else
                        {
                            NewLine(this, args);
                        }
                    }

                    if (index > entries.Count - 1)
                        break;
                }
                catch (Exception e)
                {
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, e.Message);
                    Thread.Sleep(1000);
                }

                double speed = 1.0;

#if DEBUG
                speed = GSC.Settings.SimulationSpeed;
#endif

                if (firstRun)
                    Thread.Sleep(1);
                else
                    Thread.Sleep(Math.Max(1, (int)((nextTime - time).TotalMilliseconds / speed)));

                firstRun = false;
            }
            if (reader != null)
            {
                try
                {
                    reader.Close();
                    reader = null;
                }
                catch
                {
                }
            }
        }
    }
}
