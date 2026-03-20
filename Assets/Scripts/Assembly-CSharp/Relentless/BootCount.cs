namespace Relentless
{
	public class BootCount : RelentlessMonoBehaviour, SaveObject
	{
		private int m_BootCount;

		public int BootCounter
		{
			get
			{
				return m_BootCount;
			}
		}

		private void Awake()
		{
			Singleton<SaveGame>.Instance.Register(new SaveGameItem("BootCount", this));
		}

		public void SerializeTo(SaveGameWriter writer)
		{
			writer.WriteInt(m_BootCount);
		}

		public void DeserializeFrom(SaveGameReader reader)
		{
			m_BootCount = reader.ReadInt();
			m_BootCount++;
		}

		public int GetVersion()
		{
			return 1;
		}
	}
}
