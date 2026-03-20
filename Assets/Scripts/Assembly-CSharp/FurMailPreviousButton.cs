using UnityEngine;

public class FurMailPreviousButton : MonoBehaviour
{
	[SerializeField]
	private FurMailMediator m_Mediator;

	private void OnClick()
	{
		m_Mediator.ShowPreviousMessage();
	}
}
