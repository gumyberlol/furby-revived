using System;
using System.Collections;
using UnityEngine;

namespace Furby
{
	public class FurballGuiGameController : CommonGuiGameController
	{
		[SerializeField]
		private GameObject m_returnPrompt;

		[SerializeField]
		private float m_showPromptDuration = 3f;

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			switch ((SharedGuiEvents)(object)enumValue)
			{
			case SharedGuiEvents.Pause:
				SetObjectStates(false);
				break;
			case SharedGuiEvents.Resume:
				SetObjectStates(true);
				break;
			case SharedGuiEvents.Restart:
				Application.LoadLevelAsync(Application.loadedLevelName);
				break;
			case SharedGuiEvents.Quit:
				StartCoroutine(DoFurballQuit());
				break;
			}
		}

		private IEnumerator DoFurballQuit()
		{
			ScreenOrientation currentOrientation = ScreenOrientationHelper.GetCurrentOrientation();
			ScreenOrientation desiredOrientation = ScreenOrientationHelper.GetCorrectOrientationForDevice_OrCustomizedOverride();
			if (currentOrientation != desiredOrientation && m_returnPrompt != null)
			{
				GameObject pauseMenuPanel = GameObject.Find("PauseMenuPanel");
				if (pauseMenuPanel != null)
				{
					pauseMenuPanel.SetActive(false);
				}
				SetObjectStates(false);
				m_returnPrompt.SetActive(true);
				Screen.orientation = desiredOrientation;
				float timeSinceStart = Time.realtimeSinceStartup;
				while (Time.realtimeSinceStartup - timeSinceStart < m_showPromptDuration)
				{
					yield return null;
				}
			}
			FurbyGlobals.ScreenSwitcher.BackToScreen("Playroom");
		}
	}
}
