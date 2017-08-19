using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UKLib.DXF
{
    public partial class DXFLayerForm : Form
    {
        private DXFContainer dxfCont;

        public DXFLayerForm(DXFContainer cont)
        {
            InitializeComponent();

            dxfCont = cont;

            foreach (DXFLayer layer in dxfCont.LayerList)
                clbLayers.Items.Add(layer, layer.Visible);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int max = clbLayers.Items.Count;

            for (int i = 0; i < max; i++)
                (clbLayers.Items[i] as DXFLayer).Visible = clbLayers.GetItemChecked(i);
        }

        private void clbLayers_SelectedIndexChanged(object sender, EventArgs e)
        {
            pgLayer.SelectedObject = clbLayers.SelectedItem;
        }
    }
}
