using System;
using System.Collections.Generic;
using System.Text;
using SlimDX;
using System.Windows.Forms;

namespace sonOmeter.Classes
{
    /// <summary>
    /// Used to map keys to the camera
    /// </summary>
    public enum CameraKeys : byte
    {
        StrafeLeft,
        StrafeRight,
        MoveForward,
        MoveBackward,
        MoveUp,
        MoveDown,
        Reset,
        ControlDown,
        MaxKeys,
        Unknown = 0xff
    }

    /// <summary>
    /// Class holds arcball data
    /// </summary>
    public class ArcBall
    {
        #region Instance Data
        protected Matrix rotation; // Matrix for arc ball's orientation
        protected Matrix translation; // Matrix for arc ball's position
        protected Matrix translationDelta; // Matrix for arc ball's position

        protected int width; // arc ball's window width
        protected int height; // arc ball's window height
        protected Vector2 center;  // center of arc ball 
        protected float radius; // arc ball's radius in screen coords
        protected float radiusTranslation; // arc ball's radius for translating the target

        protected Quaternion downQuat; // Quaternion before button down
        protected Quaternion nowQuat; // Composite quaternion for current drag
        protected bool isDragging; // Whether user is dragging arc ball

        protected System.Drawing.Point lastMousePosition; // position of last mouse point
        protected Vector3 downPt; // starting point of rotation arc
        protected Vector3 currentPt; // current point of rotation arc
        #endregion

        #region Simple Properties
        /// <summary>Gets the rotation matrix</summary>
        public Matrix RotationMatrix { get { return rotation = Matrix.RotationQuaternion(nowQuat); } }
        /// <summary>Gets the translation matrix</summary>
        public Matrix TranslationMatrix { get { return translation; } }
        /// <summary>Gets the translation delta matrix</summary>
        public Matrix TranslationDeltaMatrix { get { return translationDelta; } }
        /// <summary>Gets the dragging state</summary>
        public bool IsBeingDragged { get { return isDragging; } }
        /// <summary>Gets or sets the current quaternion</summary>
        public Quaternion CurrentQuaternion { get { return nowQuat; } set { nowQuat = value; } }
        #endregion

        #region Class methods
        /// <summary>
        /// Create new instance of the arcball class
        /// </summary>
        public ArcBall(int w, int h)
        {
            Reset();
            downPt = Vector3.Zero;
            currentPt = Vector3.Zero;

            SetWindow(w, h);
        }

        /// <summary>
        /// Resets the arcball
        /// </summary>
        public void Reset()
        {
            downQuat = Quaternion.Identity;
            nowQuat = Quaternion.Identity;
            rotation = Matrix.Identity;
            translation = Matrix.Identity;
            translationDelta = Matrix.Identity;
            isDragging = false;
            radius = 1.0f;
            radiusTranslation = 1.0f;
        }

        /// <summary>
        /// Convert a screen point to a vector
        /// </summary>
        public Vector3 ScreenToVector(float screenPointX, float screenPointY)
        {
            float x = -(screenPointX - width / 2.0f) / (radius * width / 2.0f);
            float y = (screenPointY - height / 2.0f) / (radius * height / 2.0f);
            float z = 0.0f;
            float mag = (x * x) + (y * y);

            if (mag > 1.0f)
            {
                float scale = 1.0f / (float)Math.Sqrt(mag);
                x *= scale;
                y *= scale;
            }
            else
                z = (float)Math.Sqrt(1.0f - mag);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Set window paramters
        /// </summary>
        public void SetWindow(int w, int h, float r)
        {
            width = w; height = h; radius = r;
            center = new Vector2(w / 2.0f, h / 2.0f);
        }
        public void SetWindow(int w, int h)
        {
            SetWindow(w, h, 0.9f); // default radius
        }

        /// <summary>
        /// Computes a quaternion from ball points
        /// </summary>
        public static Quaternion QuaternionFromBallPoints(Vector3 from, Vector3 to)
        {
            float dot = Vector3.Dot(from, to);
            Vector3 part = Vector3.Cross(from, to);
            return new Quaternion(part.X, part.Y, part.Z, dot);
        }

        /// <summary>
        /// Begin the arcball 'dragging'
        /// </summary>
        public void OnBegin(int x, int y)
        {
            isDragging = true;
            downQuat = nowQuat;
            downPt = ScreenToVector((float)x, (float)y);
        }
        /// <summary>
        /// The arcball is 'moving'
        /// </summary>
        public void OnMove(int x, int y)
        {
            if (isDragging)
            {
                currentPt = ScreenToVector((float)x, (float)y);
                nowQuat = downQuat * QuaternionFromBallPoints(currentPt, downPt);
            }
        }
        /// <summary>
        /// Done dragging the arcball
        /// </summary>
        public void OnEnd()
        {
            isDragging = false;
        } 
        #endregion

        #region Mouse
        public void OnMouseDown(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    // Set capture
                    // tbd
                    OnBegin(e.X, e.Y);
                    break;

                case MouseButtons.Middle:
                case MouseButtons.Right:
                    // Set capture
                    // tbd
                    // Store off the position of the cursor
                    lastMousePosition = e.Location;
                    break;
            }
        }

        public void OnMouseDoubleClick(MouseEventArgs e)
        {
            OnMouseDown(e);
        }

        public void OnMouseUp(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    // Release capture
                    // tbd
                    OnEnd();
                    break;

                case MouseButtons.Middle:
                case MouseButtons.Right:
                    // Release capture
                    // tbd
                    break;
            }
        }

        public void OnMouseMove(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    OnMove(e.X, e.Y);
                    break;

                case MouseButtons.Right:
                case MouseButtons.Middle:
                    // Normalize based on size of window and bounding sphere radius
                    float deltaX = (lastMousePosition.X - e.X) * radiusTranslation / width;
                    float deltaY = (lastMousePosition.Y - e.Y) * radiusTranslation / height;

                    if (e.Button == MouseButtons.Right)
                    {
                        translationDelta = Matrix.Translation(-2 * deltaX, 2 * deltaY, 0.0f);
                        translation *= translationDelta;
                    }
                    else // Middle button
                    {
                        translationDelta = Matrix.Translation(0.0f, 0.0f, 5 * deltaY);
                        translation *= translationDelta;
                    }
                    // Store off the position of the cursor
                    lastMousePosition = e.Location;
                    break;
            }
        } 
        #endregion
    }

    /// <summary>
    /// Simple base camera class that moves and rotates.  The base class
    /// records mouse and keyboard input for use by a derived class, and 
    /// keeps common state.
    /// </summary>
    public abstract class D3DCamera
    {
        #region Static key mapper
        /// <summary>
        /// Maps NativeMethods.WindowMessage.Key* msg to a camera key
        /// </summary>
        protected static CameraKeys MapKey(Keys key)
        {
            switch (key)
            {
                case System.Windows.Forms.Keys.ControlKey: return CameraKeys.ControlDown;
                case System.Windows.Forms.Keys.Left: return CameraKeys.StrafeLeft;
                case System.Windows.Forms.Keys.Right: return CameraKeys.StrafeRight;
                case System.Windows.Forms.Keys.Up: return CameraKeys.MoveForward;
                case System.Windows.Forms.Keys.Down: return CameraKeys.MoveBackward;
                case System.Windows.Forms.Keys.Prior: return CameraKeys.MoveUp; // pgup
                case System.Windows.Forms.Keys.Next: return CameraKeys.MoveDown; // pgdn

                case System.Windows.Forms.Keys.A: return CameraKeys.StrafeLeft;
                case System.Windows.Forms.Keys.D: return CameraKeys.StrafeRight;
                case System.Windows.Forms.Keys.W: return CameraKeys.MoveForward;
                case System.Windows.Forms.Keys.S: return CameraKeys.MoveBackward;
                case System.Windows.Forms.Keys.Q: return CameraKeys.MoveUp;
                case System.Windows.Forms.Keys.E: return CameraKeys.MoveDown;

                case System.Windows.Forms.Keys.NumPad4: return CameraKeys.StrafeLeft;
                case System.Windows.Forms.Keys.NumPad6: return CameraKeys.StrafeRight;
                case System.Windows.Forms.Keys.NumPad8: return CameraKeys.MoveForward;
                case System.Windows.Forms.Keys.NumPad2: return CameraKeys.MoveBackward;
                case System.Windows.Forms.Keys.NumPad9: return CameraKeys.MoveUp;
                case System.Windows.Forms.Keys.NumPad3: return CameraKeys.MoveDown;

                case System.Windows.Forms.Keys.Back: return CameraKeys.Reset;
            }

            // No idea
            return (CameraKeys)byte.MaxValue;
        }
        #endregion

        #region Instance Data
        protected Matrix viewMatrix; // View Matrix
        protected Matrix projMatrix; // Projection matrix

        protected System.Drawing.Point lastMousePosition;  // Last absolute position of mouse cursor
        protected MouseButtons currentButtonMask = MouseButtons.None;   // mask of which buttons are down
        protected int mouseWheelDelta;     // Amount of middle wheel scroll (+/-) 
        protected Vector2 mouseDelta;          // Mouse relative delta smoothed over a few frames
        protected float framesToSmoothMouseData; // Number of frames to smooth mouse data over

        protected Vector3 defaultEye;          // Default camera eye position
        protected Vector3 defaultLookAt;       // Default LookAt position
        protected Vector3 eye;                 // Camera eye position
        protected Vector3 lookAt;              // LookAt position
        protected float cameraYawAngle;      // Yaw angle of camera
        protected float cameraPitchAngle;    // Pitch angle of camera

        protected System.Drawing.Rectangle dragRectangle; // Rectangle within which a drag can be initiated.
        protected Vector3 velocity;            // Velocity of camera
        protected bool isMovementDrag;        // If true, then camera movement will slow to a stop otherwise movement is instant
        protected Vector3 velocityDrag;        // Velocity drag force
        protected float dragTimer;           // Countdown timer to apply drag
        protected float totalDragTimeToZero; // Time it takes for velocity to go from full to 0
        protected Vector2 rotationVelocity;         // Velocity of camera

        protected float fieldOfView;                 // Field of view
        protected float aspectRatio;              // Aspect ratio
        protected float nearPlane;           // Near plane
        protected float farPlane;            // Far plane

        protected float rotationScaler;      // Scaler for rotation
        protected float moveScaler;          // Scaler for movement

        protected bool isInvertPitch;         // Invert the pitch axis
        protected bool isEnablePositionMovement; // If true, then the user can translate the camera/model 
        protected bool isEnableYAxisMovement; // If true, then camera can move in the y-axis

        protected bool isClipToBoundary;      // If true, then the camera will be clipped to the boundary
        protected Vector3 minBoundary;         // Min point in clip boundary
        protected Vector3 maxBoundary;         // Max point in clip boundary

        protected bool isResetCursorAfterMove;// If true, the class will reset the cursor position so that the cursor always has space to move 

        // State of the input
        protected bool[] keys;
        public static readonly Vector3 UpDirection = new Vector3(0, 1, 0);
        #endregion

        #region Properties
        /// <summary>Is the camera being 'dragged' at all?</summary>
        public bool IsBeingDragged { get { return (currentButtonMask != MouseButtons.None); } }
        /// <summary>Is the left mouse button down</summary>
        public bool IsMouseLeftButtonDown { get { return ((currentButtonMask & MouseButtons.Left) != MouseButtons.None); } }
        /// <summary>Is the right mouse button down</summary>
        public bool IsMouseRightButtonDown { get { return ((currentButtonMask & MouseButtons.Right) != MouseButtons.None); } }
        /// <summary>Is the middle mouse button down</summary>
        public bool IsMouseMiddleButtonDown { get { return ((currentButtonMask & MouseButtons.Middle) != MouseButtons.None); } }
        /// <summary>Returns the view transformation matrix</summary>
        public Matrix ViewMatrix { get { return viewMatrix; } }
        /// <summary>Returns the projection transformation matrix</summary>
        public Matrix ProjectionMatrix { get { return projMatrix; } }
        /// <summary>Returns the location of the eye</summary>
        public Vector3 EyeLocation { get { return eye; } }
        /// <summary>Returns the look at point of the camera</summary>
        public Vector3 LookAtPoint { get { return lookAt; } }
        /// <summary>Is position movement enabled</summary>
        public bool IsPositionMovementEnabled { get { return isEnablePositionMovement; } set { isEnablePositionMovement = value; } }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for the base camera class (Sets up camera defaults)
        /// </summary>
        protected D3DCamera()
        {
            // Create the keys
            keys = new bool[(int)CameraKeys.MaxKeys];

            // Set attributes for the view matrix
            eye = Vector3.Zero;
            lookAt = new Vector3(0.0f, 0.0f, 1.0f);

            // Setup the view matrix
            SetViewParameters(eye, lookAt);

            // Setup the projection matrix
            SetProjectionParameters((float)Math.PI / 4, 1.0f, 1.0f, 1000.0f);

            // Store mouse information
            lastMousePosition = System.Windows.Forms.Cursor.Position;
            currentButtonMask = MouseButtons.None;
            mouseWheelDelta = 0;

            // Setup camera rotations
            cameraYawAngle = 0.0f;
            cameraPitchAngle = 0.0f;

            dragRectangle = new System.Drawing.Rectangle(0, 0, int.MaxValue, int.MaxValue);
            velocity = Vector3.Zero;
            isMovementDrag = false;
            velocityDrag = Vector3.Zero;
            dragTimer = 0.0f;
            totalDragTimeToZero = 0.25f;
            rotationVelocity = Vector2.Zero;
            rotationScaler = 0.1f;
            moveScaler = 5.0f;
            isInvertPitch = false;
            isEnableYAxisMovement = true;
            isEnablePositionMovement = true;
            mouseDelta = Vector2.Zero;
            framesToSmoothMouseData = 2.0f;
            isClipToBoundary = false;
            minBoundary = new Vector3(-1.0f, -1.0f, -1.0f);
            maxBoundary = new Vector3(1, 1, 1);
            isResetCursorAfterMove = false;
        } 
        #endregion

        #region Keyboard
        public virtual void OnKeyDown(KeyEventArgs e)
        {
            CameraKeys mappedKey = MapKey(e.KeyData);
            if (mappedKey != CameraKeys.Unknown)
            {
                // Valid key was pressed, mark it as 'down'
                keys[(int)mappedKey] = true;
            }
        }

        public virtual void OnKeyUp(KeyEventArgs e)
        {
            CameraKeys mappedKey = MapKey(e.KeyData);
            if (mappedKey != CameraKeys.Unknown)
            {
                // Valid key was released, mark it as 'up'
                keys[(int)mappedKey] = false;
            }
        } 
        #endregion

        #region Mouse
        public virtual void OnMouseDown(MouseEventArgs e)
        {
            // Update the variable state
            if (dragRectangle.Contains(e.Location))
                currentButtonMask |= e.Button;

            // Capture the mouse, so if the mouse button is 
            // released outside the window, we'll get the button up messages
            // tbd

            lastMousePosition = System.Windows.Forms.Cursor.Position;
        }

        public virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            // Update the variable state
            if (dragRectangle.Contains(e.Location))
                currentButtonMask |= e.Button;

            // Capture the mouse, so if the mouse button is 
            // released outside the window, we'll get the button up messages
            // tbd

            lastMousePosition = System.Windows.Forms.Cursor.Position;
        }

        public virtual void OnMouseUp(MouseEventArgs e)
        {
            // Update the variable state
            currentButtonMask &= ~e.Button;

            if (currentButtonMask == MouseButtons.None)
            {
                // Release capture
                // tbd
            }
        }

        public virtual void OnMouseWheel(MouseEventArgs e)
        {
            mouseWheelDelta = e.Delta / 120;
        }

        public virtual void OnMouseMove(MouseEventArgs e)
        {
            // do nothing :-)
        }
        #endregion

        #region Set & Reset
        /// <summary>
        /// Reset the camera's position back to the default
        /// </summary>
        public virtual void Reset()
        {
            SetViewParameters(defaultEye, defaultLookAt);
        }

        /// <summary>
        /// Client can call this to change the position and direction of camera
        /// </summary>
        public virtual void SetViewParameters(Vector3 eyePt, Vector3 lookAtPt)
        {
            // Store the data
            defaultEye = eye = eyePt;
            defaultLookAt = lookAt = lookAtPt;

            // Calculate the view matrix
            viewMatrix = Matrix.LookAtRH(eye, lookAt, UpDirection);

            // Get the inverted matrix
            Matrix inverseView = Matrix.Invert(viewMatrix);

            // The axis basis vectors and camera position are stored inside the 
            // position matrix in the 4 rows of the camera's world matrix.
            // To figure out the yaw/pitch of the camera, we just need the Z basis vector
            Vector3 zBasis = new Vector3(viewMatrix.M31, viewMatrix.M32, viewMatrix.M33);
            cameraYawAngle = (float)Math.Atan2(zBasis.X, zBasis.Z);
            float len = (float)Math.Sqrt(zBasis.Z * zBasis.Z + zBasis.X * zBasis.X);
            cameraPitchAngle = -(float)Math.Atan2(zBasis.Y, len);
        }

        /// <summary>
        /// Calculates the projection matrix based on input params
        /// </summary>
        public virtual void SetProjectionParameters(float fov, float aspect, float near, float far)
        {
            // Set attributes for the projection matrix
            fieldOfView = fov;
            aspectRatio = aspect;
            nearPlane = near;
            farPlane = far;

            projMatrix = Matrix.PerspectiveFovRH(fov, aspect, near, far);
        } 
        #endregion

        #region Update
        /// <summary>
        /// Figure out the mouse delta based on mouse movement
        /// </summary>
        protected void UpdateMouseDelta(float elapsedTime)
        {
            // Get the current mouse position
            System.Drawing.Point current = System.Windows.Forms.Cursor.Position;

            // Calculate how far it's moved since the last frame
            System.Drawing.Point delta = new System.Drawing.Point(current.X - lastMousePosition.X,
                current.Y - lastMousePosition.Y);

            // Record the current position for next time
            lastMousePosition = current;

            if (isResetCursorAfterMove)
            {
                // Set position of camera to center of desktop, 
                // so it always has room to move.  This is very useful
                // if the cursor is hidden.  If this isn't done and cursor is hidden, 
                // then invisible cursor will hit the edge of the screen 
                // and the user can't tell what happened
                System.Windows.Forms.Screen activeScreen = System.Windows.Forms.Screen.PrimaryScreen;
                System.Drawing.Point center = new System.Drawing.Point(activeScreen.Bounds.Width / 2,
                    activeScreen.Bounds.Height / 2);
                System.Windows.Forms.Cursor.Position = center;
                lastMousePosition = center;
            }

            // Smooth the relative mouse data over a few frames so it isn't 
            // jerky when moving slowly at low frame rates.
            float percentOfNew = 1.0f / framesToSmoothMouseData;
            float percentOfOld = 1.0f - percentOfNew;
            mouseDelta.X = mouseDelta.X * percentOfNew + delta.X * percentOfNew;
            mouseDelta.Y = mouseDelta.Y * percentOfNew + delta.Y * percentOfNew;

            rotationVelocity = mouseDelta * rotationScaler;
        }

        /// <summary>
        /// Figure out the velocity based on keyboard input & drag if any
        /// </summary>
        protected void UpdateVelocity(float elapsedTime, float scaler)
        {
            Vector3 accel = Vector3.Zero;

            if (isEnablePositionMovement)
            {
                // Update acceleration vector based on keyboard state
                if (keys[(int)CameraKeys.MoveForward])
                {
                    accel.Y -= 1.0f;
                    keys[(int)CameraKeys.MoveForward] = false;
                }
                if (keys[(int)CameraKeys.MoveBackward])
                {
                    accel.Y += 1.0f;
                    keys[(int)CameraKeys.MoveBackward] = false;
                }
                if (isEnableYAxisMovement)
                {
                    if (keys[(int)CameraKeys.MoveUp])
                    {
                        accel.Z += 1.0f;
                        keys[(int)CameraKeys.MoveUp] = false;
                    }
                    if (keys[(int)CameraKeys.MoveDown])
                    {
                        accel.Z -= 1.0f;
                        keys[(int)CameraKeys.MoveDown] = false;
                    }
                }
                if (keys[(int)CameraKeys.StrafeRight])
                {
                    accel.X += 1.0f;
                    keys[(int)CameraKeys.StrafeRight] = false;
                }
                if (keys[(int)CameraKeys.StrafeLeft])
                {
                    accel.X -= 1.0f;
                    keys[(int)CameraKeys.StrafeLeft] = false;
                }
            }
            // Normalize vector so if moving 2 dirs (left & forward), 
            // the camera doesn't move faster than if moving in 1 dir
            accel.Normalize();
            // Scale the acceleration vector
            accel *= scaler;

            if (isMovementDrag)
            {
                // Is there any acceleration this frame?
                if (accel.LengthSquared() > 0)
                {
                    // If so, then this means the user has pressed a movement key
                    // so change the velocity immediately to acceleration 
                    // upon keyboard input.  This isn't normal physics
                    // but it will give a quick response to keyboard input
                    velocity = accel;
                    dragTimer = totalDragTimeToZero;
                    velocityDrag = accel * (1 / dragTimer);
                }
                else
                {
                    // If no key being pressed, then slowly decrease velocity to 0
                    if (dragTimer > 0)
                    {
                        velocity -= (velocityDrag * elapsedTime);
                        dragTimer -= elapsedTime;
                    }
                    else
                    {
                        // Zero velocity
                        velocity = Vector3.Zero;
                    }
                }
            }
            else
            {
                // No drag, so immediately change the velocity
                velocity = accel;
            }
        } 
        #endregion

        #region Misc
        /// <summary>
        /// Clamps V to lie inside boundaries
        /// </summary>
        protected void ConstrainToBoundary(ref Vector3 v)
        {
            // Constrain vector to a bounding box 
            v.X = Math.Max(v.X, minBoundary.X);
            v.Y = Math.Max(v.Y, minBoundary.Y);
            v.Z = Math.Max(v.Z, minBoundary.Z);

            v.X = Math.Min(v.X, maxBoundary.X);
            v.Y = Math.Min(v.Y, maxBoundary.Y);
            v.Z = Math.Min(v.Z, maxBoundary.Z);
        }

        /// <summary>
        /// Abstract method to control camera during frame move
        /// </summary>
        public abstract void FrameMove(float elapsedTime); 
        #endregion
    }

    /// <summary>
    /// Simple model viewing camera class that rotates around the object.
    /// </summary>
    public class ModelViewerCamera : D3DCamera
    {
        #region Instance Data
        protected ArcBall worldArcball = new ArcBall(10, 10);
        protected ArcBall viewArcball = new ArcBall(10, 10);
        protected Vector3 modelCenter;
        protected Matrix lastModelRotation; // Last arcball rotation matrix for model 
        protected Matrix lastCameraRotation; // Last rotation matrix for camera
        protected Matrix modelRotation; // Rotation matrix of model
        protected Matrix world; // World Matrix of model

        protected MouseButtons rotateModelButtonMask;
        protected MouseButtons zoomButtonMask;
        protected MouseButtons rotateCameraButtonMask;

        protected bool isPitchLimited;
        protected float radius; // Distance from the camera to model 
        protected float defaultRadius; // Distance from the camera to model 
        protected float minRadius; // Min radius
        protected float maxRadius; // Max radius
        protected bool attachCameraToModel;
        protected bool adaptiveVelocity;
        #endregion

        #region Properties
        /// <summary>The minimum radius</summary>
        public float MinimumRadius { get { return minRadius; } set { minRadius = value; } }
        /// <summary>The maximum radius</summary>
        public float MaximumRadius { get { return maxRadius; } set { maxRadius = value; } }
        /// <summary>Gets the world matrix</summary>
        public Matrix WorldMatrix { get { return world; } }
        /// <summary>Sets the world quat</summary>
        public void SetWorldQuat(Quaternion q) { worldArcball.CurrentQuaternion = q; }
        /// <summary>Sets the view quat</summary>
        public void SetViewQuat(Quaternion q) { viewArcball.CurrentQuaternion = q; }
        /// <summary>Sets whether the pitch is limited or not</summary>
        public void SetIsPitchLimited(bool limit) { isPitchLimited = limit; }
        /// <summary>Sets the model's center</summary>
        public void SetModelCenter(Vector3 c) { modelCenter = c; }
        /// <summary>Sets radius</summary>
        public void SetRadius(float r, float min, float max) { radius = defaultRadius = r; minRadius = min; maxRadius = max; }
        /// <summary>Sets radius</summary>
        public void SetRadius(float r) { defaultRadius = r; minRadius = 1.0f; maxRadius = float.MaxValue; }
        /// <summary>Sets arcball window</summary>
        public void SetWindow(int w, int h, float r) { worldArcball.SetWindow(w, h, r); viewArcball.SetWindow(w, h, r); }
        /// <summary>Sets arcball window</summary>
        public void SetWindow(int w, int h) { worldArcball.SetWindow(w, h, 0.9f); viewArcball.SetWindow(w, h, 0.9f); }
        /// <summary>Sets button masks</summary>
        public void SetButtonMasks(MouseButtons rotateModel, MouseButtons zoom, MouseButtons rotateCamera) { rotateCameraButtonMask = rotateCamera; zoomButtonMask = zoom; rotateModelButtonMask = rotateModel; }
        /// <summary>Is the camera attached to a model</summary>
        public bool IsAttachedToModel { get { return attachCameraToModel; } set { attachCameraToModel = value; } }
        /// <summary>Is the velocity adaptively adjusted based on the view radius.</summary>
        public bool IsAdaptiveVelocity { get { return adaptiveVelocity; } set { adaptiveVelocity = value; } }
        /// <summary> 
        /// Sets the velocity parameters.
        /// </summary>
        /// <param name="scaler">The velocity scaler.</param>
        /// <param name="adaptive">Toggles adaptive velocity adjustment.</param>
        public void SetVelocityScaler(float scaler, bool adaptive) { moveScaler = scaler; adaptiveVelocity = adaptive; }
        #endregion

        #region Constructor
        /// <summary>
        /// Creates new instance of the model viewer camera
        /// </summary>
        public ModelViewerCamera()
        {
            world = Matrix.Identity;
            modelRotation = Matrix.Identity;
            lastModelRotation = Matrix.Identity;
            lastCameraRotation = Matrix.Identity;
            modelCenter = Vector3.Zero;
            radius = 5.0f;
            defaultRadius = 5.0f;
            minRadius = 1.0f;
            maxRadius = float.MaxValue;
            isPitchLimited = false;
            isEnablePositionMovement = false;
            attachCameraToModel = false;
            adaptiveVelocity = false;

            // Set button masks
            rotateModelButtonMask = MouseButtons.Left;
            zoomButtonMask = MouseButtons.Middle;
            rotateCameraButtonMask = MouseButtons.Right;
        } 
        #endregion

        #region FrameMove
        /// <summary>
        /// Update the view matrix based on user input & elapsed time
        /// </summary>
        public override void FrameMove(float elapsedTime)
        {
            // Reset the camera if necessary
            if (keys[(int)CameraKeys.Reset])
            {
                Reset();
                keys[(int)CameraKeys.Reset] = false;
            }

            // Get the mouse movement (if any) if the mouse buttons are down
            if (currentButtonMask != MouseButtons.None)
                UpdateMouseDelta(elapsedTime);

            // Change the radius from the camera to the model based on wheel scrolling
            if ((mouseWheelDelta != 0) && (zoomButtonMask == MouseButtons.Middle))
                radius -= mouseWheelDelta * radius * 0.1f;
            radius = Math.Min(maxRadius, radius);
            radius = Math.Max(minRadius, radius);
            mouseWheelDelta = 0;

            // Get amount of velocity based on the keyboard input and drag (if any)
            UpdateVelocity(elapsedTime, moveScaler * (adaptiveVelocity ? radius : 1.0F));

            // Simple euler method to calculate position delta
            //Vector3 posDelta = velocity * elapsedTime;
            Vector3 posDelta = Vector3.Zero;

            // Get the inverse of the arcball's rotation matrix
            Matrix cameraRotation = Matrix.Invert(viewArcball.RotationMatrix);

            // Transform vectors based on camera's rotation matrix
            Vector3 localUp = new Vector3(0, 1, 0);
            Vector3 localAhead = new Vector3(0, 0, 1);
            Vector3 worldUp = Vector3.TransformCoordinate(localUp, cameraRotation);
            Vector3 worldAhead = Vector3.TransformCoordinate(localAhead, cameraRotation);

            // Transform the position delta by the camera's rotation 
            Vector3 posDeltaWorld = Vector3.TransformCoordinate(posDelta, cameraRotation);

            // Move the lookAt position 
            lookAt += posDeltaWorld;
            if (isClipToBoundary)
                ConstrainToBoundary(ref lookAt);

            // Update the eye point based on a radius away from the lookAt position
            eye = lookAt - worldAhead * radius;

            // Update the view matrix
            viewMatrix = Matrix.LookAtRH(eye, lookAt, worldUp);
            Matrix invView = Matrix.Invert(viewMatrix);
            invView.M41 = invView.M42 = invView.M43 = 0;
            Matrix modelLastRotInv = Matrix.Invert(lastModelRotation);

            // Accumulate the delta of the arcball's rotation in view space.
            // Note that per-frame delta rotations could be problematic over long periods of time.
            Matrix localModel = worldArcball.RotationMatrix;
            modelRotation *= viewMatrix * modelLastRotInv * localModel * invView;
            if (viewArcball.IsBeingDragged && attachCameraToModel && !keys[(int)CameraKeys.ControlDown])
            {
                // Attach camera to model by inverse of the model rotation
                Matrix cameraRotInv = Matrix.Invert(lastCameraRotation);
                Matrix delta = cameraRotInv * cameraRotation; // local to world matrix
                modelRotation *= delta;
            }
            lastCameraRotation = cameraRotation;
            lastModelRotation = localModel;

            // Since we're accumulating delta rotations, we need to orthonormalize 
            // the matrix to prevent eventual matrix skew
            Vector3 xBasis = new Vector3(modelRotation.M11, modelRotation.M12, modelRotation.M13);
            Vector3 yBasis = new Vector3(modelRotation.M21, modelRotation.M22, modelRotation.M23);
            Vector3 zBasis = new Vector3(modelRotation.M31, modelRotation.M32, modelRotation.M33);

            xBasis = Vector3.Normalize(xBasis);
            yBasis = Vector3.Cross(zBasis, xBasis);
            yBasis = Vector3.Normalize(yBasis);
            zBasis = Vector3.Cross(xBasis, yBasis);

            modelRotation.M11 = xBasis.X; modelRotation.M12 = xBasis.Y; modelRotation.M13 = xBasis.Z;
            modelRotation.M21 = yBasis.X; modelRotation.M22 = yBasis.Y; modelRotation.M23 = yBasis.Z;
            modelRotation.M31 = zBasis.X; modelRotation.M32 = zBasis.Y; modelRotation.M33 = zBasis.Z;

            // Translate the rotation matrix to the same position as the lookAt position
            modelRotation.M41 = lookAt.X;
            modelRotation.M42 = lookAt.Y;
            modelRotation.M43 = lookAt.Z;

            // Translate world matrix so its at the center of the model
            velocity = Vector3.TransformCoordinate(velocity, cameraRotation);
            modelCenter -= velocity;
            Matrix trans = Matrix.Translation(-modelCenter.X, -modelCenter.Y, -modelCenter.Z);
            world = modelRotation * trans;
        } 
        #endregion

        #region Set / Reset
        /// <summary>
        /// Reset the camera's position back to the default
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            world = Matrix.Identity;
            modelRotation = Matrix.Identity;
            lastModelRotation = Matrix.Identity;
            lastCameraRotation = Matrix.Identity;
            radius = defaultRadius;
            worldArcball.Reset();
            viewArcball.Reset();
            SetViewParameters(eye, lookAt);
        }

        /// <summary>
        /// Override for setting the view parameters
        /// </summary>
        public override void SetViewParameters(Vector3 eyePt, Vector3 lookAtPt)
        {
            // Call base first
            base.SetViewParameters(eyePt, lookAtPt);

            // Propogate changes to the member arcball
            Matrix rotation = Matrix.LookAtRH(eyePt, lookAtPt, UpDirection);
            viewArcball.CurrentQuaternion = Quaternion.RotationMatrix(rotation);

            // Set the radius according to the distance
            Vector3 eyeToPoint = lookAtPt - eyePt;
            SetRadius(eyeToPoint.Length());
        } 
        #endregion

        #region Mouse
        public override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // Current mouse position
            worldArcball.OnMove(e.X, e.Y);
            viewArcball.OnMove(e.X, e.Y);
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if ((rotateModelButtonMask & e.Button) != MouseButtons.None)
                worldArcball.OnBegin(e.X, e.Y);
            if ((rotateCameraButtonMask & e.Button) != MouseButtons.None)
                viewArcball.OnBegin(e.X, e.Y);
        }

        public override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if ((rotateModelButtonMask & e.Button) != MouseButtons.None)
                worldArcball.OnBegin(e.X, e.Y);
            if ((rotateCameraButtonMask & e.Button) != MouseButtons.None)
                viewArcball.OnBegin(e.X, e.Y);
        }

        public override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if ((rotateModelButtonMask & e.Button) != MouseButtons.None)
                worldArcball.OnEnd();
            if ((rotateCameraButtonMask & e.Button) != MouseButtons.None)
                viewArcball.OnEnd();
        } 
        #endregion
    }
}
