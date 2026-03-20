using System;
using UnityEngine;

namespace Relentless
{
	[Serializable]
	public class OrthographicCameraZoomSettings
	{
		[SerializeField]
		public float m_MinOrthographicSize = 0.5f;

		[SerializeField]
		public float m_MaxOrthographicSize = 1f;

		[SerializeField]
		public float m_OrthographicZoomIncrement = 0.001f;
	}
}
