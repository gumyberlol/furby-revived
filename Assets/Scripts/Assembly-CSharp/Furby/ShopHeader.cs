using UnityEngine;

namespace Furby
{
	public class ShopHeader : MonoBehaviour
	{
		private ShopType m_shopType;

		[SerializeField]
		private UILabel m_shoptypeLabel;

		[SerializeField]
		private UISprite m_logoSprite;

		[SerializeField]
		private Transform m_scaleNode;

		public void SetShopType(ShopType shopType)
		{
			m_shopType = shopType;
			m_logoSprite.atlas = shopType.GetAtlas();
			m_logoSprite.spriteName = shopType.GetSpriteName();
			m_logoSprite.transform.localScale = new Vector2(m_logoSprite.sprite.inner.width, m_logoSprite.sprite.inner.height);
			m_scaleNode.transform.localScale = Vector3.one * shopType.GetScale();
			m_shoptypeLabel.text = shopType.GetNamedText();
		}

		public ShopType GetShopType()
		{
			return m_shopType;
		}
	}
}
