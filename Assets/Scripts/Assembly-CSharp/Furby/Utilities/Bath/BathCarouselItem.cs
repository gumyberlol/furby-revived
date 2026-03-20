using Relentless;
using UnityEngine;

namespace Furby.Utilities.Bath
{
	public class BathCarouselItem : MonoBehaviour
	{
		public BathIngredient m_ItemIngredient;

		public void OnClick()
		{
			Logging.Log("BathCarouselItem::OnClick");
			if (Singleton<BathContents>.Instance.AddItem(this))
			{
				Logging.Log("BathCarouselItem::OnClick - Added");
			}
			else if (Singleton<BathContents>.Instance.RemoveItem(this))
			{
				Logging.Log("BathCarouselItem::OnClick - Removed");
			}
			else
			{
				Logging.Log("BathCarouselItem::OnClick - Nop");
			}
		}
	}
}
