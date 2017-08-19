using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using UKLib.MathEx;
using UKLib.Survey.Math;
using System.Collections.ObjectModel;
using System.Drawing.Design;
using System.Xml;
using UKLib.Xml;
using System.Globalization;
using UKLib.Survey.Parse;
using System.Windows.Forms;
using UKLib.Controls;
using System.Collections.Generic;
using UKLib.Arrays;
using System.Xml.Serialization;

namespace sonOmeter.Classes.Sonar2D
{
    #region Base class and enumerations
    /// <summary>
    /// This enumeration type holds the layer types for the 2D view.
    /// </summary>
    public enum Sonar2DLayers
    {
        Zero = 0,
        Background = 1,
        Grid = 2,
        SonarLine = 3,
        RecordMarker = 4,
        Marker = 6,
        Pointer = 7,
        WorkLineMarker = 8
    }

    /// <summary>
    /// A layer in the 2D view.
    /// </summary>
    public class Sonar2DLayer
    {
        #region Variables
        private Collection<Sonar2DElement> elements = new Collection<Sonar2DElement>();
        #endregion

        #region Properties
        public Collection<Sonar2DElement> Elements
        {
            get { return elements; }
        }
        #endregion

        #region Functions
        public void Paint(Graphics g, RectangleD rcRegion, double scaleFactor)
        {
            int count = elements.Count;

            for (int i = 0; i < count; i++)
                elements[i].Paint(g, rcRegion, scaleFactor);
        }
        #endregion
    }

    /// <summary>
    /// This class generalizes the different 2D elements.
    /// </summary>
    public class Sonar2DElement : FilterablePropertyBase, IHasID
    {
        #region Variables
        protected Sonar2DLayers layer;
        protected int id = 0;
        protected string desc = "n/a";
        protected object tag = null;
        protected Coordinate coord;
        protected Color color = Color.Black;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the actual layer of this object.
        /// </summary>
        [Browsable(false)]
        public Sonar2DLayers Layer
        {
            get { return layer; }
        }

        /// <summary>
        /// An application specific tag e.g. an object reference.
        /// </summary>
        [Browsable(false)]
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// The world coordinate of this object.
        /// </summary>
        [Browsable(false)]
        public Coordinate Coord
        {
            get { return coord; }
            set { coord = value; }
        }

        [Description("An unique identifier."), DefaultValue(0)]
        public virtual int ID
        {
            get { return id; }
            set { id = value; }
        }

        [Description("The description of the object."), DefaultValue("n/a")]
        public virtual string Description
        {
            get { return desc; }
            set { desc = value; }
        }

        [Description("The color when displayed."), DefaultValue(KnownColor.Black)]
        public virtual Color Color
        {
            get { return color; }
            set { color = value; }
        }
        #endregion

        #region Constructor
        public Sonar2DElement()
        {
            this.layer = Sonar2DLayers.Zero;
        }

        public Sonar2DElement(Sonar2DLayers layer, object tag)
        {
            this.layer = layer;
            this.tag = tag;
        }
        #endregion

        #region Functions
        public virtual void Paint(Graphics g, RectangleD rcRegion, double scaleFactor)
        {
        }
        #endregion
    }
    #endregion

    #region Pointer
    public class Pointer : Sonar2DElement
    {
        public enum PointerType
        {
            Triangle = 0,
            Cross = 1
        }

        #region Variables
        private double velocity = 0;
        private double angle = 0;
        private PointerType type = PointerType.Triangle;
        #endregion

        #region Properties
        public double Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }

        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public PointerType Type
        {
            get { return type; }
            set { type = value; }
        }
        #endregion

        public Pointer()
        {
            this.layer = Sonar2DLayers.Pointer;
        }

        public override void Paint(Graphics g, RectangleD rcRegion, double scaleFactor)
        {
            PointD pt = this.coord.Point - rcRegion.Center;
            float d = (float)(20.0 / scaleFactor);
            float vel = (float)(velocity / scaleFactor);

            Pen p = new Pen(this.color, (float)(6.0 / scaleFactor));

            Matrix t = g.Transform;
            g.TranslateTransform((float)pt.X, (float)pt.Y);
            g.RotateTransform((float)angle);

            if (this.Type == PointerType.Triangle)
            {
                g.DrawLine(p, 0, d, 0, -d);
                g.DrawLine(p, 0, -d, vel, 0);
                g.DrawLine(p, vel, 0, 0, d);
            }
            else
            {
                g.DrawLine(p, 0, d, 0, -d);
                g.DrawLine(p, -d, 0, vel, 0);
            }

            g.Transform = t;
        }
    }
    #endregion

    #region RecordMarker
    public class RecordMarker : Sonar2DElement
    {
        #region Variables
        private double angle = 0;
        #endregion

        #region Properties
        public double Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        public Coordinate NextCoord
        {
            set
            {
                double dRV = coord.RV - value.RV;
                double dHV = coord.HV - value.HV;

                angle = Math.Atan2(dHV, dRV) * 180 / Math.PI;
            }
        }
        #endregion

        public RecordMarker()
        {
            this.layer = Sonar2DLayers.RecordMarker;
        }

        public override void Paint(Graphics g, RectangleD rcRegion, double scaleFactor)
        {
            double cos = System.Math.Cos((angle * System.Math.PI) / 180.0);
            double sin = System.Math.Sin((angle * System.Math.PI) / 180.0);

            PointD pt1 = new PointD(-sin, cos) * (10.0 / scaleFactor) + this.coord.Point - rcRegion.Center;
            PointD pt2 = new PointD(sin, -cos) * (10.0 / scaleFactor) + this.coord.Point - rcRegion.Center;

            g.DrawLine(new Pen(GSC.Settings.CS.RecordMarkerColor, (float)(4.0 / scaleFactor)), pt1.PointF, pt2.PointF);
        }
    }
    #endregion

    #region WorkLineMarker
    public class WorkLineMarker : Sonar2DElement
    {
        public WorkLineMarker()
        {
            this.layer = Sonar2DLayers.WorkLineMarker;
        }

        public override void Paint(Graphics g, RectangleD rcRegion, double scaleFactor)
        {
            float penSize = (float)(3.0 / scaleFactor);
            Pen dimPen = new Pen(Color.White, penSize);
            float size = (float)(40.0 / scaleFactor);
            float sizeH = size / 2.0F;
            float offset = (float)(1.0 / scaleFactor);

            PointD pt = this.coord.Point - rcRegion.Center;

            g.DrawEllipse(new Pen(this.color, penSize), (float)pt.X - sizeH, (float)pt.Y - sizeH, size, size);
            g.DrawLine(dimPen, (float)pt.X + offset, (float)pt.Y, (float)pt.X + (sizeH - offset), (float)pt.Y);
            g.DrawLine(dimPen, (float)pt.X - offset, (float)pt.Y, (float)pt.X - (sizeH - offset), (float)pt.Y);
            g.DrawLine(dimPen, (float)pt.X, (float)pt.Y + offset, (float)pt.X, (float)pt.Y + (sizeH - offset));
            g.DrawLine(dimPen, (float)pt.X, (float)pt.Y - offset, (float)pt.X, (float)pt.Y - (sizeH - offset));
        }
    }
    #endregion

    #region Marker
    public class Marker : Sonar2DElement
    {
        #region Variables
        protected float depth = 0.0F;
        protected int type = 0;

        public static int LastType = 0;

        protected Buoy.BuoyType buoyShape = Buoy.BuoyType.Default;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the buoy type.
        /// </summary>
        [Description("Specifies the buoy shape."), DefaultValue(Buoy.BuoyType.Default), DisplayName("Buoy Shape"), LicencedProperty(Module.Modules.V22)]
        public virtual Buoy.BuoyType BuoyShape
        {
            get { return buoyShape; }
            set { buoyShape = value; }
        }

        [Description("A custom type identifier."), DefaultValue(0)]
        public virtual int Type
        {
            get { return type; }
            set { type = value; }
        }

        [Description("The depth relative to the water level."), Category("Coordinate"), DefaultValue(0.0F)]
        public virtual float Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        [Browsable(false)]
        public float DepthCorrection
        {
            get
            {
                float d = 0;

                if (tag is SonarLine)
                {
                    SonarLine line = tag as SonarLine;
                    Coordinate coordPt = GetCoordRVHV();
                    Coordinate coordLine = line.CoordRvHv;

                    if (coordLine.Type == CoordinateType.TransverseMercator)
                        d = (float)(coordPt.AL - coordLine.AL);
                }

                return d;
            }
        }

        [Description("The coordinate type."), Category("Coordinate"), DisplayName("Coordinate Type"), DefaultValue(0.0), Browsable(false)]
        public virtual CoordinateType CoordType
        {
            get { return coord.Type; }
            set
            {
                if ((value == CoordinateType.Cartesian) | (value == CoordinateType.Empty))
                    return;

                this.coord = new Coordinate(0.0, 0.0, 0.0, value);

                switch (value)
                {
                    case CoordinateType.Elliptic:
                        break;
                    case CoordinateType.TransverseMercator:
                        break;
                }
            }
        }

        [Description("The right value of the coordinate."), Category("Coordinate"), DisplayName("Right Value"), DefaultValue(0.0), Browsable(false)]
        public double RV
        {
            get { return coord.RV; }
            set { coord.RV = value; }
        }

        [Description("The high value of the coordinate."), Category("Coordinate"), DisplayName("High Value"), DefaultValue(0.0), Browsable(false)]
        public double HV
        {
            get { return coord.HV; }
            set { coord.HV = value; }
        }

        [Description("The latitude of the coordinate."), Category("Coordinate"), DisplayName("Latitude"), DefaultValue(0.0), Browsable(false)]
        public double LA
        {
            get { return coord.LA; }
            set { coord.LA = value; }
        }

        [Description("The longitude of the coordinate."), Category("Coordinate"), DisplayName("Longitude"), DefaultValue(0.0), Browsable(false)]
        public double LO
        {
            get { return coord.LO; }
            set { coord.LO = value; }
        }

        [Description("The altitude of the coordinate (water level)."), Category("Coordinate"), DisplayName("Altitude"), DefaultValue(0.0), Browsable(false)]
        public double AL
        {
            get { return coord.AL; }
            set { coord.AL = value; }
        }

        [Editor(typeof(CoordinateEditor), typeof(UITypeEditor)), DisplayName("Coordinate"), Category("Coordinate")]
        public virtual Coordinate Co
        {
            get { return coord; }
            set { coord = value; }
        }
        #endregion

        #region Paint
        public PointD GetPointRVHV()
        {
            Coordinate c = coord;

            if (c.Type == CoordinateType.Elliptic)
                c = GSC.Settings.InversTransform.Run(c, CoordinateType.TransverseMercator);

            return c.Point;
        }

        public Coordinate GetCoordRVHV()
        {
            Coordinate c = coord;

            if (c.Type == CoordinateType.Elliptic)
                c = GSC.Settings.InversTransform.Run(c, CoordinateType.TransverseMercator);

            return c;
        }

        public override void Paint(Graphics g, RectangleD rcRegion, double scaleFactor)
        {
            PointD pt = GetPointRVHV() - rcRegion.Center;

            double size = GSC.Settings.BuoySize;

            if (GSC.Settings.BuoySizeUnit == Buoy.BuoySizeUnit.Pixels)
                size /= scaleFactor;

            PaintDirect(g, (float)pt.X, (float)pt.Y, (float)size, id.ToString(), scaleFactor, true);
        }

        protected void PaintDirect(Graphics g, float x, float y, float size, string s, double scaleFactor, bool invertY)
        {
            float sizeH = size / 2.0F;
            Font font = new Font("Tahoma", 9, FontStyle.Bold);
            Pen pen = new Pen(this.color, (float)(2.0 / scaleFactor));
            Matrix t = g.Transform;

            g.TranslateTransform(x, y);
            if (invertY)
                g.ScaleTransform(1.0F, -1.0F);

            switch (buoyShape)
            {
                case Buoy.BuoyType.Default:
                    SizeF strSize = g.MeasureString(s, font);
                    float w = strSize.Width + 5.0F;

                    g.ScaleTransform(size / w, size / w);

                    pen.Width *= w / size;

                    g.DrawEllipse(pen, -w / 2.0F, -w / 2.0F, w, w);
                    g.DrawString(s, font, pen.Brush, -strSize.Width / 2.0F, -strSize.Height / 2.0F);
                    break;

                case Buoy.BuoyType.Cross:
                    g.DrawLine(pen, -sizeH, 0.0F, sizeH, 0.0F);
                    g.DrawLine(pen, 0.0F, -sizeH, 0.0F, sizeH);
                    break;

                case Buoy.BuoyType.Cross45:
                    g.RotateTransform(45);

                    g.DrawLine(pen, -sizeH, 0.0F, sizeH, 0.0F);
                    g.DrawLine(pen, 0.0F, -sizeH, 0.0F, sizeH);
                    break;
            }

            g.Transform = t;

            pen.Dispose();
            font.Dispose();
        }

        public virtual void Paint1D(Graphics g, float x, float y, float size, int w, int h)
        {
            PaintDirect(g, x, y, size, id.ToString(), 1.0, false);
        }
        #endregion

        #region XML r/w
        public virtual void WriteXml(XmlTextWriter writer)
        {
            writer.WriteStartElement(GetXmlTag());
            WriteAttributes(writer);
            writer.WriteEndElement();
        }

        public virtual void WriteAttributes(XmlWriter writer)
        {
            NumberFormatInfo nfi = GSC.Settings.NFI;

            writer.WriteAttributeString("id", id.ToString(nfi));
            writer.WriteAttributeString("desc", desc);
            if (coord.Type == CoordinateType.TransverseMercator)
            {
                writer.WriteAttributeString("rv", coord.RV.ToString(nfi));
                writer.WriteAttributeString("hv", coord.HV.ToString(nfi));
            }
            else if (coord.Type == CoordinateType.Elliptic)
            {
                writer.WriteAttributeString("la", coord.LA.ToString(nfi));
                writer.WriteAttributeString("lo", coord.LO.ToString(nfi));
            }
            writer.WriteAttributeString("al", coord.AL.ToString(nfi));
            writer.WriteAttributeString("argb", color.ToArgb().ToString("x", nfi));
            writer.WriteAttributeString("depth", depth.ToString(nfi));
            writer.WriteAttributeString("type", type.ToString(nfi));
            writer.WriteAttributeString("buoyshape", ((int)buoyShape).ToString(nfi));
            if (color.IsNamedColor)
                writer.WriteAttributeString("colorname", color.Name);
        }

        public virtual void ReadXml(XmlTextReader reader)
        {
            id = XmlReadConvert.Read(reader, "id", id);
            desc = XmlReadConvert.Read(reader, "desc", desc);

            string rv = reader.GetAttribute("rv");
            string hv = reader.GetAttribute("hv");

            string la = reader.GetAttribute("la");
            string lo = reader.GetAttribute("lo");

            string al = reader.GetAttribute("al");

            double d_la = 0, d_lo = 0, d_rv = 0, d_hv = 0, d_al = 0;

            if (rv != null)
            {
                double.TryParse(rv, NumberStyles.Float, GSC.Settings.NFI, out d_rv);
                double.TryParse(hv, NumberStyles.Float, GSC.Settings.NFI, out d_hv);
                double.TryParse(al, NumberStyles.Float, GSC.Settings.NFI, out d_al);

                coord = new Coordinate(d_rv, d_hv, d_al, CoordinateType.TransverseMercator);
            }
            else if (la != null)
            {
                double.TryParse(la, NumberStyles.Float, GSC.Settings.NFI, out d_la);
                double.TryParse(lo, NumberStyles.Float, GSC.Settings.NFI, out d_lo);
                double.TryParse(al, NumberStyles.Float, GSC.Settings.NFI, out d_al);

                coord = new Coordinate(d_la, d_lo, d_al, CoordinateType.Elliptic);
            }

            color = XmlReadConvert.Read(reader, color, GSC.Settings.NFI);
            depth = XmlReadConvert.Read(reader, "depth", depth);
            type = XmlReadConvert.Read(reader, "type", type);
            buoyShape = (Buoy.BuoyType)XmlReadConvert.Read(reader, "buoyshape", (int)Buoy.BuoyType.Default);
        }

        protected virtual string GetXmlTag()
        {
            return "marker";
        }
        #endregion
    }

    public class Buoy : Marker
    {
        #region Enumerations
        public enum BuoyType
        {
            Default = 0,
            Cross = 1,
            Cross45 = 2
        }

        public enum BuoySizeUnit
        {
            Pixels = 0,
            Meters = 1
        }
        #endregion

        #region Variables
        private double station = 0.0;
        #endregion

        #region Properties
        [Description("The station."), DefaultValue(0.0), Category("Coordinate"), LicencedProperty(Module.Modules.V22), ReadOnly(true)]
        public double Station
        {
            get { return station; }
            set { station = value; }
        }
        #endregion

        #region Constructor
        public Buoy()
        {
            this.layer = Sonar2DLayers.Marker;
            this.coord = new Coordinate(0, 0, 0, CoordinateType.TransverseMercator);
            this.depth = 0.0F;
            this.type = 0;
        }
        #endregion

        #region ToString()
        public override string ToString()
        {
            return "Buoy " + id.ToString();
        }
        #endregion

        #region XML r/w
        static public void WriteXml(string fileName, IDList<Sonar2DElement> list, IDList<Sonar2DElement> connections, NumberFormatInfo nfi)
        {
            try
            {
                XmlTextWriter writer = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.WriteStartDocument(true);

                WriteXml(writer, list, connections, nfi);

                writer.WriteEndDocument();
                writer.Close();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(null, UKLib.Debug.DebugLevel.Red, "BuoyEditorForm.WriteXml: " + e.Message);
            }
        }

        static public void WriteXml(XmlTextWriter writer, IDList<Sonar2DElement> list, IDList<Sonar2DElement> connections, NumberFormatInfo nfi)
        {
            try
            {
                writer.WriteStartElement("buoylist");

                foreach (Buoy b in list)
                    b.WriteXml(writer);

                foreach (BuoyConnection c in connections)
                {
                    writer.WriteStartElement("connection");
                    writer.WriteAttributeString("id", c.ID.ToString(nfi));
                    writer.WriteAttributeString("desc", c.Description);
                    writer.WriteAttributeString("start", c.StartBuoy.ID.ToString(nfi));
                    writer.WriteAttributeString("end", c.EndBuoy.ID.ToString(nfi));
                    writer.WriteAttributeString("argb", c.Color.ToArgb().ToString("x", nfi));
                    if (c.Color.IsNamedColor)
                        writer.WriteAttributeString("colorname", c.Color.Name);
                    writer.WriteAttributeString("width", c.Width.ToString(nfi));
                    writer.WriteAttributeString("corridor", c.Corridor.ToString(nfi));
                    writer.WriteAttributeString("catchwidth", c.CatchWidth.ToString(nfi));
                    writer.WriteAttributeString("hlargb", c.HighlightColor.ToArgb().ToString("x", nfi));
                    if (c.HighlightColor.IsNamedColor)
                        writer.WriteAttributeString("hlcolorname", c.HighlightColor.Name);
                    writer.WriteAttributeString("hlalpha", c.AlphaChannel.ToString(nfi));
                    writer.WriteAttributeString("parameter", c.Parameter.ToString(nfi));
                    writer.WriteAttributeString("radius", c.Radius.ToString(nfi));
                    writer.WriteAttributeString("type", ((int)c.Type).ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(null, UKLib.Debug.DebugLevel.Red, "BuoyEditorForm.WriteXml: " + e.Message);
            }
        }

        static public void ReadXml(string fileName, IDList<Sonar2DElement> list, IDList<Sonar2DElement> connections)
        {
            try
            {
                XmlTextReader reader = new XmlTextReader(fileName);
                reader.WhitespaceHandling = WhitespaceHandling.None;

                ReadXml(reader, list, connections);

                reader.Close();
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(null, UKLib.Debug.DebugLevel.Red, "BuoyEditorForm.ReadXml: " + e.Message);
            }
        }

        static public void ReadXml(XmlTextReader reader, IDList<Sonar2DElement> list, IDList<Sonar2DElement> connections)
        {
            try
            {
                while (!reader.EOF)
                {
                    if ((reader.Name == "buoylist") && (reader.NodeType == XmlNodeType.EndElement))
                        break;

                    if ((reader.Name == "buoy") && reader.IsStartElement())
                    {
                        Buoy b = new Buoy();
                        b.ReadXml(reader);
                        list.Add(b);
                    }
                    else if ((reader.Name == "connection") && reader.IsStartElement())
                    {
                        BuoyConnection c = new BuoyConnection();
                        c.ID = XmlReadConvert.Read(reader, "id", c.ID);
                        c.Description = XmlReadConvert.Read(reader, "desc", c.Description);
                        int start = XmlReadConvert.Read(reader, "start", -1);
                        int end = XmlReadConvert.Read(reader, "end", -1);
                        c.Color = XmlReadConvert.Read(reader, c.Color, GSC.Settings.NFI);
                        c.Width = XmlReadConvert.Read(reader, "width", 3);
                        c.Corridor = XmlReadConvert.Read(reader, "corridor", 0.0);
                        c.CatchWidth = XmlReadConvert.Read(reader, "catchwidth", 0.0);
                        c.HighlightColor = XmlReadConvert.Read(reader, c.HighlightColor, "hl", GSC.Settings.NFI);
                        c.AlphaChannel = (byte)XmlReadConvert.Read(reader, "hlalpha", 127);
                        c.Parameter = XmlReadConvert.Read(reader, "parameter", 0.0);
                        c.Radius = XmlReadConvert.Read(reader, "radius", 0.0);
                        c.Type = (DA040ElementType)XmlReadConvert.Read(reader, "type", (int)DA040ElementType.Line);

                        c.StartBuoy = list.GetByID(start) as Buoy;
                        c.EndBuoy = list.GetByID(end) as Buoy;

                        if ((c.StartBuoy != null) && (c.EndBuoy != null))
                            connections.Add(c);
                    }

                    reader.Read();
                }
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(null, UKLib.Debug.DebugLevel.Red, "BuoyEditorForm.ReadXml: " + e.Message);
            }
        }

        protected override string GetXmlTag()
        {
            return "buoy";
        }

        public override void WriteAttributes(XmlWriter writer)
        {
            base.WriteAttributes(writer);

            writer.WriteAttributeString("station", station.ToString(GSC.Settings.NFI));
        }

        public override void ReadXml(XmlTextReader reader)
        {
            base.ReadXml(reader);

            station = XmlReadConvert.Read(reader, "station", station);
        }
        #endregion

        #region DA040
        static public void ReadDA040(string fileName, IDList<Sonar2DElement> list, IDList<Sonar2DElement> connections, Buoy bTemplate, BuoyConnection cTemplate)
        {
            DA040File da040 = new DA040File(fileName);
            int max = da040.elementList.Count;
            Buoy b;
            Buoy lastBuoy = null;
            DA040Element element;

            for (int i = 0; i < max; i++)
            {
                b = new Buoy();

                // copy template
                b.BuoyShape = bTemplate.BuoyShape;
                b.Color = bTemplate.Color;
                b.Description = bTemplate.Description;
                b.Type = bTemplate.Type;

                if (i < max)
                {
                    element = da040.elementList[i];
                    b.Coord = new Coordinate(element.Pos.X, element.Pos.Y, 0, CoordinateType.TransverseMercator);
                }
                else
                {
                    // tbd: Last DA040 element contains additional end buoy and connection!
                }

                list.Add(b);

                if (lastBuoy == null)
                {
                    lastBuoy = b;
                    continue;
                }

                element = da040.elementList[i - 1];

                // create new connection and search free ID
                BuoyConnection c = new BuoyConnection();

                // set the appropriate start/end buoys
                c.StartBuoy = lastBuoy;
                c.EndBuoy = b;

                // copy template settings
                c.AlphaChannel = cTemplate.AlphaChannel;
                c.Width = cTemplate.Width;
                c.Corridor = cTemplate.Corridor;
                c.CatchWidth = cTemplate.CatchWidth;
                c.Color = cTemplate.Color;
                c.HighlightColor = cTemplate.HighlightColor;

                // set type
                switch (element.Type)
                {
                    case DA040ElementType.Line:
                        c.Description = "DA040 line element";
                        c.Type = DA040ElementType.Line;
                        break;

                    case DA040ElementType.Arc:
                        c.Description = "DA040 arc element";
                        c.Radius = element.Radius;
                        c.Type = DA040ElementType.Arc;
                        break;

                    case DA040ElementType.Clothoid:
                        // tbd
                        c.Description = "DA040 clothoid element";
                        c.Parameter = element.Parameter;
                        c.Radius = element.Radius;
                        c.Type = DA040ElementType.Clothoid;
                        break;
                }

                c.SetStation(element.Station);

                connections.Add(c);

                lastBuoy = b;
            }
        }

        static public void UpdateStation(Buoy b, double abs, List<BuoyConnection> connections, List<Buoy> buoysDone)
        {
            List<BuoyConnection> tmpFw = new List<BuoyConnection>();
            List<BuoyConnection> tmpBw = new List<BuoyConnection>();

            if ((buoysDone != null) && !buoysDone.Contains(b))
            {
                b.Station = abs;
                buoysDone.Add(b);
            }

            for (int i = 0; i < connections.Count; i++)
            {
                BuoyConnection c = connections[i];

                if (c == null)
                    continue;

                if (c.StartBuoy == b)
                {
                    tmpFw.Add(c);
                    connections.RemoveAt(i);
                    i--;
                }
                else if (c.EndBuoy == b)
                {
                    tmpBw.Add(c);
                    connections.RemoveAt(i);
                    i--;
                }
            }

            foreach (BuoyConnection c in tmpFw)
                UpdateStation(c.EndBuoy, c.StartBuoy.Station + c.Length, connections, buoysDone);

            foreach (BuoyConnection c in tmpBw)
                UpdateStation(c.StartBuoy, c.EndBuoy.Station - c.Length, connections, buoysDone);
        }
        #endregion
    }

    public class ManualPoint : Marker
    {
        #region Variables
        protected DateTime time = DateTime.Now;
        #endregion

        #region Properties
        public virtual DateTime Time
        {
            get { return time; }
            set { time = value; }
        }

        [Description("The color when displayed."), DefaultValue(KnownColor.Black)]
        public override Color Color
        {
            get { return color; }
            set { color = value; }
        }

        [Description("Toggles the display of the depth text in the record view."), DefaultValue(true), DisplayName("Show Depth"), Category("Text"), LicencedProperty(Module.Modules.V22)]
        public bool ShowTextDepth { get; set; }

        [Description("Toggles the display of the type text in the record view."), DefaultValue(true), DisplayName("Show Type"), Category("Text"), LicencedProperty(Module.Modules.V22)]
        public bool ShowTextType { get; set; }

        [Description("Toggles the display of the description text in the record view."), DefaultValue(true), DisplayName("Show Description"), Category("Text"), LicencedProperty(Module.Modules.V22)]
        public bool ShowTextDesc { get; set; }
        #endregion

        #region Constructor
        public ManualPoint(Coordinate coord, float depth, int type, string desc)
        {
            this.layer = Sonar2DLayers.Marker;
            this.coord = coord;
            this.depth = depth;
            this.type = type;
            this.desc = desc;
            this.time = DateTime.Now;
            this.ShowTextDepth = true;
            this.ShowTextType = true;
            this.ShowTextDesc = true;
        }

        public ManualPoint(Coordinate coord, float depth, int type)
        {
            this.layer = Sonar2DLayers.Marker;
            this.coord = coord;
            this.depth = depth;
            this.type = type;
            this.time = DateTime.Now;
            this.ShowTextDepth = true;
            this.ShowTextType = true;
            this.ShowTextDesc = true;
        }

        public ManualPoint(XmlTextReader reader)
        {
            this.layer = Sonar2DLayers.Marker;

            ReadXml(reader);
        }

        public ManualPoint(ManualPoint pt)
        {
            this.layer = Sonar2DLayers.Marker;
            this.coord = new Coordinate(pt.coord);
            this.depth = pt.depth;
            this.type = pt.type;
            this.time = pt.time;
            this.desc = pt.desc;
            this.tag = pt.Tag;
            this.id = pt.id;
            this.ShowTextDepth = pt.ShowTextDepth;
            this.ShowTextType = pt.ShowTextType;
            this.ShowTextDesc = pt.ShowTextDesc;
            this.Color = pt.Color;
        }

        public ManualPoint(Buoy b)
        {
            this.layer = Sonar2DLayers.Marker;
            this.coord = new Coordinate(b.Coord);
            this.depth = b.Depth;
            this.type = b.Type;
            this.time = DateTime.Now;
            this.desc = b.Description;
            this.tag = b.Tag;
            this.id = b.ID;
            this.ShowTextDepth = true;
            this.ShowTextType = true;
            this.ShowTextDesc = true;
        }

        public ManualPoint(ManualPoint pt, ManualPointTemplate template)
        {
            this.layer = Sonar2DLayers.Marker;
            
            this.coord = new Coordinate(pt.coord);
            this.depth = pt.depth;
            this.type = pt.type;
            this.time = pt.time;
            this.desc = pt.desc;
            this.id = pt.id;
            this.tag = pt.tag;

            this.ShowTextDepth = template.ShowTextDepth;
            this.ShowTextType = template.ShowTextType;
            this.ShowTextDesc = template.ShowTextDesc;
            this.Color = template.Color;
        }

        public ManualPoint()
        {
            // Do nothing.
            this.CoordType = CoordinateType.TransverseMercator;
        }
        #endregion

        #region ToString()
        public override string ToString()
        {
            return "Type " + type.ToString() + ", Depth=" + depth.ToString() + ", RV=" + coord.RV.ToString() + ", HV=" + coord.HV.ToString() + ", AL=" + coord.AL.ToString();
        }
        #endregion

        #region XML r/w
        protected override string GetXmlTag()
        {
            return "manualpoint";
        }

        public override void ReadXml(XmlTextReader reader)
        {
            base.ReadXml(reader);

            if (reader.GetAttribute("time") == null)
                return;

            if (reader.GetAttribute("date") == null)
                return;

            time = DateTime.Parse(reader.GetAttribute("time"));

            DateTime date = DateTime.Parse(reader.GetAttribute("date"));
            time = date.Add(time.TimeOfDay);

            ShowTextDepth = XmlReadConvert.Read(reader, "ShowTextDepth", true);
            ShowTextDesc = XmlReadConvert.Read(reader, "ShowTextDesc", true);
            ShowTextType = XmlReadConvert.Read(reader, "ShowTextType", true);
        }

        public override void WriteAttributes(XmlWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteAttributeString("time", time.ToLongTimeString());
            writer.WriteAttributeString("date", time.ToShortDateString());
            writer.WriteAttributeString("ShowTextDepth", ShowTextDepth.ToString());
            writer.WriteAttributeString("ShowTextDesc", ShowTextDesc.ToString());
            writer.WriteAttributeString("ShowTextType", ShowTextType.ToString());
        }
        #endregion

        #region Paint
        public override void Paint1D(Graphics g, float x, float y, float size, int w, int h)
        {
            // Draw it on the bottom or top depending on its y-axis placement.
            Pen pen = new Pen(this.color, 2);
            int sizeArrowX = 5;
            int sizeArrowY = 10;
            float sizeH = size / 2.0F;
            int hH = h >> 1;

            float yNew;
            float y2; // arrow start
            float y3; // arrow head

            // Create the string.
            string s = "";
            int sLines = 0;
            if (ShowTextType)
            {
                sLines++;
                s += this.type.ToString() + "\n";
            }
            if (ShowTextDesc)
            {
                sLines++;
                s += this.desc + "\n";
            }
            if (ShowTextDepth)
            {
                sLines++;
                s += this.depth.ToString("f3", GSC.Settings.NFI);
            }
            
            StringFormat sf = new StringFormat(StringFormat.GenericDefault);
            Font font = new Font("Tahoma", 9, FontStyle.Bold);
            SizeF strSize = g.MeasureString(s, font);

            if (y < hH)
            {
                yNew = h - size;
                y2 = yNew - sizeH;
                y3 = y + sizeArrowY;
            }
            else
            {
                yNew = size;
                y2 = yNew + sizeH;
                y3 = y - sizeArrowY;
                sf.Alignment = StringAlignment.Far;
            }

            // Draw an arrow.
            g.DrawLine(pen, x, yNew, x, y);
            g.DrawLine(pen, x - sizeArrowX, y3, x, y);
            g.DrawLine(pen, x + sizeArrowX, y3, x, y);

            // Draw the type and depth string.
            Matrix t = g.Transform;
            g.TranslateTransform(x, yNew);
            g.RotateTransform(-90);
            g.DrawString(s, font, pen.Brush, 0, -strSize.Height * (float)(sLines - 1.0F) / (float)sLines, sf);
            g.Transform = t;

            pen.Dispose();
            font.Dispose();
        }
        #endregion
    }

    [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    public class ManualPointTemplate
    {
        #region Properties
        [XmlIgnore]
        [Description("The color when displayed."), DefaultValue(KnownColor.Black)]
        public Color Color { get; set; }
        [Browsable(false), DefaultValue("Black")]
        public string XmlColor
        {
            get { return SerializeColor.Serialize(Color); }
            set { Color = SerializeColor.Deserialize(value); }
        }

        [Description("Toggles the display of the depth text in the record view."), DefaultValue(true), DisplayName("Show Depth"), Category("Text"), LicencedProperty(Module.Modules.V22)]
        public bool ShowTextDepth { get; set; }

        [Description("Toggles the display of the type text in the record view."), DefaultValue(true), DisplayName("Show Type"), Category("Text"), LicencedProperty(Module.Modules.V22)]
        public bool ShowTextType { get; set; }

        [Description("Toggles the display of the description text in the record view."), DefaultValue(true), DisplayName("Show Description"), Category("Text"), LicencedProperty(Module.Modules.V22)]
        public bool ShowTextDesc { get; set; }
        #endregion

        #region Constructors
        public ManualPointTemplate(ManualPointTemplate template)
        {
            this.ShowTextDepth = template.ShowTextDepth;
            this.ShowTextType = template.ShowTextType;
            this.ShowTextDesc = template.ShowTextDesc;
            this.Color = template.Color;
        }

        public ManualPointTemplate()
        {
            this.ShowTextDepth = true;
            this.ShowTextType = true;
            this.ShowTextDesc = true;
            this.Color = Color.Black;
        } 
        #endregion
    }
    #endregion

    #region BuoyConnection
    public class BuoyConnection : Sonar2DElement
    {
        public BuoyConnection()
        {
            this.layer = Sonar2DLayers.Marker;
        }

        #region Variables
        private Buoy startBuoy = null;
        private Buoy endBuoy = null;

        private int width = 3;
        private byte alphaChannel = 127;

        private double corridor = 0.0;
        private double catchWidth = 0.0;
        private Color highlightColor = Color.Red;

        private bool highlighted = false;

        private DA040ElementType type = DA040ElementType.Line;
        private double radius = 0.0;
        private double parameter = 0.0;
        private PointD center = new PointD(0, 0);
        private double startAngle = 0.0;
        private double endAngle = 0.0;
        private double sweepAngle = 0.0;

        private double length = 0.0;
        #endregion

        #region Properties
        [Description("The first buoy."), Category("Buoys"), DefaultValue(null), DisplayName("Start Buoy")]
        public Buoy StartBuoy
        {
            get { return startBuoy; }
            set { startBuoy = value; UpdateShape(); }
        }

        [Description("The second buoy."), Category("Buoys"), DefaultValue(null), DisplayName("End Buoy")]
        public Buoy EndBuoy
        {
            get { return endBuoy; }
            set { endBuoy = value; UpdateShape(); }
        }

        [Description("The width of the line."), DefaultValue(3), DisplayName("Line Width")]
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        [Description("The width of the corridor. The corridor will be centered above the connection. Use a 0 for just a line."), Category("Corridor"), DefaultValue(0.0), DisplayName("Corridor Width")]
        public double Corridor
        {
            get { return corridor; }
            set
            {
                corridor = Math.Abs(value);

                if (catchWidth <= corridor)
                    catchWidth = 2.0 * corridor;
            }
        }

        [Description("The width of the catch corridor. The catch corridor will be centered above the connection. This width must be bigger than corridor width."), Category("Corridor"), DefaultValue(0.0), DisplayName("Catch Corridor Width")]
        public double CatchWidth
        {
            get { return catchWidth; }
            set
            {
                catchWidth = Math.Abs(value);

                if (catchWidth <= corridor)
                    catchWidth = 2.0 * corridor;
            }
        }

        [Description("The transparency of the corridor. Values range from 0 (maximum) to 255 (minimum)."), Category("Corridor"), DefaultValue((byte)127), DisplayName("Transparency")]
        public byte AlphaChannel
        {
            get { return alphaChannel; }
            set { alphaChannel = value; }
        }

        [Description("The fill color for the highlighted state."), DefaultValue(KnownColor.Red), DisplayName("Highlight Color")]
        public Color HighlightColor
        {
            get { return highlightColor; }
            set { highlightColor = value; }
        }

        [Browsable(false)]
        public bool Highlighted
        {
            get { return highlighted; }
            set { highlighted = value; }
        }

        [Description("The type of the connection. Clothoids are not supported yet!"), DefaultValue(DA040ElementType.Line), Category("Type"), LicencedProperty(Module.Modules.V22)]
        public DA040ElementType Type
        {
            get { return type; }
            set
            {
                if (value == DA040ElementType.Clothoid)
                    MessageBox.Show("This type is not supported yet!", "Command not accepted.");
                else if ((value == DA040ElementType.Arc) && (radius == 0.0))
                    MessageBox.Show("Specify a radius first!", "Command not accepted.");
                else
                {
                    type = value;
                    UpdateShape();
                }
            }
        }

        [Description("The radius of arc typed connections."), DefaultValue(0.0), Category("Type"), LicencedProperty(Module.Modules.V22)]
        public double Radius
        {
            get { return radius; }
            set
            {
                radius = value;

                if (radius == 0.0)
                    type = DA040ElementType.Line;

                UpdateShape();
            }
        }

        [Description("The parameter A of clothoid typed connections."), DefaultValue(0.0), Category("Type"), LicencedProperty(Module.Modules.V22)]
        public double Parameter
        {
            get { return parameter; }
            set { parameter = value; UpdateShape(); }
        }

        [Description("The length of the connection."), DefaultValue(0.0), Category("Type"), LicencedProperty(Module.Modules.V22)]
        public double Length
        {
            get
            {
                switch (type)
                {
                    case DA040ElementType.Line:
                        if ((startBuoy != null) && (endBuoy != null))
                            length = startBuoy.Coord.Point.Distance(endBuoy.Coord.Point);
                        break;

                    case DA040ElementType.Arc:
                        length = Math.PI * radius * sweepAngle / 180.0;
                        break;

                    case DA040ElementType.Clothoid:
                        // tbd
                        break;
                }

                return length;
            }
        }

        [Description("The station of the start buoy."), DefaultValue(0.0), Category("Type"), LicencedProperty(Module.Modules.V22)]
        public double Station
        {
            get { return (startBuoy != null) ? startBuoy.Station : 0.0; }
        }

        public void SetStation(double station)
        {
            startBuoy.Station = station;
        }
        #endregion

        #region UpdateShape
        private void UpdateShape()
        {
            if ((startBuoy == null) || (endBuoy == null))
                return;

            if (type == DA040ElementType.Arc)
            {
                if (radius == 0.0)
                    return;

                // Get start (A) and end (B) point and calculate mid point
                PointD ptA = startBuoy.GetPointRVHV();
                PointD ptB = endBuoy.GetPointRVHV();
                PointD ptM = (ptA + ptB) / 2.0;

                // Calculate normalized 2D direction vector ptM->ptA
                PointD ptV = ptA - ptM;
                PointD ptVn = ptV / ptV.Length;

                // Calculate new direction, multiply with radius and add mid point
                PointD ptC = new PointD(-ptVn.Y, ptVn.X) * Math.Sqrt(radius * radius - ptV.Length * ptV.Length) * (radius < 0 ? -1.0 : 1.0) + ptM;
                center = ptC;

                // Calculate start and sweep angle
                startAngle = -ptC.AngleDeg(ptA);
                endAngle = -ptC.AngleDeg(ptB);

                sweepAngle = endAngle - startAngle;

                if ((startAngle > 90.0) && (endAngle < -90.0))
                    sweepAngle += 360.0;
                else if ((startAngle < -90.0) && (endAngle > 90.0))
                    sweepAngle -= 360.0;
            }
        }
        #endregion

        #region Paint
        public override void Paint(Graphics g, RectangleD rcRegion, double scaleFactor)
        {
            PointD ptStart = this.startBuoy.GetPointRVHV().Offset(-rcRegion.Center.X, -rcRegion.Center.Y);
            PointD ptEnd = this.endBuoy.GetPointRVHV().Offset(-rcRegion.Center.X, -rcRegion.Center.Y);

            float penSize = (float)(1.0 / scaleFactor);
            Pen pen = new Pen(this.color, (float)((double)this.width / scaleFactor));
            Pen penHighlight = new Pen(highlighted ? highlightColor : color, penSize);

            PointF[] pt;

            double d = ptStart.Distance(ptEnd);
            double angle = ptStart.AngleDeg(ptEnd);

            double buoySize = GSC.Settings.BuoySize;
            if (GSC.Settings.BuoySizeUnit == Buoy.BuoySizeUnit.Pixels)
                buoySize /= scaleFactor;

            float buoySizeH = (float)(buoySize / 2.0);
            float corrH = (float)(corridor / 2.0);
            float x;

            if (d < buoySize)
                return;

            // Calculate major and minor grid ticks
            double sections = (double)GSC.Settings.BuoyConnectionGridDiv + 1.0;
            double grid = (double)GSC.Settings.BuoyConnectionGrid;
            bool drawMinor = grid / sections * scaleFactor > (double)(this.width + 1);
            int start = 0;
            int end = -1;
            int i;

            if (GSC.Settings.Lic[Module.Modules.V22] && (grid > 0.0))
            {
                start = (int)Math.Ceiling(this.Station / grid * sections);
                end = (int)Math.Floor((this.Station + this.Length) / grid * sections);
            }

            // Save the last transform
            Matrix t = g.Transform;

            // Draw
            switch (type)
            {
                case DA040ElementType.Line:
                    g.TranslateTransform((float)ptStart.X, (float)ptStart.Y);
                    g.RotateTransform((float)angle);

                    g.DrawLine(pen, buoySizeH, 0.0F, (float)d - buoySizeH, 0.0F);

                    if (corridor > 0)
                    {
                        g.DrawRectangle(penHighlight, 0, -corrH, (float)d, (float)corridor);
                        g.FillRectangle(new SolidBrush(Color.FromArgb(alphaChannel, highlighted ? highlightColor : color)), 0, -corrH, (float)d, (float)corridor);

                        pt = new PointF[3];
                        pt[0] = new PointF((float)(d / 2.0) - corrH, -corrH);
                        pt[1] = new PointF((float)(d / 2.0) - corrH, corrH);
                        pt[2] = new PointF((float)(d / 2.0) + corrH, 0.0F);

                        g.FillPolygon(new SolidBrush(highlighted ? highlightColor : color), pt);

                        if (GSC.Settings.Lic[Module.Modules.V22])
                            for (i = start; i <= end; i++)
                            {
                                x = (float)((double)i * grid / sections - this.Station);
                                if (i % sections == 0)
                                    g.DrawLine(pen, x, -corrH / 2.0F, x, corrH / 2.0F);
                                else if (drawMinor)
                                    g.DrawLine(pen, x, -corrH / 4.0F, x, corrH / 4.0F);
                            }
                    }
                    break;

                case DA040ElementType.Arc:
                    PointD ptC = center.Offset(-rcRegion.Center.X, -rcRegion.Center.Y);
                    float r = (float)(ptC - ptStart).Length;
                    double off = Math.Asin(buoySizeH / r) * 180.0 / Math.PI * ((sweepAngle < 0) ? 1.0 : -1.0);
                    double sAngle = -startAngle - sweepAngle;

                    g.TranslateTransform((float)ptC.X, (float)ptC.Y);
                    g.RotateTransform((float)sAngle);

                    g.DrawArc(pen, -r, -r, r * 2.0F, r * 2.0F, -(float)off, (float)(sweepAngle + 2.0 * off));

                    if (corridor > 0)
                    {
                        GraphicsPath path = new GraphicsPath();
                        path.AddArc(-(r + corrH), -(r + corrH), r * 2.0F + (float)corridor, r * 2.0F + (float)corridor, 0, (float)sweepAngle);
                        path.AddArc(-(r - corrH), -(r - corrH), r * 2.0F - (float)corridor, r * 2.0F - (float)corridor, (float)sweepAngle, (float)-sweepAngle);

                        g.DrawPath(penHighlight, path);
                        g.FillPath(new SolidBrush(Color.FromArgb(alphaChannel, highlighted ? highlightColor : color)), path);

                        if (GSC.Settings.Lic[Module.Modules.V22])
                        {
                            Matrix t2 = g.Transform;

                            for (i = start; i <= end; i++)
                            {
                                g.RotateTransform((float)((i * grid / sections - this.Station) * sweepAngle / length));

                                if (i % GSC.Settings.BuoyConnectionGridDiv == 0)
                                    g.DrawLine(pen, r - corrH / 2.0F, 0, r + corrH / 2.0F, 0);
                                else if (drawMinor)
                                    g.DrawLine(pen, r - corrH / 4.0F, 0, r + corrH / 4.0F, 0);

                                g.Transform = t2;
                            }
                        }
                    }
                    break;
            }

            // Restore old transformation matrix
            g.Transform = t;
        }
        #endregion

        #region Corridor
        public PointD NormalizeToCorridor(PointD pt)
        {
            PointD ptNew = new PointD();

            switch (type)
            {
                case DA040ElementType.Line:
                    LineD line = new LineD(startBuoy.Coord.Point - pt, endBuoy.Coord.Point - pt);

                    double d = -line.OriginDistance();
                    double a = line.OriginAngleRad();

                    ptNew = new PointD(d * Math.Cos(a), d * Math.Sin(a)) + pt;
                    break;

                case DA040ElementType.Arc:
                    pt -= center;

                    ptNew = pt / pt.Length * Math.Abs(radius) + center;
                    break;

                case DA040ElementType.Clothoid:
                    // tbd
                    break;
            }

            return ptNew;
        }

        public Coordinate NormalizeToCorridor(Coordinate coord)
        {
            PointD pt = coord.Point;
            PointD ptNew = new PointD();

            switch (type)
            {
                case DA040ElementType.Line:
                    LineD line = new LineD(startBuoy.Coord.Point - pt, endBuoy.Coord.Point - pt);

                    double d = -line.OriginDistance();
                    double a = line.OriginAngleRad();

                    ptNew = new PointD(d * Math.Cos(a), d * Math.Sin(a)) + pt;
                    break;

                case DA040ElementType.Arc:
                    pt -= center;

                    ptNew = pt / pt.Length * Math.Abs(radius) + center;
                    break;

                case DA040ElementType.Clothoid:
                    // tbd
                    break;
            }

            coord.RV = ptNew.X;
            coord.HV = ptNew.Y;

            return coord;
        }

        public double GetCorridorDistance(PointD pt)
        {
            double distance = 0.0;

            switch (type)
            {
                case DA040ElementType.Line:
                    LineD line = new LineD(startBuoy.Coord.Point, endBuoy.Coord.Point);

                    distance = line.PointDistance(pt);
                    break;

                case DA040ElementType.Arc:
                    pt -= center;

                    distance = pt.Length - Math.Abs(radius);

                    if (radius > 0)
                        distance = -distance;
                    break;

                case DA040ElementType.Clothoid:
                    // tbd
                    break;
            }

            return distance;
        }

        public double GetStation(PointD pt)
        {
            double ptStation = 0.0;

            switch (type)
            {
                case DA040ElementType.Line:
                    if ((startBuoy == null) || (endBuoy == null))
                        return 0.0;

                    PointD ptN = NormalizeToCorridor(pt);

                    ptStation = startBuoy.Coord.Point.Distance(ptN);

                    if (endBuoy.Coord.Point.Distance(ptN) > length)
                        ptStation = -ptStation;

                    ptStation += startBuoy.Station;
                    break;

                case DA040ElementType.Arc:
                    ptStation = startBuoy.Station + Math.PI * radius * GetArcAngle(pt) / 180.0;
                    break;

                case DA040ElementType.Clothoid:
                    // tbd
                    break;
            }

            return ptStation;
        }

        public double GetNearestBuoyDistance(PointD pt)
        {
            // tbd: Type switch needed??? -> To be determined!

            double dB1 = pt.Distance(startBuoy.Coord.Point);
            double dB2 = pt.Distance(endBuoy.Coord.Point);

            return (dB1 < dB2) ? dB1 : dB2;
        }

        public double GetBiggestAngle(PointD pt)
        {
            PointD pt1 = startBuoy.Coord.Point;
            PointD pt2 = endBuoy.Coord.Point;

            double a1 = 0;
            double a2 = 0;

            switch (type)
            {
                case DA040ElementType.Line:
                    a1 = GetAngle(pt1, pt, pt2);
                    a2 = GetAngle(pt2, pt1, pt);
                    break;

                case DA040ElementType.Arc:
                    PointD pt1t = (center - pt1);
                    pt1t = new PointD(-pt1t.Y, pt1t.X) * radius + pt1;

                    PointD pt2t = (center - pt2);
                    pt2t = new PointD(pt2t.Y, -pt2t.X) * radius + pt2;

                    a1 = GetAngle(pt1, pt, pt1t);
                    a2 = GetAngle(pt2, pt2t, pt);
                    break;

                case DA040ElementType.Clothoid:
                    // tbd
                    break;
            }

            return (a1 > a2) ? a1 : a2;
        }

        private double GetAngle(PointD ptRef, PointD pt1, PointD pt2)
        {
            double a1 = ptRef.AngleDeg(pt1);
            double a2 = ptRef.AngleDeg(pt2);

            if (a1 < 0)
                a1 += 360.0;
            if (a2 < 0)
                a2 += 360.0;

            double ret = Math.Abs(a1 - a2);

            if (ret > 180.0)
                ret = 360.0 - ret;

            return ret;
        }

        private double GetArcAngle(PointD pt)
        {
            if (type != DA040ElementType.Arc)
                return 0.0;

            double angle = -center.AngleDeg(pt);
            double diffAngle = angle - startAngle;

            if ((startAngle > 90.0) && (angle < -90.0))
                diffAngle += 360.0;
            else if (startAngle < -90.0 && angle > 90)
                diffAngle -= 360.0;

            return diffAngle;
        }

        public bool IsInCorridor(PointD pt)
        {
            double angle = 0.0;
            bool ret = false;

            switch (type)
            {
                case DA040ElementType.Line:
                    angle = GetBiggestAngle(pt);
                    ret = (Math.Abs(GetCorridorDistance(pt)) <= corridor / 2.0) & ((angle <= 90.0) | (angle >= 270));
                    break;

                case DA040ElementType.Arc:
                    angle = GetArcAngle(pt);

                    pt -= center;

                    if ((pt.Length > Math.Abs(radius) + corridor / 2.0) ||
                        (pt.Length < Math.Abs(radius) - corridor / 2.0))
                        break;

                    if (sweepAngle > 0)
                        ret = (angle >= 0) && (angle <= sweepAngle);
                    else
                        ret = (angle <= 0) && (angle >= sweepAngle);
                    break;

                case DA040ElementType.Clothoid:
                    // tbd
                    break;
            }

            return ret;
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return desc.ToString();
        }
        #endregion
    }
    #endregion
}