using System;
using UnityEngine;

[Serializable]
public class RotateTransform : MonoBehaviour
{
	public enum axis
	{
		x = 0,
		y = 1,
		z = 2
	}

	public float m_speed = 1f;

	public axis m_axis;

	private Transform m_transform;

	private void Start()
	{
		m_transform = base.transform;
	}

	private void Update()
	{
		switch (m_axis)
		{
		case axis.x:
			m_transform.Rotate(new Vector3(m_speed, 0f, 0f) * Time.deltaTime);
			break;
		case axis.y:
			m_transform.Rotate(new Vector3(0f, m_speed, 0f) * Time.deltaTime);
			break;
		case axis.z:
			m_transform.Rotate(new Vector3(0f, 0f, m_speed) * Time.deltaTime);
			break;
		}
	}
}
