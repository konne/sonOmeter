using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using UKLib.Survey.Math;

namespace UKLib.Survey.Editors
{
	/// <summary>
	/// Summary description for SurveyEditorForm.
	/// </summary>
	public class frmSurveyEditor : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.Button btnSelect;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.ListBox lbList;
		
		#region Construct and dispose
		public frmSurveyEditor(Type type)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			editingType = type;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lbList = new System.Windows.Forms.ListBox();
			this.label = new System.Windows.Forms.Label();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnSelect = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lbList
			// 
			this.lbList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left)));
			this.lbList.Location = new System.Drawing.Point(8, 34);
			this.lbList.Name = "lbList";
			this.lbList.Size = new System.Drawing.Size(152, 329);
			this.lbList.TabIndex = 0;
			this.lbList.SelectedIndexChanged += new System.EventHandler(this.lbList_SelectedIndexChanged);
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(8, 8);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(152, 23);
			this.label.TabIndex = 1;
			this.label.Text = "Known Ellipsoids:";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// propertyGrid
			// 
			this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGrid.CommandsVisibleIfAvailable = true;
			this.propertyGrid.LargeButtons = false;
			this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.propertyGrid.Location = new System.Drawing.Point(168, 8);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(230, 422);
			this.propertyGrid.TabIndex = 2;
			this.propertyGrid.Text = "propertyGrid1";
			this.propertyGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.propertyGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnAdd.Location = new System.Drawing.Point(8, 374);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(72, 23);
			this.btnAdd.TabIndex = 3;
			this.btnAdd.Text = "Add";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnRemove.Location = new System.Drawing.Point(88, 374);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(72, 23);
			this.btnRemove.TabIndex = 4;
			this.btnRemove.Text = "Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnSelect
			// 
			this.btnSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSelect.Location = new System.Drawing.Point(8, 406);
			this.btnSelect.Name = "btnSelect";
			this.btnSelect.Size = new System.Drawing.Size(72, 23);
			this.btnSelect.TabIndex = 3;
			this.btnSelect.Text = "Select";
			this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(88, 406);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(72, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// frmSurveyEditor
			// 
			this.AcceptButton = this.btnSelect;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(408, 437);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.propertyGrid);
			this.Controls.Add(this.label);
			this.Controls.Add(this.lbList);
			this.Controls.Add(this.btnSelect);
			this.Controls.Add(this.btnCancel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmSurveyEditor";
			this.Text = "Ellipsoid Editor";
			this.ResumeLayout(false);

		}
		#endregion

		#region Variables
		private object editingInstance = null;
		private Type editingType;
		#endregion

		#region Properties
		public object EditingInstance
		{
			get { return editingInstance; }
			set { editingInstance = value; }
		}

		public Type EditingType
		{
			get { return editingType; }
			set { editingType = value; }
		}
		#endregion

		private void BuildList()
		{
			lbList.BeginUpdate();
			lbList.Items.Clear();

			if (editingType == typeof(Ellipsoid))
			{
				if (!Ellipsoid.KnownList.Contains(editingInstance as Ellipsoid))
					Ellipsoid.KnownList.Add(editingInstance as Ellipsoid);

				foreach (Ellipsoid instance in Ellipsoid.KnownList)
				{
					lbList.Items.Add(instance);
				}

				label.Text = "Known Ellipsoids:";
				this.Text = "Ellipsoid Editor";
			}
			else if (editingType == typeof(Datum2WGS))
			{
				if (!Datum2WGS.KnownList.Contains(editingInstance as Datum2WGS))
					Datum2WGS.KnownList.Add(editingInstance as Datum2WGS);
				
				foreach (Datum2WGS instance in Datum2WGS.KnownList)
				{
					lbList.Items.Add(instance);
				}

				label.Text = "Known Datums:";
				this.Text = "Datum Editor";
			}
			else
				Close();

			lbList.EndUpdate();
			lbList.SelectedItem = editingInstance;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			BuildList();
		}

		private void lbList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			propertyGrid.SelectedObject = lbList.SelectedItem;
			editingInstance = lbList.SelectedItem;
		}

		private void propertyGrid_PropertyValueChanged(object s, System.Windows.Forms.PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.Label == "Name")
			{
				BuildList();
			}
		}

		#region Buttons
		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			object instance = null;

			if (editingType == typeof(Ellipsoid))
			{
				instance = new Ellipsoid();
				Ellipsoid.KnownList.Add(instance as Ellipsoid);
			}
			else if (editingType == typeof(Datum2WGS))
			{
				instance = new Datum2WGS();
				Datum2WGS.KnownList.Add(instance as Datum2WGS);
			}
			else
				return;

			lbList.Items.Add(instance);
			lbList.SelectedItem = instance;
		}

		private void btnRemove_Click(object sender, System.EventArgs e)
		{
			if (lbList.SelectedItem != null)
			{
				if (editingType == typeof(Ellipsoid))
				{
					Ellipsoid.KnownList.Remove(lbList.SelectedItem as Ellipsoid);
				}
				else if (editingType == typeof(Datum2WGS))
				{
					Datum2WGS.KnownList.Remove(lbList.SelectedItem as Datum2WGS);
				}
				lbList.Items.Remove(lbList.SelectedItem);
			}
		}

		private void btnSelect_Click(object sender, System.EventArgs e)
		{
			if (lbList.SelectedItem != null)
			{
				editingInstance = lbList.SelectedItem;
				Close();
			}
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}
		#endregion
	}
}
