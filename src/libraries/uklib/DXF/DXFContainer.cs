using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Forms;
using System.Globalization;
using System.Drawing;
using UKLib.MathEx;

namespace UKLib.DXF
{
    public class DXFContainer
    {
        #region Variables
        private List<DXFEntity> entityList = new List<DXFEntity>();
        private List<DXFblock> blockList = new List<DXFblock>();
        
        public List<DXFLayer> LayerList { get; protected set; }
        public List<DXFLType> LTypeList { get; protected set; }
        public DXFsectionHEADER Header { get; protected set; }

        public static NumberFormatInfo NFI = new CultureInfo("en-US", false).NumberFormat;
        #endregion

        public DXFContainer()
        {
            Header = new DXFsectionHEADER();
            LayerList = new List<DXFLayer>();
            LTypeList = new List<DXFLType>();
        }

        #region File I/O
        public void ReadFile(string fileName)
        {
            DXFGroup group = new DXFGroup("", "");
            StreamReader reader = null;

            try
            {
                reader = new StreamReader(fileName, Encoding.Default);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.FileName.ToString() + " cannot be found");
                return;
            }
            catch
            {
                MessageBox.Show("An error occured while opening the DXF file");
                return;
            }

            if (reader == null)
                return;

            DXFEntity entity = null;

            entityList.Clear();
            blockList.Clear();

            List<string> unusedAtomsList = new List<string>();

            while ((group.Data != "EOF") && !reader.EndOfStream)
            {
                if (group.Code == "0")
                {
                    switch (group.Data)
                    {
                        case "SECTION":
                            DXFsection section = new DXFsection();
                            group = section.Read(reader);

                            if (section.Name == "HEADER")
                                Header = new DXFsectionHEADER(section);
                            break;

                        case "LTYPE":
                            DXFLType ltype = new DXFLType();
                            group = ltype.Read(reader);
                            LTypeList.Add(ltype);
                            break;

                        case "LAYER":
                            DXFLayer layer = new DXFLayer();
                            group = layer.Read(reader);
                            LayerList.Add(layer);
                            layer.LType = FindLType(layer.LineType);
                            break;

                        case "MTEXT":
                            entity = new DXFmtext();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "TEXT":
                            entity = new DXFtext();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "HATCH":
                            entity = new DXFhatch();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "LINE":
                            entity = new DXFline();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "LWPOLYLINE":
                            entity = new DXFlwpolyline();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "POLYLINE":
                            entity = new DXFpolyline();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "CIRCLE":
                            entity = new DXFcircle();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "ELLIPSE":
                            entity = new DXFcircle();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "ARC":
                            entity = new DXFarc();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "POINT":
                            entity = new DXFpoint();
                            group = entity.Read(reader);
                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "INSERT":
                            entity = new DXFinsert();
                            group = entity.Read(reader);

                            (entity as DXFinsert).Block = FindBlock((entity as DXFinsert).BlockName);

                            entityList.Add(entity);
                            FindLayer(entity.Layer).Entities.Add(entity);
                            entity.LType = FindLType(entity.LineType);
                            break;

                        case "BLOCK":
                            DXFblock block = new DXFblock();
                            group = block.Read(reader);
                            blockList.Add(block);
                            break;

                        default:
                            if (!unusedAtomsList.Contains(group.Data))
                                unusedAtomsList.Add(group.Data);
                            group = DXFAtom.ReadLines(reader);
                            break;
                    }
                }
                else
                    group = DXFAtom.ReadLines(reader);
            }

            foreach (string unusedAtom in unusedAtomsList)
                Console.WriteLine(unusedAtom);

            reader.DiscardBufferedData();
            reader.Close();
        }
        #endregion

        public virtual void OnDraw(Graphics g, RectangleD rcRegion, double scale, double skipDetail)
        {
            PointD ptOffset = new PointD(0, 0);

            int max = LayerList.Count - 1;

            for (int i = max; i >= 0; i--)
                LayerList[i].Draw(g, rcRegion, ptOffset, scale, skipDetail);
        }

        public virtual void OnUpdate()
        {
            int max = LayerList.Count - 1;

            for (int i = max; i >= 0; i--)
                LayerList[i].Update();
        }

        public DXFLayer FindLayer(string name)
        {
            int max = LayerList.Count;

            for (int i = 0; i < max; i++)
                if (LayerList[i].Name == name)
                    return LayerList[i];

            return null;
        }

        public DXFLType FindLType(string name)
        {
            int max = LTypeList.Count;

            for (int i = 0; i < max; i++)
                if (LTypeList[i].Name == name)
                    return LTypeList[i];

            return null;
        }

        public DXFblock FindBlock(string name)
        {
            int max = blockList.Count;

            for (int i = 0; i < max; i++)
                if (blockList[i].Name == name)
                    return blockList[i];

            return null;
        }

        public void Dispose()
        {
            entityList.Clear();
            blockList.Clear();

            LayerList.Clear();
            LTypeList.Clear();
        }
    }
}
