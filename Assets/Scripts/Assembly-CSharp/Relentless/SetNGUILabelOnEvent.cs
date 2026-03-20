using System;
using System.Linq;
using UnityEngine;

namespace Relentless
{
	public class SetNGUILabelOnEvent : MonoBehaviour
	{
		[SerializeField]
		private SerialisableEnum[] m_triggerEvents;

		private GameEventSubscription m_subscription;

		private void OnEnable()
		{
			m_subscription = new GameEventSubscription(OnTriggerEvent, m_triggerEvents.Select((SerialisableEnum x) => x.Value).ToArray());
		}

		private void OnDisable()
		{
			m_subscription.Dispose();
		}

		private void OnTriggerEvent(Enum enumType, GameObject originator, params object[] parameters)
		{
			UILabel[] componentsInChildren = base.gameObject.GetComponentsInChildren<UILabel>();
			foreach (UILabel uILabel in componentsInChildren)
			{
				uILabel.text = parameters[0].ToString();
			}
		}
	}
}
