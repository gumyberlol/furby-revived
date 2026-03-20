using System;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurbyStatsGameEventTranslator : MonoBehaviour
	{
		[Serializable]
		public class StatReaction
		{
			public SerialisableEnum Trigger;

			public FurbyStatEvent StatUpdate;

			public float Amount;

			public bool CureSickness;
		}

		[SerializeField]
		private StatReaction[] m_statReactions;

		private GameEventSubscription m_eventSubscription;

		private void OnEnable()
		{
			m_eventSubscription = new GameEventSubscription(OnTrigger, m_statReactions.Select((StatReaction x) => x.Trigger.Value).ToArray());
		}

		private void OnDisable()
		{
			m_eventSubscription.Dispose();
			m_eventSubscription = null;
		}

		private void OnTrigger(Enum evt, GameObject originator, params object[] parameters)
		{
			StatReaction statReaction = m_statReactions.Where((StatReaction x) => x.Trigger.Value.Equals(evt)).FirstOrDefault();
			if (statReaction != null)
			{
				PlayerFurby player = FurbyGlobals.Player;
				FurbyBaby selectedFurbyBaby = player.SelectedFurbyBaby;
				if (selectedFurbyBaby != null)
				{
					switch (statReaction.StatUpdate)
					{
					case FurbyStatEvent.UpdateFurbyBabyAttention:
						selectedFurbyBaby.NewAttention += statReaction.Amount;
						break;
					case FurbyStatEvent.UpdateFurbyBabyCleanliness:
						selectedFurbyBaby.NewCleanliness += statReaction.Amount;
						break;
					case FurbyStatEvent.UpdateFurbyBabySatiatedness:
						selectedFurbyBaby.NewSatiatedness += statReaction.Amount;
						break;
					}
				}
				switch (statReaction.StatUpdate)
				{
				case FurbyStatEvent.UpdateFurbyBowelEmptiness:
					player.NewBowelEmptiness += statReaction.Amount;
					break;
				case FurbyStatEvent.UpdateFurbyCleanliness:
					player.NewCleanliness += statReaction.Amount;
					break;
				case FurbyStatEvent.UpdateFurbyHappiness:
					player.NewHappiness += statReaction.Amount;
					break;
				case FurbyStatEvent.UpdateFurbySatietedness:
					player.NewSatiatedness += statReaction.Amount;
					break;
				}
				if (statReaction.CureSickness)
				{
					player.Sickness = false;
				}
				Singleton<GameDataStoreObject>.Instance.Save();
			}
			else
			{
				Logging.LogError(string.Format("FurbyStatsGameEventTranslator recieved {0} but had no handler for it", evt));
			}
		}
	}
}
