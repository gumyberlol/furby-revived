using System;

namespace Furby.Playroom
{
	[Serializable]
	public class ActiveSelectableThemes : AllSelectableThemes
	{
		public int m_ActiveIndex = -1;

		public void Select(int index)
		{
			EnableTheme();
			RemoveTheme(index);
		}

		private void EnableTheme()
		{
			if (m_ActiveIndex > -1)
			{
				m_Themes[m_ActiveIndex].RemoveFromScene();
			}
		}

		private void RemoveTheme(int index)
		{
			if (m_Themes.Count > 0)
			{
				m_Themes[index].AddToScene();
				m_ActiveIndex = index;
			}
		}
	}
}
