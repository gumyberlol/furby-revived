using System;
using System.Collections;
using Relentless;
using UnityEngine;

namespace Furby.Dashboard
{
	public class WerewolfModeLogic : RelentlessMonoBehaviour
	{
		private static bool s_IsEnabled = true;

		[SerializeField]
		private float m_WaitBetweenTriggerAttempts = 30f;

		private DateTime[] m_FullMoonTimes = new DateTime[259]
		{
			new DateTime(2013, 1, 27),
			new DateTime(2013, 2, 25),
			new DateTime(2013, 3, 27),
			new DateTime(2013, 4, 25),
			new DateTime(2013, 5, 25),
			new DateTime(2013, 6, 23),
			new DateTime(2013, 7, 22),
			new DateTime(2013, 8, 21),
			new DateTime(2013, 9, 19),
			new DateTime(2013, 10, 18),
			new DateTime(2013, 11, 17),
			new DateTime(2013, 12, 17),
			new DateTime(2014, 1, 16),
			new DateTime(2014, 2, 14),
			new DateTime(2014, 3, 16),
			new DateTime(2014, 4, 15),
			new DateTime(2014, 5, 14),
			new DateTime(2014, 6, 13),
			new DateTime(2014, 7, 12),
			new DateTime(2014, 8, 10),
			new DateTime(2014, 9, 9),
			new DateTime(2014, 10, 8),
			new DateTime(2014, 11, 6),
			new DateTime(2014, 12, 6),
			new DateTime(2015, 1, 5),
			new DateTime(2015, 2, 3),
			new DateTime(2015, 3, 5),
			new DateTime(2015, 4, 4),
			new DateTime(2015, 5, 4),
			new DateTime(2015, 6, 2),
			new DateTime(2015, 7, 2),
			new DateTime(2015, 7, 31),
			new DateTime(2015, 8, 29),
			new DateTime(2015, 9, 28),
			new DateTime(2015, 10, 27),
			new DateTime(2015, 11, 25),
			new DateTime(2015, 12, 25),
			new DateTime(2016, 1, 24),
			new DateTime(2016, 2, 22),
			new DateTime(2016, 3, 23),
			new DateTime(2016, 4, 22),
			new DateTime(2016, 5, 21),
			new DateTime(2016, 6, 20),
			new DateTime(2016, 7, 19),
			new DateTime(2016, 8, 18),
			new DateTime(2016, 9, 16),
			new DateTime(2016, 10, 16),
			new DateTime(2016, 11, 14),
			new DateTime(2016, 12, 14),
			new DateTime(2017, 1, 12),
			new DateTime(2017, 2, 11),
			new DateTime(2017, 3, 12),
			new DateTime(2017, 4, 11),
			new DateTime(2017, 5, 10),
			new DateTime(2017, 6, 9),
			new DateTime(2017, 7, 9),
			new DateTime(2017, 8, 7),
			new DateTime(2017, 9, 6),
			new DateTime(2017, 10, 5),
			new DateTime(2017, 11, 4),
			new DateTime(2017, 12, 3),
			new DateTime(2018, 1, 2),
			new DateTime(2018, 1, 31),
			new DateTime(2018, 3, 2),
			new DateTime(2018, 3, 31),
			new DateTime(2018, 4, 30),
			new DateTime(2018, 5, 29),
			new DateTime(2018, 6, 28),
			new DateTime(2018, 7, 27),
			new DateTime(2018, 8, 26),
			new DateTime(2018, 9, 25),
			new DateTime(2018, 10, 24),
			new DateTime(2018, 11, 23),
			new DateTime(2018, 12, 22),
			new DateTime(2019, 1, 21),
			new DateTime(2019, 2, 19),
			new DateTime(2019, 3, 21),
			new DateTime(2019, 4, 19),
			new DateTime(2019, 5, 18),
			new DateTime(2019, 6, 17),
			new DateTime(2019, 7, 16),
			new DateTime(2019, 8, 15),
			new DateTime(2019, 9, 14),
			new DateTime(2019, 10, 13),
			new DateTime(2019, 11, 12),
			new DateTime(2019, 12, 12),
			new DateTime(2020, 1, 10),
			new DateTime(2020, 2, 9),
			new DateTime(2020, 3, 9),
			new DateTime(2020, 4, 8),
			new DateTime(2020, 5, 7),
			new DateTime(2020, 6, 5),
			new DateTime(2020, 7, 5),
			new DateTime(2020, 8, 3),
			new DateTime(2020, 9, 2),
			new DateTime(2020, 10, 1),
			new DateTime(2020, 10, 31),
			new DateTime(2020, 11, 30),
			new DateTime(2020, 12, 30),
			new DateTime(2021, 1, 28),
			new DateTime(2021, 2, 27),
			new DateTime(2021, 3, 28),
			new DateTime(2021, 4, 27),
			new DateTime(2021, 5, 26),
			new DateTime(2021, 6, 24),
			new DateTime(2021, 7, 24),
			new DateTime(2021, 8, 22),
			new DateTime(2021, 9, 20),
			new DateTime(2021, 10, 20),
			new DateTime(2021, 11, 19),
			new DateTime(2021, 12, 19),
			new DateTime(2022, 1, 17),
			new DateTime(2022, 2, 16),
			new DateTime(2022, 3, 18),
			new DateTime(2022, 4, 16),
			new DateTime(2022, 5, 16),
			new DateTime(2022, 6, 14),
			new DateTime(2022, 7, 13),
			new DateTime(2022, 8, 12),
			new DateTime(2022, 9, 10),
			new DateTime(2022, 10, 9),
			new DateTime(2022, 11, 8),
			new DateTime(2022, 12, 8),
			new DateTime(2023, 1, 6),
			new DateTime(2023, 2, 5),
			new DateTime(2023, 3, 7),
			new DateTime(2023, 4, 6),
			new DateTime(2023, 5, 5),
			new DateTime(2023, 6, 4),
			new DateTime(2023, 7, 3),
			new DateTime(2023, 8, 1),
			new DateTime(2023, 8, 31),
			new DateTime(2023, 9, 29),
			new DateTime(2023, 10, 28),
			new DateTime(2023, 11, 27),
			new DateTime(2023, 12, 27),
			new DateTime(2024, 1, 25),
			new DateTime(2024, 2, 24),
			new DateTime(2024, 3, 25),
			new DateTime(2024, 4, 23),
			new DateTime(2024, 5, 23),
			new DateTime(2024, 6, 22),
			new DateTime(2024, 7, 21),
			new DateTime(2024, 8, 19),
			new DateTime(2024, 9, 18),
			new DateTime(2024, 10, 17),
			new DateTime(2024, 11, 15),
			new DateTime(2024, 12, 15),
			new DateTime(2025, 1, 13),
			new DateTime(2025, 2, 12),
			new DateTime(2025, 3, 14),
			new DateTime(2025, 4, 13),
			new DateTime(2025, 5, 12),
			new DateTime(2025, 6, 11),
			new DateTime(2025, 7, 10),
			new DateTime(2025, 8, 9),
			new DateTime(2025, 9, 7),
			new DateTime(2025, 10, 7),
			new DateTime(2025, 11, 5),
			new DateTime(2025, 12, 4),
			new DateTime(2026, 1, 3),
			new DateTime(2026, 2, 1),
			new DateTime(2026, 3, 3),
			new DateTime(2026, 4, 2),
			new DateTime(2026, 5, 1),
			new DateTime(2026, 5, 31),
			new DateTime(2026, 6, 29),
			new DateTime(2026, 7, 29),
			new DateTime(2026, 8, 28),
			new DateTime(2026, 9, 26),
			new DateTime(2026, 10, 26),
			new DateTime(2026, 11, 24),
			new DateTime(2026, 12, 24),
			new DateTime(2027, 1, 22),
			new DateTime(2027, 2, 20),
			new DateTime(2027, 3, 22),
			new DateTime(2027, 4, 20),
			new DateTime(2027, 5, 20),
			new DateTime(2027, 6, 19),
			new DateTime(2027, 7, 18),
			new DateTime(2027, 8, 17),
			new DateTime(2027, 9, 15),
			new DateTime(2027, 10, 15),
			new DateTime(2027, 11, 14),
			new DateTime(2027, 12, 13),
			new DateTime(2028, 1, 12),
			new DateTime(2028, 2, 10),
			new DateTime(2028, 3, 11),
			new DateTime(2028, 4, 9),
			new DateTime(2028, 5, 8),
			new DateTime(2028, 6, 7),
			new DateTime(2028, 7, 6),
			new DateTime(2028, 8, 5),
			new DateTime(2028, 9, 3),
			new DateTime(2028, 10, 3),
			new DateTime(2028, 11, 2),
			new DateTime(2028, 12, 2),
			new DateTime(2028, 12, 31),
			new DateTime(2029, 1, 30),
			new DateTime(2029, 2, 28),
			new DateTime(2029, 3, 30),
			new DateTime(2029, 4, 28),
			new DateTime(2029, 5, 27),
			new DateTime(2029, 6, 26),
			new DateTime(2029, 7, 25),
			new DateTime(2029, 8, 24),
			new DateTime(2029, 9, 22),
			new DateTime(2029, 10, 22),
			new DateTime(2029, 11, 21),
			new DateTime(2029, 12, 20),
			new DateTime(2030, 1, 19),
			new DateTime(2030, 2, 18),
			new DateTime(2030, 3, 19),
			new DateTime(2030, 4, 18),
			new DateTime(2030, 5, 17),
			new DateTime(2030, 6, 15),
			new DateTime(2030, 7, 15),
			new DateTime(2030, 8, 13),
			new DateTime(2030, 9, 11),
			new DateTime(2030, 10, 11),
			new DateTime(2030, 11, 10),
			new DateTime(2030, 12, 9),
			new DateTime(2031, 1, 8),
			new DateTime(2031, 2, 7),
			new DateTime(2031, 3, 9),
			new DateTime(2031, 4, 7),
			new DateTime(2031, 5, 7),
			new DateTime(2031, 6, 5),
			new DateTime(2031, 7, 4),
			new DateTime(2031, 8, 3),
			new DateTime(2031, 9, 1),
			new DateTime(2031, 9, 30),
			new DateTime(2031, 10, 30),
			new DateTime(2031, 11, 28),
			new DateTime(2031, 12, 28),
			new DateTime(2032, 1, 27),
			new DateTime(2032, 2, 26),
			new DateTime(2032, 3, 27),
			new DateTime(2032, 4, 25),
			new DateTime(2032, 5, 25),
			new DateTime(2032, 6, 23),
			new DateTime(2032, 7, 22),
			new DateTime(2032, 8, 21),
			new DateTime(2032, 9, 19),
			new DateTime(2032, 10, 18),
			new DateTime(2032, 11, 17),
			new DateTime(2032, 12, 16),
			new DateTime(2033, 1, 15),
			new DateTime(2033, 2, 14),
			new DateTime(2033, 3, 16),
			new DateTime(2033, 4, 14),
			new DateTime(2033, 5, 14),
			new DateTime(2033, 6, 12),
			new DateTime(2033, 7, 12),
			new DateTime(2033, 8, 10),
			new DateTime(2033, 9, 9),
			new DateTime(2033, 10, 8),
			new DateTime(2033, 11, 6),
			new DateTime(2033, 12, 6)
		};

		private GameEventSubscription m_DebugPanelSub;

		private bool m_ShouldUseFakeDate;

		private int m_FakeYear = 2013;

		private int m_FakeMonth = 7;

		private int m_FakeDay = 17;

		private int m_FakeHour = 15;

		public static void DisableWerewolfMode()
		{
			s_IsEnabled = false;
		}

		public static void EnableWerewolfMode()
		{
			s_IsEnabled = true;
		}

		private bool IsFullMoon()
		{
			if (FurbyGlobals.Player.NoFurbyForEitherReason())
			{
				return false;
			}
			DateTime timeNow = DateTime.Now;
			if (m_ShouldUseFakeDate)
			{
				timeNow = new DateTime(m_FakeYear, m_FakeMonth, m_FakeDay, m_FakeHour, 0, 0);
			}
			if (IsFullMoonDay(timeNow))
			{
				return true;
			}
			if (timeNow.Hour <= 4)
			{
				timeNow = timeNow.AddDays(-1.0);
				return IsFullMoonDay(timeNow);
			}
			return false;
		}

		private bool IsFullMoonDay(DateTime timeNow)
		{
			if (timeNow.Day == 31 && timeNow.Month == 10)
			{
				return true;
			}
			DateTime[] fullMoonTimes = m_FullMoonTimes;
			for (int i = 0; i < fullMoonTimes.Length; i++)
			{
				DateTime dateTime = fullMoonTimes[i];
				if (dateTime.Year == timeNow.Year && dateTime.Month == timeNow.Month && dateTime.Day == timeNow.Day)
				{
					return true;
				}
			}
			return false;
		}

		private IEnumerator Start()
		{
			while (true)
			{
				if (s_IsEnabled && IsFullMoon())
				{
					GameEventRouter.SendEvent(WerewolfModeEvent.TriggerWerewolfMode);
				}
				yield return new WaitForSeconds(m_WaitBetweenTriggerAttempts);
			}
		}

		private void OnEnable()
		{
			m_DebugPanelSub = new GameEventSubscription(OnDebugPanelGUI, DebugPanelEvent.DrawElementRequested);
		}

		private void OnDestroy()
		{
			m_DebugPanelSub.Dispose();
		}

		private void OnDebugPanelGUI(Enum evtType, GameObject gObj, params object[] parameters)
		{
			if (DebugPanel.StartSection("Werewolf Mode"))
			{
				GUILayout.BeginHorizontal();
				string text = string.Format("Fake Date ({0})", (!m_ShouldUseFakeDate) ? "Off" : "On");
				if (GUILayout.Button(text))
				{
					m_ShouldUseFakeDate = !m_ShouldUseFakeDate;
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Fake Year");
				GUILayout.TextField(string.Format("{0:0000}", m_FakeYear), GUILayout.ExpandWidth(false));
				float num = GUILayout.HorizontalSlider(m_FakeYear, 2013f, 2033f);
				m_FakeYear = (int)num;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Fake Month");
				GUILayout.TextField(string.Format("{0:00}", m_FakeMonth), GUILayout.ExpandWidth(false));
				float num2 = GUILayout.HorizontalSlider(m_FakeMonth, 1f, 12f);
				m_FakeMonth = (int)num2;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Fake Day");
				GUILayout.TextField(string.Format("{0:00}", m_FakeDay), GUILayout.ExpandWidth(false));
				float num3 = GUILayout.HorizontalSlider(m_FakeDay, 1f, 31f);
				m_FakeDay = (int)num3;
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				GUILayout.Label("Fake Hour");
				GUILayout.TextField(string.Format("{0:00}", m_FakeHour), GUILayout.ExpandWidth(false));
				float num4 = GUILayout.HorizontalSlider(m_FakeHour, 0f, 23f);
				m_FakeHour = (int)num4;
				GUILayout.EndHorizontal();
			}
			DebugPanel.EndSection();
		}
	}
}
