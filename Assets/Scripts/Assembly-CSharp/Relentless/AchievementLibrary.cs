using System.Collections.Generic;
using UnityEngine;

namespace Relentless
{
	public class AchievementLibrary : Singleton<AchievementLibrary>, SaveObject
	{
		private Achievement[] m_achievementList;

		private Dictionary<Achievement, AchievementStatus> m_achievementStatus = new Dictionary<Achievement, AchievementStatus>();

		private List<Achievement> m_unshownAwardedAchievments = new List<Achievement>();

		private void Awake()
		{
			m_achievementList = GetComponentsInChildren<Achievement>();
			Achievement[] achievementList = m_achievementList;
			foreach (Achievement key in achievementList)
			{
				m_achievementStatus[key] = new AchievementStatus();
			}
			Singleton<SaveGame>.Instance.Register(new SaveGameItem("LocalAchievements", this));
		}

		public Achievement GetNextUnshownAchievement()
		{
			if (m_unshownAwardedAchievments.Count > 0)
			{
				return m_unshownAwardedAchievments[0];
			}
			return null;
		}

		public void SetShown(Achievement achievement)
		{
			m_unshownAwardedAchievments.Remove(achievement);
		}

		public Achievement[] GetAchievementList()
		{
			return m_achievementList;
		}

		public void OnGameEvent(string gameEvent, bool allowUnlocksAtThisPoint)
		{
			Achievement[] achievementList = m_achievementList;
			foreach (Achievement achievement in achievementList)
			{
				if (!m_achievementStatus[achievement].IsUnlocked())
				{
					achievement.BroadcastMessage(gameEvent, SendMessageOptions.DontRequireReceiver);
					if (allowUnlocksAtThisPoint && achievement.IsUnlocked())
					{
						m_unshownAwardedAchievments.Add(achievement);
						m_achievementStatus[achievement].Unlock();
					}
				}
			}
		}

		public void OnGameEvent(string gameEvent, Object linkedObject, bool allowUnlocksAtThisPoint)
		{
			Achievement[] achievementList = m_achievementList;
			foreach (Achievement achievement in achievementList)
			{
				if (!m_achievementStatus[achievement].IsUnlocked())
				{
					achievement.BroadcastMessage(gameEvent, linkedObject, SendMessageOptions.DontRequireReceiver);
					if (allowUnlocksAtThisPoint && achievement.IsUnlocked())
					{
						m_unshownAwardedAchievments.Add(achievement);
						m_achievementStatus[achievement].Unlock();
					}
				}
			}
		}

		public void SerializeTo(SaveGameWriter writer)
		{
			writer.WriteInt(m_achievementStatus.Count);
			foreach (KeyValuePair<Achievement, AchievementStatus> item in m_achievementStatus)
			{
				writer.WriteString(item.Key.name);
				writer.WriteBool(item.Value.IsUnlocked());
			}
		}

		public void DeserializeFrom(SaveGameReader reader)
		{
			int num = reader.ReadInt();
			for (int i = 0; i < num; i++)
			{
				string text = reader.ReadString();
				Achievement achievement = null;
				Achievement[] achievementList = m_achievementList;
				foreach (Achievement achievement2 in achievementList)
				{
					if (achievement2.name == text)
					{
						achievement = achievement2;
						break;
					}
				}
				bool flag = reader.ReadBool();
				if (achievement != null)
				{
					m_achievementStatus[achievement] = new AchievementStatus();
					if (flag)
					{
						m_achievementStatus[achievement].Unlock();
					}
				}
			}
		}

		public int GetVersion()
		{
			return 1;
		}

		public AchievementStatus GetAchievementState(Achievement ach)
		{
			return m_achievementStatus[ach];
		}
	}
}
