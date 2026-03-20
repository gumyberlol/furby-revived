using System;
using Furby.Utilities.Salon;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Salon2
{
	public class CarouselItem : InGamePurchaseableItem
	{
		public delegate void ClickHandler();

		private SalonItem m_item;

		public SalonItem SalonItem
		{
			get
			{
				return m_item;
			}
		}

		public event ClickHandler Clicked;

		public void SetupFrom(SalonItem item)
		{
			m_item = item;
			UISprite itemSprite = GetItemSprite();
			UIAtlas graphicAtlas = item.GraphicAtlas;
			string graphic = item.Graphic;
			if (graphicAtlas.GetSprite(graphic) == null)
			{
				throw new ApplicationException(string.Format("When setting up for SalonItem \"{0}\", atlas \"{1}\" does not have a sprite called \"{2}\"", item.Name, graphicAtlas.gameObject.name, graphic));
			}
			itemSprite.atlas = graphicAtlas;
			itemSprite.spriteName = graphic;
			Rect outer = itemSprite.sprite.outer;
			Vector3 localScale = itemSprite.transform.localScale;
			itemSprite.transform.localScale = new Vector3(localScale.x * outer.width, localScale.y * outer.height, localScale.z);
			BoxCollider component = GetComponent<BoxCollider>();
			component.size = itemSprite.transform.localScale;
			base.gameObject.name = item.Name;
			SetLocked(!WholeGameShopHelpers.IsItemUnlocked(item));
		}

		public override void OnClickAlreadyPurchased()
		{
			if (this.Clicked != null)
			{
				this.Clicked();
			}
		}

		public override int GetFurbucksCost()
		{
			return m_item.Cost;
		}

		public override string GetItemName()
		{
			return Singleton<Localisation>.Instance.GetText(m_item.Name);
		}

		public override void Purchase()
		{
			WholeGameShopHelpers.PurchaseItem(m_item);
		}

		public override bool ShouldUseAfterPurchase()
		{
			return true;
		}
	}
}
