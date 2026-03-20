using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class AllSelectableThemes
	{
		[SerializeField]
		public List<SelectableTheme> m_Themes;

		public List<SelectableTheme> Themes
		{
			get
			{
				return m_Themes;
			}
			set
			{
				m_Themes = value;
			}
		}

		public int Count()
		{
			return Themes.Count;
		}
	}
}
