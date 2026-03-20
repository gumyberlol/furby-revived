using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class GiftAwarder : MonoBehaviour
	{
		public bool CanAwardAGift()
		{
			int num = Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices.Count + Singleton<GameDataStoreObject>.Instance.Data.m_OpenedGiftIndices.Count;
			int count = FurbyGlobals.GiftList.m_GiftItemData.Count;
			return count > num;
		}

		public int AwardRandomGift()
		{
			int num = 0;
			if (CanAwardAGift())
			{
				num = GetIndexOfAnUnwardedGift(Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices, Singleton<GameDataStoreObject>.Instance.Data.m_OpenedGiftIndices);
				AwardGift(num);
			}
			else
			{
				string text = "No gifts spare!";
				DebugUtils.Assert(false, text);
				Logging.LogError(text);
			}
			return num;
		}

		private void AwardGift(int indexIntoGiftList)
		{
			if (!Singleton<GameDataStoreObject>.Instance.Data.GiftExists(indexIntoGiftList))
			{
				DebugUtils.Assert(false, "Gift_" + indexIntoGiftList + " does not exist, aborting");
				return;
			}
			if (Singleton<GameDataStoreObject>.Instance.Data.GiftHasBeenAwardedButIsUnopened(indexIntoGiftList))
			{
				DebugUtils.Assert(false, "Gift_" + indexIntoGiftList + " already awarded, aborting");
				return;
			}
			if (Singleton<GameDataStoreObject>.Instance.Data.GiftHasBeenAwardedAndOpened(indexIntoGiftList))
			{
				DebugUtils.Assert(false, "Gift_" + indexIntoGiftList + " already awarded and opened , aborting");
				return;
			}
			Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices.Add(indexIntoGiftList);
			Singleton<GameDataStoreObject>.Instance.Save();
		}

		private int GetIndexOfAnUnwardedGift(List<int> alreadyUnopened, List<int> alreadyOpened)
		{
			int num = -1;
			List<int> list = new List<int>();
			for (int i = 0; i < FurbyGlobals.GiftList.m_GiftItemData.Count; i++)
			{
				list.Add(i);
			}
			foreach (int item in alreadyUnopened)
			{
				list[item] = num;
			}
			foreach (int item2 in alreadyOpened)
			{
				list[item2] = num;
			}
			int num2 = num;
			while (num2 == num)
			{
				int index = Random.Range(0, list.Count);
				num2 = list[index];
			}
			return list[num2];
		}
	}
}
