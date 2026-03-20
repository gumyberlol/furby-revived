using System.Collections;
using UnityEngine;

namespace Relentless
{
	public class UIGridWrapper : MonoBehaviour
	{
		private void OnEnable()
		{
			StartCoroutine(RunUIGridWrapper());
		}

		private IEnumerator RunUIGridWrapper()
		{
			yield return null;
			yield return null;
			UIPanel panel = base.transform.parent.GetComponent<UIPanel>();
			UIGrid grid = GetComponent<UIGrid>();
			if (!panel || !grid)
			{
				yield break;
			}
			float size = (float)base.transform.childCount * grid.cellWidth;
			float extents = size / 2f;
			while (true)
			{
				float newX = panel.clipRange.x;
				foreach (Transform child in base.transform)
				{
					if (newX + extents < child.transform.localPosition.x)
					{
						child.transform.localPosition -= new Vector3(size, 0f, 0f);
					}
					if (newX - extents > child.transform.localPosition.x)
					{
						child.transform.localPosition += new Vector3(size, 0f, 0f);
					}
				}
				yield return null;
			}
		}
	}
}
