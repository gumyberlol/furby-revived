using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class CreateShopVerticalPanel : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_shopItemPrefab;

		[SerializeField]
		private ShopType m_shopType;

		private List<GameObject> m_createdObjects = new List<GameObject>();

		private void Start()
		{
			GetComponent<UIGrid>().Reposition();
			Rebuild();
			GameEventRouter.AddDelegateForEnums(PurchaseEventHandler, ShopGameEvents.ShopItemsUpdated);
		}

		private void OnDestroy()
		{
			GameEventRouter.RemoveDelegateForEnums(PurchaseEventHandler, ShopGameEvents.ShopItemsUpdated);
		}

		private void Awake()
		{
		}

		private void Rebuild()
		{
			foreach (GameObject createdObject in m_createdObjects)
			{
				UnityEngine.Object.DestroyImmediate(createdObject);
			}
			m_createdObjects.Clear();
			m_shopType.RefreshList();
			int numItems = m_shopType.GetNumItems();
			for (int i = 0; i < numItems; i++)
			{
				GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_shopItemPrefab, base.transform.position, base.transform.rotation);
				if (m_shopType.Category == ShopTypeCategory.PurchaseWithRealMoney)
				{
					ShopItemUI_InApp component = gameObject.GetComponent<ShopItemUI_InApp>();
					m_shopType.SetUpItem_InApp(component, i);
				}
				else
				{
					ShopItemUI component2 = gameObject.GetComponent<ShopItemUI>();
					m_shopType.SetUpItem_Virtual(component2, i);
				}
				m_createdObjects.Add(gameObject);
			}
			foreach (GameObject createdObject2 in m_createdObjects)
			{
				Vector3 localScale = createdObject2.transform.localScale;
				createdObject2.transform.parent = base.transform;
				createdObject2.transform.localScale = localScale;
			}
			GetComponent<UIGrid>().Reposition();
		}

		private void OnEnable()
		{
			DebugUtils.Log_InMagenta("CreateShopVerticalPanel OnEnable");
			Rebuild();
			GetComponent<UIGrid>().Reposition();
		}

		private void PurchaseEventHandler(Enum eventType, GameObject gameObject, params object[] parameters)
		{
			Rebuild();
		}

		private void Update()
		{
		}
	}
}
