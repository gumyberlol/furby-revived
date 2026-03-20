using Relentless;
using UnityEngine;

namespace Furby.SaveSlotSelect
{
	public class ShowBasedOnSaveState : MonoBehaviour
	{
		public enum ShowType
		{
			ShowIfSpecifiedSlotExists = 0,
			ShowIfAnySlotEmpty = 1,
			ShowIfAnySlotExists = 2,
			ShowIfAllSlotsEmpty = 3,
			ShowIfSpecifiedSlotDoesntExist = 4,
			ShowIfSpecifiedSlotIsNoFurby = 5,
			ShowIfSpecifiedSlotIsHasAFurby = 6,
			NeverShow = 7
		}

		[SerializeField]
		private ShowType m_showType;

		[SerializeField]
		private int m_specifiedSlot;

		[SerializeField]
		private GameObject m_optionalTarget;

		private void Start()
		{
			bool flag = false;
			switch (m_showType)
			{
			case ShowType.ShowIfAnySlotEmpty:
			{
				for (int i = 0; i < Singleton<GameDataStoreObject>.Instance.GetNumSlots(); i++)
				{
					if (!Singleton<GameDataStoreObject>.Instance.GetSlot(i).HasCompletedFirstTimeFlow)
					{
						flag = true;
					}
				}
				break;
			}
			case ShowType.ShowIfSpecifiedSlotExists:
				if (Singleton<GameDataStoreObject>.Instance.GetSlot(m_specifiedSlot).HasCompletedFirstTimeFlow)
				{
					flag = true;
				}
				break;
			case ShowType.ShowIfAnySlotExists:
			{
				for (int k = 0; k < Singleton<GameDataStoreObject>.Instance.GetNumSlots(); k++)
				{
					if (Singleton<GameDataStoreObject>.Instance.GetSlot(k).HasCompletedFirstTimeFlow)
					{
						flag = true;
					}
				}
				break;
			}
			case ShowType.ShowIfAllSlotsEmpty:
			{
				flag = true;
				for (int j = 0; j < Singleton<GameDataStoreObject>.Instance.GetNumSlots(); j++)
				{
					if (Singleton<GameDataStoreObject>.Instance.GetSlot(j).HasCompletedFirstTimeFlow)
					{
						flag = false;
					}
				}
				break;
			}
			case ShowType.ShowIfSpecifiedSlotDoesntExist:
				if (!Singleton<GameDataStoreObject>.Instance.GetSlot(m_specifiedSlot).HasCompletedFirstTimeFlow)
				{
					flag = true;
				}
				break;
			case ShowType.ShowIfSpecifiedSlotIsNoFurby:
				if (Singleton<GameDataStoreObject>.Instance.GetSlot(m_specifiedSlot).HasCompletedFirstTimeFlow && Singleton<GameDataStoreObject>.Instance.GetSlot(m_specifiedSlot).NoFurbyMode)
				{
					flag = true;
				}
				break;
			case ShowType.ShowIfSpecifiedSlotIsHasAFurby:
				if (Singleton<GameDataStoreObject>.Instance.GetSlot(m_specifiedSlot).HasCompletedFirstTimeFlow && !Singleton<GameDataStoreObject>.Instance.GetSlot(m_specifiedSlot).NoFurbyMode)
				{
					flag = true;
				}
				break;
			case ShowType.NeverShow:
				flag = false;
				break;
			}
			if (m_optionalTarget == null)
			{
				if (!flag)
				{
					base.gameObject.SetActive(false);
				}
			}
			else
			{
				m_optionalTarget.SetActive(flag);
			}
		}
	}
}
