namespace NSExportClass
{
    #region Usings
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using sonOmeter.Classes;
    using sonOmeter.Classes.Sonar2D;
    using UKLib.MathEx;
    using UKLib.Survey.Math;
    #endregion

#if DEBUG
  
    public class CustomerDefault : CustomerExport
    {
        #region Variables
        string lastPosString = "";
        #endregion

        #region Constructor
        public CustomerDefault()
        {
        }
        #endregion

        #region New export variant switch
        public enum ExportVariant
        {
            Rech3D = 0,
            LALO = 1           
        }

        protected string attachPosString = "";
        public string AttachPosString
        {
            get { return attachPosString; }
            set { attachPosString = value; }
        }

        protected ExportVariant variant = ExportVariant.Rech3D;
        public ExportVariant Variant
        {
            get { return variant; }
            set { variant = value; }
        }
        #endregion

        #region Export functions
        public override string ExportLine(SonarLine line, LineData data, SonarPanelType type, ExportSettings cfg, ref long count, int RecordNr, DateTime time, string exportDate, double depth, int colorid)
        {
            string s = "";

            switch (variant)
            {
                case ExportVariant.Rech3D:
                    s += count.ToString().PadLeft(6, ' ');	// fortlaufenden Nummer
                    nfi.NumberDecimalDigits = 3;
                    double RV = line.CoordRvHv.RV;
                    double HV = line.CoordRvHv.HV;

                    //double dx = -788628.878;
                    //double dy = -4683898.507;

                    //double ddx = 676316.840;
                    //double ddy = 104465.130;
                    //double dsc = 1.0110117761;
                    //double da = 0.0206172017;

                    //RV = RV + dx - ddx;
                    //HV = HV + dy - ddy;
                    //try
                    //{
                    //    double r = Math.Sqrt(RV * RV + HV * HV) * dsc;
                    //    double a = Math.Atan2(HV, RV) + da;
                    //    RV = Math.Cos(a) * r + ddx;
                    //    HV = Math.Sin(a) * r + ddy;
                    //}
                    //catch
                    //{
                    //    RV = ddx;
                    //    HV = ddy;
                    //}

                    s += "," + RV.ToString("F", nfi); // Rechtswert Sonar
                    s += "," + HV.ToString("F", nfi); // Hochwert Sonar
                    nfi.NumberDecimalDigits = 2;
                    s += "," + line.CoordRvHv.AL.ToString("F", nfi); // Hoehe Sonar
                    s += "," + data.GetVolume().ToString("F", nfi); // Volume
                    s += "," + depth.ToString("F", nfi); // Tiefe
                    s += "," + colorid.ToString(); // Farbe
                    break;

                case ExportVariant.LALO:                
                    s += count.ToString().PadLeft(6, ' ');	// fortlaufenden Nummer
                    s += ";" + line.Time.ToShortDateString() + " " + line.Time.ToLongTimeString();
                    nfi.NumberDecimalDigits = 3;
                    double LA = line.CoordLaLo.LA * 180 / System.Math.PI;
                    double LO = line.CoordLaLo.LO * 180 / System.Math.PI;

                    s += ";" + ((int)(LO)).ToString();
                    LO = (LO - (int)LO) * 60;
                    s += "," + ((int)(LO)).ToString();
                    LO = (LO - (int)LO) * 60;
                    s += "," + (LO).ToString("F", nfi);

                    s += ";" + ((int)(LA)).ToString();
                    LA = (LA - (int)LA) * 60;
                    s += "," + ((int)(LA)).ToString();
                    LA = (LA - (int)LA) * 60;
                    s += "," + (LA).ToString("F", nfi);

                    nfi.NumberDecimalDigits = 2;
                    s += ";" + line.CoordLaLo.AL.ToString("F", nfi); // Hoehe Sonar
                    s += ";" + depth.ToString("F", nfi); // Tiefe
                    s += ";" + colorid.ToString(); // Farbe
                    s += ";" + ((int)data.GetVolume()).ToString(); // Volume                                        
                    break;
            }
            if (!string.IsNullOrWhiteSpace(AttachPosString))
            {
                string postString = "";
                foreach (SonarPos item in line.PosList)
                {
                    if (item.strPos.Contains(AttachPosString))
                        postString = item.strPos.Replace(",", ";");
                }
                if (postString == "")
                    postString = lastPosString + ";old";
                else
                    lastPosString = postString;

                s += ";" + postString;
            }

            return s;
        }
        #endregion
    }
   
#endif
}