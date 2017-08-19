using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Globalization;
using UKLib.Xml;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;

namespace sonOmeter.Classes
{
    /// <summary>
    /// Summary description for Arch.
    /// </summary>
    public class Arch
    {
        public struct ArchBounds
        {
            public float MaxHF;
            public float MinHF;
            public float MaxNF;
            public float MinNF;
        }

        #region Apply
        public void ApplyValues(SonarProject prj)
        {
            try
            {
                for (int i = 0; i < prj.RecordCount; i++)
                    prj.Record(i).DepthFieldBounds = ApplyValues(prj.Record(i).SonarLines(), null);

                for (int j = 0; j < prj.Record3DCount; j++)
                    prj.Record3D(j).DepthFieldBounds = ApplyValues(prj.Record3D(j).SonarLines(), null);

                prj.Interpolate3DCells();
            }
            catch
            {
                UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, "Arch::ApplyValues failed on parsing prj.");
            }
        }

        public ArchBounds ApplyValues(List<SonarLine> lines, bool? isCut)
        {
            ArchBounds bounds = new ArchBounds() { MaxHF = (float)GSC.Settings.DepthBottom, MinHF = (float)GSC.Settings.DepthTop, MaxNF = (float)GSC.Settings.DepthBottom, MinNF = (float)GSC.Settings.DepthTop };

            if ((lines == null) || (lines.Count == 0))
                return bounds;

            int max = lines.Count;
            SonarLine line;
            float sDepth;

            for (int i = 0; i < max; i++)
            {
                line = lines[i];
                if (isCut.HasValue)
                    line.IsCut = isCut.Value;
                else
                    isCut = line.IsCut;
                sDepth = line.SubDepth;

                ApplyValues(line.HF, isCut.Value, sDepth);
                ApplyValues(line.NF, isCut.Value, sDepth);

                if (bounds.MaxHF < line.HF.Depth)
                    bounds.MaxHF = line.HF.Depth;
                if (bounds.MinHF > line.HF.Depth)
                    bounds.MinHF = line.HF.Depth;

                if (bounds.MaxNF < line.NF.Depth)
                    bounds.MaxNF = line.NF.Depth;
                if (bounds.MinNF > line.NF.Depth)
                    bounds.MinNF = line.NF.Depth;
            }

            return bounds;
        }

        /// <summary>
        /// Applies the virtual archeology settings to the SonarLine data.
        /// </summary>
        /// <param name="data">The data element (usually HF or NF).</param>
        /// <param name="isCut">The cut flag.</param>
        /// <param name="subDepth">The submarine depth.</param>
        public void ApplyValues(LineData data, bool isCut, float subDepth)
        {
            if (data == null)
                return;

            bool start = false;
            float dStart = 0;
            int lastCol = -1;
            int i = 0, j = 0;
            int max = 0;
            int colCount = GSC.Settings.SECL.Count;

            DataEntry entry;
            DataEntry nextEntry = new DataEntry();

            // Speed up things by prefetching arch settings.
            float archDepth = GSC.Settings.ArchDepth;
            float archTopColorDepth = GSC.Settings.ArchDepthsIndependent ? GSC.Settings.ArchTopColorDepth : archDepth;
            bool archActive = GSC.Settings.ArchActive;
            bool archStopAtStrongLayer = GSC.Settings.ArchStopAtStrongLayer;
            BindingList<SonarEntryConfig> SECL = GSC.Settings.SECL;
            float TCut = data.TCut;
            float BCut = data.BCut;
            
            int dLastCol = -1;
            int dTopColor = -1;
            float dDepth = dStart - archDepth;

            double[] volCol = new double[colCount];

            bool breakDepth = false;
            bool breakColor = false;

            if (data.Entries != null)
            {
                try
                {
                    data.TopColor = -1;
                    max = data.Entries.Length;

                    if (max > 0)
                        nextEntry = data.Entries[0];

                    // UBOOT
                    nextEntry.high -= subDepth;
                    nextEntry.low -= subDepth;
                    nextEntry.uncutHigh -= subDepth;
                    // UBOOT
                        
                    for (i = 1; i <= max; i++)
                    {
                        entry = nextEntry;
                        if (i != max)
                            nextEntry = data.Entries[i];

                        // UBOOT
                        nextEntry.high -= subDepth;
                        nextEntry.low -= subDepth;
                        nextEntry.uncutHigh -= subDepth;
                        // UBOOT

                        if ((entry.low < BCut) & isCut)
                            entry.low = BCut;

                        if (!start)
                        {
                            if ((entry.low > TCut) & isCut)
                                continue;

                            start = true;

                            if ((entry.high > TCut) & isCut)
                                dStart = TCut;          // Use cut line border as start.
                            else
                                dStart = entry.high;    // Use top of the element.
                            
                            if (!archActive)
                            {
                                // Arch is not active - take the first element.
                                dLastCol = entry.colorID;
                                dTopColor = entry.colorID;
                                dDepth = dStart;
                                break;
                            }
                        }
                        else if ((entry.high < BCut) & isCut)
                        {
                            // No more valid data available - take -1 color and standard depth.
                            dLastCol = -1;
                            dTopColor = -1;
                            dDepth = dStart - archDepth;
                            break;
                        }
                        else
                        {
                            if ((entry.high < dStart - archTopColorDepth) && !breakColor)
                            {
                                // Element is out of range - take previous color.
                                dLastCol = lastCol;
                                dTopColor = lastCol;
                                breakColor = true;
                            }

                            if ((entry.high < dStart - archDepth) && !breakDepth)
                            {
                                // Element is out of range - take standard depth.
                                // removed by archTopColorDepth: dLastCol = lastCol;
                                // removed by archTopColorDepth: dTopColor = lastCol;
                                dDepth = dStart - archDepth;
                                breakDepth = true;
                            }

                            if (breakColor & breakDepth)
                                break;
                        }

                        if ((archStopAtStrongLayer && (i < max - 1) && (nextEntry.colorID < entry.colorID)) || SECL[entry.colorID].ArchStop)
                        {
                            // Element stops at a layer change - save special depth.
                            if (!breakColor)
                            {
                                dLastCol = -2;
                                dTopColor = entry.colorID;
                            }
                            if (!breakDepth)
                                dDepth = entry.high;
                            break;
                        }
                        else if (i == max)
                        {
                            // No more valid data available - take -1 color and standard depth.
                            if (!breakColor)
                            {
                                dLastCol = -1;
                                dTopColor = -1;
                            }
                            if (!breakDepth)
                                dDepth = dStart - archDepth;
                            break;
                        }
                        else
                        {
                            if ((entry.low < dStart - archTopColorDepth) && !breakColor)
                            {
                                // Element is splitted by range.
                                dLastCol = -2;
                                dTopColor = entry.colorID;
                                breakColor = true;
                            }

                            if ((entry.low < dStart - archDepth) && !breakDepth)
                            {
                                // Element is splitted by range - save standard depth.
                                // removed by archTopColorDepth: dLastCol = -2;
                                // removed by archTopColorDepth: dTopColor = entry.colorID;
                                dDepth = dStart - archDepth;
                                breakDepth = true;
                            }

                            if (breakColor & breakDepth)
                                break;
                        }

                        lastCol = entry.colorID;
                        dDepth = entry.high;
                    }
                }
                catch
                {
                }
            }

            data.LastCol = dLastCol;
            data.TopColor = dTopColor;
            data.Depth = dDepth;
            data.Volume = 0;

            for (j = i; j < max; j++)
            {
                entry = data.Entries[j];
                float low = entry.low;
                float high = entry.high;

                if ((low > TCut) | (high < BCut))
                    continue;

                if (low < BCut)
                    low = BCut;
                if (high > TCut)
                    high = TCut;

                if ((entry.colorID < 0) | (entry.colorID > colCount - 1))
                    continue;
                if (archActive & (low > dDepth))
                    continue;

                try
                {
                    if (archActive & (high > dDepth))
                        volCol[entry.colorID] += dDepth - low;
                    else
                        volCol[entry.colorID] += high - low;
                }
                catch
                {
                    // Filter out all unwanted events. :-) 
                }
            }

            for (j = 0; j < colCount; j++)
            {
                if (volCol[j] >= 0.1)
                    data.Volume += (float)System.Math.Log10(volCol[j] * 10) * GSC.Settings.SECL[j].VolumeWeight;
            }
        }
        #endregion
    }
}
