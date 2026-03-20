using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Furby.Playroom;
using Furby.Utilities.FriendsBook;
using Furby.Utilities.Pantry;
using Relentless;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class EggProductUnlockingLogic : RelentlessMonoBehaviour
	{
		[SerializeField]
		public EggProductUnlockGroup[] m_UnlockGroups;

		[SerializeField]
		private List<string> m_IndirectedCodes = new List<string>();

		public SelectableFeatureList[] m_PlayroomFeatures;

		public PantryFoodDataList m_PantryData;

		[SerializeField]
		public List<EggProductUnlock> m_FurblingUnlocks;

		private List<EggProductUnlock> m_Unlocks = new List<EggProductUnlock>();

		private EggProductUnlock m_ActiveUnlock;

		public FurbyNamingData m_NamingData;

		public SceneReferences m_SceneReferences;

		public string m_ScannedQRCode = string.Empty;

		public string m_UnlockQRCode = string.Empty;

		public float m_TimeoutDurationSecs = 30f;

		public float m_DurationSecsBeforeTimeout;

		private GameEventSubscription m_DebugPanelSub;

		private bool CanQRCodeBeUsed(string qrCode)
		{
			if (Util_CodeIsRegistered(qrCode))
			{
				EggProductUnlockGroup eggProductUnlockGroup = null;
				EggProductUnlockGroup[] unlockGroups = m_UnlockGroups;
				foreach (EggProductUnlockGroup eggProductUnlockGroup2 in unlockGroups)
				{
					if (eggProductUnlockGroup2.m_ScannableQRCode.Equals(qrCode))
					{
						eggProductUnlockGroup = eggProductUnlockGroup2;
						break;
					}
				}
				if (eggProductUnlockGroup != null)
				{
					List<FurbyBabyTypeInfo> list = new List<FurbyBabyTypeInfo>();
					string[] unlockableQRCodes = eggProductUnlockGroup.m_UnlockableQRCodes;
					foreach (string qrCode2 in unlockableQRCodes)
					{
						EggProductUnlock eggProductUnlockFromCode = GetEggProductUnlockFromCode(qrCode2);
						FurbyBabyTypeInfo type = eggProductUnlockFromCode.m_FurblingSpecific.m_Type;
						list.Add(type);
					}
					int numFurbyBabies = Singleton<GameDataStoreObject>.Instance.Data.GetNumFurbyBabies();
					for (int k = 0; k < numFurbyBabies; k++)
					{
						FurbyBaby furbyBabyByIndex = Singleton<GameDataStoreObject>.Instance.Data.GetFurbyBabyByIndex(k);
						foreach (FurbyBabyTypeInfo item in list)
						{
							if (item.TypeID.Equals(furbyBabyByIndex.Type) && !furbyBabyByIndex.m_persistantData.CanBeGifted && furbyBabyByIndex.m_persistantData.FixedIncubationTime && furbyBabyByIndex.m_persistantData.PreAllocatedPersonality)
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}

		private bool DoesQRCodeExist(string qrCode)
		{
			foreach (EggProductUnlock unlock in m_Unlocks)
			{
				if (unlock.m_ScannableQRCode.Equals(qrCode))
				{
					return true;
				}
			}
			return false;
		}

		public EggProductUnlock GetEggProductUnlockFromCode(string qrCode)
		{
			foreach (EggProductUnlock unlock in m_Unlocks)
			{
				if (unlock.m_ScannableQRCode.Equals(qrCode))
				{
					return unlock;
				}
			}
			return null;
		}

		public List<EggProductUnlock> GetAllEggProductUnlocksFromCode(string qrCode)
		{
			List<EggProductUnlock> list = new List<EggProductUnlock>();
			foreach (EggProductUnlock unlock in m_Unlocks)
			{
				if (unlock.m_ScannableQRCode.Equals(qrCode))
				{
					list.Add(unlock);
				}
			}
			return list;
		}

		private void SetActiveUnlock(string qrCode, string variantCode)
		{
			Logging.Log("SetActiveUnlock: qrCode: [" + qrCode + "|" + variantCode + "]");
			foreach (EggProductUnlock unlock in m_Unlocks)
			{
				if (qrCode.Equals(unlock.m_ScannableQRCode) && variantCode.Equals(unlock.m_VariantCode))
				{
					m_ActiveUnlock = unlock;
				}
			}
			Util_UnlockItem(qrCode, variantCode);
			if (m_ActiveUnlock.m_UnlockType == UnlockType.EggWithPredefinedFlair)
			{
				UnlockAndActivateFurbingBaby();
			}
		}

		private void BuildListOfAcceptableCodes()
		{
			SelectableFeatureList[] playroomFeatures = m_PlayroomFeatures;
			foreach (SelectableFeatureList selectableFeatureList in playroomFeatures)
			{
				foreach (SelectableFeature item in selectableFeatureList)
				{
					if (item.IsUnlockedByScannedQRCode())
					{
						string qRCode = item.GetQRCode();
						if (DoesQRCodeExist(qRCode))
						{
							m_IndirectedCodes.Add(qRCode);
						}
						EggProductUnlock eggProductUnlock = new EggProductUnlock();
						eggProductUnlock.m_ScannableQRCode = qRCode;
						eggProductUnlock.m_VariantCode = item.GetVariantCode();
						eggProductUnlock.m_UiAtlas = item.GetUIAtlas();
						eggProductUnlock.m_SpriteName = item.GetSpriteName();
						eggProductUnlock.m_UnlockType = UnlockType.PlayroomItem;
						m_Unlocks.Add(eggProductUnlock);
					}
				}
			}
			foreach (PantryFoodData pantryDatum in m_PantryData)
			{
				if (pantryDatum.UnlockedByQRCode)
				{
					EggProductUnlock eggProductUnlock2 = new EggProductUnlock();
					eggProductUnlock2.m_ScannableQRCode = pantryDatum.QRUnlockCode;
					eggProductUnlock2.m_UiAtlas = pantryDatum.GraphicAtlas;
					eggProductUnlock2.m_SpriteName = pantryDatum.GraphicName;
					eggProductUnlock2.m_UnlockType = UnlockType.PantryItem;
					m_Unlocks.Add(eggProductUnlock2);
				}
			}
			m_Unlocks.AddRange(m_FurblingUnlocks);
		}

		public IEnumerator Start()
		{
			BuildListOfAcceptableCodes();
			yield return StartCoroutine(WaitForEventRouterToInitialize());
			yield return StartCoroutine(DoUnlockingSequence());
		}

		private IEnumerator WaitForEventRouterToInitialize()
		{
			while (!GameEventRouter.Exists)
			{
				yield return null;
			}
		}

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("QR Unlocking (State)"))
			{
				GUILayout.Label("Note: Won't show the burst screen...");
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("QR Code", GUILayout.Width(100f));
				GUILayout.Label("QR+Variant", GUILayout.Width(150f));
				GUILayout.Label("Scan Status", GUILayout.Width(200f));
				GUILayout.Label("Unlock Status", GUILayout.Width(200f));
				GUILayout.EndHorizontal();
				foreach (EggProductUnlock unlock in m_Unlocks)
				{
					bool flag = Util_CodeIsRegistered(unlock.m_ScannableQRCode);
					bool flag2 = Util_IsItemUnlocked(unlock.m_ScannableQRCode, unlock.m_VariantCode);
					GUILayout.BeginHorizontal();
					GUILayout.Label(unlock.m_ScannableQRCode, GUILayout.Width(100f));
					GUILayout.Label(unlock.GetQRCodeAndVariant(), GUILayout.Width(150f));
					if (GUILayout.Button((!flag) ? "Scan" : "Scanned, Forget", GUILayout.Width(200f)))
					{
						if (flag)
						{
							Singleton<GameDataStoreObject>.Instance.Data.RecognizedQRCodes.Remove(unlock.m_ScannableQRCode);
						}
						else
						{
							Singleton<GameDataStoreObject>.Instance.Data.RecognizedQRCodes.Add(unlock.m_ScannableQRCode);
						}
						Singleton<GameDataStoreObject>.Instance.Save();
					}
					if (GUILayout.Button((!flag2) ? "Unlock" : "Unlocked, Lock", GUILayout.Width(200f)))
					{
						if (flag2)
						{
							Singleton<GameDataStoreObject>.Instance.Data.QRItemsUnlocked.Remove(unlock.m_ScannableQRCode + unlock.m_VariantCode);
						}
						else
						{
							Singleton<GameDataStoreObject>.Instance.Data.QRItemsUnlocked.Add(unlock.m_ScannableQRCode + unlock.m_VariantCode);
						}
						Singleton<GameDataStoreObject>.Instance.Save();
					}
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			DebugPanel.EndSection();
			if (DebugPanel.StartSection("QR Unlocking (Trigger)") && Application.isPlaying && m_ScannedQRCode == string.Empty)
			{
				GUILayout.Space(50f);
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				GUILayout.Label("QR Code", GUILayout.Width(100f));
				GUILayout.Label("Item", GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();
				for (int i = 0; i < m_Unlocks.Count; i++)
				{
					EggProductUnlock eggProductUnlock = m_Unlocks[i];
					GUILayout.BeginHorizontal();
					if (GUILayout.Button(eggProductUnlock.m_ScannableQRCode, GUILayout.Width(100f)))
					{
						m_ScannedQRCode = eggProductUnlock.m_ScannableQRCode;
					}
					GUILayout.Label(eggProductUnlock.m_SpriteName.ToString(), GUILayout.ExpandWidth(true));
					GUILayout.EndHorizontal();
				}
				GUILayout.EndVertical();
			}
			DebugPanel.EndSection();
		}

		public void OnDestroy()
		{
			m_DebugPanelSub.Dispose();
			GameEventRouter.SendEvent(VideoCameraCommand.StopCapture);
		}

		private IEnumerator DoUnlockingSequence()
		{
			yield return null;
			Helper_ActivateObject(m_SceneReferences.m_BackButton);
			GameEventRouter.SendEvent(EggProductUnlockingEvents.ScanningForQRCode);
			Helper_DeactivateObject(m_SceneReferences.m_ContinueButton_ToPantry);
			Helper_DeactivateObject(m_SceneReferences.m_ContinueButton_ToDashboard);
			Helper_DeactivateObject(m_SceneReferences.m_ContinueButton_ToEggCarton);
			Helper_DeactivateObject(m_SceneReferences.m_ContinueButton_ToPlayroomViaHood);
			Helper_ActivateObject(m_SceneReferences.m_VideoFeed);
			Helper_ActivateObject(m_SceneReferences.m_ScanBar);
			Helper_ActivateObject(m_SceneReferences.m_ScanBlanker);
			Helper_ActivateObject(m_SceneReferences.m_ScanText);
			yield return StartCoroutine(HandleQRScan());
			GameEventRouter.SendEvent(EggProductUnlockingEvents.FoundValidQRCode);
			Helper_DeactivateObject(m_SceneReferences.m_VideoFeed);
			Helper_DeactivateObject(m_SceneReferences.m_ScanBar);
			Helper_DeactivateObject(m_SceneReferences.m_ScanText);
			bool requiresChoice = true;
			EggProductUnlockGroup targetGroup = null;
			EggProductUnlockGroup[] unlockGroups = m_UnlockGroups;
			foreach (EggProductUnlockGroup group in unlockGroups)
			{
				if (m_UnlockQRCode.Equals(group.m_ScannableQRCode))
				{
					targetGroup = group;
					break;
				}
			}
			if (targetGroup == null)
			{
				requiresChoice = m_IndirectedCodes.Contains(m_UnlockQRCode);
			}
			yield return StartCoroutine(ShowUnlockVFX(requiresChoice));
			if (targetGroup == null)
			{
				if (m_IndirectedCodes.Contains(m_UnlockQRCode))
				{
					yield return StartCoroutine(DoUnlockingSequence_Indirected());
				}
				else
				{
					yield return StartCoroutine(DoUnlockingSequence_OneToOne());
				}
			}
			else
			{
				yield return StartCoroutine(DoUnlockingSequence_Grouped(targetGroup));
			}
		}

		private IEnumerator DoUnlockingSequence_OneToOne()
		{
			SetActiveUnlock(m_UnlockQRCode, string.Empty);
			PopulateUnlockItem();
			yield return StartCoroutine(ActiveYouHaveUnlockedUI());
			GameObject startPantryFoodItem = new GameObject("StartPantryFoodItem");
			PantryFood_v03 foodItem = startPantryFoodItem.AddComponent<PantryFood_v03>();
			startPantryFoodItem.AddComponent<DontDestroyOnLoad>();
			PantryFoodData pantryItem = m_PantryData.Items.First((PantryFoodData pi) => pi.UnlockedByQRCode && pi.QRUnlockCode == m_ScannedQRCode);
			foodItem.FoodData = pantryItem;
		}

		private IEnumerator DoUnlockingSequence_Indirected()
		{
			List<EggProductUnlock> unlocks = GetAllEggProductUnlocksFromCode(m_UnlockQRCode);
			m_SceneReferences.m_SlotPopulator.SetInSlotMode(SlotMode.TwoSlots);
			for (int nth = 0; nth < unlocks.Count; nth++)
			{
				SlotReference slotRef = m_SceneReferences.m_SlotPopulator.SlotReferencesX2[nth];
				PopulateSlot(slotRef, unlocks[nth]);
			}
			yield return StartCoroutine(m_SceneReferences.m_SlotPopulator.WaitForActivationCodeToChange());
			SetActiveUnlock(m_SceneReferences.m_SlotPopulator.QRCode, m_SceneReferences.m_SlotPopulator.VariantCode);
			PopulateUnlockItem();
			yield return StartCoroutine(ActiveYouHaveUnlockedUI());
		}

		private IEnumerator DoUnlockingSequence_Grouped(EggProductUnlockGroup targetGroup)
		{
			m_SceneReferences.m_SlotPopulator.SetInSlotMode(SlotMode.ThreeSlots);
			for (int nth = 0; nth < targetGroup.m_UnlockableQRCodes.Length; nth++)
			{
				string thisQRCode = targetGroup.m_UnlockableQRCodes[nth];
				EggProductUnlock unlockMeta = GetEggProductUnlockFromCode(thisQRCode);
				SlotReference slotRef = m_SceneReferences.m_SlotPopulator.SlotReferencesX3[nth];
				PopulateSlot(slotRef, unlockMeta);
			}
			yield return StartCoroutine(m_SceneReferences.m_SlotPopulator.WaitForActivationCodeToChange());
			SetActiveUnlock(m_SceneReferences.m_SlotPopulator.QRCode, m_SceneReferences.m_SlotPopulator.VariantCode);
			PopulateUnlockItem();
			yield return StartCoroutine(ActiveYouHaveUnlockedUI());
		}

		private void PopulateSlot(SlotReference slotRunner, EggProductUnlock unlockMeta)
		{
			slotRunner.Handler.QRCode = unlockMeta.m_ScannableQRCode;
			slotRunner.Handler.VariantCode = unlockMeta.m_VariantCode;
			slotRunner.Sprite.atlas = unlockMeta.m_UiAtlas;
			slotRunner.Sprite.spriteName = unlockMeta.m_SpriteName;
			slotRunner.Sprite.MakePixelPerfect();
			switch (unlockMeta.m_UnlockType)
			{
			case UnlockType.EggWithPredefinedFlair:
				slotRunner.Sprite.transform.localScale *= 2.3f;
				slotRunner.Sprite.transform.localPosition = new Vector3(slotRunner.Sprite.transform.localPosition.x, slotRunner.Sprite.transform.localPosition.y + 40f, slotRunner.Sprite.transform.localPosition.z);
				break;
			case UnlockType.PantryItem:
				slotRunner.Sprite.transform.localScale *= 1.2f;
				break;
			case UnlockType.PlayroomItem:
				slotRunner.Sprite.transform.localScale *= 3f;
				break;
			}
			slotRunner.Collider.enabled = true;
			slotRunner.Handler.enabled = true;
		}

		private IEnumerator HandleQRScan()
		{
			WaitForGameEvent waiter = new WaitForGameEvent();
			CodeReader.ResultHandler setCode = delegate(CodeReader.Result r)
			{
				m_ScannedQRCode = r.text;
			};
			m_SceneReferences.m_CodeReader.CodeScanned += setCode;
			GameEventRouter.SendEvent(VideoCameraCommand.StartCapture);
			RestartScan();
			while (m_UnlockQRCode == string.Empty)
			{
				m_DurationSecsBeforeTimeout -= Time.deltaTime;
				if (m_DurationSecsBeforeTimeout <= 0f)
				{
					GameEventRouter.SendEvent(EggProductErrorEvents.TimeoutFailedToRead);
					Helper_DeactivateObject(m_SceneReferences.m_ScanBar);
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
					m_SceneReferences.m_CodeReader.CodeScanned += setCode;
				}
				if (m_ScannedQRCode != string.Empty)
				{
					m_SceneReferences.m_CodeReader.CodeScanned -= setCode;
					if (DoesQRCodeExist(m_ScannedQRCode))
					{
						if (CanQRCodeBeUsed(m_ScannedQRCode))
						{
							if (IsThereSpaceToUnlock(m_ScannedQRCode))
							{
								Util_RegisterCode(m_ScannedQRCode);
								m_UnlockQRCode = m_ScannedQRCode;
								if (m_UnlockQRCode == string.Empty)
								{
									GameEventRouter.SendEvent(EggProductErrorEvents.UnrecognizedQRCode);
									Helper_DeactivateObject(m_SceneReferences.m_ScanBar);
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
									m_SceneReferences.m_CodeReader.CodeScanned += setCode;
									yield return new WaitForSeconds(0.25f);
								}
							}
							else
							{
								base.gameObject.SendGameEvent(FriendsBookEvent.EggCartonFull);
								Helper_DeactivateObject(m_SceneReferences.m_ScanBar);
								yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
								RestartScan();
								m_SceneReferences.m_CodeReader.CodeScanned += setCode;
								yield return new WaitForSeconds(0.25f);
							}
						}
						else
						{
							GameEventRouter.SendEvent(EggProductErrorEvents.AlreadyUsedQRCode);
							Helper_DeactivateObject(m_SceneReferences.m_ScanBar);
							yield return StartCoroutine(waiter.WaitForEvent(SharedGuiEvents.DialogAccept, SharedGuiEvents.DialogCancel));
							RestartScan();
							m_SceneReferences.m_CodeReader.CodeScanned += setCode;
							yield return new WaitForSeconds(0.25f);
						}
					}
				}
				yield return null;
			}
		}

		private void RestartScan()
		{
			Helper_ActivateObject(m_SceneReferences.m_ScanBar);
			m_DurationSecsBeforeTimeout = m_TimeoutDurationSecs;
			m_UnlockQRCode = string.Empty;
			m_ScannedQRCode = string.Empty;
		}

		private IEnumerator ActiveYouHaveUnlockedUI()
		{
			GameEventRouter.SendEvent(EggProductUnlockingEvents.YouHaveUnlockedSequenceStarted);
			Helper_ActivateObject(m_SceneReferences.m_UnlockedItem);
			Animation targetAnimation = m_SceneReferences.m_UnlockedItem.GetComponent<Animation>();
			while (targetAnimation.isPlaying)
			{
				yield return null;
			}
			GameEventRouter.SendEvent(EggProductUnlockingEvents.YouHaveUnlockedSequenceEnded);
			switch (m_ActiveUnlock.m_UnlockType)
			{
			case UnlockType.PantryItem:
				Helper_ActivateObject(m_SceneReferences.m_ContinueButton_ToPantry);
				break;
			case UnlockType.PlayroomItem:
				Helper_ActivateObject(m_SceneReferences.m_ContinueButton_ToDashboard);
				break;
			case UnlockType.EggWithPredefinedFlair:
				Helper_ActivateObject(m_SceneReferences.m_ContinueButton_ToEggCarton);
				break;
			}
		}

		private IEnumerator ShowUnlockVFX(bool requiresChoice)
		{
			Helper_DeactivateObject(m_SceneReferences.m_BackButton);
			GameEventRouter.SendEvent(VideoCameraCommand.PauseCapture);
			GameEventRouter.SendEvent(EggProductUnlockingEvents.SwirlVFXSequenceStarted);
			if (requiresChoice)
			{
				GameEventRouter.SendEvent(EggProductUnlockingEvents.UnlockChoiceOffered);
			}
			else
			{
				GameEventRouter.SendEvent(EggProductUnlockingEvents.UnlockChoiceNotOffered);
			}
			Helper_ActivateObject(m_SceneReferences.m_VFXSpiral);
			Helper_ActivateObject(m_SceneReferences.m_VFXUnlock);
			yield return new WaitForSeconds(9.25f);
			Helper_DeactivateObject(m_SceneReferences.m_VFXSpiral);
			Helper_DeactivateObject(m_SceneReferences.m_VFXUnlock);
			GameEventRouter.SendEvent(EggProductUnlockingEvents.SwirlVFXSequenceEnded);
			Helper_DeactivateObject(m_SceneReferences.m_ScanBlanker);
			Helper_DeactivateObject(m_SceneReferences.m_VideoFeed);
			GameEventRouter.SendEvent(VideoCameraCommand.StopCapture);
		}

		private void UnlockAndActivateFurbingBaby()
		{
			FurbyBabyTypeInfo type = m_ActiveUnlock.m_FurblingSpecific.m_Type;
			FurbyBabyPersonality personality = m_ActiveUnlock.m_FurblingSpecific.m_Personality;
			FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(type.TypeID);
			FabricateRandomName(furbyBaby);
			furbyBaby.Iteration = type.Iteration;
			furbyBaby.SetPersonality(personality, furbyBaby.Level);
			furbyBaby.m_persistantData.flairs = m_ActiveUnlock.m_FurblingSpecific.m_Flair;
			furbyBaby.Progress = FurbyBabyProgresss.E;
			furbyBaby.m_persistantData.newToCarton = true;
			furbyBaby.m_persistantData.Attention = 1f;
			furbyBaby.m_persistantData.Cleanliness = 1f;
			furbyBaby.m_persistantData.Satiatedness = 1f;
			furbyBaby.m_persistantData.NewAttention = 1f;
			furbyBaby.m_persistantData.NewCleanliness = 1f;
			furbyBaby.m_persistantData.NewSatiatedness = 1f;
			furbyBaby.m_persistantData.TimeOfLastStatUpdate = 0L;
			furbyBaby.m_persistantData.IncubationTime = 0L;
			furbyBaby.m_persistantData.IncubationProgress = 0f;
			furbyBaby.m_persistantData.FixedIncubationTime = true;
			furbyBaby.m_persistantData.PreAllocatedPersonality = true;
			furbyBaby.m_persistantData.CanBeGifted = false;
		}

		private void FabricateRandomName(FurbyBaby baby)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			bool flag = true;
			while (flag)
			{
				text = m_NamingData.m_leftNames[UnityEngine.Random.Range(0, m_NamingData.m_leftNames.Length)];
				text2 = m_NamingData.m_rightNames[UnityEngine.Random.Range(0, m_NamingData.m_rightNames.Length)];
				flag = false;
				FurbyNamingData.DisallowedName[] disallowedNames = m_NamingData.m_disallowedNames;
				foreach (FurbyNamingData.DisallowedName disallowedName in disallowedNames)
				{
					if (disallowedName.m_disallowedLeft == text && disallowedName.m_disallowedRight == text2)
					{
						flag = true;
					}
				}
			}
			baby.NameLeft = text;
			baby.NameRight = text2;
		}

		private void PopulateUnlockItem()
		{
			m_SceneReferences.m_UnlockedItemTarget.atlas = m_ActiveUnlock.m_UiAtlas;
			m_SceneReferences.m_UnlockedItemTarget.spriteName = m_ActiveUnlock.m_SpriteName;
			m_SceneReferences.m_UnlockedItemTarget.MakePixelPerfect();
			switch (m_ActiveUnlock.m_UnlockType)
			{
			case UnlockType.EggWithPredefinedFlair:
				m_SceneReferences.m_UnlockedItemTarget.transform.localScale *= 2f;
				break;
			case UnlockType.PantryItem:
				m_SceneReferences.m_UnlockedItemTarget.transform.localScale *= 1.75f;
				break;
			case UnlockType.PlayroomItem:
				m_SceneReferences.m_UnlockedItemTarget.transform.localScale *= 3.5f;
				break;
			}
		}

		private static void Helper_ActivateObject(GameObject obj)
		{
			if (obj != null)
			{
				obj.SetActive(true);
			}
		}

		private static void Helper_DeactivateObject(GameObject obj)
		{
			if (obj != null)
			{
				obj.SetActive(false);
			}
		}

		private static void Util_RegisterCode(string qrCode)
		{
			if (!Util_CodeIsRegistered(qrCode))
			{
				Logging.Log("Util_RegisterCode: Registering qrCode: " + qrCode);
				Singleton<GameDataStoreObject>.Instance.Data.RecognizedQRCodes.Add(qrCode);
				Singleton<GameDataStoreObject>.Instance.Save();
			}
			else
			{
				Logging.Log("Util_RegisterCode: Already registered qrCode: " + qrCode);
			}
		}

		private static bool Util_CodeIsRegistered(string qrCode)
		{
			return Singleton<GameDataStoreObject>.Instance.Data.RecognizedQRCodes.Contains(qrCode);
		}

		private static void Util_UnlockItem(string qrCode, string variantCode)
		{
			if (!Util_IsItemUnlocked(qrCode, variantCode))
			{
				Logging.Log("Util_RegisterCode: Registering qrCode: " + qrCode + " -> variantCode: " + variantCode);
				Singleton<GameDataStoreObject>.Instance.Data.QRItemsUnlocked.Add(qrCode + variantCode);
				Singleton<GameDataStoreObject>.Instance.Save();
			}
			else
			{
				Logging.Log("Util_RegisterCode: Already registered qrCode: " + qrCode + " -> variantCode: " + variantCode);
			}
		}

		private static bool Util_IsItemUnlocked(string qrCode, string variantCode)
		{
			return Singleton<GameDataStoreObject>.Instance.Data.QRItemsUnlocked.Contains(qrCode + variantCode);
		}

		private bool IsThereSpaceToUnlock(string qrCode)
		{
			EggProductUnlockGroup eggProductUnlockGroup = null;
			EggProductUnlockGroup[] unlockGroups = m_UnlockGroups;
			foreach (EggProductUnlockGroup eggProductUnlockGroup2 in unlockGroups)
			{
				string[] unlockableQRCodes = eggProductUnlockGroup2.m_UnlockableQRCodes;
				foreach (string text in unlockableQRCodes)
				{
					if (text.Equals(qrCode))
					{
						eggProductUnlockGroup = eggProductUnlockGroup2;
						break;
					}
				}
			}
			if (eggProductUnlockGroup != null)
			{
				int num = FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count();
				int maxEggsInCarton = FurbyGlobals.BabyLibrary.GetMaxEggsInCarton();
				return num < maxEggsInCarton;
			}
			return true;
		}
	}
}
