using UnityEngine;

namespace Relentless
{
	public class TransformTracker : MonoBehaviour
	{
		[SerializeField]
		private Transform m_target;

		[SerializeField]
		private bool m_trackPosition = true;

		[SerializeField]
		private bool m_trackRotation;

		[SerializeField]
		private bool m_trackLocalScale;

		[SerializeField]
		private bool m_trackOffset;

		[SerializeField]
		private string m_childNodeName;

		private Transform m_actualTarget;

		private Vector3 m_offset = Vector3.zero;

		private Vector3 m_projectedForward = Vector3.forward;

		private Vector3 m_projectedUpward = Vector3.up;

		public Transform Target
		{
			get
			{
				return m_target;
			}
			set
			{
				m_target = value;
			}
		}

		public bool TrackPosition
		{
			get
			{
				return m_trackPosition;
			}
			set
			{
				m_trackPosition = value;
			}
		}

		public bool TrackRotation
		{
			get
			{
				return m_trackRotation;
			}
			set
			{
				m_trackRotation = value;
			}
		}

		public bool TrackOffset
		{
			get
			{
				return m_trackOffset;
			}
			set
			{
				m_trackOffset = value;
			}
		}

		public bool TrackLocalScale
		{
			get
			{
				return m_trackLocalScale;
			}
			set
			{
				m_trackLocalScale = value;
			}
		}

		private void GetActualTarget()
		{
			Transform transform = m_target;
			ModelInstance component = transform.GetComponent<ModelInstance>();
			if (component != null)
			{
				if (component.Instance == null)
				{
					return;
				}
				transform = component.Instance.transform;
			}
			if (!string.IsNullOrEmpty(m_childNodeName))
			{
				transform = transform.GetNamedChildTransform(m_childNodeName);
			}
			if (m_trackOffset)
			{
				m_offset = transform.transform.worldToLocalMatrix.MultiplyPoint(base.transform.position);
				m_projectedForward = transform.transform.worldToLocalMatrix.MultiplyVector(base.transform.forward).normalized;
				m_projectedUpward = transform.transform.worldToLocalMatrix.MultiplyVector(base.transform.up).normalized;
			}
			m_actualTarget = transform;
		}

		public void Track()
		{
			if (m_actualTarget == null)
			{
				GetActualTarget();
				return;
			}
			if (m_trackPosition)
			{
				base.transform.position = m_actualTarget.localToWorldMatrix.MultiplyPoint(m_offset);
			}
			if (m_trackRotation)
			{
				if (m_trackOffset)
				{
					Vector3 forward = m_actualTarget.localToWorldMatrix.MultiplyVector(m_projectedForward);
					Vector3 upwards = m_actualTarget.localToWorldMatrix.MultiplyVector(m_projectedUpward);
					base.transform.rotation = Quaternion.LookRotation(forward, upwards) * m_actualTarget.localRotation;
				}
				else
				{
					base.transform.rotation = m_actualTarget.rotation;
				}
			}
			if (m_trackLocalScale)
			{
				base.transform.localScale = m_actualTarget.localScale;
			}
		}

		private void LateUpdate()
		{
			Track();
		}
	}
}
