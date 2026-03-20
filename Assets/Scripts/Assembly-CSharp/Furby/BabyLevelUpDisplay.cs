using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class BabyLevelUpDisplay : RelentlessMonoBehaviour
	{
		[SerializeField]
		private UISlider m_progressSlider;

		[SerializeField]
		private XpSlider m_progressXPSlider;

		private float m_maxXP;

		private GameEventSubscription m_xpGainedSubscription;

		private void SetValue(float sliderValue)
		{
			if (m_progressSlider != null)
			{
				m_progressSlider.sliderValue = sliderValue;
			}
			if (m_progressXPSlider != null)
			{
				m_progressXPSlider.UpdateSlider(sliderValue);
			}
		}

		private void Start()
		{
			FurbyBaby inProgressFurbyBaby = FurbyGlobals.Player.InProgressFurbyBaby;
			if (inProgressFurbyBaby != null)
			{
				m_maxXP = FurbyGlobals.BabyLibrary.GetBabyFurby(inProgressFurbyBaby.Type).XpToLevelUp;
				SetValue(Mathf.Clamp01((float)inProgressFurbyBaby.XP / m_maxXP));
				m_xpGainedSubscription = new GameEventSubscription(OnGainedXP, PlayerFurbyEvent.BabyGainedXP);
			}
		}

		private void OnDestroy()
		{
			if (m_xpGainedSubscription != null)
			{
				m_xpGainedSubscription.Dispose();
			}
		}

		private void OnGainedXP(Enum enumValue, GameObject originator, params object[] objects)
		{
			SetValue(Mathf.Clamp01((float)FurbyGlobals.Player.InProgressFurbyBaby.XP / m_maxXP));
		}
	}
}
