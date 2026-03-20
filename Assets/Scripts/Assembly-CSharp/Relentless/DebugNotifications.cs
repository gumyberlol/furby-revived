using System;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class DebugNotifications : SingletonInstance<DebugNotifications>
	{
		public class DebugNotification
		{
			public DateTime FirstDisplayed { get; set; }

			public TimeSpan Delay { get; set; }

			public string Message { get; set; }
		}

		private static readonly List<DebugNotification> s_notifications = new List<DebugNotification>();

		public Rect LabelPosition;

		public GUIStyle TextStyle;

		public bool RelativeToLeftOfScreen;

		public bool RelativeToTopOfScreen;

		public bool DisplayListDownwards = true;

		public static void AddNotification(string message, TimeSpan delay)
		{
			s_notifications.Add(new DebugNotification
			{
				Delay = delay,
				Message = message,
				FirstDisplayed = DateTime.Now
			});
		}

		private void Start()
		{
			if (TextStyle == null)
			{
				TextStyle = new GUIStyle();
			}
		}
	}
}
