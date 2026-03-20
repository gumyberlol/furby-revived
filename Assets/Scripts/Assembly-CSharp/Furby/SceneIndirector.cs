using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SceneIndirector : MonoBehaviour
	{
		[LevelReferenceRootFolder("Furby/Scenes")]
		public LevelReference m_Original;

		[LevelReferenceRootFolder("Furby/Scenes")]
		public LevelReference m_Crystal;

		public float m_DelayTimeSecs;

		private void OnEnable()
		{
			Invoke("InvokeSceneIndirection", m_DelayTimeSecs);
		}

		private static void SceneIndirectorLog(string logstring)
		{
			Logging.Log("<color=green> SceneIndirector: " + logstring + "</color>");
		}

		private void InvokeSceneIndirection()
		{
			SceneIndirectorLog("Traversing indirection [" + Application.loadedLevelName + "]");
			StartCoroutine(OnInvokeSceneIndirection());
		}

		private IEnumerator OnInvokeSceneIndirection()
		{
			yield return new WaitForSeconds(m_DelayTimeSecs);
			yield return StartCoroutine(WaitForSaveDataToLoad());
			yield return StartCoroutine(WaitForPreviousScreenSwitchToComplete());
			LevelReference levelRef = GetAppropriateLevelReference();
			SceneIndirectorLog("Conducting indirection to [" + levelRef.SceneName + "]");
			FurbyGlobals.ScreenSwitcher.SwitchScreen(GetAppropriateLevelReference(), false);
		}

		private IEnumerator WaitForPreviousScreenSwitchToComplete()
		{
			while (FurbyGlobals.ScreenSwitcher.IsSwitching())
			{
				yield return new WaitForEndOfFrame();
			}
		}

		private IEnumerator WaitForSaveDataToLoad()
		{
			while (!Singleton<GameDataStoreObject>.Exists)
			{
				yield return new WaitForEndOfFrame();
			}
			while (Singleton<GameDataStoreObject>.Instance.GlobalData == null)
			{
				yield return new WaitForEndOfFrame();
			}
		}

		private LevelReference GetAppropriateLevelReference()
		{
			GameData data = Singleton<GameDataStoreObject>.Instance.Data;
			switch (data.AppLookAndFeel)
			{
			case AppLookAndFeel.Crystal:
				return m_Crystal;
			case AppLookAndFeel.Normal:
				return m_Original;
			default:
				return m_Original;
			}
		}
	}
}
