using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby
{
	public class ThemeIcons : ScriptableObject
	{
		[Serializable]
		public class ThemeIcon
		{
			public ThemePeriod period;

			public string spriteName;
		}

		[SerializeField]
		private UIAtlas m_atlas;

		[SerializeField]
		private List<ThemeIcon> m_icons = new List<ThemeIcon>();

		public string GetSpriteNameFor(ThemePeriod p)
		{
			ThemeIcon themeIcon = m_icons.Find((ThemeIcon ti) => ti.period == p);
			if (themeIcon == null)
			{
				throw new ApplicationException(string.Format("Theme period {0} does not have an icon", p.name));
			}
			return themeIcon.spriteName;
		}
	}
}
