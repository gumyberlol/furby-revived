using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class PerspectiveCameraZoomSettings
	{
		[SerializeField]
		public float m_MinPerspectiveFOV;

		[SerializeField]
		public float m_MaxPerspectiveFOV = 20f;

		[SerializeField]
		public float m_FOVZoomSpeed = 0.02f;
	}
}
