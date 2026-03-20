using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class ComAirToneToBabyType
	{
		public string m_DisplayName = string.Empty;

		public int m_ComAirTone;

		public Texture2D m_EggTexture;

		public FurbyBabyTypeInfo m_FurbyType;
	}
}
