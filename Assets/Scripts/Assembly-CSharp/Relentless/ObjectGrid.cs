using System.Collections;
using UnityEngine;

namespace Relentless
{
	public class ObjectGrid : RelentlessMonoBehaviour
	{
		public Vector3 m_gridSize;

		public bool m_renderGrid;

		public bool m_renderGridOnChildren;

		public bool m_drawAxisGridOnly;

		public Vector3 m_renderGridSize;

		public GameObject[] FindAt(int gridX, int gridY, int gridZ, bool includeInactive)
		{
			ArrayList arrayList = new ArrayList();
			GridSnapper[] componentsInChildren = GetComponentsInChildren<GridSnapper>(includeInactive);
			GridSnapper[] array = componentsInChildren;
			foreach (GridSnapper gridSnapper in array)
			{
				Vector3 localPosition = gridSnapper.transform.localPosition;
				int num = Mathf.RoundToInt(localPosition.x / ((m_gridSize.x != 0f) ? m_gridSize.x : 1f));
				int num2 = Mathf.RoundToInt(localPosition.y / ((m_gridSize.y != 0f) ? m_gridSize.y : 1f));
				int num3 = Mathf.RoundToInt(localPosition.z / ((m_gridSize.z != 0f) ? m_gridSize.z : 1f));
				if (num == gridX && num2 == gridY && num3 == gridZ)
				{
					arrayList.Add(gridSnapper.gameObject);
				}
			}
			GameObject[] array2 = new GameObject[arrayList.Count];
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j] = (GameObject)arrayList[j];
			}
			return array2;
		}

		public void GetXYZFrom(Vector3 objectIn, out int x, out int y, out int z)
		{
			x = Mathf.RoundToInt(objectIn.x / ((m_gridSize.x != 0f) ? m_gridSize.x : 1f));
			y = Mathf.RoundToInt(objectIn.y / ((m_gridSize.y != 0f) ? m_gridSize.y : 1f));
			z = Mathf.RoundToInt(objectIn.z / ((m_gridSize.z != 0f) ? m_gridSize.z : 1f));
		}

		public Vector3 Snap(Vector3 posIn)
		{
			Vector3 vector = base.transform.InverseTransformPoint(posIn);
			int num = Mathf.RoundToInt(vector.x / m_gridSize.x);
			int num2 = Mathf.RoundToInt(vector.y / m_gridSize.y);
			int num3 = Mathf.RoundToInt(vector.z / m_gridSize.z);
			return base.transform.TransformPoint(new Vector3((float)num * m_gridSize.x, (float)num2 * m_gridSize.y, (float)num3 * m_gridSize.z));
		}
	}
}
