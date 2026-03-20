using Relentless;
using UnityEngine;

namespace Furby
{
	public class UpsellMessaging : MonoBehaviour
	{
		public int m_UpsellMessageFrequency = 10;

		private int m_CountOfScreenLoads;

		public void IncrementScreenLoad()
		{
			m_CountOfScreenLoads++;
			if (m_CountOfScreenLoads > m_UpsellMessageFrequency)
			{
				ResetScreenLoadCounter();
			}
		}

		public void ResetScreenLoadCounter()
		{
			m_CountOfScreenLoads = 0;
		}

		public bool ShouldTriggerUpsellMessage()
		{
			if (EligibleForUpsellMessages())
			{
				return m_CountOfScreenLoads >= m_UpsellMessageFrequency;
			}
			return false;
		}

		private bool EligibleForUpsellMessages()
		{
			if (Singleton<GameDataStoreObject>.Instance.HasAGameLoaded())
			{
				return FurbyGlobals.Player.NoFurbyOnSaveGame();
			}
			return false;
		}
	}
}
