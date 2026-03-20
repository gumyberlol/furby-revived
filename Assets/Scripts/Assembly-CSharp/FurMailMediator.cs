using System;
using System.Collections.Generic;
using Furby;
using Furby.Scripts.FurMail;
using Relentless;
using UnityEngine;

public class FurMailMediator : MonoBehaviour
{
	private struct MessageData
	{
		public FurMailMessage message;

		public FurMailPreviewButton button;

		public FurMailMessageContents contents;
	}

	[Serializable]
	public class SenderData
	{
		public string SpriteName = "GUI_furmail_portrait_ChvHrt01";

		public string BackgroundSpriteName = "GUI_furmail_BG_ChvHrt01";

		public string FurbyName = "NOO-BOO";
	}

	[SerializeField]
	private UIGrid m_MessagePreviewUIGrid;

	[SerializeField]
	private GameObject m_MessagePreviewPrefab;

	[SerializeField]
	private GameObject m_MessageDisplayRoot;

	[SerializeField]
	private GameObject m_MessageMovementRoot;

	[SerializeField]
	private UICenterOnChildWithSnap m_MessageContentCentreOnChild;

	[SerializeField]
	private GameObject m_MessageContentPrefab;

	[SerializeField]
	private UICamera[] m_CamerasToDisable;

	[SerializeField]
	private GameObject m_PreviousButton;

	[SerializeField]
	private GameObject m_NextButton;

	private List<MessageData> m_MessageData = new List<MessageData>();

	private int m_CurrentMessageIndex = -1;

	[SerializeField]
	private SenderData[] m_SenderData;

	private void Start()
	{
		GameEventRouter.SendEvent(FurMailEvents.Opened);
		m_MessageDisplayRoot.SetActive(false);
		if (SingletonInstance<FurMailManager>.Instance != null)
		{
			SingletonInstance<FurMailManager>.Instance.IterateMessages(ProcessFurMailMessage);
		}
	}

	public void ProcessFurMailMessage(FurMailMessage message)
	{
		AddMessage(message);
	}

	private void AddMessage(FurMailMessage message)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(m_MessagePreviewPrefab);
		gameObject.name = string.Format("{0:D4}_MessagePreview", m_MessageData.Count);
		gameObject.transform.parent = m_MessagePreviewUIGrid.transform;
		gameObject.transform.localScale = Vector3.one;
		FurMailPreviewButton componentInChildren = gameObject.GetComponentInChildren<FurMailPreviewButton>();
		componentInChildren.SetMessage(m_MessageData.Count, this, message);
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(m_MessageContentPrefab);
		gameObject2.name = string.Format("{0:D4}_MessageContent", m_MessageData.Count);
		gameObject2.transform.parent = m_MessageMovementRoot.transform;
		gameObject2.transform.localScale = Vector3.one;
		gameObject2.transform.localPosition = Vector3.zero + (m_MessageData.Count - 1) * new Vector3(560f, 0f, 0f);
		int num = FindSenderIndexForTemplate(message.TemplateName);
		SenderData senderData = m_SenderData[num];
		FurMailMessageContents component = gameObject2.GetComponent<FurMailMessageContents>();
		component.UpdateForNewMessage(message, senderData);
		MessageData item = new MessageData
		{
			message = message,
			button = componentInChildren,
			contents = component
		};
		m_MessageData.Add(item);
	}

	public void ShowMessage(int messageIndex, bool snap)
	{
		int num = m_CamerasToDisable.Length;
		for (int i = 0; i < num; i++)
		{
			m_CamerasToDisable[i].enabled = false;
		}
		MessageData messageData = m_MessageData[messageIndex];
		m_MessageDisplayRoot.gameObject.SetActive(true);
		m_MessageContentCentreOnChild.Recenter(messageData.contents.transform, snap);
	}

	public void UpdateButtonState(int messageIndex)
	{
		if (messageIndex == 0)
		{
			m_PreviousButton.SetActive(false);
		}
		else
		{
			m_PreviousButton.SetActive(true);
		}
		if (messageIndex == m_MessageData.Count - 1)
		{
			m_NextButton.SetActive(false);
		}
		else
		{
			m_NextButton.SetActive(true);
		}
		MessageData messageData = m_MessageData[messageIndex];
		if (!messageData.message.IsRead)
		{
			messageData.message.IsRead = true;
			if (SingletonInstance<FurMailManager>.Instance != null)
			{
				SingletonInstance<FurMailManager>.Instance.Save();
			}
			messageData.button.UpdateVisualState(messageData.message);
		}
		m_CurrentMessageIndex = messageIndex;
	}

	public void CloseMessage()
	{
		int num = m_CamerasToDisable.Length;
		for (int i = 0; i < num; i++)
		{
			m_CamerasToDisable[i].enabled = true;
		}
		m_MessageDisplayRoot.gameObject.SetActive(false);
	}

	public void ShowNextMessage()
	{
		if (m_CurrentMessageIndex != m_MessageData.Count - 1)
		{
			bool snap = false;
			ShowMessage(m_CurrentMessageIndex + 1, snap);
		}
	}

	public void ShowPreviousMessage()
	{
		if (m_CurrentMessageIndex != 0)
		{
			bool snap = false;
			ShowMessage(m_CurrentMessageIndex - 1, snap);
		}
	}

	private int FindSenderIndexForTemplate(string templateName)
	{
		int num = 0;
		SenderData[] senderData = m_SenderData;
		foreach (SenderData senderData2 in senderData)
		{
			if (senderData2.FurbyName == templateName)
			{
				return num;
			}
			num++;
		}
		return 0;
	}

	private void Update()
	{
		GameObject centeredObject = m_MessageContentCentreOnChild.centeredObject;
		for (int i = 0; i < m_MessageData.Count; i++)
		{
			if (centeredObject == m_MessageData[i].contents.gameObject)
			{
				UpdateButtonState(i);
				break;
			}
		}
	}
}
