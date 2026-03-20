using UnityEngine;

namespace Relentless
{
	public class CameraControllerDragToPan : CameraController
	{
		public CameraPanSettings m_PanSettings;

		[HideInInspector]
		public ScreenBounds m_ScreenBounds = new ScreenBounds();

		public bool m_TapActive;

		public bool m_DragActive;

		private Vector2 m_PreviousFingerPos = Vector2.zero;

		private Vector2 m_PreviousDeltaPosition = Vector2.zero;

		private Vector2 m_PreviousMovementDelta = Vector2.zero;

		protected override bool ValidateConfiguration()
		{
			if ((bool)m_TheCamera)
			{
				return true;
			}
			return false;
		}

		protected override void UpdateCameraControls()
		{
		}

		protected override void Initialise()
		{
			m_ScreenBounds.InitialiseScreenBounds(m_TheCamera.transform, m_TheCamera.camera);
			base.IsActive = true;
		}

		private void OnEnable()
		{
			Logging.Log("CameraControllerDragToPan - OnEnable");
			if (m_TapActive)
			{
				FingerGestures.OnFingerTap += FingerGestures_OnFingerTap;
			}
			if (m_DragActive)
			{
				FingerGestures.OnDragBegin += FingerGestures_OnDragBegin;
				FingerGestures.OnDragMove += FingerGestures_OnDragMove;
				FingerGestures.OnDragEnd += FingerGestures_OnDragEnd;
			}
		}

		private void OnDisable()
		{
			Logging.Log("CameraControllerDragToPan - OnDisable");
			if (m_TapActive)
			{
				FingerGestures.OnFingerTap -= FingerGestures_OnFingerTap;
			}
			if (m_DragActive)
			{
				FingerGestures.OnDragBegin -= FingerGestures_OnDragBegin;
				FingerGestures.OnDragMove -= FingerGestures_OnDragMove;
				FingerGestures.OnDragEnd -= FingerGestures_OnDragEnd;
			}
		}

		private void FingerGestures_OnFingerTap(int fingerIndex, Vector2 fingerPos)
		{
			if (m_CameraType == CameraType.Orthographic)
			{
			}
			if (m_CameraType != CameraType.Perspective)
			{
			}
		}

		private void FingerGestures_OnDragMove(Vector2 fingerPos, Vector2 delta)
		{
			if (m_CameraType == CameraType.Orthographic)
			{
				MoveToFingerPos_Clamped(fingerPos);
			}
			if (m_CameraType != CameraType.Perspective)
			{
			}
		}

		private void FingerGestures_OnDragBegin(Vector2 fingerPos, Vector2 startPos)
		{
			MoveToFingerPos_Clamped(fingerPos);
		}

		private void FingerGestures_OnDragEnd(Vector2 fingerPos)
		{
			MoveToFingerPos_Clamped(fingerPos);
		}

		private void ApplyFingerPositionToCameraPositionCumulatively_V1(Vector2 currentFingerPos)
		{
			currentFingerPos.x = (float)Screen.width - currentFingerPos.x;
			currentFingerPos.y = (float)Screen.height - currentFingerPos.y;
			Vector3 vector = Util_GetPositionOnPlane(currentFingerPos);
			Vector3 vector2 = Util_GetPositionOnPlane(m_PreviousFingerPos);
			Vector3 vector3 = vector2 - vector;
			m_PreviousFingerPos = currentFingerPos;
			ApplyDeltaToCameraPosition(vector3);
		}

		private Vector3 Util_GetPositionOnPlane(Vector2 posOnViewport)
		{
			Vector3 vector = new Vector3(0f, 0f, 0f);
			Vector3 position = new Vector3(posOnViewport.x, posOnViewport.y, 0f);
			Ray ray = m_TheCamera.ViewportPointToRay(position);
			float num = (vector.z - ray.origin.z) / ray.direction.z;
			float x = ray.direction.x * num + ray.origin.x;
			float y = ray.direction.y * num + ray.origin.y;
			return new Vector3(x, y, vector.z);
		}

		private void ApplyDeltaToCameraPosition(Vector2 fingerPos)
		{
			float num = 1f - m_TheCamera.orthographicSize;
			Vector3 in_position = m_TheCamera.transform.position;
			float num2 = fingerPos.x * m_PanSettings.m_OrthographicSettings.m_Sensitivity * num;
			float num3 = fingerPos.y * m_PanSettings.m_OrthographicSettings.m_Sensitivity * num;
			in_position.x += num2;
			in_position.y += num3;
			m_ScreenBounds.ClampToBounds(ref m_TheCamera, ref in_position);
			m_TheCamera.transform.position = in_position;
		}

		private void ApplyFingerPositionToCameraPositionCumulatively_V2(Vector2 currentFingerPos)
		{
			Vector2 vector = currentFingerPos - m_PreviousFingerPos;
			m_PreviousFingerPos = currentFingerPos;
			Vector2 vector2 = (m_PreviousDeltaPosition = Vector2.Lerp(vector, m_PreviousDeltaPosition, 0.5f)) * m_PanSettings.m_OrthographicSettings.m_Sensitivity;
			Vector3 position = m_TheCamera.transform.position;
			Vector3 in_position = position;
			in_position.x += vector2.x;
			in_position.y += vector2.y;
			m_ScreenBounds.ClampToBounds(ref m_TheCamera, ref in_position);
			m_TheCamera.transform.position = in_position;
		}

		private void ApplyFingerPositionToCameraPositionCumulatively_V3(Vector2 currentFingerPos)
		{
			currentFingerPos.x = (float)Screen.width - currentFingerPos.x;
			currentFingerPos.y = (float)Screen.height - currentFingerPos.y;
			Vector2 vector = currentFingerPos - m_PreviousFingerPos;
			m_PreviousFingerPos = currentFingerPos;
			Vector2 posOnViewport = vector * m_PanSettings.m_OrthographicSettings.m_Sensitivity;
			Vector3 vector2 = Util_GetPositionOnPlane(posOnViewport);
			Vector3 in_position = vector2;
			if (!m_PreviousMovementDelta.Equals(Vector2.zero))
			{
				Vector3 vector3 = Util_GetPositionOnPlane(m_PreviousMovementDelta);
				in_position -= vector3;
			}
			m_PreviousMovementDelta = in_position;
			m_ScreenBounds.ClampToBounds(ref m_TheCamera, ref in_position);
			m_TheCamera.transform.position += in_position;
			Vector3 in_position2 = m_TheCamera.transform.position;
			m_ScreenBounds.ClampToBounds(ref m_TheCamera, ref in_position2);
			m_TheCamera.transform.position = in_position2;
		}

		private void MoveToFingerPos_Clamped(Vector2 currentFingerPos)
		{
			Vector2 vector = currentFingerPos;
			Vector3 in_position = m_TheCamera.ScreenToWorldPoint(new Vector3(vector.x, vector.y, Mathf.Abs(base.transform.position.z - m_TheCamera.transform.position.z)));
			m_ScreenBounds.ClampToBounds(ref m_TheCamera, ref in_position);
			m_TheCamera.transform.position = in_position;
		}
	}
}
