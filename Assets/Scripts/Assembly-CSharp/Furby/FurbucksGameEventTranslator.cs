using System;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbucksGameEventTranslator : MonoBehaviour
	{
		[Serializable]
		public class FurbucksEvent
		{
			public SerialisableEnum Trigger;

			public int FurbucksEarned;

			public int XPAwarded;

			public bool FurbucksRequireFurbyMode;

			public bool XPRequiresFurbyMode;
		}

		[SerializeField]
		[EasyEditArray]
		private FurbucksEvent[] m_furbuckEvents;

		private Dictionary<Enum, FurbucksEvent> m_furbuckEventMap;

		private GameEventSubscription m_eventSubs;

		private void OnEnable()
		{
			m_furbuckEventMap = new Dictionary<Enum, FurbucksEvent>();
			FurbucksEvent[] furbuckEvents = m_furbuckEvents;
			foreach (FurbucksEvent furbucksEvent in furbuckEvents)
			{
				m_furbuckEventMap.Add(furbucksEvent.Trigger, furbucksEvent);
			}
			m_eventSubs = new GameEventSubscription(OnFurbuckEvent, m_furbuckEvents.Select((FurbucksEvent x) => x.Trigger.Value).ToArray());
		}

		private void OnDisable()
		{
			m_eventSubs.Dispose();
		}

		private void OnFurbuckEvent(Enum enumType, GameObject originator, params object[] parameters)
		{
			FurbucksEvent value;
			if (m_furbuckEventMap.TryGetValue(enumType, out value))
			{
				if (!value.FurbucksRequireFurbyMode || !Singleton<GameDataStoreObject>.Instance.Data.NoFurbyMode)
				{
					Singleton<FurbucksWallet>.Instance.Balance += value.FurbucksEarned;
				}
				if (!value.XPRequiresFurbyMode || !Singleton<GameDataStoreObject>.Instance.Data.NoFurbyMode)
				{
					FurbyGlobals.Player.EarnedXP += value.XPAwarded;
				}
			}
		}
	}
}
