using UnityEngine;

namespace Relentless
{
	public class UpdateCopyTransform : RelentlessMonoBehaviour
	{
		private Transform m_target;

		public bool m_ignoreVerticalTilt = true;

		public void SetTarget(Transform t)
		{
			m_target = t;
			Update();
		}

		public void SetTarget(GameObject go)
		{
			SetTarget(go.transform);
		}

		private void Update()
		{
			if (m_target != null)
			{
				base.transform.position = m_target.position;
				if (m_ignoreVerticalTilt)
				{
					Vector3 vector = m_target.TransformDirection(new Vector3(0f, 0f, 1f));
					vector.y = 0f;
					base.transform.rotation = Quaternion.LookRotation(vector.normalized);
				}
				else
				{
					base.transform.localRotation = m_target.rotation;
				}
			}
		}
	}
}
