using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ShopLogic : GameEventReceiver
	{
		private List<ShopType> m_shopTypes = new List<ShopType>();

		[SerializeField]
		private GameObject m_RestorePurchasesRef;

		public override Type EventType
		{
			get
			{
				return typeof(ShopGameEvents);
			}
		}

		private void Start()
		{
			bool amEligibleForCrystal = Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal;
			ShopType[] componentsInChildren = GetComponentsInChildren<ShopType>(true);
			ShopType[] array = componentsInChildren;
			foreach (ShopType shopType in array)
			{
				if (!amEligibleForCrystal && shopType.Category == ShopTypeCategory.PurchaseWithRealMoney)
				{
					Logging.Log("ShopLogic:: NOT Adding " + shopType.name);
					continue;
				}
				Logging.Log("ShopLogic:: Adding " + shopType.name);
				m_shopTypes.Add(shopType);
			}
			ReconcilePurchases();
		}

		private void ReconcilePurchases()
		{
			if (Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal && !Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked)
			{
				string itemID = "FurbyIAPBundle4";
				if (SingletonInstance<RsStoreMediator>.Instance.HasItemBeenPurchased(itemID))
				{
					Singleton<GameDataStoreObject>.Instance.UnlockCrystal();
				}
			}
		}

		private IEnumerator WaitForInAppAndNetwork()
		{
			while (SingletonInstance<RsStoreMediator>.Instance == null)
			{
				yield return new WaitForEndOfFrame();
			}
			while (!SingletonInstance<RsStoreMediator>.Instance.AmAbleToMediatePurchases())
			{
				yield return new WaitForEndOfFrame();
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			if ((ShopGameEvents)(object)enumValue != ShopGameEvents.ShopTypeSelected)
			{
				return;
			}
			foreach (ShopType shopType2 in m_shopTypes)
			{
				shopType2.Hide();
			}
			ShopType shopType = gameObject.GetComponentInChildren<ShopHeader>().GetShopType();
			if (m_RestorePurchasesRef != null)
			{
				if (shopType.Category == ShopTypeCategory.PurchaseWithRealMoney)
				{
					m_RestorePurchasesRef.SetActive(true);
				}
				else
				{
					m_RestorePurchasesRef.SetActive(false);
				}
			}
			shopType.Show();
		}
	}
}
