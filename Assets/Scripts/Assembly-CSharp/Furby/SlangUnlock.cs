using System;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class SlangUnlock : MonoBehaviour
	{
		public delegate void PhraseUnlockHandler(string phrase);

		private static string s_lastUnlocked = string.Empty;

		private GameEventSubscription m_sub;

		public static string LastUnlocked
		{
			get
			{
				return s_lastUnlocked;
			}
		}

		public event PhraseUnlockHandler PhraseUnlocked;

		public void Start()
		{
			m_sub = new GameEventSubscription(typeof(DashboardGameEvent), HandleLevelUpEvent);
		}

		private void HandleLevelUpEvent(Enum eventType, GameObject gameObject, params object[] parameters)
		{
			if (eventType.Equals(DashboardGameEvent.XP_Level_Increased))
			{
				LevelUp();
			}
		}

		private void LevelUp()
		{
			GameData gameData = Singleton<GameDataStoreObject>.Instance.Data;
			while (gameData.m_slangLastLevelSeen < gameData.Level)
			{
				List<string> list = default(SlangActionsEnumerable).Where((string s) => !gameData.m_unlockedSlangs.Contains(s)).ToList();
				int index = UnityEngine.Random.Range(0, list.Count() - 1);
				string text = list[index];
				gameData.m_unlockedSlangs.Add(text);
				gameData.m_unconfirmedSlangs.Add(text);
				gameData.m_slangLastLevelSeen++;
				FurbyAction msgCode = (FurbyAction)(int)Enum.Parse(typeof(FurbyAction), text);
				Singleton<FurbyDataChannel>.Instance.PostAction(msgCode, null);
				if (this.PhraseUnlocked != null)
				{
					this.PhraseUnlocked(text);
				}
			}
		}
	}
}
