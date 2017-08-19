using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;
using Microsoft.VisualBasic.FileIO;
using System.Xml;
using System.Xml.Serialization;

namespace sonOmeter.Classes
{
	public delegate void OpenMRUFileHandler(string fileName);

	/// <summary>
	/// MRU manager - manages Most Recently Used Files list
	/// for Windows Form application.
	/// </summary>
	public class MRUManager : Component
	{
		#region Members
		private ToolStripMenuItem menuItemMRU;		// Recent Files menu item
		private ToolStripMenuItem menuItemParent;	// Recent Files menu item parent

		private int maxNumberOfFiles = 5;			// maximum number of files in MRU list
		private int maxDisplayLength = 40;			// maximum length of file name for display

		private string currentDirectory;			// current directory

		private Collection<string> mruList;			// MRU list (file names)
		#endregion

		#region Constructor
		public MRUManager()
		{
			mruList = new Collection<string>();

			// keep current directory in the time of initialization
			currentDirectory = Directory.GetCurrentDirectory();
		}
		#endregion

		#region Public Properties
		/// <summary>
		/// Maximum length of displayed file name in menu (default is 40).
		/// </summary>
		public int MaxDisplayNameLength
		{
			get { return maxDisplayLength; }
			set
			{
				maxDisplayLength = value;

				if (maxDisplayLength < 10)
					maxDisplayLength = 10;
			}
		}

		/// <summary>
		/// Maximum length of MRU list (default is 10).
		/// </summary>
		public int MaxMRULength
		{
			get { return maxNumberOfFiles; }
			set
			{
				maxNumberOfFiles = value;

				if (maxNumberOfFiles < 1)
					maxNumberOfFiles = 1;

				while (mruList.Count > maxNumberOfFiles)
					mruList.RemoveAt(mruList.Count - 1);
			}
		}

		/// <summary>
		/// Set current directory.
		/// Default value is program current directory which is set when
		/// Initialize function is called.
		/// </summary>
		public string CurrentDir
		{
			get { return currentDirectory; }
			set { currentDirectory = value; }
		}

		public ToolStripMenuItem MenuItemMRU
		{
			get { return menuItemMRU; }
			set
			{
				// keep reference to MRU menu item
				menuItemMRU = value;

				// keep reference to MRU menu item parent
				try
				{
					menuItemParent = (ToolStripMenuItem)menuItemMRU.OwnerItem;
				}
				catch
				{
				}

				if (menuItemParent == null)
				{
					throw new Exception(
						"MRUManager: Cannot find parent of MRU menu item");
				}

				// subscribe to MRU parent Popup event
				menuItemParent.DropDownOpening += new EventHandler(OnMRUParentDropDown);

				// load MRU list from XML
				LoadMRU();
			}
		}
		#endregion

		#region Public Functions
		/// <summary>
		/// Initialization. Call this function in form Load handler.
		/// </summary>
		/// <param name="owner">Owner form</param>
		public void Initialize(Form owner)
		{
			// subscribe to owner form Closing event            
			owner.Closing += new System.ComponentModel.CancelEventHandler(OnOwnerClosing);
		}

		/// <summary>
		/// Add file name to MRU list.
		/// Call this function when file is opened successfully.
		/// If file already exists in the list, it is moved to the first place.
		/// </summary>
		/// <param name="file">File Name</param>
		public void Add(string file)
		{
			Remove(file);

			// if array has maximum length, remove last element
			if (mruList.Count == maxNumberOfFiles)
				mruList.RemoveAt(maxNumberOfFiles - 1);

			// add new file name to the start of array
			mruList.Insert(0, file);
		}

		/// <summary>
		/// Remove file name from MRU list.
		/// Call this function when File - Open operation failed.
		/// </summary>
		/// <param name="file">File Name</param>
		public void Remove(string file)
		{
			foreach (string f in mruList)
			{
				if (f == file)
				{
					mruList.Remove(f);
					return;
				}
			}
		}
		#endregion

		#region Event Handlers
		/// <summary>
		/// Update MRU list when MRU menu item parent is opened
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMRUParentDropDown(object sender, EventArgs e)
		{        
			// remove all childs
			if (menuItemMRU.HasDropDownItems)
				menuItemMRU.DropDownItems.Clear();
			
			// Disable menu item if MRU list is empty
			if (mruList.Count == 0)
			{
				menuItemMRU.Enabled = false;
				return;
			}

			// enable menu item and add child items
			menuItemMRU.Enabled = true;

			ToolStripMenuItem item;
		
			for (int i = mruList.Count - 1; i >= 0; i--)
			{
                try
                {
                    item = new ToolStripMenuItem(GetDisplayName(mruList[i]));

                    // subscribe to item's Click event
                    item.Click += new EventHandler(this.OnMRUClicked);
                    item.Tag = mruList[i];
                    item.ShortcutKeys = Keys.Control | (Keys)((int)(i + 1).ToString()[0]);

                    menuItemMRU.DropDownItems.Insert(0, item);
                }
                catch(Exception ex)
                {
                    UKLib.Debug.DebugClass.SendDebugLine(this, UKLib.Debug.DebugLevel.Red, ex.Message);
                }
			}
		}

		/// <summary>
		/// MRU menu item is clicked - call owner's OpenMRUFile function
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMRUClicked(object sender, EventArgs e)
		{
			string s;

			try
			{
				// cast sender object to MenuItem
				ToolStripMenuItem item = (ToolStripMenuItem)sender;

				if (item != null)
				{
					// Get file name from list using item index
					s = (string)item.Tag;

					// call owner's OpenMRUFile function
					if (s.Length > 0)
					{
						if (OpenMRUFile != null)
							OpenMRUFile(s);
					}
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Exception in OnMRUClicked: " + ex.Message);
                
			}
		}

		/// <summary>
		/// Save MRU list in Registry when owner form is closing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnOwnerClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
                XmlSerializer xs = new XmlSerializer(mruList.GetType());
                StringWriter sw = new StringWriter();
                xs.Serialize(sw, mruList);
               // sonOmeter.
                


				string path = SpecialDirectories.CurrentUserApplicationData;
				string file = path.Remove(path.LastIndexOf('\\')) + "\\MRUlist.xml";

				// Save the MRU list.
				XmlTextWriter writer = new XmlTextWriter(file, Encoding.UTF8);

				writer.WriteStartDocument();
				writer.WriteStartElement("MRUlist");

				for (int i = mruList.Count - 1; i >= 0; i--)
				{
					writer.WriteStartElement("MRU");
					writer.WriteAttributeString("value", mruList[i]);
					writer.WriteEndElement();
				}

				writer.WriteEndElement();
				writer.WriteEndDocument();
				writer.Close();
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Saving MRU to Registry failed: " + ex.Message);
                
			}
		}
		#endregion

		#region Private Functions
		/// <summary>
		/// Load MRU list from Registry.
		/// Called from Initialize.
		/// </summary>
		private void LoadMRU()
		{
			try
			{
				mruList.Clear();

				// Load the MRU list.
				string path = SpecialDirectories.CurrentUserApplicationData;
				string file = path.Remove(path.LastIndexOf('\\')) + "\\MRUlist.xml";

				if (!File.Exists(file))
					return;

				XmlTextReader reader = new XmlTextReader(file);
				reader.WhitespaceHandling = WhitespaceHandling.None;

				while (reader.Read())
				{
					if (!reader.IsStartElement())
						continue;

					if (reader.Name != "MRU")
						continue;

					string mru = reader.GetAttribute("value");

					if ((mru == null) || !File.Exists(mru))
						continue;

					Add(mru);
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine("Loading MRU from Registry failed: " + ex.Message);
			}
		}

		/// <summary>
		/// Get display file name from full name.
		/// </summary>
		/// <param name="fullName">Full file name</param>
		/// <returns>Short display name</returns>
		private string GetDisplayName(string fullName)
		{
			// if file is in current directory, show only file name
			FileInfo fileInfo = new FileInfo(fullName);

			return GetShortDisplayName(fileInfo, maxDisplayLength, (fileInfo.DirectoryName != currentDirectory));
		}

		/// <summary>
		/// Truncate a path to fit within a certain number of characters 
		/// by replacing path components with ellipses.
		/// </summary>
		/// <param name="fileInfo">The file info structure</param>
		/// <param name="maxLen">Maximum length</param>
		/// <returns>Truncated file name</returns>
		/// <param name="showDir">Add the directory name</param>
		public static string GetShortDisplayName(FileInfo fileInfo, int maxLen, bool showDir)
		{
			string longName = showDir ? fileInfo.FullName : fileInfo.Name;

			if (longName.Length <= maxLen)
				return longName;

			int delta = fileInfo.Name.Length+5 - maxLen;

			if (delta > 0)
			{
				return "..." + fileInfo.Name.Substring(delta + 3);
			}

			return fileInfo.FullName.Remove(maxLen - 5 - fileInfo.Name.Length) + "...\\" + fileInfo.Name;
		}
		#endregion

		public event OpenMRUFileHandler OpenMRUFile;
	}
}
