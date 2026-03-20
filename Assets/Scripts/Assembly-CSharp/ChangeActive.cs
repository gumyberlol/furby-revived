public class ChangeActive : ChangeLookAndFeel
{
	public bool m_activeState = true;

	protected override void OnChangeTheme()
	{
		base.gameObject.SetActive(m_activeState);
	}
}
