using UnityEngine;

public class FurMailCloseButton : MonoBehaviour
{
	[SerializeField]
	private FurMailMediator m_Mediator;

	private void OnClick()
	{
		m_Mediator.CloseMessage();
	}
}
