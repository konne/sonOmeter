using System;
using System.Collections;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace sonOmeter.Classes
{
    /// <summary>
    /// Summary description for CutMode.
    /// </summary>
    public enum CutMode
    {
        Nothing,
        Top,
        Bottom,
        CDepth,
        Surface
    }

    /// <summary>
    /// Summary description for CutLine.
    /// </summary>
    public class CutLine
    {
        #region Variables
        Stack<Collection<PointF>> histpointList = new Stack<Collection<PointF>>();

        Collection<PointF> pointList = new Collection<PointF>();
        #endregion

        #region Properties
        public PointF this[int index]
        {
            get { return pointList[index]; }
        }

        public Collection<PointF> PointList
        {
            get { return pointList; }
        }

        public int Count
        {
            get { return pointList.Count; }
        }
        #endregion

        public void Revert()
        {
            if (histpointList.Count > 0)
                pointList = histpointList.Pop();
        }

        #region Point handling

        private DateTime lastBackup;
        private void Backup()
        {
            if ((DateTime.Now - lastBackup).TotalSeconds > 0.5)
            {
                var copy = new Collection<PointF>();
                foreach (var item in pointList) copy.Add(new PointF(item.X, item.Y));
                histpointList.Push(copy);
                lastBackup = DateTime.Now;
            }
        }


        public void Clear()
        {
            pointList.Clear();
        }

        public void Clear(float start, float end, float y)
        {
            Backup();
            pointList.Clear();
            Add(new PointF(start, y));
            Add(new PointF(end, y));
        }

        public void Insert(PointF ptAdd)
        {
            Backup();

            int i = 0;
            int max = pointList.Count;
            float ptAddX = ptAdd.X;
            float ptX = 0;

            for (i = 0; i < max; i++)
            {
                ptX = pointList[i].X;

                if (ptX > ptAddX)
                    break;
                else if (ptX == ptAddX)
                {
                    pointList.RemoveAt(i);
                    break;
                }
            }

            pointList.Insert(i, ptAdd);
        }

        public void Insert(float x, float y)
        {
            Insert(new PointF(x, y));
        }

        public void SafeInsert(PointF ptAdd)
        {
            if ((ptAdd.X < this[0].X) || (ptAdd.X > this[Count - 1].X))
                return;

            Insert(ptAdd);
        }

        public void SafeInsert(float x, float y)
        {
            SafeInsert(new PointF(x, y));
        }

        public void Add(PointF ptAdd)
        {
            Backup();
            pointList.Add(ptAdd);
        }

        public void Add(float x, float y)
        {
            Add(new PointF(x, y));
        }

        public void Remove(PointF ptRemove)
        {            
            int i = 0;
            int max = pointList.Count;
            PointF pt;

            for (i = 0; i < max; i++)
            {
                pt = pointList[i];

                if (pt.X == ptRemove.X)
                {
                    Backup();
                    pointList.RemoveAt(i);
                    return;
                }
            }
        }

        public void Remove(int index)
        {
            if ((index > 0) && (index < pointList.Count - 1))
            {
                Backup();
                pointList.RemoveAt(index);
            }
        }

        public float InterpolateAt(float x)
        {
            if (Count <= 0)
                return 0;

            PointF pt = this[0];
            int max = pointList.Count;
            float y = pt.Y;

            for (int i = 1; i < max; i++)
            {
                pt = this[i];

                if (pt.X == x)
                {
                    y = pt.Y;
                    break;
                }
                else if (pt.X > x)
                {
                    y = (x - this[i - 1].X) * (pt.Y - this[i - 1].Y) / (pt.X - this[i - 1].X) + this[i - 1].Y;
                    break;
                }
            }

            return y;
        }

        public int GetNearestAt(float x)
        {
            if (Count <= 0)
                return -1;

            PointF pt = this[0];
            int index = 0;

            for (int i = 1; i < Count; i++)
            {
                pt = this[i];

                if (pt.X == x)
                {
                    index = i;
                    break;
                }
                else if (pt.X > x)
                {
                    index = i;

                    if (x - this[i - 1].X < pt.X - x)
                        index--;

                    break;
                }
            }

            return index;
        }

        public int GetNearestAt(float x, float dist)
        {
            if (Count <= 0)
                return -1;

            PointF pt = this[0];
            int index = -1;

            for (int i = 1; i < Count; i++)
            {
                pt = this[i];

                if (pt.X == x)
                {
                    index = i;
                    break;
                }
                else if (pt.X > x)
                {
                    index = i;

                    if (x - this[i - 1].X < pt.X - x)
                        index--;

                    break;
                }
            }

            if ((index > -1) && (Math.Abs(this[index].X - x) > dist))
                index = -1;

            return index;
        }

        public int GetAt(float x)
        {
            if (Count <= 0)
                return -1;

            for (int i = 1; i < Count; i++)
            {
                if (this[i].X == x)
                {
                    return i;
                }
            }

            return -1;
        }

        public PointF[] GetNeighbours(int index)
        {
            try
            {
                if ((Count <= 0) || (pointList[index] == null))
                    return null;

                PointF[] list;

                if (index > 0)
                {
                    if (index < Count - 1)
                    {
                        list = new PointF[3];
                        list[0] = this[index - 1];
                        list[1] = this[index];
                        list[2] = this[index + 1];
                    }
                    else
                    {
                        list = new PointF[2];
                        list[0] = this[index - 1];
                        list[1] = this[index];
                    }
                }
                else
                {
                    if (index < Count - 1)
                    {
                        list = new PointF[2];
                        list[0] = this[index];
                        list[1] = this[index + 1];
                    }
                    else
                    {
                        list = new PointF[1];
                        list[0] = this[index - 1];
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
        }

        public void Move(int index, PointF ptNew)
        {
            try
            {
                if ((Count <= 0) || (index < 0) || (pointList[index] == null))
                    return;

                Remove(index);
                Insert(ptNew);
            }
            catch
            {
            }
        }

        public void Displace(CutLine cl, float offset, bool twice)
        {
            Backup();

            bool self = cl == this;
            int max = cl.PointList.Count;

            // Clear old points.
            if (!self)
                Clear();

            // Go through passed cut line and apply offset.
            if (self)
                for (int i = 0; i < max; i++)
                {
                    PointF pt = cl.PointList[i];

                    if (twice)
                        pointList[i] = new PointF(pt.X, pt.Y * 2);
                    else
                        pointList[i] = new PointF(pt.X, pt.Y + offset);
                }
            else
                for (int i = 0; i < max; i++)
                {
                    PointF pt = cl.PointList[i];

                    if (twice)
                        Add(pt.X, pt.Y * 2);
                    else
                        Add(pt.X, pt.Y + offset);
                }
        }

        public void RemoveSelected(int disableSelStart, int disableSelStop)
        {
            List<int> removeList = new List<int>();            
            int max = this.Count;
            int i = 0;

            for (i = 0; i < max; i++)
            {
                PointF pt = this[i];

                if (disableSelStart <= pt.X && pt.X <= disableSelStop)                
                    removeList.Add(i);                
            }

            // Remove points in between safely backwards.
            max = removeList.Count;

            for (i = max - 1; i >= 0; i--)
            {                
                Remove(removeList[i]);
            }
        }


        public void DisableSelected(int disableSelStart, int disableSelStop)
        {
            Backup();

            PointF ptStartPrev = PointF.Empty;
            PointF ptStartNext = PointF.Empty;
            PointF ptStopPrev = PointF.Empty;
            PointF ptStopNext = PointF.Empty;
            List<int> removeList = new List<int>();
            float factor = 0;
            int insertIndex = 0;
            int max = this.Count;
            int i = 0;

            // Find insert region and points lying in between.
            for (i = 0; i < max; i++)
            {
                PointF pt = this[i];

                if (pt.X < disableSelStart)
                {
                    insertIndex = i + 1;
                    ptStopPrev = ptStartPrev = pt;
                }
                else if (pt.X < disableSelStop)
                {
                    ptStopPrev = ptStopNext = ptStartNext = pt;
                    removeList.Add(i);
                }
                else
                {
                    ptStopNext = ptStartNext = pt;
                    break;
                }
            }

            // Remove points in between safely backwards.
            max = removeList.Count;

            for (i = max - 1; i >= 0; i--)
            {
                ptStartNext = this[removeList[i]];
                Remove(removeList[i]);
            }

            // Interpolate heights at the selection borders and insert points.
            if (ptStartNext.X != ptStartPrev.X)
                factor = (ptStartNext.Y - ptStartPrev.Y) / (ptStartNext.X - ptStartPrev.X);
            else
                factor = 0.0F;
            pointList.Insert(insertIndex, new PointF(disableSelStart, factor * ((float)disableSelStart - ptStartPrev.X) + ptStartPrev.Y));
            pointList.Insert(insertIndex + 1, new PointF(disableSelStart, (float)GSC.Settings.DepthTop));

            if (ptStopNext.X != ptStopPrev.X)
                factor = (ptStopNext.Y - ptStopPrev.Y) / (ptStopNext.X - ptStopPrev.X);
            else
                factor = 0.0F;
            pointList.Insert(insertIndex + 2, new PointF(disableSelStop, (float)GSC.Settings.DepthTop));
            pointList.Insert(insertIndex + 3, new PointF(disableSelStop, factor * ((float)disableSelStop - ptStopPrev.X) + ptStopPrev.Y));
        }
        #endregion

        public CutLine()
        {
            Backup();
            pointList.Clear();
        }

        public CutLine(float start, float end, float y)
        {
            Clear(start, end, y);
        }

        public bool IsEmpty()
        {
            return !(pointList.Count > 0);
        }
    }

    /// <summary>
    /// Summary description for CutLineSet.
    /// </summary>
    public class CutLineSet
    {
        #region Variables
        CutLine cutTop = new CutLine();
        CutLine cutBottom = new CutLine();
        CutLine cutCalcDepth = new CutLine();
        CutLine cutSurface = new CutLine();

        bool calcDepthAv = false;

        SonarPanelType panelType = SonarPanelType.Void;
        #endregion

        #region Properties
        public CutLine CutTop
        {
            get { return cutTop; }
            set { cutTop = value; }
        }

        public CutLine CutBottom
        {
            get { return cutBottom; }
            set { cutBottom = value; }
        }

        public CutLine CutCalcDepth
        {
            get { return cutCalcDepth; }
            set { cutCalcDepth = value; }
        }

        public CutLine CutSurface
        {
            get { return cutSurface; }
            set { cutSurface = value; }
        }

        public bool CalcDepthAv
        {
            get { return calcDepthAv; }
            set { calcDepthAv = value; }
        }

        public SonarPanelType PanelType
        {
            get { return panelType; }
            set { panelType = value; }
        }
        #endregion

        public CutLineSet(SonarPanelType panelType)
        {
            this.panelType = panelType;
        }
    }
}
