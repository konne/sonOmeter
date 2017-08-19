using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using UKLib.MathEx;

namespace UKLib.DXF
{
    public class DXFsection : DXFAtom
    {
        public string Name { get; protected set; }
        public List<DXFGroup> Groups { get; protected set; }

        public DXFsection()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Name = "";
            Groups = new List<DXFGroup>();
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            if ((group.Code == "2") && (Name == ""))
                Name = group.Data;
            else
                Groups.Add(group);

            return true;
        }
    }

    public class DXFsectionHEADER : DXFsection
    {
        public BoxD BoundingBox { get; protected set; }

        public DXFsectionHEADER()
        {
            Init();

            Name = "HEADER";
            Groups = new List<DXFGroup>();
        }

        public DXFsectionHEADER(DXFsection section)
        {
            Init();

            Name = section.Name;
            Groups = section.Groups;

            ParseGroups();
        }

        protected override void Init()
        {
            base.Init();

            BoundingBox = new BoxD(0, 0, 0, 0, 0, 0);
        }

        public void ParseGroups()
        {
            Point3D ptMin = new Point3D(0, 0, 0);
            Point3D ptMax = new Point3D(0, 0, 0);
            int max = Groups.Count;

            for (int i = 0; i < max; i++)
            {
                if (Groups[i].Code == "9")
                {
                    // New header property.
                    if (Groups[i].Data == "$EXTMIN")
                        ptMin = ParsePoint(ref i, max);
                    else if (Groups[i].Data == "$EXTMAX")
                        ptMax = ParsePoint(ref i, max);
                }
            }

            BoundingBox = new BoxD(ptMin, ptMax);
        }

        private Point3D ParsePoint(ref int i, int max)
        {
            Point3D pt = new Point3D(0, 0, 0);

            for (i = i + 1; i < max; i++)
            {
                if (Groups[i].Code == "10")
                    pt.X = Convert.ToDouble(Groups[i].Data, DXFContainer.NFI);
                else if (Groups[i].Code == "20")
                    pt.Y = Convert.ToDouble(Groups[i].Data, DXFContainer.NFI);
                else if (Groups[i].Code == "30")
                    pt.Z = Convert.ToDouble(Groups[i].Data, DXFContainer.NFI);
                else
                    break;
            }

            i--;

            return pt;
        }
    }
}
