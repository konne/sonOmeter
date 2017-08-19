using System;
using System.Drawing;

namespace DockDotNET
{
	/// <summary>
	/// FormLoadEventArgs is used with FormLoadEventHandler to enable the main application to react properly on loaded windows from a previously saved hierarchy file.
	/// </summary>
	public class FormLoadEventArgs : EventArgs
	{
		#region Variables
        private DockWindow form = null;
        private Type type = null;
        private bool cancel = false;
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the form that should be used for hierarchy load process.
		/// </summary>
		public DockWindow Form
		{
			get { return form; }
			set { form = value; }
		}

        /// <summary>
        /// Gets the type of the form that is to be loaded.
        /// </summary>
        public Type Type
        {
            get { return type; }
        }

        /// <summary>
        /// Gets or sets the cancel flag.
        /// </summary>
        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
		#endregion

		#region Constructor
		/// <summary>
        /// Creates a new FormLoadEventArgs object.
		/// </summary>
        /// <param name="type">The type of the form that is to be loaded.</param>
        public FormLoadEventArgs(Type type)
		{
            this.form = null;
            this.type = type;
            this.cancel = false;
		}
		#endregion
	}

	/// <summary>
	/// The event handler for DockManager.FormLoad events.
	/// </summary>
    public delegate void FormLoadEventHandler(object sender, FormLoadEventArgs e);
}
