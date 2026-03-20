using UnityEngine;

namespace Relentless
{
	public class SystemTimer : SingletonInstance<SystemTimer>, SaveObject
	{
		private float m_timeSecsEverPlayed;

		private float m_prevTime;

		private void Start()
		{
			m_prevTime = Time.realtimeSinceStartup;
			Singleton<SaveGame>.Instance.Register(new SaveGameItem("SystemTimer", this));
		}

		private void Update()
		{
			m_timeSecsEverPlayed += Time.realtimeSinceStartup - m_prevTime;
			m_prevTime = Time.realtimeSinceStartup;
		}

		public float GetTimePlayedEver()
		{
			return m_timeSecsEverPlayed;
		}

		public void SerializeTo(SaveGameWriter writer)
		{
			writer.WriteFloat(m_timeSecsEverPlayed);
		}

		public void DeserializeFrom(SaveGameReader reader)
		{
			m_timeSecsEverPlayed = reader.ReadFloat();
		}

		public int GetVersion()
		{
			return 1;
		}
	}
}
