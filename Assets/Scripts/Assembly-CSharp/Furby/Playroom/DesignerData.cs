using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class DesignerData
	{
		[SerializeField]
		public int m_DefaultProp;

		[SerializeField]
		public int m_DefaultRug;

		[SerializeField]
		public int m_DefaultWallArt;

		[SerializeField]
		public int m_DefaultDecor;

		[SerializeField]
		public int m_DefaultLightFixture;
	}
}
