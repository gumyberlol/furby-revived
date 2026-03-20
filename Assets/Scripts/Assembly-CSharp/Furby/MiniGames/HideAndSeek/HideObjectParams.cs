using System;

namespace Furby.MiniGames.HideAndSeek
{
	[Serializable]
	public class HideObjectParams
	{
		public int m_ObjectsAlongWidth = 10;

		public int m_ObjectsAlongHeight = 20;

		public float m_ObjectScale = 1.5f;

		public float m_ObjectRotation = 30f;
	}
}
