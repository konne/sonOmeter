using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Runtime.InteropServices;

namespace UKLib.TreeView
{

	public class CoolTreeView : System.Windows.Forms.UserControl
	{
		private enum NodeState
		{
			Nothing = -1,
			Item = 0,
			RootItem = 1,
			Unchecked = 2,
			Checked = 3,
			GreyChecked = 4
		}

		#region Variables
		private System.Windows.Forms.ImageList ilPics;
		private System.Windows.Forms.TreeView tv;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ImageList imageList = new ImageList();
		private System.Windows.Forms.ImageList tmpList = new ImageList();

		private int imageListoffs = 0;

		#endregion

		#region Properties

		public int ImageIndex
		{
			get { return tv.ImageIndex; }
			set { tv.ImageIndex = value; }
		}


		public int SelectedImageIndex
		{
			get { return tv.SelectedImageIndex; }
			set { tv.SelectedImageIndex = value; }
		}


		public System.Windows.Forms.TreeNodeCollection Nodes
		{
			get { return tv.Nodes; }
		}


		public System.Windows.Forms.ImageList ImageList
		{
			get { return imageList; }
			/*set 
			{ 
				imageList = value;
				imageListoffs = 0;
				
				tmpList.Images.Clear();
			
				if (imageList != null) 
				{
					for(int i=0; i < imageList.Images.Count; i++)
					{
						if (imageList.Images[i] != null) 
						{
							tmpList.Images.Add(imageList.Images[i]);
							imageListoffs ++;
						}
					}
				}
				for(int i=0; i < ilPics.Images.Count; i++)
				{
					if (ilPics.Images[i] != null) 
						tmpList.Images.Add(ilPics.Images[i]);
				}
			}*/
		}


		public TreeNode SelectedNode
		{
			get { return tv.SelectedNode; }
			set { tv.SelectedNode = value; }
		}
		#endregion

		#region Design & Create
		public CoolTreeView()
		{
			InitializeComponent();

			/*	tmpList = new ImageList();
				for (int i = 0; i < ilPics.Images.Count; i++)
					tmpList.Images.Add(ilPics.Images[i]); /**/

			tv.ImageList = ilPics;
			tv.ImageIndex = (int)NodeState.Item;
			tv.SelectedImageIndex = (int)NodeState.Item; /**/
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CoolTreeView));
			this.ilPics = new System.Windows.Forms.ImageList(this.components);
			this.tv = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// ilPics
			// 
			this.ilPics.ImageSize = new System.Drawing.Size(16, 16);
			this.ilPics.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilPics.ImageStream")));
			this.ilPics.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// tv
			// 
			this.tv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tv.ImageList = this.ilPics;
			this.tv.Location = new System.Drawing.Point(0, 0);
			this.tv.Name = "tv";
			this.tv.Size = new System.Drawing.Size(288, 272);
			this.tv.TabIndex = 0;
			this.tv.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tv_KeyDown);
			this.tv.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.tv_AfterExpand);
			this.tv.Click += new System.EventHandler(this.tv_Click);
			this.tv.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.tv_AfterCollapse);
			this.tv.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tv_KeyPress);
			this.tv.DoubleClick += new System.EventHandler(this.tv_DoubleClick);
			// 
			// CoolTreeView
			// 
			this.Controls.Add(this.tv);
			this.Name = "CoolTreeView";
			this.Size = new System.Drawing.Size(288, 272);
			this.ResumeLayout(false);

		}


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

		#region Functions
		private bool isCheckbox(TreeNode node)
		{
			if ((GetState(node) == NodeState.Checked)
				| (GetState(node) == NodeState.Unchecked)
				| (GetState(node) == NodeState.GreyChecked))
				return true;
			else
				return false;
		}

		private int st2ind(NodeState state)
		{
			return (int)state + imageListoffs;
		}

		public void ExpandAll()
		{
			tv.ExpandAll();
		}

		private NodeState GetState(TreeNode node)
		{
			return (NodeState)(node.ImageIndex - imageListoffs);
		}

		private void SetState(TreeNode node, NodeState state)
		{
			NodeState oldstate = GetState(node);

			if (oldstate != state)
			{
				node.ImageIndex = st2ind(state);
				node.SelectedImageIndex = st2ind(state);
				switch (state)
				{
					case NodeState.Checked:
						node.Checked = true;
						break;
					case NodeState.Unchecked:
						node.Checked = false;
						break;
					case NodeState.GreyChecked:
						node.Checked = false;
						break;
				}
				if (NodeStateChanged != null)
					NodeStateChanged(this, new TreeViewEventArgs(node));
			}
		}

		public void SetNodeType(TreeNode node, bool CheckBox)
		{
			if (CheckBox)
			{
				if (node.Checked)
				{
					SetState(node, NodeState.Checked);
				}
				else
				{
					SetState(node, NodeState.Unchecked);
				}
			}
			else
			{
				if (node.IsExpanded)
					SetState(node, NodeState.RootItem);
				else
					SetState(node, NodeState.Item);
			}

		}


		private void UpdateCheckboxNode(TreeNode node)
		{
			if (node.ImageIndex == st2ind(NodeState.Nothing))
				return;
			if (node.ImageIndex == st2ind(NodeState.Item))
				return;
			if (node.ImageIndex == st2ind(NodeState.RootItem))
				return;

			if (node.Checked)
			{
				SetState(node, NodeState.Checked);
			}
			else
			{
				SetState(node, NodeState.Unchecked);
			}
		}


		private void UpdateNodes(TreeNode node)
		{
			foreach (TreeNode cnode in node.Nodes)
			{
				UpdateNodes(cnode);
			}

			if (node.ImageIndex == st2ind(NodeState.Nothing))
				return;
			if (node.ImageIndex == st2ind(NodeState.Item))
				return;
			if (node.ImageIndex == st2ind(NodeState.RootItem))
				return;

			NodeState nodestate = NodeState.Item;

			foreach (TreeNode cnode in node.Nodes)
			{
				if (nodestate != NodeState.GreyChecked)
				{
					switch (GetState(cnode))
					{
						case NodeState.Checked:
							if (nodestate == NodeState.Item)
								nodestate = NodeState.Checked;
							if (nodestate == NodeState.Unchecked)
								nodestate = NodeState.GreyChecked;
							break;
						case NodeState.Unchecked:
							if (nodestate == NodeState.Item)
								nodestate = NodeState.Unchecked;
							if (nodestate == NodeState.Checked)
								nodestate = NodeState.GreyChecked;
							break;
						case NodeState.GreyChecked:
							nodestate = NodeState.GreyChecked;
							break;
					}
				}
			}
			if (nodestate != NodeState.Item)
			{
				SetState(node, nodestate);

			}
			else
			{
				UpdateCheckboxNode(node);
			}
		}


		public void UpdateTree()
		{
			foreach (TreeNode cnode in tv.Nodes)
			{
				UpdateNodes(cnode);
			}
		}


		private void SetSubItems(TreeNode node)
		{
			foreach (TreeNode cnode in node.Nodes)
			{
				if (isCheckbox(node))
				{
					cnode.Checked = node.Checked;
					SetSubItems(cnode);
				}
			}
		}

		#endregion

		#region Events

		public event TreeViewEventHandler NodeClick;
		public event TreeViewEventHandler NodeDoubleClick;
		public event TreeViewEventHandler NodeStateChanged;

		private void tv_AfterExpand(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if ((e.Node.ImageIndex == st2ind(NodeState.Item)) | (e.Node.ImageIndex == -1))
			{
				e.Node.ImageIndex = st2ind(NodeState.RootItem);
				e.Node.SelectedImageIndex = st2ind(NodeState.RootItem);
			}
		}


		private void tv_AfterCollapse(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (e.Node.ImageIndex == st2ind(NodeState.RootItem))
			{
				e.Node.ImageIndex = st2ind(NodeState.Item);
				e.Node.SelectedImageIndex = st2ind(NodeState.Item);
			}
		}


		private void tv_Click(object sender, System.EventArgs e)
		{
			Point pt = tv.PointToClient(Control.MousePosition);
			TreeNode node = tv.GetNodeAt(pt);

			if (node != null)
			{

				tv.SelectedNode = node;

				if (node.Bounds.Contains(pt) & (NodeClick != null))
				{
					NodeClick(this, new TreeViewEventArgs(node));
				}

				Rectangle rt = node.Bounds;

				rt.Height = 16;
				rt.Width = 16;
				rt.X -= 16;

				if (rt.Contains(pt))
				{
					if (isCheckbox(node))
					{
						node.Checked = !node.Checked;
						SetSubItems(node);
						UpdateTree();
					}
					else
						node.Checked = false;
				}
			}
		}


		private void tv_DoubleClick(object sender, System.EventArgs e)
		{
			Point pt = tv.PointToClient(Control.MousePosition);
			TreeNode node = tv.GetNodeAt(pt);

			if (node != null)
			{

				if (node.Bounds.Contains(pt) & (NodeDoubleClick != null))
				{
					NodeDoubleClick(this, new TreeViewEventArgs(node));
				}

				Rectangle rt = node.Bounds;

				rt.Height = 16;
				rt.Width = 16;
				rt.X -= 16;

				if (rt.Contains(pt))
				{
					if (isCheckbox(node))
					{
						node.Checked = !node.Checked;
						UpdateCheckboxNode(node);
					}
					else
						node.Checked = false;
				}
			}
		}


		#endregion

		private void tv_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			this.OnKeyPress(e);
		}

		private void tv_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			this.OnKeyDown(e);
		}

	}
}
