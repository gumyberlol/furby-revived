using UnityEngine;

namespace Relentless
{
	public class PooledObject : RelentlessMonoBehaviour
	{
		public int m_defaultPooledObjectsPerUser = 1;

		private GameObject m_prefabRef;

		private Vector3 m_originalPosition;

		private Quaternion m_originalRotation;

		private bool m_isInPool;

		private void Awake()
		{
			m_originalPosition = base.transform.localPosition;
			m_originalRotation = base.transform.localRotation;
		}

		public void SetPoolSettings(GameObject pool)
		{
			m_prefabRef = pool;
		}

		public GameObject GetPool()
		{
			return m_prefabRef;
		}

		private void OnEnable()
		{
			base.transform.localPosition = m_originalPosition;
			base.transform.localRotation = m_originalRotation;
			SetInPool(false);
		}

		public bool IsInPool()
		{
			return m_isInPool;
		}

		public void SetInPool(bool inPool)
		{
			m_isInPool = inPool;
		}
	}
}
