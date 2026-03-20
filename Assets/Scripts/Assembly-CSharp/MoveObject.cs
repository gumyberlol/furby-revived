using UnityEngine;

public class MoveObject : MonoBehaviour
{
	public Transform m_Destination;

	public float m_TimeToTransition;

	public float m_Damping;

	private bool m_Activated;

	private float m_CurrentTime;

	public void ActivateTransition()
	{
		m_Activated = true;
	}

	private void Start()
	{
		m_Activated = false;
	}

	private void Update()
	{
		if (m_Activated)
		{
			m_CurrentTime += Time.smoothDeltaTime / m_TimeToTransition;
			Vector3 vector = (m_Destination.position - base.transform.position) * m_CurrentTime * (1f - m_Damping);
			base.transform.position += vector;
		}
		else
		{
			m_CurrentTime = 0f;
		}
	}
}
