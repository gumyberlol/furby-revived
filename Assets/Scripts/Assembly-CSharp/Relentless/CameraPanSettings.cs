using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class CameraPanSettings
	{
		[SerializeField]
		public PerspectiveCameraPanSettings m_PerspectiveSettings;

		[SerializeField]
		public OrthographicCameraPanSettings m_OrthographicSettings;
	}
}
