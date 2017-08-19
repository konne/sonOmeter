using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using SlimDX;
using SlimDX.Direct3D9;
using UKLib.MathEx;
using UKLib.Survey.Math;
using UKLib;
using UKLib.Debug;
using System.Threading;

namespace sonOmeter.Classes
{
    #region 3D Record helper classes
    #region Cell and vertex position enumerations
    public enum CellPosition
    {
        BL = 0,
        ML = 1,
        TL = 2,
        BM = 3,
        MM = 4,
        TM = 5,
        BR = 6,
        MR = 7,
        TR = 8
    }

    public enum VertexPosition
    {
        MM = 0,
        TL = 1,
        TR = 2,
        BR = 3,
        BL = 4,
        TL2 = 5
    }

    public enum WallPosition
    {
        L = 0,
        T = 1,
        R = 2,
        B = 3
    }
    #endregion

    public enum Sonar3DDrawMode
    {
        BoundingBox,
        Wireframe,
        Solid
    }

    public enum Sonar3DIntMethod
    {
        NearestNeighbours,
        MaximumNeighbourDistance
    }

    public enum Sonar3DIntWeighting
    {
        Linear,
        Quadratic
    }

    #region Vertex types
    [StructLayout(LayoutKind.Sequential)]
    public struct PositionNormalColored
    {
        public Vector3 Position;
        public Vector3 Normal;
        public int Color;
        public static readonly VertexFormat Format = VertexFormat.Diffuse | VertexFormat.Position | VertexFormat.Normal;

        public readonly static VertexElement[] VertexElements = {
            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
            new VertexElement(0, 24, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
            VertexElement.VertexDeclarationEnd
        };

        public static readonly int Size = 28; // 4 * 7

        public PositionNormalColored(Vector3 position, Vector3 normal, int color)
        {
            this.Position = position;
            this.Color = color;
            this.Normal = normal;
        }

        public PositionNormalColored(float posX, float posY, float posZ, float normX, float normY, float normZ, int color)
        {
            this.Position = new Vector3(posX, posY, posZ);
            this.Color = color;
            this.Normal = new Vector3(normX, normY, normZ);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PositionColored
    {
        public Vector3 Position;
        public int Color;
        public static readonly VertexFormat Format = VertexFormat.Diffuse | VertexFormat.Position;

        public readonly static VertexElement[] VertexElements = {
            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
            VertexElement.VertexDeclarationEnd
        };

        public static readonly int Size = 16; // 4 * 4

        public PositionColored(Vector3 position, int color)
        {
            this.Position = position;
            this.Color = color;
        }

        public PositionColored(float posX, float posY, float posZ, int color)
        {
            this.Position = new Vector3(posX, posY, posZ);
            this.Color = color;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PositionColoredWall
    {
        public Vector3 Position;
        public int Color;
        public float Cell_Index;
        public float Z2;

        public static readonly VertexFormat Format = VertexFormat.Diffuse | VertexFormat.Position | VertexFormat.PositionBlend1 | VertexFormat.PointSize;

        public readonly static VertexElement[] VertexElements = {
            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Color, DeclarationMethod.Default, DeclarationUsage.Color, 0),
            new VertexElement(0, 16, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.BlendWeight, 0),
            new VertexElement(0, 20, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.PointSize, 0),
            VertexElement.VertexDeclarationEnd
        };

        public static readonly int Size = 24; // 6 * 4

        public PositionColoredWall(Vector3 position, int color, int cell_index, float z2)
        {
            this.Position = position;
            this.Color = color;
            this.Cell_Index = cell_index;
            this.Z2 = z2;
        }

        public PositionColoredWall(float posX, float posY, float posZ, int color, int cell_index, float z2)
        {
            this.Position = new Vector3(posX, posY, posZ);
            this.Color = color;
            this.Cell_Index = cell_index;
            this.Z2 = z2;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PositionNormal
    {
        public Vector3 Position;
        public Vector3 Normal;
        public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.Normal;

        public static readonly int Size = 24; // 4 * 6

        public PositionNormal(Vector3 position, Vector3 normal)
        {
            this.Position = position;
            this.Normal = normal;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Position
    {
        public Vector3 Pos;
        public static readonly VertexFormat Format = VertexFormat.Position;

        public readonly static VertexElement[] VertexElements = {
            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            VertexElement.VertexDeclarationEnd
        };

        public static readonly int Size = 12; // 4 * 3

        public Position(Vector3 position)
        {
            this.Pos = position;
        }

        public Position(float posX, float posY, float posZ)
        {
            this.Pos = new Vector3(posX, posY, posZ);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PositionCell
    {
        public Vector3 Pos;
        public float Cell_Index;

        public static readonly VertexFormat Format = VertexFormat.Position | VertexFormat.PositionBlend1;

        public readonly static VertexElement[] VertexElements = {
            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float1, DeclarationMethod.Default, DeclarationUsage.BlendWeight, 0),
            VertexElement.VertexDeclarationEnd
        };

        public static readonly int Size = 16; // 4 * 4

        public PositionCell(Vector3 position, int cell_index)
        {
            this.Pos = position;
            this.Cell_Index = (float)cell_index;
        }

        public PositionCell(float posX, float posY, float posZ, int cell_index)
        {
            this.Pos = new Vector3(posX, posY, posZ);
            this.Cell_Index = (float)cell_index;
        }
    }
    #endregion

    public class Rect3D : IDisposable
    {
        #region Variables
        private VertexBuffer vb = null;
        private PositionColored[] vertices = new PositionColored[5];
        #endregion

        #region Constructor and Dispose
        public Rect3D(Color color, Vector3[] v)
        {
            if (v.Length != 4)
                return;

            vertices[0] = new PositionColored(v[0], color.ToArgb());
            vertices[1] = new PositionColored(v[1], color.ToArgb());
            vertices[2] = new PositionColored(v[2], color.ToArgb());
            vertices[3] = new PositionColored(v[3], color.ToArgb());
            vertices[4] = new PositionColored(v[0], color.ToArgb());

            if (vb != null)
                this.OnCreateVertexBuffer(vb, null);
        }

        public void Dispose()
        {
            if (vb != null)
                vb.Dispose();

            if (vertices != null)
                vertices = null;
        }
        #endregion

        #region VertexBuffer management and drawing
        public void Init(Device dev)
        {
            if (dev == null)
                return;

            if (vertices == null)
                return;

            if (vertices.Length == 0)
                return;

            // Create the VertexBuffer.
            vb = new VertexBuffer(dev, vertices.Length * PositionColored.Size, Usage.WriteOnly, PositionColored.Format, Pool.Default);

            this.OnCreateVertexBuffer(vb, null);
        }

        public void OnCreateVertexBuffer(object sender, EventArgs e)
        {
            if ((vb != null) && (vertices != null))
            {
                vb.Lock(0, vertices.Length * PositionColored.Size, LockFlags.None).WriteRange<PositionColored>(vertices, 0, 0);
                vb.Unlock();
            }
        }

        public void Draw(Device dev)
        {
            if (vb == null)
                Init(dev);

            if ((vb != null) && !vb.Disposed)
            {
                dev.SetStreamSource(0, vb, 0, PositionColored.Size);
                dev.VertexFormat = PositionColored.Format;
                dev.DrawPrimitives(PrimitiveType.TriangleFan, 0, 2);
                dev.VertexFormat = Position.Format;
                dev.DrawPrimitives(PrimitiveType.LineStrip, 0, 4);
            }
        }
        #endregion
    }

    public class Box3D : IDisposable
    {
        #region Variables
        private Rect3D[] faces = new Rect3D[6];

        public BoxF Box { get; set; }
        #endregion

        #region Constructor and Dispose
        public Box3D()
        {
        }

        public void Dispose()
        {
            foreach (Rect3D face in faces)
                if (face != null)
                    face.Dispose();
        }
        #endregion

        #region VertexBuffer management and drawing
        public void Init(Device dev)
        {
            foreach (Rect3D face in faces)
                face.Init(dev);
        }

        public void Draw(Device dev)
        {
            foreach (Rect3D face in faces)
                face.Draw(dev);
        }
        #endregion

        #region Vertex management
        public void BuildVertices(BoxF box)
        {
            Box = box;

            Vector3[] vBox = Point3F2Vector3(box.Corners);
            Vector3[] v = new Vector3[4];
            //Vector3 vn;

            // tbd: Which color?
            Color color = Color.Gray;

            // We need 6 faces. Each one has its own orientation (normal).
            // Therefore each one has to be built seperately using 4 edge coordinates.

            // Front
            //vn = new Vector3(0, -1, 0);
            v[0] = vBox[(int)BoxCorners.xyz];
            v[1] = vBox[(int)BoxCorners.xyZ];
            v[2] = vBox[(int)BoxCorners.XyZ];
            v[3] = vBox[(int)BoxCorners.Xyz];
            faces[0] = new Rect3D(color, v);

            // Back
            //vn = new Vector3(0, 1, 0);
            v[0] = vBox[(int)BoxCorners.XYz];
            v[1] = vBox[(int)BoxCorners.XYZ];
            v[2] = vBox[(int)BoxCorners.xYZ];
            v[3] = vBox[(int)BoxCorners.xYz];
            faces[1] = new Rect3D(color, v);

            // Left
            //vn = new Vector3(-1, 0, 0);
            v[0] = vBox[(int)BoxCorners.xYz];
            v[1] = vBox[(int)BoxCorners.xYZ];
            v[2] = vBox[(int)BoxCorners.xyZ];
            v[3] = vBox[(int)BoxCorners.xyz];
            faces[2] = new Rect3D(color, v);

            // Right
            //vn = new Vector3(1, 0, 0);
            v[0] = vBox[(int)BoxCorners.Xyz];
            v[1] = vBox[(int)BoxCorners.XyZ];
            v[2] = vBox[(int)BoxCorners.XYZ];
            v[3] = vBox[(int)BoxCorners.XYz];
            faces[3] = new Rect3D(color, v);

            // Bottom
            //vn = new Vector3(0, 0, -1);
            v[0] = vBox[(int)BoxCorners.xYz];
            v[1] = vBox[(int)BoxCorners.xyz];
            v[2] = vBox[(int)BoxCorners.Xyz];
            v[3] = vBox[(int)BoxCorners.XYz];
            faces[4] = new Rect3D(color, v);

            // Top
            //vn = new Vector3(0, 0, 1);
            v[0] = vBox[(int)BoxCorners.XYZ];
            v[1] = vBox[(int)BoxCorners.XyZ];
            v[2] = vBox[(int)BoxCorners.xyZ];
            v[3] = vBox[(int)BoxCorners.xYZ];
            faces[5] = new Rect3D(color, v);
        }

        private Vector3[] Point3F2Vector3(Point3F[] pts)
        {
            int max = pts.Length;

            Vector3[] v = new Vector3[max];

            for (int i = 0; i < max; i++)
                v[i] = new Vector3(pts[i].X, pts[i].Y, pts[i].Z);

            return v;
        }
        #endregion
    }

    public class Sonar3DWall : IDisposable
    {
        #region Variables
        private VertexBuffer vb = null;
        private int triangleCount = 0;
        private PositionColoredWall[] vertices = null;
        private static VertexDeclaration vertDecl = null;
        private static EffectHandle valueDepth = null;
        float[] depths = new float[3];
        int clipL = 0, clipH = 0;
        #endregion

        #region Constructor and Dispose
        public void Init(Sonar3DRecord rec, SonarLine line, SonarPanelType type, PositionCell v01, PositionCell v02)
        {
            List<PositionColoredWall> verts = new List<PositionColoredWall>();
            ColorScheme cs = GSC.Settings.CS;
            var secl = GSC.Settings.SECL;

            LineData data = (type == SonarPanelType.HF) ? line.HF : line.NF;

            if (data.Entries == null)
                return;

            float depth = Math.Min(data.Depth, Math.Min(v01.Pos.Z, v02.Pos.Z));
            float zh = (float)rec.Depth3DTop, last_zh = (float)rec.Depth3DTop, zl = (float)rec.Depth3DBottom;
            int max = data.Entries.Length - 1;

            if (max < 0)
                return;

            // Store X and Y information.
            Vector3[] v = new Vector3[4];
            v[0] = v01.Pos;
            v[1] = v02.Pos;
            v[2] = v02.Pos;
            v[3] = v01.Pos;

            // Draw the bottom blue block.
            last_zh = zh = Math.Max(Math.Max(rec.Cut3DMultiples ? depth * 2.0F : zl, depth - (float)rec.MaxRange), data.Entries[max].low);

            // Store depths to vertex shader structure.
            depths[0] = v01.Pos.Z;
            depths[1] = v02.Pos.Z;
            depths[2] = zh;

            if (zl != zh)
                AddRect(verts, cs.BackColor, v, zl, zh);

            clipH = clipL = 0;

            // Build a rectangle for each visible element, starting with the bottom one.
            for (int i = max; i >= 0; i--)
            {
                DataEntry entry = data.Entries[i];

                zh = entry.high;
                zl = entry.low;

                // Draw the dummy filling structures.
                if (zl != last_zh)
                    AddRect(verts, cs.BackColor, v, last_zh, zl);

                // Draw the real rect.
                AddRect(verts, (entry.colorID != 0xff) ? secl[entry.colorID].SonarColor : System.Drawing.Color.White, v, zl, zh);

                // Clip or skip if below lowest depth.
                if (zl < last_zh)
                {
                    if (zh < last_zh)
                        clipL = verts.Count / 3;

                    //if (zh < last_zh)
                    //    continue;
                    //else
                    //    zl = last_zh;
                }

                // Visible? If not, break!
                if ((zh > depth) && (clipH != 0))
                {
                    if (zl >= depth)
                        clipH = verts.Count / 3;

                    //if (zl >= depth)
                    //    break;
                    //else
                    //    zh = depth;
                }

                last_zh = zh;
            }

            if (clipH == 0)
                clipH = verts.Count / 3;

            // Copy buffer.
            vertices = new PositionColoredWall[verts.Count];
            verts.CopyTo(vertices, 0);
            verts.Clear();
        }

        public Sonar3DWall(Sonar3DRecord rec, SonarLine line, SonarPanelType type, PositionCell v01, PositionCell v02)
        {
            Init(rec, line, type, v01, v02);
        }

        public void AddRect(List<PositionColoredWall> verts, Color color, Vector3[] v, float bottom, float top)
        {
            int col = color.ToArgb();

            int off = (color == GSC.Settings.CS.BackColor) ? 2 : 0;

            // Create two new triangles for this block.
            v[0].Z = v[1].Z = top;
            v[2].Z = v[3].Z = bottom;

            verts.Add(new PositionColoredWall(v[0], col, off + 0, v[3].Z));
            verts.Add(new PositionColoredWall(v[1], col, off + 1, v[2].Z));
            verts.Add(new PositionColoredWall(v[3], col, off + 0, v[0].Z));
            verts.Add(new PositionColoredWall(v[1], col, off + 1, v[2].Z));
            verts.Add(new PositionColoredWall(v[2], col, off + 1, v[1].Z));
            verts.Add(new PositionColoredWall(v[3], col, off + 0, v[0].Z));
        }

        public void Dispose()
        {
            if (vb != null)
                vb.Dispose();

            if (vertices != null)
                vertices = null;
        }

        public static void DisposeStatic()
        {
            if ((vertDecl != null) && !vertDecl.Disposed)
                vertDecl.Dispose();
            vertDecl = null;
        }
        #endregion

        #region VertexBuffer management and drawing
        public void Init(Device dev)
        {
            if (dev == null)
                return;

            if (vertices == null)
                return;

            if (vertices.Length == 0)
                return;

            // Create the VertexBuffer.
            vb = new VertexBuffer(dev, vertices.Length * PositionColoredWall.Size, Usage.WriteOnly, PositionColoredWall.Format, Pool.Default);

            if (vertDecl == null)
                vertDecl = new VertexDeclaration(dev, PositionColoredWall.VertexElements);

            this.OnCreateVertexBuffer(vb, null);
        }

        public void OnCreateVertexBuffer(object sender, EventArgs e)
        {
            try
            {
                VertexBuffer vb = (VertexBuffer)sender;

                if ((vb != null) && (vertices != null))
                {
                    triangleCount = vertices.Length / 3;
                    vb.Lock(0, vertices.Length * PositionColoredWall.Size, LockFlags.None).WriteRange<PositionColoredWall>(vertices, 0, 0);
                    vb.Unlock();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void Draw(Device dev, Effect effect)
        {
            if (vb == null)
                Init(dev);

            if ((vb == null) || vb.Disposed)
                return;

            if ((effect == null) || effect.Disposed)
                return;

            if (valueDepth == null)
                valueDepth = effect.GetParameter(null, "depthWall");

            dev.SetStreamSource(0, vb, 0, PositionColoredWall.Size);
            dev.VertexDeclaration = vertDecl;

            effect.SetValue<float>(valueDepth, depths);

            effect.Begin();
            effect.BeginPass(2);
            dev.DrawPrimitives(PrimitiveType.TriangleList, clipL * 3, clipH - clipL);
            effect.EndPass();
            effect.End();
        }
        #endregion
    }

    public class Sonar3DCell : IDisposable
    {
        #region Variables
        private Sonar3DWall[] walls = new Sonar3DWall[4];
        private SonarLine line = null;
        private SonarPanelType type = SonarPanelType.HF;
        private VertexBuffer vb = null;
        private static Effect effect = null;
        private static VertexDeclaration vertDecl = null;
        private static EffectHandle valueWVP = null;
        private static EffectHandle valueHeightFactor = null;
        private static EffectHandle valueBackColor = null;
        private static EffectHandle valueColor = null;
        private static EffectHandle valueDepth = null;
        private PositionCell[] vertices = null;
        private Sonar3DCell[] neighbourhood;
        float[] depths = new float[5];
        private int color = 0;
        #endregion

        #region Properties
        public SonarLine Line
        {
            get { return line; }
        }
        #endregion

        #region Constructor and Dispose
        public Sonar3DCell(SonarLine line)
        {
            this.line = line;
        }

        public void Dispose()
        {
            if ((vb != null) && !vb.Disposed)
                vb.Dispose();

            if (vertices != null)
                vertices = null;

            foreach (Sonar3DWall wall in walls)
                if (wall != null)
                    wall.Dispose();
        }

        public static void DisposeStatic()
        {
            if ((effect != null) && !effect.Disposed)
                effect.Dispose();
            effect = null;

            if ((vertDecl != null) && !vertDecl.Disposed)
                vertDecl.Dispose();
            vertDecl = null;
        }
        #endregion

        #region VertexBuffer management and drawing
        public void Init(Device dev)
        {
            if (dev == null)
                return;

            // Create the VertexBuffer.
            vb = new VertexBuffer(dev, 6 * PositionCell.Size, Usage.Dynamic, PositionCell.Format, Pool.Default);

            if (effect == null)
            {
                effect = Effect.FromStream(dev, System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("sonOmeter.Classes.Sonar3DCell.fx"), ShaderFlags.None);
                var technique = effect.GetTechnique(0);
                effect.Technique = technique;
                valueWVP = effect.GetParameter(null, "WorldViewProj");
                valueColor = effect.GetParameter(null, "col");
                valueBackColor = effect.GetParameter(null, "backCol");
                valueDepth = effect.GetParameter(null, "depth");
                valueHeightFactor = effect.GetParameter(null, "heightFactor");

                vertDecl = new VertexDeclaration(dev, PositionCell.VertexElements);
            }

            this.OnCreateVertexBuffer(vb, null);
        }

        public void SetVertexBuffer(Sonar3DCell cell)
        {
            if (cell.vb == null)
                return;

            vb = cell.vb;

            this.OnCreateVertexBuffer(vb, null);
        }

        public void OnCreateVertexBuffer(object sender, EventArgs e)
        {
            try
            {
                VertexBuffer vb = (VertexBuffer)sender;

                if ((vb != null) && (vertices != null))
                {
                    DataStream stream = vb.Lock(0, 6 * PositionCell.Size, LockFlags.Discard);
                    stream.WriteRange<PositionCell>(vertices, 0, 0);
                    vb.Unlock();
                }
            }
            catch { }
        }

        public void Draw(Device dev, Matrix wvp, ref bool setWVP)
        {
            if (vb == null)
                Init(dev);

            if ((vb == null) || vb.Disposed)
                return;

            if ((effect == null) || effect.Disposed)
                return;

            dev.SetStreamSource(0, vb, 0, PositionCell.Size);
            dev.VertexDeclaration = vertDecl;

            if (setWVP)
            {
                effect.SetValue<Matrix>(valueWVP, wvp);
                effect.SetValue<float>(valueHeightFactor, GSC.Settings.HeightFactor);
                effect.SetValue<Color4>(valueBackColor, new Color4(GSC.Settings.CS.BackColor));
                setWVP = false;
            }
            effect.SetValue<Color4>(valueColor, new Color4(color));
            effect.SetValue<float>(valueDepth, depths);

            effect.Begin();
            effect.BeginPass(0);
            dev.DrawPrimitives(PrimitiveType.TriangleFan, 0, 4);
            effect.EndPass();

            if (GSC.Settings.Show3DSurfaceGrid)
            {
                effect.BeginPass(1);
                dev.DrawPrimitives(PrimitiveType.LineStrip, 1, 4);
                effect.EndPass();
            }

            effect.End();

            if (!GSC.Settings.Show3DWalls)
                return;

            for (int i = 0; i < 4; i++)
                if (walls[i] != null)
                    walls[i].Draw(dev, effect);
        }
        #endregion

        #region Vertex management
        public void BuildVertices(Sonar3DRecord rec, Sonar3DCell[] neighbourhood, SonarPanelType type, TopColorMode mode, PointD ptOrigin, double gridX, double gridY)
        {
            List<Sonar3DCell> list = new List<Sonar3DCell>();
            Sonar3DCell cell = null;
            ColorScheme cs = GSC.Settings.CS;
            var secl = GSC.Settings.SECL;
            this.neighbourhood = neighbourhood;

            this.type = type;

            LineData data = (type == SonarPanelType.HF) ? line.HF : line.NF;
            PointD pt = line.CoordRvHv.Point - ptOrigin;
            float left = (float)(pt.X - gridX / 2.0);
            float top = (float)(pt.Y + gridY / 2.0);
            float right = (float)(pt.X + gridX / 2.0);
            float bottom = (float)(pt.Y - gridY / 2.0);
            int sum = 0;

            #region Get neighbour depths
            float[] neighbour_depths = new float[9];

            neighbour_depths[(int)CellPosition.TL] = SetNeighbourDepth(neighbourhood[(int)CellPosition.TL]);
            neighbour_depths[(int)CellPosition.TM] = SetNeighbourDepth(neighbourhood[(int)CellPosition.TM]);
            neighbour_depths[(int)CellPosition.TR] = SetNeighbourDepth(neighbourhood[(int)CellPosition.TR]);
            neighbour_depths[(int)CellPosition.ML] = SetNeighbourDepth(neighbourhood[(int)CellPosition.ML]);
            neighbour_depths[(int)CellPosition.MM] = SetNeighbourDepth(this);
            neighbour_depths[(int)CellPosition.MR] = SetNeighbourDepth(neighbourhood[(int)CellPosition.MR]);
            neighbour_depths[(int)CellPosition.BL] = SetNeighbourDepth(neighbourhood[(int)CellPosition.BL]);
            neighbour_depths[(int)CellPosition.BM] = SetNeighbourDepth(neighbourhood[(int)CellPosition.BM]);
            neighbour_depths[(int)CellPosition.BR] = SetNeighbourDepth(neighbourhood[(int)CellPosition.BR]);
            #endregion

            #region Top color selection
            switch (mode)
            {
                case TopColorMode.Top:
                    color = (data.TopColor != -1) ? secl[data.TopColor].SonarColor.ToArgb() : System.Drawing.Color.White.ToArgb();
                    break;
                case TopColorMode.Vol:
                    sum = (int)data.GetVolume(false);
                    if (sum < 0)
                        sum = 0;
                    if (sum > 255)
                        sum = 255;
                    color = Color.FromArgb(sum, sum, sum).ToArgb();
                    break;
                case TopColorMode.Dep:
                    sum = (int)(255.0 * (rec.Depth3DTop - (double)data.Depth) / (rec.Depth3DTop - rec.Depth3DBottom));
                    if (sum < 0)
                        sum = 0;
                    if (sum > 255)
                        sum = 255;
                    sum = 255 - sum;
                    color = Color.FromArgb(sum, sum, sum).ToArgb();
                    break;
            }
            #endregion

            #region MM vertex
            vertices = new PositionCell[6];

            depths[(int)VertexPosition.MM] = neighbour_depths[(int)CellPosition.MM];
            vertices[(int)VertexPosition.MM] = new PositionCell((float)pt.X, (float)pt.Y, depths[(int)VertexPosition.MM], (int)VertexPosition.MM);
            #endregion

            #region BL vertex
            list.Clear();
            list.Add(this);

            cell = neighbourhood[(int)CellPosition.BL];
            if (cell != null)
                list.Add(cell);

            cell = neighbourhood[(int)CellPosition.BM];
            if (cell != null)
                list.Add(cell);

            cell = neighbourhood[(int)CellPosition.ML];
            if (cell != null)
                list.Add(cell);

            depths[(int)VertexPosition.BL] = (neighbour_depths[(int)CellPosition.BL] + neighbour_depths[(int)CellPosition.BM] + neighbour_depths[(int)CellPosition.ML] + neighbour_depths[(int)CellPosition.MM]) / (float)list.Count;
            vertices[(int)VertexPosition.BL] = new PositionCell(left, bottom, depths[(int)VertexPosition.BL], (int)VertexPosition.BL);
            #endregion

            #region BR vertex
            list.Clear();
            list.Add(this);

            cell = neighbourhood[(int)CellPosition.BM];
            if (cell != null)
                list.Add(cell);

            cell = neighbourhood[(int)CellPosition.BR];
            if (cell != null)
                list.Add(cell);

            cell = neighbourhood[(int)CellPosition.MR];
            if (cell != null)
                list.Add(cell);

            depths[(int)VertexPosition.BR] = (neighbour_depths[(int)CellPosition.BM] + neighbour_depths[(int)CellPosition.BR] + neighbour_depths[(int)CellPosition.MR] + neighbour_depths[(int)CellPosition.MM]) / (float)list.Count;
            vertices[(int)VertexPosition.BR] = new PositionCell(right, bottom, depths[(int)VertexPosition.BR], (int)VertexPosition.BR);
            #endregion

            #region TL vertex
            list.Clear();
            list.Add(this);

            cell = neighbourhood[(int)CellPosition.ML];
            if (cell != null)
                list.Add(cell);

            cell = neighbourhood[(int)CellPosition.TL];
            if (cell != null)
                list.Add(cell);

            cell = neighbourhood[(int)CellPosition.TM];
            if (cell != null)
                list.Add(cell);

            depths[(int)VertexPosition.TL] = (neighbour_depths[(int)CellPosition.TL] + neighbour_depths[(int)CellPosition.TM] + neighbour_depths[(int)CellPosition.ML] + neighbour_depths[(int)CellPosition.MM]) / (float)list.Count;
            vertices[(int)VertexPosition.TL] = new PositionCell(left, top, depths[(int)VertexPosition.TL], (int)VertexPosition.TL);

            vertices[(int)VertexPosition.TL2] = vertices[(int)VertexPosition.TL];
            #endregion

            #region TR vertex
            list.Clear();
            list.Add(this);

            cell = neighbourhood[(int)CellPosition.TM];
            if (cell != null)
                list.Add(cell);

            cell = neighbourhood[(int)CellPosition.TR];
            if (cell != null)
                list.Add(cell);

            cell = neighbourhood[(int)CellPosition.MR];
            if (cell != null)
                list.Add(cell);

            depths[(int)VertexPosition.TR] = (neighbour_depths[(int)CellPosition.TM] + neighbour_depths[(int)CellPosition.TR] + neighbour_depths[(int)CellPosition.MR] + neighbour_depths[(int)CellPosition.MM]) / (float)list.Count;
            vertices[(int)VertexPosition.TR] = new PositionCell(right, top, depths[(int)VertexPosition.TR], (int)VertexPosition.TR);
            #endregion

            #region Walls
            cell = neighbourhood[(int)CellPosition.BM];
            if (cell == null)
            {
                if (walls[(int)WallPosition.B] != null)
                    walls[(int)WallPosition.B].Init(rec, line, type, vertices[(int)VertexPosition.BL], vertices[(int)VertexPosition.BR]);
                else
                    walls[(int)WallPosition.B] = new Sonar3DWall(rec, line, type, vertices[(int)VertexPosition.BL], vertices[(int)VertexPosition.BR]);
            }

            cell = neighbourhood[(int)CellPosition.TM];
            if (cell == null)
            {
                if (walls[(int)WallPosition.T] != null)
                    walls[(int)WallPosition.T].Init(rec, line, type, vertices[(int)VertexPosition.TR], vertices[(int)VertexPosition.TL]);
                else
                    walls[(int)WallPosition.T] = new Sonar3DWall(rec, line, type, vertices[(int)VertexPosition.TR], vertices[(int)VertexPosition.TL]);
            }

            cell = neighbourhood[(int)CellPosition.ML];
            if (cell == null)
            {
                if (walls[(int)WallPosition.L] != null)
                    walls[(int)WallPosition.L].Init(rec, line, type, vertices[(int)VertexPosition.TL], vertices[(int)VertexPosition.BL]);
                else
                    walls[(int)WallPosition.L] = new Sonar3DWall(rec, line, type, vertices[(int)VertexPosition.TL], vertices[(int)VertexPosition.BL]);
            }

            cell = neighbourhood[(int)CellPosition.MR];
            if (cell == null)
            {
                if (walls[(int)WallPosition.R] != null)
                    walls[(int)WallPosition.R].Init(rec, line, type, vertices[(int)VertexPosition.BR], vertices[(int)VertexPosition.TR]);
                else
                    walls[(int)WallPosition.R] = new Sonar3DWall(rec, line, type, vertices[(int)VertexPosition.BR], vertices[(int)VertexPosition.TR]);
            }
            #endregion
        }

        public float SetNeighbourDepth(Sonar3DCell cell)
        {
            if (cell == null)
                return 0;

            return (type == SonarPanelType.HF ? cell.line.HF : cell.line.NF).Depth;
        }
        #endregion
    }

    internal struct Sonar3DLineAndPoint
    {
        public SonarLine Line;
        public int X;
        public int Y;

        internal void UpdateLine(SonarLine sonarLine)
        {
            this.Line = sonarLine;
        }
    }
    #endregion

    public class DisposableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IDisposable where TValue : IDisposable
    {
        public void Dispose()
        {
            foreach (TValue value in this.Values)
                value.Dispose();
        }
    }

    public class Sonar3DRecord : SonarRecord, INotifyPropertyChanged
    {
        private class BWResult
        {
            public bool tryKeepAppend;
        }

        #region Variables
        private Sonar3DCell[,] surface = null;
        private Size surfaceSize = new Size(0, 0);
        private Box3D box = new Box3D();

        private SonarPanelType lastType = SonarPanelType.HF;
        private TopColorMode lastMode = TopColorMode.Top;
        private BlankLine blankLine = null;
        private SonarProject project = null;
        private bool showInTrace = true;
        private bool cut3DMultiples = false;
        private double depth3DTop = 0;
        private double depth3DBottom = -100;
        private double maxRange = 50;
        private PointD ptOrigin = PointD.Origin;
        private bool isDrawing = false;
        private bool isInterpolating = false;
        private bool isMeshing = false;

        private Sonar3DIntMethod intMethod = Sonar3DIntMethod.MaximumNeighbourDistance;
        private Sonar3DIntWeighting intWeighting = Sonar3DIntWeighting.Quadratic;
        private int intThreshold = 5;
        private int intIterations = 5;
        private LineData.MergeMode mode = LineData.MergeMode.Strongest;

        BackgroundWorker<bool, bool> bwInterpolate = null;
        BackgroundWorker<bool, BWResult> bwBuildMesh = null;
        bool bwInterpolateRestart = false;
        bool bwBuildMeshRestart = false;
        #endregion

        #region Properties
        private void NotifyPropertyChanged(String info)
        {
            //Interpolate();

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info));
        }

        [Browsable(false)]
        public bool IsInterpolating
        {
            //get { return (bwInterpolate == null) ? false : bwInterpolate.IsBusy; }
            get { return isInterpolating; }
        }

        [Browsable(false)]
        public bool IsMeshing
        {
            //get { return (bwBuildMesh == null) ? false : bwBuildMesh.IsBusy; }
            get { return isMeshing; }
        }        
        
        [Browsable(false), Category("3D surface"), DisplayName("X grid spacing (m)"), DefaultValue(1.0)]
        public double GridX { get; set; }
        [Browsable(false), Category("3D surface"), DisplayName("Y grid spacing (m)"), DefaultValue(1.0)]
        public double GridY { get; set; }

        [Browsable(false)]
        public bool EnableInterpolation { get; set; }

        #region Hidden properties from SonarRecord base
        [Browsable(false)]
        public override string LinkedVideoFile
        {
            get { return base.LinkedVideoFile; }
            set { base.LinkedVideoFile = value; }
        }

        [Browsable(false)]
        public new bool ShowInTrace
        {
            get { return showInTrace; }
            set { showInTrace = value; }
        }

        /// <summary>
        /// The start time of the record.
        /// </summary>
        [Browsable(false)]
        public override DateTime TimeStart
        {
            get { return timeStart; }
            set { timeStart = value; }
        }

        /// <summary>
        /// The end time of the record.
        /// </summary>
        [Browsable(false)]
        public override DateTime TimeEnd
        {
            get { return timeEnd; }
            set { timeEnd = value; }
        }

        [Browsable(false)]
        public override bool Interpolated
        {
            get { return true; }
        }
        #endregion

        [Browsable(false)]
        public BlankLine BlankLine
        {
            get { return blankLine; }
            set { blankLine = value; }
        }

        [Browsable(false)]
        public SonarProject Project
        {
            get { return project; }
            set { project = value; }
        }

        [Browsable(false)]
        public PointD Origin
        {
            get { return ptOrigin; }
            set { ptOrigin = value; Build3DMesh(); }
        }

        /// <summary>
        /// Gets or sets the interpolation threshold.
        /// </summary>
        [Description("Gets or sets the interpolation threshold (maximum number of nearest neighbours or maximum neighbour distance in grid units)."), Category("3D surface"), DefaultValue(5), DisplayName("3D Interpolation Threshold")]
        public int Int3DThreshold
        {
            get { return intThreshold; }
            set { intThreshold = value; NotifyPropertyChanged("Int3DThreshold"); Interpolate(); }
        }

        /// <summary>
        /// Gets or sets the interpolation iteration count.
        /// </summary>
        [Description("Gets or sets the interpolation iteration count."), Category("3D surface"), DefaultValue(5), DisplayName("3D Interpolation Iterations")]
        public int Int3DIterations
        {
            get { return intIterations; }
            set { intIterations = value; NotifyPropertyChanged("Int3DIterations"); Interpolate(); }
        }

        /// <summary>
        /// Gets or sets the interpolation method.
        /// </summary>
        [Description("Gets or sets the interpolation method (nearest neighbours or maximum neighbour distance)."), Category("3D surface"), DefaultValue(Sonar3DIntMethod.MaximumNeighbourDistance), DisplayName("3D Interpolation Method")]
        public Sonar3DIntMethod Int3DMethod
        {
            get { return intMethod; }
            set { intMethod = value; NotifyPropertyChanged("Int3DMethod"); Interpolate(); }
        }

        /// <summary>
        /// Gets or sets the interpolation method.
        /// </summary>
        [Description("Gets or sets the interpolation weighting type (linear or quadratic)."), Category("3D surface"), DefaultValue(Sonar3DIntWeighting.Quadratic), DisplayName("3D Interpolation Weighting")]
        public Sonar3DIntWeighting Int3DWeighting
        {
            get { return intWeighting; }
            set { intWeighting = value; NotifyPropertyChanged("Int3DWeighting"); Interpolate(); }
        }

        /// <summary>
        /// Gets or sets the color interpolation mode.
        /// </summary>
        [Description("The color interpolation mode. The interpolation picks the color of each block based on most occurances of a color or the strongest of all present colors."), Category("3D surface"), DisplayName("Color interpolation mode"), DefaultValue(LineData.MergeMode.Strongest)]
        public LineData.MergeMode Mode
        {
            get { return mode; }
            set { mode = value; NotifyPropertyChanged("Mode"); Interpolate(); }
        }

        /// <summary>
        /// Gets or sets the multiple flag.
        /// </summary>
        [Description("Toggles the extraction of the maximum depth from the 1st multiple."), Category("3D surface"), DefaultValue(false), DisplayName("Cut Multiples"), LicencedProperty(Module.Modules.ThreeD)]
        public bool Cut3DMultiples
        {
            get { return cut3DMultiples; }
            set { cut3DMultiples = value; NotifyPropertyChanged("Cut3DMultiples"); Build3DMesh(); }
        }

        /// <summary>
        /// Gets or sets the maximum range of the sonar.
        /// </summary>
        [Description("Specifies the maximum range of the sonar."), Category("3D surface"), DefaultValue(50.0), DisplayName("Maximum Range"), LicencedProperty(Module.Modules.ThreeD)]
        public double MaxRange
        {
            get { return maxRange; }
            set { maxRange = value; NotifyPropertyChanged("MaxRange"); Build3DMesh(); }
        }

        /// <summary>
        /// Gets or sets the top depth for 3D.
        /// </summary>
        [Description("This value controls the top limit in the 3D view in meters."), Category("3D surface"), DefaultValue(0.0), DisplayName("Depth Limit (Top)")]
        public double Depth3DTop
        {
            get { return depth3DTop; }
            set { depth3DTop = value; NotifyPropertyChanged("Depth3DTop"); ApplyArchAndVolume(); }
        }

        /// <summary>
        /// Gets or sets the bottom depth for 3D.
        /// </summary>
        [Description("This value controls the bottom limit in the 3D view in meters."), Category("3D surface"), DefaultValue(-100.0), DisplayName("Depth Limit (Bottom)")]
        public double Depth3DBottom
        {
            get { return depth3DBottom; }
            set { depth3DBottom = value; NotifyPropertyChanged("Depth3DBottom"); Build3DMesh(); }
        }
        #endregion

        #region Constructor and Dispose
        public Sonar3DRecord()
        {
            this.Description = "3D Record";
        }

        public override void Dispose()
        {
            for (int x = 0; x < surfaceSize.Width; x++)
                for (int y = 0; y < surfaceSize.Height; y++)
                    if (surface[x, y] != null)
                        surface[x, y].Dispose();
            
            box.Dispose();

            base.Dispose();
        }
        #endregion

        #region 3D cell list functions
        public Sonar3DCell Get3DCell(Point pt)
        {
            if ((pt.X < 0) || (pt.X >= surfaceSize.Width))
                return null;

            if ((pt.Y < 0) || (pt.Y >= surfaceSize.Height))
                return null;

            return surface[pt.X, pt.Y];
        }

        public Sonar3DCell[] Get3DNeighbours(Point pt)
        {
            Sonar3DCell[] list = new Sonar3DCell[9];
            int i = 0;

            for (int x = pt.X - 1; x <= pt.X + 1; x++)
                for (int y = pt.Y - 1; y <= pt.Y + 1; y++)
                    list[i++] = Get3DCell(new Point(x, y));

            return list;
        }
        #endregion

        #region Sonar line management
        public int AddSonarLine(Point pt, SonarLine line)
        {
            line.IsProfile = true;
            RefreshCoordLimits(line.CoordRvHv, GSC.Settings.InterpolationThreshold, GSC.Settings.InterpolationAltitudeThreshold);

            sonarLines.Add(line);

            // Add 3D cell
            Sonar3DCell cell = new Sonar3DCell(line);
            
            if ((pt.X >= 0) & (pt.X < surfaceSize.Width) & (pt.Y >= 0) & (pt.Y < surfaceSize.Height))
                surface[pt.X, pt.Y] = cell;

            if (sonarDevices.Count > 0)
                sonarDevices[0].AddLine(line);

            return 0;
        }

        /// <summary>
        /// Adds a new line to the list and recalculates the bounding rectangle.
        /// </summary>
        /// <param name="line">The sonar line.</param>
        /// <returns>The index of the line.</returns>
        /// <param name="scanPos">Scan for positions in the sonar line - ignored for 3D records.</param>
        public override object AddSonarLine(SonarLine line, bool scanPos)
        {
            return AddSonarLine((line.CoordRvHv.Point - coordLimits.BL).Scale(1.0 / GridX, 1.0 / GridY).Offset(-0.5, -0.5).Point, line);
        }

        public override void UpdateAllCoordinates()
        {
            // Skip...
        }

        /// <summary>
        /// Recalculates the bounding box with a specific coordinate.
        /// </summary>
        /// <param name="coord">The coordinate to merge with the existing bounding box.</param>
        /// <param name="maxRVHV">The maximum difference between the coordinate and the limit bounds.</param>
        /// <param name="maxAL">The maximum difference betweeen the coordinate altitude and the limit bounds.</param>
        /// <returns>True, if the limits were updated.</returns>
        public override bool RefreshCoordLimits(Coordinate coord, double maxRVHV, double maxAL)
        {
            // Calculate new bounding box.
            PointD pt = coord.Point;

            if (coordLimits.IsZero)
                coordLimits = new RectangleD(pt);
            else if (!pt.IsZero)
            {
                coordLimits.Merge(pt);
                lastAltitude = coord.AL;
            }
            else
                return false;

            return true;
        }
        #endregion

        #region 3D cell management
        public void Init3DCells(Device dev)
        {
            for (int x = 0; x < surfaceSize.Width; x++)
                for (int y = 0; y < surfaceSize.Height; y++)
                    if (surface[x, y] != null)
                        surface[x, y].Init(dev);

            box.Init(dev);
        }
        
        public bool Draw3DCells(Device dev)
        {
            isDrawing = true;

            if (this.IsInterpolating || this.IsMeshing)
            {
                isDrawing = false;
                return true;
            }

            bool setWVP = true;
            Matrix wvp = dev.GetTransform(TransformState.World) * (dev.GetTransform(TransformState.View) * dev.GetTransform(TransformState.Projection));

            for (int x = 0; x < surfaceSize.Width; x++)
                for (int y = 0; y < surfaceSize.Height; y++)
                    if (surface[x, y] != null)
                        surface[x, y].Draw(dev, wvp, ref setWVP);

            isDrawing = false;

            return true;
        }

        public bool DrawBoundingBox(Device dev)
        {
            isDrawing = true;

            box.Draw(dev);

            isDrawing = false;

            return true;
        }
        #endregion

        #region Read / Write operations
        public Sonar3DRecord(SonarProject project, XmlTextReader readerXML, BinaryReader readerBin)
        {
            this.Description = "3D Record";
            this.project = project;

            // Read XML values.
            if (readerXML.GetAttribute("desc") != null)
                desc = readerXML.GetAttribute("desc");
            if (readerXML.GetAttribute("grid") != null)
            {
                this.GridX = double.Parse(readerXML.GetAttribute("grid"));
                this.GridY = double.Parse(readerXML.GetAttribute("grid"));
            }
            if (readerXML.GetAttribute("gridX") != null)
                this.GridX = double.Parse(readerXML.GetAttribute("gridX"));
            if (readerXML.GetAttribute("gridY") != null)
                this.GridY = double.Parse(readerXML.GetAttribute("gridY"));

            if (readerXML.GetAttribute("mode") != null)
                this.Mode = (LineData.MergeMode)Enum.Parse(typeof(LineData.MergeMode), readerXML.GetAttribute("mode"));

            if (readerXML.GetAttribute("intMethod") != null)
                intMethod = (Sonar3DIntMethod)Enum.Parse(typeof(Sonar3DIntMethod), readerXML.GetAttribute("intMethod"));

            if (readerXML.GetAttribute("intWeighting") != null)
                intWeighting = (Sonar3DIntWeighting)Enum.Parse(typeof(Sonar3DIntWeighting), readerXML.GetAttribute("intWeighting"));

            if (readerXML.GetAttribute("intThreshold") != null)
                intThreshold = int.Parse(readerXML.GetAttribute("intThreshold"));

            if (readerXML.GetAttribute("cut3DMultiples") != null)
                cut3DMultiples = bool.Parse(readerXML.GetAttribute("cut3DMultiples"));

            if (readerXML.GetAttribute("depth3DTop") != null)
                depth3DTop = double.Parse(readerXML.GetAttribute("depth3DTop"));

            if (readerXML.GetAttribute("depth3DBottom") != null)
                depth3DBottom = double.Parse(readerXML.GetAttribute("depth3DBottom"));

            if (readerXML.GetAttribute("maxRange") != null)
                maxRange = double.Parse(readerXML.GetAttribute("maxRange"));

            readerXML.Read();
            blankLine = new BlankLine();
            blankLine.ReadFromXml(readerXML);

            EnableInterpolation = true;

            try
            {
                Interpolate();
            }
            catch
            {
                // Drop events...
            }
        }

        public override void Write(XmlTextWriter writerXML, BinaryWriter writerBin, bool binary)
        {
            WriteHeader(writerXML);
            WriteFooter(writerXML);
        }

        protected override void WriteHeader(XmlTextWriter writer)
        {
            try
            {
                NumberFormatInfo nfi = GSC.Settings.NFI;

                // Start record node.
                writer.WriteStartElement("record3D");

                // Write attributes.
                writer.WriteAttributeString("desc", desc);
                writer.WriteAttributeString("gridX", GridX.ToString(nfi));
                writer.WriteAttributeString("gridY", GridY.ToString(nfi));
                writer.WriteAttributeString("mode", Mode.ToString());
                writer.WriteAttributeString("intMethod", intMethod.ToString());
                writer.WriteAttributeString("intWeighting", intWeighting.ToString());
                writer.WriteAttributeString("intThreshold", intThreshold.ToString());
                writer.WriteAttributeString("cut3DMultiples", cut3DMultiples.ToString());
                writer.WriteAttributeString("depth3DTop", depth3DTop.ToString(nfi));
                writer.WriteAttributeString("depth3DBottom", depth3DBottom.ToString(nfi));
                writer.WriteAttributeString("maxRange", maxRange.ToString(nfi));

                blankLine.WriteToXml(writer, false);
            }
            catch (Exception e)
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Sonar3DRecord.WriteHeader: " + e.Message);
            }
        }

        class WriteSurferData
        {
            public double ZMin = double.MaxValue;
            public double ZMax = double.MinValue;
            public List<string> Lines = new List<string>();
            public string Line = "";

            public string FormatCode = "";
            public NumberFormatInfo NFI = new CultureInfo("en-US").NumberFormat;

            public void NewLine()
            {
                Lines.Add(Line);
                Line = "";
            }

            public void AddPoint(double z)
            {
                if (z < ZMin)
                    ZMin = z;
                if (z > ZMax)
                    ZMax = z;

                if (Line.Length > 0)
                    Line += " ";

                Line += z.ToString(FormatCode, NFI);
            }

            public void Write(StreamWriter writer, int numX, int numY, PointD bl, PointD tr)
            {
                if (writer == null)
                    return;

                writer.WriteLine("DSAA");
                writer.WriteLine(numX.ToString() + " " + numY.ToString());
                writer.WriteLine(bl.X.ToString("0.00000", NFI) + " " + tr.X.ToString("0.00000", NFI));
                writer.WriteLine(bl.Y.ToString("0.00000", NFI) + " " + tr.Y.ToString("0.00000", NFI));
                writer.WriteLine(ZMin.ToString(FormatCode, NFI) + " " + ZMax.ToString(FormatCode, NFI));

                int max = Lines.Count;

                for (int i = 0; i < max; i++)
                {
                    writer.WriteLine(Lines[i].Replace("NaN", ZMin.ToString(FormatCode, NFI)));
                }
            }
        }

        /// <summary>
        /// Writes Surfer files.
        /// </summary>
        /// <param name="dstFile">The destination path and file name.</param>
        /// <param name="writeZ">Toggles the creation of a height map.</param>
        /// <param name="writeB">Toggles the creation of a blank file.</param>
        /// <param name="writeV">Toggles the creation of a volume map.</param>
        /// <param name="writeC">Toggles the creation of a color map.</param>
        /// <param name="elliptic">Toggles elliptic coordinates.</param>
        /// <param name="type">Sets the used sonar data type (HF/NF).</param>
        public void WriteSurfer(string dstFile, bool writeZ, bool writeB, bool writeV, bool writeC, bool elliptic, SonarPanelType type)
        {
            NumberFormatInfo nfi = new CultureInfo("en-US").NumberFormat;

            string path = Path.GetDirectoryName(dstFile);
            string ext = Path.GetExtension(dstFile);
            string fileBase = Path.GetFileNameWithoutExtension(dstFile);

            RecalcVolume();

            // Write Blankline. Ignore extension and add BLN for blank file.
            if (writeB && blankLine != null)
                blankLine.WriteToFileSurfer(path + "\\" + fileBase + ".BLN", elliptic);

            // Prepare writers.
            StreamWriter writerZ = null, writerV = null, writerC = null;

            if (writeZ) writerZ = new StreamWriter(path + "\\" + fileBase + "_Z.GRD");
            if (writeV) writerV = new StreamWriter(path + "\\" + fileBase + "_V.GRD");
            if (writeC) writerC = new StreamWriter(path + "\\" + fileBase + "_C.GRD");

            // Get the bounding box and iterate through all grid points.
            int xMin = (int)Math.Floor(coordLimits.Left / GridX);
            int xMax = (int)Math.Ceiling(coordLimits.Right / GridX);
            int yMin = (int)Math.Floor(coordLimits.Bottom / GridY);
            int yMax = (int)Math.Ceiling(coordLimits.Top / GridY);

            WriteSurferData dataZ = new WriteSurferData() { FormatCode = "F", NFI = nfi };
            WriteSurferData dataV = new WriteSurferData() { FormatCode = "F", NFI = nfi };
            WriteSurferData dataC = new WriteSurferData() { FormatCode = "F", NFI = nfi };

            for (int y = 0; y < surfaceSize.Height; y++)
            {
                for (int x = 0; x < surfaceSize.Width; x++)
                {
                    Sonar3DCell cell = surface[x, y];

                    if (cell != null)
                    {
                        // Grid entry exists. Get Z information.
                        LineData data = cell.Line.GetData(type);

                        dataZ.AddPoint(data.Depth);
                        dataC.AddPoint(data.topColor + 1);
                        dataV.AddPoint(data.Volume);
                    }
                    else
                    {
                        // Grid entry empty.
                        dataZ.AddPoint(double.NaN);
                        dataC.AddPoint(0);
                        dataV.AddPoint(0);
                    }
                }

                dataZ.NewLine();
                dataV.NewLine();
                dataC.NewLine();
            }

            int numX = Math.Abs(xMax - xMin);
            int numY = Math.Abs(yMax - yMin);

            var bl = new Coordinate(coordLimits.Left, coordLimits.Bottom, 0.0, CoordinateType.TransverseMercator);
            var tr = new Coordinate(coordLimits.Right, coordLimits.Top, 0.0, CoordinateType.TransverseMercator);

            var blp = bl.Point;
            var trp = tr.Point;

            if (elliptic)
            {
                blp = GSC.Settings.ForwardTransform.Run(bl, CoordinateType.Elliptic).Point;
                var temp = blp.X * 180 / System.Math.PI;
                blp.X = blp.Y * 180 / System.Math.PI;
                blp.Y = temp;

                trp = GSC.Settings.ForwardTransform.Run(tr, CoordinateType.Elliptic).Point;
                temp = trp.X * 180 / System.Math.PI;
                trp.X = trp.Y * 180 / System.Math.PI;
                trp.Y = temp;
            }


            dataZ.Write(writerZ, numX, numY, blp, trp);
            dataC.Write(writerC, numX, numY, blp, trp);
            dataV.Write(writerV, numX, numY, blp, trp);

            // Finish writers.
            if (writeZ) writerZ.Close();
            if (writeV) writerV.Close();
            if (writeC) writerC.Close();
        }
        #endregion

        #region ToString()
        public override string ToString()
        {
            return desc;
        }
        #endregion

        #region Background Workers
        #region Arch extended
        public override void ApplyArchAndVolume()
        {
            if (!this.IsInterpolating)
                ApplyArchAndVolume(false);
        }

        protected override void OnBwArchCompleted(object sender, RunWorkerCompletedEventArgs<bool> e)
        {
            if (!bwArchRestart & !e.Cancelled)
                Build3DMesh(false);

            base.OnBwArchCompleted(sender, e);
        }

        protected override ArchBounds ApplyArchAndVolumeToDevice(SonarDevice dev)
        {
            return dev.ApplyArchAndVolume(null, (float)depth3DTop);
        }

        public void ApplySpecialArchSync(SonarPanelType type, float archDepth, float archTopColorDepth)
        {
            int max = sonarLines.Count;

            for (int i = 0; i < max; i++)
                sonarLines[i].ApplyArchAndVolume(type, true, true, archDepth, archTopColorDepth, (float)depth3DTop);
        }
        #endregion

        #region Interpolation
        public void Interpolate()
        {
            Interpolate(false);
        }

        public void Interpolate(bool tryKeepAppend)
        {
            if (!EnableInterpolation)
                return;

            if (bwInterpolate == null)
            {
                bwInterpolate = new BackgroundWorker<bool, bool>();
                bwInterpolate.RunWorkerCompleted += new RunWorkerCompletedEventHandler<RunWorkerCompletedEventArgs<bool>>(OnBwInterpolateCompleted);
                bwInterpolate.DoWork += new DoWorkEventHandler<DoWorkEventArgs<bool, bool>>(OnBwInterpolateDoWork);
            }
            if (!bwInterpolate.IsBusy)
            {
                CancelEventArgs e = new CancelEventArgs(false);
                if (StartingInterpolation != null)
                    StartingInterpolation(this, e);
                if (!e.Cancel)
                    bwInterpolate.RunWorkerAsync(tryKeepAppend);
            }
            else
            {
                bwInterpolateRestart = true;
            }
        }

        void OnBwInterpolateCompleted(object sender, RunWorkerCompletedEventArgs<bool> e)
        {
            try
            {
                if (SurfaceInterpolated != null)
                    SurfaceInterpolated(this, EventArgs.Empty);

                if (bwInterpolateRestart)
                {
                    bwInterpolateRestart = false;
                    Interpolate(e.Result);
                }

                ApplyArchAndVolume(false);
            }
            catch (Exception ex) { DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message); }
        }

        void AddToNeighbours(List<SonarLine> neighbours, List<double> neighbourDistances, SonarLine line, SonarLine lineData, int intTh)
        {
            PointD ptD;
            double d, d2;

            ptD = line.CoordRvHv.Point;
            d = ptD.Distance(lineData.CoordRvHv);

            // No neighbours? Add directly and continue.
            int maxNeighbours = neighbours.Count;

            if (maxNeighbours == 0)
            {
                neighbours.Add(lineData);
                neighbourDistances.Add(d);
                return;
            }

            // Go through all neighbours.
            for (int i = 0; i < maxNeighbours; i++)
            {
                d2 = neighbourDistances[i];

                // Is current distance smaller than that of the saved neighbour? Insert this data line as neighbour.
                if (d < d2)
                {
                    neighbours.Insert(i, lineData);
                    neighbourDistances.Insert(i, d);

                    // If the nearest neighbours constraint is selected, the last neighbour drops out.
                    if ((intMethod == Sonar3DIntMethod.NearestNeighbours) && (neighbours.Count > intTh))
                    {
                        neighbours.RemoveAt(neighbours.Count - 1);
                        neighbourDistances.RemoveAt(neighbourDistances.Count - 1);
                    }
                    break;
                }
            }
        }

        void OnBwInterpolateDoWork(object sender, DoWorkEventArgs<bool, bool> e)
        {
            DateTime time = DateTime.Now;

            while (isDrawing)
            {
                if ((DateTime.Now - time).TotalSeconds >= 1)
                {
                    e.Cancel = true;
                    e.Result = true;
                    return;
                }

                Thread.Sleep(5);
            }

            isInterpolating = true;

            try
            {
                if (project == null)
                    return;

                if (blankLine == null)
                    return;

                double gridX = GridX;
                double gridY = GridY;
                double depthRes = DepthRes;
                int intTh = intThreshold;
                int intIt = intIterations;

                if ((gridX <= 0.0) || (gridY <= 0.0) || (depthRes <= 0.0))
                    return;

                #region Get the bounding rect and let it snap to the defined grid.
                PolygonD poly = blankLine.Poly;

                if (poly.BoundingBox == null)
                    return;

                coordLimits = poly.BoundingBox;

                int xMin = (int)Math.Floor(coordLimits.Left / gridX);
                int xMax = (int)Math.Ceiling(coordLimits.Right / gridX);
                int yMin = (int)Math.Floor(coordLimits.Bottom / gridY);
                int yMax = (int)Math.Ceiling(coordLimits.Top / gridY);

                coordLimits.Left = gridX * xMin;
                coordLimits.Right = gridX * xMax;
                coordLimits.Bottom = gridY * yMin;
                coordLimits.Top = gridY * yMax;

                if ((xMax - xMin != surfaceSize.Width) | (yMax - yMin != surfaceSize.Height))
                {
                    surfaceSize = new Size(xMax - xMin, yMax - yMin);
                    surface = new Sonar3DCell[surfaceSize.Width, surfaceSize.Height];
                }
                #endregion

                #region Variables
                object[,] rasterizedSurface = new object[xMax - xMin, yMax - yMin];
                List<Sonar3DLineAndPoint> interpolatedLines = new List<Sonar3DLineAndPoint>();
                List<Sonar3DLineAndPoint> emptyLines = new List<Sonar3DLineAndPoint>();
                List<Sonar3DLineAndPoint> emptyLinesRemaining = new List<Sonar3DLineAndPoint>();
                List<SonarLine> neighbours = new List<SonarLine>();
                List<double> neighbourDistances = new List<double>();
                List<SonarLine> directNeighbours = new List<SonarLine>();
                Sonar3DLineAndPoint emptyLineAndPoint;
                SonarLine emptyLine;
                SonarLine newLine;
                SonarLine line = null;
                DateTime timeStart = DateTime.MinValue;
                DateTime timeEnd = DateTime.MinValue;
                Point pt;
                PointD ptEmpty;
                int i, j, xRef, yRef;
                int maxEmpty;
                #endregion

                #region Prepare the record object.
                this.Devices.Clear();
                this.sonarLines.Clear();
                this.Devices.Add(new SonarDevice(0, "Profile", 0, 0, 0, 0, 0, true, "", "", true, true, 0, 0, ""));
                #endregion

                #region Rasterize surface.
                foreach (SonarRecord rec in project.Records)
                    if (rec.ShowInTrace)
                        rec.RasterizeSurface(rasterizedSurface, xMin, xMax, yMin, yMax, gridX, gridY);
                #endregion

                #region Iterate through each cell and do the PolygonD.Contains(..) test.
                for (xRef = xMin; xRef < xMax; xRef++)
                {
                    for (yRef = yMin; yRef < yMax; yRef++)
                    {
                        if (!poly.Contains(new PointD(((double)xRef + 0.5) * gridX, ((double)yRef + 0.5) * gridY)))
                        {
                            rasterizedSurface[xRef - xMin, yRef - yMin] = null;
                            continue;
                        }

                        pt = new Point(xRef, yRef);

                        List<SonarLine> listSL = rasterizedSurface[xRef - xMin, yRef - yMin] as List<SonarLine>;

                        line = InterpolateListToLine(listSL, new RectangleD(xRef * gridX, yRef * gridY, (xRef + 1) * gridX, (yRef + 1) * gridY), Mode, GSC.Settings.ColorUsedIn3D, depthRes, GSC.Settings.MergeWithAbsoluteDepths);

                        if (line != null)
                        {
                            string posString = "fixed;rv=" + yRef.ToString("0.000", GSC.Settings.NFI) + ";hv=" + xRef.ToString("0.000", GSC.Settings.NFI) + ";al=" + 0.ToString("0.000", GSC.Settings.NFI) + ";";

                            line.PosList.Add(new SonarPos(DateTime.Now, PosType.Fixed, false, posString));
                            line.IsProfile = true;

                            if (line.IsEmpty)
                            {
                                // If the sonar line is empty, then fill it the surface spot with null.
                                emptyLinesRemaining.Add(new Sonar3DLineAndPoint() { Line = line, X = xRef - xMin, Y = yRef - yMin });
                                rasterizedSurface[xRef - xMin, yRef - yMin] = null;
                            }
                            else
                            {
                                // Add sonar line.
                                AddSonarLine(new Point(xRef - xMin, yRef - yMin), line);
                                rasterizedSurface[xRef - xMin, yRef - yMin] = line;
                            }
                        }
                        else
                            rasterizedSurface[xRef - xMin, yRef - yMin] = null;
                    }
                }
                #endregion

                #region Complete the empty entries.
                for (i = 0; i < intIt; i++)
                {
                    interpolatedLines.Clear();
                    emptyLines = emptyLinesRemaining;
                    emptyLinesRemaining = new List<Sonar3DLineAndPoint>();

                    maxEmpty = emptyLines.Count;

                    for (j = 0; j < maxEmpty; j++)
                    {
                        emptyLineAndPoint = emptyLines[j];
                        emptyLine = emptyLineAndPoint.Line;
                        xRef = emptyLineAndPoint.X;
                        yRef = emptyLineAndPoint.Y;
                        neighbours.Clear();
                        neighbourDistances.Clear();
                        directNeighbours.Clear();

                        // Find neighbours, going from near to far circles around the line.
                        int n, x, y, xSize, ySize;
                        int nMax = intThreshold;

                        // Define the maximum circle distance. Higher iterations only test the direct neighbours.
                        if (i > 0)
                            nMax = 1;

                        // Iterate through the circles.
                        for (n = 1; n <= nMax; n++)
                        {
                            // Find the Top and Right borders - Left and Bottom borders are 0.
                            xSize = Math.Min(xMax - xMin - 1, xRef + n);
                            ySize = Math.Min(yMax - yMin - 1, yRef + n);

                            // Go from left to right.
                            for (x = Math.Max(0, xRef - n); x <= xSize; x++)
                            {
                                // Check top line.
                                y = yRef - n;
                                if ((y >= 0) && (rasterizedSurface[x, y] != null))
                                {
                                    if (n == 1)
                                        directNeighbours.Add(rasterizedSurface[x, y] as SonarLine);
                                    if (i == 0)
                                        AddToNeighbours(neighbours, neighbourDistances, emptyLine, rasterizedSurface[x, y] as SonarLine, intTh);
                                }
                                // Check bottom line.
                                y = yRef + n;
                                if ((y <= ySize) && (rasterizedSurface[x, y] != null))
                                {
                                    if (n == 1)
                                        directNeighbours.Add(rasterizedSurface[x, y] as SonarLine);
                                    if (i == 0)
                                        AddToNeighbours(neighbours, neighbourDistances, emptyLine, rasterizedSurface[x, y] as SonarLine, intTh);
                                }
                            }

                            // Update ySize to top-1
                            ySize = Math.Min(yMax - yMin - 1, yRef + n - 1);

                            // Go from bottom+1 to top-1
                            for (y = Math.Max(0, yRef - n + 1); y <= ySize; y++)
                            {
                                // Check left line.
                                x = xRef - n;
                                if ((x >= 0) && (rasterizedSurface[x, y] != null))
                                {
                                    if (n == 1)
                                        directNeighbours.Add(rasterizedSurface[x, y] as SonarLine);
                                    if (i == 0)
                                        AddToNeighbours(neighbours, neighbourDistances, emptyLine, rasterizedSurface[x, y] as SonarLine, intTh);
                                }
                                // Check right line.
                                x = xRef + n;
                                if ((x <= xSize) && (rasterizedSurface[x, y] != null))
                                {
                                    if (n == 1)
                                        directNeighbours.Add(rasterizedSurface[x, y] as SonarLine);
                                    if (i == 0)
                                        AddToNeighbours(neighbours, neighbourDistances, emptyLine, rasterizedSurface[x, y] as SonarLine, intTh);
                                }
                            }
                        }

                        if (neighbours.Count > 0)
                        {
                            // Now, interpolate the height of the line using these neighbours.
                            PointD ptD = emptyLine.CoordRvHv.Point;
                            pt = ptD.Scale(1.0 / GridX, 1.0 / GridY).Offset(-0.5, -0.5).Point;

                            emptyLineAndPoint = emptyLines[j];
                            emptyLine = emptyLineAndPoint.Line = InterpolateListToLine(neighbours, ptD, neighbourDistances[neighbourDistances.Count - 1], Mode, GSC.Settings.ColorUsedIn3D, DepthRes, false, intWeighting, null, true);
                            emptyLines[j] = emptyLineAndPoint;
                        }

                        if (directNeighbours.Count > 0)
                        {
                            ptEmpty = emptyLine.CoordRvHv.Point;
                            newLine = InterpolateListToLine(directNeighbours, ptEmpty, double.NaN, Mode, GSC.Settings.ColorUsedIn3D, depthRes, false, intWeighting, emptyLine);
                            interpolatedLines.Add(new Sonar3DLineAndPoint() { Line = newLine, X = xRef, Y = yRef });
                        }
                        else
                            emptyLinesRemaining.Add(emptyLineAndPoint);
                    }

                    maxEmpty = interpolatedLines.Count;

                    for (j = 0; j < maxEmpty; j++)
                    {
                        Sonar3DLineAndPoint interpolatedLineAndPoint = interpolatedLines[j];
                        SonarLine interpolatedLine = interpolatedLineAndPoint.Line;
                        xRef = interpolatedLineAndPoint.X;
                        yRef = interpolatedLineAndPoint.Y;

                        AddSonarLine(new Point(xRef, yRef), interpolatedLine);

                        rasterizedSurface[xRef, yRef] = interpolatedLine;
                    }
                }
                #endregion

                #region Add remaining empty sonar lines.
                emptyLines = emptyLinesRemaining;
                maxEmpty = emptyLines.Count;

                for (j = 0; j < maxEmpty; j++)
                {
                    emptyLineAndPoint = emptyLines[j];
                    emptyLine = emptyLineAndPoint.Line;
                    AddSonarLine(new Point(emptyLineAndPoint.X, emptyLineAndPoint.Y), emptyLine);
                }
                #endregion

                #region Finish the record object.
                RefreshDevices();
                ShowManualPoints = false;

                Build3DMesh();
                #endregion

                e.Result = e.Argument;
            }
            catch (Exception ex) { DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message); }
        }
        #endregion

        #region Meshing
        public void Build3DMesh(SonarPanelType type, TopColorMode mode)
        {
            lastType = type;
            lastMode = mode;
            Build3DMesh();
        }

        public void Build3DMesh()
        {
            if (!this.ApplyingArchAndVolume & !this.IsInterpolating)
                Build3DMesh(false);
        }

        public void Build3DMesh(bool tryKeepAppend)
        {
            if (bwBuildMesh == null)
            {
                bwBuildMesh = new BackgroundWorker<bool, BWResult>();
                bwBuildMesh.WorkerSupportsCancellation = true;
                bwBuildMesh.RunWorkerCompleted += new RunWorkerCompletedEventHandler<RunWorkerCompletedEventArgs<BWResult>>(OnBwBuildMeshCompleted);
                bwBuildMesh.DoWork += new DoWorkEventHandler<DoWorkEventArgs<bool, BWResult>>(OnBwBuildMeshDoWork);
            }
            if (!bwBuildMesh.IsBusy)
            {
                bwBuildMesh.RunWorkerAsync(tryKeepAppend);
            }
            else
            {
                bwBuildMeshRestart = true;
                bwBuildMesh.CancelAsync();
            }
        }

        void OnBwBuildMeshCompleted(object sender, RunWorkerCompletedEventArgs<BWResult> e)
        {
            try
            {
                isMeshing = false;
                isInterpolating = false;

                if (!e.Cancelled && (e.Result != null))
                {
                    if (MeshBuilt != null)
                        MeshBuilt(this, EventArgs.Empty);
                }

                if (bwBuildMeshRestart)
                {
                    bwBuildMeshRestart = false;
                    if (e.Result != null)
                        Build3DMesh(e.Result.tryKeepAppend);
                    else
                        Build3DMesh(false);
                }
            }
            catch (Exception ex) { DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message); }
        }

        void OnBwBuildMeshDoWork(object sender, DoWorkEventArgs<bool, BWResult> e)
        {
            DateTime timeStart = DateTime.Now;

            while (isDrawing)
            {
                if ((DateTime.Now - timeStart).TotalSeconds >= 1)
                {
                    e.Cancel = true;
                    return;
                }

                Thread.Sleep(5);
            }
            
            isMeshing = true;

            Box3D newBox = new Box3D();

            try
            {
                for (int x = 0; x < surfaceSize.Width; x++)
                    for (int y = 0; y < surfaceSize.Height; y++)
                        if (surface[x, y] != null)
                            surface[x, y].BuildVertices(this, Get3DNeighbours(new Point(x, y)), lastType, lastMode, ptOrigin, GridX, GridY);
            
                RectangleD rc = new RectangleD(coordLimits);
                rc.Offset(-ptOrigin.X, -ptOrigin.Y);

                box.BuildVertices(new BoxF(rc, (float)this.Depth3DBottom, (float)this.Depth3DTop));
            }
            catch (Exception ex) { DebugClass.SendDebugLine(this, DebugLevel.Red, ex.Message); }
            finally
            {
                // Create result.
                e.Result = new BWResult() { tryKeepAppend = (bool)e.Argument };
            }
        }
        #endregion
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler SurfaceInterpolated;
        public event EventHandler MeshBuilt;
        public event CancelEventHandler StartingInterpolation;
        #endregion
    }
}
