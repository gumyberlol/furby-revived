using UnityEngine;

namespace Furby
{
	public class ShopItemUI_InApp : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_AlreadyPurchasedHierarchy;

		[SerializeField]
		private GameObject m_PurchaseableHierarchy;

		[SerializeField]
		private GameObject m_ShopUnavailableHierarchy;

		[SerializeField]
		private UILabel[] m_priceLabel;

		[SerializeField]
		private UISprite[] m_itemSprite;

		[SerializeField]
		private UILabel[] m_nameLabel;

		public GameObject AlreadyPurchasedHierarchy
		{
			get
			{
				return m_AlreadyPurchasedHierarchy;
			}
		}

		public GameObject PurchaseableHierarchy
		{
			get
			{
				return m_PurchaseableHierarchy;
			}
		}

		public GameObject ShopUnavailableHierarchy
		{
			get
			{
				return m_ShopUnavailableHierarchy;
			}
		}

		public void SetUp(float spriteScale)
		{
			ShopPurchaseableItem_InApp component = GetComponent<ShopPurchaseableItem_InApp>();
			if (component != null)
			{
				UILabel[] priceLabel = m_priceLabel;
				foreach (UILabel uILabel in priceLabel)
				{
					uILabel.text = component.GetRealPrice().ToString();
				}
				UILabel[] nameLabel = m_nameLabel;
				foreach (UILabel uILabel2 in nameLabel)
				{
					uILabel2.text = component.GetItemName();
				}
				UISprite[] itemSprite = m_itemSprite;
				foreach (UISprite uISprite in itemSprite)
				{
					uISprite.sprite = null;
					uISprite.atlas = component.GetAtlas();
					uISprite.spriteName = component.GetSpriteName();
					uISprite.transform.localScale = new Vector2(uISprite.sprite.inner.width, uISprite.sprite.inner.height);
					uISprite.MakePixelPerfect();
					uISprite.transform.localScale *= spriteScale;
				}
				AlreadyPurchasedHierarchy.SetActive(false);
				PurchaseableHierarchy.SetActive(false);
				ShopUnavailableHierarchy.SetActive(false);
			}
			UIPanel component2 = base.gameObject.GetComponent<UIPanel>();
			if (component2 != null)
			{
				Object.Destroy(component2);
			}
		}

		private void OnDestroy()
		{
			UISprite[] itemSprite = m_itemSprite;
			foreach (UISprite uISprite in itemSprite)
			{
				uISprite.atlas = null;
				uISprite.material = null;
			}
		}
	}
}
