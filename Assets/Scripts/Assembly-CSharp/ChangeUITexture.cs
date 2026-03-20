using UnityEngine;

public class ChangeUITexture : ChangeLookAndFeel
{
	public Texture2D m_NewTexture;

	protected override void OnChangeTheme()
	{
		UITexture component = base.gameObject.GetComponent<UITexture>();
		component.mainTexture = m_NewTexture;
	}
}
