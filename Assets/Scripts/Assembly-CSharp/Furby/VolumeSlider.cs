using Fabric;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class VolumeSlider : MonoBehaviour
	{
		private bool m_IsControllingGlobalInGameVolume;

		private UISlider m_Slider;

		private bool m_HasStarted;

		private void Start()
		{
			m_HasStarted = true;
			SyncSliderValueToSavedValue();
		}

		public void SetIsControllingGlobalInGameVolume(bool isControllingGlobalVolume)
		{
			m_IsControllingGlobalInGameVolume = isControllingGlobalVolume;
			SyncSliderValueToSavedValue();
			UpdateAudioVolume();
		}

		public void SyncSliderValueToSavedValue()
		{
			if (m_Slider == null)
			{
				m_Slider = GetComponent<UISlider>();
			}
			m_Slider.sliderValue = GetAudioVolume();
		}

		private float GetAudioVolume()
		{
			if (m_IsControllingGlobalInGameVolume)
			{
				return Singleton<GameDataStoreObject>.Instance.GlobalData.GetPreSaveGameLoadAudioVolume();
			}
			return Singleton<GameDataStoreObject>.Instance.Data.AudioVolume;
		}

		private void OnSliderChange(float amt)
		{
			if (m_HasStarted)
			{
				GameEventRouter.SendEvent(SettingsPageEvents.VolumeSliderValueChanged, null, amt);
				SetSavedValue(amt);
				UpdateAudioVolume();
			}
		}

		private void UpdateAudioVolume()
		{
			float audioVolume = GetAudioVolume();
			EventManager.Instance.PostEvent("all", EventAction.SetVolume, audioVolume);
		}

		private void SetSavedValue(float amt)
		{
			if (m_IsControllingGlobalInGameVolume)
			{
				Singleton<GameDataStoreObject>.Instance.GlobalData.SetPreSaveGameLoadAudioVolume(amt);
			}
			else
			{
				Singleton<GameDataStoreObject>.Instance.Data.AudioVolume = amt;
			}
		}

		private void OnPress(bool pressed)
		{
			if (!pressed)
			{
				GameEventRouter.SendEvent(SettingsPageEvents.VolumeSliderReleased);
				Singleton<GameDataStoreObject>.Instance.Save();
			}
		}
	}
}
