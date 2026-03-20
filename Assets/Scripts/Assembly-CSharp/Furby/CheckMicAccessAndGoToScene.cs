using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class CheckMicAccessAndGoToScene : MonoBehaviour
	{
		[SerializeField]
		private MicAvailableChecker m_checker;

		[SerializeField]
		private Transform m_errorSpawnPoint;

		[LevelReferenceRootFolder("Furby/Scenes")]
		[SerializeField]
		private LevelReference m_targetScene;

		public void OnClick()
		{
			StartCoroutine(Check());
		}

		private IEnumerator Check()
		{
			bool waiting = true;
			bool available = false;
			yield return StartCoroutine(m_checker.Check(m_errorSpawnPoint, delegate(bool b)
			{
				waiting = false;
				available = b;
			}));
			while (waiting)
			{
				yield return null;
			}
			if (available)
			{
				bool includeInHistory = true;
				FurbyGlobals.ScreenSwitcher.SwitchScreen(m_targetScene, includeInHistory);
			}
		}
	}
}
