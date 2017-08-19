using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;
using sonOmeter.Classes.Sonar2D;

namespace sonOmeter.Classes
{
  


    public class CustomerExport
    {
        #region Log
        [Browsable(false)]
        public string log
        {
            get
            {
                string s = "";
                s += "Min Höhe UK Schwinger: " + (minUKS.HasValue ? minUKS.Value.ToString("0.00", nfi).PadLeft(9, ' ') : "-") + "\r\n";
                s += "Max Höhe UK Schwinger: " + (maxUKS.HasValue ? maxUKS.Value.ToString("0.00", nfi).PadLeft(9, ' ') : "-") + "\r\n";
                s += "Mittelwert UK Schwinger: " + (sumUKS / (count - 1)).ToString("0.00", nfi).PadLeft(6, ' ') + "\r\n";
                s += "Wasserspiegel + " + hoeheLot.ToString("0.00", nfi) + ":" + ((sumUKS / (count - 1)) + hoeheLot).ToString("0.00", nfi).PadLeft(10, ' ') + "\r\n";
                s += "Minimum Wert Tiefe: " + (minDepth.HasValue ? minDepth.Value.ToString("0.00", nfi).PadLeft(11, ' ') : "-") + "\r\n";
                s += "Maximum Wert Tiefe: " + (maxDepth.HasValue ? maxDepth.Value.ToString("0.00", nfi).PadLeft(11, ' ') : "-") + "\r\n";
                s += "Anzahl der Punkte: " + (count - 1).ToString().PadLeft(12, ' ') + "\r\n";
                return s;
            }
        }
        #endregion

        [Description("Set Start Count for Export"), Category("Export"), DefaultValue(0)]
        public long Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
            }
        }

        #region Variables
        protected NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
        NumberFormatInfo nfiDE = new CultureInfo("de-DE", false).NumberFormat;

        long count = 1;
        double? minDepth = null;
        double? maxDepth = null;
        double sumUKS = 0;
        double? maxUKS = null;
        double? minUKS = null;
        double hoeheLot = 0.24;
        string exportDate = DateTime.Now.Day.ToString("DDMMYY");
        #endregion

        #region Constructors
        public CustomerExport()
        {
            nfi.NumberDecimalDigits = 3;	// 2 Nachkommastellen
        }
        #endregion

        #region ExportLine
        public string ExportLine(SonarLine line, SonarPanelType type, ExportSettings cfg, int RecordNr, DateTime time)
        {
            double? depth = null;
            int colorid = 0;

            try
            {
                LineData data = line.PrepareExportData(out depth, out colorid, type, cfg);

                if (data == null)
                    return "";

                if (depth.HasValue & (colorid > -1) & (line.CoordRvHv.Type != UKLib.Survey.Math.CoordinateType.Empty))
                {
                    #region Logging
                    if (!minDepth.HasValue || (depth > minDepth)) minDepth = depth;
                    if (!maxDepth.HasValue || (depth < maxDepth)) maxDepth = depth;
                    if (!minUKS.HasValue || (line.CoordRvHv.AL < minUKS)) minUKS = line.CoordRvHv.AL;
                    if (!maxUKS.HasValue || (line.CoordRvHv.AL > maxUKS)) maxUKS = line.CoordRvHv.AL;
                    sumUKS += line.CoordRvHv.AL;
                    #endregion

                    string s = ExportLine(line, data, type, cfg, ref count, RecordNr, time, exportDate, depth.Value, colorid);

                    count++;
                    return s;
                }
                else
                {
                    return ""; // keine Daten in der Zeile
                }
            }
            catch
            {
                return ""; // bei Fehler leerer String
            }
        }
        #endregion

        #region ExportPoint
        public string ExportPoint(ManualPoint p, int RecordNr, ExportSettings cfg)
        {
            string s = ExportPoint(p, ref count, RecordNr, exportDate, cfg);
            count++;
            return s;
        }
        #endregion

        #region Export functions
        /// <summary>
        /// Default export function for all customers. Overwrite to change export behaviour for sonar lines.
        /// </summary>
        /// <param name="line">The sonar line.</param>
        /// <param name="data">The sonar data.</param>
        /// <param name="type">The sonar data type (HF/NF).</param>
        /// <param name="cfg">The export Settings.Sets.</param>
        /// <param name="count">The number of the line.</param>
        /// <param name="RecordNr">The record number.</param>
        /// <param name="time">The start time of the record.</param>
        /// <param name="exportDate">The export date.</param>
        /// <param name="depth">The (cut) depth.</param>
        /// <param name="colorid">The top color id.</param>
        /// <param name="nfi">The number formats (culture specific).</param>
        /// <returns></returns>
        public virtual string ExportLine(SonarLine line, LineData data, SonarPanelType type, ExportSettings cfg, ref long count, int RecordNr, DateTime time, string exportDate, double depth, int colorid)
        {
            string s = "";
            s += count.ToString().PadLeft(6, ' ');	// fortlaufenden Nummer
            s += "," + line.SonID.ToString(); // SonarID
            s += "," + line.CoordRvHv.RV.ToString("F", nfi).PadLeft(8, ' '); // Rechtswert Sonar
            s += "," + line.CoordRvHv.HV.ToString("F", nfi).PadLeft(8, ' '); // Hochwert Sonar
            s += "," + line.CoordRvHv.AL.ToString("F", nfi).PadLeft(6, ' '); // Hoehe Sonar
            s += "," + type.ToString(); // HF oder NF
            s += "," + depth.ToString("F", nfi); // Tiefe
            s += "," + colorid.ToString(); // Farbe   
            return s;
        }

        /// <summary>
        /// Default export function for all customers. Overwrite to change export behaviour for manual points.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="count"></param>
        /// <param name="RecordNr">The record number.</param>
        /// <param name="exportDate">The export date.</param>
        /// <param name="cfg">The export Settings.Sets.</param>
        /// <param name="nfi"></param>
        /// <returns></returns>
        public virtual string ExportPoint(ManualPoint p, ref long count, int RecordNr, string exportDate, ExportSettings cfg)
        {
            string s = "";
            s += count.ToString().PadLeft(6, ' ');	// fortlaufenden Nummer     

            s += "," + p.Coord.RV.ToString("F", nfi).PadLeft(8, ' '); // Rechtswert Sonar
            s += "," + p.Coord.HV.ToString("F", nfi).PadLeft(8, ' '); // Hochwert Sonar
            s += "," + p.Coord.AL.ToString("F", nfi).PadLeft(6, ' '); // Hoehe Sonar
            s += "," + p.Depth.ToString("F", nfi); // Depth
            s += "," + p.CoordType.ToString(); // Type
            s += "," + p.Color.ToString(); // Farbe   
            return s;
        }

        /// <summary>
        /// Default export function for all customers. Overwrite to change export behaviour for the beginning of records.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public virtual string ExportBeginRecord(SonarRecord record)
        {
            string s = "";
            return s;
        }

        /// <summary>
        /// Default export function for all customers. Overwrite to change export behaviour for the end of records.
        /// </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public virtual string ExportEndRecord(SonarRecord record)
        {
            string s = "";
            return s;
        }
        #endregion

        public override string ToString()
        {
            string name = base.ToString();
            name = name.Substring(name.LastIndexOf(".") + 1);
            return name.Replace("Customer", "");
        }
    }
}
