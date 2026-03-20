using System;
using Relentless;
using UnityEngine;

public class SetActiveOnGameEvent : GameEventReceiver
{
	[SerializeField]
	private SerialisableEnum m_eventToListenFor;

	[SerializeField]
	private bool m_setEnabled;

	[SerializeField]
	public GameObject m_gameObjectToSet;

	[SerializeField]
	public float m_Delay;

	public override Type EventType
	{
		get
		{
			return m_eventToListenFor.Type;
		}
	}

	protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
	{
		if (enumValue.Equals(m_eventToListenFor.Value))
		{
			if (m_Delay <= 0f)
			{
				_SetTargetObjectActiveOrInactive();
			}
			else
			{
				Invoke("_SetTargetObjectActiveOrInactive", m_Delay);
			}
		}
	}

	private void _SetTargetObjectActiveOrInactive()
	{
		m_gameObjectToSet.SetActive(m_setEnabled);
	}
}
