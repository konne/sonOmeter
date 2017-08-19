using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Drawing;
using UKLib.MathEx;
using System.Reflection;

namespace UKLib.DXF
{
    public class DXFblock : DXFAtom
    {
        public string Name { get; protected set; }
        public string Layer { get; protected set; }

        protected Point3D basePoint = new Point3D(0, 0, 0);

        public Point3D BasePoint
        {
            get { return basePoint; }
            set { basePoint = value; }
        }

        public List<DXFEntity> Entities { get; protected set; }

        public DXFblock()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Name = "";
            Layer = "";
            basePoint = new Point3D(0, 0, 0);

            Type = "BLOCK";

            Entities = new List<DXFEntity>();
        }

        public override DXFGroup Read(System.IO.StreamReader reader)
        {
            DXFGroup group;

            // Read own data.
            group = base.Read(reader);

            // Read until end of block.
            while (group.Data != "ENDBLK")
            {
                // Try to get the appropriate type, otherwise read dummy atom.
                System.Type t = System.Type.GetType("UKLib.DXF.DXF" + group.Data.ToLower());

                if (t != null)
                {
                    ConstructorInfo ci = t.GetConstructor(System.Type.EmptyTypes);
                    DXFEntity entity = (DXFEntity)ci.Invoke(System.Type.EmptyTypes);
                    Entities.Add(entity);
                    group = entity.Read(reader);
                }
                else
                    group = new DXFAtom().Read(reader);
            }

            // Read one more dummy atom for ENDBLK.
            group = new DXFAtom().Read(reader);

            // Return next type specifier.
            return group;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            if (group.Code == "2")
                Name = group.Data;
            else if (group.Code == "8")
                Layer = group.Data;
            else if (group.Code == "10")
                basePoint.X = Convert.ToDouble(group.Data, DXFContainer.NFI);
            else if (group.Code == "20")
                basePoint.Y = Convert.ToDouble(group.Data, DXFContainer.NFI);
            else if (group.Code == "30")
                basePoint.Z = Convert.ToDouble(group.Data, DXFContainer.NFI);
            else
                return false;

            return true;
        }

        public override void Draw(Graphics g, RectangleD rcRegion, double scale, double skipDetail, PointD ptOffset)
        {
            int max = Entities.Count;

            ptOffset -= basePoint.PointXY;

            for (int i = 0; i < max; i++)
                Entities[i].Draw(g, rcRegion, scale, skipDetail, ptOffset);
        }

        public override void Update(Pen pen, Pen penBlock, Pen penLayer)
        {
            int max = Entities.Count;

            for (int i = 0; i < max; i++)
                Entities[i].Update(pen, penBlock, penLayer);
        }
    }
}
