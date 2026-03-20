using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class GUIScreenManager : RelentlessMonoBehaviour
	{
		public bool InitialiseOnStart = true;

		private List<GameObject> m_screenList;

		private GameObject m_currentScreen;

		private bool m_initialised;

		public string CurrentScreenName
		{
			get
			{
				return m_currentScreen.name;
			}
		}

		public void Start()
		{
			if (InitialiseOnStart)
			{
				Initialise();
			}
		}

		public void Initialise()
		{
			if (m_initialised)
			{
				return;
			}
			m_initialised = true;
			m_screenList = new List<GameObject>();
			foreach (Transform item in base.gameObject.transform)
			{
				m_screenList.Add(item.gameObject);
				item.gameObject.SetActive(false);
				foreach (Transform item2 in item.gameObject.transform)
				{
					item2.gameObject.SetActive(true);
				}
			}
		}

		public void SwitchScreen(GameObject screen)
		{
			if (m_currentScreen != null)
			{
				HideScreen(m_currentScreen);
			}
			m_currentScreen = screen;
			if (m_currentScreen != null)
			{
				ShowScreen(m_currentScreen);
			}
		}

		public bool SwitchScreen(string screenName)
		{
			return SwitchScreen(screenName, false);
		}

		public bool SwitchScreen(string screenName, bool hideCurrentIfNotFound)
		{
			foreach (GameObject screen in m_screenList)
			{
				if (string.Compare(screen.name, screenName) == 0)
				{
					SwitchScreen(screen);
					return true;
				}
			}
			if (hideCurrentIfNotFound && m_currentScreen != null)
			{
				HideScreen(m_currentScreen);
			}
			return false;
		}

		public void HideScreen(string screenName)
		{
			foreach (GameObject screen in m_screenList)
			{
				if (screen == null || !screen.activeSelf || string.Compare(screen.name, screenName) != 0)
				{
					continue;
				}
				HideScreen(screen);
				break;
			}
		}

		public void HideScreen(GameObject guiScreen)
		{
			if (m_currentScreen == guiScreen)
			{
				m_currentScreen = null;
			}
			if (guiScreen != null)
			{
				guiScreen.SetActive(false);
				IScreenViewModel screenViewModel = guiScreen.GetComponentInChildren(typeof(IScreenViewModel)) as IScreenViewModel;
				if (screenViewModel != null)
				{
					screenViewModel.OnHide();
				}
			}
		}

		private void ShowScreen(GameObject guiScreen)
		{
			if (!(guiScreen == null))
			{
				guiScreen.SetActive(true);
				IScreenViewModel screenViewModel = guiScreen.GetComponentInChildren(typeof(IScreenViewModel)) as IScreenViewModel;
				if (screenViewModel != null)
				{
					screenViewModel.OnShow();
				}
			}
		}

		public void DisableAll()
		{
			foreach (GameObject screen in m_screenList)
			{
				screen.SetActive(false);
			}
		}
	}
}
