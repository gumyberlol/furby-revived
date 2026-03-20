using System;
using System.Collections;
using UnityEngine;

namespace Relentless
{
	public class WaitForGameEvent
	{
		public delegate void WaitWork();

		private bool m_finished;

		private Enum[] m_events;

		private Enum m_returnedEvent;

		private GameObject m_returnedGameObject;

		private object[] m_returnedParameters;

		public Enum ReturnedEvent
		{
			get
			{
				return m_returnedEvent;
			}
		}

		public GameObject ReturnedGameObject
		{
			get
			{
				return m_returnedGameObject;
			}
		}

		public object[] ReturnedParameters
		{
			get
			{
				return m_returnedParameters;
			}
		}

		private void RecieveEvent(Enum eventType, GameObject gameObject, params object[] paramList)
		{
			Enum[] events = m_events;
			foreach (Enum obj in events)
			{
				if (eventType.Equals(obj))
				{
					m_returnedEvent = eventType;
					m_returnedGameObject = gameObject;
					m_returnedParameters = paramList;
					m_finished = true;
				}
			}
		}

		private void RecieveAnyEvent(Enum eventType, GameObject gameObject, params object[] paramList)
		{
			m_returnedEvent = eventType;
			m_returnedGameObject = gameObject;
			m_returnedParameters = paramList;
			m_finished = true;
		}

		public IEnumerator WaitForEvent(WaitWork work, params Enum[] eventTypes)
		{
			m_events = eventTypes;
			m_finished = false;
			GameEventRouter.AddDelegateForEnums(RecieveEvent, eventTypes);
			try
			{
				while (!m_finished)
				{
					if (work != null)
					{
						work();
					}
					yield return null;
				}
			}
			finally
			{
				GameEventRouter.RemoveDelegateForEnums(RecieveEvent, eventTypes);
			}
		}

		public IEnumerator WaitForEvent(params Enum[] eventTypes)
		{
			m_events = eventTypes;
			m_finished = false;
			GameEventRouter.AddDelegateForEnums(RecieveEvent, eventTypes);
			try
			{
				while (!m_finished)
				{
					yield return null;
				}
			}
			finally
			{
				GameEventRouter.RemoveDelegateForEnums(RecieveEvent, eventTypes);
			}
		}

		public IEnumerator WaitForAnyEventOfType(Type type, WaitWork work = null)
		{
			m_finished = false;
			m_returnedEvent = null;
			GameEventRouter.AddDelegateForType(type, RecieveAnyEvent);
			try
			{
				while (!m_finished)
				{
					if (work != null)
					{
						work();
					}
					yield return null;
				}
			}
			finally
			{
				GameEventRouter.RemoveDelegateForType(type, RecieveAnyEvent);
			}
		}
	}
}
