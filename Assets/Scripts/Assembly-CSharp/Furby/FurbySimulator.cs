using System;
using UnityEngine;

namespace Furby
{
	public class FurbySimulator : MonoBehaviour
	{
		public UICheckbox m_ActiveCheckBox;

		public UISlider m_FailureSlider;

		public UIPopupList m_NameDropDown;

		public UIPopupList m_PersonalityDropDown;

		public UISlider m_UniqueIDSlider;

		public UISlider m_ColourSlider;

		public UISlider m_HungerSlider;

		public UISlider m_HappynessSlider;

		public UISlider m_SicknessSlider;

		public UIPopupList m_SensorDropDown;

		public UILabel m_SentLabel;

		public UILabel m_ReceivedLabel;

		private FurbyStatus m_FurbyStatus;

		private FurbySensor m_FurbySensor;

		private long m_SentTone;

		private long m_ReceivedTone;

		public void Start()
		{
			m_ReceivedTone = -1L;
			m_SentTone = -1L;
			SetListOptions(m_SensorDropDown, FurbySensor.Tail);
			SetListOptions(m_PersonalityDropDown, FurbyPersonality.Gobbler);
			SetListOptions(m_NameDropDown, FurbyReceiveName.Ah_Bay);
			ComAirChannel.ComAirTick += OnTickComAir;
		}

		public void Update()
		{
			lock (this)
			{
				bool isChecked = m_ActiveCheckBox.isChecked;
				if (isChecked && m_ReceivedTone != -1)
				{
					FurbyMessageType messageType = FurbyDataChannel.GetMessageType(m_ReceivedTone);
					UpdateStatus();
					switch (messageType)
					{
					case FurbyMessageType.Action:
						m_SentTone = (long)m_FurbyStatus.Personality;
						break;
					case FurbyMessageType.Status:
						m_SentTone |= Convert.ToInt64(m_FurbyStatus.Pattern) - 610 << 11;
						m_SentTone |= Convert.ToInt64(m_FurbyStatus.Name) << 3;
						m_SentTone |= Convert.ToInt64(m_FurbyStatus.Personality) - 916 << 32;
						m_SentTone |= Convert.ToInt64(m_FurbyStatus.Sickness);
						m_SentTone |= Convert.ToInt64(m_FurbyStatus.Happyness) << 28;
						m_SentTone |= Convert.ToInt64(m_FurbyStatus.Fullness) << 24;
						break;
					case FurbyMessageType.Command:
						m_SentTone = (long)m_FurbyStatus.Personality;
						break;
					}
					DisplayMessage(m_ReceivedLabel, m_ReceivedTone);
					m_ReceivedTone = -1L;
				}
				if (isChecked && m_SentTone != -1)
				{
					DisplayMessage(m_SentLabel, m_SentTone);
					m_SentTone = -1L;
				}
			}
		}

		private void UpdateStatus()
		{
			m_FurbySensor = GetListOption<FurbySensor>(m_SensorDropDown);
			m_FurbyStatus.Pattern = (FurbyPattern)GetSliderValue(m_ColourSlider, 12f);
			m_FurbyStatus.Happyness = GetSliderValue(m_HappynessSlider, 100f);
			m_FurbyStatus.Fullness = GetSliderValue(m_HungerSlider, 100f);
			m_FurbyStatus.Sickness = GetSliderValue(m_SicknessSlider, 1f);
			m_FurbyStatus.Personality = GetListOption<FurbyPersonality>(m_PersonalityDropDown);
			m_FurbyStatus.Name = GetListOption<FurbyReceiveName>(m_NameDropDown);
		}

		public void OnTickComAir(ComAirChannel.Tone? tone)
		{
			lock (this)
			{
				if (m_ReceivedTone == -1 && tone.HasValue)
				{
					m_ReceivedTone = tone.Value.Inbound;
				}
			}
		}

		public void OnClickSensorButton()
		{
			if (m_ActiveCheckBox.isChecked)
			{
				UpdateStatus();
				m_SentTone = (long)m_FurbySensor;
			}
		}

		private int GetSliderValue(UISlider sliderControl, float scaleValue)
		{
			return Mathf.RoundToInt(sliderControl.sliderValue * scaleValue);
		}

		private void SetListOptions<Type>(UIPopupList popupList, Type selectedValue)
		{
			string[] names = Enum.GetNames(typeof(Type));
			foreach (string item in names)
			{
				popupList.items.Add(item);
			}
			popupList.selection = selectedValue.ToString();
		}

		private Type GetListOption<Type>(UIPopupList popupList)
		{
			return (Type)Enum.Parse(typeof(Type), popupList.selection);
		}

		private static void DisplayMessage(UILabel msgLabel, long code)
		{
			string format = "{0} ({1})";
			string messageName = FurbyDataChannel.GetMessageName(code);
			msgLabel.text = string.Format(format, code, messageName);
		}
	}
}
