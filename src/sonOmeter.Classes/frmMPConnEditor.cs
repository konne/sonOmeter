using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using UKLib.Arrays;
using UKLib.Controls;

namespace sonOmeter.Classes
{
    public partial class frmMPConnEditor : Form
    {
        protected SerializeDictionary<int, MPConnStyle> styleList = new SerializeDictionary<int,MPConnStyle>();

        public SerializeDictionary<int, MPConnStyle> StyleList
        {
            get { return styleList; }
            set
            {
                if (value == null)
                    return;

                styleList = value;

                UpdateStyleList();
            }
        }

        private void UpdateStyleList()
        {
            lbTypes.Items.Clear();

            foreach (int key in styleList.Keys)
                lbTypes.Items.Add(key);
        }

        public frmMPConnEditor()
        {
            InitializeComponent();

            lbTypes.Items.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            InputBoxResult result = new InputBoxResult(System.Windows.Forms.DialogResult.None, "0");
            string append = "";

            while (result.DialogResult != System.Windows.Forms.DialogResult.Cancel)
            {
                result = InputBox.Show(result.InputText, "Manual point type" + append, "Add new connection style");

                if (result.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    int type = 0;

                    if (!int.TryParse(result.InputText, out type))
                    {
                        append = " - Only numbers allowed!";
                        continue;
                    }

                    if (!StyleList.ContainsKey(type))
                    {
                        StyleList.Add(type, new MPConnStyle() { Alpha = 255, Color = Color.FromKnownColor(KnownColor.Black), Width = 1.0F });
                        lbTypes.Items.Add(type);
                        lbTypes.SelectedIndex = lbTypes.Items.Count - 1;
                        break;
                    }
                    else
                        append = " - Type already defined!";
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lbTypes.SelectedIndex != -1)
            {
                StyleList.Remove((int)lbTypes.SelectedItem);
                lbTypes.Items.RemoveAt(lbTypes.SelectedIndex);
            }
        }

        private void lbTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbTypes.SelectedIndex != -1)
                pgMPConnStyle.SelectedObject = StyleList[(int)lbTypes.SelectedItem];
        }

        private void lbTypes_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lbTypes.SelectedIndex == -1)
                return;

            int oldType = (int)lbTypes.SelectedItem;
            InputBoxResult result = new InputBoxResult(System.Windows.Forms.DialogResult.None, oldType.ToString());
            string append = "";

            while (result.DialogResult != System.Windows.Forms.DialogResult.Cancel)
            {
                result = InputBox.Show(result.InputText, "Manual point type" + append, "Edit connection style");

                if (result.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    int type = 0;

                    if (!int.TryParse(result.InputText, out type))
                    {
                        append = " - Only numbers allowed!";
                        continue;
                    }

                    if ((type == oldType) || !StyleList.ContainsKey(type))
                    {
                        MPConnStyle style = StyleList[oldType];
                        int index = lbTypes.SelectedIndex;
                        StyleList.Remove(oldType);
                        StyleList.Add(type, style);
                        lbTypes.Items.Insert(lbTypes.SelectedIndex, type);
                        lbTypes.Items.RemoveAt(lbTypes.SelectedIndex);
                        lbTypes.SelectedIndex = index;
                        break;
                    }
                    else
                        append = " - Type already defined!";
                }
            }
        }

        private void frmMPConnEditor_Load(object sender, EventArgs e)
        {
            UpdateStyleList();

            if (lbTypes.Items.Count > 0)
                lbTypes.SelectedIndex = 0;
            else
                pgMPConnStyle.SelectedObject = null;
        }
    }
}
