using Relentless;
using UnityEngine;

namespace Furby
{
	public class CommsLevelMeter : MonoBehaviour
	{
		private void Start()
		{
			UpdateSliderValue();
		}

		public void UpdateSliderValue()
		{
			UISlider component = GetComponent<UISlider>();
			component.sliderValue = Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel;
		}

		private void OnSliderChange(float amt)
		{
			GameEventRouter.SendEvent(SettingsPageEvents.CommsSliderValueChanged, null, amt);
		}

		private void OnPress(bool pressed)
		{
			if (!pressed)
			{
				GameEventRouter.SendEvent(SettingsPageEvents.CommsSliderReleased);
				UISlider component = GetComponent<UISlider>();
				float sliderValue = component.sliderValue;
				Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel = sliderValue;
				Singleton<GameDataStoreObject>.Instance.GlobalData.CommsLevel_UserCustomized = true;
				Singleton<GameDataStoreObject>.Instance.GlobalData.ApplyCommsLevel();
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}
	}
}
