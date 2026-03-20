using Relentless;

namespace Furby.Playroom
{
	public class PlayroomIdlingController : Singleton<PlayroomIdlingController>
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

		private PlayroomIdlingController()
		{
			m_Enable = true;
		}
	}
}
