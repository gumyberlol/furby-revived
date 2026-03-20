using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class VitalBarBasic : MonoBehaviour
	{
		public enum StatType
		{
			Happiness = 0,
			BowelEmptiness = 1,
			Cleanliness = 2,
			Satiatedness = 3
		}

		[SerializeField]
		private UISlider slider;

		[SerializeField]
		private Color[] Colors = new Color[3]
		{
			Color.red,
			Color.yellow,
			Color.green
		};

		[SerializeField]
		private int m_steps = 26;

		[SerializeField]
		private StatType m_statType;

		private void Start()
		{
			UpdateFromFurby(FurbyGlobals.Player);
		}

		private void OnEnable()
		{
			GameEventRouter.AddDelegateForType(typeof(PlayerFurbyEvent), ReceivePlayerFubyEvent);
		}

		private void OnDisable()
		{
			if (GameEventRouter.Exists)
			{
				GameEventRouter.RemoveDelegateForType(typeof(PlayerFurbyEvent), ReceivePlayerFubyEvent);
			}
		}

		private void ReceivePlayerFubyEvent(Enum eventType, GameObject gObj, params object[] parameters)
		{
			if ((PlayerFurbyEvent)(object)eventType == PlayerFurbyEvent.StatusUpdated)
			{
				PlayerFurby furby = (PlayerFurby)parameters[0];
				UpdateFromFurby(furby);
			}
		}

		private void UpdateFromFurby(PlayerFurby furby)
		{
			float x = 0f;
			switch (m_statType)
			{
			case StatType.Happiness:
				x = furby.Happiness;
				break;
			case StatType.BowelEmptiness:
				x = furby.BowelEmptiness;
				break;
			case StatType.Cleanliness:
				x = furby.Cleanliness;
				break;
			case StatType.Satiatedness:
				x = furby.Satiatedness;
				break;
			}
			UpdateDisplay(x);
		}

		public void UpdateDisplay(float x)
		{
			slider.sliderValue = Mathf.Clamp01(Mathf.Round(x * (float)m_steps) / (float)m_steps);
			slider.foreground.GetComponent<UISprite>().color = Colors[Mathf.Clamp((int)(x / 0.333f), 0, 2)];
		}
	}
}
