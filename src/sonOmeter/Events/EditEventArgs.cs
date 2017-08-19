using System;
using System.Globalization;
using sonOmeter.Classes;

namespace sonOmeter
{
	/// <summary>
	/// Summary description for EditEventArgs.
	/// </summary>
	public class EditEventArgs : EventArgs
	{
		#region Variables
		string editString = "0";
		
		EditModes editMode = EditModes.Nothing;
		CutMode cutMode = CutMode.Nothing;
		CutMode invCutMode = CutMode.Nothing;
		
		SonarPanelType srcPanelType = SonarPanelType.Void;
		SonarPanelType dstPanelType = SonarPanelType.Void;
		#endregion

		#region Properties
		public string EditString
		{
			get { return editString; }
		}
		
		public EditModes EditMode
		{
			get { return editMode; }
		}

		public CutMode CutMode
		{
			get { return cutMode; }
		}

		public CutMode InvCutMode
		{
			get { return invCutMode; }
		}

		public SonarPanelType SrcPanelType
		{
			get { return srcPanelType; }
		}

		public SonarPanelType DstPanelType
		{
			get { return dstPanelType; }
		}
		#endregion

		public EditEventArgs(string editString, EditModes editMode, CutMode cutMode, CutMode invCutMode, SonarPanelType srcPanelType, SonarPanelType dstPanelType)
		{
			this.editMode = editMode;
			this.cutMode = cutMode;
			this.invCutMode = invCutMode;
			this.editString = editString;
			this.srcPanelType = srcPanelType;
			this.dstPanelType = dstPanelType;
		}

		public float ToSingle(NumberFormatInfo nfi)
		{
			return Convert.ToSingle(editString, nfi);
		}

		public double ToDouble(NumberFormatInfo nfi)
		{
			return Convert.ToDouble(editString, nfi);
		}
	}

	/// <summary>
	/// Edit event callback.
	/// </summary>
	public delegate void EditEventHandler(object sender, EditEventArgs e);
}
