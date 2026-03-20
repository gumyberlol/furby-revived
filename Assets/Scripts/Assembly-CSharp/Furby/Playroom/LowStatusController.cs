using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public abstract class LowStatusController : RelentlessMonoBehaviour
	{
		public GameObject m_FurbyBabyInstance;

		protected GameObject m_ModelInstance;

		protected bool m_Suspended;

		private void Start()
		{
			CacheTargetModel();
			InitializeController();
			m_Suspended = false;
		}

		public abstract void InitializeController();

		public abstract void UpdateController();

		private void Update()
		{
			if (!m_Suspended)
			{
				UpdateController();
			}
		}

		private void CacheTargetModel()
		{
			ModelInstance component = m_FurbyBabyInstance.GetComponent<ModelInstance>();
			if (component != null)
			{
				m_ModelInstance = component.Instance;
			}
		}
	}
}
