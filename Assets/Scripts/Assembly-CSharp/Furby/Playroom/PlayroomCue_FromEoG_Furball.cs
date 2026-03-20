using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	public class PlayroomCue_FromEoG_Furball : RelentlessMonoBehaviour
	{
		[SerializeField]
		private GameObject m_returnPrompt;

		[SerializeField]
		private float m_showPromptDuration = 3f;

		protected void SetObjectStates(bool objectState)
		{
			Type typeFromHandle = typeof(BoxCollider);
			UnityEngine.Object[] array = UnityEngine.Object.FindObjectsOfType(typeFromHandle);
			for (int i = 0; i < array.Length; i++)
			{
				Collider collider = (Collider)array[i];
				collider.enabled = objectState;
			}
			Time.timeScale = ((!objectState) ? 0f : 1f);
		}

		private void OnClick()
		{
			if (GetComponent<SpsSwitchAction>() != null)
			{
				Logging.LogError("BIG ERROR. There is a switch action on this button which shouldn't be here. It will cause many problems (double loading of scene).");
			}
			Logging.Log("PlayroomCue_FromEoG::OnCLick");
			GameObject gameObject = new GameObject("SENTINEL_TransitionToHood");
			gameObject.AddComponent<PlayroomStartup_TransitionToHood>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
			StartCoroutine(HandleLoadingOfPlayroom());
		}

		protected virtual IEnumerator HandleLoadingOfPlayroom()
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
			Time.timeScale = 1f;
			string oldLevel = Application.loadedLevelName;
			GameEventRouter.SendEvent(SpsEvent.ExitScreen, null, oldLevel, "Playroom");
			yield return new WaitForEndOfFrame();
			yield return Application.LoadLevelAsync("Playroom");
			GameEventRouter.SendEvent(SpsEvent.EnterScreen, null, oldLevel, "Playroom");
		}
	}
}
