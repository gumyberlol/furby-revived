using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Relentless
{
	public class RelativeTransformTracker : MonoBehaviour
	{
		[SerializeField]
		private Transform m_ParentTransform;

		[SerializeField]
		private Transform m_ChildTransform;

		[SerializeField]
		private string m_ChildName;

		private IEnumerable<Transform> Traverse(Transform node)
		{
			while (node != m_ParentTransform)
			{
				yield return node;
				node = node.parent;
			}
		}

		private void LateUpdate()
		{
			if ((bool)m_ChildTransform)
			{
				Matrix4x4 identity = Matrix4x4.identity;
				IEnumerable<Transform> source = Traverse(m_ChildTransform);
				foreach (Transform item in source.Reverse())
				{
					Vector3 localPosition = item.localPosition;
					Quaternion localRotation = item.localRotation;
					Vector3 localScale = item.localScale;
					identity *= Matrix4x4.TRS(localPosition, localRotation, localScale);
				}
				Decompose(identity);
			}
			else
			{
				m_ChildTransform = m_ParentTransform.GetNamedChildTransform(m_ChildName);
			}
		}

		private void Decompose(Matrix4x4 matrix)
		{
			Vector3 vector = matrix.GetColumn(0);
			Vector3 upwards = matrix.GetColumn(1);
			Vector3 forward = matrix.GetColumn(2);
			Vector3 localPosition = matrix.GetColumn(3);
			base.transform.localPosition = localPosition;
			base.transform.localRotation = Quaternion.LookRotation(forward, upwards);
			base.transform.localScale = new Vector3(vector.magnitude, upwards.magnitude, forward.magnitude);
		}
	}
}
