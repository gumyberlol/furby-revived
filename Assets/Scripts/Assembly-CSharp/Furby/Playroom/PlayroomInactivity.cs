using System;

namespace Furby.Playroom
{
	[Serializable]
	public class PlayroomInactivity
	{
		public float m_InactivityPeriod = 30f;

		public float m_DurationToShowCrying = 5f;

		public float m_DurationToWaitForSympathyToRegister = 5f;

		public float m_DurationToShowThatBabyHappy = 5f;

		public float m_DurationToWaitAfterReceivingAResponse = 2f;

		public float m_DurationToShowCryingAfterNoResponse = 5f;
	}
}
