using UnityEngine;

public class FurMailNextButton : MonoBehaviour
{
	[SerializeField]
	private FurMailMediator m_Mediator;

	private void OnClick()
	{
		m_Mediator.ShowNextMessage();
	}
}
