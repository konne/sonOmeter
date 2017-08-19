using System;
using System.Collections.Generic;
using System.Text;
using UKLib.MathEx;
using System.Globalization;
using System.Drawing;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;

namespace UKLib.Survey.Parse
{
    public enum DA040ElementType
    {
        Line,
        Arc,
        Clothoid
    }

    public class DA040Element
    {
        private int roadway = 0;
        private double station = 0;
        private double radius = 0;
        private double parameter = 0;
        private double angle = 0;
        private double startAngle = 0;
        private double sweepAngle = 0;
        private PointD pos = new PointD(0, 0); // rv and hv
        private PointD posC = new PointD(0, 0); // rv and hv of circle center point
        private DA040ElementType type = DA040ElementType.Line;

        public double Angle
        {
            get { return angle; }
        }

        public double Radius
        {
            get { return radius; }
        }

        public double Parameter
        {
            get { return parameter; }
        }

        public double Station
        {
            get { return station; }
        }

        public PointD Pos
        {
            get { return pos; }
        }

        public DA040ElementType Type
        {
            get { return type; }
        }

        public DA040Element(string line, NumberFormatInfo nfi)
        {
            ReadLine(line, nfi);
        }

        public void ReadLine(string line, NumberFormatInfo nfi)
        {
            // Trasse / Roadway
            int.TryParse(line.Substring(3, 2).Trim(), NumberStyles.Integer, nfi, out roadway);
            // Station
            double.TryParse(line.Substring(5, 10).Trim(), NumberStyles.Float, nfi, out station);
            // Radius
            double.TryParse(line.Substring(23, 12).Trim(), NumberStyles.Float, nfi, out radius);
            // Parameter A
            double.TryParse(line.Substring(35, 9).Trim(), NumberStyles.Float, nfi, out parameter);
            // Angle
            double.TryParse(line.Substring(44, 12).Trim(), NumberStyles.Float, nfi, out angle);
            // RV
            double rv = 0;
            double.TryParse(line.Substring(56, 12).Trim(), NumberStyles.Float, nfi, out rv);
            // HV
            double hv = 0;
            double.TryParse(line.Substring(68, 12).Trim(), NumberStyles.Float, nfi, out hv);

            // Build element start point.
            pos = new PointD(rv, hv);

            // Find the element type.
            if (parameter != 0.0)
            {
                // Nonzero parameter A -> Clothoid
                type = DA040ElementType.Clothoid;
            }
            else if (radius != 0.0)
            {
                // Nonzero radius -> Arc
                type = DA040ElementType.Arc;
            }
            else
            {
                // The rest -> Line
                type = DA040ElementType.Line;
            }
        }

        public string WriteLine(ref double previousStation, NumberFormatInfo nfi)
        {
            string s = "";

            // Datenart (DA) 040
            s += "040";
            // Trasse / Roadway
            s += roadway.ToString().PadLeft(2);
            // Station
            s += station.ToString("F3", nfi).PadLeft(10);
            // Difference to previous station
            s += (station - previousStation).ToString("F3", nfi).PadLeft(8);
            previousStation = station;
            // Radius
            s += radius.ToString("F4", nfi).PadLeft(12);
            // Parameter A
            s += parameter.ToString("F3", nfi).PadLeft(9);
            // Angle
            s += angle.ToString("F7", nfi).PadLeft(12);
            // RV
            s += pos.X.ToString("F3", nfi).PadLeft(12);
            // HV
            s += pos.Y.ToString("F3", nfi).PadLeft(12);

            return s;
        }

        public void Prepare(double nextAngle)
        {
            if (type == DA040ElementType.Arc)
            {
                double phi = System.Math.PI * (1.0 - angle / 200.0);
                
                if (radius < 0)
                    startAngle = (2.0 - nextAngle / 200.0) * 180.0;
                else
                    startAngle = (-angle / 200.0) * 180.0;

                while (startAngle < 0)
                    startAngle += 360.0;

                if (radius < 0)
                    sweepAngle = (-angle / 200.0) * 180.0 - startAngle;
                else
                    sweepAngle = (2.0 - nextAngle / 200.0) * 180.0 - startAngle;

                posC = new PointD(-radius * System.Math.Cos(phi), -radius * System.Math.Sin(phi)) + pos;
            }
        }

        public void Draw(Graphics g, Pen pen, MatrixD mat, PointD nextPos)
        {
            double r = System.Math.Abs(radius);
            double d = 2.0 * r;

            PointD pos1 = mat.TransformPoint(nextPos);
            PointD pos2 = mat.TransformPoint(pos);
            PointD pos3 = mat.TransformPoint(posC - r);
            PointD pos4 = mat.TransformPoint(posC + r);

            RectangleD rcEllipse = new RectangleD(pos3, pos4);

            switch (type)
            {
                case DA040ElementType.Line:
                    pen.Color = Color.Green;
                    g.DrawLine(pen, (float)pos1.X, (float)pos1.Y, (float)pos2.X, (float)pos2.Y);
                    break;

                case DA040ElementType.Arc:
                    pen.Color = Color.Red;
                    g.DrawArc(pen, (float)pos3.X, (float)pos4.Y, (float)(pos4.X - pos3.X), (float)(pos3.Y - pos4.Y), (float)startAngle, (float)sweepAngle);
                    break;

                case DA040ElementType.Clothoid:
                    // tbd
                    break;
            }
        }

        public override string ToString()
        {
            return "DA040Element: " + type.ToString();
        }
    }

    public class DA040File
    {
        public Collection<DA040Element> elementList = new Collection<DA040Element>();

        public DA040File(string fileName)
        {
            ReadFile(fileName);
        }

        public void ReadFile(string fileName)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
            StreamReader reader = new StreamReader(fileName);
            DA040Element element = null;

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                if (line == "")
                    continue;

                element = new DA040Element(line, nfi);
                elementList.Add(element);

                if (elementList.Count > 1)
                    elementList[elementList.Count - 2].Prepare(element.Angle);
            }

            if ((elementList.Count > 1) && (element != null))
                elementList[elementList.Count - 2].Prepare(element.Angle);

            reader.Close();
        }

        public void Draw(Graphics g, Pen pen, MatrixD mat)
        {
            int max = elementList.Count - 1;

            for (int i = 0; i < max; i++)
            {
                elementList[i].Draw(g, pen, mat, (i + 1 < max) ? elementList[i + 1].Pos : new PointD());
            }
        }
    }
}
