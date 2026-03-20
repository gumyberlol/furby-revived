using System;
using System.Collections;
using System.Linq;
using Furby.Utilities.EggCarton;
using Relentless;
using UnityEngine;

namespace Furby.EggCarton
{
	public class EggCartonFlow : RelentlessMonoBehaviour
	{
		[SerializeField]
		private CodeControlledDialogBox m_EggDialogBox;

		[SerializeField]
		private float m_ComAirTimeOut = 15f;

		[SerializeField]
		private float m_TimeBeforeEggDetailsMessage = 4f;

		private FurbyBaby m_EggBeingSentOrDeleted;

		[SerializeField]
		private CartonEggGrid m_CartonEggGrid;

		[SerializeField]
		private EggReceivingFlow m_EggReceivingFlow;

		[SerializeField]
		private GameObject m_BackButton;

		[SerializeField]
		private GameObject m_TitleBanner;

		[SerializeField]
		private GameObject m_GiftEggButton;

		[SerializeField]
		private GameObject m_AddEggButton;

		private bool m_IsWaitingBeforeSendingEggDetails;

		[SerializeField]
		private GameObject m_EggGiftingBackground;

		[SerializeField]
		private EggGiftingDialogBox m_EggGiftingDialogBox;

		[SerializeField]
		private ErrorMessageBox m_ErrorBox;

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
			if (m_EggReceivingFlow == null)
			{
				Logging.LogError("No Egg Receiving Flow Reference!");
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
			Logging.Log("EggCartonFlow.ResetFlow");
			StopAllCoroutines();
			ResetState();
			StartCoroutine(StartEggSendingFlow());
		}

		private void ResetState()
		{
			m_EggDialogBox.Hide();
			m_EggGiftingDialogBox.Hide();
			m_ErrorBox.Hide();
			m_EggBeingSentOrDeleted = null;
			m_EggReceivingFlow.EnableNowNotSendingGiftOrShowingIncubateDialog();
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

		private void Start()
		{
			Singleton<FurbyDataChannel>.Instance.DisableCommunications = false;
			ResetFlow();
		}

		private IEnumerator StartEggSendingFlow()
		{
			Logging.Log("EggCartonFlow.StartEggSendingFlow");
			if (FurbyGlobals.SettingsHelper.IsDeleteEggRequested())
			{
				yield return StartCoroutine(StartEggDeleting());
			}
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.GiftButtonClicked, CartonGameEvent.EggClickedUpon));
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				m_EggReceivingFlow.DisableWhilstSendingGiftOrShowingIncubateDialog();
				HideEggButtons();
				bool isEggCartonEmpty = IsEggCartonEmpty();
				switch (cartonEvent)
				{
				case CartonGameEvent.EggClickedUpon:
					if (FurbyGlobals.Player.InProgressFurbyBaby == null || FurbyGlobals.Player.InProgressFurbyBaby == (FurbyBaby)waiter.ReturnedParameters[0] || FurbyGlobals.Player.InProgressFurbyBaby.Progress == FurbyBabyProgresss.N)
					{
						yield return StartCoroutine(StartIncubationDialog());
					}
					else
					{
						yield return StartCoroutine(EggCannotBeIncubated());
					}
					break;
				case CartonGameEvent.GiftButtonClicked:
					if (isEggCartonEmpty)
					{
						yield return StartCoroutine(ShowEggCartonEmptyDialog());
					}
					else
					{
						yield return StartCoroutine(StartEggSending());
					}
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::StartEggSendingFlow : {0}", cartonEvent.ToString()));
					break;
				}
			}
		}

		private bool IsEggCartonEmpty()
		{
			if (FurbyGlobals.BabyRepositoryHelpers.EggCarton.Count() == 0)
			{
				return true;
			}
			return false;
		}

		private IEnumerator ShowEggCartonEmptyDialog()
		{
			Logging.Log("EggCartonFlow.ShowEggCartonEmptyDialog");
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "EGGGIFTD2D_MENU_NOEGGS");
			m_EggDialogBox.SetDialogType(CodeControlledDialogBox.DialogType.OneButton);
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.OkButton, "MENU_OPTION_OK");
			m_EggDialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.OkButton, CartonGameEvent.EggDialogGenericDecline);
			m_EggDialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericDecline));
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				if (cartonEvent == CartonGameEvent.EggDialogGenericDecline)
				{
					ResetFlow();
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::ShowEggCartonEmptyDialog : {0}", cartonEvent.ToString()));
				}
			}
		}

		private IEnumerator EggCannotBeIncubated()
		{
			Logging.Log("EggCartonFlow.EggCannotBeIncubated");
			GameEventRouter.SendEvent(CartonGameEvent.EggCannotBeIncubated);
			if (FurbyGlobals.Player.InProgressFurbyBaby.Progress == FurbyBabyProgresss.E)
			{
				m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "EGGCARTON_ALREADYHAVEACTIVEEGG");
			}
			else
			{
				m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "EGGCARTON_ALREADYHAVEACTIVE");
			}
			m_EggDialogBox.SetDialogType(CodeControlledDialogBox.DialogType.OneButton);
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.OkButton, "MENU_OPTION_OK");
			m_EggDialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.OkButton, CartonGameEvent.EggDialogGenericDecline);
			m_EggDialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericDecline));
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				if (cartonEvent == CartonGameEvent.EggDialogGenericDecline)
				{
					ResetFlow();
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::EggCannotBeIncubated : {0}", cartonEvent.ToString()));
				}
			}
		}

		private IEnumerator StartIncubationDialog()
		{
			Logging.Log("EggCartonFlow.StartIncubationDialog");
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "EGGCARTON_SELECTEGG_CONFIRM");
			m_EggDialogBox.SetDialogType(CodeControlledDialogBox.DialogType.TwoButtons);
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, "MENU_OPTION_OK");
			m_EggDialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, CartonGameEvent.EggSelectedToIncubate);
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.CancelButton, "MENU_OPTION_CANCEL");
			m_EggDialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.CancelButton, CartonGameEvent.EggDialogGenericDecline);
			m_EggDialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericDecline));
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				if (cartonEvent == CartonGameEvent.EggDialogGenericDecline)
				{
					ResetFlow();
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::StartIncubationDialog : {0}", cartonEvent.ToString()));
				}
			}
		}

		private IEnumerator StartEggSending()
		{
			Logging.Log("EggCartonFlow.StartEggSending");
			ShowEggGiftingBackground();
			m_EggGiftingDialogBox.SetChooseEggState("EGGGIFTD2D_TITLE_CHOOSEEGG", CartonGameEvent.EggDialogGenericDecline);
			m_EggGiftingDialogBox.Show(false);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggClickedUpon, CartonGameEvent.EggDialogGenericDecline));
				m_EggGiftingDialogBox.Hide();
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				switch (cartonEvent)
				{
				case CartonGameEvent.EggClickedUpon:
					m_EggBeingSentOrDeleted = (FurbyBaby)waiter.ReturnedParameters[0];
					if (IsEggValidForGifting())
					{
						yield return StartCoroutine(StartGiftThisEggDialog());
					}
					else
					{
						yield return StartCoroutine(ShowEggCantBeGiftedDialog());
					}
					break;
				case CartonGameEvent.EggDialogGenericDecline:
					ResetFlow();
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::StartEggSending : {0}", cartonEvent.ToString()));
					break;
				}
			}
		}

		private bool IsEggValidForGifting()
		{
			return m_EggReceivingFlow.IsEggValidForGifting(m_EggBeingSentOrDeleted);
		}

		private IEnumerator ShowEggCantBeGiftedDialog()
		{
			Logging.Log("EggCartonFlow.ShowEggCantBeGiftedDialog");
			m_EggGiftingDialogBox.Hide();
			m_ErrorBox.SetOKState("EGGGIFTD2D_MENU_NOTALLOWED", "MENU_OPTION_OK", CartonGameEvent.EggDialogGenericAccept);
			m_ErrorBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericAccept));
				m_ErrorBox.Hide();
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				if (cartonEvent == CartonGameEvent.EggDialogGenericAccept)
				{
					yield return StartCoroutine(StartEggSending());
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::ShowEggCantBeGiftedDialog : {0}", cartonEvent.ToString()));
				}
			}
		}

		private IEnumerator StartGiftThisEggDialog()
		{
			Logging.Log("EggCartonFlow.StartGiftThisEggDialog");
			GameEventRouter.SendEvent(CartonGameEvent.EggGiftChosen);
			m_EggGiftingDialogBox.InitConfirmGiftEgg(m_EggBeingSentOrDeleted);
			while (!m_EggGiftingDialogBox.IsEggReadyToBeRendered())
			{
				yield return null;
			}
			m_EggGiftingDialogBox.SetConfirmGiftState("EGGGIFTD2D_MENU_CONFIRMGIFT", "MENU_OPTION_OK", CartonGameEvent.EggDialogGenericAccept, "MENU_OPTION_CANCEL", CartonGameEvent.EggDialogGenericDecline);
			m_EggGiftingDialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericAccept, CartonGameEvent.EggDialogGenericDecline));
				m_EggGiftingDialogBox.Hide();
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				switch (cartonEvent)
				{
				case CartonGameEvent.EggDialogGenericAccept:
					yield return StartCoroutine(ShowSendingAdviceDialog());
					break;
				case CartonGameEvent.EggDialogGenericDecline:
					GameEventRouter.SendEvent(CartonGameEvent.EggGiftFailedDialogCancel);
					yield return StartCoroutine(StartEggSending());
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::StartGiftThisEggDialog : {0}", cartonEvent.ToString()));
					break;
				}
			}
		}

		private IEnumerator ShowSendingAdviceDialog()
		{
			Logging.Log("EggCartonFlow.ShowSendingAdviceDialog");
			m_EggGiftingDialogBox.SetSendingAdviceState("EGGGIFTD2D_MENU_ADVICE", "MENU_OPTION_OK", CartonGameEvent.EggDialogGenericAccept);
			m_EggGiftingDialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericAccept, CartonGameEvent.EggDialogGenericDecline));
				m_EggGiftingDialogBox.Hide();
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				switch (cartonEvent)
				{
				case CartonGameEvent.EggDialogGenericAccept:
					yield return StartCoroutine(FinallyStartSendingTheEgg());
					break;
				case CartonGameEvent.EggDialogGenericDecline:
					ResetFlow();
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::ShowSendingAdviceDialog : {0}", cartonEvent.ToString()));
					break;
				}
			}
		}

		private IEnumerator WaitBeforeSendingEggDetails()
		{
			yield return new WaitForSeconds(m_TimeBeforeEggDetailsMessage);
			GameEventRouter.SendEvent(CartonGameEvent.SendEggTimedOut);
		}

		private IEnumerator HandleTimeOut()
		{
			while (true)
			{
				yield return new WaitForSeconds(m_ComAirTimeOut);
				GameEventRouter.SendEvent(CartonGameEvent.SendEggTimedOut);
			}
		}

		private void FurbyToneEvent(FurbyMessageType msgType, long msgTone)
		{
			switch (msgType)
			{
			case FurbyMessageType.EggGifting:
				switch (msgTone)
				{
				case 234L:
					ShowDebugMessageBox("<DEBUG> RECEIVED ACCEPT MESSAGE (HANDSHAKE)");
					GameEventRouter.SendEvent(CartonGameEvent.ReceivedHandshake);
					break;
				case 235L:
					ShowDebugMessageBox("<DEBUG> RECEIVED DECLINE MESSAGE");
					GameEventRouter.SendEvent(CartonGameEvent.EggDeclined);
					break;
				case 233L:
					ShowDebugMessageBox("<DEBUG> RECEIVED INITIATE MESSAGE (RECEIVED RECEIVED REPLY)");
					GameEventRouter.SendEvent(CartonGameEvent.EggReceived);
					break;
				}
				break;
			case FurbyMessageType.Translator:
			{
				int handshakeCodeForFurbyBabyTypeID = m_EggReceivingFlow.GetHandshakeCodeForFurbyBabyTypeID(m_EggBeingSentOrDeleted.Type);
				if ((int)msgTone == handshakeCodeForFurbyBabyTypeID)
				{
					ShowDebugMessageBox(string.Format("<DEBUG> RECEIVED HANDSHAKE CODE: {0}", msgTone));
					GameEventRouter.SendEvent(CartonGameEvent.ReceivedHandshakeCode);
				}
				else
				{
					ShowDebugMessageBox(string.Format("<DEBUG> RECEIVED BAD HANDSHAKE CODE: {0}\nWANTED {1}", msgTone, handshakeCodeForFurbyBabyTypeID));
				}
				break;
			}
			}
		}

		private IEnumerator FinallyStartSendingTheEgg()
		{
			Logging.Log("EggCartonFlow.FinallyStartSendingTheEgg");
			GameEventRouter.SendEvent(CartonGameEvent.D2DTransferStarted);
			m_EggGiftingDialogBox.SetSendingState("EGGGIFTD2D_MENU_SENDING", "MENU_OPTION_CANCEL", CartonGameEvent.EggDialogGenericDecline);
			m_EggGiftingDialogBox.Show(true);
			FurbyDataChannel dataChannel = Singleton<FurbyDataChannel>.Instance;
			while (!dataChannel.PostMessage(233))
			{
				yield return null;
			}
			ShowDebugMessageBox("<DEBUG> SENT INITIATE MESSAGE");
			StartCoroutine("WaitBeforeSendingEggDetails");
			WaitForGameEvent firstWaiter = new WaitForGameEvent();
			m_IsWaitingBeforeSendingEggDetails = true;
			while (m_IsWaitingBeforeSendingEggDetails)
			{
				yield return StartCoroutine(firstWaiter.WaitForEvent(CartonGameEvent.EggDialogGenericDecline, CartonGameEvent.SendEggTimedOut));
				StopCoroutine("WaitBeforeSendingEggDetails");
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)firstWaiter.ReturnedEvent;
				switch (cartonEvent)
				{
				case CartonGameEvent.EggDialogGenericDecline:
					HandleTransferCancelledByUser();
					break;
				case CartonGameEvent.SendEggTimedOut:
					m_IsWaitingBeforeSendingEggDetails = false;
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::FinallyStartSendingTheEgg : {0}", cartonEvent.ToString()));
					break;
				}
			}
			int eggCode = m_EggReceivingFlow.GetIntCodeForFurbyBabyTypeID(m_EggBeingSentOrDeleted.TypeAll);
			while (!dataChannel.PostMessage(eggCode))
			{
				yield return null;
			}
			ShowDebugMessageBox(string.Format("<DEBUG> SENT EGG DETAILS: {0}", eggCode));
			dataChannel.ToneEvent += FurbyToneEvent;
			StartCoroutine("HandleTimeOut");
			WaitForGameEvent secondWaiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(secondWaiter.WaitForEvent(CartonGameEvent.ReceivedHandshake, CartonGameEvent.EggDeclined, CartonGameEvent.EggDialogGenericDecline, CartonGameEvent.SendEggTimedOut));
				StopCoroutine("HandleTimeOut");
				CartonGameEvent cartonEvent2 = (CartonGameEvent)(object)secondWaiter.ReturnedEvent;
				switch (cartonEvent2)
				{
				case CartonGameEvent.ReceivedHandshake:
					yield return StartCoroutine(ReceivedHandshake());
					break;
				case CartonGameEvent.EggDeclined:
					yield return StartCoroutine(EggDeclined());
					break;
				case CartonGameEvent.EggDialogGenericDecline:
					HandleTransferCancelledByUser();
					break;
				case CartonGameEvent.SendEggTimedOut:
					yield return StartCoroutine(EggTimedOut());
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::FinallyStartSendingTheEgg : {0}", cartonEvent2.ToString()));
					break;
				}
			}
		}

		private void HandleTransferCancelledByUser()
		{
			GameEventRouter.SendEvent(CartonGameEvent.D2DTransferFailed);
			GameEventRouter.SendEvent(CartonGameEvent.EggGiftFailedDialogCancel);
			ResetFlow();
		}

		private IEnumerator ReceivedHandshake()
		{
			Logging.Log("EggCartonFlow.ReceivedHandshake");
			StartCoroutine("HandleTimeOut");
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.ReceivedHandshakeCode, CartonGameEvent.EggDialogGenericDecline, CartonGameEvent.SendEggTimedOut));
				StopCoroutine("HandleTimeOut");
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				switch (cartonEvent)
				{
				case CartonGameEvent.ReceivedHandshakeCode:
					yield return StartCoroutine(ReceivedHandshakeCode());
					break;
				case CartonGameEvent.EggDialogGenericDecline:
					HandleTransferCancelledByUser();
					break;
				case CartonGameEvent.SendEggTimedOut:
					yield return StartCoroutine(EggTimedOut());
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::ReceivedHandshake : {0}", cartonEvent.ToString()));
					break;
				}
			}
		}

		private IEnumerator ReceivedHandshakeCode()
		{
			Logging.Log("EggCartonFlow.ReceivedHandshakeCode");
			FurbyDataChannel dataChannel = Singleton<FurbyDataChannel>.Instance;
			while (!dataChannel.PostMessage(234))
			{
				yield return null;
			}
			ShowDebugMessageBox("<DEBUG> SENT ACCEPT MESSAGE (CONFIRM HANDSHAKE)");
			StartCoroutine("HandleTimeOut");
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggReceived, CartonGameEvent.EggDialogGenericDecline, CartonGameEvent.SendEggTimedOut));
				StopCoroutine("HandleTimeOut");
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				switch (cartonEvent)
				{
				case CartonGameEvent.EggReceived:
					yield return StartCoroutine(EggReceived());
					break;
				case CartonGameEvent.EggDialogGenericDecline:
					HandleTransferCancelledByUser();
					break;
				case CartonGameEvent.SendEggTimedOut:
					yield return StartCoroutine(EggTimedOut());
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::ReceivedHandshakeCode : {0}", cartonEvent.ToString()));
					break;
				}
			}
		}

		private void ShowDebugMessageBox(string text)
		{
		}

		private IEnumerator EggReceived()
		{
			Logging.Log("EggCartonFlow.EggReceived");
			GameEventRouter.SendEvent(CartonGameEvent.D2DTransferSucceeded);
			ShowDebugMessageBox("<DEBUG> RECEIVED RECEIVED REPLY MESSAGE");
			m_CartonEggGrid.RemoveEgg(m_EggBeingSentOrDeleted);
			Singleton<GameDataStoreObject>.Instance.Data.RemoveFurbyBaby(m_EggBeingSentOrDeleted);
			m_EggGiftingDialogBox.SetReceivedState("EGGGIFTD2D_MENU_TRANSFERSUCCESS", "EGGGIFTD2D_MENU_SUCCESSBANNER", "MENU_OPTION_OK", CartonGameEvent.EggDialogGenericAccept);
			m_EggGiftingDialogBox.Show(true);
			m_EggGiftingDialogBox.ShowEgg();
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericAccept));
				m_EggGiftingDialogBox.Hide();
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				if (cartonEvent == CartonGameEvent.EggDialogGenericAccept)
				{
					ResetFlow();
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::EggRecieved : {0}", cartonEvent.ToString()));
				}
			}
		}

		private IEnumerator EggDeclined()
		{
			Logging.Log("EggCartonFlow.EggDeclined");
			GameEventRouter.SendEvent(CartonGameEvent.D2DTransferFailed);
			m_EggGiftingDialogBox.Hide();
			m_ErrorBox.SetOKState("EGGGIFTD2D_MENU_TRANSFERDECLINED", "MENU_OPTION_OK", CartonGameEvent.EggDialogGenericAccept);
			m_ErrorBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericAccept));
				m_ErrorBox.Hide();
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				if (cartonEvent == CartonGameEvent.EggDialogGenericAccept)
				{
					ResetFlow();
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::EggDeclined : {0}", cartonEvent.ToString()));
				}
			}
		}

		private IEnumerator EggTimedOut()
		{
			Logging.Log("EggCartonFlow.EggTimedOut");
			GameEventRouter.SendEvent(CartonGameEvent.D2DTransferFailed);
			m_EggGiftingDialogBox.Hide();
			m_ErrorBox.SetAcceptCancelState("EGGGIFTD2D_MENU_TRANSFERFAILED", "MENU_BUTTON_TRYAGAIN", CartonGameEvent.EggDialogGenericAccept, "MENU_OPTION_CANCEL", CartonGameEvent.EggDialogGenericDecline);
			m_ErrorBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericAccept, CartonGameEvent.EggDialogGenericDecline));
				m_ErrorBox.Hide();
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				switch (cartonEvent)
				{
				case CartonGameEvent.EggDialogGenericAccept:
					GameEventRouter.SendEvent(CartonGameEvent.EggGiftFailedDialogTryAgain);
					yield return StartCoroutine(FinallyStartSendingTheEgg());
					break;
				case CartonGameEvent.EggDialogGenericDecline:
					GameEventRouter.SendEvent(CartonGameEvent.EggGiftFailedDialogCancel);
					ResetFlow();
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::EggTimedOut : {0}", cartonEvent.ToString()));
					break;
				}
			}
		}

		private IEnumerator StartEggDeleting()
		{
			Logging.Log("EggCartonFlow.StartEggDeleting");
			m_EggReceivingFlow.DisableWhilstSendingGiftOrShowingIncubateDialog();
			HideEggButtons();
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "SETTINGS_DELETEEGG_CHOOSEEGG");
			m_EggDialogBox.SetDialogType(CodeControlledDialogBox.DialogType.SmallMessageOneButton);
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.OkButton, "MENU_OPTION_CANCEL");
			m_EggDialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.OkButton, CartonGameEvent.EggDialogGenericDecline);
			m_EggDialogBox.Show(false);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggClickedUpon, CartonGameEvent.EggDialogGenericDecline));
			m_EggDialogBox.Hide();
			CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
			switch (cartonEvent)
			{
			case CartonGameEvent.EggClickedUpon:
				m_EggBeingSentOrDeleted = (FurbyBaby)waiter.ReturnedParameters[0];
				yield return StartCoroutine(StartDeleteWarningDialog());
				break;
			case CartonGameEvent.EggDialogGenericDecline:
				FurbyGlobals.SettingsHelper.ClearDeleteEggRequest();
				GameEventRouter.SendEvent(CartonGameEvent.EggDeleteCompleted);
				break;
			default:
				Logging.LogError(string.Format("Unexpected event in EggCartonFlow::StartEggDeleting : {0}", cartonEvent.ToString()));
				break;
			}
		}

		private IEnumerator StartDeleteWarningDialog()
		{
			Logging.Log("EggCartonFlow.StartDeleteWarningDialog");
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "SETTINGS_DELETEEGG_WARNING");
			m_EggDialogBox.SetDialogType(CodeControlledDialogBox.DialogType.TwoButtons);
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, "SAVEGAME_DELETE_YES");
			m_EggDialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.AcceptButton, CartonGameEvent.EggDialogGenericAccept);
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.CancelButton, "SAVEGAME_DELETE_NO");
			m_EggDialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.CancelButton, CartonGameEvent.EggDialogGenericDecline);
			m_EggDialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericAccept, CartonGameEvent.EggDialogGenericDecline));
				m_EggDialogBox.Hide();
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				switch (cartonEvent)
				{
				case CartonGameEvent.EggDialogGenericAccept:
					yield return StartCoroutine(StartDeleteConfirmDialog());
					break;
				case CartonGameEvent.EggDialogGenericDecline:
					ResetFlow();
					break;
				default:
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::StartDeleteWarningDialog : {0}", cartonEvent.ToString()));
					break;
				}
			}
		}

		private IEnumerator StartDeleteConfirmDialog()
		{
			Logging.Log("EggCartonFlow.StartDeleteConfirmDialog");
			GameEventRouter.SendEvent(CartonGameEvent.EggWasDeleted);
			m_CartonEggGrid.RemoveEgg(m_EggBeingSentOrDeleted);
			Singleton<GameDataStoreObject>.Instance.Data.RemoveFurbyBaby(m_EggBeingSentOrDeleted);
			FurbyGlobals.SettingsHelper.ClearDeleteEggRequest();
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.DialogText, "SETTINGS_DELETEEGG_CONFIRMED");
			m_EggDialogBox.SetDialogType(CodeControlledDialogBox.DialogType.OneButton);
			m_EggDialogBox.SetLocalisedText(CodeControlledDialogBox.WidgetIdentifier.OkButton, "MENU_OPTION_OK");
			m_EggDialogBox.SetGameEvent(CodeControlledDialogBox.WidgetIdentifier.OkButton, CartonGameEvent.EggDialogGenericDecline);
			m_EggDialogBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				yield return StartCoroutine(waiter.WaitForEvent(CartonGameEvent.EggDialogGenericDecline));
				m_EggDialogBox.Hide();
				CartonGameEvent cartonEvent = (CartonGameEvent)(object)waiter.ReturnedEvent;
				if (cartonEvent == CartonGameEvent.EggDialogGenericDecline)
				{
					GameEventRouter.SendEvent(CartonGameEvent.EggDeleteCompleted);
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in EggCartonFlow::StartDeleteConfirmDialog : {0}", cartonEvent.ToString()));
				}
			}
		}

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDestroy()
		{
			Singleton<FurbyDataChannel>.Instance.DisableCommunications = FurbyGlobals.Player.NoFurbyOnSaveGame();
			m_DebugPanelSub.Dispose();
			RemoveFurbyToneDelegate();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Egg Sending"))
			{
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Handshake Received"))
				{
					GameEventRouter.SendEvent(CartonGameEvent.ReceivedHandshake, null, null);
				}
				if (GUILayout.Button("Handshake Code Received"))
				{
					GameEventRouter.SendEvent(CartonGameEvent.ReceivedHandshakeCode, null, null);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if (GUILayout.Button("Egg Received"))
				{
					GameEventRouter.SendEvent(CartonGameEvent.EggReceived, null, null);
				}
				if (GUILayout.Button("Egg Declined"))
				{
					GameEventRouter.SendEvent(CartonGameEvent.EggDeclined, null, null);
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
	}
}
