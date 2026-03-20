using Furby.Utilities.Help;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class CrystalUpsellFindOutMoreExitHandler : MonoBehaviour
	{
		[HideInInspector]
		public HelpPanel m_helpPanel;

		public void OnClick()
		{
			base.gameObject.SendGameEvent(SharedGuiEvents.CrystalThemeLocked);
			m_helpPanel.gameObject.SetActive(false);
		}
	}
}
