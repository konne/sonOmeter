using System;

namespace sonOmeter.Classes
{
	/// <summary>
	/// Argument for events concerning a record, profile or 3D record and one of its devices.
	/// </summary>
	public class RecordEventArgs : EventArgs
	{
        #region Properties
        private long id;
        public long ID
        {
            get { return id; }
        }

        private SonarRecord rec;
        public SonarRecord Rec
        {
            get { return rec; }
        }

		private SonarDevice dev;
        public SonarDevice Dev
		{
			get { return dev; }
		}

		private object tag;
		public object Tag
		{
			get { return tag; }
		}
		#endregion

        static long counter = 0;

        public RecordEventArgs(SonarRecord rec, SonarDevice dev)
		{
			this.rec = rec;
			this.dev = dev;
            this.tag = null;

            this.id = counter++;
		}

		public RecordEventArgs(SonarRecord rec, SonarDevice dev, object tag)
		{
			this.rec = rec;
			this.dev = dev;
            this.tag = tag;

            this.id = counter++;
		}
	}
}
