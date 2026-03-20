using Furby;
using UnityEngine;

public class SwitchDependingOnSettings : MonoBehaviour
{
	[SerializeField]
	private GameObject m_ObjectToHideIfCameFromSettings;

	[SerializeField]
	private GameObject m_ObjectToShowIfCameFromSettings;

	private void Start()
	{
		if (FurbyGlobals.SettingsHelper.CameFromSettings())
		{
			if (m_ObjectToShowIfCameFromSettings != null)
			{
				m_ObjectToShowIfCameFromSettings.SetActive(true);
			}
			if (m_ObjectToHideIfCameFromSettings != null)
			{
				m_ObjectToHideIfCameFromSettings.SetActive(false);
			}
		}
		else
		{
			if (m_ObjectToShowIfCameFromSettings != null)
			{
				m_ObjectToShowIfCameFromSettings.SetActive(false);
			}
			if (m_ObjectToHideIfCameFromSettings != null)
			{
				m_ObjectToHideIfCameFromSettings.SetActive(true);
			}
		}
		FurbyGlobals.SettingsHelper.ClearCameFromSettings();
	}
}
