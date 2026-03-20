namespace Relentless
{
	public class AchievementStatus
	{
		private bool m_isUnlocked;

		public bool IsUnlocked()
		{
			return m_isUnlocked;
		}

		public void Unlock()
		{
			m_isUnlocked = true;
		}
	}
}
