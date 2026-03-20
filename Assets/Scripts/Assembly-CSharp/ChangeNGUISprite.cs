using UnityEngine;

public class ChangeNGUISprite : ChangeLookAndFeel
{
	public UIAtlas m_atlas;

	public string m_spriteName = string.Empty;

	public Color m_colour = Color.white;

	protected override void OnChangeTheme()
	{
		UISprite component = base.gameObject.GetComponent<UISprite>();
		if ((bool)m_atlas)
		{
			component.atlas = m_atlas;
		}
		if (!string.IsNullOrEmpty(m_spriteName))
		{
			component.spriteName = m_spriteName;
		}
		component.color = m_colour;
	}
}
