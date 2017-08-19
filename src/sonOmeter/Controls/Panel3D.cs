using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SlimDX.Direct3D9;
using SlimDX;
using System.Threading;
using sonOmeter.Classes;
using System.ComponentModel;

namespace sonOmeter
{
    public class Panel3D : System.Windows.Forms.UserControl
    {
        #region Variables
        private PresentParameters presentParams = null;
        private ModelViewerCamera camera;
        private Direct3D d3D = null;
        private Direct3DEx d3DEx = null;
        private Device device = null;
        private DeviceEx deviceEx = null;
        private bool useD3DEx = false;
        private Thread thread = null;
        private bool pause = false;
        private bool rendering = false;
        private DateTime time = DateTime.Now;

        private Vector3 viewCenter = new Vector3(0, 0, 0);
        private float viewDistance = 1;

        private System.Drawing.Point ptStart = System.Drawing.Point.Empty;

        BackgroundWorker bwRender = null;
        bool bwRenderAgain = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets the Direct3DDevice object of the panel.
        /// </summary>
        private Direct3D D3D
        {
            get { return useD3DEx ? d3DEx : d3D; }
        }

        /// <summary>
        /// Gets the Direct3DDevice object of the panel.
        /// </summary>
        public Device Device
        {
            get { return useD3DEx ? deviceEx : device; }
        }

        /// <summary>
        /// Gets or sets the priority of the polling thread.
        /// Careless use of this feature may cause unwanted effects on other applications!
        /// </summary>
        public ThreadPriority Priority
        {
            get { return thread.Priority; }
            set { thread.Priority = value; }
        }

        /// <summary>
        /// Gets the state of the underlying thread.
        /// </summary>
        public bool Running
        {
            get { return thread.IsAlive; }
        }

        /// <summary>
        /// Gets or sets the viewing center.
        /// </summary>
        public Vector3 ViewCenter
        {
            get { return viewCenter; }
            set { viewCenter = value; SetupMatrices(); }
        }

        /// <summary>
        /// Gets or sets the viewing distance.
        /// </summary>
        public float ViewDistance
        {
            get { return viewDistance; }
            set { viewDistance = value; SetupMatrices(); }
        }
        #endregion

        #region Init
        public bool InitializeGraphics()
        {
            try
            {
                if (presentParams==null) presentParams= new PresentParameters();

                if (camera == null) camera = new ModelViewerCamera();

                Configuration.AddResultWatch(ResultCode.DeviceLost, ResultWatchFlags.AlwaysIgnore);

                camera.SetButtonMasks(MouseButtons.Right, MouseButtons.Middle, MouseButtons.Left);
                camera.IsPositionMovementEnabled = true;

                try
                {
                    d3DEx = new Direct3DEx();
                    useD3DEx = true;
                }
                catch
                {
                    d3D = new Direct3D();
                    useD3DEx = false;
                }

                CreateFlags createFlags = CreateFlags.FpuPreserve;
                Capabilities caps = D3D.GetDeviceCaps(0, DeviceType.Hardware);

                if ((caps.DeviceCaps & DeviceCaps.HWTransformAndLight) == DeviceCaps.HWTransformAndLight)
                    createFlags |= CreateFlags.HardwareVertexProcessing;
                else
                    createFlags |= CreateFlags.SoftwareVertexProcessing;

                // Let's setup our D3D stuff.
                presentParams.Windowed = true;
                presentParams.SwapEffect = SwapEffect.Discard;
                presentParams.EnableAutoDepthStencil = true; // Turn on a Depth stencil
                presentParams.AutoDepthStencilFormat = Format.D16; // And the stencil format
                presentParams.PresentFlags = PresentFlags.DiscardDepthStencil;
                presentParams.DeviceWindowHandle = this.Handle;
                if (useD3DEx)
                    deviceEx = new DeviceEx(d3DEx, 0, DeviceType.Hardware, this.Handle, createFlags, presentParams);
                else
                    device = new Device(d3D, 0, DeviceType.Hardware, this.Handle, createFlags, presentParams);

                this.OnCreateDevice(Device, EventArgs.Empty);
                this.OnResetDevice(Device, EventArgs.Empty);

                pause = false;
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            if (OnDisposeDevice != null)
                OnDisposeDevice(this, new RenderEventArgs(Device, false));

            if (Device != null)
                Device.Dispose();

            if (D3D != null)
                D3D.Dispose();

            base.Dispose(disposing);
        }

        public event RenderEventHandler OnDisposeDevice;

        #region Device creation and reset
        public void OnCreateDevice(object sender, EventArgs e)
        {
        }

        public void OnResetDevice(object sender, EventArgs e)
        {
            if (Device != null)
            {
                Device.SetRenderState<Cull>(RenderState.CullMode, Cull.Counterclockwise);
                Device.SetRenderState(RenderState.Lighting, false);
                Device.SetRenderState(RenderState.ZEnable, true);

                UpdateFillMode();

                // Setup the world, view, and projection matrices
                SetupMatrices();
            }
        }

        public void UpdateFillMode()
        {
            sonOmeter.Classes.Sonar3DDrawMode mode = (MouseButtons == MouseButtons.None) ? GSC.Settings.DrawMode3D : GSC.Settings.DragDrawMode3D;
            Device.SetRenderState<FillMode>(RenderState.FillMode, (mode == sonOmeter.Classes.Sonar3DDrawMode.Wireframe) ? FillMode.Wireframe : FillMode.Solid);
        }
        #endregion

        #region Rendering and animation
        public void Update3D()
        {
            if (!this.Parent.Visible)
                return;

            if (bwRender == null)
            {
                bwRender = new BackgroundWorker();
                bwRender.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OnRenderCompleted);
                bwRender.DoWork += new DoWorkEventHandler(Render);
            }
            if (!bwRender.IsBusy)
            {
                bwRender.RunWorkerAsync();
            }
            else
            {
                bwRenderAgain = true;
            }
        }

        void OnRenderCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IsDeviceLost(true);

            try
            {
                if (bwRenderAgain)
                {
                    bwRenderAgain = false;
                    Update3D();
                }
            }
            catch { }
        }

        public event RenderEventHandler OnRender;

        void Render(object sender, DoWorkEventArgs e)
        {
            try
            {
                if (Device == null)
                    return;

                if (!this.Parent.Visible)
                    return;

                if (pause || rendering)
                    return;

                rendering = true;

                try
                {
                    if (!IsDeviceLost(false))
                    {
                        FrameMove();

                        // Clear the backbuffer to a blue color
                        Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, GSC.Settings.CS.BackColor, 1.0f, 0);

                        // Begin the scene
                        Device.BeginScene();

                        // Draw the scene
                        RenderEventArgs args = new RenderEventArgs(Device, GSC.Settings.DrawMode3D == sonOmeter.Classes.Sonar3DDrawMode.BoundingBox);
                        if (OnRender != null)
                            OnRender(this, args);

                        // End the scene
                        Device.EndScene();

                        // Finally, draw if everything is fine.
                        if (!args.Cancel)
                            Device.Present();
                    }
                }
                catch (Exception ex)
                {
                    this.Dispose();
                    this.InitializeGraphics();
                }

                rendering = false;
            }
            catch { }
        }

        private bool IsDeviceLost(bool performReset)
        {
            try
            {
                Result result = Device.TestCooperativeLevel();

                if (result == ResultCode.DeviceLost)
                {
                    System.Diagnostics.Debug.WriteLine("Device lost.");
                    return true;
                }
                else if (result == ResultCode.DriverInternalError)
                {
                    MessageBox.Show("Internal 3D Driver Error!");
                    return true;
                }
                else if (result == ResultCode.DeviceNotReset)
                {
                    System.Diagnostics.Debug.WriteLine("Device ready for reset.");

                    if (performReset)
                    {
                        Device.Reset(presentParams);
                        OnResetDevice(this, EventArgs.Empty);
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return true;
            }
        }

        private void FrameMove()
        {
            // Update time and camera
            DateTime newTime = DateTime.Now;
            float elapsedTime = (float)(newTime - time).Ticks / (float)TimeSpan.TicksPerSecond;
            time = newTime;

            camera.FrameMove(elapsedTime);

            // Build the world matrix
            Matrix worldMatrix = Matrix.Translation(-viewCenter);
            worldMatrix *= camera.WorldMatrix;

            // Set transformation matrices
            Device.SetTransform(TransformState.World, worldMatrix);
            Device.SetTransform(TransformState.Projection, camera.ProjectionMatrix);
            Device.SetTransform(TransformState.View, camera.ViewMatrix);
        }

        private void SetupMatrices()
        {
            // Setup the camera's projection parameters
            float aspectRatio = (float)this.Width / (float)this.Height;
            camera.SetProjectionParameters((float)Math.PI / 4, aspectRatio, viewDistance / 64.0f, viewDistance * 200.0f);
            camera.SetViewParameters(new Vector3(0, 0, -2.2f * viewDistance), Vector3.Zero);
            camera.SetWindow(this.Width, this.Height);
            camera.SetRadius(viewDistance * 2.2f, viewDistance * 0.5f, viewDistance * 10.0f);
            camera.SetVelocityScaler(5.0F / (viewDistance * 2.2f), true);

            // Build the world matrix
            Matrix worldMatrix = Matrix.Translation(-viewCenter);
            worldMatrix *= camera.WorldMatrix;

            // Set transformation matrices - avoid failure by concurrent render processes
            while (rendering) Application.DoEvents();
            pause = true;

            Device.SetTransform(TransformState.World, worldMatrix);
            Device.SetTransform(TransformState.Projection, camera.ProjectionMatrix);
            Device.SetTransform(TransformState.View, camera.ViewMatrix);

            pause = false;
        }
        #endregion

        #region Overriden functions
        protected override void OnPaint(PaintEventArgs e)
        {
            Update3D();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            OnResetDevice(this, EventArgs.Empty);
        }
        #endregion

        #region Mouse and keyboard interaction
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            camera.OnMouseWheel(e);
            Update3D();
        }

        public void InvokeOnMouseWheel(MouseEventArgs e)
        {
            OnMouseWheel(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            camera.OnMouseMove(e);
            Update3D();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            camera.OnMouseDown(e);
            Update3D();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            camera.OnMouseDoubleClick(e);
            Update3D();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            camera.OnMouseUp(e);
            Update3D();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            camera.OnKeyDown(e);
            Update3D();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }
        #endregion
    }

    public class RenderEventArgs : EventArgs
    {
        bool boundingBox = false;
        Device dev = null;
        bool cancel = false;

        #region Properties
        public Device Device
        {
            get { return dev; }
        }

        public bool BoundingBox
        {
            get { return boundingBox; }
        }

        public bool Cancel
        {
            get { return cancel; }
            set { cancel = value; }
        }
        #endregion

        public RenderEventArgs(Device dev, bool boundingBox)
        {
            this.dev = dev;
            this.boundingBox = boundingBox;
            this.cancel = false;
        }
    }

    public delegate void RenderEventHandler(object sender, RenderEventArgs e);
}
