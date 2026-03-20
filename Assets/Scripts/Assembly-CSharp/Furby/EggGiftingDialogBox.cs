using Relentless;
using UnityEngine;

namespace Furby
{
	public class EggGiftingDialogBox : MonoBehaviour
	{
		private UIPanel m_Panel;

		[SerializeField]
		private GameObject m_Node01;

		[SerializeField]
		private GameObject m_Background;

		[SerializeField]
		private GameObject m_Node02;

		[SerializeField]
		private GameObject m_Node02AcceptButton;

		[SerializeField]
		private GameObject m_Node02CancelButton;

		[SerializeField]
		private BabyInstance m_Node02Egg;

		[SerializeField]
		private GameObject m_Node03;

		[SerializeField]
		private GameObject m_Node03AcceptButton;

		[SerializeField]
		private GameObject m_Node04;

		[SerializeField]
		private GameObject m_Node04CancelButton;

		[SerializeField]
		private GameObject m_Node05;

		[SerializeField]
		private GameObject m_Node05AcceptButton;

		[SerializeField]
		private GameObject m_Node05Banner;

		[SerializeField]
		private GameObject m_CloseButton;

		[SerializeField]
		private GameObject m_FadeBackground;

		[SerializeField]
		private UICamera m_DisabledUICamera;

		private void Awake()
		{
			m_Panel = GetComponent<UIPanel>();
		}

		private void DisableAll()
		{
			EnableNode(m_Node04CancelButton);
			DisableNode(m_Node01);
			DisableNode(m_Background);
			DisableNode(m_Node02);
			DisableNode(m_Node03);
			DisableNode(m_Node04);
			DisableNode(m_Node05);
			DisableNode(m_CloseButton);
			m_Node02Egg.Hide();
		}

		private void DisableNode(GameObject node)
		{
			node.SetActive(false);
		}

		private void EnableNode(GameObject node)
		{
			node.SetActive(true);
		}

		private void ShowFadeBackground()
		{
			m_FadeBackground.SetActive(true);
		}

		private void HideFadeBackground()
		{
			m_FadeBackground.SetActive(false);
		}

		public void Hide()
		{
			DisableAll();
			m_Panel.enabled = false;
			GameEventRouter.SendEvent(SharedGuiEvents.DialogWasHidden);
			GameEventRouter.SendEvent(SharedGuiEvents.MessageBoxDisappear);
			if (m_DisabledUICamera != null)
			{
				m_DisabledUICamera.enabled = true;
			}
		}

		public void Show(bool disableCamera)
		{
			m_Panel.enabled = true;
			GameEventRouter.SendEvent(SharedGuiEvents.DialogWasShown);
			GameEventRouter.SendEvent(SharedGuiEvents.MessageBoxAppear);
			if (disableCamera)
			{
				if (m_DisabledUICamera != null)
				{
					m_DisabledUICamera.enabled = false;
				}
				ShowFadeBackground();
			}
			else
			{
				HideFadeBackground();
			}
		}

		private void SetGameEvent(GameObject button, SerialisableEnum gameEvent)
		{
			button.GetComponent<GameEventOnNGUIEvent>().GameEvent = gameEvent;
		}

		public void SetLocalisedText(GameObject button, string namedText)
		{
			SetText(button, Singleton<Localisation>.Instance.GetText(namedText));
		}

		public void SetVirtualFurbyLocalisedTextWithSubstitution(string namedText, string substitution, string acceptNamedText, string cancelNamedText)
		{
			string text = Singleton<Localisation>.Instance.GetText(namedText);
			string text2 = string.Format(text, substitution);
			SetText(m_Node02, text2);
			SetLocalisedText(m_Node02AcceptButton, acceptNamedText);
			SetLocalisedText(m_Node02CancelButton, cancelNamedText);
		}

		private void SetText(GameObject button, string text)
		{
			UILabel[] componentsInChildren = button.GetComponentsInChildren<UILabel>();
			foreach (UILabel uILabel in componentsInChildren)
			{
				uILabel.text = text;
			}
		}

		public void SetChooseEggState(string dialogNamedText, SerialisableEnum closeGameEvent)
		{
			DisableAll();
			EnableNode(m_Node01);
			EnableNode(m_CloseButton);
			SetLocalisedText(m_Node01, dialogNamedText);
			SetGameEvent(m_CloseButton, closeGameEvent);
		}

		public void InitConfirmGiftEgg(FurbyBaby eggToSend)
		{
			m_Node02Egg.SetTargetFurbyBaby(eggToSend);
			m_Node02Egg.ForceInstantiateObject();
		}

		public bool IsEggReadyToBeRendered()
		{
			return m_Node02Egg.IsReadyToBeRendered();
		}

		public void SetConfirmGiftState(string dialogNamedText, string acceptNamedText, SerialisableEnum acceptGameEvent, string cancelNamedText, SerialisableEnum cancelGameEvent)
		{
			DisableAll();
			EnableNode(m_Background);
			EnableNode(m_Node02);
			SetLocalisedText(m_Node02, dialogNamedText);
			SetLocalisedText(m_Node02AcceptButton, acceptNamedText);
			SetGameEvent(m_Node02AcceptButton, acceptGameEvent);
			SetLocalisedText(m_Node02CancelButton, cancelNamedText);
			SetGameEvent(m_Node02CancelButton, cancelGameEvent);
			ShowEgg();
		}

		public void SetSendingAdviceState(string dialogNamedText, string acceptNamedText, SerialisableEnum acceptGameEvent)
		{
			DisableAll();
			EnableNode(m_Background);
			EnableNode(m_Node03);
			SetLocalisedText(m_Node03, dialogNamedText);
			SetLocalisedText(m_Node03AcceptButton, acceptNamedText);
			SetGameEvent(m_Node03AcceptButton, acceptGameEvent);
		}

		public void SetAcceptGiftState(string dialogNamedText, string acceptNamedText, SerialisableEnum acceptGameEvent, string cancelNamedText, SerialisableEnum cancelGameEvent)
		{
			DisableAll();
			EnableNode(m_Background);
			EnableNode(m_Node02);
			SetLocalisedText(m_Node02, dialogNamedText);
			SetLocalisedText(m_Node02AcceptButton, acceptNamedText);
			SetGameEvent(m_Node02AcceptButton, acceptGameEvent);
			SetLocalisedText(m_Node02CancelButton, cancelNamedText);
			SetGameEvent(m_Node02CancelButton, cancelGameEvent);
		}

		public void ShowEgg()
		{
			m_Node02Egg.Show();
		}

		public void SetSendingState(string dialogNamedText, string cancelNamedText, SerialisableEnum cancelGameEvent)
		{
			DisableAll();
			EnableNode(m_Background);
			EnableNode(m_Node04);
			SetLocalisedText(m_Node04, dialogNamedText);
			SetLocalisedText(m_Node04CancelButton, cancelNamedText);
			SetGameEvent(m_Node04CancelButton, cancelGameEvent);
		}

		public void SetReceivingState(string dialogNamedText)
		{
			DisableAll();
			EnableNode(m_Background);
			EnableNode(m_Node04);
			SetLocalisedText(m_Node04, dialogNamedText);
			DisableNode(m_Node04CancelButton);
		}

		public void SetReceivedState(string dialogNamedText, string bannerNamedText, string acceptNamedText, SerialisableEnum acceptGameEvent)
		{
			DisableAll();
			EnableNode(m_Background);
			EnableNode(m_Node05);
			SetLocalisedText(m_Node05, dialogNamedText);
			SetLocalisedText(m_Node05Banner, bannerNamedText);
			SetLocalisedText(m_Node05AcceptButton, acceptNamedText);
			SetGameEvent(m_Node05AcceptButton, acceptGameEvent);
		}
	}
}
