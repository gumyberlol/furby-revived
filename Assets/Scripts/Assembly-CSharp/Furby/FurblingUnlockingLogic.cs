using System;
using System.Collections;
using Furby.Playroom;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class FurblingUnlockingLogic : RelentlessMonoBehaviour
	{
		public SelectableFeatureList[] m_PlayroomFeatures;

		public ComAirToneToBabyType[] m_ComAirToneMap;

		public FurblingProductSceneReferences m_SceneReferences;

		private ComAirToneToBabyType m_UnlockTarget;

		public long m_ReceivedTone = -1L;

		public float m_TimeoutDurationSecs = 30f;

		public float m_DurationSecsBeforeTimeout;

		private GameEventSubscription m_DebugPanelSub;

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("ComAir Unlocking") && m_UnlockTarget == null)
			{
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Current Unlocks: ", RelentlessGUIStyles.Style_Header);
				GUILayout.Label(Singleton<GameDataStoreObject>.Instance.Data.RecognizedComAirTones.Count.ToString(), RelentlessGUIStyles.Style_Column);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Unlock Everything!", GUILayout.ExpandWidth(true)))
				{
					ComAirToneToBabyType[] comAirToneMap = m_ComAirToneMap;
					foreach (ComAirToneToBabyType comAirToneToBabyType in comAirToneMap)
					{
						Util_RegisterCode(comAirToneToBabyType.m_ComAirTone);
					}
					Singleton<GameDataStoreObject>.Instance.Save();
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10f);
				GUILayout.Label("Simulated ComAir Tones", RelentlessGUIStyles.Style_Header, GUILayout.Width(100f));
				GUILayout.BeginHorizontal();
				GUILayout.Label("ComAir", RelentlessGUIStyles.Style_Column, GUILayout.Width(100f));
				GUILayout.Label("Tribe", RelentlessGUIStyles.Style_Column, GUILayout.Width(200f));
				GUILayout.Label("Iteration", RelentlessGUIStyles.Style_Column);
				GUILayout.FlexibleSpace();
				GUILayout.Label("Unlock!", RelentlessGUIStyles.Style_Column);
				GUILayout.EndHorizontal();
				ComAirToneToBabyType[] comAirToneMap2 = m_ComAirToneMap;
				foreach (ComAirToneToBabyType comAirToneToBabyType2 in comAirToneMap2)
				{
					GUILayout.BeginHorizontal();
					GUILayout.Label(comAirToneToBabyType2.m_ComAirTone.ToString(), RelentlessGUIStyles.Style_Normal, GUILayout.Width(100f));
					GUILayout.Label(comAirToneToBabyType2.m_FurbyType.TypeID.Tribe.Name, RelentlessGUIStyles.Style_Normal, GUILayout.Width(200f));
					GUILayout.Label(comAirToneToBabyType2.m_FurbyType.TypeID.Iteration.ToString(), RelentlessGUIStyles.Style_Normal);
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(comAirToneToBabyType2.m_EggTexture, GUILayout.Width(60f), GUILayout.Height(60f)))
					{
						m_ReceivedTone = comAirToneToBabyType2.m_ComAirTone;
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			DebugPanel.EndSection();
		}

		public IEnumerator Start()
		{
			yield return StartCoroutine(WaitForEventRouterToInitialize());
			yield return StartCoroutine(ExecuteProductUnlockingSequence());
		}

		public void OnDestroy()
		{
			m_DebugPanelSub.Dispose();
			StopListeningForComAir();
			m_PlayroomFeatures = null;
			m_UnlockTarget = null;
			m_ComAirToneMap = null;
			ClearSceneReferences();
			Resources.UnloadUnusedAssets();
		}

		private void ClearSceneReferences()
		{
			m_SceneReferences.m_ItemCenter = null;
			m_SceneReferences.m_ItemRight = null;
			m_SceneReferences.m_ItemLeft = null;
			m_SceneReferences.m_EggTexture = null;
			if ((bool)m_SceneReferences.m_FurblingUnlockRoot)
			{
				m_SceneReferences.m_FurblingUnlockRoot.GetComponent<Animation>().Stop();
			}
			m_SceneReferences = null;
		}

		private void PrepareToReceiveComAir()
		{
			Singleton<FurbyDataChannel>.Instance.DisableCommunications = false;
			Singleton<FurbyDataChannel>.Instance.AutoConnect = false;
			ComAirChannel.ComAirTick += OnReceiveComAirToneCallback;
			m_ReceivedTone = -1L;
		}

		private void StopListeningForComAir()
		{
			Singleton<FurbyDataChannel>.Instance.AutoConnect = true;
			ComAirChannel.ComAirTick -= OnReceiveComAirToneCallback;
			m_ReceivedTone = -1L;
		}

		public void OnReceiveComAirToneCallback(ComAirChannel.Tone? receivedTone)
		{
			lock (this)
			{
				if (receivedTone.HasValue)
				{
					m_ReceivedTone = receivedTone.Value.Inbound;
				}
			}
		}

		private void UnlockEgg()
		{
			bool flag = false;
			bool flag2 = m_UnlockTarget.m_FurbyType.Tribe.TribeSet == Tribeset.CrystalGem;
			if (flag2 && !Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked)
			{
				Singleton<GameDataStoreObject>.Instance.UnlockCrystal();
				flag = true;
			}
			FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(m_UnlockTarget.m_FurbyType.TypeID, true);
			furbyBaby.Progress = FurbyBabyProgresss.E;
			if (flag2 && flag)
			{
				GameEventRouter.SendEvent(CrystalUnlockTelemetryEvents.CrystalUnlocked_ToyScan);
			}
		}

		private IEnumerator WaitForEventRouterToInitialize()
		{
			while (!GameEventRouter.Exists)
			{
				yield return null;
			}
		}

		private IEnumerator ExecuteProductUnlockingSequence()
		{
			yield return null;
			GameEventRouter.SendEvent(FurblingProductUnlockingEvents.WaitingForComAirTone);
			yield return StartCoroutine(ShowAndWaitForComAirRecog());
			GameEventRouter.SendEvent(FurblingProductUnlockingEvents.ReceivedComAirTone_ValidForUnlocking);
			UnlockEgg();
			Util_RegisterCode(m_UnlockTarget.m_ComAirTone);
			yield return StartCoroutine(ShowRecognizedComAirSequence());
			yield return StartCoroutine(ShowUnlockSuccessfulSequence());
		}

		private IEnumerator ShowAndWaitForComAirRecog()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			Helper_ActivateObject(m_SceneReferences.m_BackButton);
			Helper_DeactivateObject(m_SceneReferences.m_ContinueButton);
			Helper_ActivateObject(m_SceneReferences.m_FurblingUnlockRoot);
			m_SceneReferences.m_FurblingUnlockRoot.GetComponent<Animation>().Play(m_SceneReferences.m_LoopingAnim.name, PlayMode.StopSameLayer);
			Logging.Log("ShowAndWaitForComAirRecog:: Started waiting for a valid unlock target");
			m_ReceivedTone = -1L;
			PrepareToReceiveComAir();
			RestartScan();
			while (m_UnlockTarget == null)
			{
				m_DurationSecsBeforeTimeout -= Time.deltaTime;
				if (m_DurationSecsBeforeTimeout <= 0f)
				{
					GameEventRouter.SendEvent(FurblingProductErrorEvents.TimeoutFailedToRead);
					yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
					if (waiter.ReturnedEvent.Equals(SharedGuiEvents.DialogCancel))
					{
						FurbyGlobals.ScreenSwitcher.BackScreen();
						while (FurbyGlobals.ScreenSwitcher.IsSwitching())
						{
							yield return null;
						}
					}
					RestartScan();
				}
				if (m_ReceivedTone != -1)
				{
					ComAirToneToBabyType[] comAirToneMap = m_ComAirToneMap;
					foreach (ComAirToneToBabyType mapping in comAirToneMap)
					{
						if (mapping.m_ComAirTone == m_ReceivedTone)
						{
							m_UnlockTarget = mapping;
							Logging.Log("CompareReceivedToneAgainstExpected:: Success! It unlocks: " + m_UnlockTarget.m_DisplayName);
						}
					}
					if (m_UnlockTarget != null && FurbyGlobals.BabyRepositoryHelpers.IsEggCartonFull())
					{
						GameEventRouter.SendEvent(FurblingProductErrorEvents.EggCartonIsFull);
						yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
						if (waiter.ReturnedEvent.Equals(SharedGuiEvents.DialogCancel))
						{
							FurbyGlobals.ScreenSwitcher.BackScreen();
							while (FurbyGlobals.ScreenSwitcher.IsSwitching())
							{
								yield return null;
							}
						}
						RestartScan();
					}
				}
				yield return null;
			}
			StopListeningForComAir();
			m_SceneReferences.m_FurblingUnlockRoot.GetComponent<Animation>().Stop(m_SceneReferences.m_LoopingAnim.name);
		}

		private void RestartScan()
		{
			m_DurationSecsBeforeTimeout = m_TimeoutDurationSecs;
			m_ReceivedTone = -1L;
			m_UnlockTarget = null;
		}

		private IEnumerator ShowRecognizedComAirSequence()
		{
			PopulateUnlockItem();
			Helper_DeactivateObject(m_SceneReferences.m_BackButton);
			Helper_ActivateObject(m_SceneReferences.m_FurblingUnlockRoot);
			m_SceneReferences.m_FurblingUnlockRoot.GetComponent<Animation>().Play(m_SceneReferences.m_UnlockAnim.name, PlayMode.StopSameLayer);
			yield return new WaitForSeconds(10.5f);
		}

		private IEnumerator ShowUnlockSuccessfulSequence()
		{
			GameEventRouter.SendEvent(FurblingProductUnlockingEvents.YouHaveUnlockedSequenceStarted);
			Helper_ActivateObject(m_SceneReferences.m_LayoutAnimRoot);
			Animation targetAnimation = m_SceneReferences.m_LayoutAnimRoot.GetComponent<Animation>();
			while (targetAnimation.isPlaying)
			{
				yield return null;
			}
			GameEventRouter.SendEvent(FurblingProductUnlockingEvents.YouHaveUnlockedSequenceEnded);
			Helper_ActivateObject(m_SceneReferences.m_ContinueButton);
		}

		private void PopulateUnlockItem()
		{
			NewAssets[] array = new NewAssets[m_PlayroomFeatures.Length];
			int num = 0;
			SelectableFeatureList[] playroomFeatures = m_PlayroomFeatures;
			foreach (SelectableFeatureList selectableFeatureList in playroomFeatures)
			{
				foreach (SelectableFeature item in selectableFeatureList)
				{
					if ((item.IsUnlockedByComAirTone() || item.IsGoldenItemOrComAirTone()) && item.GetComAirTone() != 0 && m_UnlockTarget.m_ComAirTone == item.GetComAirTone())
					{
						array[num] = default(NewAssets);
						array[num].atlas = item.GetUIAtlas();
						array[num].spritename = item.GetSpriteName();
						num++;
					}
				}
			}
			if (array.Length >= 0)
			{
				m_SceneReferences.m_ItemLeft.spriteName = array[0].spritename;
				m_SceneReferences.m_ItemLeft.atlas = array[0].atlas;
				m_SceneReferences.m_ItemLeft.MakePixelPerfect();
				m_SceneReferences.m_ItemLeft.transform.localScale *= 1.75f;
			}
			if (array.Length >= 1)
			{
				m_SceneReferences.m_ItemCenter.spriteName = array[1].spritename;
				m_SceneReferences.m_ItemCenter.atlas = array[1].atlas;
				m_SceneReferences.m_ItemCenter.MakePixelPerfect();
				m_SceneReferences.m_ItemCenter.transform.localScale *= 1.75f;
			}
			if (array.Length >= 2)
			{
				m_SceneReferences.m_ItemRight.spriteName = array[2].spritename;
				m_SceneReferences.m_ItemRight.atlas = array[2].atlas;
				m_SceneReferences.m_ItemRight.MakePixelPerfect();
				m_SceneReferences.m_ItemRight.transform.localScale *= 1.75f;
			}
			m_SceneReferences.m_EggTexture.mainTexture = m_UnlockTarget.m_EggTexture;
			m_SceneReferences.m_EggTexture.MakePixelPerfect();
			m_SceneReferences.m_EggTexture.transform.localScale *= 1.5f;
		}

		private static void Util_RegisterCode(int code)
		{
			if (!Util_CodeIsRegistered(code))
			{
				Singleton<GameDataStoreObject>.Instance.Data.RecognizedComAirTones.Add(code);
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}

		private static bool Util_CodeIsRegistered(int code)
		{
			return Singleton<GameDataStoreObject>.Instance.Data.RecognizedComAirTones.Contains(code);
		}

		private static void Helper_ActivateObject(GameObject obj)
		{
			obj.SetActive(true);
		}

		private static void Helper_DeactivateObject(GameObject obj)
		{
			obj.SetActive(false);
		}
	}
}
