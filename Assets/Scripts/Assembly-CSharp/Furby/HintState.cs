using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class HintState
	{
		private class EnabledPeriod : IDisposable
		{
			private HintState m_hint;

			public EnabledPeriod(HintState hint)
			{
				m_hint = hint;
				hint.SoftEnable();
			}

			public void Dispose()
			{
				m_hint.SoftDisable();
			}
		}

		[SerializeField]
		public float m_Interval = 7f;

		private bool m_Enable;

		private bool m_Triggered;

		private uint m_refCount;

		[SerializeField]
		public HintEvents m_PayloadEvent;

		[SerializeField]
		public List<HintEvents> m_DisableEvents;

		public bool IsEnabled()
		{
			return m_Enable;
		}

		public bool IsTriggered()
		{
			return m_Triggered;
		}

		public void Enable()
		{
			m_Enable = true;
			m_Triggered = false;
			FurbyGlobals.InputInactivity.ResetInactivity();
		}

		public void Disable()
		{
			m_Enable = false;
			if (m_DisableEvents == null)
			{
				return;
			}
			foreach (HintEvents disableEvent in m_DisableEvents)
			{
				GameEventRouter.SendEvent(disableEvent);
			}
		}

		public void SetEnabled(bool b)
		{
			if (b)
			{
				Enable();
			}
			else
			{
				Disable();
			}
		}

		public void SoftEnable()
		{
			bool flag = IsEnabled();
			m_refCount++;
			if (!flag)
			{
				Enable();
			}
		}

		public void SoftDisable()
		{
			bool flag = IsEnabled();
			m_refCount--;
			if (flag && m_refCount == 0)
			{
				Disable();
			}
		}

		public void TestAndBroadcastState()
		{
			if (m_Enable && FurbyGlobals.InputInactivity.HasIntervalPassed(m_Interval))
			{
				FurbyGlobals.InputInactivity.ResetInactivity();
				GameEventRouter.SendEvent(m_PayloadEvent);
				m_Triggered = true;
			}
		}

		public IDisposable GetEnabledPeriod()
		{
			return new EnabledPeriod(this);
		}
	}
}
