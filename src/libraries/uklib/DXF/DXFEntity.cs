using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using UKLib.MathEx;
using System.IO;
using System.Drawing.Drawing2D;

namespace UKLib.DXF
{
    public class DXFEntity : DXFAtom
    {
        public string Layer { get; protected set; }
        public string LineType { get; protected set; }
        public DXFLType LType { get; set; }
        public double LineTypeScale { get; protected set; }
        public Int16 LineWeight { get; set; }
        public Int16 Color { get; protected set; }
        public bool Visible { get; protected set; }

        public Color DrawColor { get; set; }

        protected Point3D extrusionDirection = new Point3D(0, 0, 1);

        public Point3D ExtrusionDirection
        {
            get { return extrusionDirection; }
            set { extrusionDirection = value; }
        }

        protected PointD ptStart;
        protected PointD ptEnd;
        protected RectangleD rcBoundary = null;

        protected Pen ownPen = new Pen(System.Drawing.Color.White, 1.0F);
        protected bool hairLine = true;

        protected override void Init()
        {
            base.Init();

            Layer = "";
            LineType = "";
            LineTypeScale = 1.0;
            Color = 0;
            Visible = true;

            DrawColor = System.Drawing.Color.Black;

            extrusionDirection = new Point3D(0, 0, 1);
            LType = new DXFLType();

            hairLine = true;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            switch (group.Code)
            {
                case "6":
                    LineType = group.Data;
                    break;
                case "8":
                    Layer = group.Data;
                    break;
                case "48":
                    LineTypeScale = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "60":
                    Visible = (group.Data == "0");
                    break;
                case "62":
                    Color = Convert.ToInt16(group.Data, DXFContainer.NFI);
                    DrawColor = DXFDefinitions.DXFColor(Color);
                    break;
                case "370":
                    LineWeight = Convert.ToInt16(group.Data, DXFContainer.NFI);
                    hairLine = false;
                    break;
                case "210":
                    extrusionDirection.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "220":
                    extrusionDirection.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                case "230":
                    extrusionDirection.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
                    break;
                default:
                    return false;
            }

            return true;
        }

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            if (hairLine)
                ownPen.Width = (float)(1.0 / scale);

            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            Draw(path, rcRegion, scale, skipDetail, ptOffset);
            g.DrawPath(ownPen, path);
        }

        public override void Update(Pen pen, Pen penBlock, Pen penLayer)
        {
            if (Color == 0)
                ownPen = penBlock.Clone() as Pen;
            else if (Color == 256)
                ownPen = penLayer.Clone() as Pen;
            else
            {
                ownPen = pen.Clone() as Pen;

                if (ownPen.Color != DrawColor)
                    ownPen.Color = DrawColor;
            }

            if (LineWeight > 0)
            {
                float penWidth = (float)LineWeight * 0.01F; // 0.01 mm resolution divided by default value (0.25 mm)

                if (ownPen.Width != penWidth)
                    ownPen.Width = penWidth;
            }

            if (LType != null)
                LType.SetupPen(ownPen, LineTypeScale);

            pen = ownPen;
        }
    }
}
