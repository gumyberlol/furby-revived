using System;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public abstract class GameEventTranslator<T> : GameEventReceiver where T : GameEventReaction
	{
		[SerialisableType.ValidTypes(SerialisableType.ValidTypes.StandardTypeSets.GameEventEnums)]
		[SerializeField]
		public SerialisableType m_gameEventType;

		private Dictionary<Enum, List<T>> m_eventLookup;

		public override Type EventType
		{
			get
			{
				return m_gameEventType.Type;
			}
		}

		protected abstract T[] EventTable { get; }

		private void Awake()
		{
			m_eventLookup = new Dictionary<Enum, List<T>>();
			T[] eventTable = EventTable;
			foreach (T val in eventTable)
			{
				Enum value = val.GameEvent.Value;
				List<T> value2;
				if (!m_eventLookup.TryGetValue(value, out value2))
				{
					value2 = new List<T>();
					m_eventLookup.Add(value, value2);
				}
				value2.Add(val);
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] list)
		{
			List<T> value;
			if (!m_eventLookup.TryGetValue(enumValue, out value))
			{
				return;
			}
			foreach (T item in value)
			{
				T current = item;
				current.React(gameObject, list);
			}
		}
	}
}
