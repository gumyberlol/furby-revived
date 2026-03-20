using System;
using System.Collections;
using System.Linq;
using Furby.Utilities.EggCarton;
using Relentless;
using UnityEngine;

namespace Furby.EggCarton
{
	public class EggReceivingFlow : RelentlessMonoBehaviour
	{
		[SerializeField]
		private CodeControlledDialogBox m_EggDialogBox;

		[SerializeField]
		private float m_ComAirTimeOut = 15f;

		[SerializeField]
		private float m_TimeBeforeEggDetailsMessage = 4f;

		[SerializeField]
		private CartonEggGrid m_CartonEggGrid;

		private FurbyBabyTypeID m_EggDetails;

		private bool m_DisabledWhilstSendingGiftOrShowingIncubateDialog;

		private bool m_DisabledWhilstEggMenuIsUp;

		[SerializeField]
		private GameObject m_BackButton;

		[SerializeField]
		private GameObject m_TitleBanner;

		[SerializeField]
		private GameObject m_GiftEggButton;

		[SerializeField]
		private GameObject m_AddEggButton;

		[SerializeField]
		private GameObject m_EggGiftingBackground;

		[SerializeField]
		private EggGiftingDialogBox m_EggGiftingDialogBox;

		[SerializeField]
		private ErrorMessageBox m_ErrorBox;

		[SerializeField]
		private GameObject m_EggMenu;

		private GameEventSubscription m_DebugPanelSub;

		private void Awake()
		{
			if (m_EggDialogBox == null)
			{
				Logging.LogError("No Egg Dialog Box Reference!");
			}
			if (m_CartonEggGrid == null)
			{
				Logging.LogError("No Carton Egg Grid Reference!");
			}
			if (m_GiftEggButton == null)
			{
				Logging.LogError("No Gift Egg Button Reference!");
			}
			if (m_AddEggButton == null)
			{
				Logging.LogError("No Add Egg Button Reference!");
			}
		}

		private void ResetFlow()
		{
			Logging.Log("EggReceivingFlow.ResetFlow");
			StopAllCoroutines();
			ResetState();
			StartCoroutine(StartEggReceivingFlow());
		}

		private void ResetState()
		{
			m_EggDialogBox.Hide();
			m_EggGiftingDialogBox.Hide();
			m_ErrorBox.Hide();
			RemoveFurbyToneDelegate();
			ShowEggButtons();
			ShowNormalBackground();
		}

		private void ShowNormalBackground()
		{
			m_EggGiftingBackground.SetActive(false);
		}

		private void ShowEggGiftingBackground()
		{
			m_EggGiftingBackground.SetActive(true);
		}

		private void ShowEggButtons()
		{
			m_BackButton.SetActive(true);
			m_TitleBanner.SetActive(true);
			m_GiftEggButton.SetActive(true);
			m_AddEggButton.SetActive(true);
		}

		private void HideEggButtons()
		{
			m_BackButton.SetActive(false);
			m_TitleBanner.SetActive(false);
			m_GiftEggButton.SetActive(false);
			m_AddEggButton.SetActive(false);
		}

		private void RemoveFurbyToneDelegate()
		{
			if (Singleton<FurbyDataChannel>.Instance != null)
			{
				Singleton<FurbyDataChannel>.Instance.ToneEvent -= FurbyToneEvent;
			}
		}

		private void OnDisable()
		{
			RemoveFurbyToneDelegate();
		}

		public void DisableWhilstSendingGiftOrShowingIncubateDialog()
		{
			m_DisabledWhilstSendingGiftOrShowingIncubateDialog = true;
			ResetFlow();
		}

		public void EnableNowNotSendingGiftOrShowingIncubateDialog()
		{
			m_DisabledWhilstSendingGiftOrShowingIncubateDialog = false;
			ResetFlow();
		}

		private IEnumerator Start()
		{
			yield return StartCoroutine(StartEggReceivingFlow());
		}

		private void FurbyToneEvent(FurbyMessageType msgType, long msgTone)
		{
			switch (msgType)
			{
			case FurbyMessageType.EggGifting:
				switch (msgTone)
				{
				case 233L:
					ShowDebugMessageBox("<DEBUG> RECEIVED INITIATE MESSAGE");
					GameEventRouter.SendEvent(EggReceivingEvent.ReceivedInitiateTransferMessage);
					break;
				case 234L:
					ShowDebugMessageBox("<DEBUG> RECEIVED ACCEPT MESSAGE (REPLY TO HANDSHAKE)");
					GameEventRouter.SendEvent(EggReceivingEvent.ReceivedHandshakeReply);
					break;
				}
				break;
			case FurbyMessageType.Translator:
			{
				ShowDebugMessageBox(string.Format("<DEBUG> RECEIVED EGG DETAILS: {0}", msgTone));
				bool isValidIntCode = false;
				m_EggDetails = GetFurbyBabyTypeIDForIntCode((int)msgTone, out isValidIntCode, false, true);
				Logging.Log(string.Format("Egg Details: {0} MsgTone: {1}", m_EggDetails.Tribe.Name, (int)msgTone));
				if (isValidIntCode && IsEggValidForGifting(m_EggDetails))
				{
					GameEventRouter.SendEvent(EggReceivingEvent.ReceivedEggDetails);
				}
				break;
			}
			}
		}

		private void Update()
		{
			bool activeSelf = m_EggMenu.activeSelf;
			if (!m_DisabledWhilstEggMenuIsUp && activeSelf)
			{
				m_DisabledWhilstEggMenuIsUp = true;
				ResetFlow();
				DisableInputOnLayer componentInChildren = m_EggMenu.GetComponentInChildren<DisableInputOnLayer>();
				componentInChildren.ForceDisableInput();
			}
			else if (m_DisabledWhilstEggMenuIsUp && !activeSelf)
			{
				m_DisabledWhilstEggMenuIsUp = false;
				ResetFlow();
			}
		}

		private IEnumerator StartEggReceivingFlow()
		{
			Logging.Log("EggReceivingFlow.StartEggReceivingFlow");
			if (FurbyGlobals.Player.FlowStage != FlowStage.Normal)
			{
				yield break;
			}
			while (m_DisabledWhilstSendingGiftOrShowingIncubateDialog || m_DisabledWhilstEggMenuIsUp)
			{
				yield return null;
			}
			Singleton<FurbyDataChannel>.Instance.ToneEvent += FurbyToneEvent;
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(EggReceivingEvent.ReceivedInitiateTransferMessage));
				EggReceivingEvent eggReceivingEvent = (EggReceivingEvent)(object)waiter.ReturnedEvent;
				if (eggReceivingEvent == EggReceivingEvent.ReceivedInitiateTransferMessage)
				{
					yield return StartCoroutine(WaitForEggDetails());
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggReceivingFlow::StartEggReceivingFlow : {0}", eggReceivingEvent.ToString()));
				}
			}
		}

		private bool IsEggCartonFull()
		{
			if (FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count() >= FurbyGlobals.BabyLibrary.GetMaxEggsInCarton())
			{
				return true;
			}
			return false;
		}

		private IEnumerator HandleTimeOut()
		{
			while (true)
			{
				yield return new WaitForSeconds(m_ComAirTimeOut);
				GameEventRouter.SendEvent(EggReceivingEvent.EggCommunicationsTimedOut);
			}
		}

		private void ShowDebugMessageBox(string text)
		{
		}

		private IEnumerator WaitForEggDetails()
		{
			Logging.Log("EggReceivingFlow.WaitForEggDetails");
			StartCoroutine("HandleTimeOut");
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(EggReceivingEvent.ReceivedEggDetails, EggReceivingEvent.EggCommunicationsTimedOut));
				StopCoroutine("HandleTimeOut");
				ShowEggGiftingBackground();
				bool isEggCartonFull = IsEggCartonFull();
				EggReceivingEvent eggReceivingEvent = (EggReceivingEvent)(object)waiter.ReturnedEvent;
				switch (eggReceivingEvent)
				{
				case EggReceivingEvent.ReceivedEggDetails:
					HideEggButtons();
					if (isEggCartonFull)
					{
						yield return StartCoroutine(ShowEggCartonFullDialog());
					}
					else
					{
						yield return StartCoroutine(ShowAcceptGiftDialog());
					}
					break;
				case EggReceivingEvent.EggCommunicationsTimedOut:
					ResetFlow();
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggReceivingFlow::WaitForEggDetails : {0}", eggReceivingEvent.ToString()));
					break;
				}
			}
		}

		private IEnumerator ShowAcceptGiftDialog()
		{
			Logging.Log("EggReceivingFlow.ShowAcceptGiftDialog");
			m_EggGiftingDialogBox.SetAcceptGiftState("EGGGIFTD2D_MENU_ACCEPTGIFT", "MENU_BUTTON_ACCEPT", EggReceivingEvent.EggDialogGenericAccept, "MENU_BUTTON_DECLINE", EggReceivingEvent.EggDialogGenericCancel);
			m_EggGiftingDialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(EggReceivingEvent.EggDialogGenericAccept, EggReceivingEvent.EggDialogGenericCancel));
				EggReceivingEvent eggReceivingEvent = (EggReceivingEvent)(object)waiter.ReturnedEvent;
				m_EggGiftingDialogBox.Hide();
				switch (eggReceivingEvent)
				{
				case EggReceivingEvent.EggDialogGenericAccept:
					yield return StartCoroutine(EggHandshake());
					break;
				case EggReceivingEvent.EggDialogGenericCancel:
					while (!Singleton<FurbyDataChannel>.Instance.PostMessage(235))
					{
						yield return null;
					}
					ResetFlow();
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggReceivingFlow::ShowAcceptGiftDialog : {0}", eggReceivingEvent.ToString()));
					break;
				}
			}
		}

		private IEnumerator EggHandshake()
		{
			Logging.Log("EggReceivingFlow.EggHandshake");
			GameEventRouter.SendEvent(CartonGameEvent.D2DTransferStartedReceiver);
			m_EggGiftingDialogBox.SetReceivingState("EGGGIFTD2D_MENU_RECEIVING");
			m_EggGiftingDialogBox.Show(true);
			FurbyDataChannel dataChannel = Singleton<FurbyDataChannel>.Instance;
			while (!dataChannel.PostMessage(234))
			{
				yield return null;
			}
			ShowDebugMessageBox("<DEBUG> SENT ACCEPT MESSAGE");
			yield return new WaitForSeconds(m_TimeBeforeEggDetailsMessage);
			int handshakeCode = GetHandshakeCodeForFurbyBabyTypeID(m_EggDetails, true);
			while (!dataChannel.PostMessage(handshakeCode))
			{
				yield return null;
			}
			ShowDebugMessageBox(string.Format("<DEBUG> SENT HANDSHAKE CODE: {0}", handshakeCode));
			StartCoroutine("HandleTimeOut");
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(EggReceivingEvent.ReceivedHandshakeReply, EggReceivingEvent.EggCommunicationsTimedOut));
				StopCoroutine("HandleTimeOut");
				EggReceivingEvent eggReceivingEvent = (EggReceivingEvent)(object)waiter.ReturnedEvent;
				switch (eggReceivingEvent)
				{
				case EggReceivingEvent.ReceivedHandshakeReply:
					yield return StartCoroutine(EggReceived());
					break;
				case EggReceivingEvent.EggCommunicationsTimedOut:
					yield return StartCoroutine(EggTimedOut());
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggReceivingFlow::EggHandshake : {0}", eggReceivingEvent.ToString()));
					break;
				}
			}
		}

		private IEnumerator EggReceived()
		{
			Logging.Log("EggReceivingFlow.EggReceived");
			GameEventRouter.SendEvent(CartonGameEvent.D2DTransferSucceededReceiver);
			bool crystal_will_be_unlocked = false;
			bool isCrystalTribe = m_EggDetails.Tribe.TribeSet == Tribeset.CrystalGem;
			if (isCrystalTribe && !Singleton<GameDataStoreObject>.Instance.GlobalData.CrystalUnlocked)
			{
				Singleton<GameDataStoreObject>.Instance.UnlockCrystal();
				crystal_will_be_unlocked = true;
			}
			FurbyBaby furbyBaby = FurbyGlobals.BabyRepositoryHelpers.CreateNewBaby(m_EggDetails, true);
			furbyBaby.Progress = FurbyBabyProgresss.E;
			if (isCrystalTribe && crystal_will_be_unlocked)
			{
				GameEventRouter.SendEvent(CrystalUnlockTelemetryEvents.CrystalUnlocked_ReceivedEggFromFriendsDevice);
			}
			GameEventRouter.SendEvent(BabyLifecycleEvent.FromFriendsDevice, null, furbyBaby);
			FurbyDataChannel dataChannel = Singleton<FurbyDataChannel>.Instance;
			while (!dataChannel.PostMessage(233))
			{
				yield return null;
			}
			ShowDebugMessageBox("<DEBUG> SENT INITIATE (RECEIVED REPLY MESSAGE)");
			m_EggGiftingDialogBox.InitConfirmGiftEgg(furbyBaby);
			while (!m_EggGiftingDialogBox.IsEggReadyToBeRendered())
			{
				yield return null;
			}
			m_EggGiftingDialogBox.SetReceivedState("EGGGIFTD2D_MENU_RECEIVEDSUCCESS", "EGGGIFTD2D_MENU_SUCCESSBANNER", "MENU_OPTION_OK", EggReceivingEvent.EggDialogGenericAccept);
			m_EggGiftingDialogBox.Show(true);
			m_EggGiftingDialogBox.ShowEgg();
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(EggReceivingEvent.EggDialogGenericAccept));
				m_EggGiftingDialogBox.Hide();
				EggReceivingEvent eggReceivingEvent = (EggReceivingEvent)(object)waiter.ReturnedEvent;
				if (eggReceivingEvent == EggReceivingEvent.EggDialogGenericAccept)
				{
					bool shouldOnlyAddNewEggs = true;
					m_CartonEggGrid.HandleEggEntrancing(shouldOnlyAddNewEggs);
					ResetFlow();
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggReceivingFlow::EggRecieved : {0}", eggReceivingEvent.ToString()));
				}
			}
		}

		private IEnumerator ShowEggCartonFullDialog()
		{
			Logging.Log("EggReceivingFlow.ShowEggCartonFullDialog");
			m_ErrorBox.SetOKState("EGGGIFTD2D_MENU_EGGCARTONFULL", "MENU_OPTION_OK", EggReceivingEvent.EggDialogGenericCancel);
			m_ErrorBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(EggReceivingEvent.EggDialogGenericCancel));
				EggReceivingEvent eggReceivingEvent = (EggReceivingEvent)(object)waiter.ReturnedEvent;
				if (eggReceivingEvent == EggReceivingEvent.EggDialogGenericCancel)
				{
					ResetFlow();
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggReceivingFlow::ShowEggCartonFullDialog : {0}", eggReceivingEvent.ToString()));
				}
			}
		}

		private IEnumerator EggTimedOut()
		{
			Logging.Log("EggReceivingFlow.EggTimedOut");
			GameEventRouter.SendEvent(CartonGameEvent.D2DTransferFailedReceiver);
			m_EggGiftingDialogBox.Hide();
			m_ErrorBox.SetOKState("EGGGIFTD2D_MENU_RECEIVEFAILED", "MENU_OPTION_OK", EggReceivingEvent.EggDialogGenericAccept);
			m_ErrorBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(EggReceivingEvent.EggDialogGenericAccept));
				EggReceivingEvent eggReceivingEvent = (EggReceivingEvent)(object)waiter.ReturnedEvent;
				if (eggReceivingEvent == EggReceivingEvent.EggDialogGenericAccept)
				{
					ResetFlow();
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggReceivingFlow::EggTimedOut : {0}", eggReceivingEvent.ToString()));
				}
			}
		}

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDestroy()
		{
			m_DebugPanelSub.Dispose();
			RemoveFurbyToneDelegate();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Egg Receiving"))
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Send Initiate Egg Transfer"))
				{
					GameEventRouter.SendEvent(EggReceivingEvent.ReceivedInitiateTransferMessage, null, null);
				}
				if (GUILayout.Button("Send Egg Details"))
				{
					bool isValidIntCode = false;
					m_EggDetails = GenerateRandomEgg(out isValidIntCode);
					if (isValidIntCode && IsEggValidForGifting(m_EggDetails))
					{
						GameEventRouter.SendEvent(EggReceivingEvent.ReceivedEggDetails, null, null);
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Received Handshake Reply"))
				{
					GameEventRouter.SendEvent(EggReceivingEvent.ReceivedHandshakeReply, null, null);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Com Air Time Out");
				GUILayout.TextField(string.Format("{0:0.00}", m_ComAirTimeOut), GUILayout.ExpandWidth(false));
				m_ComAirTimeOut = GUILayout.HorizontalSlider(m_ComAirTimeOut, 0f, 20f);
				m_ComAirTimeOut = Mathf.Round(m_ComAirTimeOut * 2f) / 2f;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Time Before Egg Details");
				GUILayout.TextField(string.Format("{0:0.00}", m_TimeBeforeEggDetailsMessage), GUILayout.ExpandWidth(false));
				m_TimeBeforeEggDetailsMessage = GUILayout.HorizontalSlider(m_TimeBeforeEggDetailsMessage, 0f, 5f);
				m_TimeBeforeEggDetailsMessage = Mathf.Round(m_TimeBeforeEggDetailsMessage * 2f) / 2f;
				GUILayout.EndHorizontal();
			}
			DebugPanel.EndSection();
		}

		public int GetIntCodeForFurbyBabyTypeID(FurbyBabyTypeID typeID)
		{
			return GetIntCodeForFurbyBabyTypeID(typeID, false);
		}

		public int GetIntCodeForFurbyBabyTypeID(FurbyBabyTypeID typeID, bool listAll)
		{
			int num = 0;
			foreach (FurbyTribeType item in (!listAll) ? FurbyGlobals.BabyLibrary.TribeList.List : FurbyGlobals.BabyLibrary.TribeList.ListAll)
			{
				foreach (FurbyTribeType.BabyUnlockLevel unlockLevel in item.UnlockLevels)
				{
					FurbyBabyTypeInfo[] babyTypes = unlockLevel.BabyTypes;
					foreach (FurbyBabyTypeInfo furbyBabyTypeInfo in babyTypes)
					{
						if (furbyBabyTypeInfo.TypeID.Equals(typeID))
						{
							return num;
						}
						num++;
					}
				}
			}
			return -1;
		}

		public int GetHandshakeCodeForFurbyBabyTypeID(FurbyBabyTypeID typeID)
		{
			return GetHandshakeCodeForFurbyBabyTypeID(typeID, false);
		}

		public int GetHandshakeCodeForFurbyBabyTypeID(FurbyBabyTypeID typeID, bool listAll)
		{
			return 226 - GetIntCodeForFurbyBabyTypeID(typeID, listAll);
		}

		public FurbyBabyTypeID GetFurbyBabyTypeIDForIntCode(int code, out bool isValidIntCode, bool wasReEntered)
		{
			return GetFurbyBabyTypeIDForIntCode(code, out isValidIntCode, wasReEntered, false);
		}

		public FurbyBabyTypeID GetFurbyBabyTypeIDForIntCode(int code, out bool isValidIntCode, bool wasReEntered, bool listAll)
		{
			int num = 0;
			foreach (FurbyTribeType item in (!listAll) ? FurbyGlobals.BabyLibrary.TribeList.List : FurbyGlobals.BabyLibrary.TribeList.ListAll)
			{
				foreach (FurbyTribeType.BabyUnlockLevel unlockLevel in item.UnlockLevels)
				{
					FurbyBabyTypeInfo[] babyTypes = unlockLevel.BabyTypes;
					foreach (FurbyBabyTypeInfo furbyBabyTypeInfo in babyTypes)
					{
						if (code == num)
						{
							isValidIntCode = !wasReEntered;
							Logging.Log(string.Format("Code: {0} IsValid: {1} TypeID: {2}", code, isValidIntCode, furbyBabyTypeInfo.TypeID));
							return furbyBabyTypeInfo.TypeID;
						}
						num++;
					}
				}
			}
			Logging.LogError(string.Format("Could not find a FurbyBabyTypeID for IntCode {0}!", code));
			isValidIntCode = false;
			return GetFurbyBabyTypeIDForIntCode(0, out isValidIntCode, true, listAll);
		}

		private FurbyBabyTypeID GenerateRandomEgg(out bool isValidIntCode)
		{
			int code = UnityEngine.Random.Range(0, 300);
			return GetFurbyBabyTypeIDForIntCode(code, out isValidIntCode, false);
		}

		public bool IsEggValidForGifting(FurbyBabyTypeID typeID)
		{
			if (typeID.Tribe.TribeSet == Tribeset.Promo)
			{
				return false;
			}
			if (typeID.Tribe.TribeSet == Tribeset.Golden || typeID.Tribe.TribeSet == Tribeset.CrystalGolden)
			{
				return false;
			}
			return true;
		}

		public bool IsEggValidForGifting(FurbyBaby furbyBaby)
		{
			return furbyBaby.CanBeGifted;
		}
	}
}
