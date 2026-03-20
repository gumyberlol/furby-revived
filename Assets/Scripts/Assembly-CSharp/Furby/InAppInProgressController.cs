using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class InAppInProgressController : MonoBehaviour
	{
		private GameEventSubscription m_EventSubscription;

		[SerializeField]
		private GameObject m_Panel;

		[SerializeField]
		private GameObject m_BlankingBackground;

		[SerializeField]
		private UILabel m_BodyLabel;

		[SerializeField]
		private GameObject m_ProgressBar;

		[SerializeField]
		private GameObject m_ExitButton;

		[SerializeField]
		private GameObject m_OKButton;

		[SerializeField]
		private GameObject m_RestoreTransactionsButton;

		private void Awake()
		{
			m_EventSubscription = new GameEventSubscription(typeof(InAppInProgressEvent), OnEventHandler);
		}

		private void OnDestroy()
		{
			m_EventSubscription.Dispose();
		}

		private void OnEventHandler(Enum eventType, GameObject originator, params object[] parameters)
		{
			InAppInProgressEvent inAppInProgressEvent = (InAppInProgressEvent)(object)eventType;
			DebugUtils.Log_InCyan("InAppInProgressController: Event=" + inAppInProgressEvent);
			switch (inAppInProgressEvent)
			{
			case InAppInProgressEvent.ShowInProgressDialog_ModalityMode:
				m_BlankingBackground.SetActive(true);
				if (m_RestoreTransactionsButton != null)
				{
					m_RestoreTransactionsButton.SetActive(false);
				}
				m_Panel.SetActive(true);
				m_ProgressBar.SetActive(false);
				m_OKButton.SetActive(true);
				m_ExitButton.SetActive(false);
				break;
			case InAppInProgressEvent.ShowInProgressDialog_ConfirmMode:
				m_BlankingBackground.SetActive(true);
				if (m_RestoreTransactionsButton != null)
				{
					m_RestoreTransactionsButton.SetActive(false);
				}
				m_Panel.SetActive(true);
				m_ProgressBar.SetActive(false);
				m_OKButton.SetActive(true);
				m_ExitButton.SetActive(true);
				break;
			case InAppInProgressEvent.ShowInProgressDialog_ProgressMode:
				m_BlankingBackground.SetActive(true);
				if (m_RestoreTransactionsButton != null)
				{
					m_RestoreTransactionsButton.SetActive(false);
				}
				m_Panel.SetActive(true);
				m_ProgressBar.SetActive(true);
				m_OKButton.SetActive(false);
				m_ExitButton.SetActive(false);
				break;
			case InAppInProgressEvent.HideInProgressDialog:
				m_BlankingBackground.SetActive(false);
				if (m_RestoreTransactionsButton != null)
				{
					m_RestoreTransactionsButton.SetActive(true);
				}
				m_Panel.SetActive(false);
				break;
			case InAppInProgressEvent.UpdateContent:
				if (parameters.Length == 1)
				{
					string text = parameters[0] as string;
					if (text != string.Empty)
					{
						DebugUtils.Log_InCyan("ORIGINATOR: " + originator.ToString());
						InAppShopItem component = originator.GetComponent<InAppShopItem>();
						if (component != null)
						{
							InAppItemData itemData = component.m_ItemData;
							string displayName = itemData.m_DisplayName;
							string text2 = Singleton<Localisation>.Instance.GetText(displayName);
							string text3 = Singleton<Localisation>.Instance.GetText(text);
							string text4 = string.Format(text3, text2);
							m_BodyLabel.text = text4;
						}
						else
						{
							m_BodyLabel.text = Singleton<Localisation>.Instance.GetText(text);
						}
						DebugUtils.Log_InCyan("Updated text... to:" + m_BodyLabel.text.ToString());
					}
					else
					{
						DebugUtils.Log_InCyan("Cant update, no text...");
					}
				}
				else
				{
					DebugUtils.Log_InCyan("Cant update, no params...");
				}
				break;
			}
		}
	}
}
