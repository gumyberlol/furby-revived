using UnityEngine;

namespace Furby
{
	public class InvokeShopButton : MonoBehaviour
	{
		[SerializeField]
		public ShopSelectPopulator m_ShopSelectPopulator;

		private void OnClick()
		{
			if (m_ShopSelectPopulator != null)
			{
				m_ShopSelectPopulator.gameObject.SetActive(!m_ShopSelectPopulator.gameObject.activeSelf);
				if (m_ShopSelectPopulator != null)
				{
					m_ShopSelectPopulator.PopulateMenu();
				}
			}
		}
	}
}
