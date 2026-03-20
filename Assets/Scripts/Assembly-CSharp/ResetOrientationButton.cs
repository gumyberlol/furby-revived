using Furby;
using Relentless;
using UnityEngine;

public class ResetOrientationButton : MonoBehaviour
{
	private void OnClick()
	{
		Screen.orientation = ScreenOrientationHelper.GetCorrectOrientationForDevice();
		Singleton<GameDataStoreObject>.Instance.GlobalData.m_ScreenOrientation_UserCustomized = false;
		Singleton<GameDataStoreObject>.Instance.Save();
	}
}
