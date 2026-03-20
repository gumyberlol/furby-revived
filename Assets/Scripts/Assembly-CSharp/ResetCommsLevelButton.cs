using Furby;
using Relentless;
using UnityEngine;

public class ResetCommsLevelButton : MonoBehaviour
{
	[SerializeField]
	private CommsLevelMeter m_CommsLevelMeter;

	private void OnClick()
	{
		float comAirVolume = FurbyGlobals.DeviceSettings.DeviceProperties.m_ApplicationModifiers.m_ComAirVolume;
		Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel = comAirVolume;
		Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel_UserCustomized = false;
		Singleton<GameDataStoreObject>.Instance.GlobalData.ApplyCommsLevel();
		Singleton<GameDataStoreObject>.Instance.Save();
		m_CommsLevelMeter.UpdateSliderValue();
	}
}
