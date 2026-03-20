using System;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ValueChangeEffect_Shake : MonoBehaviour
	{
		[SerializeField]
		private SerialisableEnum[] m_TriggerEvents;

		private GameEventSubscription m_Subscription;

		public float m_ShakeDurationSecs = 0.5f;

		public Vector3 m_ShakeMagnitude = new Vector3(0.02f, 0.02f, 0.02f);

		private void OnEnable()
		{
			m_Subscription = new GameEventSubscription(OnTriggerEvent, m_TriggerEvents.Select((SerialisableEnum x) => x.Value).ToArray());
		}

		private void OnDisable()
		{
			m_Subscription.Dispose();
		}

		private void OnTriggerEvent(Enum enumType, GameObject originator, params object[] parameters)
		{
			iTween.ShakePosition(base.gameObject, iTween.Hash("time", m_ShakeDurationSecs, "amount", m_ShakeMagnitude));
		}
	}
}
