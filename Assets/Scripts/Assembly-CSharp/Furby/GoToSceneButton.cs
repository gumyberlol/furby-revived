using Relentless;
using UnityEngine;

namespace Furby
{
	public class GoToSceneButton : MonoBehaviour
	{
		[SerializeField]
		private string m_targetScene;

		[SerializeField]
		private bool m_rememberBack = true;

		public void OnClick()
		{
			SpsScreenSwitcher screenSwitcher = FurbyGlobals.ScreenSwitcher;
			screenSwitcher.SwitchScreen(m_targetScene, m_rememberBack);
		}
	}
}
