using Furby.Neighbourhood;
using Relentless;

namespace Furby.Playroom
{
	public class PlayroomCue_FromHood : RelentlessMonoBehaviour
	{
		public FurbyBaby m_FurbyBaby;

		private bool m_IsActive = true;

		public FurbyBaby FurbyBaby
		{
			get
			{
				return m_FurbyBaby;
			}
			set
			{
				m_FurbyBaby = value;
			}
		}

		public bool IsActive
		{
			set
			{
				m_IsActive = value;
			}
		}

		private void OnClick()
		{
			if (m_IsActive)
			{
				base.gameObject.SendGameEvent(HoodEvents.Hood_BabySelected, m_FurbyBaby);
			}
		}
	}
}
