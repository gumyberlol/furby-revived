using UnityEngine;

namespace Furby
{
	public class ObjectStatePropagator : MonoBehaviour
	{
		[SerializeField]
		private GameObject[] m_ObjectArray;

		[SerializeField]
		private bool m_InvertState;

		public void OnEnable()
		{
			if (m_ObjectArray != null)
			{
				GameObject[] objectArray = m_ObjectArray;
				foreach (GameObject gameObject in objectArray)
				{
					gameObject.SetActive(!m_InvertState);
				}
			}
		}

		public void OnDisable()
		{
			if (m_ObjectArray != null)
			{
				GameObject[] objectArray = m_ObjectArray;
				foreach (GameObject gameObject in objectArray)
				{
					gameObject.SetActive(m_InvertState);
				}
			}
		}
	}
}
