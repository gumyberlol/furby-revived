using UnityEngine;

public class ChangeTransform : ChangeLookAndFeel
{
	public Vector3 m_localPosition;

	public Quaternion m_localRotation;

	public Vector3 m_localScale;

	protected override void OnChangeTheme()
	{
		base.transform.localPosition = m_localPosition;
		base.transform.localRotation = m_localRotation;
		base.transform.localScale = m_localScale;
	}
}
