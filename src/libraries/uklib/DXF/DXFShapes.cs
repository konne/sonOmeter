using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.ObjectModel;
using System.IO;
using UKLib.MathEx;
using System.Collections.Generic;
using UKLib.Strings;

namespace UKLib.DXF
{
    #region INSERT
    public class DXFinsert : DXFEntity
    {
        public string BlockName { get; protected set; }
        public double Rotation { get; protected set; }
        public double RowSpacing { get; protected set; }
        public double ColumnSpacing { get; protected set; }
        public Int16 RowCount { get; protected set; }
        public Int16 ColumnCount { get; protected set; }

        public DXFblock Block { get; set; }

        protected Point3D insertPoint = new Point3D(0, 0, 0);
        protected Point3D scaleFactor = new Point3D(0, 0, 0);

        public Point3D InsertPoint
        {
            get { return insertPoint; }
            set { insertPoint = value; }
        }

        public Point3D ScaleFactor
        {
            get { return scaleFactor; }
            set { scaleFactor = value; }
        }

        public DXFinsert()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            BlockName = "";
            insertPoint = new Point3D(0, 0, 0);
            scaleFactor = new Point3D(0, 0, 0);
            Rotation = 0;
            RowCount = 1;
            ColumnCount = 1;
            RowSpacing = 0;
            ColumnSpacing = 0;

            Type = "INSERT";

            Block = null;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "2":
                    BlockName = group.Data;
                    break;
                case "10":
                    insertPoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "20":
                    insertPoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "30":
                    insertPoint.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "41":
                    scaleFactor.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "42":
                    scaleFactor.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "43":
                    scaleFactor.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "50":
                    Rotation = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "44":
                    ColumnSpacing = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "45":
                    RowSpacing = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "70":
                    ColumnCount = Convert.ToInt16(group.Data, DXFContainer.NFI);
                    break;
                case "71":
                    RowCount = Convert.ToInt16(group.Data, DXFContainer.NFI);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            if (Block == null)
                return;

            ptOffset += insertPoint.PointXY;

            System.Drawing.Drawing2D.Matrix t = g.Transform;

            if (Rotation != 0)
            {
                PointF pt = (ptOffset - rcRegion.Center).PointF;

                g.TranslateTransform(pt.X, pt.Y);
                g.RotateTransform((float)Rotation);
                g.TranslateTransform(-pt.X, -pt.Y);
            }

            for (int x = 0; x < ColumnCount; x++)
            {
                for (int y = 0; y < RowCount; y++)
                {
                    Block.Draw(g, rcRegion, scale, skipDetail, ptOffset.Offset(x * ColumnSpacing, y * RowSpacing));
                }
            }

            if (Rotation != 0)
                g.Transform = t;
        }

        public override void Update(Pen pen, Pen penBlock, Pen penLayer)
        {
            //base.Update(pen, penBlock, penLayer);
            penBlock = ownPen.Clone() as Pen;
            
            if (Block == null)
                return;

            for (int x = 0; x < ColumnCount; x++)
            {
                for (int y = 0; y < RowCount; y++)
                {
                    Block.Update(pen, penBlock, penLayer);
                }
            }
        }
    }
    #endregion

    #region LINE
    public class DXFline : DXFEntity
    {
        // TODO: DXF Extrusion direction (group code 210)
        public double Thickness { get; protected set; }

        protected Point3D startPoint = new Point3D(0, 0, 0);
        protected Point3D endPoint = new Point3D(0, 0, 0);

        public Point3D StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; }
        }

        public Point3D EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; }
        }
        
        public DXFline()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Thickness = 0;

            Type = "LINE";

            startPoint = new Point3D(0, 0, 0);
            endPoint = new Point3D(0, 0, 0);
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "10":
                    startPoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "20":
                    startPoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "30":
                    startPoint.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "11":
                    endPoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "21":
                    endPoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "31":
                    endPoint.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "39":
                    Thickness = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override void Draw(GraphicsPath path, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            ptStart = startPoint.PointXY + ptOffset;
            ptEnd = endPoint.PointXY + ptOffset;

            rcBoundary = new RectangleD(ptStart, ptEnd);
            
            if (!rcRegion.Contains(rcBoundary))
                return;
            if (rcBoundary.Diag < skipDetail)
                return;

            path.AddLine((ptStart - rcRegion.Center).PointF, (ptEnd - rcRegion.Center).PointF);
        }
    }
    #endregion

    #region LWPOLYLINE
    public class DXFlwpolyline : DXFEntity
    {
        // TODO: DXF Extrusion direction (group code 210)
        public double Thickness { get; protected set; }
        public double Width { get; protected set; }
        public bool Closed { get; set; }

        protected List<DXFvertex> pointList = new List<DXFvertex>();

        public List<DXFvertex> PointList
        {
            get { return pointList; }
            set { pointList = value; }
        }

        public DXFlwpolyline()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Thickness = 0;
            Closed = false;

            Type = "LWPOLYLINE";

            pointList = new List<DXFvertex>();
        }

        private bool foundX = false;
        private bool foundY = false;
        private bool foundWS = false;
        private bool foundWE = false;
        private bool foundB = false;

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            double elevation = 0.0;

            switch (group.Code)
            {
                #region Iterative stuff (point list)
                case "10":
                    if (foundX || (pointList.Count == 0))
                    {
                        #region Add new point to list.
                        foundX = false;
                        foundY = false;
                        foundWS = false;
                        foundWE = false;
                        foundB = false;

                        pointList.Add(new DXFvertex());
                        #endregion
                    }
                    foundX = true;
                    pointList[pointList.Count - 1].StartPoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    pointList[pointList.Count - 1].StartPoint.Z = elevation;
                    break;
                case "20":
                    if (foundY || (pointList.Count == 0))
                    {
                        #region Add new point to list.
                        foundX = false;
                        foundY = false;
                        foundWS = false;
                        foundWE = false;
                        foundB = false;

                        pointList.Add(new DXFvertex());
                        #endregion
                    }
                    foundY = true;
                    pointList[pointList.Count - 1].StartPoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    pointList[pointList.Count - 1].StartPoint.Z = elevation;
                    break;
                case "40":
                    if (foundWS || (pointList.Count == 0))
                    {
                        #region Add new point to list.
                        foundX = false;
                        foundY = false;
                        foundWS = false;
                        foundWE = false;
                        foundB = false;

                        pointList.Add(new DXFvertex());
                        #endregion
                    }
                    foundWS = true;
                    pointList[pointList.Count - 1].WidthStart = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "41":
                    if (foundWE || (pointList.Count == 0))
                    {
                        #region Add new point to list.
                        foundX = false;
                        foundY = false;
                        foundWS = false;
                        foundWE = false;
                        foundB = false;

                        pointList.Add(new DXFvertex());
                        #endregion
                    }
                    foundWE = true;
                    pointList[pointList.Count - 1].WidthEnd = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "42":
                    if (foundB || (pointList.Count == 0))
                    {
                        #region Add new point to list.
                        foundX = false;
                        foundY = false;
                        foundWS = false;
                        foundWE = false;
                        foundB = false;

                        pointList.Add(new DXFvertex());
                        #endregion
                    }
                    foundB = true;
                    pointList[pointList.Count - 1].Bulge = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break; 
                #endregion

                case "43":
                    Width = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    hairLine = false;
                    break;
                case "38":
                    elevation = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    foreach (DXFvertex v in pointList)
                        v.StartPoint.Offset(0, 0, elevation);
                    break;
                case "39":
                    Thickness = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "70":
                    Closed = ((group.Data == "1") || (group.Data == "129"));
                    break;
                case "90":
                    // Number of vertices - N/A (dynamic list)
                    break;
                default:
                    return false;
            }

            return true;
        }

        internal override void EndRead()
        {
            base.EndRead();

            // Link all points so that they can draw themselves.
            int max = pointList.Count - 1;

            for (int i = 0; i < max; i++)
            {
                pointList[i].SetNext(pointList[i + 1].StartPoint);
            }

            if (Closed)
                pointList[max].SetNext(pointList[0].StartPoint);
        }

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            int max = pointList.Count - 1;
            
            GraphicsPath path = new GraphicsPath();
            this.Draw(path, rcRegion, scale, skipDetail, ptOffset);
            g.DrawPath(ownPen, path);
        }

        public override void Draw(GraphicsPath path, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            int max = pointList.Count - 1;

            path.StartFigure();

            for (int i = 0; i < max; i++)
                pointList[i].Draw(path, rcRegion, scale, skipDetail, ptOffset);

            if (Closed)
                path.CloseFigure();
        }

        public override void Update(Pen pen, Pen penBlock, Pen penLayer)
        {
            base.Update(pen, penBlock, penLayer);
            ownPen.LineJoin = LineJoin.Miter;

            if (Width > 0)
                ownPen.Width = (float)Width;
        }
    }
    #endregion

    #region VERTEX
    public class DXFvertex : DXFEntity
    {
        public Point3D StartPoint = new Point3D();
        protected Point3D EndPoint = new Point3D();
        public double WidthStart = 0.0;
        public double WidthEnd = 0.0;
        public double Bulge = 0.0;
        protected Point3D BulgeCenter = new Point3D();
        protected double BulgeRadius = 0.0;
        protected double BulgeStartAngle = 0.0;
        protected double BulgeEndAngle = 0.0;
        protected double BulgeSweepAngle = 0.0;

        public DXFvertex()
        {
            Init();
        }

        public DXFvertex(double x, double y, double z)
        {
            Init();

            StartPoint = new Point3D(x, y, z);
        }

        protected override void Init()
        {
            base.Init();
            
            Type = "VERTEX";
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "10":
                    StartPoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "20":
                    StartPoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "30":
                    StartPoint.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "40":
                    WidthStart = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "41":
                    WidthEnd = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "42":
                    Bulge = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "50":
                    // Curve fit tangent direction
                    break;
                case "70":
                    // 1 = Extra vertex created by curve-fitting
                    // 2 = Curve-fit tangent defined for this vertex. A curve-fit tangent direction of 0 may be omitted
                    //     from DXF output but is significant if this bit is set
                    // 4 = Not used
                    // 8 = Spline vertex created by spline-fitting
                    // 16 = Spline frame control point
                    // 32 = 3D polyline vertex
                    // 64 = 3D polygon mesh
                    // 128 = Polyface mesh vertex
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override void Draw(GraphicsPath path, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            if (Bulge == 0.0)
            {
                PointD ptStart = StartPoint.PointXY + ptOffset;
                PointD ptEnd = EndPoint.PointXY + ptOffset;

                RectangleD rcBoundary = new RectangleD(ptStart, ptEnd);

                if (!rcRegion.Contains(rcBoundary))
                    return;
                if (rcBoundary.Diag < skipDetail)
                    return;

                path.AddLine((ptStart - rcRegion.Center).PointF, (ptEnd - rcRegion.Center).PointF);
            }
            else
            {
                PointD ptStart = (BulgeCenter.PointXY + ptOffset).Offset(-BulgeRadius, -BulgeRadius);
                PointD ptEnd = (BulgeCenter.PointXY + ptOffset).Offset(BulgeRadius, BulgeRadius);

                RectangleD rcBoundary = new RectangleD(ptStart, ptEnd);
                if (!rcRegion.Contains(rcBoundary))
                    return;
                if (rcBoundary.Diag < skipDetail)
                    return;

                ptStart -= rcRegion.Center;
                ptEnd -= rcRegion.Center;

                path.AddArc((float)ptStart.X, (float)ptStart.Y, (float)(2.0 * BulgeRadius), (float)(2.0 * BulgeRadius), (float)BulgeStartAngle, (float)BulgeSweepAngle);
            }
        }

        public void SetNext(Point3D pt)
        {
            EndPoint = pt;

            if (Bulge == 0.0)
                return;

            double dx = EndPoint.X - StartPoint.X;
            double dy = EndPoint.Y - StartPoint.Y;
            double xmid = dx / 2 + StartPoint.X;
            double ymid = dy / 2 + StartPoint.Y;
            double l = Math.Sqrt(dx * dx + dy * dy);
            BulgeRadius = Math.Abs(l * (Bulge * Bulge + 1) / Bulge / 4);

            double a = Math.Abs(Bulge * l / 2.0);
            double sb = Bulge / Math.Abs(Bulge); //sign of bulge
            double theta_p = 4.0 * Math.Atan(Bulge);
            double theta_c = (dx != 0.0 ? Math.Atan(dy / dx) : Math.PI / 2.0);
            if (dx > 0.0) sb *= -1.0; // Correct for different point ordering and bulge direction

            double cx = xmid + sb * (BulgeRadius - a) * Math.Sin(theta_c);
            double cy = ymid - sb * (BulgeRadius - a) * Math.Cos(theta_c);

            BulgeCenter = new Point3D(cx, cy, StartPoint.Z);

            // Now calculate the angles
            //BulgeStartAngle = Math.Asin(StartPoint.X / BulgeRadius) * 180.0 / Math.PI - 90.0;
            BulgeStartAngle = BulgeCenter.PointXY.AngleDeg(StartPoint.PointXY);
            //if (dy < 0)
            //    BulgeStartAngle = 360.0 - BulgeStartAngle;  // The angle is greater than pi so fix this because max(asin) = pi

            //BulgeEndAngle = Math.Asin(EndPoint.X / BulgeRadius) * 180.0 / Math.PI - 90.0;
            BulgeEndAngle = BulgeCenter.PointXY.AngleDeg(EndPoint.PointXY);
            //if (dy < 0)
            //    BulgeEndAngle = 360.0 - BulgeEndAngle;  // The angle is greater than pi so fix this because max(asin) = pi

            BulgeSweepAngle = BulgeEndAngle - BulgeStartAngle;

            if ((BulgeStartAngle > 90.0) && (BulgeEndAngle < -90.0))
                BulgeSweepAngle += 360.0;
            else if ((BulgeStartAngle < -90.0) && (BulgeEndAngle > 90.0))
                BulgeSweepAngle -= 360.0;

            //if (BulgeSweepAngle < 0)
            //    BulgeSweepAngle += 360;
        }
    }
    #endregion

    #region POLYLINE
    public class DXFpolyline : DXFEntity
    {
        // TODO: DXF Extrusion direction (group code 210)
        public double Elevation { get; protected set; }
        public double Thickness { get; protected set; }
        public bool Closed { get; protected set; }

        protected List<DXFvertex> pointList = new List<DXFvertex>();

        public List<DXFvertex> PointList
        {
            get { return pointList; }
            set { pointList = value; }
        }

        public DXFpolyline()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Elevation = 0;
            Thickness = 0;
            Closed = false;

            Type = "POLYLINE";

            pointList = new List<DXFvertex>();
        }

        public override DXFGroup Read(System.IO.StreamReader reader)
        {
            DXFvertex vPrev = null;
            DXFGroup group;

            // Read own data.
            group = base.Read(reader);

            // Read until no more VERTEX data appears.
            while (group.Data == "VERTEX")
            {
                DXFvertex v = new DXFvertex();
                group = v.Read(reader);
                pointList.Add(v);

                if (vPrev != null)
                    vPrev.SetNext(v.StartPoint);
                vPrev = v;
            }

            if (Closed && (pointList.Count > 1))
                vPrev.SetNext(pointList[0].StartPoint);

            // Return next type specifier.
            return group;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            int code = 0;

            switch (group.Code)
            {
                case "30":
                    Elevation = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    /*foreach (DXFvertex v in pointList)
                        v.StartPoint.Offset(0, 0, Elevation);*/
                    break;
                case "40":
                    // Default start width
                    break;
                case "41":
                    // Default end width
                    break;
                case "70":
                    int.TryParse(group.Data, out code);
                    if ((code & 1) != 0)
                        this.Closed = true;
                    // 1 = This is a closed polyline (or a polygon mesh closed in the M direction)
                    // 2 = Curve-fit vertices have been added
                    // 4 = Spline-fit vertices have been added
                    // 8 = This is a 3D polyline
                    // 16 = This is a 3D polygon mesh
                    // 32 = The polygon mesh is closed in the N direction
                    // 64 = The polyline is a polyface mesh
                    // 128 = The linetype pattern is generated continuously around the vertices of this polyline
                    break;
                case "71":
                    // Polygon mesh M vertex count (optional; default = 0)
                    break;
                case "72":
                    // Polygon mesh N vertex count (optional; default = 0)
                    break;
                case "73":
                    // Smooth surface M density (optional; default = 0)
                    break;
                case "74":
                    // Smooth surface N density (optional; default = 0)
                    break;
                case "75":
                    // Curves and smooth surface type (optional; default = 0); integer codes, not bit-coded:
                    // 0 = No smooth surface fitted
                    // 5 = Quadratic B-spline surface
                    // 6 = Cubic B-spline surface
                    // 8 = Bezier surface
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            int max = pointList.Count - 1;

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();

            for (int i = 0; i < max; i++)
                pointList[i].Draw(path, rcRegion, scale, skipDetail, ptOffset);

            if (Closed)
                path.CloseFigure();

            g.DrawPath(ownPen, path);
        }

        public override void Update(Pen pen, Pen penBlock, Pen penLayer)
        {
            base.Update(pen, penBlock, penLayer);
            ownPen.LineJoin = LineJoin.Miter;

            int max = pointList.Count;

            for (int i = 0; i < max; i++)
                pointList[i].Update(pen, penBlock, penLayer);
        }
    }
    #endregion

    #region CIRCLE
    public class DXFcircle : DXFEntity
    {
        public double Radius { get; set; }

        protected Point3D centerPoint = new Point3D(0, 0, 0);

        public Point3D CenterPoint
        {
            get { return centerPoint; }
            set { centerPoint = value; }
        }

        public DXFcircle()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Type = "CIRCLE";

            centerPoint = new Point3D(0, 0, 0);
            Radius = 1;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "10":
                    centerPoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "20":
                    centerPoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "30":
                    centerPoint.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "40":
                    Radius = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override void Draw(GraphicsPath path, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            PointD ptStart = (centerPoint.PointXY + ptOffset).Offset(-Radius, -Radius);
            PointD ptEnd = (centerPoint.PointXY + ptOffset).Offset(Radius, Radius);

            RectangleD rcBoundary = new RectangleD(ptStart, ptEnd);
            if (!rcRegion.Contains(rcBoundary))
                return;
            if (rcBoundary.Diag < skipDetail)
                return;

            ptStart -= rcRegion.Center;
            ptEnd -= rcRegion.Center;

            path.AddEllipse((float)ptStart.X, (float)ptStart.Y, (float)(2.0 * Radius), (float)(2.0 * Radius));
        }
    }
    #endregion

    #region ELLIPSE
    public class DXFellipse : DXFcircle
    {
        protected Point3D majorAxis = new Point3D(0, 0, 0);

        public Point3D MajorAxis
        {
            get { return majorAxis; }
            set { majorAxis = value; }
        }

        public double StartAngle { get; set; }
        public double EndAngle { get; set; }

        public bool IsCCW { get; set; }

        private double sweepAngle = 0.0;
        private double axisAngle = 0.0;
        private double radiusX = 1.0;
        private double radiusY = 1.0;

        public DXFellipse()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Type = "ELLIPSE";

            majorAxis = new Point3D(0, 0, 0);
            Radius = 1;
            StartAngle = 0.0;
            EndAngle = 360.0;

            sweepAngle = 0.0;
            axisAngle = 0.0;
            radiusX = 1.0;
            radiusY = 1.0;

            IsCCW = false;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "11":
                    majorAxis.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "21":
                    majorAxis.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "31":
                    majorAxis.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "41":
                    StartAngle = Convert.ToDouble(group.Data, DXFContainer.NFI) * 180.0 / Math.PI;
                    break;
                case "42":
                    EndAngle = Convert.ToDouble(group.Data, DXFContainer.NFI) * 180.0 / Math.PI;
                    break;
                default:
                    return false;
            }

            return true;
        }

        internal override void EndRead()
        {
            sweepAngle = EndAngle - StartAngle;

            if (sweepAngle < 0)
                sweepAngle += 360;

            radiusX = PointD.Origin.Distance(majorAxis.PointXY);
            radiusY = radiusX * Radius; // Radius stores the ratio (was read by circle base class).

            axisAngle = PointD.Origin.AngleDeg(majorAxis.PointXY);
        }

        public override void Draw(GraphicsPath path, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            PointD ptStart = (centerPoint.PointXY + ptOffset).Offset(-radiusX, -radiusX);
            PointD ptEnd = (centerPoint.PointXY + ptOffset).Offset(radiusX, radiusX);

            RectangleD rcBoundary = new RectangleD(ptStart, ptEnd);
            if (!rcRegion.Contains(rcBoundary))
                return;
            if (rcBoundary.Diag < skipDetail)
                return;

            ptStart = (centerPoint.PointXY + ptOffset).Offset(-radiusX, -radiusY) - rcRegion.Center;

            if (axisAngle > 0)
            {
                MatrixD t = new MatrixD();
                PointD pt = centerPoint.PointXY + ptOffset - rcRegion.Center;

                t.Translate(pt.X, pt.Y);
                t.Rotate((float)axisAngle);
                t.Translate(-pt.X, -pt.Y);

                ptStart = t.TransformPoint(ptStart);
            }

            path.AddArc((float)ptStart.X, (float)ptStart.Y, (float)(2.0 * radiusX), (float)(2.0 * radiusY), (float)StartAngle, (float)sweepAngle);
        }
    }
    #endregion

    #region ARC
    public class DXFarc : DXFcircle
    {
        public double StartAngle { get; set; }
        public double EndAngle { get; set; }

        public bool IsCCW { get; set; }

        private double sweepAngle = 0.0;

        public DXFarc()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Type = "ARC";
            StartAngle = 0;
            EndAngle = 0;

            IsCCW = false;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "50":
                    StartAngle = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "51":
                    EndAngle = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                default:
                    return false;
            }

            return true;
        }

        internal override void EndRead()
        {
            sweepAngle = EndAngle - StartAngle;

            if (sweepAngle < 0)
                sweepAngle += 360;
        }

        public override void Draw(GraphicsPath path, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            PointD ptStart = (centerPoint.PointXY + ptOffset).Offset(-Radius, -Radius);
            PointD ptEnd = (centerPoint.PointXY + ptOffset).Offset(Radius, Radius);

            RectangleD rcBoundary = new RectangleD(ptStart, ptEnd);
            if (!rcRegion.Contains(rcBoundary))
                return;
            if (rcBoundary.Diag < skipDetail)
                return;

            ptStart -= rcRegion.Center;
            ptEnd -= rcRegion.Center;

            path.AddArc((float)ptStart.X, (float)ptStart.Y, (float)(2.0 * Radius), (float)(2.0 * Radius), (float)StartAngle, (float)sweepAngle);
        }
    }
    #endregion

    #region POINT
    public class DXFpoint : DXFEntity
    {
        protected Point3D centerPoint = new Point3D(0, 0, 0);

        public Point3D CenterPoint
        {
            get { return centerPoint; }
            set { centerPoint = value; }
        }

        public DXFpoint()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Type = "POINT";

            centerPoint = new Point3D(0, 0, 0);
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "10":
                    centerPoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "20":
                    centerPoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "30":
                    centerPoint.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            double rad = 1.0 / scale;

            PointD ptStart = (centerPoint.PointXY + ptOffset).Offset(-rad, -rad);
            PointD ptEnd = (centerPoint.PointXY + ptOffset).Offset(rad, rad);

            RectangleD rcBoundary = new RectangleD(ptStart, ptEnd);
            if (!rcRegion.Contains(rcBoundary))
                return;
            if (rcBoundary.Diag < skipDetail)
                return;

            ptStart -= rcRegion.Center;
            ptEnd -= rcRegion.Center;

            g.FillEllipse(new SolidBrush(ownPen.Color), (float)ptStart.X, (float)ptStart.Y, (float)(2.0 * rad), (float)(2.0 * rad));
        }
    }
    #endregion

    #region TEXT formats
    #region Base class
    public class DXFTextBase : DXFEntity
    {
        #region Variables
        public double Width { get; protected set; }

        public string Text { get; protected set; }
        public string TextStyle { get; protected set; }
        public double Height { get; protected set; }
        public double Rotation { get; protected set; }

        public StringFormat Format { get; protected set; }
        public Font TextFont { get; protected set; }

        protected Point3D insertionPoint = new Point3D(0, 0, 0);
        public Point3D InsertionPoint
        {
            get { return insertionPoint; }
            set { insertionPoint = value; }
        }

        protected Point3D directionVector = new Point3D(0, 0, 0);
        #endregion

        protected override void Init()
        {
            base.Init();

            Text = "";
            TextStyle = "";
            Height = 1.0;
            Rotation = 0.0;

            Format = new StringFormat();

            TextFont = new Font("Arial", 1.0F, GraphicsUnit.World);

            insertionPoint = new Point3D(0, 0, 0);
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "1":
                    Text += group.Data;
                    Text = Text.Substring(Text.LastIndexOf(';') + 1);
                    break;
                case "3":
                    Text += group.Data;
                    break;
                case "7":
                    TextStyle = group.Data;
                    break;
                case "10":
                    insertionPoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "20":
                    insertionPoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "30":
                    insertionPoint.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "11":
                    directionVector.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "21":
                    directionVector.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "31":
                    directionVector.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "40":
                    Height = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    TextFont = new Font("Arial", (float)Height);
                    break;
                case "41":
                    Width = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "50":
                    Rotation = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                default:
                    return false;
            }

            return true;
        }
    } 
    #endregion

    #region MTEXT
    public class DXFmtext : DXFTextBase
    {
        protected SizeF strSize = Size.Empty;
            
        public DXFmtext()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Type = "MTEXT";
            
            insertionPoint = new Point3D(0, 0, 0);
            strSize = SizeF.Empty;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            int code = 0;

            switch (group.Code)
            {
                case "43":
                    code = (int)Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "45":
                    code = (int)Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "71":
                    #region String Alignment
                    int.TryParse(group.Data, out code);

                    switch (code)
                    {
                        case 1:
                            Format.LineAlignment = StringAlignment.Near;
                            Format.Alignment = StringAlignment.Far;
                            break;
                        case 2:
                            Format.LineAlignment = StringAlignment.Center;
                            Format.Alignment = StringAlignment.Far;
                            break;
                        case 3:
                            Format.LineAlignment = StringAlignment.Far;
                            Format.Alignment = StringAlignment.Far;
                            break;
                        case 4:
                            Format.LineAlignment = StringAlignment.Near;
                            Format.Alignment = StringAlignment.Center;
                            break;
                        case 5:
                            Format.LineAlignment = StringAlignment.Center;
                            Format.Alignment = StringAlignment.Center;
                            break;
                        case 6:
                            Format.LineAlignment = StringAlignment.Far;
                            Format.Alignment = StringAlignment.Center;
                            break;
                        case 7:
                            Format.LineAlignment = StringAlignment.Center;
                            Format.Alignment = StringAlignment.Near;
                            break;
                        case 8:
                            Format.LineAlignment = StringAlignment.Center;
                            Format.Alignment = StringAlignment.Near;
                            break;
                        case 9:
                            Format.LineAlignment = StringAlignment.Far;
                            Format.Alignment = StringAlignment.Near;
                            break;
                    }
                    #endregion
                    break;
                case "72":
                    #region String Drawing Direction
                    int.TryParse(group.Data, out code);

                    switch (code)
                    {
                        case 1:
                            Format.FormatFlags &= ~StringFormatFlags.DirectionVertical;
                            break;
                        case 3:
                            Format.FormatFlags |= StringFormatFlags.DirectionVertical;
                            break;
                        case 5:
                            break;
                    }
                    #endregion
                    break;
                case "73":
                    #region Line Spacing - not implemented yet
                    int.TryParse(group.Data, out code);

                    switch (code)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                    }
                    #endregion
                    break;
                default:
                    return false;
            }

            return true;
        }

        internal override void EndRead()
        {
            base.EndRead();

            if (!directionVector.IsEmpty)
            {
                Rotation = PointD.Origin.AngleDeg(directionVector.PointXY);
            }
        }

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            System.Drawing.Drawing2D.Matrix t = g.Transform;

            if (strSize == SizeF.Empty)
            {
                double descent = (double)TextFont.FontFamily.GetCellDescent(FontStyle.Regular) * TextFont.Size / (double)TextFont.FontFamily.GetEmHeight(FontStyle.Regular);
                insertionPoint.X -= descent;

                if (Width != 0)
                    strSize = g.MeasureString(Text, TextFont, new SizeF((float)Width, (float)Height), Format);
                else
                    strSize = g.MeasureString(Text, TextFont, PointF.Empty, Format);

                if (Format.LineAlignment == StringAlignment.Center)
                    insertionPoint.X -= strSize.Width / 2.0;
                else if (Format.LineAlignment == StringAlignment.Far)
                    insertionPoint.X -= strSize.Width;

                if (Format.Alignment == StringAlignment.Center)
                    insertionPoint.Y -= strSize.Height / 2.0;
                else if (Format.Alignment == StringAlignment.Far)
                    insertionPoint.Y -= strSize.Height;
            }

            PointD ptStart = insertionPoint.PointXY + ptOffset;
            PointD ptEnd = ptStart.Offset(strSize.Width, strSize.Height);

            RectangleD rcBoundary = new RectangleD(ptStart, ptEnd);
            if (!rcRegion.Contains(rcBoundary))
                return;
            if (rcBoundary.Diag < skipDetail)
                return;

            PointD ptCenter = rcBoundary.Center - rcRegion.Center;

            ptStart -= rcBoundary.Center;

            g.TranslateTransform((float)ptCenter.X, (float)ptCenter.Y);
            g.RotateTransform((float)Rotation);
            g.ScaleTransform(1.0F, -1.0F);

            g.DrawString(Text, TextFont, ownPen.Brush, new RectangleF((float)ptStart.X, (float)ptStart.Y, strSize.Width, strSize.Height), Format);

            g.Transform = t;
        }
    }
    #endregion

    #region TEXT
    public class DXFtext : DXFTextBase
    {
        protected SizeF strSize = Size.Empty;
        
        public double ScaleX { get; protected set; }
        public double ScaleY { get; protected set; }
        public double ObliqueAngle { get; protected set; }

        public int VerTextJust { get; protected set; }
        public int HorTextJust { get; protected set; }

        public DXFtext()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Type = "TEXT";

            insertionPoint = new Point3D(0, 0, 0);
            strSize = SizeF.Empty;

            ScaleX = 1.0;
            ScaleY = 1.0;
            ObliqueAngle = 0.0;

            VerTextJust = 0;
            HorTextJust = 0;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            int code = 0;

            switch (group.Code)
            {
                case "51":
                    ObliqueAngle = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "71":
                    #region Mirror
                    int.TryParse(group.Data, out code);

                    switch (code)
                    {
                        case 2: ScaleX = -1; break;
                        case 4: ScaleY = -1; break;
                    }
                    #endregion
                    break;
                case "72":
                    #region Horizontal Alignment
                    int.TryParse(group.Data, out code);
                    HorTextJust = code;
                    #endregion
                    break;
                case "73":
                    #region Vertical Alignment
                    int.TryParse(group.Data, out code);
                    VerTextJust = code;
                    #endregion
                    break;
                default:
                    return false;
            }

            return true;
        }

        internal override void EndRead()
        {
            base.EndRead();

            if (this.Width != 0.0)
                this.ScaleX *= this.Width;

            iPt = insertionPoint;

            if (!directionVector.IsEmpty && (HorTextJust + VerTextJust > 0))
                insertionPoint = directionVector;
        }

        Point3D iPt;

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            System.Drawing.Drawing2D.Matrix t = g.Transform;
            
            if (strSize == SizeF.Empty)
            {
                strSize = g.MeasureString(Text, TextFont, PointF.Empty, Format);
                double ascent = (double)TextFont.FontFamily.GetCellAscent(FontStyle.Regular) * TextFont.Size / (double)TextFont.FontFamily.GetEmHeight(FontStyle.Regular);
                double descent = (double)TextFont.FontFamily.GetCellDescent(FontStyle.Regular) * TextFont.Size / (double)TextFont.FontFamily.GetEmHeight(FontStyle.Regular);
                double spacing = (double)TextFont.FontFamily.GetLineSpacing(FontStyle.Regular) * TextFont.Size / (double)TextFont.FontFamily.GetEmHeight(FontStyle.Regular);
                double fontHeight = spacing + descent / 2.0;

                switch (HorTextJust)
                {
                    case 0:
                        break;
                    case 1:
                        insertionPoint.X -= strSize.Width / 2.0;
                        break;
                    case 2:
                        insertionPoint.X -= strSize.Width;
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        break;
                }

                switch (VerTextJust)
                {
                    case 0:
                        insertionPoint.Y -= 2.0 * descent;
                        break;
                    case 1:
                        insertionPoint.Y -= spacing / 2.0;
                        break;
                    case 2:
                        insertionPoint.Y -= spacing;
                        break;
                    case 3:
                        break;
                }
            }

            PointD ptStart = insertionPoint.PointXY + ptOffset;
            PointD ptStart2 = iPt.PointXY + ptOffset;
            PointD ptEnd = ptStart.Offset(strSize.Width, strSize.Height);

            RectangleD rcBoundary = new RectangleD(ptStart, ptEnd);
            if (!rcRegion.Contains(rcBoundary))
                return;
            if (rcBoundary.Diag < skipDetail)
                return;

            PointD ptCenter = rcBoundary.Center - rcRegion.Center;

            ptStart -= rcBoundary.Center;
            ptStart2 -= rcBoundary.Center;
            RectangleD rc2 = new RectangleD(ptStart, ptStart2);

            g.TranslateTransform((float)ptCenter.X, (float)ptCenter.Y);
            g.RotateTransform((float)Rotation);
            g.ScaleTransform((float)ScaleX, -(float)ScaleY);
            
            // Add shear factor, if needed.
            if (ObliqueAngle != 0.0)
            {
                Matrix shear = new Matrix();
                shear.Shear((float)(-Math.Tan(ObliqueAngle * Math.PI / 180.0)), 0.0F);
                g.MultiplyTransform(shear);
            }

            g.DrawString(Text, TextFont, ownPen.Brush, new RectangleF((float)ptStart.X, (float)ptStart.Y, strSize.Width, strSize.Height), Format);
            
            g.Transform = t;
        }
    }
    #endregion
    #endregion

    #region HATCH
    public class DXFhatch : DXFEntity
    {
        public string Name { get; protected set; }
        public bool Solid { get; protected set; }

        protected DXFBoundaryPath[] boundaryPaths = new DXFBoundaryPath[0];
        protected int boundaryPathReadIndex = 0;

        protected Point3D elevationPoint = new Point3D(0, 0, 0);

        public Point3D ElevationPoint
        {
            get { return elevationPoint; }
            set { elevationPoint = value; }
        }

        public DXFhatch()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();
            
            Type = "HATCH";
            Name = "";

            Solid = true;

            elevationPoint = new Point3D(0, 0, 0);
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Check if boundary path has to be read.
            if (boundaryPathReadIndex < boundaryPaths.Length)
            {
                if (group.Code == "75") // Terminate path reading at this group type (part of HATCH entity).
                    boundaryPathReadIndex = boundaryPaths.Length;
                else if (group.Code == "92") // First group of each boundary path.
                {
                    boundaryPathReadIndex++;
                    boundaryPaths[boundaryPathReadIndex] = new DXFBoundaryPath();
                    return boundaryPaths[boundaryPathReadIndex].NewGroupCode(group);
                }
                else if (boundaryPathReadIndex > -1)
                    return boundaryPaths[boundaryPathReadIndex].NewGroupCode(group);
                else
                    return false;
            }

            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "2":
                    Name = group.Data;
                    break;
                case "10":
                    elevationPoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "20":
                    elevationPoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "30":
                    elevationPoint.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "70":
                    Solid = (group.Data == "1");
                    break;
                case "91":
                    boundaryPaths = new DXFBoundaryPath[Convert.ToInt32(group.Data, DXFContainer.NFI)];
                    boundaryPathReadIndex = -1;
                    break;
                default:
                    return false;
            }

            return true;
        }

        internal override void EndRead()
        {
            int max = boundaryPaths.Length;

            for (int i = 0; i < max; i++)
                boundaryPaths[i].EndRead();
        }

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            GraphicsPath path = new GraphicsPath();

            int max = boundaryPaths.Length;

            for (int i = 0; i < max; i++)
            {
                boundaryPaths[i].Draw(path, rcRegion, scale, skipDetail, ptOffset);
            }

            if (this.Solid)
                g.FillPath(ownPen.Brush, path);
            else
                g.DrawPath(ownPen, path);
        }
    }

    public enum DXFBoundaryPathEdgeTypes
    {
        Line = 1,
        CircularArc = 2,
        EllipticArc = 3,
        Spline = 4,
        PolyLine = 5
    }

    public class DXFBoundaryPath : DXFEntity
    {
        public Int32 PathType { get; protected set; }

        protected List<DXFEntity> pathElements = new List<DXFEntity>();

        protected bool isExternal = false;
        protected bool isPolyLine = false;
        protected bool isDerived = false;
        protected bool isTextbox = false;
        protected bool isOutermost = false;

        private DXFBoundaryPathEdgeTypes edgeType;

        public DXFBoundaryPath()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Type = "Boundary Path";

            pathElements.Clear();
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            double x, y;
            int n = pathElements.Count - 1;
            int data = 0;

            switch (group.Code)
            {
                case "92":
                    #region Code 92
                    PathType = Convert.ToInt16(group.Data, DXFContainer.NFI);

                    if ((PathType & 1) == 1)
                        isExternal = true;
                    if ((PathType & 2) == 2)
                    {
                        isPolyLine = true;
                        edgeType = DXFBoundaryPathEdgeTypes.PolyLine;
                        pathElements.Add(new DXFlwpolyline());
                    }
                    if ((PathType & 4) == 4)
                        isDerived = true;
                    if ((PathType & 8) == 8)
                        isTextbox = true;
                    if ((PathType & 16) == 16)
                        isOutermost = true;
                    #endregion
                    break; 

                case "72":
                    #region code 72
                    if (isPolyLine)
                    {
                        // TBD: has bulge flag
                    }
                    else
                    {
                        data = Convert.ToInt16(group.Data, DXFContainer.NFI);
                        edgeType = (DXFBoundaryPathEdgeTypes)data;
                    } 
                    #endregion
                    break;

                case "73":
                    #region code 73
                    bool flag = (group.Data == "1");

                    switch (edgeType)
                    {
                        case DXFBoundaryPathEdgeTypes.PolyLine:
                            (pathElements[n] as DXFlwpolyline).Closed = flag;
                            break;

                        case DXFBoundaryPathEdgeTypes.CircularArc:
                            (pathElements[n] as DXFarc).IsCCW = flag;
                            break;

                        case DXFBoundaryPathEdgeTypes.EllipticArc:
                            (pathElements[n] as DXFellipse).IsCCW = flag;
                            break;

                        case DXFBoundaryPathEdgeTypes.Spline:
                            // TBD: spline Rational value
                            break;
                    } 
                    #endregion
                    break;

                case "93":
                    // TBD: numbers of vertices or edges -> needed?
                    break;

                case "10":
                    #region code 10
                    x = Convert.ToDouble(group.Data, DXFContainer.NFI);

                    switch (edgeType)
                    {
                        case DXFBoundaryPathEdgeTypes.PolyLine:
                            (pathElements[n] as DXFlwpolyline).PointList.Add(new DXFvertex(x, 0, 0));
                            break;

                        case DXFBoundaryPathEdgeTypes.Line:
                            DXFline line = new DXFline();
                            line.StartPoint = new Point3D(x, 0, 0);
                            pathElements.Add(line);
                            break;

                        case DXFBoundaryPathEdgeTypes.CircularArc:
                            DXFarc arc = new DXFarc();
                            arc.CenterPoint = new Point3D(x, 0, 0);
                            pathElements.Add(arc);
                            break;

                        case DXFBoundaryPathEdgeTypes.EllipticArc:
                            DXFellipse ell = new DXFellipse();
                            ell.CenterPoint = new Point3D(x, 0, 0);
                            pathElements.Add(ell);
                            break;

                        case DXFBoundaryPathEdgeTypes.Spline:
                            // TBD: no splines supported.
                            break;
                    } 
                    #endregion
                    break;

                case "20":
                    #region code 20
                    y = Convert.ToDouble(group.Data, DXFContainer.NFI);

                    switch (edgeType)
                    {
                        case DXFBoundaryPathEdgeTypes.PolyLine:
                            (pathElements[n] as DXFlwpolyline).PointList[(pathElements[n] as DXFlwpolyline).PointList.Count - 1].StartPoint.Offset(0, y, 0);
                            break;

                        case DXFBoundaryPathEdgeTypes.Line:
                            (pathElements[n] as DXFline).StartPoint.Offset(0, y, 0);
                            break;

                        case DXFBoundaryPathEdgeTypes.CircularArc:
                            (pathElements[n] as DXFarc).CenterPoint.Offset(0, y, 0);
                            break;

                        case DXFBoundaryPathEdgeTypes.EllipticArc:
                            (pathElements[n] as DXFellipse).CenterPoint.Offset(0, y, 0);
                            break;

                        case DXFBoundaryPathEdgeTypes.Spline:
                            // TBD: no splines supported.
                            break;
                    }
                    #endregion
                    break;

                case "11":
                    #region code 11
                    x = Convert.ToDouble(group.Data, DXFContainer.NFI);

                    switch (edgeType)
                    {
                        case DXFBoundaryPathEdgeTypes.Line:
                            (pathElements[n] as DXFline).EndPoint.Offset(x, 0, 0);
                            break;

                        case DXFBoundaryPathEdgeTypes.EllipticArc:
                            (pathElements[n] as DXFellipse).MajorAxis.Offset(x, 0, 0);
                            break;
                    }
                    #endregion
                    break;

                case "21":
                    #region code 21
                    y = Convert.ToDouble(group.Data, DXFContainer.NFI);

                    switch (edgeType)
                    {
                        case DXFBoundaryPathEdgeTypes.Line:
                            (pathElements[n] as DXFline).EndPoint.Offset(0, y, 0);
                            break;

                        case DXFBoundaryPathEdgeTypes.EllipticArc:
                            (pathElements[n] as DXFellipse).MajorAxis.Offset(0, y, 0);
                            break;
                    }
                    #endregion
                    break;

                case "40":
                    #region code 40
                    x = Convert.ToDouble(group.Data, DXFContainer.NFI);

                    switch (edgeType)
                    {
                        case DXFBoundaryPathEdgeTypes.CircularArc:
                            (pathElements[n] as DXFarc).Radius = x;
                            break;

                        case DXFBoundaryPathEdgeTypes.EllipticArc:
                            (pathElements[n] as DXFellipse).Radius = x;
                            break;

                        case DXFBoundaryPathEdgeTypes.Spline:
                            // TBD: no splines supported.
                            break;
                    }
                    #endregion
                    break;

                case "42":
                    #region code 42
                    x = Convert.ToDouble(group.Data, DXFContainer.NFI);

                    switch (edgeType)
                    {
                        case DXFBoundaryPathEdgeTypes.PolyLine:
                            (pathElements[n] as DXFlwpolyline).PointList[(pathElements[n] as DXFlwpolyline).PointList.Count - 1].Bulge = x;
                            break;
                    }
                    #endregion
                    break;

                case "50":
                    #region code 50
                    x = Convert.ToDouble(group.Data, DXFContainer.NFI);

                    switch (edgeType)
                    {
                        case DXFBoundaryPathEdgeTypes.CircularArc:
                            (pathElements[n] as DXFarc).StartAngle = x;
                            break;

                        case DXFBoundaryPathEdgeTypes.EllipticArc:
                            (pathElements[n] as DXFellipse).StartAngle = x;
                            break;
                    }
                    #endregion
                    break;

                case "51":
                    #region code 51
                    x = Convert.ToDouble(group.Data, DXFContainer.NFI);

                    switch (edgeType)
                    {
                        case DXFBoundaryPathEdgeTypes.CircularArc:
                            (pathElements[n] as DXFarc).EndAngle = x;
                            break;

                        case DXFBoundaryPathEdgeTypes.EllipticArc:
                            (pathElements[n] as DXFellipse).EndAngle = x;
                            break;
                    }
                    #endregion
                    break;

                default:
                    return false;
            }

            return true;
        }

        internal override void EndRead()
        {
            int max = pathElements.Count;

            for (int i = 0; i < max; i++)
            {
                pathElements[i].EndRead();
            }
        }

        public override void Draw(GraphicsPath path, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            if (!isPolyLine)
                path.StartFigure();

            int max = pathElements.Count;

            for (int i = 0; i < max; i++)
                pathElements[i].Draw(path, rcRegion, scale, skipDetail, ptOffset);

            if (!isPolyLine)
                path.CloseFigure();
        }
    }
    #endregion
}