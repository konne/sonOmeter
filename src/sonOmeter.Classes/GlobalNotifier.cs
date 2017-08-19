using System;
using UKLib.Debug;
using System.Collections.Generic;
using System.Diagnostics;

namespace sonOmeter.Classes
{
	/// <summary>
	/// The GlobalNotifier class generalizes event transport in the application.
	/// </summary>
	public class GlobalNotifier
	{
		public enum MsgTypes
		{
			WorkLineChanged,
			RecordingChanged,
			NewSonarLine,
			DeviceChanged,
			LineFound,
			SwitchProperties,
			ToggleDevice,
			TogglePoints,
			Toggle3DRecord,
			ToggleDXFFile,
			ToggleBlankLine,
			SetStatusBar,
			PlaceBuoy,
			PlaceManualPoint,
			BuoyListChanged,
			NewRecord,
			NewDeviceList,
			NewCoordinate,
			NewBlankLine,
			UpdateCoordinates,
			UpdateProfiles,
            UpdateRecord,
			CutEvent,
			Interpolate,
            EditBlankLine,
            Close,
			None
		}

		private static Dictionary<MsgTypes, GlobalEventHandler> eventList = new Dictionary<MsgTypes, GlobalEventHandler>();

		private static MsgTypes state = MsgTypes.None;

		public static BeepThreadClass BeepThread = new BeepThreadClass();

		private static bool Contains(GlobalEventHandler eventHandler, MsgTypes type)
		{
			if (eventList[type] == null)
				return false;

			Array invList = eventList[type].GetInvocationList();

			for (int i = 0; i < invList.Length; i++)
				if (invList.GetValue(i) as GlobalEventHandler == eventHandler)
					return true;

			return false;
		}

		public static void SignIn(GlobalEventHandler eventHandler, MsgTypes type)
		{
			// Parameter validation.
			if (eventHandler == null)
				throw new ArgumentNullException("eventHandler");

			// Sign in.
			if (!eventList.ContainsKey(type))
				eventList.Add(type, null);

			if (!Contains(eventHandler, type))
				eventList[type] += eventHandler;
		}

		public static void SignIn(GlobalEventHandler eventHandler, List<MsgTypes> list)
		{
			if (eventHandler == null)
				throw new ArgumentNullException("eventHandler");

			foreach (MsgTypes itm in list)
			{
				SignIn(eventHandler, itm);
			}
		}

		public static void SignOut(GlobalEventHandler eventHandler, MsgTypes type)
		{
			// Parameter validation.
			if (eventHandler == null)
				throw new ArgumentNullException("eventHandler");

			// Sign out.
			try
			{
				if (eventList.ContainsKey(type))
					if (Contains(eventHandler, type))
						eventList[type] -= eventHandler;
			}
			catch
			{
			}
		}

		public static void SignOut(GlobalEventHandler eventHandler, List<MsgTypes> list)
		{
			// Parameter validation.			
			if (eventHandler == null)
				throw new ArgumentNullException("eventHandler");

			foreach (MsgTypes itm in list)
			{
				SignOut(eventHandler, itm);
			}
		}

		public static long counter = 0;
		public static DateTime time = DateTime.MinValue;
		public static void DebugMessage(string msg, bool reset)
		{
			if ((time == DateTime.MinValue) | reset)
				time = DateTime.Now;

			TimeSpan span = DateTime.Now.Subtract(time);

			Debug.WriteLine((counter++).ToString("000000") + ", time " + span.ToString().Substring(0, 8) + ": " + msg);
			time = DateTime.Now;
		}

		public static void Invoke(object sender, object args, MsgTypes msgType)
		{
			// Parameter validation.
			if (sender == null)
				throw new ArgumentNullException("GlobalNotifier : sender");

			// Invoke.
			state = msgType;

			if (eventList.ContainsKey(msgType))
			{
				if (eventList[msgType] != null)
					eventList[msgType](sender, args, msgType);
			}

			state = MsgTypes.None;
		}
	}

	public delegate void GlobalEventHandler(object sender, object args, GlobalNotifier.MsgTypes state);
}
