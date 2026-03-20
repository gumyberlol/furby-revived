using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpMenuButton : MonoBehaviour
	{
		public HelpMenu m_helpMenu;

		private void Start()
		{
			if (!m_helpMenu.gameObject.activeInHierarchy)
			{
				m_helpMenu.gameObject.SetActive(true);
			}
			m_helpMenu.ShowMenu(false);
		}

		private void OnClick()
		{
			m_helpMenu.ShowMenu(true);
		}
	}
}
