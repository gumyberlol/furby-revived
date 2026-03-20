using System.Collections;
using Fabric;
using Relentless;
using Relentless.Network.Analytics;
using UnityEngine;

namespace Furby
{
	public class FurbyScreenSwitcher : SpsScreenSwitcher
	{
		[SerializeField]
		private float m_FadeTime = 0.25f;

		[SerializeField]
		private string m_blankLevelName = "DeliberatelyEmptyScene";

		[SerializeField]
		private GameObject m_LoadingScene;

		[SerializeField]
		private UIWidget m_FaderSprite;

		[SerializeField]
		private UIWidget[] m_alphaWidgets;

		[SerializeField]
		private LevelReference[] m_AbortLevels;

		[SerializeField]
		private GameObject m_NoFurbyModeLoadingScreen;

		[SerializeField]
		private GameObject m_FurbyModeLoadingScreen;

		[SerializeField]
		private float m_MinimumDurationIfShowingInteractionMessage = 5f;

		[SerializeField]
		private float m_MinimumDurationIfShowingUpsellMessage = 7f;

		private void Start()
		{
			base.gameObject.SetLayerInChildren(31);
		}

		public void OnApplicationPause(bool pausing)
		{
			if (!pausing || Application.isLoadingLevel)
			{
				return;
			}
			string loadedLevelName = Application.loadedLevelName;
			LevelReference[] abortLevels = m_AbortLevels;
			foreach (LevelReference levelReference in abortLevels)
			{
				if (levelReference.SceneName == loadedLevelName)
				{
					Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
					Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
					Singleton<FurbyDataChannel>.Instance.AutoConnect = true;
					Singleton<FurbyDataChannel>.Instance.DisconnectDetection = true;
					if (!MobileMovieTexturePlayer.AnyVideosPlaying())
					{
						BackToScreen("Main");
					}
					break;
				}
			}
		}

		protected override IEnumerator InternalSwitchScreen(string level)
		{
			GameEventRouter.SendEvent(SpsEvent.ExitScreen, null, Application.loadedLevelName, level);
			m_LoadingScene.SetActive(true);
			FurbyGlobals.HardwareSettingsScreenFlow.Disable();
			yield return new WaitForEndOfFrame();
			yield return StartCoroutine(FadeGameToLoadingScreen());
			LoadingScreenPresentation.LoadingScreenMode mode = FurbyGlobals.LoadingScreenPresentation.GetLoadingScreenMode();
			switch (mode)
			{
			case LoadingScreenPresentation.LoadingScreenMode.Plain:
				yield return StartCoroutine(LoadingScreenFlow(level));
				yield return StartCoroutine(FadeLoadingScreenToGame());
				break;
			case LoadingScreenPresentation.LoadingScreenMode.InteractionMessaging:
				yield return StartCoroutine(LoadingScreenFlow_FurbyMode(level));
				break;
			case LoadingScreenPresentation.LoadingScreenMode.UpsellMessaging:
				yield return StartCoroutine(LoadingScreenFlow_NoFurbyMode(level));
				break;
			}
			FurbyGlobals.LoadingScreenPresentation.MarkLoadingScreenAsViewed(mode);
			m_LoadingScene.SetActive(false);
			FurbyGlobals.HardwareSettingsScreenFlow.Enable();
		}

		private IEnumerator FadeGameToLoadingScreen()
		{
			GameEventRouter.SendEvent(FurbyScreenSwitchEvent.StartFadeDown);
			yield return StartCoroutine(FadeLoadingScene(0f, 1f, m_FadeTime));
			GameEventRouter.SendEvent(FurbyScreenSwitchEvent.StartLevelLoad);
		}

		private IEnumerator FadeLoadingScreenToGame()
		{
			GameEventRouter.SendEvent(FurbyScreenSwitchEvent.StartFadeup);
			yield return StartCoroutine(FadeLoadingScene(1f, 0f, m_FadeTime));
			GameEventRouter.SendEvent(FurbyScreenSwitchEvent.EndFadeup);
		}

		private IEnumerator LoadingScreenFlow(string nextScene)
		{
			SingletonInstance<TelemetryManager>.Instance.TagScreen(nextScene);
			string lastScene = Application.loadedLevelName;
			string loadingScene = m_blankLevelName;
			yield return Application.LoadLevelAsync(loadingScene);
			yield return Resources.UnloadUnusedAssets();
			yield return Application.LoadLevelAsync(nextScene);
			yield return Resources.UnloadUnusedAssets();
			GameEventRouter.SendEvent(SpsEvent.EnterScreen, null, lastScene, nextScene);
			yield return new WaitForEndOfFrame();
			GameEventRouter.SendEvent(FurbyScreenSwitchEvent.StartAssetBundlesLoad);
			if (AssetBundleHelpers.IsLoading())
			{
				float timeScale = Time.timeScale;
				Time.timeScale = 0f;
				EventManager.Instance.PostEvent("pause", EventAction.PauseSound);
				while (AssetBundleHelpers.IsLoading())
				{
					yield return new WaitForEndOfFrame();
				}
				Time.timeScale = timeScale;
				EventManager.Instance.PostEvent("pause", EventAction.UnpauseSound);
			}
		}

		private IEnumerator LoadingScreenFlow_FurbyMode(string level)
		{
			double start = AudioSettings.dspTime;
			Application.backgroundLoadingPriority = ThreadPriority.Low;
			m_FurbyModeLoadingScreen.SetActive(true);
			while (AudioSettings.dspTime - start < (double)m_MinimumDurationIfShowingInteractionMessage)
			{
				yield return null;
			}
			yield return StartCoroutine(LoadingScreenFlow(level));
			m_FurbyModeLoadingScreen.SetActive(false);
			yield return StartCoroutine(FadeLoadingScreenToGame());
			Application.backgroundLoadingPriority = ThreadPriority.Normal;
		}

		private IEnumerator LoadingScreenFlow_NoFurbyMode(string level)
		{
			double start = AudioSettings.dspTime;
			Application.backgroundLoadingPriority = ThreadPriority.Low;
			m_NoFurbyModeLoadingScreen.SetActive(true);
			while (AudioSettings.dspTime - start < (double)m_MinimumDurationIfShowingUpsellMessage)
			{
				yield return null;
			}
			yield return StartCoroutine(LoadingScreenFlow(level));
			m_NoFurbyModeLoadingScreen.SetActive(false);
			yield return StartCoroutine(FadeLoadingScreenToGame());
			Application.backgroundLoadingPriority = ThreadPriority.Normal;
		}

		private IEnumerator FadeLoadingScene(float alpha0, float alpha1, float span)
		{
			double start = AudioSettings.dspTime;
			double time = 0.0;
			do
			{
				float ratio = Mathf.Clamp01((float)time / span);
				float alpha2 = Mathf.Lerp(alpha0, alpha1, ratio);
				SetFadeAmount(alpha2);
				yield return new WaitForEndOfFrame();
				time = AudioSettings.dspTime - start;
			}
			while (time < (double)span);
			SetFadeAmount(alpha1);
		}

		private void SetFadeAmount(float alpha)
		{
			m_FaderSprite.alpha = alpha;
			UIWidget[] alphaWidgets = m_alphaWidgets;
			foreach (UIWidget uIWidget in alphaWidgets)
			{
				uIWidget.alpha = alpha;
			}
		}
	}
}
