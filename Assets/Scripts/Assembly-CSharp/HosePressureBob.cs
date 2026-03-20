using UnityEngine;

public class HosePressureBob : MonoBehaviour
{
	public float m_frequency = 1f;

	public float m_magnitude = 1f;

	private Vector3 m_offset = Vector3.zero;

	private float m_time;

	private void LateUpdate()
	{
		m_time += Time.deltaTime * m_frequency;
		float z = Mathf.Sin(m_time) * m_magnitude;
		m_offset.z = z;
		base.transform.position += m_offset;
	}
}
