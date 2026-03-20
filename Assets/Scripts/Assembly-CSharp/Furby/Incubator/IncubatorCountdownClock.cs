using System;
using UnityEngine;

namespace Furby.Incubator
{
	public class IncubatorCountdownClock : MonoBehaviour
	{
		[SerializeField]
		private UILabel m_TimerLabel;

		public void Update()
		{
			FurbyBaby inProgressFurbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			float num = inProgressFurbyBaby.IncubationDuration - inProgressFurbyBaby.IncubationProgress;
			SetDuration(TimeSpan.FromSeconds(num));
		}

		public void SetDuration(TimeSpan time)
		{
			string format = "{0:00}:{1:00}:{2:00}";
			int hours = time.Hours;
			int minutes = time.Minutes;
			int seconds = time.Seconds;
			m_TimerLabel.text = string.Format(format, hours, minutes, seconds);
		}
	}
}
