using UnityEngine;

namespace Relentless
{
	public class DraggableCamera : MonoBehaviour
	{
		public Camera TargetCamera;

		public bool LimitCameraPosition;

		public Vector2 CameraLimits;

		public bool ReverseAxis;

		private bool m_isDragging;

		private Vector3 m_startPosition;

		private Plane m_plane;

		private void OnEnable()
		{
			m_isDragging = false;
			m_plane = new Plane(base.transform.forward, base.transform.position);
		}

		private void LateUpdate()
		{
			if (m_isDragging)
			{
				if (Input.GetMouseButtonUp(0))
				{
					m_isDragging = false;
				}
				Vector3 worldPos = Vector3.zero;
				Vector3 mousePosition = Input.mousePosition;
				if (ReverseAxis)
				{
					mousePosition = new Vector3((float)Screen.width - mousePosition.x, (float)Screen.height - mousePosition.y, mousePosition.z);
				}
				if (GetScreenPositionOnPlane(Input.mousePosition, ref worldPos))
				{
					TargetCamera.transform.position -= worldPos - m_startPosition;
					if (LimitCameraPosition)
					{
						ApplyCameraLimits();
					}
				}
			}
			else if ((!Input.GetMouseButtonDown(0) || !Input.GetMouseButtonDown(1)) && Input.GetMouseButtonDown(0) && GetScreenPositionOnPlane(Input.mousePosition, ref m_startPosition))
			{
				m_isDragging = true;
			}
		}

		private void ApplyCameraLimits()
		{
			bool flag = true;
			Vector3 worldPos = new Vector3(TargetCamera.pixelRect.xMin, 0f, 0f);
			Vector3 worldPos2 = new Vector3(TargetCamera.pixelRect.xMax, 0f, 0f);
			Vector3 worldPos3 = new Vector3(0f, TargetCamera.pixelRect.yMin, 0f);
			Vector3 worldPos4 = new Vector3(0f, TargetCamera.pixelRect.yMax, 0f);
			flag &= GetScreenPositionOnPlane(worldPos, ref worldPos);
			flag &= GetScreenPositionOnPlane(worldPos2, ref worldPos2);
			flag &= GetScreenPositionOnPlane(worldPos3, ref worldPos3);
			if (flag & GetScreenPositionOnPlane(worldPos4, ref worldPos4))
			{
				worldPos = base.transform.InverseTransformPoint(worldPos);
				worldPos2 = base.transform.InverseTransformPoint(worldPos2);
				worldPos3 = base.transform.InverseTransformPoint(worldPos3);
				worldPos4 = base.transform.InverseTransformPoint(worldPos4);
				float num = Mathf.Clamp(0f - CameraLimits.x - worldPos.x, 0f, float.MaxValue);
				num += Mathf.Clamp(CameraLimits.x - worldPos2.x, float.MinValue, 0f);
				float num2 = Mathf.Clamp(0f - CameraLimits.y - worldPos3.y, 0f, float.MaxValue);
				num2 += Mathf.Clamp(CameraLimits.y - worldPos4.y, float.MinValue, 0f);
				Vector3 vector = base.transform.TransformPoint(new Vector3(num, num2, 0f)) - base.transform.position;
				TargetCamera.transform.position += vector;
			}
		}

		private bool GetScreenPositionOnPlane(Vector3 screenPos, ref Vector3 worldPos)
		{
			Ray ray = TargetCamera.ScreenPointToRay(screenPos);
			float enter;
			if (m_plane.Raycast(ray, out enter))
			{
				worldPos = ray.GetPoint(enter);
				return true;
			}
			return false;
		}

		private void OnDrawGizmosSelected()
		{
			if (LimitCameraPosition)
			{
				Matrix4x4 matrix = Gizmos.matrix;
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawWireCube(Vector3.zero, new Vector3(CameraLimits.x * 2f, CameraLimits.y * 2f, 0f));
				Gizmos.matrix = matrix;
			}
		}
	}
}
