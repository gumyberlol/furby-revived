using Relentless;
using UnityEngine;

public class ChangeTiledGradient : ChangeNGUISprite
{
	public Color m_gradient1 = Color.white;

	public Color m_gradient2 = Color.white;

	protected override void OnChangeTheme()
	{
		base.OnChangeTheme();
		UIGradientTiledSprite component = GetComponent<UIGradientTiledSprite>();
		component.mGradient.mGradient1 = m_gradient1;
		component.mGradient.mGradient2 = m_gradient2;
	}
}
