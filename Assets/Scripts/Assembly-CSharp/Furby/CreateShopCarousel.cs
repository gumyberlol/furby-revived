using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class CreateShopCarousel : RelentlessMonoBehaviour
	{
		[SerializeField]
		[EasyEditArray]
		private ShopType[] m_shopTypes;

		[SerializeField]
		private Transform m_gridRoot;

		[SerializeField]
		private UICenterOnChild m_centerOnChild;

		[SerializeField]
		private GameObject m_horizontalSliderPrefab;

		private GameObject m_currentlyCenteredShop;

		private bool m_initialised;

		private IEnumerator Start()
		{
			SetupShopSections();
			yield return null;
			Recenter();
			m_initialised = true;
		}

		private void Update()
		{
			if (m_initialised && m_centerOnChild.centeredObject != m_currentlyCenteredShop)
			{
				DebugUtils.Log_InCyan("CreateShopCarousel Update centered on " + m_centerOnChild.centeredObject.name);
				DebugUtils.Log_InCyan("CreateShopCarousel Update not centered on " + m_currentlyCenteredShop.name);
				m_currentlyCenteredShop = m_centerOnChild.centeredObject;
				GameEventRouter.SendEvent(ShopGameEvents.ShopTypeSelected, m_currentlyCenteredShop);
				m_centerOnChild.Recenter(m_centerOnChild.centeredObject.transform);
			}
		}

		private void Recenter()
		{
			m_centerOnChild.Recenter(m_currentlyCenteredShop.transform);
			GameEventRouter.SendEvent(ShopGameEvents.ShopTypeSelected, m_currentlyCenteredShop);
		}

		private void SetupShopSections()
		{
			bool amEligibleForCrystal = Singleton<GameDataStoreObject>.Instance.GlobalData.AmEligibleForCrystal;
			ShopType[] shopTypes = m_shopTypes;
			foreach (ShopType shopType in shopTypes)
			{
				bool flag = true;
				if (shopType.Category == ShopTypeCategory.PurchaseWithRealMoney && !amEligibleForCrystal)
				{
					flag = false;
				}
				if (flag)
				{
					DebugUtils.Log_InCyan("CreateShopCarousel::Adding " + shopType.ToString());
					GameObject gameObject = (GameObject)Object.Instantiate(m_horizontalSliderPrefab);
					if (m_currentlyCenteredShop == null)
					{
						m_currentlyCenteredShop = gameObject;
					}
					gameObject.transform.parent = m_gridRoot;
					gameObject.transform.localPosition = Vector3.zero;
					gameObject.transform.localScale = Vector3.one;
					ShopHeader component = gameObject.GetComponent<ShopHeader>();
					component.SetShopType(shopType);
					m_gridRoot.GetComponent<UIGrid>().Reposition();
					if (SetStartingShop.s_startingShop != null && shopType.name.ToLower().Contains(SetStartingShop.s_startingShop.ToLower()))
					{
						DebugUtils.Log_InCyan("CreateShopCarousel Centering on " + shopType.name.ToLower());
						m_currentlyCenteredShop = gameObject;
					}
				}
				else
				{
					DebugUtils.Log_InCyan("CreateShopCarousel::Excluding " + shopType.ToString());
				}
			}
		}

		private IEnumerator WaitForIAP()
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
	}
}
