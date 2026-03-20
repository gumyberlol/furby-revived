using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class HintHandler : RelentlessMonoBehaviour
	{
		private List<HintEvents> m_ExpirationList = new List<HintEvents>();

		private GameEventSubscription m_EventSubscriptions;

		private float m_StartingTime;

		private float m_EndingTime;

		private bool m_TimeExpired;

		private bool m_DebugMode;

		public HintHandler()
		{
			m_DebugMode = false;
		}

		public void Start()
		{
			m_DebugMode = false;
		}

		public void Update()
		{
			if (m_TimeExpired && Time.time > m_EndingTime)
			{
				if (m_DebugMode)
				{
					Logging.Log("Hint " + base.name + " Expired because of time");
				}
				ExpireHint();
			}
			if (!SingletonInstance<ModalityMediator>.Instance.IsAvailable())
			{
				ExpireHint();
			}
		}

		private void OnDisable()
		{
			ClearSubscriptions();
		}

		private void OnDestroy()
		{
			ClearSubscriptions();
		}

		private void ClearSubscriptions()
		{
			if (m_EventSubscriptions != null)
			{
				m_EventSubscriptions.Dispose();
			}
		}

		private void ExpireHint()
		{
			if ((bool)this)
			{
				UnityEngine.Object.Destroy(base.transform.root.gameObject);
			}
		}

		public void RegisterExpiry(HintExpiryCausation[] expirationList)
		{
			foreach (HintExpiryCausation hintExpiryCausation in expirationList)
			{
				if (hintExpiryCausation.m_Causation == ExpiryCausation.CloseOnEvent)
				{
					m_ExpirationList.Add(hintExpiryCausation.m_EventExpiry);
				}
				if (hintExpiryCausation.m_Causation == ExpiryCausation.CloseAfterDuration)
				{
					m_StartingTime = Time.time;
					m_EndingTime = m_StartingTime + hintExpiryCausation.m_DurationExpiry;
					m_TimeExpired = true;
				}
			}
			m_EventSubscriptions = new GameEventSubscription(typeof(HintEvents), HandleExpiryEvent);
		}

		public void HandleExpiryEvent(Enum enumValue, GameObject gameObject, params object[] list)
		{
			HintEvents hintEvents = (HintEvents)(object)enumValue;
			if (m_ExpirationList.Contains(hintEvents))
			{
				if (m_DebugMode)
				{
					Logging.Log("Hint " + base.name + " Expired because of event: " + hintEvents);
				}
				ExpireHint();
			}
		}
	}
}
