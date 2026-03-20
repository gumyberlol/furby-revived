using Relentless;
using UnityEngine;

public class SpeakerAtBottomButton : MonoBehaviour
{
	private void OnClick()
	{
		switch (Screen.orientation)
		{
		case ScreenOrientation.Portrait:
			Screen.orientation = ScreenOrientation.PortraitUpsideDown;
			break;
		case ScreenOrientation.PortraitUpsideDown:
			Screen.orientation = ScreenOrientation.Portrait;
			break;
		}
		Singleton<GameDataStoreObject>.Instance.GlobalData.StoreCustomizedScreenOrientation(Screen.orientation);
		Singleton<GameDataStoreObject>.Instance.GlobalData.m_ScreenOrientation_UserCustomized = true;
		Singleton<GameDataStoreObject>.Instance.Save();
	}
}
