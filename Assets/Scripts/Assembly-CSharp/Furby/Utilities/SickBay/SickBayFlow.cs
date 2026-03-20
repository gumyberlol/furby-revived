using System;
using System.Collections;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.SickBay
{
	public class SickBayFlow : RelentlessMonoBehaviour
	{
		[Serializable]
		private class XrayForeground
		{
			public SickBayDisease Disease;

			public GameObject Foreground;
		}

		[Serializable]
		private class CureVFX
		{
			public string CureName = string.Empty;

			public GameObject VFX;
		}

		private const int NUM_CURE_INGREDIENTS = 2;

		[SerializeField]
		private float m_ForcedSicknessHappinessCutOff = 1f / 3f;

		[SerializeField]
		private SickBayIngredient[] m_Ingredients;

		private GameObject[] m_CureIngredients = new GameObject[2];

		[SerializeField]
		private GameObject m_GUIHintGraphics;

		[SerializeField]
		private GameObject m_VFX_CombineCure;

		[SerializeField]
		private GameObject m_VFX_Munching;

		[SerializeField]
		private GameObject[] m_CurePositions = new GameObject[2];

		private Vector3[] m_CureStartPositions = new Vector3[2];

		private Vector3[] m_CureStartScales = new Vector3[2];

		private Quaternion[] m_CureStartRotations = new Quaternion[2];

		[SerializeField]
		private GameObject m_MedicalCaseRoot;

		[SerializeField]
		private GameObject m_CombineButton;

		[SerializeField]
		private GameObject m_DiagnoseButton;

		[SerializeField]
		private GameObject m_CureButton;

		[SerializeField]
		private SickBayCureLibrary m_CureLibrary;

		[SerializeField]
		private GameObject m_Cure;

		[SerializeField]
		private Vector3 m_CureRevealPosition = Vector3.zero;

		private BounceItem m_CureBounceItem;

		private bool m_IsCureAvailable;

		private bool m_HasCureBeenSent;

		private SickBayDisease m_SicknessStatus;

		private SickBayCureData m_CurrentCureData;

		private bool m_ScanSucceeded;

		[SerializeField]
		private float m_CombineEffectWobbleTime = 3f;

		[SerializeField]
		private float m_CombineEffectTime = 3.5f;

		[SerializeField]
		private float m_BoredWaitTime = 10f;

		[SerializeField]
		private float m_ItemReactionTime = 2f;

		[SerializeField]
		private float m_CureEffectTime = 2f;

		[SerializeField]
		private float m_FurbyCuringTime = 2f;

		[SerializeField]
		private float m_DiagnosisScanTime = 2f;

		[SerializeField]
		private float m_DiagnosisChangeShowTime = 2f;

		[SerializeField]
		private float m_CureSentWaitTimeSicknessNotKnown = 4f;

		[SerializeField]
		private float m_CureSentWaitTimeSicknessIsKnown = 2f;

		[SerializeField]
		private ErrorMessageBox m_ErrorBox;

		[SerializeField]
		private GameObject m_TransitionCamera;

		[SerializeField]
		private float m_TransitionCameraTweenFromRotationX = 323.8429f;

		[SerializeField]
		private float m_TransitionCameraTweenToRotationX = 270f;

		[SerializeField]
		private Vector3 m_TransitionCameraTweenFromPosition = new Vector3(0f, -1.343489f, -0.5601f);

		[SerializeField]
		private Vector3 m_TransitionCameraTweenToPosition = new Vector3(0f, 0f, 0f);

		[SerializeField]
		private float m_TransitionCameraTweenTime = 1f;

		[SerializeField]
		private GameObject m_FurbyDiseaseMessageBackground;

		[SerializeField]
		private GameObject m_FurbyDiseaseMessageSinglePart;

		[SerializeField]
		private GameObject m_FurbyDiseaseMessageTwoPart;

		[SerializeField]
		private GameObject m_FurbyDiseaseReminderMessage;

		[SerializeField]
		private GameObject m_CureHintMessage;

		private FurbyComAirWaiter m_ComAirWaiter;

		[SerializeField]
		private GameObject m_XrayBackground;

		[SerializeField]
		private GameObject m_XrayReminderBackground;

		[SerializeField]
		private GameObject m_XrayScanline;

		[SerializeField]
		private AnimationClip m_CaseRemoveAnimation;

		[SerializeField]
		private AnimationClip m_CaseReturnAnimation;

		[SerializeField]
		private XrayForeground[] m_XrayForegrounds;

		[SerializeField]
		private XrayForeground[] m_XrayReminderForegrounds;

		[SerializeField]
		private HintState m_HintStateDiagnose = new HintState();

		public HintState m_HintStateSelectItem = new HintState();

		public HintState m_HintStateCombineItems = new HintState();

		public HintState m_HintStateSendCure = new HintState();

		[SerializeField]
		private CureVFX[] m_CureVFXs;

		[SerializeField]
		private GameObject m_ResetButton;

		private int m_CureHintIndex = -1;

		public bool m_GotSickbayEvent;

		public SickBayEvent m_RaisedEvent;

		private object[] m_SuppliedParameters;

		private GameEventSubscription m_DebugPanelSub;

		private bool m_ShouldForceDisease;

		private int m_ForcedDiseaseIndex;

		private SickBayDisease m_ForcedDisease;

		private void Awake()
		{
			if (m_MedicalCaseRoot == null)
			{
				Logging.LogError("No Medical Case Root Reference!");
			}
			if (m_GUIHintGraphics == null)
			{
				Logging.LogError("No GUI Hint Graphics Reference!");
			}
			if (m_CombineButton == null)
			{
				Logging.LogError("No Combine Button Reference!");
			}
			if (m_DiagnoseButton == null)
			{
				Logging.LogError("No Diagnose Button Reference!");
			}
			if (m_CureButton == null)
			{
				Logging.LogError("No Cure Button Reference!");
			}
			if (m_CureLibrary == null)
			{
				Logging.LogError("No Cure Library Reference!");
			}
			if (m_Cure == null)
			{
				Logging.LogError("No Cure Reference!");
			}
			BounceItem bounceItem = (m_CureBounceItem = m_Cure.GetComponentInChildren<BounceItem>());
			if (m_CureBounceItem == null)
			{
				Logging.LogError("No Cure BounceItem!");
			}
			bounceItem.OnGiven += OnCureGiven;
			bounceItem.OnBounce += OnCureBounced;
			bounceItem.OnFlicked += OnCureFlicked;
			bounceItem.OnTimeOut += OnCureTimedOut;
			if (m_TransitionCamera == null)
			{
				Logging.LogError("No Transition Camera Reference!");
			}
			if (m_FurbyDiseaseMessageSinglePart == null)
			{
				Logging.LogError("No Furby Disease Message Single Part Reference!");
			}
			if (m_FurbyDiseaseMessageTwoPart == null)
			{
				Logging.LogError("No Furby Disease Message Two Part Reference!");
			}
			m_ComAirWaiter = new FurbyComAirWaiter();
			if (m_XrayBackground == null)
			{
				Logging.LogError("No Xray Background Reference!");
			}
			if (m_XrayScanline == null)
			{
				Logging.LogError("No Xray Scanline Reference!");
			}
			m_CureStartPositions[0] = m_CurePositions[0].transform.position;
			m_CureStartPositions[1] = m_CurePositions[1].transform.position;
			m_CureStartScales[0] = m_CurePositions[0].transform.localScale;
			m_CureStartScales[1] = m_CurePositions[1].transform.localScale;
			m_CureStartRotations[0] = m_CurePositions[0].transform.rotation;
			m_CureStartRotations[1] = m_CurePositions[1].transform.rotation;
			if (m_ResetButton == null)
			{
				Logging.LogError("No Reset Button Reference!");
			}
			SickBayResetButton componentInChildren = m_ResetButton.GetComponentInChildren<SickBayResetButton>();
			if (componentInChildren == null)
			{
				Logging.LogError("No SickBay Reset Button on Reset Button!");
			}
			componentInChildren.OnReset += OnResetClicked;
		}

		private void ResetCommunications()
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
		}

		private void ReceiveEvent(Enum eventType, GameObject gameObject, params object[] paramList)
		{
			m_RaisedEvent = (SickBayEvent)(object)eventType;
			m_SuppliedParameters = paramList;
			m_GotSickbayEvent = true;
		}

		private IEnumerator Start()
		{
			SetSicknessStatus(SickBayDisease.Unknown);
			ResetCommunications();
			ResetCureSectionState();
			AttachDelegates();
			while (true)
			{
				if (!m_GotSickbayEvent)
				{
					m_HintStateDiagnose.TestAndBroadcastState();
					m_HintStateSelectItem.TestAndBroadcastState();
					m_HintStateCombineItems.TestAndBroadcastState();
					m_HintStateSendCure.TestAndBroadcastState();
					yield return null;
					continue;
				}
				SickBayEvent raisedEvent = m_RaisedEvent;
				if (raisedEvent != SickBayEvent.IngredientClicked)
				{
					if (raisedEvent != SickBayEvent.CombineIngredientsButtonClicked)
					{
						if (raisedEvent == SickBayEvent.DiagnoseButtonClicked)
						{
							m_HintStateDiagnose.Disable();
							if (FurbyGlobals.Player.NoFurbyOnSaveGame())
							{
								yield return StartCoroutine(ShowNoFurbyMessage());
							}
							else
							{
								yield return StartCoroutine(StartDiagnosisSection(false));
							}
						}
						else
						{
							Logging.LogError(string.Format("Unexpected event in SickBayFlow::Start : {0}", m_RaisedEvent.ToString()));
						}
					}
					else
					{
						m_HintStateCombineItems.Disable();
						yield return StartCoroutine(HandleCombineIngredients());
					}
				}
				else
				{
					m_HintStateDiagnose.Disable();
					HandleIngredientClicked((SickBayIngredient)m_SuppliedParameters[0]);
				}
				m_GotSickbayEvent = false;
			}
		}

		private void AttachDelegates()
		{
			GameEventRouter.AddDelegateForEnums(ReceiveEvent, SickBayEvent.IngredientClicked);
			GameEventRouter.AddDelegateForEnums(ReceiveEvent, SickBayEvent.CombineIngredientsButtonClicked);
			GameEventRouter.AddDelegateForEnums(ReceiveEvent, SickBayEvent.DiagnoseButtonClicked);
		}

		private void DetachDelegates()
		{
			GameEventRouter.RemoveDelegateForEnums(ReceiveEvent, SickBayEvent.IngredientClicked);
			GameEventRouter.RemoveDelegateForEnums(ReceiveEvent, SickBayEvent.CombineIngredientsButtonClicked);
			GameEventRouter.RemoveDelegateForEnums(ReceiveEvent, SickBayEvent.DiagnoseButtonClicked);
		}

		private void OnDisable()
		{
			DetachDelegates();
		}

		private void ResetCureSectionState()
		{
			TransitionCameraToCureSection();
			ClearCureIngredients();
			ShowGUIHintGraphics();
			HideCombineCureEffect();
			m_ErrorBox.Hide();
			HideCure();
			SetCurrentCureData(null);
			HideCombineButton();
			HideCureButton();
			ShowDiagnoseButton();
			HideResetButton();
			ShowSicknessReminderXrayIfStatusKnown();
			HideXrayBackground();
			HideXrayScanline();
			HideXrayForeground();
			HideFurbyDiseaseMessage();
			MoveMedicalCaseOnScreen();
			StopMunchVFX();
			GameEventRouter.SendEvent(SickBayEvent.CureSectionStarted, null, null);
			HintState hintState = ((m_SicknessStatus == SickBayDisease.Unknown && !FurbyGlobals.Player.NoFurbyOnSaveGame()) ? m_HintStateDiagnose : m_HintStateSelectItem);
			hintState.Enable();
			HintState hintState2 = ((hintState != m_HintStateSelectItem) ? m_HintStateSelectItem : m_HintStateDiagnose);
			hintState2.Disable();
			m_HintStateCombineItems.Disable();
			m_HintStateSendCure.Disable();
			m_GotSickbayEvent = false;
		}

		private void ShowResetButton()
		{
			m_ResetButton.SetActive(true);
		}

		private void HideResetButton()
		{
			m_ResetButton.SetActive(false);
		}

		private void StartMunchVFX()
		{
			GameEventRouter.SendEvent(SickBayEvent.CrossVFXStarted);
			m_VFX_Munching.animation.Play("VFX_sickMunchCross_emit_ANIM");
		}

		private void StopMunchVFX()
		{
			GameEventRouter.SendEvent(SickBayEvent.CrossVFXStopped);
			m_VFX_Munching.animation.Play("VFX_sickMunchCross_reset_ANIM");
		}

		private void ClearCureIngredients()
		{
			for (int i = 0; i < 2; i++)
			{
				if (m_CureIngredients[i] != null)
				{
					UnityEngine.Object.Destroy(m_CureIngredients[i]);
				}
				m_CureIngredients[i] = null;
			}
		}

		private void HandleIngredientClicked(SickBayIngredient clickedIngredient)
		{
			if (!clickedIngredient.IsCureIngredient())
			{
				AddCureIngredient(clickedIngredient);
			}
			else
			{
				RemoveCureIngredient(clickedIngredient);
			}
		}

		private void AddCureIngredient(SickBayIngredient ingredient)
		{
			if (AreCureSlotsAllFull())
			{
				return;
			}
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(ingredient.gameObject);
			SickBayIngredient component = gameObject.GetComponent<SickBayIngredient>();
			component.SetAsCureIngredient();
			for (int i = 0; i < 2; i++)
			{
				if (m_CureIngredients[i] == null)
				{
					m_CureIngredients[i] = gameObject;
					Transform transform = m_CurePositions[i].transform;
					gameObject.transform.position = transform.position;
					gameObject.transform.localScale = transform.localScale;
					gameObject.transform.rotation = transform.rotation;
					gameObject.transform.parent = transform;
					GameEventRouter.SendEvent(SickBayEvent.IngredientAdded, null, null);
					switch (component.GetIngredientType())
					{
					case SickBayIngredient.IngredientType.Cola:
						GameEventRouter.SendEvent(SickBayEvent.IngredientAddedCola, null, null);
						break;
					case SickBayIngredient.IngredientType.Honey:
						GameEventRouter.SendEvent(SickBayEvent.IngredientAddedHoney, null, null);
						break;
					case SickBayIngredient.IngredientType.HotWater:
						GameEventRouter.SendEvent(SickBayEvent.IngredientAddedHotWater, null, null);
						break;
					case SickBayIngredient.IngredientType.IceCube:
						GameEventRouter.SendEvent(SickBayEvent.IngredientAddedIceCube, null, null);
						break;
					case SickBayIngredient.IngredientType.Mints:
						GameEventRouter.SendEvent(SickBayEvent.IngredientAddedMints, null, null);
						break;
					}
					break;
				}
			}
			if (AreCureSlotsAllFull())
			{
				OnCureSlotsFull();
			}
			else
			{
				m_HintStateSelectItem.Enable();
			}
		}

		private bool AreCureSlotsAllFull()
		{
			for (int i = 0; i < 2; i++)
			{
				if (m_CureIngredients[i] == null)
				{
					return false;
				}
			}
			return true;
		}

		private void RemoveCureIngredient(SickBayIngredient ingredient)
		{
			bool flag = AreCureSlotsAllFull();
			GameObject gameObject = ingredient.gameObject;
			for (int i = 0; i < 2; i++)
			{
				if (m_CureIngredients[i] == gameObject)
				{
					m_CureIngredients[i] = null;
					UnityEngine.Object.Destroy(gameObject);
					GameEventRouter.SendEvent(SickBayEvent.IngredientRemoved, null, null);
					break;
				}
			}
			if (flag)
			{
				OnCureSlotsNotFullAnymore();
			}
		}

		private void OnCureSlotsFull()
		{
			GameEventRouter.SendEvent(SickBayEvent.IngredientSlotsBecameFull, null, null);
			m_HintStateSelectItem.Disable();
			m_HintStateCombineItems.Enable();
			ShowCombineButton();
		}

		private void OnCureSlotsNotFullAnymore()
		{
			GameEventRouter.SendEvent(SickBayEvent.IngredientSlotsBecameNotFull, null, null);
			m_HintStateSelectItem.Enable();
			m_HintStateCombineItems.Disable();
			HideCombineButton();
		}

		private IEnumerator HandleCombineIngredients()
		{
			MoveMedicalCaseOffScreen();
			HideSicknessReminderXray();
			HideDiagnoseButton();
			HideCombineButton();
			HideGUIHintGraphics();
			GameEventRouter.SendEvent(SickBayEvent.CombineIngredientsStarted, null, null);
			m_CurePositions[0].animation.Play();
			m_CurePositions[1].animation.Play();
			SickBayCureData cureToCreate = WorkoutCureFromIngredients();
			yield return new WaitForSeconds(m_CombineEffectWobbleTime);
			ShowCombineCureEffect();
			yield return new WaitForSeconds(0.2f);
			ClearCureIngredients();
			m_CurePositions[0].animation.Stop();
			m_CurePositions[1].animation.Stop();
			yield return null;
			m_CurePositions[0].transform.position = m_CureStartPositions[0];
			m_CurePositions[0].transform.localScale = m_CureStartScales[0];
			m_CurePositions[0].transform.rotation = m_CureStartRotations[0];
			m_CurePositions[1].transform.position = m_CureStartPositions[1];
			m_CurePositions[1].transform.localScale = m_CureStartScales[1];
			m_CurePositions[1].transform.rotation = m_CureStartRotations[1];
			yield return new WaitForSeconds(m_CombineEffectTime);
			ShowCure(cureToCreate);
			yield return new WaitForSeconds(1f);
			HideCombineCureEffect();
			ShowResetButton();
			GameEventRouter.SendEvent(SickBayEvent.CombineIngredientsFinished, null, null);
			yield return StartCoroutine(WaitForCureToBeSent());
		}

		private SickBayCureData WorkoutCureFromIngredients()
		{
			SickBayIngredient.IngredientType[] array = new SickBayIngredient.IngredientType[2];
			for (int i = 0; i < 2; i++)
			{
				GameObject gameObject = m_CureIngredients[i];
				SickBayIngredient component = gameObject.GetComponent<SickBayIngredient>();
				array[i] = component.GetIngredientType();
			}
			SickBayCureData sickBayCureData = null;
			foreach (SickBayCureData cureDatum in m_CureLibrary.m_CureData)
			{
				bool[] array2 = new bool[2];
				for (int j = 0; j < 2; j++)
				{
					array2[j] = false;
				}
				for (int k = 0; k < cureDatum.m_Ingredients.Length; k++)
				{
					for (int l = 0; l < 2; l++)
					{
						if (!array2[l] && array[l] == cureDatum.m_Ingredients[k])
						{
							array2[l] = true;
							break;
						}
					}
				}
				bool flag = true;
				for (int m = 0; m < 2; m++)
				{
					if (!array2[m])
					{
						flag = false;
						break;
					}
				}
				if (flag)
				{
					sickBayCureData = cureDatum;
					break;
				}
			}
			if (sickBayCureData == null)
			{
				Logging.LogError("No matching cure found in SickBayCureLibrary!");
			}
			return sickBayCureData;
		}

		private IEnumerator WaitForCureToBeSent()
		{
			StartCoroutine("BoredLoop");
			while (m_IsCureAvailable)
			{
				yield return null;
			}
			if (m_HasCureBeenSent)
			{
				HideResetButton();
				if (FurbyGlobals.Player.NoFurbyOnSaveGame())
				{
					yield return StartCoroutine(ShowNoFurbyMessage());
					ResetCureSectionState();
				}
				else
				{
					yield return StartCoroutine(DoCureSequence());
				}
			}
		}

		private IEnumerator BoredLoop()
		{
			m_HintStateSendCure.Enable();
			float timeNow = Time.time;
			float timeEnd = timeNow + m_BoredWaitTime;
			while (Time.time != timeEnd)
			{
				m_HintStateSendCure.TestAndBroadcastState();
				yield return null;
			}
			m_HintStateSendCure.Disable();
			GameEventRouter.SendEvent(SickBayEvent.FurbyBoredOfWaitingForCure, null, null);
			Logging.Log("Furby is bored waiting for the cure!");
		}

		private IEnumerator DoCureSequence()
		{
			Logging.Log("DoCureSequence");
			StartMunchVFX();
			if (IsSicknessStatusKnown())
			{
				yield return new WaitForSeconds(m_CureSentWaitTimeSicknessIsKnown);
			}
			else
			{
				yield return new WaitForSeconds(m_CureSentWaitTimeSicknessNotKnown);
			}
			yield return StartCoroutine(ScanSicknessStatus());
			if (DidScanSucceed())
			{
				yield return StartCoroutine(TransitionToDiagnosisSection());
				if (DoesCureMatchSickness())
				{
					yield return StartCoroutine(DoActuallyBeingCuredSequence());
				}
				else
				{
					yield return StartCoroutine(DoNotBeingCuredSequence());
				}
			}
			ResetCureSectionState();
		}

		private IEnumerator DoActuallyBeingCuredSequence()
		{
			Logging.Log("DoActuallyBeingCuredSequence");
			yield return StartCoroutine(SendItemReactionEvents(false));
			yield return StartCoroutine(SendCureEffectEvents(false));
			StopMunchVFX();
			ShowSicknessStatusXray();
			ShowSicknessStatusMessage();
			yield return StartCoroutine(CureFurby(true));
			if (m_ComAirWaiter.ReceivedResponse())
			{
				Logging.Log("Received Response");
				yield return StartCoroutine(StartDiagnosisSection(true));
			}
			else
			{
				Logging.Log("NO Response Received");
				yield return StartCoroutine(ShowCommunicationsErrorMessage());
			}
		}

		private IEnumerator DoNotBeingCuredSequence()
		{
			Logging.Log("DoNotBeingCuredSequence");
			yield return StartCoroutine(SendItemReactionEvents(true));
			bool recievedInitialResponse = m_ComAirWaiter.ReceivedResponse();
			if (!recievedInitialResponse)
			{
				yield return StartCoroutine(SendCureEffectEvents(true));
			}
			else
			{
				yield return StartCoroutine(SendCureEffectEvents(false));
			}
			StopMunchVFX();
			ShowSicknessStatusXray();
			ShowSicknessStatusMessage();
			yield return new WaitForSeconds(1f);
			if (recievedInitialResponse || m_ComAirWaiter.ReceivedResponse())
			{
				Logging.Log("At Least One Response Received");
				yield return StartCoroutine(StartDiagnosisSection(true));
			}
			else
			{
				Logging.Log("NO Response Received");
				yield return StartCoroutine(ShowCommunicationsErrorMessage());
			}
		}

		private IEnumerator SendItemReactionEvents(bool shouldWaitForResponse)
		{
			Logging.Log("Sending Item Reaction Events");
			if (m_CurrentCureData.m_ItemReactionAction == (FurbyAction)0)
			{
				GameEventRouter.SendEvent(SickBayEvent.FurbyReactingToItem, null, null);
				if (shouldWaitForResponse)
				{
					m_ComAirWaiter.OverrideReceivedResponse(false);
				}
			}
			else
			{
				GameEventRouter.SendEvent(SickBayEvent.FurbyReactingToItem, null, null);
				yield return StartCoroutine(SendComAirActionAndWaitForResponse(m_CurrentCureData.m_ItemReactionAction, shouldWaitForResponse));
			}
			yield return new WaitForSeconds(m_ItemReactionTime);
		}

		private IEnumerator SendCureEffectEvents(bool shouldWaitForResponse)
		{
			Logging.Log("Sending Cure Effect Events");
			GameEventRouter.SendEvent(SickBayEvent.FurbyReactingToEffectsOfCure, null, null);
			yield return StartCoroutine(SendComAirActionAndWaitForResponse(m_CurrentCureData.m_CureEffectAction, shouldWaitForResponse));
			yield return new WaitForSeconds(m_CureEffectTime);
		}

		private IEnumerator CureFurby(bool shouldWaitForResponse)
		{
			Logging.Log("Sending Furby Being Cured Event");
			GameEventRouter.SendEvent(SickBayEvent.FurbyBeingCured, null, null);
			yield return StartCoroutine(SendComAirCommandAndWaitForResponse(FurbyCommand.Cure, shouldWaitForResponse));
			yield return new WaitForSeconds(m_FurbyCuringTime);
		}

		private IEnumerator ShowCommunicationsErrorMessage()
		{
			HideResetButton();
			GameEventRouter.SendEvent(SickBayEvent.CommunicationsError, null, null);
			m_ErrorBox.SetOKState("FURBYCOMMSERROR_MESSAGE", "MENU_OPTION_OK", SickBayEvent.ErrorDialogOK);
			m_ErrorBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(SickBayEvent.ErrorDialogOK));
			m_ErrorBox.Hide();
		}

		private IEnumerator ShowNoFurbyMessage()
		{
			GameEventRouter.SendEvent(SickBayEvent.NoFurby, null, null);
			yield break;
		}

		private void MoveMedicalCaseOffScreen()
		{
			GameEventRouter.SendEvent(SickBayEvent.MedicalCaseMovedOffScreen);
			m_MedicalCaseRoot.animation.Play(m_CaseRemoveAnimation.name);
		}

		private void MoveMedicalCaseOnScreen()
		{
			GameEventRouter.SendEvent(SickBayEvent.MedicalCaseMovedOnScreen);
			m_MedicalCaseRoot.animation.Play(m_CaseReturnAnimation.name);
		}

		private void ShowCombineButton()
		{
			m_CombineButton.SetActive(true);
		}

		private void HideCombineButton()
		{
			m_CombineButton.SetActive(false);
		}

		private void ShowDiagnoseButton()
		{
			m_DiagnoseButton.SetActive(true);
		}

		private void HideDiagnoseButton()
		{
			m_DiagnoseButton.SetActive(false);
		}

		private void ShowCureButton()
		{
			m_CureButton.SetActive(true);
		}

		private void HideCureButton()
		{
			m_CureButton.SetActive(false);
		}

		private void ShowGUIHintGraphics()
		{
			m_GUIHintGraphics.SetActive(true);
		}

		private void HideGUIHintGraphics()
		{
			m_GUIHintGraphics.SetActive(false);
		}

		private void ShowCombineCureEffect()
		{
			m_VFX_CombineCure.SetActive(true);
			m_VFX_CombineCure.animation.Play();
		}

		private void HideCombineCureEffect()
		{
			m_VFX_CombineCure.SetActive(false);
		}

		private void ShowXrayReminderBackground()
		{
			m_XrayReminderBackground.SetActive(true);
		}

		private void HideXrayReminderBackground()
		{
			m_XrayReminderBackground.SetActive(false);
		}

		private void ShowXrayBackground()
		{
			m_XrayBackground.SetActive(true);
		}

		private void HideXrayBackground()
		{
			m_XrayBackground.SetActive(false);
		}

		private void ShowXrayScanline()
		{
			m_XrayScanline.SetActive(true);
		}

		private void HideXrayScanline()
		{
			m_XrayScanline.SetActive(false);
		}

		private void ShowXrayReminderForeground(SickBayDisease disease)
		{
			XrayForeground[] xrayReminderForegrounds = m_XrayReminderForegrounds;
			foreach (XrayForeground xrayForeground in xrayReminderForegrounds)
			{
				if (xrayForeground.Disease == disease)
				{
					xrayForeground.Foreground.SetActive(true);
				}
			}
		}

		private void HideXrayReminderForeground()
		{
			XrayForeground[] xrayReminderForegrounds = m_XrayReminderForegrounds;
			foreach (XrayForeground xrayForeground in xrayReminderForegrounds)
			{
				xrayForeground.Foreground.SetActive(false);
			}
		}

		private void ShowXrayForeground(SickBayDisease disease)
		{
			XrayForeground[] xrayForegrounds = m_XrayForegrounds;
			foreach (XrayForeground xrayForeground in xrayForegrounds)
			{
				if (xrayForeground.Disease == disease)
				{
					xrayForeground.Foreground.SetActive(true);
				}
			}
		}

		private void HideXrayForeground()
		{
			XrayForeground[] xrayForegrounds = m_XrayForegrounds;
			foreach (XrayForeground xrayForeground in xrayForegrounds)
			{
				xrayForeground.Foreground.SetActive(false);
			}
		}

		private void ShowCure(SickBayCureData cureData)
		{
			m_CureBounceItem.Activate(cureData.m_GraphicAtlas, cureData.m_GraphicName, m_CureRevealPosition);
			m_IsCureAvailable = true;
			m_HasCureBeenSent = false;
			CureVFX[] cureVFXs = m_CureVFXs;
			foreach (CureVFX cureVFX in cureVFXs)
			{
				if (!(cureVFX.CureName == cureData.m_CureName) || !(cureVFX.VFX != null))
				{
					continue;
				}
				cureVFX.VFX.SetActive(true);
				List<ParticleSystem> list = new List<ParticleSystem>();
				cureVFX.VFX.GetComponentsInChildrenIncludeInactive(list);
				foreach (ParticleSystem item in list)
				{
					item.Play();
				}
			}
			SetCurrentCureData(cureData);
		}

		private void HideCure()
		{
			m_CureBounceItem.Deactivate();
			m_IsCureAvailable = false;
			m_HasCureBeenSent = false;
			CureVFX[] cureVFXs = m_CureVFXs;
			foreach (CureVFX cureVFX in cureVFXs)
			{
				if (!(cureVFX.VFX != null))
				{
					continue;
				}
				List<ParticleSystem> list = new List<ParticleSystem>();
				cureVFX.VFX.GetComponentsInChildrenIncludeInactive(list);
				foreach (ParticleSystem item in list)
				{
					item.Stop();
					item.Clear();
				}
				cureVFX.VFX.SetActive(false);
			}
		}

		private void OnCureGiven()
		{
			Logging.Log("OnCureGiven");
			GameEventRouter.SendEvent(SickBayEvent.CureGiven, null, null);
			HideCure();
			m_HasCureBeenSent = true;
			StopBoredLoop();
		}

		private void StopBoredLoop()
		{
			StopCoroutine("BoredLoop");
			m_HintStateSendCure.Disable();
			GameEventRouter.SendEvent(HintEvents.DeactivateAll);
		}

		private void OnCureBounced()
		{
			GameEventRouter.SendEvent(SickBayEvent.CureBounced, null, null);
		}

		private void OnCureFlicked()
		{
			GameEventRouter.SendEvent(SickBayEvent.CureFlicked, null, null);
		}

		private void OnCureTimedOut()
		{
			Logging.Log("OnCureTimedOut");
			GameEventRouter.SendEvent(SickBayEvent.CureTimedOut, null, null);
			StopBoredLoop();
			ResetCureSectionState();
		}

		private void OnResetClicked()
		{
			Logging.Log("OnResetClicked");
			StopBoredLoop();
			ResetCureSectionState();
		}

		private void SetSicknessStatus(SickBayDisease sicknessStatus)
		{
			m_SicknessStatus = sicknessStatus;
			Logging.Log(string.Format("Sickness Status: {0}", sicknessStatus.ToString()));
		}

		private bool IsSicknessStatusKnown()
		{
			return m_SicknessStatus != SickBayDisease.Unknown;
		}

		private bool IsFurbySick()
		{
			return IsSicknessStatusKnown() && m_SicknessStatus != SickBayDisease.Nothing;
		}

		private void SetCurrentCureData(SickBayCureData cureData)
		{
			m_CurrentCureData = cureData;
		}

		private bool DoesCureMatchSickness()
		{
			if (!IsFurbySick())
			{
				Logging.Log("Furby is NOT Sick");
				return false;
			}
			bool flag = m_CurrentCureData.m_CuredDisease == m_SicknessStatus;
			Logging.Log(string.Format("Cure: {0} {1} Sickness: {2}", m_CurrentCureData.m_CuredDisease, (!flag) ? "DOES NOT MATCH" : "MATCHES", m_SicknessStatus));
			return flag;
		}

		private IEnumerator TransitionToDiagnosisSection()
		{
			HideSicknessReminderXray();
			HideDiagnoseButton();
			HideCombineButton();
			TransitionCameraToDiagnosisSection();
			yield return new WaitForSeconds(m_TransitionCameraTweenTime);
		}

		private void ShowSicknessReminderXrayIfStatusKnown()
		{
			if (IsSicknessStatusKnown())
			{
				ShowSicknessReminderXray();
			}
			else
			{
				HideSicknessReminderXray();
			}
		}

		private void ShowSicknessReminderXray()
		{
			ShowXrayReminderBackground();
			if (IsFurbySick())
			{
				ShowXrayReminderForeground(m_SicknessStatus);
				ShowCureHintMessage();
			}
			else
			{
				HideXrayReminderForeground();
				HideCureHintMessage();
			}
			ShowSicknessStatusReminderMessage();
		}

		private void HideSicknessReminderXray()
		{
			HideXrayReminderBackground();
			HideXrayReminderForeground();
			HideFurbyDiseaseReminderMessage();
			HideCureHintMessage();
		}

		private void ShowSicknessStatusReminderMessage()
		{
			if (IsFurbySick())
			{
				ShowFurbyDiseaseReminderMessage(GetFurbyDiseaseString2());
			}
			else
			{
				ShowFurbyDiseaseReminderMessage("SICKBAY_ILLNESS_HEALTHY");
			}
		}

		private void ShowSicknessStatusXray()
		{
			ShowXrayBackground();
			if (IsFurbySick())
			{
				ShowXrayForeground(m_SicknessStatus);
			}
			else
			{
				HideXrayForeground();
			}
		}

		private void ShowSicknessStatusMessage()
		{
			if (IsFurbySick())
			{
				ShowFurbyDiseaseMessage(GetFurbyDiseaseString1(), GetFurbyDiseaseString2());
			}
			else
			{
				ShowFurbyDiseaseMessage("SICKBAY_SCAN_HEALTHY");
			}
		}

		private IEnumerator StartDiagnosisSection(bool isAutoReScan)
		{
			if (!isAutoReScan)
			{
				yield return StartCoroutine(TransitionToDiagnosisSection());
				ShowXrayBackground();
			}
			GameEventRouter.SendEvent(SickBayEvent.DiagnosisSectionStarted, null, null);
			ShowXrayScanline();
			ShowFurbyDiseaseMessage("SICKBAY_SCAN_DIAGNOSING");
			bool wasFurbySick = IsFurbySick();
			if (isAutoReScan)
			{
				if (!IsSicknessStatusKnown())
				{
					Logging.LogError("Not expected to get to Auto Re-Scan state without knowing what the Sickness Status is!");
				}
				if (!IsFurbySick())
				{
					GameEventRouter.SendEvent(SickBayEvent.FurbyWasntSickBeforeCureGiven, null, null);
				}
				else if (DoesCureMatchSickness())
				{
					SetSicknessStatus(SickBayDisease.Nothing);
				}
			}
			else
			{
				yield return StartCoroutine(ScanSicknessStatus());
				if (!DidScanSucceed())
				{
					ResetCureSectionState();
					yield break;
				}
			}
			yield return new WaitForSeconds(m_DiagnosisScanTime);
			HideXrayScanline();
			ShowSicknessStatusXray();
			if (IsFurbySick())
			{
				GameEventRouter.SendEvent(SickBayEvent.DiagnosisSick, null, null);
			}
			else
			{
				GameEventRouter.SendEvent(SickBayEvent.DiagnosisWell, null, null);
			}
			if (isAutoReScan)
			{
				if (wasFurbySick)
				{
					if (!IsFurbySick())
					{
						GameEventRouter.SendEvent(SickBayEvent.FurbyWasCured, null, null);
						ShowFurbyDiseaseMessage("SICKBAY_SCAN_CURED");
					}
					else
					{
						ShowFurbyDiseaseMessage("SICKBAY_SCAN_STILLSICK");
					}
				}
				yield return new WaitForSeconds(m_DiagnosisChangeShowTime);
			}
			ShowSicknessStatusMessage();
			ShowCureButton();
			WaitForGameEvent waiter = new WaitForGameEvent();
			bool waitingForClick = true;
			while (waitingForClick)
			{
				yield return StartCoroutine(waiter.WaitForEvent(SickBayEvent.CureButtonClicked));
				SickBayEvent sickBayEvent = (SickBayEvent)(object)waiter.ReturnedEvent;
				if (sickBayEvent == SickBayEvent.CureButtonClicked)
				{
					waitingForClick = false;
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in SickBayFlow::StartDiagnosisSection : {0}", sickBayEvent.ToString()));
				}
			}
			ResetCureSectionState();
		}

		private IEnumerator ScanSicknessStatus()
		{
			m_ScanSucceeded = false;
			if (!IsSicknessStatusKnown())
			{
				bool shouldWaitForResponse = true;
				yield return StartCoroutine(SendComAirCommandAndWaitForResponse(FurbyCommand.Status, shouldWaitForResponse));
				if (m_ComAirWaiter.ReceivedResponse() || m_ShouldForceDisease)
				{
					if (!m_ShouldForceDisease)
					{
						bool isSick = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Sickness > 0;
						bool isHappinessBelowThreshold = FurbyGlobals.Player.Happiness <= m_ForcedSicknessHappinessCutOff;
						if (isSick || isHappinessBelowThreshold)
						{
							SetSicknessStatus(GenerateRandomDisease());
						}
						else
						{
							SetSicknessStatus(SickBayDisease.Nothing);
						}
					}
					else
					{
						SetSicknessStatus(m_ForcedDisease);
					}
				}
				else
				{
					Logging.Log("NO Sickness Status Response Received");
					yield return StartCoroutine(ShowCommunicationsErrorMessage());
				}
				m_ScanSucceeded = m_ComAirWaiter.ReceivedResponse();
			}
			else
			{
				m_ScanSucceeded = true;
			}
		}

		private bool DidScanSucceed()
		{
			return m_ScanSucceeded;
		}

		private SickBayDisease GenerateRandomDisease()
		{
			return (SickBayDisease)UnityEngine.Random.Range(2, 7);
		}

		private string GetLocalisedMessage(string message)
		{
			string text = Singleton<Localisation>.Instance.GetText(message);
			if (text.Contains("{0}"))
			{
				text = string.Format(text, FurbyGlobals.Player.FullName);
			}
			return text;
		}

		private void ShowFurbyDiseaseReminderMessage(string message)
		{
			m_FurbyDiseaseReminderMessage.SetActive(true);
			UILabel[] componentsInChildren = m_FurbyDiseaseReminderMessage.GetComponentsInChildren<UILabel>();
			componentsInChildren[0].text = GetLocalisedMessage(message);
		}

		private void HideFurbyDiseaseReminderMessage()
		{
			m_FurbyDiseaseReminderMessage.SetActive(false);
		}

		private void ShowCureHintMessage()
		{
			m_CureHintMessage.SetActive(true);
			string cureHintMessageString = GetCureHintMessageString();
			UILabel[] componentsInChildren = m_CureHintMessage.GetComponentsInChildren<UILabel>();
			componentsInChildren[0].text = GetLocalisedMessage(cureHintMessageString);
		}

		private void HideCureHintMessage()
		{
			m_CureHintMessage.SetActive(false);
		}

		private void ShowFurbyDiseaseMessage(string message)
		{
			m_FurbyDiseaseMessageBackground.SetActive(true);
			m_FurbyDiseaseMessageSinglePart.SetActive(true);
			m_FurbyDiseaseMessageTwoPart.SetActive(false);
			UILabel[] componentsInChildren = m_FurbyDiseaseMessageSinglePart.GetComponentsInChildren<UILabel>();
			componentsInChildren[0].text = GetLocalisedMessage(message);
		}

		private void ShowFurbyDiseaseMessage(string message1, string message2)
		{
			m_FurbyDiseaseMessageBackground.SetActive(true);
			m_FurbyDiseaseMessageSinglePart.SetActive(false);
			m_FurbyDiseaseMessageTwoPart.SetActive(true);
			UILabel[] componentsInChildren = m_FurbyDiseaseMessageTwoPart.GetComponentsInChildren<UILabel>();
			componentsInChildren[0].text = GetLocalisedMessage(message1);
			componentsInChildren[1].text = GetLocalisedMessage(message2);
		}

		private void HideFurbyDiseaseMessage()
		{
			m_FurbyDiseaseMessageBackground.SetActive(false);
			m_FurbyDiseaseMessageSinglePart.SetActive(false);
			m_FurbyDiseaseMessageTwoPart.SetActive(false);
		}

		private string GetFurbyDiseaseString1()
		{
			return "SICKBAY_SCAN_SICK";
		}

		private string GetFurbyDiseaseString2()
		{
			switch (m_SicknessStatus)
			{
			case SickBayDisease.Difurrhoea:
				return "SICKBAY_ILLNESS_DIFURRHOEA";
			case SickBayDisease.Hypofurbia:
				return "SICKBAY_ILLNESS_HYPOFURBIA";
			case SickBayDisease.Furbilitis:
				return "SICKBAY_ILLNESS_FURBILITIS";
			case SickBayDisease.Furlorn:
				return "SICKBAY_ILLNESS_FURLORN";
			case SickBayDisease.MuscleFurtigue:
				return "SICKBAY_ILLNESS_MUSCLEFURTIGUE";
			default:
				Logging.LogError("FURBY has an unknown disease!");
				return "SICKBAY_ILLNESS_MUSCLEFURTIGUE";
			}
		}

		private string GetCureHintMessageString()
		{
			if (m_CureHintIndex == -1)
			{
				int cureHintIndex = UnityEngine.Random.Range(0, 2);
				m_CureHintIndex = cureHintIndex;
			}
			bool flag = m_CureHintIndex == 0;
			switch (m_SicknessStatus)
			{
			case SickBayDisease.Difurrhoea:
				return (!flag) ? "SICKBAY_THOUGHT_DIFURRHOEA2" : "SICKBAY_THOUGHT_DIFURRHOEA1";
			case SickBayDisease.Hypofurbia:
				return (!flag) ? "SICKBAY_THOUGHT_HYPOFURBIA2" : "SICKBAY_THOUGHT_HYPOFURBIA1";
			case SickBayDisease.Furbilitis:
				return (!flag) ? "SICKBAY_THOUGHT_FURBILITIS2" : "SICKBAY_THOUGHT_FURBILITIS1";
			case SickBayDisease.Furlorn:
				return (!flag) ? "SICKBAY_THOUGHT_FURLORN2" : "SICKBAY_THOUGHT_FURLORN1";
			case SickBayDisease.MuscleFurtigue:
				return (!flag) ? "SICKBAY_THOUGHT_MUSCLEFURTIGUE2" : "SICKBAY_THOUGHT_MUSCLEFURTIGUE1";
			default:
				Logging.LogError("FURBY has an unknown disease!");
				return "SICKBAY_THOUGHT_MUSCLEFURTIGUE1";
			}
		}

		private void TransitionCameraToDiagnosisSection()
		{
			iTween.RotateTo(m_TransitionCamera, new Vector3(m_TransitionCameraTweenToRotationX, 0f, 0f), m_TransitionCameraTweenTime);
			iTween.MoveTo(m_TransitionCamera, m_TransitionCameraTweenToPosition, m_TransitionCameraTweenTime);
		}

		private void TransitionCameraToCureSection()
		{
			iTween.RotateTo(m_TransitionCamera, new Vector3(m_TransitionCameraTweenFromRotationX, 0f, 0f), m_TransitionCameraTweenTime);
			iTween.MoveTo(m_TransitionCamera, m_TransitionCameraTweenFromPosition, m_TransitionCameraTweenTime);
		}

		public IEnumerator SendComAirActionAndWaitForResponse(FurbyAction action, bool shouldWaitForResponse)
		{
			yield return StartCoroutine(m_ComAirWaiter.SendComAirEventAndWaitForResponse(action, (FurbyCommand)0, shouldWaitForResponse));
		}

		public IEnumerator SendComAirCommandAndWaitForResponse(FurbyCommand command, bool shouldWaitForResponse)
		{
			yield return StartCoroutine(m_ComAirWaiter.SendComAirEventAndWaitForResponse((FurbyAction)0, command, shouldWaitForResponse));
		}

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDestroy()
		{
			m_DebugPanelSub.Dispose();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Sick Bay"))
			{
				GUILayout.BeginHorizontal();
				string text = string.Format("Force Disease ({0})", (!m_ShouldForceDisease) ? "Off" : "On");
				if (GUILayout.Button(text))
				{
					m_ShouldForceDisease = !m_ShouldForceDisease;
				}
				string[] texts = new string[6] { "None", "Difurrhoea", "Hypofurbia", "Furbilitis", "Furlorn", "Muscle Furtigue" };
				int xCount = 3;
				m_ForcedDiseaseIndex = GUILayout.SelectionGrid(m_ForcedDiseaseIndex, texts, xCount);
				switch (m_ForcedDiseaseIndex)
				{
				case 0:
					m_ForcedDisease = SickBayDisease.Nothing;
					break;
				case 1:
					m_ForcedDisease = SickBayDisease.Difurrhoea;
					break;
				case 2:
					m_ForcedDisease = SickBayDisease.Hypofurbia;
					break;
				case 3:
					m_ForcedDisease = SickBayDisease.Furbilitis;
					break;
				case 4:
					m_ForcedDisease = SickBayDisease.Furlorn;
					break;
				case 5:
					m_ForcedDisease = SickBayDisease.MuscleFurtigue;
					break;
				default:
					m_ForcedDisease = SickBayDisease.Unknown;
					break;
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Wait Time Sickness Known");
				GUILayout.TextField(string.Format("{0:0.00}", m_CureSentWaitTimeSicknessIsKnown), GUILayout.ExpandWidth(false));
				m_CureSentWaitTimeSicknessIsKnown = GUILayout.HorizontalSlider(m_CureSentWaitTimeSicknessIsKnown, 0f, 10f);
				m_CureSentWaitTimeSicknessIsKnown = Mathf.Round(m_CureSentWaitTimeSicknessIsKnown * 2f) / 2f;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Wait Time Sickness NOT Known");
				GUILayout.TextField(string.Format("{0:0.00}", m_CureSentWaitTimeSicknessNotKnown), GUILayout.ExpandWidth(false));
				m_CureSentWaitTimeSicknessNotKnown = GUILayout.HorizontalSlider(m_CureSentWaitTimeSicknessNotKnown, 0f, 10f);
				m_CureSentWaitTimeSicknessNotKnown = Mathf.Round(m_CureSentWaitTimeSicknessNotKnown * 2f) / 2f;
				GUILayout.EndHorizontal();
			}
			DebugPanel.EndSection();
		}
	}
}
