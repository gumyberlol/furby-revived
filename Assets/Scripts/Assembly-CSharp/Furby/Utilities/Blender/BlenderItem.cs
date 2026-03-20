using Relentless;
using UnityEngine;

namespace Furby.Utilities.Blender
{
	public class BlenderItem : MonoBehaviour
	{
		public Ingredient m_ItemIngredient;

		public int Index
		{
			get
			{
				return GetComponent<UISprite>().depth;
			}
		}

		public static int Comparer(BlenderItem left, BlenderItem right)
		{
			return left.Index - right.Index;
		}

		public void OnIndexChange(int index)
		{
			UISprite component = GetComponent<UISprite>();
			BoxCollider component2 = GetComponent<BoxCollider>();
			component2.center = new Vector3(0f, 0f, (float)index * -0.1f - 0.1f);
			component.depth = index;
		}

		public void OnClick()
		{
			Singleton<Blender>.Instance.OnRemoveItem(this);
		}
	}
}
