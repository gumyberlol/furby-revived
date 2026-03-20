using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class PlayroomTheme
	{
		public Material m_WallMaterial;

		public Texture m_WallTexture;

		public Material m_InteriorMaterial;

		public Texture m_InteriorTexture;

		public void ReplaceTextures()
		{
			m_WallMaterial.SetTexture("_MainTex", m_WallTexture);
			m_InteriorMaterial.SetTexture("_MainTex", m_InteriorTexture);
			m_WallMaterial.SetTexture("_Texture", m_WallTexture);
			m_InteriorMaterial.SetTexture("_Texture", m_InteriorTexture);
		}

		public void Clear()
		{
		}
	}
}
