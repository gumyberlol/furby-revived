using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Pantry
{
	public class PantryShelf_v03 : MonoBehaviour
	{
		[SerializeField]
		private GameObject m_ItemPrefab;

		private PantryFoodData[] m_Foods;

		private UIGrid m_carouselGrid;

		private UIDraggablePanel m_dragPanel;

		public void InitialiseCarousel(PantryFoodData[] foods)
		{
			m_carouselGrid = GetComponentInChildren<UIGrid>();
			m_dragPanel = GetComponent<UIDraggablePanel>();
			m_Foods = foods;
			PantryFoodData[] foods2 = m_Foods;
			foreach (PantryFoodData pantryFoodData in foods2)
			{
				if (!pantryFoodData.UnlockedByQRCode || WholeGameShopHelpers.IsItemUnlocked(pantryFoodData))
				{
					GameObject gameObject = Object.Instantiate(m_ItemPrefab) as GameObject;
					PantryFood_v03 component = gameObject.GetComponent<PantryFood_v03>();
					component.FoodData = pantryFoodData;
					UISprite componentInChildren = gameObject.GetComponentInChildren<UISprite>();
					componentInChildren.pivot = UIWidget.Pivot.Bottom;
					componentInChildren.atlas = pantryFoodData.GraphicAtlas;
					componentInChildren.spriteName = pantryFoodData.GraphicName;
					componentInChildren.MakePixelPerfect();
					componentInChildren.transform.localScale *= 0.75f;
					UIDragPanelContents component2 = gameObject.GetComponent<UIDragPanelContents>();
					component2.draggablePanel = m_dragPanel;
					gameObject.gameObject.SetParentTransformIdentityLocalTransforms(m_carouselGrid.transform);
					gameObject.gameObject.layer = m_carouselGrid.gameObject.layer;
				}
			}
			Reposition();
		}

		private void Reposition()
		{
			m_carouselGrid.Reposition();
		}

		public void FadeShelf(bool visible)
		{
			StartCoroutine(Fade(0.75f, visible));
		}

		public void HideShelf()
		{
			m_dragPanel.enabled = false;
			List<PantryFood_v03> list = new List<PantryFood_v03>();
			m_carouselGrid.gameObject.GetComponentsInChildrenIncludeInactive(list);
			foreach (PantryFood_v03 item in list)
			{
				item.Fade(0f, false);
			}
		}

		private IEnumerator Fade(float duration, bool fadeToVisible)
		{
			m_dragPanel.enabled = false;
			List<PantryFood_v03> allFood = new List<PantryFood_v03>();
			m_carouselGrid.gameObject.GetComponentsInChildrenIncludeInactive(allFood);
			foreach (PantryFood_v03 food in allFood)
			{
				food.Fade(duration, fadeToVisible);
			}
			yield return new WaitForSeconds(duration);
			m_dragPanel.enabled = fadeToVisible;
		}
	}
}
