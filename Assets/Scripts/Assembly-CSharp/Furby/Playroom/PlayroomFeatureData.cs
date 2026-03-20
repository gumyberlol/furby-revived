using System;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class PlayroomFeatureData
	{
		[SerializeField]
		public PlayroomItemState m_State;

		[SerializeField]
		public string m_Name;

		public UIAtlas m_UIAtlas;

		[SerializeField]
		public string m_SpriteName = string.Empty;

		[SerializeField]
		public string m_ObjectName;

		[SerializeField]
		public int m_Cost;

		[SerializeField]
		public string m_UnlockCode = string.Empty;

		[SerializeField]
		public int m_ComAirTone;

		[SerializeField]
		public ThemePeriod m_ThemePeriod;

		[SerializeField]
		public string m_VariantCode = string.Empty;
	}
}
