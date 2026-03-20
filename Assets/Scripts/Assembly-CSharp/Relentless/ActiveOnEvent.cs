using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Relentless
{
	public class ActiveOnEvent : MonoBehaviour
	{
		public enum ActionType
		{
			Active = 1,
			Inactive = 0,
			Disabled = 2
		}

		[SerializeField]
		private ActionType m_initialState;

		[SerializeField]
		private ActionType m_action;

		[SerializeField]
		private SerialisableEnum[] m_triggerEvents;

		[SerializeField]
		private float m_delay;

		[SerializeField]
		private float m_revertAfterSeconds;

		[SerializeField]
		private SerialisableEnum m_eventOnRevert;

		private GameEventSubscription m_eventSubscription;

		public ActionType Action
		{
			get
			{
				return m_action;
			}
			set
			{
				m_action = value;
			}
		}

		private void Awake()
		{
			m_eventSubscription = new GameEventSubscription(OnGameEvent, m_triggerEvents.Select((SerialisableEnum x) => x.Value).ToArray());
			base.gameObject.SetActive(m_initialState == ActionType.Active);
		}

		private void OnDestroy()
		{
			CancelInvoke();
			m_eventSubscription.Dispose();
		}

		private void OnGameEvent(Enum eventType, GameObject originator, params object[] parameters)
		{
			GameEventRouter.Instance.StartCoroutine(DoActivation());
		}

		private IEnumerator DoActivation()
		{
			if (m_delay != 0f)
			{
				yield return new WaitForSeconds(m_delay);
			}
			base.gameObject.SetActive(m_action == ActionType.Active);
			if (m_revertAfterSeconds != 0f)
			{
				yield return new WaitForSeconds(m_revertAfterSeconds);
				base.gameObject.SetActive(m_action == ActionType.Inactive);
				if (m_eventOnRevert.IsTypeSet())
				{
					GameEventRouter.SendEvent(m_eventOnRevert.Value);
				}
			}
		}
	}
}
