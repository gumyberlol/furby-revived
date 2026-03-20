using System;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ValueChangeEffect_ScaleUp : MonoBehaviour
	{
		[SerializeField]
		private SerialisableEnum[] m_TriggerEvents;

		private GameEventSubscription m_Subscription;

		public float m_DurationSecs = 0.5f;

		public Vector3 m_ScaleStart = new Vector3(1f, 1f, 1f);

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
			iTween.ScaleFrom(base.gameObject, iTween.Hash("time", m_DurationSecs, "scale", m_ScaleStart));
		}
	}
}
