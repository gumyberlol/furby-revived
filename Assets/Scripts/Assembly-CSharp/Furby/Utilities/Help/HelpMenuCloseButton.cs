using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpMenuCloseButton : MonoBehaviour
	{
		public HelpMenu m_helpMenu;

		private void OnClick()
		{
			m_helpMenu.ShowMenu(false);
		}
	}
}
