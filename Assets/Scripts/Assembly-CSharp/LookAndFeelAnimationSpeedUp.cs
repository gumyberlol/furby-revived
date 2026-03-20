using UnityEngine;

public class LookAndFeelAnimationSpeedUp : MonoBehaviour
{
	private const float SPEED_TO_BREAK = 40f;

	private const float MAX_SPEED = 50f;

	private Transform m_transform;

	private float m_speed;

	private float m_speedToAdd;

	private bool m_startedParticles;

	public GameObject m_particles;

	private void Awake()
	{
		m_transform = base.transform;
	}

	private void LateUpdate()
	{
		m_transform.Rotate(Vector3.forward * m_speed);
		m_speed += m_speedToAdd;
		if (m_speed > 40f && !m_startedParticles)
		{
			StartParticles();
		}
		if (m_speed > 50f)
		{
			m_speed = 50f;
		}
	}

	private void StartParticles()
	{
		m_particles.SetActive(true);
		m_startedParticles = true;
	}

	public void StartRotation()
	{
		m_speedToAdd = 0.25f;
	}
}
