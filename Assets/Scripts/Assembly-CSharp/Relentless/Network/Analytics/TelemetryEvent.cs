using UnityEngine;

namespace Relentless.Network.Analytics
{
	public class TelemetryEvent : RelentlessMonoBehaviour
	{
		public enum EventType
		{
			Button = 0
		}

		public enum Trigger
		{
			OnClick = 0
		}

		public EventType eventType;

		public Trigger trigger;

		private string m_ScreenName;

		public void Awake()
		{
			m_ScreenName = string.Empty;
			Transform parent = base.gameObject.transform;
			while (parent.parent != null && parent.GetComponents<GUIScreen>().Length == 0)
			{
				parent = parent.parent;
			}
			if (parent != null)
			{
				m_ScreenName = parent.name;
			}
		}

		private void OnPress(bool isDown)
		{
		}

		private void OnClick()
		{
			if (base.enabled && trigger == Trigger.OnClick)
			{
				LogEvent();
			}
		}

		private void LogEvent()
		{
			TelemetryParams telemetryParams = new TelemetryParams("Screen", m_ScreenName);
			telemetryParams.Add("GameObject", base.gameObject.name);
			SingletonInstance<TelemetryManager>.Instance.LogEvent(trigger.ToString(), telemetryParams, false);
		}
	}
}
