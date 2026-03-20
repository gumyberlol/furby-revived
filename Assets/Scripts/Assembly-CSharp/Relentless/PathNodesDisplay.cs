using UnityEngine;

namespace Relentless
{
	public class PathNodesDisplay : RelentlessMonoBehaviour
	{
		public Color m_lineColour = new Color(1f, 1f, 1f, 0.5f);

		public bool m_displayAsLoop = true;

		public bool m_displayUnselected = true;

		private void OnDrawGizmos()
		{
			if (m_displayUnselected)
			{
				OnDrawGizmosSelected();
			}
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.color = m_lineColour;
			if (base.transform.childCount < 2)
			{
				return;
			}
			Vector3 position = base.transform.GetChild(0).position;
			if (m_displayAsLoop)
			{
				position = base.transform.GetChild(base.transform.childCount - 1).position;
			}
			int num = 0;
			foreach (Transform item in base.transform)
			{
				Gizmos.DrawLine(position, item.position);
				position = item.position;
				num++;
			}
		}
	}
}
