using Relentless;
using UnityEngine;

namespace Furby.Utilities.Bath
{
	public class BathShelf : RelentlessMonoBehaviour
	{
		public GameObject[] possibleItems;

		private void Start()
		{
			if (possibleItems == null || possibleItems.Length == 0)
			{
				return;
			}
			int num = 12;
			Vector3 localPosition = new Vector3(0f - (float)num * 0.5f, 0f, 0f);
			Vector3 vector = new Vector3(1f, 0f, 0f);
			int num2 = -1;
			for (int i = 0; i < num; i++)
			{
				int num3 = num2;
				do
				{
					num3 = Random.Range(0, possibleItems.Length);
				}
				while (num2 == num3 && possibleItems.Length > 1);
				num2 = num3;
				GameObject gameObject = (GameObject)Object.Instantiate(possibleItems[num3], base.transform.position, Quaternion.identity);
				gameObject.SetParentTransformIdentityLocalTransforms(base.transform);
				gameObject.transform.localPosition = localPosition;
				BathItem component = gameObject.GetComponent<BathItem>();
				if (component != null)
				{
					component.dragTarget = base.gameObject;
				}
				localPosition += vector;
			}
		}

		private void Update()
		{
		}

		public void DetachItem(DragItem item, Transform newParent)
		{
			item.transform.parent = newParent;
		}

		public void RattachItem(DragItem item)
		{
			item.transform.parent = base.transform;
		}
	}
}
