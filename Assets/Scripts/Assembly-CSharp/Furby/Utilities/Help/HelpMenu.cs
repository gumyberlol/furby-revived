using System;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpMenu : MonoBehaviour
	{
		[Serializable]
		public class HelpDataGroup
		{
			public string m_localisedTitleKey;

			public HelpData[] m_helpData;
		}

		public Transform m_helpSpawnPoint;

		public HelpMenu m_helpMenu;

		public GameObject m_helpMenuItemPrefab;

		public GameObject m_helpMenuGroupTitleItemPrefab;

		public UIGrid m_itemGrid;

		private int m_gridOrder = 1000;

		public GameObject m_backgroundRoot;

		public GameObject m_contentsRoot;

		public HelpButton m_helpButton;

		public HelpDataGroup[] m_helpDataGroups;

		private bool m_menuSetup;

		public DisableOtherLayersForPooledObjects m_disableOtherLayers;

		public MobileMovieTexturePlayer m_moviePlayer;

		public UILabel m_VideoText_Caption;

		public GameObject m_SubtitleGameObject;

		public void AttachDelegate()
		{
			RemoveDelegate();
			GameEventRouter.AddDelegateForEnums(OnReceiveDialogClose, SharedGuiEvents.DialogCancel);
		}

		public void RemoveDelegate()
		{
			GameEventRouter.RemoveDelegateForEnums(OnReceiveDialogClose, SharedGuiEvents.DialogCancel);
		}

		private void OnReceiveDialogClose(Enum eventType, GameObject gameObject, params object[] paramList)
		{
			GameEventRouter.SendEvent(VideoPlayerGameEvents.RequestVideoStop);
			GameEventRouter.SendEvent(VideoPlayerGameEvents.VideoHasFinished);
		}

		public void SetupHelpMenu()
		{
			if (!m_menuSetup)
			{
				HelpDataGroup[] helpDataGroups = m_helpDataGroups;
				foreach (HelpDataGroup helpDataGroup in helpDataGroups)
				{
					GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_helpMenuGroupTitleItemPrefab, Vector3.zero, Quaternion.identity);
					gameObject.transform.parent = m_itemGrid.transform;
					gameObject.transform.localScale = Vector3.one;
					gameObject.name = m_gridOrder + "_HelpMenuGroupTitleItem";
					m_gridOrder++;
					HelpMenuGroupTitleItem component = gameObject.GetComponent<HelpMenuGroupTitleItem>();
					component.SetupMenuItem(helpDataGroup.m_localisedTitleKey);
					HelpData[] helpData = helpDataGroup.m_helpData;
					foreach (HelpData helpData2 in helpData)
					{
						GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(m_helpMenuItemPrefab, Vector3.zero, Quaternion.identity);
						gameObject2.transform.parent = m_itemGrid.transform;
						gameObject2.transform.localScale = Vector3.one;
						gameObject2.name = m_gridOrder + "_HelpMenuItem";
						m_gridOrder++;
						helpData2.m_moviePlayer = m_moviePlayer;
						helpData2.m_VideoText_Caption = m_VideoText_Caption;
						helpData2.m_SubtitleGameObject = m_SubtitleGameObject;
						HelpMenuItem component2 = gameObject2.GetComponent<HelpMenuItem>();
						component2.SetupMenuItem(helpData2, m_helpButton);
					}
				}
				m_menuSetup = true;
			}
			m_itemGrid.Reposition();
		}

		public void ShowMenu(bool show)
		{
			if (show)
			{
				AttachDelegate();
				m_contentsRoot.SetActive(true);
				m_backgroundRoot.SetActive(true);
				SetupHelpMenu();
				m_disableOtherLayers.DisableOtherLayers();
				SingletonInstance<ModalityMediator>.Instance.Acquire(this, null);
			}
			else
			{
				RemoveDelegate();
				m_disableOtherLayers.EnableOtherLayers();
				m_contentsRoot.SetActive(false);
				m_backgroundRoot.SetActive(false);
				SingletonInstance<ModalityMediator>.Instance.Release(this);
			}
		}
	}
}
