using Relentless;
using UnityEngine;

namespace Furby
{
	public class ShopSelectPopulator : MonoBehaviour
	{
		[SerializeField]
		public GameObject m_InAppShop;

		public void PopulateMenu()
		{
			if (!Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal)
			{
				m_InAppShop.gameObject.SetActive(false);
			}
		}
	}
}
