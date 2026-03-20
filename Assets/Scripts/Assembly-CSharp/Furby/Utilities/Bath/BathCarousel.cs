using Relentless;
using UnityEngine;

namespace Furby.Utilities.Bath
{
	public class BathCarousel : Singleton<BathCarousel>
	{
		public GameObject m_ItemPrefab;

		[SerializeField]
		private BathIngredientList m_ingredientList;

		public void Start()
		{
			UIGrid componentInChildren = GetComponentInChildren<UIGrid>();
			UIDraggablePanel component = GetComponent<UIDraggablePanel>();
			if (m_ingredientList == null)
			{
				m_ingredientList = ScriptableObject.CreateInstance<BathIngredientList>();
			}
			if (m_ingredientList.Ingredients.Count > 0)
			{
				foreach (Transform item in componentInChildren.transform)
				{
					Object.Destroy(item.gameObject);
				}
			}
			int num = 0;
			foreach (BathIngredient ingredient in m_ingredientList.Ingredients)
			{
				GameObject gameObject = Object.Instantiate(m_ItemPrefab) as GameObject;
				BathCarouselItem component2 = gameObject.GetComponent<BathCarouselItem>();
				UIDragPanelContents component3 = gameObject.GetComponent<UIDragPanelContents>();
				UISprite component4 = gameObject.GetComponent<UISprite>();
				Vector3 one = Vector3.one;
				component4.spriteName = ingredient.Graphic;
				component3.draggablePanel = component;
				component2.m_ItemIngredient = ingredient;
				one.x = component4.sprite.inner.width;
				one.y = component4.sprite.inner.height;
				gameObject.transform.parent = componentInChildren.transform;
				gameObject.transform.localScale = one;
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localPosition = new Vector3(gameObject.transform.localPosition.x, gameObject.transform.localPosition.y, -5f);
				num = (component4.depth = num + 1);
			}
			componentInChildren.Reposition();
		}
	}
}
