using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Utilities.VoiceChanger
{
	public class VoiceChangerFlow : RelentlessMonoBehaviour
	{
		[SerializeField]
		private GameObject[] m_Potions;

		[SerializeField]
		private GameObject m_PotionItem;

		private BounceItem m_PotionBounceItem;

		private bool m_IsPotionAvailable;

		private bool m_HasPotionBeenSent;

		private FurbyCommand m_CurrentPotionCommand;

		[SerializeField]
		private float m_BoredWaitTime = 10f;

		[SerializeField]
		private float m_PotionReactionTime = 2f;

		[SerializeField]
		private ErrorMessageBox m_ErrorBox;

		private FurbyComAirWaiter m_ComAirWaiter;

		[SerializeField]
		private GameObject m_ResetButton;

		[SerializeField]
		public HintState m_HintStateSelectPotion = new HintState();

		[SerializeField]
		public HintState m_HintStateSendPotion = new HintState();

		[SerializeField]
		public HintState m_HintStateGoBack = new HintState();

		private GameEventSubscription m_DebugPanelSub;

		private void Awake()
		{
			if (m_PotionItem == null)
			{
				Logging.LogError("No Potion Item Reference!");
			}
			BounceItem bounceItem = (m_PotionBounceItem = m_PotionItem.GetComponentInChildren<BounceItem>());
			if (m_PotionBounceItem == null)
			{
				Logging.LogError("No Potion BounceItem!");
			}
			bounceItem.OnGiven += OnPotionGiven;
			bounceItem.OnBounce += OnPotionBounced;
			bounceItem.OnFlicked += OnPotionFlicked;
			bounceItem.OnTimeOut += OnPotionTimedOut;
			m_ComAirWaiter = new FurbyComAirWaiter();
			if (m_ResetButton == null)
			{
				Logging.LogError("No Reset Button Reference!");
			}
			VoiceChangerResetButton componentInChildren = m_ResetButton.GetComponentInChildren<VoiceChangerResetButton>();
			if (componentInChildren == null)
			{
				Logging.LogError("No VoiceChanger Reset Button on Reset Button!");
			}
			componentInChildren.OnReset += OnResetClicked;
		}

		private void ResetCommunications()
		{
			Singleton<FurbyDataChannel>.Instance.SetConnectionTone(FurbyCommand.Application);
			Singleton<FurbyDataChannel>.Instance.SetHeartBeatActive(true);
		}

		private void ResetState()
		{
			ShowPotions();
			HidePotionBounceItem();
			HideResetButton();
			m_HintStateSendPotion.Disable();
			m_HintStateGoBack.Disable();
		}

		private void ShowPotions()
		{
			GameObject[] potions = m_Potions;
			foreach (GameObject gameObject in potions)
			{
				gameObject.SetActive(true);
			}
		}

		private void HidePotions()
		{
			GameObject[] potions = m_Potions;
			foreach (GameObject gameObject in potions)
			{
				gameObject.SetActive(false);
			}
		}

		public void Update()
		{
			HandleInvocationOfHints();
		}

		private void HandleInvocationOfHints()
		{
			if (m_HintStateSelectPotion.IsEnabled())
			{
				m_HintStateSelectPotion.TestAndBroadcastState();
			}
		}

		private IEnumerator Start()
		{
			GameEventRouter.SendEvent(VoiceChangerEvent.VoiceChangerUtilityStarted, null, null);
			ResetCommunications();
			ResetState();
			WaitForGameEvent waiter = new WaitForGameEvent();
			while (true)
			{
				EnableAppropriateHints();
				yield return StartCoroutine(waiter.WaitForEvent(VoiceChangerEvent.PotionItemClicked));
				m_HintStateSelectPotion.Disable();
				VoiceChangerEvent voiceChangerEvent = (VoiceChangerEvent)(object)waiter.ReturnedEvent;
				if (voiceChangerEvent == VoiceChangerEvent.PotionItemClicked)
				{
					yield return StartCoroutine(HandlePotionItemClicked(waiter));
				}
				else
				{
					Logging.LogError(string.Format("Unexpected event in VoiceChangerFlow::Start : {0}", voiceChangerEvent.ToString()));
				}
			}
		}

		public void EnableAppropriateHints()
		{
			m_HintStateSelectPotion.Enable();
		}

		private IEnumerator HandlePotionItemClicked(WaitForGameEvent waiter)
		{
			GameEventRouter.SendEvent(VoiceChangerEvent.PotionItemSelected, null, null);
			VoiceChangerPotion potion = (VoiceChangerPotion)waiter.ReturnedParameters[0];
			HidePotions();
			ShowPotionItem(potion);
			ShowResetButton();
			if (!FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				bool shouldWaitForResponse = false;
				yield return StartCoroutine(SendComAirCommandAndWaitForResponse(FurbyCommand.VoiceNormal, shouldWaitForResponse));
			}
			yield return StartCoroutine(WaitForPotionToBeSent());
		}

		private IEnumerator WaitForPotionToBeSent()
		{
			StartCoroutine("BoredLoop");
			m_HintStateGoBack.Disable();
			m_HintStateSendPotion.Enable();
			while (m_IsPotionAvailable)
			{
				m_HintStateSendPotion.TestAndBroadcastState();
				m_HintStateGoBack.TestAndBroadcastState();
				yield return null;
			}
			m_HintStateGoBack.Disable();
			m_HintStateSendPotion.Disable();
			GameEventRouter.SendEvent(HintEvents.DeactivateAll);
			if (m_HasPotionBeenSent)
			{
				if (FurbyGlobals.Player.NoFurbyOnSaveGame())
				{
					yield return StartCoroutine(ShowNoFurbyMessage());
					ResetState();
				}
				else
				{
					yield return StartCoroutine(DoVoiceChangingSequence());
				}
			}
		}

		private IEnumerator BoredLoop()
		{
			FurbyGlobals.InputInactivity.ResetInactivity();
			while (true)
			{
				yield return new WaitForSeconds(m_BoredWaitTime);
				GameEventRouter.SendEvent(VoiceChangerEvent.FurbyBoredOfWaitingForPotion, null, null);
				Logging.Log("Furby is bored waiting for the potion!");
			}
		}

		private IEnumerator DoVoiceChangingSequence()
		{
			GameEventRouter.SendEvent(VoiceChangerEvent.FurbyReactingToPotion, null, null);
			bool shouldWaitForResponse = false;
			yield return StartCoroutine(SendComAirActionAndWaitForResponse(FurbyAction.Slurp, shouldWaitForResponse));
			yield return new WaitForSeconds(m_PotionReactionTime);
			shouldWaitForResponse = true;
			yield return StartCoroutine(SendComAirCommandAndWaitForResponse(m_CurrentPotionCommand, shouldWaitForResponse));
			yield return new WaitForSeconds(m_PotionReactionTime);
			if (!m_ComAirWaiter.ReceivedResponse())
			{
				Logging.Log("NO Response Received");
				yield return StartCoroutine(ShowCommunicationsErrorMessage());
				ResetState();
			}
			else
			{
				shouldWaitForResponse = false;
				yield return StartCoroutine(SendComAirActionAndWaitForResponse(FurbyAction.Surprised, shouldWaitForResponse));
				ResetState();
			}
		}

		private IEnumerator SendComAirActionAndWaitForResponse(FurbyAction action, bool shouldWaitForResponse)
		{
			yield return StartCoroutine(m_ComAirWaiter.SendComAirEventAndWaitForResponse(action, (FurbyCommand)0, shouldWaitForResponse));
		}

		private IEnumerator SendComAirCommandAndWaitForResponse(FurbyCommand command, bool shouldWaitForResponse)
		{
			yield return StartCoroutine(m_ComAirWaiter.SendComAirEventAndWaitForResponse((FurbyAction)0, command, shouldWaitForResponse));
		}

		private IEnumerator ShowCommunicationsErrorMessage()
		{
			GameEventRouter.SendEvent(VoiceChangerEvent.CommunicationsError, null, null);
			HideResetButton();
			m_ErrorBox.SetOKState("FURBYCOMMSERROR_MESSAGE", "MENU_OPTION_OK", VoiceChangerEvent.ErrorDialogOK);
			m_ErrorBox.Show(true);
			WaitForGameEvent waiter = new WaitForGameEvent();
			yield return StartCoroutine(waiter.WaitForEvent(VoiceChangerEvent.ErrorDialogOK));
			m_ErrorBox.Hide();
		}

		private IEnumerator ShowNoFurbyMessage()
		{
			GameEventRouter.SendEvent(VoiceChangerEvent.NoFurby, null, null);
			yield break;
		}

		private void ShowPotionItem(VoiceChangerPotion potion)
		{
			Vector3 position = potion.gameObject.transform.position;
			m_PotionBounceItem.Activate(potion.GetGraphicsAtlas(), potion.GetGraphicsName(), position);
			m_IsPotionAvailable = true;
			m_HasPotionBeenSent = false;
			SetCurrentPotionCommand(potion.GetPotionCommand());
		}

		private void SetCurrentPotionCommand(FurbyCommand potionCommand)
		{
			m_CurrentPotionCommand = potionCommand;
		}

		private void HidePotionBounceItem()
		{
			m_PotionBounceItem.Deactivate();
			m_IsPotionAvailable = false;
			m_HasPotionBeenSent = false;
		}

		private void ShowResetButton()
		{
			m_ResetButton.SetActive(true);
		}

		private void HideResetButton()
		{
			m_ResetButton.SetActive(false);
		}

		private void OnPotionGiven()
		{
			Logging.Log("OnPotionGiven");
			GameEventRouter.SendEvent(VoiceChangerEvent.PotionGiven, null, null);
			HidePotionBounceItem();
			m_HasPotionBeenSent = true;
			StopBoredLoop();
		}

		private void StopBoredLoop()
		{
			StopCoroutine("BoredLoop");
			GameEventRouter.SendEvent(HintEvents.DeactivateAll);
			FurbyGlobals.InputInactivity.ResetInactivity();
			m_HintStateGoBack.Disable();
			m_HintStateSendPotion.Disable();
		}

		private void OnPotionBounced()
		{
			GameEventRouter.SendEvent(VoiceChangerEvent.PotionBounced, null, null);
		}

		private void OnPotionFlicked()
		{
			GameEventRouter.SendEvent(VoiceChangerEvent.PotionFlicked, null, null);
		}

		private void OnPotionTimedOut()
		{
			Logging.Log("OnPotionTimedOut");
			GameEventRouter.SendEvent(VoiceChangerEvent.PotionTimedOut, null, null);
			StopBoredLoop();
			ResetState();
		}

		private void OnResetClicked()
		{
			Logging.Log("OnResetClicked");
			StopAllCoroutines();
			ResetState();
			StartCoroutine(Start());
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
			if (DebugPanel.StartSection("VoiceChanger"))
			{
			}
			DebugPanel.EndSection();
		}
	}
}
