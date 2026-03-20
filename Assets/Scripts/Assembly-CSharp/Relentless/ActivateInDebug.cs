using UnityEngine;

namespace Relentless
{
	public class ActivateInDebug : MonoBehaviour
	{
		public GameObject m_TargetObject;

		public void Start()
		{
			if (Debug.isDebugBuild)
			{
				m_TargetObject.SetActive(true);
			}
		}
	}
}
