using Relentless;
using UnityEngine;

namespace Furby
{
	public class CodeControlledDialogBox : MonoBehaviour
	{
		public enum DialogType
		{
			NoButtons = 0,
			SmallMessageOneButton = 1,
			OneButton = 2,
			TwoButtons = 3
		}

		public enum WidgetIdentifier
		{
			DialogText = 0,
			AcceptButton = 1,
			CancelButton = 2,
			OkButton = 3
		}

		private UIPanel m_Panel;

		[SerializeField]
		private GameObject m_Background;

		[SerializeField]
		private float m_NormalBackgroundScaleY = 400f;

		[SerializeField]
		private float m_NormalBackgroundPositionY = 35.5f;

		[SerializeField]
		private float m_NormalDialogTextPositionY = 45.5f;

		[SerializeField]
		private float m_NormalDialogOKButtonPositionY = -140f;

		[SerializeField]
		private float m_SmallBackgroundScaleY = 100f;

		[SerializeField]
		private float m_SmallBackgroundPositionY = 350f;

		[SerializeField]
		private float m_SmallDialogTextPositionY = 360f;

		[SerializeField]
		private float m_SmallDialogOKButtonPositionY = 340f;

		[SerializeField]
		private GameObject m_DialogText;

		[SerializeField]
		private GameObject m_AcceptButton;

		[SerializeField]
		private GameObject m_CancelButton;

		[SerializeField]
		private GameObject m_OkButton;

		public bool m_triggerEffects;

		[SerializeField]
		private TriggerEffects[] m_effectTriggers;

		[SerializeField]
		private GameObject m_FadeBackground;

		[SerializeField]
		private UICamera m_DisabledUICamera;

		public UILabel DialogText
		{
			get
			{
				return (!(m_DialogText != null)) ? null : m_DialogText.GetComponent<UILabel>();
			}
		}

		private void Awake()
		{
			m_Panel = GetComponent<UIPanel>();
		}

		public void SetDialogType(DialogType dialogType)
		{
			switch (dialogType)
			{
			case DialogType.NoButtons:
				m_AcceptButton.SetActive(false);
				m_CancelButton.SetActive(false);
				m_OkButton.SetActive(false);
				break;
			case DialogType.SmallMessageOneButton:
			case DialogType.OneButton:
				m_AcceptButton.SetActive(false);
				m_CancelButton.SetActive(false);
				m_OkButton.SetActive(true);
				break;
			case DialogType.TwoButtons:
				m_AcceptButton.SetActive(true);
				m_CancelButton.SetActive(true);
				m_OkButton.SetActive(false);
				break;
			}
			Vector3 localScale = m_Background.transform.localScale;
			Vector3 localPosition = m_Background.transform.localPosition;
			Vector3 localPosition2 = m_DialogText.transform.localPosition;
			Vector3 localPosition3 = m_OkButton.transform.localPosition;
			if (dialogType == DialogType.SmallMessageOneButton)
			{
				localScale.y = m_SmallBackgroundScaleY;
				localPosition.y = m_SmallBackgroundPositionY;
				localPosition2.y = m_SmallDialogTextPositionY;
				localPosition3.y = m_SmallDialogOKButtonPositionY;
				HideFadeBackground();
			}
			else
			{
				localScale.y = m_NormalBackgroundScaleY;
				localPosition.y = m_NormalBackgroundPositionY;
				localPosition2.y = m_NormalDialogTextPositionY;
				localPosition3.y = m_NormalDialogOKButtonPositionY;
				ShowFadeBackground();
			}
			m_Background.transform.localScale = localScale;
			m_Background.transform.localPosition = localPosition;
			m_DialogText.transform.localPosition = localPosition2;
			m_OkButton.transform.localPosition = localPosition3;
		}

		public void SetLocalisedText(WidgetIdentifier identifier, string namedText)
		{
			SetText(identifier, Singleton<Localisation>.Instance.GetText(namedText));
		}

		public void SetLocalisedTextWithSubstitutedBabyName(WidgetIdentifier identifier, string namedText, string babyName)
		{
			string text = Singleton<Localisation>.Instance.GetText(namedText);
			string text2 = string.Format(text, babyName);
			SetText(identifier, text2);
		}

		private void SetText(WidgetIdentifier identifier, string text)
		{
			switch (identifier)
			{
			case WidgetIdentifier.DialogText:
				SetText(m_DialogText, text);
				break;
			case WidgetIdentifier.AcceptButton:
				SetText(m_AcceptButton, text);
				break;
			case WidgetIdentifier.CancelButton:
				SetText(m_CancelButton, text);
				break;
			case WidgetIdentifier.OkButton:
				SetText(m_OkButton, text);
				break;
			}
		}

		private void SetText(GameObject button, string text)
		{
			UILabel[] componentsInChildren = button.GetComponentsInChildren<UILabel>();
			foreach (UILabel uILabel in componentsInChildren)
			{
				uILabel.text = text;
			}
		}

		public void SetGameEvent(WidgetIdentifier identifier, SerialisableEnum gameEvent)
		{
			switch (identifier)
			{
			case WidgetIdentifier.AcceptButton:
				SetGameEvent(m_AcceptButton, gameEvent);
				break;
			case WidgetIdentifier.CancelButton:
				SetGameEvent(m_CancelButton, gameEvent);
				break;
			case WidgetIdentifier.OkButton:
				SetGameEvent(m_OkButton, gameEvent);
				break;
			}
		}

		private void SetGameEvent(GameObject button, SerialisableEnum gameEvent)
		{
			button.GetComponent<GameEventOnNGUIEvent>().GameEvent = gameEvent;
		}

		private void ShowFadeBackground()
		{
			m_FadeBackground.SetActive(true);
		}

		private void HideFadeBackground()
		{
			m_FadeBackground.SetActive(false);
		}

		public void Show(bool disableCamera)
		{
			m_Panel.enabled = true;
			GameEventRouter.SendEvent(SharedGuiEvents.DialogWasShown);
			GameEventRouter.SendEvent(SharedGuiEvents.MessageBoxAppear);
			if (disableCamera && m_DisabledUICamera != null)
			{
				m_DisabledUICamera.enabled = false;
			}
			if (m_triggerEffects && m_effectTriggers.Length > 0)
			{
				TriggerEffects[] effectTriggers = m_effectTriggers;
				foreach (TriggerEffects triggerEffects in effectTriggers)
				{
					triggerEffects.EffectsEmit();
				}
			}
		}

		public void Hide()
		{
			m_Panel.enabled = false;
			GameEventRouter.SendEvent(SharedGuiEvents.DialogWasHidden);
			GameEventRouter.SendEvent(SharedGuiEvents.MessageBoxDisappear);
			if (m_DisabledUICamera != null)
			{
				m_DisabledUICamera.enabled = true;
			}
			if (m_effectTriggers.Length > 0)
			{
				TriggerEffects[] effectTriggers = m_effectTriggers;
				foreach (TriggerEffects triggerEffects in effectTriggers)
				{
					triggerEffects.EffectsReset();
				}
			}
			SetDialogType(DialogType.NoButtons);
		}
	}
}
