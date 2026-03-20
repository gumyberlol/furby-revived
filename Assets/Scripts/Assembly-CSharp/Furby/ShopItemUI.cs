using UnityEngine;

namespace Furby
{
	public class ShopItemUI : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_canBuyHierarchy;

		[SerializeField]
		private GameObject m_isBoughtHierarchy;

		[SerializeField]
		private GameObject m_unavailableHierarchy;

		[SerializeField]
		private UILabel[] m_priceLabel;

		[SerializeField]
		private UISprite[] m_itemSprite;

		[SerializeField]
		private UILabel[] m_nameLabel;

		public void SetUp(float spriteScale)
		{
			ShopPurchaseableItem component = GetComponent<ShopPurchaseableItem>();
			if (component != null)
			{
				UILabel[] priceLabel = m_priceLabel;
				foreach (UILabel uILabel in priceLabel)
				{
					uILabel.text = component.GetFurbucksCost().ToString();
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
				m_isBoughtHierarchy.SetActive(component.IsPurchased());
				m_canBuyHierarchy.SetActive(!component.IsPurchased());
				if (m_unavailableHierarchy != null)
				{
					m_unavailableHierarchy.SetActive(false);
				}
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
