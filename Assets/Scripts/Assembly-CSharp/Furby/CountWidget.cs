using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Furby.Scripts.FurMail;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class CountWidget : MonoBehaviour
	{
		public enum CountType
		{
			EggsInCarton = 0,
			FriendsInFriendsbook = 1,
			FurbiesInNeighbourhood = 2,
			EggsAvailableToLay = 3,
			FurMailUnReadMessages = 4,
			UnopenedGiftsYouHaveToPayFor = 5
		}

		[SerializeField]
		private CountType m_type;

		[SerializeField]
		private bool m_hideOnZero = true;

		[SerializeField]
		private UILabel m_label;

		[SerializeField]
		private float m_PollTime;

		private void Start()
		{
			UpdateCount();
			if (m_PollTime != 0f)
			{
				StartCoroutine(PollCount());
			}
		}

		private IEnumerator PollCount()
		{
			while (true)
			{
				yield return new WaitForSeconds(m_PollTime);
				UpdateCount();
			}
		}

		private void UpdateCount()
		{
			int num = 0;
			switch (m_type)
			{
			case CountType.EggsInCarton:
				num = FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count();
				break;
			case CountType.FriendsInFriendsbook:
				num = GetNumFriendsInFriendsBook();
				break;
			case CountType.FurbiesInNeighbourhood:
				num = FurbyGlobals.BabyRepositoryHelpers.Neighbourhood.Count();
				break;
			case CountType.EggsAvailableToLay:
				num = GetNumAvailableEggs();
				break;
			case CountType.FurMailUnReadMessages:
				num = GetFurMailNumberOfUnReadMessages();
				break;
			case CountType.UnopenedGiftsYouHaveToPayFor:
				num = GetNumberOfUnopenedGifts();
				break;
			}
			if (num == 0 && m_hideOnZero)
			{
				foreach (Transform item in base.transform)
				{
					item.gameObject.SetActive(false);
				}
			}
			else
			{
				foreach (Transform item2 in base.transform)
				{
					item2.gameObject.SetActive(true);
				}
			}
			if (m_label != null)
			{
				m_label.text = num.ToString();
			}
		}

		private int GetFurMailNumberOfUnReadMessages()
		{
			if (SingletonInstance<FurMailManager>.Instance != null)
			{
				return SingletonInstance<FurMailManager>.Instance.NewMessageCount;
			}
			return 0;
		}

		private int GetNumFriendsInFriendsBook()
		{
			IEnumerable<AdultFurbyType> unlocksInOrder = FurbyGlobals.Player.Furby.UnlocksInOrder;
			if (FurbyGlobals.Player.Furby.AdultType == AdultFurbyType.NoFurby)
			{
				unlocksInOrder = FurbyGlobals.Player.NoFurbyUnlock.UnlocksInOrder;
			}
			return unlocksInOrder.Where((AdultFurbyType x) => FurbyGlobals.Player.IsFurbyUnlocked(x)).Count();
		}

		private int GetNumAvailableEggs()
		{
			int num = FurbyGlobals.Player.NumEggsAvailable;
			int num2 = FurbyGlobals.Player.EarnedXP;
			int num3 = FurbyGlobals.Player.XP;
			int num4 = FurbyGlobals.Player.Level;
			while (num2 > 0 && num4 < FurbyGlobals.AdultLibrary.XpLevels.Count - 1)
			{
				int num5 = FurbyGlobals.AdultLibrary.XpLevels[num4 + 1];
				int num6 = num5 - num3;
				if (num6 > num2)
				{
					break;
				}
				num2 -= num6;
				num3 += num6;
				num4++;
				num++;
			}
			return num;
		}

		private int GetNumberOfUnopenedGifts()
		{
			return Singleton<GameDataStoreObject>.Instance.Data.m_UnopenedGiftIndices.Count;
		}
	}
}
