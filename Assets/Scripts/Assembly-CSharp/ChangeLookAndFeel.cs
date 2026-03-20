using Relentless;
using UnityEngine;

public abstract class ChangeLookAndFeel : MonoBehaviour
{
	public AppLookAndFeel m_lookAndFeel = AppLookAndFeel.Crystal;

	private void Awake()
	{
		GameData data = Singleton<GameDataStoreObject>.Instance.Data;
		AppLookAndFeel appLookAndFeel = data.AppLookAndFeel;
		if (appLookAndFeel != AppLookAndFeel.Normal && data.CanPlayAppChangeAnimation)
		{
			appLookAndFeel = AppLookAndFeel.Normal;
		}
		if (appLookAndFeel == m_lookAndFeel)
		{
			OnChangeTheme();
		}
	}

	protected abstract void OnChangeTheme();
}
