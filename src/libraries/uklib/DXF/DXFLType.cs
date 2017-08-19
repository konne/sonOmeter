using System;
using System.Collections.Generic;
using System.Text;

namespace UKLib.DXF
{
    public class DXFLType : DXFAtom
    {
        public string Name { get; protected set; }
        public string DescriptiveText { get; protected set; }
        public double TotalLength { get; protected set; }

        protected double[] elements = new double[0];
        protected int counter = -1;

        public DXFLType()
        {
            Init();
        }

        protected override void Init()
        {
            base.Init();

            Name = "";
            DescriptiveText = "";
            Type = "LTYPE";
            TotalLength = 1.0;
        }

        internal override bool NewGroupCode(DXFGroup group)
        {
            // Call base class function for common group codes.
            if (base.NewGroupCode(group))
                return true;

            if (group.Code == "2")
                Name = group.Data;
            else if (group.Code == "3")
                DescriptiveText = group.Data;
            else if (group.Code == "73")
            {
                elements = new double[Convert.ToInt32(group.Data, DXFContainer.NFI)];
                counter = -1;
            }
            else if (group.Code == "40")
                TotalLength = Convert.ToDouble(group.Data, DXFContainer.NFI);
            else if (group.Code == "49")
            {
                counter++;
                elements[counter] = Convert.ToDouble(group.Data, DXFContainer.NFI);
            }
            else
                return false;

            return true;
        }

        public void SetupPen(System.Drawing.Pen pen, double scale)
        {
            int max = elements.Length;

            if (max == 0)
            {
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                return;
            }

            /*pen.DashPattern = new float[max];

            for (int i = 0; i < max; i++)
                pen.DashPattern[i] = (float)(System.Math.Abs(elements[i] * scale));*/

            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }
    }
}
