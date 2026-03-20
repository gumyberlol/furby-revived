using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class LevelUpDisplay : RelentlessMonoBehaviour
	{
		[SerializeField]
		private UISlider m_progressSlider;

		[SerializeField]
		private float m_sections = 11f;

		[SerializeField]
		private float m_levelSectionPause = 0.25f;

		[SerializeField]
		private GameObject[] m_eggAvailableObjects;

		private int m_currentLevel;

		private int m_currentXP;

		private int m_currentSection;

		private GameEventSubscription m_levelUpSubscription;

		private void Start()
		{
			m_currentLevel = FurbyGlobals.Player.Level;
			m_currentXP = FurbyGlobals.Player.XP;
			float num = GetLevelCompletion(m_currentXP, m_currentLevel);
			m_currentSection = Mathf.RoundToInt(num * m_sections);
			if (num == 0f && FurbyGlobals.Player.NumEggsAvailable > 0)
			{
				num = 1f;
			}
			float num2 = Mathf.RoundToInt(num * m_sections);
			m_progressSlider.sliderValue = num2 / m_sections;
			m_levelUpSubscription = new GameEventSubscription(OnGainedXP, PlayerFurbyEvent.AdultGainedXP);
			UpdateEggAvailableObjects();
		}

		private void UpdateEggAvailableObjects()
		{
			bool flag = false;
			bool flag2 = FurbyGlobals.Player.NumEggsAvailable > 0;
			GameObject[] eggAvailableObjects = m_eggAvailableObjects;
			foreach (GameObject gameObject in eggAvailableObjects)
			{
				flag |= gameObject.activeSelf;
				gameObject.SetActive(flag2);
			}
			if (flag2 && !flag)
			{
				StartCoroutine(EmitEggSparkling());
			}
		}

		private IEnumerator EmitEggSparkling()
		{
			yield return null;
			base.gameObject.SendGameEvent(DashboardGameEvent.Egg_Sparkling);
		}

		private void OnDestroy()
		{
			if (m_levelUpSubscription != null)
			{
				m_levelUpSubscription.Dispose();
			}
		}

		private void OnGainedXP(Enum enumValue, GameObject originator, params object[] objects)
		{
			StartCoroutine(AnimateXPIncrease());
		}

		private IEnumerator AnimateXPIncrease()
		{
			yield return new WaitForSeconds(2f);
			float completion = Mathf.Clamp01(GetLevelCompletion(level: m_currentLevel, xp: FurbyGlobals.Player.XP));
			int endSection = Mathf.RoundToInt(completion * m_sections);
			for (int section = m_currentSection; section <= endSection; section++)
			{
				m_progressSlider.sliderValue = (float)section / m_sections;
				GameEventRouter.SendEvent(DashboardGameEvent.XP_Section_Increased);
				GameEventRouter.SendEvent((DashboardGameEvent)(145 + section));
				yield return new WaitForSeconds(m_levelSectionPause);
			}
			m_currentSection = 0;
			if (completion == 1f)
			{
				GameEventRouter.SendEvent(DashboardGameEvent.XP_Level_Increased);
				if ((bool)base.animation)
				{
					base.animation.Play();
				}
			}
			UpdateEggAvailableObjects();
		}

		private float GetLevelCompletion(int xp, int level)
		{
			float result = 1f;
			if (level < FurbyGlobals.AdultLibrary.XpLevels.Count - 1)
			{
				float num = FurbyGlobals.AdultLibrary.XpLevels[level];
				float num2 = FurbyGlobals.AdultLibrary.XpLevels[level + 1];
				result = ((float)xp - num) / (num2 - num);
			}
			return result;
		}
	}
}
