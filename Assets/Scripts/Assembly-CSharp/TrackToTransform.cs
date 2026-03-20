using UnityEngine;

public class TrackToTransform : MonoBehaviour
{
	public bool m_enabled = true;

	private Vector3 m_initPos = Vector3.zero;

	private bool m_setToInitPos;

	private Vector3 m_offset = Vector3.zero;

	public Transform m_targetTransform;

	private void Awake()
	{
		m_initPos = base.transform.position;
		m_offset = m_initPos - m_targetTransform.position;
	}

	private void Update()
	{
		if (m_enabled)
		{
			base.transform.position = m_targetTransform.position + m_offset;
			if (m_setToInitPos)
			{
				m_setToInitPos = false;
			}
		}
		else if (!m_setToInitPos)
		{
			base.transform.position = m_initPos;
			m_setToInitPos = true;
		}
	}
}
