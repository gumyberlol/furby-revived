using UnityEngine;

namespace Relentless
{
	public class ZoomDefaults
	{
		public float m_DefaultOrthographicSize;

		public float m_DefaultFOV;

		public void CaptureDefaults(CameraType cameraType, Camera theCamera)
		{
			if (cameraType == CameraType.Orthographic)
			{
				m_DefaultOrthographicSize = theCamera.orthographicSize;
			}
			if (cameraType == CameraType.Perspective)
			{
				m_DefaultFOV = theCamera.fieldOfView;
			}
		}
	}
}
