using Relentless;
using UnityEngine;

namespace Furby
{
	public class LaunchThemePopupDialog : MonoBehaviour
	{
		[SerializeField]
		private ThemePopupDialog m_prefab;

		[SerializeField]
		private Transform m_spawnPoint;

		private GameEventSubscription m_sub;

		public void Start()
		{
			GameEventRouter.EventHandler handler = delegate
			{
				ThemePopupDialog themePopupDialog = Object.Instantiate(m_prefab) as ThemePopupDialog;
				themePopupDialog.transform.parent = m_spawnPoint;
				themePopupDialog.transform.localScale = Vector3.one;
				themePopupDialog.transform.localPosition = Vector3.zero;
			};
			m_sub = new GameEventSubscription(handler, SettingsPageEvents.ChangeTheme);
		}

		public void OnDestroy()
		{
			m_sub.Dispose();
		}
	}
}
