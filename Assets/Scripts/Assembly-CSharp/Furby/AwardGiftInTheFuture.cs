using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class AwardGiftInTheFuture : MonoBehaviour
	{
		[SerializeField]
		private GiftEligibilityThresholds m_thresholds;

		private GameData m_gameData;

		private Action m_action;

		public void Start()
		{
			m_gameData = Singleton<GameDataStoreObject>.Instance.Data;
			m_action = delegate
			{
				GameData data = Singleton<GameDataStoreObject>.Instance.Data;
				int numFurblingsSinceGifting = data.NumFurblingsSinceGifting;
				int num = m_thresholds.GiftCountForHatchCount(numFurblingsSinceGifting);
				int numGiftsAwarded = data.NumGiftsAwarded;
				int num2 = num - numGiftsAwarded;
				if (num2 > 0)
				{
					data.MarkEligibilityForGift();
				}
			};
			m_gameData.EggHatched += m_action;
		}

		public void OnDestroy()
		{
			m_gameData.EggHatched -= m_action;
		}
	}
}
