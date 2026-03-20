using UnityEngine;

namespace Furby
{
	public class FurbyGUIController : MonoBehaviour
	{
		public bool m_gameHUDScreen;

		public bool m_pauseButton;

		public bool m_furbucks;

		public bool m_backScreen;

		public bool m_pauseMenuScreen;

		public bool m_endGame;

		public GameObject PauseTargetObject;

		public GameObject ResumeTargetObject;

		public GameObject MainTargetObject;

		public GameObject RestartTargetObject;

		private PlayMakerFSM PausePlayMaker;

		private PlayMakerFSM ResumePlayMaker;

		private PlayMakerFSM MainPlayMaker;

		private PlayMakerFSM RestartPlayMaker;

		private void Start()
		{
			FurbyGlobals.Overlay.eventManager.UpdateTarget(this);
			if (PauseTargetObject != null)
			{
				PausePlayMaker = null;
				PausePlayMaker = PauseTargetObject.GetComponent<PlayMakerFSM>();
				ResumePlayMaker = null;
				ResumePlayMaker = ResumeTargetObject.GetComponent<PlayMakerFSM>();
				MainPlayMaker = null;
				MainPlayMaker = MainTargetObject.GetComponent<PlayMakerFSM>();
				RestartPlayMaker = null;
				RestartPlayMaker = RestartTargetObject.GetComponent<PlayMakerFSM>();
			}
			reset(m_gameHUDScreen, FurbyGUISection.GameHUDScreen);
			reset(m_pauseButton, FurbyGUISection.PauseButton);
			reset(m_furbucks, FurbyGUISection.Furbucks);
			reset(m_backScreen, FurbyGUISection.BackScreen);
			reset(m_pauseMenuScreen, FurbyGUISection.PauseMenuScreen);
			reset(m_endGame, FurbyGUISection.EndGameScreen);
		}

		public void resetAll()
		{
			GameObject[] prefabbedScreens = FurbyGlobals.Overlay.m_prefabbedScreens;
			foreach (GameObject temp in prefabbedScreens)
			{
				FurbyGlobals.Overlay.Hide(temp);
			}
		}

		public void reset(bool setting, GameObject screen)
		{
			if (setting)
			{
				FurbyGlobals.Overlay.Show(screen);
			}
			else
			{
				FurbyGlobals.Overlay.Hide(screen);
			}
		}

		public void MENU_OPTION_PAUSE()
		{
			if (PausePlayMaker == null)
			{
				PauseTargetObject.SendMessage("MENU_OPTION_PAUSE");
			}
			else
			{
				PausePlayMaker.SendEvent("MENU_OPTION_PAUSE");
			}
		}

		public void MENU_OPTION_RESUME()
		{
			FurbyGlobals.Overlay.Hide(FurbyGUISection.PauseMenuScreen);
			if (ResumePlayMaker == null)
			{
				PauseTargetObject.SendMessage("MENU_OPTION_RESUME");
			}
			else
			{
				ResumePlayMaker.SendEvent("MENU_OPTION_RESUME");
			}
		}

		public void MENU_OPTION_MAINMENU()
		{
			if (MainPlayMaker == null)
			{
				RestartTargetObject.SendMessage("MENU_OPTION_MAINMENU");
			}
			else
			{
				RestartPlayMaker.SendEvent("MENU_OPTION_MAINMENU");
			}
		}

		public void MENU_OPTION_RESTART()
		{
			if (RestartPlayMaker == null)
			{
				RestartTargetObject.SendMessage("MENU_OPTION_RESTART");
			}
			else
			{
				RestartPlayMaker.SendEvent("MENU_OPTION_RESTART");
			}
		}

		public void LevelComplete(string endLevelText)
		{
			FurbyGlobals.Overlay.Show(FurbyGUISection.EndGameScreen);
			FurbyGlobals.Overlay.Hide(FurbyGUISection.PauseButton);
			FurbyGlobals.Overlay.Hide(FurbyGUISection.GameHUDScreen);
			FurbyGUISection.EndGameScreen.GetComponent<GuiEndLevelText>().setText(endLevelText);
		}

		public void OnPause()
		{
			FurbyGUISection.PauseMenuScreen.SetActive(true);
		}

		private void OnDisable()
		{
			resetAll();
		}
	}
}
