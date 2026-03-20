using System;
using UnityEngine;

[Serializable]
public class EyeMovementLevers
{
	public float m_MaxEyeMovementRange = 5f;

	[SerializeField]
	[Range(0f, 0.5f)]
	public float m_TimeSecsForEyeToFocus = 0.025f;

	[Range(0f, 0.5f)]
	[SerializeField]
	public float m_TimeSecsForEyeToDefocus = 0.025f;

	[SerializeField]
	public bool m_EyesShouldCross;

	[SerializeField]
	public Vector3 m_FocusPosition = Vector3.zero;
}
