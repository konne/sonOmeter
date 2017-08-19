using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;
using System.Xml;

namespace DockDotNET
{
    /// <summary>
    /// Enumerates the standard types for dockable windows.
    /// </summary>
    public enum DockContainerType
    {
        /// <summary>
        /// No docking features enabled.
        /// This window will behave normally.
        /// </summary>
        None,
        /// <summary>
        /// The window will get the document status.
        /// Document windows can solely be added (fill or split) to other documents.
        /// </summary>
        Document,
        /// <summary>
        /// The window will get the tool window status.
        /// Tool windows may be docked everywhere.
        /// </summary>
        ToolWindow
    }

    /// <summary>
    /// The DockWindow class is derived from the standard framework class System.Windows.Forms.Form.
    /// It prepares the window for docking with the help of an own container of the DockPanel type.
    /// </summary>
    [Designer(typeof(System.Windows.Forms.Design.ControlDesigner))]
    public class DockWindow : System.Windows.Forms.Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        #region Construction and dispose
        /// <summary>
        /// Initializes a new instance of the <see cref="DockWindow"/> class.
        /// </summary>
        public DockWindow()
        {
            InitializeComponent();

            if (!this.Modal)
            {
                this.Opacity = 0;

                ShowInTaskbar = false;

                controlContainer.Form = this;
                controlContainer.Resize += new EventHandler(controlContainer_Resize);
                controlContainer.Activated += new EventHandler(controlContainer_Activated);
                controlContainer.Deactivate += new EventHandler(controlContainer_Deactivate);
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DockWindow
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(256, 228);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DockWindow";
            this.ShowInTaskbar = false;
            this.Text = "DockWindow";
            this.ResumeLayout(false);

        }
        #endregion

        #region Variables
        private bool isLoaded = false;
        private bool wasDocked = false;
        private bool hideOnClose = false;
        private bool allowDock = true;
        private bool allowSave = true;
        private bool allowUnDock = true;
        private bool allowClose = true;
        private bool allowSplit = true;
        private bool layoutFinished = false;
        private bool showFormAtOnLoad = true;
        private bool showCommandInProcess = false;

        private DockContainerType dockType = DockContainerType.None;
        private DockContainer lastHost = null;
        internal DockContainer dragTarget = null;

        private DockPanel controlContainer = new DockPanel();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the dock element type.
        /// </summary>
        [Category("DockDotNET"), Description("Gets or sets the dock element type.")]
        public DockContainerType DockType
        {
            get { return dockType; }
            set { dockType = value; }
        }

        /// <summary>
        /// Allow this window to be undocked.
        /// </summary>
        [Category("DockDotNET"), Description("Allow this window to be undocked.")]
        public bool AllowUnDock
        {
            get { return allowUnDock; }
            set { allowUnDock = value; }
        }

        /// <summary>
        /// Allow this window to be closed.
        /// </summary>
        [Category("DockDotNET"), Description("Allow this window to be closed.")]
        public bool AllowClose
        {
            get { return allowClose; }
            set { allowClose = value; }
        }

        /// <summary>
        /// Allow other windows or containers to dock into this one.
        /// </summary>
        [Category("DockDotNET"), Description("Allow other windows or containers to dock into this one.")]
        public bool AllowDock
        {
            get { return allowDock; }
            set { allowDock = value; }
        }

        /// <summary>
        /// Allow other windows to split the control container.
        /// </summary>
        [Category("DockDotNET"), Description("Allow other windows to split the control container.")]
        public bool AllowSplit
        {
            get { return allowSplit; }
            set { allowSplit = value; }
        }

        /// <summary>
        /// Allow the framework to save and restore the window position.
        /// </summary>
        [Category("DockDotNET"), Description("Allow the framework to save and restore the window position.")]
        public bool AllowSave
        {
            get { return allowSave; }
            set { allowSave = value; }
        }

        /// <summary>
        /// Gets the panel that is connected to this window. All controls are transferred to this container yielding an exact copy of the window content.
        /// </summary>
        [Browsable(false)]
        public DockPanel ControlContainer
        {
            get { return controlContainer; }
            set { controlContainer = value; }
        }

        /// <summary>
        /// Gets the retrieved target of a drag operation.
        /// </summary>
        [Browsable(false)]
        internal DockContainer DragTarget
        {
            get { return dragTarget; }
        }

        /// <summary>
        /// Gets the container of the associated panel (null if not docked).
        /// </summary>
        [Browsable(false)]
        public DockContainer HostContainer
        {
            get { return controlContainer.Parent as DockContainer; }
        }

        /// <summary>
        /// Gets the docking state of the window.
        /// </summary>
        [Browsable(false)]
        public bool IsDocked
        {
            get
            {
                DockContainer cont = controlContainer.Parent as DockContainer;

                if (cont == null)
                    return false;

                if (cont.IsRootContainer & (cont.panList.Count == 1) & (cont.conList.Count == 0))
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Gets the docking state after last close.
        /// </summary>
        [Browsable(false)]
        public bool WasDocked
        {
            get { return wasDocked; }
        }

        /// <summary>
        /// Gets the loaded flag.
        /// </summary>
        [Browsable(false)]
        public bool IsLoaded
        {
            get { return isLoaded; }
        }

        /// <summary>
        /// Overwrites the Focused property from the base class.
        /// </summary>
        public override bool Focused
        {
            get
            {
                if (!this.Visible)
                    return false;

                if (controlContainer.Parent is DockContainer)
                {
                    if ((controlContainer.Parent as DockContainer).ActivePanel == controlContainer)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// This property is used by the DockContainer class while docking a form directly to a container,
        /// to prevent the form from being shown for a short time.
        /// </summary>
        [Browsable(false)]
        internal bool ShowFormAtOnLoad
        {
            get { return showFormAtOnLoad; }
            set { showFormAtOnLoad = value; }
        }
        #endregion

        #region Load and container management
        /// <summary>
        /// Calls the <see cref="CreateContainer"/> function, if not in design mode.
        /// Raises the Load event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            if (this.Modal)
            {
                base.OnLoad(e);
                return;
            }

            if (!this.IsDocked & !this.DesignMode)
            {
                if (dockType != DockContainerType.None)
                {
                    this.Opacity = 0;

                    CreateContainer();
                    LoadDockForm();

                    isLoaded = true;
                }
                else
                {
                    this.Opacity = 1;
                }
            }

            base.OnLoad(e);
        }

        private void LoadDockForm()
        {
            DockForm form = new DockForm();
            CopyToDockForm(form);
            if (showFormAtOnLoad)
                form.Show();
        }

        internal void CopyToDockForm(DockForm form)
        {
            form.Location = this.Location;
            form.Size = this.Size;
            form.RootContainer.Controls.Add(controlContainer);
            form.RootContainer.DockType = this.DockType;

            CopyPropToDockForm(form);
        }

        internal void CopyPropToDockForm(DockForm form)
        {
            form.AllowDock = this.allowDock;
            form.FormBorderStyle = this.FormBorderStyle;
            form.Icon = this.Icon;
            form.Text = this.Text;
        }

        /// <summary>
        /// Creates the control container and fills it with the controls contained by the window.
        /// The controls will behave like at design time.
        /// </summary>
        public void CreateContainer()
        {
            if (this.DesignMode)
                return;

            DockManager.RegisterWindow(this);

            controlContainer.Dock = DockStyle.Fill;

            int max = this.Controls.Count;
            int off = 0;
            Control c;

            if (!this.Controls.Contains(controlContainer))
            {
                controlContainer.Dock = DockStyle.None;
                this.Controls.Add(controlContainer);
                controlContainer.Location = Point.Empty;
                controlContainer.Size = this.ClientSize;

                Size diffSize = Size.Subtract(this.Size, this.ClientSize);

                if (!this.MinimumSize.IsEmpty)
                    controlContainer.MinFormSize = Size.Subtract(this.MinimumSize, diffSize);

                if (!this.MaximumSize.IsEmpty)
                    controlContainer.MaxFormSize = Size.Subtract(this.MaximumSize, diffSize);
            }

            while (this.Controls.Count > off)
            {
                if (this.Controls[0] != controlContainer)
                {
                    c = this.Controls[off];
                    this.Controls.Remove(c);

                    if (c != null)
                        controlContainer.Controls.Add(c);
                }
                else
                    off = 1;
            }

            controlContainer.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// Releases the window from its host container.
        /// </summary>
        internal void Release()
        {
            DockContainer host = controlContainer.Parent as DockContainer;

            if (host != null)
                host.ReleaseWindow(this);
        }

        /// <summary>
        /// Raises the <see cref="Control.TextChanged"/> event.
        /// Used also to set the text of the host container.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.HostContainer != null)
                this.HostContainer.SetWindowText();
        }

        /// <summary>
        /// Overrides the OnLayout event handler.
        /// </summary>
        /// <param name="levent">A LayoutEventArgs object.</param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            layoutFinished = true;
            base.OnLayout(levent);
        }

        private void controlContainer_Resize(object sender, EventArgs e)
        {
            this.OnResize(e);
        }

        private void controlContainer_Activated(object sender, EventArgs e)
        {
            this.OnActivated(e);
        }

        private void controlContainer_Deactivate(object sender, EventArgs e)
        {
            this.OnDeactivate(e);
        }
        #endregion

        #region Key mapping
        /// <summary>
        /// Invokes the KeyDown event.
        /// Is used as interface to the key routines of the <see cref="DockManager"/>.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> that contains the event data.</param>
        public void InvokeKeyDown(System.Windows.Forms.KeyEventArgs e)
        {
            OnKeyDown(e);
        }

        /// <summary>
        /// Invokes the KeyUp event.
        /// Is used as interface to the key routines of the <see cref="DockManager"/>.
        /// </summary>
        /// <param name="e">A <see cref="KeyEventArgs"/> that contains the event data.</param>
        public void InvokeKeyUp(System.Windows.Forms.KeyEventArgs e)
        {
            OnKeyUp(e);
        }
        #endregion

        #region Basic Show / Close / Hide window functions wrapper
        /// <summary>
        /// Displays the window to the user.
        /// </summary>
        public new void Show()
        {
            if (showCommandInProcess)
                return;

            showCommandInProcess = true;

            if (!isLoaded)
                this.OnLoad(EventArgs.Empty);

            if (showFormAtOnLoad)
                this.Visible = true;

            showCommandInProcess = false;
        }

        /// <summary>
        /// Conceals the windows from the user.
        /// </summary>
        public new void Hide()
        {
            this.Visible = false;
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        public new void Close()
        {
            if (hideOnClose)
                this.Hide();
            else
            {
                FormClosingEventArgs e = new FormClosingEventArgs(CloseReason.UserClosing, false);
                base.OnFormClosing(e);

                if (!e.Cancel)
                {
                    this.Visible = false;
                    base.OnFormClosed(new FormClosedEventArgs(CloseReason.UserClosing));
                    base.Close();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control and all its parent controls are displayed.
        /// </summary>
        [Description("Determines whether the window is visible or hidden."), Category("Behavior"), DefaultValue(true)]
        public new bool Visible
        {
            get
            {
                // Return base class visibility state for modal execution.
                if (this.Modal)
                    return base.Visible;

                // Evaluate visibility for non-modal execution (docked or not docket but visible).
                bool bVisible = false;

                if (controlContainer.TopLevelControl != null)
                    bVisible = controlContainer.TopLevelControl.Visible;

                return this.IsDocked | bVisible;
            }
            set
            {
                bool visible = this.Visible;

                // Set base class visibility state for modal execution.
                if (this.Modal)
                {
                    base.Visible = value;
                    return;
                }

                // Get host (parent object).
                DockContainer host = controlContainer.Parent as DockContainer;

                // Switch for value.
                if (value)
                {
                    // Was it already loaded?
                    if (!isLoaded)
                    {
                        this.Show();
                        return;
                    }

                    // Was it already docked?
                    if (lastHost != null)
                    {
                        // Check if last host is already disposed. Otherwise dock the window into it and return.
                        if (!lastHost.IsDisposed)
                        {
                            lastHost.DockWindow(this, DockStyle.Fill);
                            return;
                        }

                        // Reset last host.
                        lastHost = null;
                    }

                    // If everythings fine (not in design mode and layout finished) then create the container and load the form (open the window, not docked).
                    if (!this.DesignMode && layoutFinished && !this.Visible)
                    {
                        CreateContainer();
                        LoadDockForm();
                    }
                }
                else
                {
                    // Stop auto hide in the case of being in this mode.
                    if ((host != null) && host.AutoHide)
                        host.StopAutoHide();

                    // Save last docking state and release the window (copy content container back to this object).
                    wasDocked = this.IsDocked;
                    lastHost = (wasDocked) ? host : null;
                    Release();

                    // Hide everything.
                    if (controlContainer.TopLevelControl != null)
                        controlContainer.TopLevelControl.Hide();
                }

                // Visibility changed - fire event.
                if (this.Visible != visible)
                    OnVisibleChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the real visibility state of the window contents.
        /// Use this property instead of Visible, since it can not be overridden.
        /// </summary>
        [Browsable(false), Obsolete("This property is obsolete. Use the Visible property instead.")]
        public bool IsVisible
        {
            get { return this.Visible; }
            set { this.Visible = value; }
        }

        /// <summary>
        /// Gets or sets a flag that prevents the window from closing.
        /// </summary>
        [Category("DockDotNET"), Description("Gets or sets a flag that prevents the window from closing.")]
        public bool HideOnClose
        {
            get { return hideOnClose; }
            set { hideOnClose = value; }
        }

        /// <summary>
        /// Implements the VisibleChanged event of the base class.
        /// Releases the window if showed.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (this.Modal)
                return;

            if (base.Visible && !this.DesignMode && (dockType != DockContainerType.None))
                base.Visible = false;
        }

        /// <summary>
        /// Sets the focus to the <see cref="DockForm"/> or selects the <see cref="DockPanel"/> in its <see cref="DockContainer"/>.
        /// </summary>
        /// <returns></returns>
        public new bool Focus()
        {
            if (this.Modal)
                return base.Focus();

            if (this.IsDocked)
                controlContainer.SelectTab();
            else
                controlContainer.TopLevelControl.Focus();

            return this.Focused;
        }
        #endregion

        #region XML r/w
        /// <summary>
        /// Writes user specific data to the window save list.
        /// </summary>
        /// <param name="writer">The <see cref="XmlTextWriter"/> object that writes to the target stream.</param>
        public virtual void WriteXml(XmlTextWriter writer) { }

        /// <summary>
        /// Reads user specific data from the window save list.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> object that reads from the source stream.</param>
        public virtual void ReadXml(XmlReader reader) { }
        #endregion

        internal void InvokeVisibleChanged(EventArgs eventArgs)
        {
            OnVisibleChanged(eventArgs);
        }
    }
}
