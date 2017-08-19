namespace sonOmeter.Export
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using Microsoft.Win32;
    using System.ComponentModel;
    using sonOmeter.Data;
    using sonOmeter.Data.Devices;
    using ukmaLib.Geo.Positions;
    using ukmaLib.Data;
    using System.Collections.ObjectModel;
    using ukmaLib.Connectors;
    using System.IO.Ports;

    /// <summary>
    /// Interaction logic for Export2sonOmeter2012.xaml
    /// </summary>
    public partial class Export2sonOmeter2012 : Window
    {
        public Classes.SonarProject ProjectOld { get; set; }

        private Data.SonarProject projectNew = null;
        public Data.SonarProject ProjectNew
        {
            get
            {
                if (projectNew == null)
                    projectNew = new Data.SonarProject();
                return projectNew;
            }
        }

        public Export2sonOmeter2012(Classes.SonarProject projectOld)
        {
            this.ProjectOld = projectOld;

            InitializeComponent();
            this.DataContext = this;
        }

        private void ConvertProject()
        {
#if DEBUG1
		            this.ProjectNew.Clear();

            foreach (var recOld in this.ProjectOld.Records)
            {
                ImportRecord(recOld);
            }
  
	#endif
        }

        private class DeviceComparer : IEqualityComparer<DeviceBase>
        {
            public bool Equals(DeviceBase x, DeviceBase y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(DeviceBase obj)
            {
                return obj.GetHashCode();
            }
        }

#if DEBUG1
        private void ImportRecord(Classes.SonarRecord recOld)
        {
            //if (projectNew.TimeEnd != DateTime.MinValue && (recOld.TimeStart - projectNew.TimeEnd).TotalSeconds > 2 * 10 + 30)
            //{
            //    projectNew.TimeLapseList.Add(new TimeLapse(projectNew.TimeEnd.AddSeconds(10.0), recOld.TimeStart.AddSeconds(-10.0)));
            //}

            projectNew.TimeStampList.Add(new Data.TimeStamp(recOld.TimeStart, true) { Name = recOld.Description + " Start" });
            projectNew.TimeStampList.Add(new Data.TimeStamp(recOld.TimeEnd, true) { Name = recOld.Description + " Stop" });
            var comp = new DeviceComparer();
            
            for (int d = 0; d < recOld.Devices.Count; d++)
            {
                // Fetch the old and new device (create new, if not existing)
                var devOld = recOld.Devices[d];

                if (d == projectNew.DeviceList.OfType<CompositeSonarDevice>().Count())
                {
                    // Split former device into HF and NF devices.
                    var dev = new CompositeSonarDevice() { Name = devOld.Description };
                    var devHF = new SonarDevice()
                    {
                        Name = devOld.Description + " HF",
                        Tag = "HF",
                    };
                    var devNF = new SonarDevice()
                    {
                        Name = devOld.Description + " NF",
                        Tag = "NF",
                    };
                    dev.SubDevices.Add(devHF);
                    dev.SubDevices.Add(devNF);
                    dev.DX = devOld.DX;
                    dev.DY = devOld.DY;
                    dev.DZ = devOld.DZ;
                    dev.DP = devOld.DP;
                    dev.DR = devOld.DR;
                    
                    projectNew.DeviceList.Insert(d, dev);
                }

                var devNew = (CompositeSonarDevice)projectNew.DeviceList[d];
                var devNewHF = (from dev in devNew.SubDevices
                                where dev.Tag == "HF"
                                select (SonarDevice)dev).FirstOrDefault();
                var recOffHF = devNewHF.Lines.Count();

                var devNewNF = (from dev in devNew.SubDevices
                                where dev.Tag == "NF"
                                select (SonarDevice)dev).FirstOrDefault();
                var recOffNF = devNewNF.Lines.Count();

                List<TimedData> linesOfSameTimeHF = new List<TimedData>();
                List<TimedData> linesOfSameTimeNF = new List<TimedData>();
                List<TimedData> positionsOfSameTime = new List<TimedData>();
                Dictionary<TimedData, List<TimedData>> positionBundles = new Dictionary<TimedData, List<TimedData>>();

                // Copy the content of each sonar line.
                for (int i = 0; i < devOld.SonarLines.Count; i++)
                {
                    var lineOld = devOld.SonarLines[i];

                    var line = CopySonarLine(lineOld, lineOld.HF);
                    devNewHF.AddData(line);

                    if (linesOfSameTimeHF.Count != 0 &&
                        linesOfSameTimeHF[0].Time != line.Time)
                        InterpolateTimeAndClearSameTimeList(linesOfSameTimeHF, line.Time);

                    linesOfSameTimeHF.Add(line);

                    line = CopySonarLine(lineOld, lineOld.NF);
                    devNewNF.AddData(line);

                    if (linesOfSameTimeNF.Count > 0 &&
                        linesOfSameTimeNF[0].Time != line.Time)
                        InterpolateTimeAndClearSameTimeList(linesOfSameTimeNF, line.Time);

                    linesOfSameTimeNF.Add(line);

                    // Copy position data.
                    if (d == 0)
                    {
                        var posDataList = new List<TimedData>();
                        var str = "";
                        IEnumerable<PositionsBase> list = null;
                        TimedData data;

                        // Collect all position data entries in this line.
                        foreach (var pos in lineOld.PosList)
                        {
                            List<TimedData> bundle = null;

                            switch (pos.type)
                            {
                                case Classes.PosType.NMEA:
                                    str = pos.strPos;
                                    list = NMEAParser.Parse(str) ?? new List<PositionsBase>();

                                    var nmea = projectNew.DeviceList.OfType<NMEADevice>().FirstOrDefault();

                                    if (nmea == null)
                                    {
                                        nmea = new NMEADevice()
                                        {
                                            Name = pos.type.ToString(),
                                            Tag = "NMEA",
                                        };
                                        projectNew.DeviceList.Add(nmea);
                                    }

                                    var rawPos = nmea.AddData(str + "\r\n");

                                    foreach (var item in list)
                                    {
                                        if (item is RvHvCoordinate)
                                        {
                                            data = AddPosToDevice((CoordBase)item, pos, nmea.SubDevices, str, rawPos);
                                            if (data == null)
                                                continue;
                                        }
                                        else if (item is LaLoCoordinate)
                                        {
                                            data = AddPosToDevice((CoordBase)item, pos, nmea.SubDevices, str, rawPos);
                                            if (data == null)
                                                continue;
                                        }
                                        else if (item is QualityIndicators)
                                        {
                                            data = AddPosToDevice((QualityIndicators)item, pos, nmea.SubDevices, str, rawPos);
                                            if (data == null)
                                                continue;
                                        }
                                        else
                                            continue;

                                        if (bundle == null)
                                        {
                                            bundle = new List<TimedData>();
                                            posDataList.Add(data);
                                            positionBundles.Add(data, bundle);
                                        }
                                        else
                                            bundle.Add(data);
                                    }
                                    break;

                                case Classes.PosType.LeicaTPS:
                                    break;

                                case Classes.PosType.Geodimeter:
                                    str = "<\r\n" + pos.strPos.Replace(";;", "\r\n").Replace(";", "\r\n");
                                    list = GeodimeterParser.Parse(str) ?? new List<PositionsBase>();

                                    data = AddPosToDevice((CoordBase)list.OfType<RvHvCoordinate>().FirstOrDefault(), pos, projectNew.DeviceList, str, -1);
                                    if (data != null)
                                        posDataList.Add(data);
                                    break;

                                case Classes.PosType.Compass:
                                case Classes.PosType.HMR3300:
                                    str = pos.strPos;
                                    if (pos.type == Classes.PosType.Compass)
                                    {
                                        str = "S" + str;
                                        list = SOSOCompassDepthGaugeParser.Parse(str);
                                    }
                                    else
                                    {
                                        list = HMR3x00Parser.Parse(str);
                                    }

                                    list = list ?? new List<PositionsBase>();

                                    data = AddPosToDevice(list.OfType<HeadingPitchRoll>().FirstOrDefault(), pos, projectNew.DeviceList, str + "\r\n", -1);
                                    if (data != null)
                                        posDataList.Add(data);
                                    break;
                                
                                default:
                                    posDataList.Add(new TimedDurationData<string>() { Time = pos.time, Data = pos.strPos });
                                    break;
                            }
                        }

                        // Postprocessing step.
                        foreach (var posData in posDataList)
                        {
                            // Interpolate time stamps.
                            if (positionsOfSameTime.Count > 0 &&
                                positionsOfSameTime[0].Time != posData.Time)
                            {
                                InterpolateTime(positionsOfSameTime, posData.Time);

                                foreach (var pos in positionsOfSameTime)
                                {
                                    if (!positionBundles.ContainsKey(pos))
                                        continue;

                                    foreach (var bundledPos in positionBundles[pos])
                                        bundledPos.Time = pos.Time;

                                    positionBundles[pos].Clear();
                                    positionBundles.Remove(pos);
                                }

                                positionsOfSameTime.Clear();
                            }

                            positionsOfSameTime.Add(posData);
                        }

                        posDataList.Clear();
                    }
                }

                // Copy cut lines.
                CopyCutlines(devOld.ClSetHF, devNewHF, recOffHF, recOld.Description);
                CopyCutlines(devOld.ClSetNF, devNewNF, recOffNF, recOld.Description);
            }
        }


        private TimedDurationDataRef<TData, int> AddPosToDevice<TData>(TData newPos, Classes.SonarPos pos, ObservableCollection<DeviceBase> devList, string str, int rawPos)
    where TData : PositionsBase
        {
            if (newPos == null)
                return null;

            PositionDevice<TData> dev = null;

            if (newPos is RvHvCoordinate)
            {
                dev = devList.OfType<CoordRvHvDevice>().FirstOrDefault() as PositionDevice<TData>;

                if (dev == null)
                {
                    dev = new CoordRvHvDevice()
                    {
                        Name = "RV/HV",
                        Tag = "CoordRvHv",
                    } as PositionDevice<TData>;
                    devList.Add(dev);
                    projectNew.SelectedCoordDevice = dev as CoordDevice;
                }
            }
            else if (newPos is LaLoCoordinate)
            {
                dev = devList.OfType<CoordLaLoDevice>().FirstOrDefault() as PositionDevice<TData>;

                if (dev == null)
                {
                    dev = new CoordLaLoDevice()
                    {
                        Name = "LA/LO",
                        Tag = "CoordLaLo",
                    } as PositionDevice<TData>;
                    devList.Add(dev);
                }
            }
            else if (newPos is HeadingPitchRoll)
            {
                dev = devList.OfType<CompassDevice>().FirstOrDefault() as PositionDevice<TData>;

                if (dev == null)
                {
                    dev = new CompassDevice()
                    {
                        Name = "Compass",
                        Tag = "Compass",
                    } as PositionDevice<TData>;
                    devList.Add(dev);
                    projectNew.SelectedCompassDevice = dev as CompassDevice;
                }
            }
            else if (newPos is QualityIndicators)
            {
                dev = devList.OfType<QualityDevice>().FirstOrDefault() as PositionDevice<TData>;

                if (dev == null)
                {
                    dev = new QualityDevice()
                    {
                        Name = "Quality",
                        Tag = "Quality",
                    } as PositionDevice<TData>;
                    devList.Add(dev);
                }
            }
            else
                return null;

            if (rawPos == -1)
                return dev.AddData(newPos, pos.time, !pos.Disabled, str);
            else
            {
                var item = new TimedDurationDataRef<TData, int>()
                {
                    Data = newPos,
                    Time = pos.time,
                    Enabled = !pos.Disabled,
                    RawDataReference = rawPos,
                };

                dev.AddData(item);

                return item;
            }
        } 
#endif

        private void CopyCutlines(sonOmeter.Classes.CutLineSet clSet, SonarDevice devNew, int recordOffset, string recordName)
        {
            var clT = new sonOmeter.Data.CutLine() { CutsTop = true, Description = recordName };
            var clB = new sonOmeter.Data.CutLine() { CutsTop = false, Description = recordName };

            CopyAndAddCutline(clSet.CutTop, clT, devNew, recordOffset);
            CopyAndAddCutline(clSet.CutBottom, clB, devNew, recordOffset);

            // TODO: Copy calc and surface lines.
        }

        private void CopyAndAddCutline(sonOmeter.Classes.CutLine clOld, sonOmeter.Data.SurfaceLine clNew, SonarDevice devNew, int recordOffset)
        {
            if (devNew.Lines.Count() == 0)
                return;

            int i, lastI = 1;
            int count = clOld.PointList.Count - 1;
            var accu = 0;

            foreach (var chunk in devNew.Chunks)
            {
                var lineCount = chunk.Lines.Count;

                for (i = lastI; i < count; i++)
                {
                    var point = clOld.PointList[i];
                    var index = (int)(recordOffset + point.X);

                    // Filter points that are outside of the device scope.
                    // Note: The X value actually is the index of the line.
                    if (index < 0 || index - accu >= lineCount)
                        break;

                    // Get the time of the line at index X.
                    var time = chunk.Lines[index - accu].Time;

                    // Add the new point.
                    // X: Seconds since beginning of record.
                    // Y: Integer depth in cm.
                    clNew.Add(new Point(time.ToDouble(), (Int16)(-point.Y * 100.0)));
                }

                accu += lineCount;
                lastI = i;
            }

            if (clNew.Points.Count > 0)
                devNew.SurfaceLines.Add(clNew);
        }

        private void InterpolateTime(List<TimedData> dataOfSameTime, DateTime nextTime)
        {
            var time = dataOfSameTime[0].Time;

            for (int i = 0; i < dataOfSameTime.Count; i++)
                dataOfSameTime[i].Time = time.AddSeconds((nextTime - time).TotalSeconds * (double)i / (double)dataOfSameTime.Count);
        }
        
        private void InterpolateTimeAndClearSameTimeList(List<TimedData> dataOfSameTime, DateTime nextTime)
        {
            InterpolateTime(dataOfSameTime, nextTime);
            dataOfSameTime.Clear();
        }

#if DEBUG1
        private Data.SonarLine CopySonarLine(Classes.SonarLine lineOld, Classes.LineData data)
        {
            var lineNew = new Data.SonarLine()
            {
                Time = lineOld.Time,
            };

            var max = data.Entries != null ? data.Entries.Length : 0;
            List<Data.SonarEcho> echoList = new List<Data.SonarEcho>();

            for (int i = 0; i < max; i++)
            {
                var entry = data.Entries[i];
                var low = -entry.low * 100.0f;
                var high = -entry.uncutHigh * 100.0f;
                var colorID = entry.colorID;
                var cmDepth = (Int16)Math.Round(high);
                var cmHeight = (Int16)Math.Round(low - high);

                // Split echos that are higher than 255 cm.
                while (cmHeight != 0)
                {
                    var cmH = (byte)(cmHeight > 255 ? 255 : cmHeight);
                    var echo = new Data.SonarEcho(cmDepth, cmH, colorID);
                    echoList.Add(echo);
                    cmHeight -= cmH;
                    cmDepth += cmH;
                }
            }

            var cT = (Int16)Math.Round(-data.TCut * 100.0);
            var cB = (Int16)Math.Round(-data.BCut * 100.0);

            // Copy temporary list to new line.
            lineNew.Data = new Data.SonarEcho[echoList.Count];
            echoList.CopyTo(lineNew.Data);
            lineNew.Cut(cT, cB);

            return lineNew;
        } 
#endif

        private void SaveNewProject(object sender, RoutedEventArgs e)
        {
            var oldCursor = Cursor;
            Cursor = Cursors.Wait;

            var sfd = new SaveFileDialog()
            {
                Filter = "sonOmeter 2012 Projektdateien|*.zip|Alle Dateien|*.*",
            };

            if (!sfd.ShowDialog().Value)
                return;

            StatusLabel.Content = "Wandle Projekt um.";

            ConvertProject();

            StatusLabel.Content = "Schreibe Projektdatei.";

            var task = this.ProjectNew.Save(sfd.FileName);

            StatusLabel.Content = "Schreibe Projektdatei...";

            task.Wait(-1);

            StatusLabel.Content = "Export abgeschlossen.";

            Cursor = oldCursor;
        }
    }
}
