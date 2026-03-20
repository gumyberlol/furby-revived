using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class CameraZoomSettings
	{
		[SerializeField]
		public PerspectiveCameraZoomSettings m_PerspectiveSettings;

		[SerializeField]
		public OrthographicCameraZoomSettings m_OrthographicSettings;
	}
}
