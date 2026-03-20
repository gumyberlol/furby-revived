using Relentless;
using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpMenuItem : MonoBehaviour
	{
		[HideInInspector]
		public HelpData m_helpData;

		public NGUILocaliser m_localiser;

		public UITexture m_iconTexture;

		[HideInInspector]
		public HelpButton m_helpButton;

		public void SetupMenuItem(HelpData helpData, HelpButton helpButton)
		{
			m_localiser.LocalisedStringKey = helpData.m_titleLocalisedKey;
			m_helpData = helpData;
			m_helpButton = helpButton;
			if (helpData.m_iconTexture != null)
			{
				m_iconTexture.mainTexture = helpData.m_iconTexture;
			}
			else
			{
				m_iconTexture.gameObject.SetActive(false);
			}
		}

		private void OnClick()
		{
			m_helpButton.AssignHelpData(m_helpData);
		}
	}
}
