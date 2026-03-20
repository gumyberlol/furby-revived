using UnityEngine;

namespace Relentless
{
	public class CameraControllerPinchToZoom : CameraController
	{
		public CameraZoomSettings m_ZoomSettings = new CameraZoomSettings();

		public ZoomDefaults m_ZoomDefaults = new ZoomDefaults();

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
			m_ZoomDefaults.CaptureDefaults(m_CameraType, m_TheCamera);
			base.IsActive = true;
		}

		private void OnEnable()
		{
			FingerGestures.OnPinchBegin += Handle_OnPinchBegin;
			FingerGestures.OnPinchMove += Handle_OnPinchMove;
			FingerGestures.OnPinchEnd += Handle_OnPinchEnd;
		}

		private void OnDisable()
		{
			FingerGestures.OnPinchBegin -= Handle_OnPinchBegin;
			FingerGestures.OnPinchMove -= Handle_OnPinchMove;
			FingerGestures.OnPinchEnd -= Handle_OnPinchEnd;
		}

		private void Handle_OnPinchBegin(Vector2 fingerPos1, Vector2 fingerPos2)
		{
			Handle_OnPinchMove(fingerPos1, fingerPos2, 100f);
		}

		private void Handle_OnPinchMove(Vector2 fingerPos1, Vector2 fingerPos2, float delta)
		{
			if (base.IsActive)
			{
				if (m_CameraType == CameraType.Orthographic)
				{
					ApplyDeltaToOrthographicSize(delta, m_TheCamera);
				}
				if (m_CameraType == CameraType.Perspective)
				{
					ApplyDeltaToPerspectiveFOV(delta);
				}
			}
		}

		private void Handle_OnPinchEnd(Vector2 fingerPos1, Vector2 fingerPos2)
		{
		}

		private void ApplyDeltaToOrthographicSize(float delta, Camera theCamera)
		{
			if (m_CameraType == CameraType.Orthographic)
			{
				float orthographicZoomIncrement = m_ZoomSettings.m_OrthographicSettings.m_OrthographicZoomIncrement;
				float minOrthographicSize = m_ZoomSettings.m_OrthographicSettings.m_MinOrthographicSize;
				float maxOrthographicSize = m_ZoomSettings.m_OrthographicSettings.m_MaxOrthographicSize;
				float orthographicSize = theCamera.orthographicSize;
				float num = orthographicZoomIncrement * delta;
				float value = (orthographicSize += num);
				value = Mathf.Clamp(value, minOrthographicSize, maxOrthographicSize);
				theCamera.orthographicSize = value;
			}
		}

		private void ApplyDeltaToPerspectiveFOV(float delta)
		{
			if (m_CameraType != CameraType.Perspective)
			{
			}
		}
	}
}
