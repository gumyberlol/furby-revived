using Relentless;
using UnityEngine;

namespace Furby
{
	public abstract class ShopType : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_verticalCarouselToEnable;

		[SerializeField]
		[NamedText]
		private string m_shopTypeNamedText;

		[SerializeField]
		private UIAtlas m_shopTypeAtlas;

		[SerializeField]
		private string m_shopTypeLogoName;

		[SerializeField]
		private float m_logoScale = 1f;

		[SerializeField]
		protected ShopTypeCategory m_Category;

		public ShopTypeCategory Category
		{
			get
			{
				return m_Category;
			}
		}

		public string GetNamedText()
		{
			return Singleton<Localisation>.Instance.GetText(m_shopTypeNamedText);
		}

		public abstract int GetNumItems();

		public virtual void SetUpItem_Virtual(ShopItemUI item, int index)
		{
		}

		public virtual void SetUpItem_InApp(ShopItemUI_InApp item, int index)
		{
		}

		public abstract bool IsUnlocked(int index);

		public abstract string DEBUG_GetItemName(int index);

		public abstract void DEBUG_UnlockItem(int index);

		public virtual void RefreshList()
		{
		}

		public void Hide()
		{
			m_verticalCarouselToEnable.SetActive(false);
		}

		public void Show()
		{
			m_verticalCarouselToEnable.SetActive(true);
		}

		public string GetSpriteName()
		{
			return m_shopTypeLogoName;
		}

		public UIAtlas GetAtlas()
		{
			return m_shopTypeAtlas;
		}

		public float GetScale()
		{
			return m_logoScale;
		}
	}
}
