using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class StatBar : MonoBehaviour
	{
		[Serializable]
		public class ColourMapping
		{
			public Color Colour;

			public float MaxLevel;
		}

		[SerializeField]
		private FurbyStatEvent m_eventType;

		[SerializeField]
		private UISlider m_slider;

		[SerializeField]
		private ColourMapping[] m_colourMappings = new ColourMapping[3]
		{
			new ColourMapping
			{
				Colour = Color.red
			},
			new ColourMapping
			{
				Colour = Color.yellow,
				MaxLevel = 0.15f
			},
			new ColourMapping
			{
				Colour = Color.green,
				MaxLevel = 0.3f
			}
		};

		[SerializeField]
		private Color m_disabledBarColor = Color.grey;

		[SerializeField]
		private float m_steps = 15f;

		[SerializeField]
		private float m_stepsSize = 3f / 46f;

		[SerializeField]
		private float m_stepOffset = 0.051630434f;

		[SerializeField]
		private GameObject m_warningFlashObject;

		[SerializeField]
		private float m_warnLimit = 0.2f;

		private float m_currentValue;

		private GameEventSubscription m_eventSubs;

		private void OnEnable()
		{
			m_eventSubs = new GameEventSubscription(OnFurbyStatEvent, m_eventType);
			if (IsDisabled())
			{
				GameEventRouter.SendEvent(DashboardGameEvent.Meters_Deactivated);
				SetBarValue(0f, m_disabledBarColor);
			}
			else
			{
				GameEventRouter.SendEvent(DashboardGameEvent.Meters_Activated);
			}
		}

		private void OnDisable()
		{
			m_eventSubs.Dispose();
			m_eventSubs = null;
		}

		private bool IsDisabled()
		{
			if (m_eventType >= FurbyStatEvent.UpdateFurbySatietedness && m_eventType <= FurbyStatEvent.UpdateFurbyBowelEmptiness)
			{
				if (FurbyGlobals.Player.NoFurbyOnSaveGame())
				{
					return true;
				}
			}
			else if (FurbyGlobals.Player.InProgressFurbyBaby == null || FurbyGlobals.Player.InProgressFurbyBaby.Progress != FurbyBabyProgresss.P)
			{
				return true;
			}
			return false;
		}

		private void OnFurbyStatEvent(Enum evt, GameObject originator, params object[] parameters)
		{
			if (IsDisabled())
			{
				SetBarValue(0f, m_disabledBarColor);
				return;
			}
			float targetValue = (float)parameters[0];
			float timeToAnimate = (float)parameters[1];
			StartCoroutine(AnimateBar(targetValue, timeToAnimate));
		}

		private void SetBarValue(float value, Color colour)
		{
			m_slider.sliderValue = Mathf.Clamp01(Mathf.Round(value * m_steps) * m_stepsSize + m_stepOffset);
			m_slider.foreground.GetComponent<UISprite>().color = colour;
			if (m_warningFlashObject != null && !FurbyGlobals.Player.NoFurbyOnSaveGame())
			{
				m_warningFlashObject.SetActive(value <= m_warnLimit);
			}
		}

		private int GetColourIndex(float value)
		{
			for (int num = m_colourMappings.Length - 1; num >= 0; num--)
			{
				if (value >= m_colourMappings[num].MaxLevel)
				{
					return num;
				}
			}
			return 0;
		}

		private IEnumerator AnimateBar(float targetValue, float timeToAnimate)
		{
			float delta = m_currentValue - targetValue;
			float timeScale = 1f / timeToAnimate;
			int previousColourIndex = GetColourIndex(m_currentValue);
			if (delta < -1f / m_steps)
			{
				if (m_eventType < FurbyStatEvent.UpdateFurbyBabySatiatedness)
				{
					GameEventRouter.SendEvent(DashboardGameEvent.StatIncrease_Adult);
				}
				else
				{
					GameEventRouter.SendEvent(DashboardGameEvent.StatIncrease_Child);
				}
			}
			while (timeToAnimate > 0f)
			{
				timeToAnimate -= Time.deltaTime;
				float value = targetValue + delta * timeToAnimate * timeScale;
				int colourIndex = GetColourIndex(value);
				Color colour = m_colourMappings[colourIndex].Colour;
				if (colourIndex != previousColourIndex)
				{
					GameEventRouter.SendEvent((DashboardGameEvent)(80 + colourIndex));
					previousColourIndex = colourIndex;
				}
				SetBarValue(value, colour);
				yield return null;
			}
			SetBarValue(targetValue, m_colourMappings[GetColourIndex(targetValue)].Colour);
			m_currentValue = targetValue;
		}
	}
}
