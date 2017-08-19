//ORIGINAL LGPL SOURCE CODE FINDED ON COMPONA LGPL SOURCE CODE
using System;
using System.Drawing;
using UKLib.Fireball.Win32;

namespace UKLib.Fireball.Drawing.GDI
{
	public class GDIPen : GDIObject
	{
		public IntPtr hPen;

		public GDIPen(Color color, int width)
		{
			hPen = NativeMethods.CreatePen(0, width, NativeMethods.ColorToInt(color));
			Create();
		}

		protected override void Destroy()
		{
			if (hPen != (IntPtr) 0)
				NativeMethods.DeleteObject(hPen);
			base.Destroy();
			hPen = (IntPtr) 0;
		}

		protected override void Create()
		{
			base.Create();
		}
	}
}