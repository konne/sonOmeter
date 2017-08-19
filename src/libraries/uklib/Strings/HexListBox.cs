using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace UKLib.Strings
{
	public partial class HexListBox : ListBox
	{
		#region Variables
		protected int bytesPerLine = 8;
		protected int maxLines = 0;
		protected int lastIndex = -1;

		protected MemoryStream stream;
		#endregion

		#region Properties
		public int BytesPerLine
		{
			get { return bytesPerLine; }
			set { bytesPerLine = value; }
		}

		public int MaxLines
		{
			get { return maxLines; }
			set { maxLines = value; }
		}
		#endregion

		#region Constructor
		public HexListBox()
		{
			InitializeComponent();

			// Initialize stream.
			stream = new MemoryStream();
		} 
		#endregion

		public void AddString(string s)
		{
			AddBytes(System.Text.Encoding.Default.GetBytes(s));
		}

		public void AddHexString(string s)
		{
			int max = s.Length >> 1;

			byte[] buf = new byte[max];

			for (int i = 0; i < max; i++)
				buf[i] = (byte)(Uri.FromHex(s[i * 2]) * 16 + Uri.FromHex(s[i * 2 + 1]));

			AddBytes(buf);
		}

		public void AddBytes(byte[] buf)
		{
			AddBytes(buf, 0, buf.Length);
		}

		public void AddBytes(byte[] buf, int count)
		{
			AddBytes(buf, 0, count);
		}

		public void AddBytes(byte[] buf, int offset, int count)
		{
			stream.Write(buf, offset, count);

			int n = (int)stream.Length / bytesPerLine;
			if (n * bytesPerLine < stream.Length)
				n++;
			n = (n - maxLines) * bytesPerLine;

			if ((maxLines > 0) && (n > 0))
			{
				byte[] myBuf = stream.ToArray();

				MemoryStream newStream = new MemoryStream();
				newStream.Write(myBuf, n, myBuf.Length - n);

				stream.Close();
				stream = newStream;

				this.Items.Clear();
				lastIndex = -1;
			}

			RebuildList();
		}

		public void RebuildList()
		{
			BeginUpdate();

			// Get the buffer.
			byte[] myBuf = stream.ToArray();

			string lineA = "";
			string lineB = "";
			int length = myBuf.Length;
			int i = 0;
			int n = 0;

			// Remove last line, if needed.            
			if (lastIndex != -1)
			{
                if (Items.Count < lastIndex)
                {
                    lastIndex = -1;
                }
                else
                {
                    this.Items.RemoveAt(lastIndex);
                    n = lastIndex * bytesPerLine;
                }
			}

			// Browse through the new buffer content.
			for (i = n; i < length; i++)
			{
				lineA += myBuf[i].ToString("X2") + " ";
				lineB += (char)myBuf[i];

				if ((i + 1) % bytesPerLine == 0)
				{
					lastIndex = this.Items.Add(lineA + "  " + lineB);
					lineA = "";
					lineB = "";
				}
			}

			// Fill line with spacers.
			if ((i != 0) && (i % bytesPerLine != 0))
			{
				while (i % bytesPerLine != 0)
				{
					lineA += "00 ";
					lineB += '\0';
					i++;
				}

				lastIndex = this.Items.Add(lineA + "  " + lineB);
			}

			EndUpdate();
		}
	}
}
