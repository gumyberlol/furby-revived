using UnityEngine;

public class EnableObjectOnLookAndFeel : ChangeLookAndFeel
{
	public GameObject m_objectToEnable;

	protected override void OnChangeTheme()
	{
		if ((bool)m_objectToEnable)
		{
			m_objectToEnable.SetActive(true);
		}
	}
}
