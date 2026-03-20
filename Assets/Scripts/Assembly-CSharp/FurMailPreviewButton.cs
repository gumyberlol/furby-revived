using Furby.Scripts.FurMail;
using UnityEngine;

public class FurMailPreviewButton : MonoBehaviour
{
	[SerializeField]
	private GameObject m_ReadRoot;

	[SerializeField]
	private GameObject m_UnReadRoot;

	[SerializeField]
	private GameObject m_UnReadSparkle;

	private float m_HideParticlesY = 0.7f;

	private int m_MessageIndex = -1;

	private FurMailMediator m_Mediator;

	public void SetMessage(int messageIndex, FurMailMediator mediator, FurMailMessage message)
	{
		m_MessageIndex = messageIndex;
		m_Mediator = mediator;
		UILabel componentInChildren = base.transform.gameObject.GetComponentInChildren<UILabel>();
		componentInChildren.text = message.MessageSubject;
		UpdateVisualState(message);
	}

	public void UpdateVisualState(FurMailMessage message)
	{
		if (message.IsRead)
		{
			m_ReadRoot.SetActive(true);
			m_UnReadRoot.SetActive(false);
		}
		else
		{
			m_ReadRoot.SetActive(false);
			m_UnReadRoot.SetActive(true);
		}
	}

	private void OnClick()
	{
		bool snap = true;
		m_Mediator.ShowMessage(m_MessageIndex, snap);
	}

	private void Update()
	{
		if (m_UnReadRoot.activeSelf)
		{
			if (base.transform.position.y > m_HideParticlesY)
			{
				m_UnReadSparkle.SetActive(false);
			}
			else
			{
				m_UnReadSparkle.SetActive(true);
			}
		}
	}
}
