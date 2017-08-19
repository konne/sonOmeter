using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using DockDotNET;
using System.Drawing.Drawing2D;

namespace sample
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class frmMain : System.Windows.Forms.Form
	{
		private DockDotNET.DockManager dockManager;
		private MenuStrip menuStrip;
		private ToolStripMenuItem windowToolStripMenuItem;
		private ToolStripMenuItem newToolWindowToolStripMenuItem;
		private ToolStripMenuItem newDocumentToolStripMenuItem;
		private ToolStripMenuItem closeAllDocumentsToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem showHideonlyWindowToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem readXmlToolStripMenuItem;
		private ToolStripMenuItem writeXmlToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem closeToolStripMenuItem;
		private System.ComponentModel.IContainer components;

		// A persistent (hide-only) window needs a variable for access
		frmHideOnly formHideOnly = new frmHideOnly();

		public frmMain()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Enable the right visual style fpr VS2005.
			DockManager.FastMoveDraw = false;
			DockManager.Style = DockVisualStyle.VS2005;

			// Some demo windows.
			frmTool tool = new frmTool();
			tool.AllowClose = false;
			frmTool tool2 = new frmTool();
			frmDoc doc = new frmDoc();
			frmDoc doc2 = new frmDoc();
			
			// This function docks a window directly to the container.
			dockManager.DockWindow(doc, DockStyle.Fill);
			dockManager.DockWindow(tool, DockStyle.Left);
			
			// This function retrieves the host of a container.
			DockContainer cont = tool.HostContainer;
			cont.DockWindow(tool2, DockStyle.Fill);
			cont.DockWindow(formHideOnly, DockStyle.Top);

			cont = doc.HostContainer;
			cont.DockWindow(doc2, DockStyle.Fill);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.dockManager = new DockDotNET.DockManager(this.components);
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newDocumentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.closeAllDocumentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.showHideonlyWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.readXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.writeXmlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// dockManager
			// 
			this.dockManager.BackColor = System.Drawing.SystemColors.ControlDark;
			this.dockManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dockManager.DockBorder = 20;
			this.dockManager.DockType = DockDotNET.DockContainerType.Document;
			this.dockManager.FastDrawing = true;
			this.dockManager.Location = new System.Drawing.Point(0, 24);
			this.dockManager.Name = "dockManager";
			this.dockManager.Padding = new System.Windows.Forms.Padding(2, 23, 2, 2);
			this.dockManager.ShowIcons = true;
			this.dockManager.Size = new System.Drawing.Size(480, 333);
			this.dockManager.SplitterWidth = 4;
			this.dockManager.TabIndex = 0;
			this.dockManager.VisualStyle = DockDotNET.DockVisualStyle.VS2003;
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(480, 24);
			this.menuStrip.TabIndex = 1;
			this.menuStrip.Text = "menuStrip1";
			// 
			// windowToolStripMenuItem
			// 
			this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolWindowToolStripMenuItem,
            this.newDocumentToolStripMenuItem,
            this.closeAllDocumentsToolStripMenuItem,
            this.toolStripSeparator1,
            this.showHideonlyWindowToolStripMenuItem,
            this.toolStripSeparator2,
            this.readXmlToolStripMenuItem,
            this.writeXmlToolStripMenuItem,
            this.toolStripSeparator3,
            this.closeToolStripMenuItem});
			this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
			this.windowToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
			this.windowToolStripMenuItem.Text = "Window";
			this.windowToolStripMenuItem.DropDownOpening += new System.EventHandler(this.windowToolStripMenuItem_DropDownOpening);
			// 
			// newToolWindowToolStripMenuItem
			// 
			this.newToolWindowToolStripMenuItem.Name = "newToolWindowToolStripMenuItem";
			this.newToolWindowToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.newToolWindowToolStripMenuItem.Text = "New tool window";
			this.newToolWindowToolStripMenuItem.Click += new System.EventHandler(this.miNewTool_Click);
			// 
			// newDocumentToolStripMenuItem
			// 
			this.newDocumentToolStripMenuItem.Name = "newDocumentToolStripMenuItem";
			this.newDocumentToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.newDocumentToolStripMenuItem.Text = "New document";
			this.newDocumentToolStripMenuItem.Click += new System.EventHandler(this.miNewDoc_Click);
			// 
			// closeAllDocumentsToolStripMenuItem
			// 
			this.closeAllDocumentsToolStripMenuItem.Name = "closeAllDocumentsToolStripMenuItem";
			this.closeAllDocumentsToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.closeAllDocumentsToolStripMenuItem.Text = "Close all documents";
			this.closeAllDocumentsToolStripMenuItem.Click += new System.EventHandler(this.CloseDocuments);
			// 
			// showHideonlyWindowToolStripMenuItem
			// 
			this.showHideonlyWindowToolStripMenuItem.Name = "showHideonlyWindowToolStripMenuItem";
			this.showHideonlyWindowToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.showHideonlyWindowToolStripMenuItem.Text = "Show hide-only window";
			this.showHideonlyWindowToolStripMenuItem.Click += new System.EventHandler(this.showHideonlyWindowToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(194, 6);
			// 
			// readXmlToolStripMenuItem
			// 
			this.readXmlToolStripMenuItem.Name = "readXmlToolStripMenuItem";
			this.readXmlToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.readXmlToolStripMenuItem.Text = "Read Xml";
			this.readXmlToolStripMenuItem.Click += new System.EventHandler(this.miReadXML_Click);
			// 
			// writeXmlToolStripMenuItem
			// 
			this.writeXmlToolStripMenuItem.Name = "writeXmlToolStripMenuItem";
			this.writeXmlToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.writeXmlToolStripMenuItem.Text = "Write Xml";
			this.writeXmlToolStripMenuItem.Click += new System.EventHandler(this.miWriteXML_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(194, 6);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
			this.closeToolStripMenuItem.Text = "Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.miClose_Click);
			// 
			// frmMain
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(480, 357);
			this.Controls.Add(this.dockManager);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "frmMain";
			this.Text = "Sample application for DockDotNET library";
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new frmMain());
		}

		private void miClose_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		#region This region demonstrates opening and closing tool and document windows
		private void miNewTool_Click(object sender, System.EventArgs e)
		{
			frmTool frm = new frmTool();
			frm.Show();
		}

		private void miNewDoc_Click(object sender, System.EventArgs e)
		{
			frmDoc frm = new frmDoc();
			frm.Show();
		}

		private void CloseDocuments(object sender, System.EventArgs e)
		{
			try
			{
				ArrayList closeList = new ArrayList();

				foreach (DockPanel p in DockManager.ListDocument)
					closeList.Add(p.Form);

				foreach (DockWindow wnd in closeList)
					wnd.Visible = false;

				closeList.Clear();
			}
			catch (Exception ex)
			{
				Console.WriteLine("frmMain.CloseDocuments: " + ex.Message);
			}
		}
		#endregion

		#region This region demonstrates loading and saving of the window structure
		private void miWriteXML_Click(object sender, EventArgs e)
		{
			DockManager.WriteXml("Test.xml");
		}

		private void miReadXML_Click(object sender, EventArgs e)
		{
			DockManager.ReadXml("Test.xml");
		}
		#endregion

		#region This region demonstrates a hide-only window
		private void showHideonlyWindowToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (formHideOnly != null)
				formHideOnly.Visible = !formHideOnly.Visible;
		}

		private void windowToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
		{
			showHideonlyWindowToolStripMenuItem.Checked = formHideOnly.Visible;
		}
		#endregion
	}
}
