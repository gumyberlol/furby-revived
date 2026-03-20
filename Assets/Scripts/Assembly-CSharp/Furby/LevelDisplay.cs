using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class LevelDisplay : MonoBehaviour
	{
		[SerializeField]
		private string m_localisedStringKey = "DASHBOARD_TEXT_LEVEL";

		private GameEventSubscription m_levelUpSubscription;

		private void Start()
		{
			UpdateLevel();
			m_levelUpSubscription = new GameEventSubscription(OnLevelUp, DashboardGameEvent.XP_Level_Increased);
		}

		private void OnDestroy()
		{
			if (m_levelUpSubscription != null)
			{
				m_levelUpSubscription.Dispose();
				m_levelUpSubscription = null;
			}
		}

		private void OnLevelUp(Enum enumValue, GameObject originator, params object[] parameters)
		{
			UpdateLevel();
			if ((bool)base.animation)
			{
				base.animation.Play();
			}
		}

		private void UpdateLevel()
		{
			if (!Singleton<FurbyGlobals>.Instance)
			{
				return;
			}
			string text = FurbyGlobals.Localisation.GetText(m_localisedStringKey);
			UILabel componentInChildren = GetComponentInChildren<UILabel>();
			if ((bool)componentInChildren)
			{
				if (FurbyGlobals.Player.Furby == null)
				{
					componentInChildren.text = string.Empty;
				}
				else
				{
					componentInChildren.text = string.Format(text, FurbyGlobals.Player.Level);
				}
			}
		}
	}
}
