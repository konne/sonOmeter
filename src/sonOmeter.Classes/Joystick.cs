using System;
using System.Collections.Generic;
using System.Text;
using SlimDX.DirectInput;
using System.Threading;
using System.ComponentModel;

namespace sonOmeter.Classes
{
    public delegate void JoystickButtonChangedEventHandler(int Button, bool State);
    public delegate void JoystickXYZChangedEventHandler(int X, int Y, int Z);

    public class Joystick
    {
        #region Variables & Properties
        SlimDX.DirectInput.Joystick joystickDevice;
        Thread thread;

        bool threadRunning = true;

        public event JoystickButtonChangedEventHandler JoystickButtonChanged;
        public event JoystickXYZChangedEventHandler JoystickXYZChanged;

        private ISynchronizeInvoke synchronizingObject;
        public ISynchronizeInvoke SynchronizingObject
        {
            get { return synchronizingObject; }
            set
            {
                synchronizingObject = value;
            }
        }
        #endregion

        #region Constructor
        public Joystick()
        {
            DirectInput dInput = new DirectInput();
            List<DeviceInstance> devicelist = new List<DeviceInstance>(dInput.GetDevices(DeviceType.Joystick, DeviceEnumerationFlags.AttachedOnly));
            
            foreach (DeviceInstance deviceinstance in devicelist)
            {
                if (deviceinstance.Type != DeviceType.Joystick)
                    continue;

                joystickDevice = new SlimDX.DirectInput.Joystick(dInput, deviceinstance.InstanceGuid);
                //joystickDevice.SetCooperativeLevel(this, CooperativeLevelFlags.Background | CooperativeLevelFlags.Exclusive);
                joystickDevice.Acquire();

                thread = new Thread(new ThreadStart(ThreadProc));
                thread.IsBackground = true;
                thread.Start();                                  
            }
        }
        #endregion

        #region Close
        public void Close()
        {
            threadRunning = false;

            try
            {              
                if (synchronizingObject != null && thread != null && thread.IsAlive)
                    thread.Join();
            }
            catch
            {
            }
        }
        #endregion

        #region Thread
        private void ThreadProc()
        {
            JoystickState lastState;
            lock (joystickDevice)
            {
                lastState = joystickDevice.GetCurrentState();
            }

            int lastX = 0;
            int lastY = 0;
            int lastZ = 0;

            while (threadRunning)
            {
                try
                {
                    JoystickState state = joystickDevice.GetCurrentState();

                    bool[] btn_new = state.GetButtons();
                    bool[] btn_old = lastState.GetButtons();

                    if (JoystickXYZChanged != null)
                    {
                        int diffX = Math.Abs(state.X - lastX);
                        int diffY = Math.Abs(state.Y - lastY);
                        int diffZ = Math.Abs(state.Z - lastZ);

                        if ((diffX > 300) | (diffY > 300) | (diffZ > 300))
                        {
                            if (synchronizingObject != null && synchronizingObject.InvokeRequired)
                            {
                                synchronizingObject.BeginInvoke(JoystickXYZChanged,
                                    new object[3] { state.X, state.Y, state.Z });
                            }
                            else
                            {
                                JoystickXYZChanged(state.X, state.Y, state.Z);
                            }
                            lastX = state.X;
                            lastY = state.Y;
                            lastZ = state.Z;
                        }
                    }


                    if (JoystickButtonChanged != null)
                    {
                        for (int i = 0; i < btn_new.Length; i++)
                        {
                            if (btn_new[i] != btn_old[i])
                            {
                                if (synchronizingObject != null && synchronizingObject.InvokeRequired)
                                {
                                    synchronizingObject.BeginInvoke(JoystickButtonChanged,
                                        new object[2] { i, btn_new[i] });
                                }
                                else
                                {
                                    JoystickButtonChanged(i, btn_new[i]);
                                }
                            }
                        }
                    }
                    lastState = state;
                }
                catch
                {
                    threadRunning = false;
                }

                Thread.Sleep(10);
            }
        }
        #endregion
    }
}
