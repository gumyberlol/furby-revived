using Relentless;
using UnityEngine;

namespace Furby.Utilities.Help
{
	public class HelpButton : MonoBehaviour
	{
		[SerializeField]
		private HelpData m_data;

		[SerializeField]
		private HelpGlobals m_globals;

		[SerializeField]
		private Transform m_spawnPoint;

		[SerializeField]
		private bool m_autoShowOnFirstPlay = true;

		public void Start()
		{
			CheckData();
			if (m_autoShowOnFirstPlay)
			{
				ShowOnFirstPlay();
			}
		}

		private void CheckData()
		{
			if (m_data == null)
			{
				Logging.LogWarning(string.Format("HelpButton \"{0}\" does not have a data.", base.gameObject.name));
			}
		}

		private void ShowOnFirstPlay()
		{
			string loadedLevelName = Application.loadedLevelName;
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			bool flag = !data.m_helpPanelsShown.Contains(loadedLevelName);
			if (false)
			{
				ShowHelp();
				data.m_helpPanelsShown.Add(loadedLevelName);
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		public void OnClick()
		{
			ShowHelp();
		}

		private void ShowHelp()
		{
			if (!(m_data == null))
			{
				HelpPanel helpPanel = Object.Instantiate(m_globals.m_panelPrefab) as HelpPanel;
				helpPanel.SetupFrom(m_data);
				helpPanel.transform.parent = m_spawnPoint;
				helpPanel.transform.localPosition = Vector3.zero;
				helpPanel.transform.localScale = Vector3.one;
				helpPanel.transform.localRotation = Quaternion.identity;
				Transform[] componentsInChildren = helpPanel.GetComponentsInChildren<Transform>();
				foreach (Transform transform in componentsInChildren)
				{
					transform.gameObject.layer = m_spawnPoint.gameObject.layer;
				}
			}
		}

		public void AssignHelpData(HelpData helpData)
		{
			m_data = helpData;
			ShowHelp();
		}
	}
}
