using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections.ObjectModel;
using UKLib.MathEx;
using System.Drawing;
using System.ComponentModel;

namespace UKLib.DXF
{
    public class DXFLayer : DXFAtom
    {
        [Browsable(false)]
        public string Name { get; protected set; }
        [Browsable(false)]
        public string LineType { get; protected set; }
        [Browsable(false)]
        public DXFLType LType { get; set; }
        [Browsable(false)]
        public Int16 Flags { get; protected set; }
        [Browsable(false)]
        public Int16 Color { get; protected set; }
        [Browsable(false)]
        public bool Visible { get; set; }
        [Browsable(false)]
        public List<DXFEntity> Entities { get; protected set; }

        public Color DrawColor { get; set; }
        public sbyte LineWeight { get; set; }


        public DXFLayer()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Name = "";
            LineType = "";
            Flags = 0;
            Color = 0;
            LineWeight = 0;

            Type = "LAYER";

            Visible = true;
            Entities = new List<DXFEntity>();
            DrawColor = System.Drawing.Color.Black;
            LType = new DXFLType();
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            if (group.Code == "2")
                Name = group.Data;
            else if (group.Code == "6")
                LineType = group.Data;
            else if (group.Code == "62")
            {
                Color = Convert.ToInt16(group.Data, DXFContainer.NFI);
                DrawColor = DXFDefinitions.DXFColor(Color);
            }
            else if (group.Code == "70")
                Flags = Convert.ToInt16(group.Data, DXFContainer.NFI);
            else if (group.Code == "370")
                LineWeight = Convert.ToSByte(group.Data, DXFContainer.NFI);
            else
                return false;

            return true;
        }

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            if (!Visible)
                return;

            int max = Entities.Count;

            for (int i = 0; i < max; i++)
                Entities[i].Draw(g, rcRegion, scale, skipDetail, ptOffset);
        }

        public override void Update(Pen pen, Pen penBlock, Pen penLayer)
        {
            penLayer = new Pen(DrawColor, 1.0F);

            LType.SetupPen(penLayer, 1.0);

            int max = Entities.Count;

            for (int i = 0; i < max; i++)
                Entities[i].Update(pen, penBlock, penLayer);
        }

        public void Draw(Graphics g, RectangleD rcRegion, PointD ptOffset, double scale, double skipDetail)
        {
            if (!Visible)
                return;

            int max = Entities.Count;

            for (int i = 0; i < max; i++)
                Entities[i].Draw(g, rcRegion, scale, skipDetail, ptOffset);
        }

        public override string ToString()
        {
            return "DXFLayer - " + Name;
        }

        public void Update()
        {
            Pen pen = new Pen(DrawColor, 1.0F);
            Pen penLayer = new Pen(DrawColor, 1.0F);
            Pen penBlock = new Pen(DrawColor, 1.0F);

            if (LType != null)
                LType.SetupPen(penLayer, 1.0);

            int max = Entities.Count;

            for (int i = 0; i < max; i++)
                Entities[i].Update(pen, penBlock, penLayer);
        }
    }
}
