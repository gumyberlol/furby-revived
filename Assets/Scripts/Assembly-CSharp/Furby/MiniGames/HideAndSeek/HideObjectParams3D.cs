using System;
using UnityEngine;

namespace Furby.MiniGames.HideAndSeek
{
	[Serializable]
	public class HideObjectParams3D
	{
		public int m_ObjectsAlongWidth = 10;

		public int m_ObjectsAlongHeight = 20;

		public int m_TopOddRowsToAvoid = 1;

		public int m_TopEvenRowsToAvoid = 1;

		public float m_ObjectScaleMin = 1.5f;

		public float m_ObjectScaleMax = 1.5f;

		public float m_ObjectRotation = 30f;

		public Vector3 m_PositionRandomizer = new Vector3(0f, 0f, 0f);
	}
}
