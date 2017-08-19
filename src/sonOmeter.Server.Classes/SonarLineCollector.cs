using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using UKLib.Debug;
using System.Timers;

namespace sonOmeter.Server.Classes
{
    public class SonarLineCollector : ISonarLineSource, ISonarLineSink, IPositionDataSink
    {
        #region Variables
        List<ISonarLineSource> sources = new List<ISonarLineSource>();
        Queue<PositionData> posQueue = new Queue<PositionData>();

        Timer tmSonID0;

        private bool tmSon0IDUse = false;
        private int tmSonID0Reload = 10000;
        private int tmSonID0EventReload = 1000;

        private bool filterType32 = false;
        private bool filterType33 = false;
        private bool filterType37 = false;
        #endregion

        #region Properties

        public bool FilterType32
        {
            get { return filterType32; }
            set { filterType32 = value; }
        }

        public bool FilterType33
        {
            get { return filterType33; }
            set { filterType33 = value; }
        }

        public bool FilterType37
        {
            get { return filterType37; }
            set { filterType37 = value; }
        }

        public bool TmSon0ID0Use
        {
            get { return tmSon0IDUse; }
            set 
            { 
                tmSon0IDUse = value;
                tmSonID0.Enabled = value;
            }
        }

        public int TmSonID0Reload
        {
            get { return tmSonID0Reload; }
            set { tmSonID0Reload = value; }
        }

        public int TmSonID0EventReload
        {
            get { return tmSonID0EventReload; }
            set { tmSonID0EventReload = value; }
        }
        #endregion

        #region Constructor
        public SonarLineCollector()
        {
            tmSonID0 = new Timer(tmSonID0Reload);
            tmSonID0.Enabled = tmSon0IDUse;
            tmSonID0.Elapsed += new ElapsedEventHandler(tmSonID0_Elapsed);
        }
        #endregion

        #region Timer SonID0 Event
        void tmSonID0_Elapsed(object sender, ElapsedEventArgs e)
        {
            source_OnNewSonarLine(tmSonID0, new SonarLineEventArgs(new SonarLine(0)));
            tmSonID0.Interval = tmSonID0EventReload;
        }
        #endregion

        #region ISonarLineSource Members

        public event SonarLineEventHandler OnNewSonarLine;
             
        public string descriptiveXML
        {
            get
            {
                string res="";
                foreach (ISonarLineSource source in sources) res += source.descriptiveXML;
                return (res);
            }
        }
        #endregion

        #region Clear
        public void Clear()
        {
            sources.Clear();
            posQueue.Clear();
        }
        #endregion

        #region ISonarLineSink Members

        public void ConnectToSource(ISonarLineSource source)
        {
            source.OnNewSonarLine += new SonarLineEventHandler(source_OnNewSonarLine);
            sources.Add(source);
        }

        void source_OnNewSonarLine(object sender, SonarLineEventArgs e)
        {
            SonarLine line = e.Line;

            lock (posQueue)
            {
                foreach (PositionData pos in line.posList)
                {
                    //Extract all the PositionData from the SonarLines into posQueue ...
                    if (posQueue.Count < 50)
                    {
                        posQueue.Enqueue(pos);
                    }
                    else
                    {
                        //To many Items in PosQueue!!
                        DebugClass.SendDebugLine(this, DebugLevel.Red, string.Format("Too many ({0}) Items in PositionQueue!", posQueue.Count));
                    }
                }
                //... wipe out all Position Entries ...
                line.posList.Clear();

                //... and add them to SonarLine with sonId=0
                //... filter posi32 (compass) only one message per line
                if (line.SonID == 0)
                {
                    tmSonID0.Enabled = false;
                    PositionData pos32 = new PositionData();
                    PositionData pos33 = new PositionData();
                    PositionData pos37 = new PositionData();
                    int pos32idx = -1;
                    int pos33idx = -1;
                    int pos37idx = -1;
                    while (posQueue.Count > 0)
                    {
                        PositionData pos = posQueue.Dequeue();
                        if (pos.Type == 32 & filterType32)
                        {
                            pos32 = pos;
                            pos32idx = line.posList.Count;
                        }
                        else
                            if (pos.Type == 33 & filterType33)
                            {
                                pos33 = pos;
                                pos33idx = line.posList.Count;
                            }
                            else
                                if (pos.Type == 37 & filterType37)
                                {
                                    pos37 = pos;
                                    pos37idx = line.posList.Count;
                                }
                                else
                            {
                                line.posList.Add(pos);
                            }
                    }
                    if (pos32idx != -1)
                        line.PosList.Insert(pos32idx, pos32);
                    if (pos33idx != -1)
                        line.PosList.Insert(pos33idx, pos33);
                    if (pos37idx != -1)
                        line.PosList.Insert(pos37idx, pos37);
                    tmSonID0.Interval = tmSonID0Reload;
                    tmSonID0.Enabled = tmSon0IDUse;
                }
            }
            //And send this
            if (OnNewSonarLine != null)
                OnNewSonarLine(this, new SonarLineEventArgs(line));            
        }

        #endregion
        
        #region IPositionDataSink Members

        public void ConnectToSource(IPositionDataSource source)
        {
            source.OnNewPositionData += new PositionDataEventHandler(source_OnNewPositionData);
        }

        void source_OnNewPositionData(object sender, PositionDataEventArgs e)
        {
            PositionData pos = e.Data;
            if (posQueue.Count < 50)
            {
                posQueue.Enqueue(pos);
            }
            else
            {
                //To many Items in PosQueue!!
                DebugClass.SendDebugLine(this, DebugLevel.Red, string.Format("Too many ({0}) Items in PositionQueue!", posQueue.Count));
            } 
        }

        #endregion     
    }
}
