using System;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Blender
{
	public class Carousel : Singleton<Carousel>
	{
		public delegate void ScrollHandler();

		public GameObject m_ItemPrefab;

		[SerializeField]
		private IngredientList m_ingredientList;

		public event ScrollHandler Scrolled;

		public int AppendinateIngredients(bool zeroCost)
		{
			int num = 0;
			UIGrid componentInChildren = GetComponentInChildren<UIGrid>();
			UIDraggablePanel component = GetComponent<UIDraggablePanel>();
			if (m_ingredientList == null)
			{
				m_ingredientList = ScriptableObject.CreateInstance<IngredientList>();
			}
			foreach (Ingredient item in m_ingredientList.Items)
			{
				bool locked = false;
				if (!WholeGameShopHelpers.IsItemUnlocked(item))
				{
					locked = true;
				}
				if (zeroCost != Convert.ToBoolean(item.Cost))
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(m_ItemPrefab) as GameObject;
					CarouselItem componentInChildren2 = gameObject.GetComponentInChildren<CarouselItem>();
					UIDragPanelContents componentInChildren3 = gameObject.GetComponentInChildren<UIDragPanelContents>();
					UISprite itemSprite = componentInChildren2.GetItemSprite();
					Vector3 one = Vector3.one;
					itemSprite.spriteName = item.Graphic;
					componentInChildren3.draggablePanel = component;
					componentInChildren2.m_ItemIngredient = item;
					componentInChildren2.SetLocked(locked);
					one.x = itemSprite.sprite.inner.width;
					one.y = itemSprite.sprite.inner.height;
					itemSprite.transform.localScale = one;
					gameObject.transform.parent = componentInChildren.transform;
					gameObject.transform.localPosition = Vector2.zero;
					gameObject.transform.localScale = Vector3.one;
					num++;
				}
			}
			return num;
		}

		public void OnEnableContents(bool enabled)
		{
			UIGrid componentInChildren = GetComponentInChildren<UIGrid>();
			CarouselItem[] componentsInChildren = componentInChildren.GetComponentsInChildren<CarouselItem>();
			CarouselItem[] array = componentsInChildren;
			foreach (CarouselItem carouselItem in array)
			{
				carouselItem.SetDisabled(!enabled);
			}
		}

		public void Start()
		{
			UIGrid componentInChildren = GetComponentInChildren<UIGrid>();
			UIDraggablePanel component = GetComponent<UIDraggablePanel>();
			int num = AppendinateIngredients(false);
			int num2 = AppendinateIngredients(true);
			int num3 = num + num2;
			int num4 = num2 + 1;
			int num5 = 0;
			int num6 = num4;
			while (num5 < num3)
			{
				Transform child = componentInChildren.transform.GetChild(num5);
				string text = string.Format("{0:00}", num6 % num3);
				child.name = text;
				num5++;
				num6++;
			}
			componentInChildren.Reposition();
			component.onDragFinished = delegate
			{
				if (this.Scrolled != null)
				{
					this.Scrolled();
				}
			};
		}
	}
}
