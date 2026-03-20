namespace Relentless
{
	public class LookAtTouch_Enabler : Singleton<LookAtTouch_Enabler>
	{
		public bool m_Enable;

		public bool Enable
		{
			get
			{
				return m_Enable;
			}
			set
			{
				m_Enable = value;
			}
		}

		private LookAtTouch_Enabler()
		{
			m_Enable = true;
		}
	}
}
