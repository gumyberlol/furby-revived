using System;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class DialogPanel : CommonPanel
	{
		public enum PanelButtonCount
		{
			NoButtons = 0,
			OneButton = 1,
			TwoButtons = 2
		}

		[SerializeField]
		private SerialisableType GameEventType = typeof(SharedGuiEvents);

		public string DialogBoxName = string.Empty;

		[NamedText]
		public string MessageNamedText;

		public PanelButtonCount PanelType;

		[NamedText]
		public string AcceptButtonNamedText;

		[NamedText]
		public string CancelButtonNamedText;

		public SerialisableEnum AcceptButtonEvent;

		public SerialisableEnum CancelButtonEvent;

		public SerialisableEnum ShowEvent;

		public SerialisableEnum HideEvent;

		public SerialisableEnum CustomButtonEvent;

		[SerializeField]
		private bool m_UseCustomEvent;

		[SerializeField]
		private GameObject m_dialogText;

		[SerializeField]
		private GameObject m_acceptButton;

		[SerializeField]
		private GameObject m_cancelButton;

		[SerializeField]
		private GameObject m_okButton;

		[SerializeField]
		private GameObject m_closeButton;

		[SerializeField]
		private GameObject m_CustomButton;

		[SerializeField]
		private float m_timeToDisable;

		public bool m_triggerEffects;

		[SerializeField]
		private TriggerEffects[] m_effectsTriggers;

		[SerializeField]
		private float m_fontAndjustment = 0.2f;

		[SerializeField]
		private float m_backgroundHeightBorder = 30f;

		[SerializeField]
		private bool m_wantCloseButton;

		private HashSet<Type> m_types;

		public override Type EventType
		{
			get
			{
				return GameEventType.Type;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			m_types = new HashSet<Type>();
			m_types.Add(EventType);
			m_types.Add(AcceptButtonEvent.Type);
			m_types.Add(CancelButtonEvent.Type);
			m_types.Add(ShowEvent.Type);
			m_types.Add(HideEvent.Type);
			if (m_UseCustomEvent)
			{
				m_types.Add(CustomButtonEvent.Type);
			}
			m_types.Remove(EventType);
			foreach (Type type in m_types)
			{
				GameEventRouter.AddDelegateForType(type, OnEvent);
			}
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (!GameEventRouter.Exists)
			{
				return;
			}
			foreach (Type type in m_types)
			{
				GameEventRouter.RemoveDelegateForType(type, OnEvent);
			}
		}

		protected override void InternalSetEnabled(bool enabled)
		{
			base.InternalSetEnabled(enabled);
			if (enabled)
			{
				GameEventRouter.SendEvent(SharedGuiEvents.DialogWasShown);
			}
			if (m_triggerEffects)
			{
				TriggerEffects();
			}
		}

		public override void SetDisabled(float disableTime)
		{
			base.SetDisabled(disableTime);
			GameEventRouter.SendEvent(SharedGuiEvents.DialogWasHidden);
			ResetEffects();
		}

		public void TriggerEffects()
		{
			if (m_effectsTriggers.Length > 0)
			{
				TriggerEffects[] effectsTriggers = m_effectsTriggers;
				foreach (TriggerEffects triggerEffects in effectsTriggers)
				{
					triggerEffects.EffectsEmit();
				}
			}
		}

		public void ResetEffects()
		{
			if (m_effectsTriggers.Length > 0)
			{
				TriggerEffects[] effectsTriggers = m_effectsTriggers;
				foreach (TriggerEffects triggerEffects in effectsTriggers)
				{
					triggerEffects.EffectsReset();
				}
			}
		}

		protected override void OnEvent(Enum enumValue, GameObject gameObject, params object[] paramList)
		{
			if ((enumValue.Equals(AcceptButtonEvent.Value) || enumValue.Equals(CancelButtonEvent.Value) || enumValue.Equals(ShowEvent.Value) || enumValue.Equals(HideEvent.Value) || (m_UseCustomEvent && enumValue.Equals(CustomButtonEvent.Value))) && paramList.Length > 0)
			{
				string text = paramList[0] as string;
				if (DialogBoxName != string.Empty && text != null && DialogBoxName != text)
				{
					return;
				}
			}
			if (enumValue.Equals(AcceptButtonEvent.Value) && IsPanelEnabled())
			{
				SetDisabled(m_timeToDisable);
			}
			if (m_UseCustomEvent && enumValue.Equals(CustomButtonEvent.Value))
			{
				return;
			}
			if (enumValue.Equals(CancelButtonEvent.Value))
			{
				if (IsPanelEnabled())
				{
					SetDisabled(m_timeToDisable);
				}
			}
			else if (enumValue.Equals(ShowEvent.Value))
			{
				if (!IsPanelEnabled())
				{
					if (paramList.Length > 1)
					{
						string text2 = Singleton<Localisation>.Instance.GetText(MessageNamedText);
						m_dialogText.GetComponent<UILabel>().text = string.Format(text2, paramList.Skip((!(DialogBoxName == string.Empty)) ? 1 : 0).ToArray());
					}
					SetEnabled(true);
				}
			}
			else if (enumValue.Equals(HideEvent.Value) && IsPanelEnabled())
			{
				DebugUtils.Log_InOrange("Hide -> Cueing a Disable delay");
				SetDisabled(0f);
			}
		}

		public Bounds SetTextAndReturnBounds(GameObject button, string namedText)
		{
			Bounds result = default(Bounds);
			UILabel[] componentsInChildren = button.GetComponentsInChildren<UILabel>();
			foreach (UILabel uILabel in componentsInChildren)
			{
				uILabel.text = namedText;
				Vector3 localPosition = uILabel.transform.localPosition;
				localPosition.y += uILabel.transform.localScale.y * m_fontAndjustment;
				uILabel.transform.localPosition = localPosition;
				result.Encapsulate(NGUIMath.CalculateRelativeWidgetBounds(uILabel.transform));
			}
			return result;
		}

		public void SetBackgroundHeight(GameObject button, float height)
		{
			UISprite[] componentsInChildren = button.GetComponentsInChildren<UISprite>();
			foreach (UISprite uISprite in componentsInChildren)
			{
				Vector3 localScale = uISprite.transform.localScale;
				localScale.y += height;
				uISprite.transform.localScale = localScale;
			}
		}

		public new void Start()
		{
			base.Start();
			m_acceptButton.SetActive(PanelType == PanelButtonCount.TwoButtons);
			m_cancelButton.SetActive(PanelType == PanelButtonCount.TwoButtons);
			m_okButton.SetActive(PanelType == PanelButtonCount.OneButton);
			if ((bool)m_closeButton)
			{
				m_closeButton.SetActive(PanelType == PanelButtonCount.OneButton || m_wantCloseButton);
			}
			if ((bool)m_CustomButton && m_UseCustomEvent)
			{
				m_CustomButton.SetActive(true);
				m_CustomButton.GetComponent<GameEventOnNGUIEvent>().GameEvent = CustomButtonEvent;
			}
			m_acceptButton.GetComponent<GameEventOnNGUIEvent>().GameEvent = AcceptButtonEvent;
			m_cancelButton.GetComponent<GameEventOnNGUIEvent>().GameEvent = CancelButtonEvent;
			m_okButton.GetComponent<GameEventOnNGUIEvent>().GameEvent = CancelButtonEvent;
			Bounds bounds = default(Bounds);
			SetTextAndReturnBounds(m_dialogText, Singleton<Localisation>.Instance.GetText(MessageNamedText));
			bounds.Encapsulate(SetTextAndReturnBounds(m_acceptButton, Singleton<Localisation>.Instance.GetText(AcceptButtonNamedText)));
			bounds.Encapsulate(SetTextAndReturnBounds(m_cancelButton, Singleton<Localisation>.Instance.GetText(CancelButtonNamedText)));
			bounds.Encapsulate(SetTextAndReturnBounds(m_okButton, Singleton<Localisation>.Instance.GetText(AcceptButtonNamedText)));
			float height = bounds.size.y + m_backgroundHeightBorder;
			SetBackgroundHeight(m_acceptButton, height);
			SetBackgroundHeight(m_cancelButton, height);
			SetBackgroundHeight(m_okButton, height);
		}
	}
}
