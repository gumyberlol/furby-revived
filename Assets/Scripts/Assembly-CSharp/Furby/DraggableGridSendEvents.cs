using Relentless;
using UnityEngine;

namespace Furby
{
	public class DraggableGridSendEvents : RelentlessMonoBehaviour
	{
		[SerializeField]
		private SerialisableEnum m_clickEvent;

		[SerializeField]
		private float m_gridSize = 100f;

		private Vector3 m_lastPosition;

		private int m_lastPlayedIndex;

		private float m_lastPlayedPosition;

		private void Awake()
		{
			m_lastPosition = base.transform.localPosition;
			m_lastPlayedPosition = m_lastPosition.x;
		}

		private void Update()
		{
			if (Mathf.Abs(base.transform.localPosition.x - m_lastPlayedPosition) >= m_gridSize * 0.98f)
			{
				GameEventRouter.SendEvent(m_clickEvent.Value);
				m_lastPlayedPosition = (float)(int)Mathf.Round(base.transform.localPosition.x / m_gridSize) * m_gridSize;
			}
			m_lastPosition = base.transform.localPosition;
		}
	}
}
