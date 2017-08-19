namespace sonOmeter
{
	partial class frmManualInput
	{
		/// <summary>
		/// Erforderliche Designervariable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Verwendete Ressourcen bereinigen.
		/// </summary>
		/// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Vom Windows Form-Designer generierter Code

		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmManualInput));
			this.labDistance = new System.Windows.Forms.Label();
			this.tbDistance = new System.Windows.Forms.TextBox();
			this.tbDepth = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.labDescription = new System.Windows.Forms.Label();
			this.lbBuoyList = new System.Windows.Forms.ListBox();
			this.btnAdd = new System.Windows.Forms.Button();
			this.tbType = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.lbBuoyList2 = new System.Windows.Forms.ListBox();
			this.cbMode = new System.Windows.Forms.ComboBox();
			this.panFullGroup = new System.Windows.Forms.Panel();
			this.panDirectOnlyGroup = new System.Windows.Forms.Panel();
			this.panFullGroup.SuspendLayout();
			this.panDirectOnlyGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// labDistance
			// 
			this.labDistance.AutoSize = true;
			this.labDistance.Location = new System.Drawing.Point(0, 30);
			this.labDistance.Name = "labDistance";
			this.labDistance.Size = new System.Drawing.Size(145, 13);
			this.labDistance.TabIndex = 0;
			this.labDistance.Text = "Distance to boat location (m):";
			// 
			// tbDistance
			// 
			this.tbDistance.Location = new System.Drawing.Point(169, 27);
			this.tbDistance.Name = "tbDistance";
			this.tbDistance.Size = new System.Drawing.Size(60, 20);
			this.tbDistance.TabIndex = 1;
			this.tbDistance.Text = "0";
			this.tbDistance.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// tbDepth
			// 
			this.tbDepth.Location = new System.Drawing.Point(169, 0);
			this.tbDepth.Name = "tbDepth";
			this.tbDepth.Size = new System.Drawing.Size(60, 20);
			this.tbDepth.TabIndex = 2;
			this.tbDepth.Text = "0";
			this.tbDepth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(0, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(104, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Measured depth (m):";
			// 
			// labDescription
			// 
			this.labDescription.AutoSize = true;
			this.labDescription.Location = new System.Drawing.Point(1, 55);
			this.labDescription.Name = "labDescription";
			this.labDescription.Size = new System.Drawing.Size(176, 13);
			this.labDescription.TabIndex = 0;
			this.labDescription.Text = "Select the destination buoy from list:";
			// 
			// lbBuoyList
			// 
			this.lbBuoyList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.lbBuoyList.FormattingEnabled = true;
			this.lbBuoyList.Location = new System.Drawing.Point(0, 71);
			this.lbBuoyList.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
			this.lbBuoyList.Name = "lbBuoyList";
			this.lbBuoyList.Size = new System.Drawing.Size(116, 108);
			this.lbBuoyList.Sorted = true;
			this.lbBuoyList.TabIndex = 3;
			this.lbBuoyList.SelectedIndexChanged += new System.EventHandler(this.lbBuoyList_SelectedIndexChanged);
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.Location = new System.Drawing.Point(0, 191);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Size = new System.Drawing.Size(229, 23);
			this.btnAdd.TabIndex = 4;
			this.btnAdd.Text = "Add to current record";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// tbType
			// 
			this.tbType.Location = new System.Drawing.Point(169, 26);
			this.tbType.Name = "tbType";
			this.tbType.Size = new System.Drawing.Size(60, 20);
			this.tbType.TabIndex = 2;
			this.tbType.Text = "0";
			this.tbType.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(1, 29);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(57, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Point type:";
			// 
			// lbBuoyList2
			// 
			this.lbBuoyList2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.lbBuoyList2.FormattingEnabled = true;
			this.lbBuoyList2.Location = new System.Drawing.Point(116, 71);
			this.lbBuoyList2.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this.lbBuoyList2.Name = "lbBuoyList2";
			this.lbBuoyList2.Size = new System.Drawing.Size(113, 108);
			this.lbBuoyList2.Sorted = true;
			this.lbBuoyList2.TabIndex = 3;
			this.lbBuoyList2.SelectedIndexChanged += new System.EventHandler(this.lbBuoyList2_SelectedIndexChanged);
			// 
			// cbMode
			// 
			this.cbMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbMode.FormattingEnabled = true;
			this.cbMode.Items.AddRange(new object[] {
            "Extrapolate from buoy",
            "Extrapolate from buoy connection",
            "Use direct coordinate"});
			this.cbMode.Location = new System.Drawing.Point(0, 0);
			this.cbMode.Name = "cbMode";
			this.cbMode.Size = new System.Drawing.Size(229, 21);
			this.cbMode.TabIndex = 5;
			this.cbMode.SelectedIndexChanged += new System.EventHandler(this.ModeChanged);
			// 
			// panFullGroup
			// 
			this.panFullGroup.Controls.Add(this.cbMode);
			this.panFullGroup.Controls.Add(this.tbDistance);
			this.panFullGroup.Controls.Add(this.labDistance);
			this.panFullGroup.Location = new System.Drawing.Point(12, 13);
			this.panFullGroup.Name = "panFullGroup";
			this.panFullGroup.Size = new System.Drawing.Size(229, 53);
			this.panFullGroup.TabIndex = 6;
			// 
			// panDirectOnlyGroup
			// 
			this.panDirectOnlyGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.panDirectOnlyGroup.Controls.Add(this.btnAdd);
			this.panDirectOnlyGroup.Controls.Add(this.lbBuoyList2);
			this.panDirectOnlyGroup.Controls.Add(this.lbBuoyList);
			this.panDirectOnlyGroup.Controls.Add(this.tbType);
			this.panDirectOnlyGroup.Controls.Add(this.tbDepth);
			this.panDirectOnlyGroup.Controls.Add(this.labDescription);
			this.panDirectOnlyGroup.Controls.Add(this.label4);
			this.panDirectOnlyGroup.Controls.Add(this.label2);
			this.panDirectOnlyGroup.Location = new System.Drawing.Point(12, 66);
			this.panDirectOnlyGroup.Name = "panDirectOnlyGroup";
			this.panDirectOnlyGroup.Size = new System.Drawing.Size(229, 214);
			this.panDirectOnlyGroup.TabIndex = 6;
			// 
			// frmManualInput
			// 
			this.ClientSize = new System.Drawing.Size(253, 292);
			this.Controls.Add(this.panDirectOnlyGroup);
			this.Controls.Add(this.panFullGroup);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MinimumSize = new System.Drawing.Size(200, 200);
			this.Name = "frmManualInput";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Manual Input";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmManualInput_FormClosed);
			this.panFullGroup.ResumeLayout(false);
			this.panFullGroup.PerformLayout();
			this.panDirectOnlyGroup.ResumeLayout(false);
			this.panDirectOnlyGroup.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labDistance;
		private System.Windows.Forms.TextBox tbDistance;
		private System.Windows.Forms.TextBox tbDepth;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labDescription;
		private System.Windows.Forms.ListBox lbBuoyList;
		private System.Windows.Forms.Button btnAdd;
		private System.Windows.Forms.TextBox tbType;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListBox lbBuoyList2;
		private System.Windows.Forms.ComboBox cbMode;
		private System.Windows.Forms.Panel panFullGroup;
		private System.Windows.Forms.Panel panDirectOnlyGroup;
	}
}
