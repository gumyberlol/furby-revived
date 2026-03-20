using System.Collections;
using System.Collections.Generic;
using Furby;
using UnityEngine;

namespace Relentless
{
	public class SpsScreenSwitcher : RelentlessMonoBehaviour
	{
		[SerializeField]
		private LevelReference m_startingBack;

		[SerializeField]
		private LevelReference m_stackClearScreen;

		public static bool s_SwitchOfAnyTypeIsBeingHandled;

		private Stack<string> m_backStack = new Stack<string>();

		private string m_previousScreen;

		private string m_currentScreen;

		private bool m_isSwitching;

		public void SwitchScreen(LevelReference level, bool storeBack)
		{
			SwitchScreen(level.SceneName, storeBack);
		}

		public void SwitchScreen(LevelReference level)
		{
			SwitchScreen(level.SceneName);
		}

		public void SwitchScreen(string level, bool storeBack)
		{
			if (m_isSwitching)
			{
				Logging.LogError("Error - trying to switch screens when already switching screens!");
				return;
			}
			if (storeBack)
			{
				m_backStack.Push(Application.loadedLevelName);
			}
			if (level == m_stackClearScreen.SceneName)
			{
				m_backStack.Clear();
				GameEventRouter.SendEvent(SpsEvent.GoBack);
			}
			else
			{
				GameEventRouter.SendEvent(SpsEvent.GoForward);
			}
			m_previousScreen = m_currentScreen;
			m_currentScreen = level;
			StartCoroutine(DoSwitchScreen(level));
		}

		public void SwitchScreen(string level)
		{
			SwitchScreen(level, true);
		}

		public void BackScreen()
		{
			if (m_backStack.Count != 0)
			{
				if (m_isSwitching)
				{
					Logging.LogError("Error - trying to switch screens when already switching screens!");
					return;
				}
				string text = m_backStack.Pop();
				m_previousScreen = m_currentScreen;
				m_currentScreen = text;
				GameEventRouter.SendEvent(SpsEvent.GoBack);
				StartCoroutine(DoSwitchScreen(text));
			}
		}

		public bool BackToScreen(string screenName)
		{
			while (m_backStack.Count > 1)
			{
				string text = m_backStack.Peek();
				if (text == screenName)
				{
					BackScreen();
					return true;
				}
				m_backStack.Pop();
			}
			SwitchScreen(screenName, false);
			return false;
		}

		private IEnumerator DoSwitchScreen(string level)
		{
			m_isSwitching = true;
			yield return StartCoroutine(InternalSwitchScreen(level));
			m_isSwitching = false;
			s_SwitchOfAnyTypeIsBeingHandled = false;
		}

		public bool IsSwitching()
		{
			return m_isSwitching;
		}

		protected virtual IEnumerator InternalSwitchScreen(string level)
		{
			string oldLevel = Application.loadedLevelName;
			GameEventRouter.SendEvent(SpsEvent.ExitScreen, null, oldLevel, level);
			yield return new WaitForEndOfFrame();
			AsyncOperation loadingOp = Application.LoadLevelAsync(level);
			if (FurbyGlobals.DeviceSettings.DeviceProperties.m_ApplicationModifiers.m_HaveBackgroundLoadingPriorityOverride)
			{
				Application.backgroundLoadingPriority = FurbyGlobals.DeviceSettings.DeviceProperties.m_ApplicationModifiers.m_BackgroundLoadingPriority;
			}
			yield return loadingOp;
			GameEventRouter.SendEvent(SpsEvent.EnterScreen, null, oldLevel, level);
		}

		private void Start()
		{
			GameEventRouter.SendEvent(SpsEvent.Initialise);
			if (m_startingBack.SceneName != string.Empty)
			{
				m_backStack.Push(m_startingBack.SceneName);
			}
			m_currentScreen = Application.loadedLevelName;
			GameEventRouter.SendEvent(SpsEvent.EnterScreen, null, string.Empty, m_currentScreen);
		}

		public void Clear()
		{
			while (m_backStack.Count > 1)
			{
				m_backStack.Pop();
			}
		}

		public string GetPreviousScreenName()
		{
			return m_previousScreen;
		}
	}
}
