using System;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ValueChangeEffect_ScaleDown : MonoBehaviour
	{
		[SerializeField]
		private SerialisableEnum[] m_TriggerEvents;

		private GameEventSubscription m_Subscription;

		public float m_DurationSecs = 0.5f;

		public Vector3 m_ScaleStart = new Vector3(1f, 1f, 1f);

		private Vector3 m_DefaultScale = new Vector3(0f, 0f, 0f);

		private void OnEnable()
		{
			m_Subscription = new GameEventSubscription(OnTriggerEvent, m_TriggerEvents.Select((SerialisableEnum x) => x.Value).ToArray());
			m_DefaultScale = base.transform.localScale;
		}

		private void OnDisable()
		{
			m_Subscription.Dispose();
		}

		private void OnTriggerEvent(Enum enumType, GameObject originator, params object[] parameters)
		{
			iTween.ScaleTo(base.gameObject, iTween.Hash("time", m_DurationSecs, "scale", m_ScaleStart));
			Invoke("ResetScale", m_DurationSecs);
		}

		private void ResetScale()
		{
			base.gameObject.transform.localScale = m_DefaultScale;
		}
	}
}
