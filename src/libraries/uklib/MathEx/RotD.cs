using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace UKLib.MathEx
{
    [Serializable]
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class RotD
    {
        #region Properties
        /// <summary>
        /// The heading or yaw angle of the object (rotation around Z axis).
        /// </summary>
        [DefaultValue(0.0)]
        public double Yaw { get; set; }

        /// <summary>
        /// The elevation or pitch angle of the object (rotation around Y axis).
        /// </summary>
        [DefaultValue(0.0)]
        public double Pitch { get; set; }
        
        /// <summary>
        /// The bank or roll angle of the object (rotation around X axis).
        /// </summary>
        [DefaultValue(0.0)]
        public double Roll { get; set; }
        #endregion

        public RotD()
        {
            this.Yaw = 0;
            this.Pitch = 0;
            this.Roll = 0;
        }

        public RotD(RotD rot)
        {
            this.Pitch = rot.Pitch;
            this.Roll = rot.Roll;
            this.Yaw = rot.Yaw;
        }

        public RotD(double yaw, double pitch, double roll)
        {
            this.Pitch = pitch;
            this.Roll = roll;
            this.Yaw = yaw;
        }

        public static RotD operator +(RotD pt1, RotD pt2)
        {
            return new RotD(pt1.Yaw + pt2.Yaw, pt1.Pitch + pt2.Pitch, pt1.Roll + pt2.Roll);
        }

        public static RotD operator -(RotD pt1, RotD pt2)
        {
            return new RotD(pt1.Yaw - pt2.Yaw, pt1.Pitch - pt2.Pitch, pt1.Roll - pt2.Roll);
        }

        public static RotD operator *(RotD pt, double d)
        {
            return new RotD(pt.Yaw * d, pt.Pitch * d, pt.Roll * d);
        }

        public static RotD operator /(RotD pt, double d)
        {
            return new RotD(pt.Yaw / d, pt.Pitch / d, pt.Roll / d);
        }

        public override string ToString()
        {
            return "R=" + Roll + ", P=" + Pitch + ", Y=" + Yaw;
        }
    }
}
