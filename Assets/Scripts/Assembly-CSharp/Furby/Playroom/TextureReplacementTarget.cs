using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class TextureReplacementTarget
	{
		[SerializeField]
		public Material m_Material;

		[SerializeField]
		public Texture m_Texture;
	}
}
