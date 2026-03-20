using Relentless;
using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpPanelCloseButton : MonoBehaviour
	{
		[HideInInspector]
		public HelpPanel m_helpPanel;

		[SerializeField]
		public bool m_SendCustomEvent;

		[SerializeField]
		public GameObject m_TargetOverride;

		public void OnClick()
		{
			if (m_SendCustomEvent)
			{
				base.gameObject.SendGameEvent(SharedGuiEvents.CrystalThemeLocked);
			}
			else
			{
				base.gameObject.SendGameEvent(SharedGuiEvents.DialogCancel);
			}
			if (m_TargetOverride == null)
			{
				Object.Destroy(m_helpPanel.gameObject);
			}
			else
			{
				m_TargetOverride.SetActive(false);
			}
		}
	}
}
