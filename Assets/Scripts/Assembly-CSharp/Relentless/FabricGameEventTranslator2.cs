using System;
using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class FabricGameEventTranslator2 : GameEventTranslator<FabricGameEventReaction>
	{
		[SerializeField]
		private FabricGameEventReaction[] m_eventTable;

		[SerializeField]
		public GameObject m_fabricPrefab;

		protected override FabricGameEventReaction[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}

		public void InitialiseFromOldFabricGET(FabricGameEventTranslator oldTranslator)
		{
			m_gameEventType = new SerialisableType();
			m_gameEventType.Type = SerialisableType.FindType(oldTranslator.Type);
			List<FabricGameEventReaction> list = new List<FabricGameEventReaction>();
			FabricGameEventTranslator.GameToFabric[] eventTable = oldTranslator.EventTable;
			foreach (FabricGameEventTranslator.GameToFabric gameToFabric in eventTable)
			{
				FabricGameEventReaction fabricGameEventReaction = new FabricGameEventReaction();
				fabricGameEventReaction.GameEvent = new SerialisableEnum((Enum)Enum.Parse(m_gameEventType.Type, gameToFabric.GameEventName));
				fabricGameEventReaction.Action = gameToFabric.Action;
				fabricGameEventReaction.Delay = gameToFabric.Delay;
				fabricGameEventReaction.FabricEventName = gameToFabric.FabricEventName;
				fabricGameEventReaction.FloatParameter = gameToFabric.FloatParameter;
				fabricGameEventReaction.IgnoreGameObject = gameToFabric.IgnoreGameObject;
				fabricGameEventReaction.OverrideParameter = gameToFabric.OverrideParameter;
				fabricGameEventReaction.Probability = gameToFabric.Probability;
				fabricGameEventReaction.StringParameter = gameToFabric.StringParameter;
				list.Add(fabricGameEventReaction);
			}
			m_eventTable = list.ToArray();
		}
	}
}
