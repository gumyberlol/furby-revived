using Relentless;
using UnityEngine;

namespace Furby
{
	public class ErrorMessageBox : MonoBehaviour
	{
		private UIPanel m_Panel;

		[SerializeField]
		private GameObject m_ErrorMessage;

		[SerializeField]
		private GameObject m_AcceptButton;

		[SerializeField]
		private GameObject m_CancelButton;

		[SerializeField]
		private GameObject m_OKButton;

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
			DisableNode(m_AcceptButton);
			DisableNode(m_CancelButton);
			DisableNode(m_OKButton);
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

		private void SetText(GameObject button, string text)
		{
			UILabel[] componentsInChildren = button.GetComponentsInChildren<UILabel>();
			foreach (UILabel uILabel in componentsInChildren)
			{
				uILabel.text = text;
			}
		}

		public void SetAcceptCancelState(string dialogNamedText, string acceptNamedText, SerialisableEnum acceptGameEvent, string cancelNamedText, SerialisableEnum cancelGameEvent)
		{
			DisableAll();
			EnableNode(m_AcceptButton);
			EnableNode(m_CancelButton);
			SetLocalisedText(m_ErrorMessage, dialogNamedText);
			SetLocalisedText(m_AcceptButton, acceptNamedText);
			SetGameEvent(m_AcceptButton, acceptGameEvent);
			SetLocalisedText(m_CancelButton, cancelNamedText);
			SetGameEvent(m_CancelButton, cancelGameEvent);
		}

		public void SetOKState(string dialogNamedText, string acceptNamedText, SerialisableEnum acceptGameEvent)
		{
			DisableAll();
			EnableNode(m_OKButton);
			SetLocalisedText(m_ErrorMessage, dialogNamedText);
			SetLocalisedText(m_OKButton, acceptNamedText);
			SetGameEvent(m_OKButton, acceptGameEvent);
		}
	}
}
