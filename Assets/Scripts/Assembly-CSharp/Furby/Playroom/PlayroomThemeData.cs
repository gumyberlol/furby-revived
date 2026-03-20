using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class PlayroomThemeData
	{
		public PlayroomItemState m_State;

		public string m_Name;

		public UIAtlas m_UIAtlas;

		public string m_SpriteName;

		public Material m_WallMaterial;

		public Texture m_WallTexture;

		public Material m_InteriorMaterial;

		public Texture m_InteriorTexture;

		public int m_cost;

		public string m_UnlockCode;

		public int m_ComAirTone;

		public ThemePeriod m_ThemePeriod;

		public string m_VariantCode;
	}
}
