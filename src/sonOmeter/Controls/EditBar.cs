using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using sonOmeter.Classes;

namespace sonOmeter
{
	public enum EditModes
	{
		Nothing,
		Displace,
		PitchH,
		PitchV,
		Grid,
		TraceGrid,
		SetGlobal,
		Arch,
		ClearCutLine
	}

	/// <summary>
	/// Summary description for EditBar.
	/// </summary>
	public class EditBar : System.Windows.Forms.UserControl
	{
		#region Create, Dispose usw.
		private System.Windows.Forms.Label label;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EditBar()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

            this.ForeColor = GSC.Settings.CS.ForeColor;
			this.BackColor = GSC.Settings.CS.BackColor;
			label.ForeColor = GSC.Settings.CS.ForeColor;
			label.BackColor = GSC.Settings.CS.BackColor;

			Invalidate();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		} 
		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label.Location = new System.Drawing.Point(0, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(176, 24);
			this.label.TabIndex = 0;
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.label.Paint += new System.Windows.Forms.PaintEventHandler(this.label_Paint);
			this.label.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label_MouseMove);
			// 
			// EditBar
			// 
			this.BackColor = System.Drawing.Color.Blue;
			this.Controls.Add(this.label);
			this.ForeColor = System.Drawing.Color.White;
			this.Name = "EditBar";
			this.Size = new System.Drawing.Size(176, 24);
			this.FontChanged += new System.EventHandler(this.EditBar_FontChanged);
			this.ResumeLayout(false);

		}
		#endregion

		#region Variables
		string editString = "";
		string labelString = "";

		SonarPanelType srcPanelType = SonarPanelType.HF;
		SonarPanelType dstPanelType = SonarPanelType.HF;
		
		EditModes editMode = EditModes.Nothing;
		CutMode cutMode = CutMode.Nothing;
		CutMode invCutMode = CutMode.Nothing;

        bool displaceMove = false;
		#endregion

		#region Properties
		public new BorderStyle BorderStyle
		{
			get { return label.BorderStyle; }
			set { label.BorderStyle = value; Invalidate(); }
		}

		public EditModes EditMode
		{
			get { return editMode; }
			set
			{
				editMode = value;
				EditString = "0";
			}
		}

		public CutMode CutMode
		{
			get { return cutMode; }
			set
			{
				cutMode = value;
				UpdateEditString();
			}
		}

		public CutMode InvCutMode
		{
			get { return invCutMode; }
			set
			{
				invCutMode = value;
				UpdateEditString();
			}
		}

        public bool DisplaceMove
        {
            get { return displaceMove; }
            set
            {
                displaceMove = value;
                UpdateEditString();
            }
        }

		public Label Label
		{
			get { return label; }
		}

		public string EditString
		{
			set
			{
				editString = value;
				UpdateEditString();
			}
		}

		public string LabelString
		{
			set
			{
				labelString = value;
				UpdateEditString();
			}
		}

		public SonarPanelType SrcPanelType
		{
			set
			{
				if (srcPanelType != dstPanelType)
				{
					if (value == SonarPanelType.HF)
						dstPanelType = SonarPanelType.NF;
					else
						dstPanelType = SonarPanelType.HF;
				}
				else
					dstPanelType = value;

				srcPanelType = value;

				UpdateEditString();
			}
		}

		public SonarPanelType DstPanelType
		{
			set
			{
				dstPanelType = value;
				UpdateEditString();
			}
		}
		#endregion

		#region Events
		public event EditEventHandler EditReady;
		#endregion

		#region General
		private void EditBar_FontChanged(object sender, System.EventArgs e)
		{
			label.Font = Font;
		}
		#endregion

		#region Editing
		void UpdateEditString()
		{
			switch (editMode)
			{
				case EditModes.Displace:
					if ((invCutMode != CutMode.Surface) && (invCutMode != CutMode.CDepth))
						invCutMode = (cutMode == CutMode.Top) ? CutMode.Bottom : CutMode.Top;

					string s = "twice the depth";

					if (editString != "d")
						s = editString + "m\t\t";

                    if (displaceMove)
                        label.Text = "Displace " + srcPanelType + " " + cutMode + " line: " + s;
                    else if (dstPanelType == srcPanelType)
                        label.Text = "Displace " + srcPanelType + " line: " + s + " from " + invCutMode + " to " + cutMode;
                    else
                        label.Text = "Displace " + cutMode + " line: " + s + " from " + srcPanelType + " to " + dstPanelType;
					break;

				case EditModes.PitchH:
				case EditModes.PitchV:
					label.Text = "Distance between marked lines: " + editString + "m";
					break;

				case EditModes.Grid:
					label.Text = "Global grid spacing: " + editString + "m";
					break;

				case EditModes.TraceGrid:
					label.Text = "Trace grid spacing: " + editString + "m";
					break;

				case EditModes.SetGlobal:
					label.Text = "Set " + srcPanelType + " global " + cutMode + " cut value: " + editString + "m";
					break;

				case EditModes.Nothing:
					label.Text = labelString;
					break;

				case EditModes.Arch:
					label.Text = "Press ENTER to apply arch depth to " + cutMode + " in " + srcPanelType + ". Press ESC to abort.";
					break;

				case EditModes.ClearCutLine:
					label.Text = "Clear cut line: " + cutMode + " in " + srcPanelType;
					break;
			}
		}

		void CommitEdit()
		{
			CutMode inv = (srcPanelType == dstPanelType) ? invCutMode : cutMode;

            if (EditReady != null)
            {
                if (editMode == EditModes.Displace && displaceMove)
                    EditReady(this, new EditEventArgs(editString, editMode, cutMode, cutMode, srcPanelType, srcPanelType));
                else
                    EditReady(this, new EditEventArgs(editString, editMode, cutMode, inv, srcPanelType, dstPanelType));
            }

			// Reset state.
			editString = "";
            displaceMove = false;
			EditMode = EditModes.Nothing;
		}

		void CancelEdit()
		{
			// Reset state.
			editString = "";
			EditMode = EditModes.Nothing;
		}

		public void Edit(System.Windows.Forms.KeyEventArgs e)
		{
            switch (e.KeyData)
            {
                case Keys.Enter:
                    CommitEdit();
                    break;

                case Keys.Escape:
                    CancelEdit();
                    break;

                case Keys.Space:
                    if (cutMode == CutMode.Top)
                        cutMode = CutMode.Bottom;
                    else if (cutMode == CutMode.Bottom)
                        cutMode = CutMode.Top;
                    UpdateEditString();
                    break;

                case Keys.F5:
                    SrcPanelType = SonarPanelType.HF;
                    break;

                case Keys.F6:
                    SrcPanelType = SonarPanelType.NF;
                    break;

                case Keys.C:
                    if (invCutMode != CutMode.CDepth)
                        InvCutMode = CutMode.CDepth;
                    else
                        InvCutMode = (cutMode == CutMode.Top) ? CutMode.Bottom : CutMode.Top;
                    break;

                case Keys.S:
                    if (invCutMode != CutMode.Surface)
                        InvCutMode = CutMode.Surface;
                    else
                        InvCutMode = (cutMode == CutMode.Top) ? CutMode.Bottom : CutMode.Top;
                    break;

                case Keys.D:
                    if (editMode == EditModes.Displace)
                    {
                        if (editString == "d")
                        {
                            EditString = "0";
                            if (displaceMove)
                                EditBiPolarValue(e);
                            else
                                EditUniPolarValue(e);
                        }
                        else
                            EditString = "d";
                    }
                    break;

                case Keys.M:
                    if (editMode == EditModes.Displace)
                    {
                        displaceMove = !displaceMove;
                        EditString = "0";
                        if (displaceMove)
                            EditBiPolarValue(e);
                        else
                            EditUniPolarValue(e);
                    }
                    break;

                default:
                    if (editMode == EditModes.Displace)
                    {
                        if (displaceMove)
                            EditBiPolarValue(e);
                        else
                            EditUniPolarValue(e);
                    }
                    else
                    {
                        switch (editMode)
                        {
                            case EditModes.PitchH:
                            case EditModes.PitchV:
                            case EditModes.Grid:
                            case EditModes.TraceGrid:
                                EditUniPolarValue(e);
                                break;

                            case EditModes.SetGlobal:
                                EditBiPolarValue(e);
                                break;
                        }
                    }
                    break;
            }
		}

		void EditUniPolarValue(System.Windows.Forms.KeyEventArgs e)
		{
			int index;

            switch (e.KeyData)
            {
                case Keys.OemPeriod:
                    // Insert decimal point.
                    index = editString.IndexOf('.');
                    if (index == -1)
                        EditString = editString.Insert(editString.Length, ".");
                    break;

                case Keys.Back:
                    if ((editString != "0") && (editString != "-0"))
                    {
                        if (editString.Length > 1)
                            EditString = editString.Remove(editString.Length - 1, 1);
                        if (editString.Length <= 1)
                            EditString = "0";
                    }
                    break;

                case Keys.F7:
                    if (srcPanelType == dstPanelType)
                    {
                        if (srcPanelType == SonarPanelType.HF)
                            DstPanelType = SonarPanelType.NF;
                        else
                            DstPanelType = SonarPanelType.HF;
                    }
                    else
                        DstPanelType = srcPanelType;
                    break;

                default:
                    index = e.KeyValue - ((e.KeyValue >= (int)Keys.NumPad0) ? (int)Keys.NumPad0 : (int)Keys.D0);

                    if ((index >= 0) && (index <= 9))
                    {
                        if ((editString == "0") || (editString == "-0"))
                            editString = editString.Remove(editString.Length - 1, 1);

                        EditString = editString.Insert(editString.Length, index.ToString());
                    }
                    break;
            }
		}

        void EditBiPolarValue(System.Windows.Forms.KeyEventArgs e)
        {
            int index;

            switch (e.KeyData)
            {
                case Keys.OemMinus:
                    // Change sign.
                    index = editString.IndexOf('-');
                    if (index > -1)
                        EditString = editString.Remove(index, 1);
                    else
                        EditString = editString.Insert(0, "-");
                    break;

                case Keys.OemPeriod:
                    // Insert decimal point.
                    index = editString.IndexOf('.');
                    if (index == -1)
                        EditString = editString.Insert(editString.Length, ".");
                    break;

                case Keys.Back:
                    if ((editString != "0") && (editString != "-0"))
                    {
                        if (editString.Length > 1)
                            EditString = editString.Remove(editString.Length - 1, 1);
                        if (editString.Length <= 1)
                            EditString = "0";
                    }
                    break;

                case Keys.F7:
                    if (srcPanelType == dstPanelType)
                    {
                        if (srcPanelType == SonarPanelType.HF)
                            DstPanelType = SonarPanelType.NF;
                        else
                            DstPanelType = SonarPanelType.HF;
                    }
                    else
                        DstPanelType = srcPanelType;
                    break;

                default:
                    index = e.KeyValue - ((e.KeyValue >= (int)Keys.NumPad0) ? (int)Keys.NumPad0 : (int)Keys.D0);

                    if ((index >= 0) && (index <= 9))
                    {
                        if ((editString == "0") || (editString == "-0"))
                            editString = editString.Remove(editString.Length - 1, 1);

                        EditString = editString.Insert(editString.Length, index.ToString());
                    }
                    break;
            }
        }
		#endregion

		#region Label and Settings
        public void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
		{
            if (e.PropertyName.StartsWith("CS.") || (e.PropertyName == "All"))
            {
                this.ForeColor = GSC.Settings.CS.ForeColor;
                this.BackColor = GSC.Settings.CS.BackColor;
                label.ForeColor = GSC.Settings.CS.ForeColor;
                label.BackColor = GSC.Settings.CS.BackColor;

                Invalidate();
            }
		}

		private void label_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			StringFormat sf = StringFormat.GenericDefault;
			sf.Trimming = StringTrimming.EllipsisCharacter;

			Graphics g = e.Graphics;
			SizeF strSize = g.MeasureString(label.Text, this.Font);
			RectangleF rc = new RectangleF(0, (label.Height - strSize.Height) / 2 - 1, label.Width, strSize.Height);

			g.Clear(this.BackColor);
			g.DrawString(label.Text, this.Font, new SolidBrush(this.ForeColor), rc, sf);
		}

		private void label_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			OnMouseMove(e);
		} 
		#endregion
	}
}
