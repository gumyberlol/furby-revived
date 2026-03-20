using UnityEngine;

namespace Furby
{
	public class FurbyBeakSyncData : ScriptableObject
	{
		[SerializeField]
		public float m_topBeakMaxRotation = -20f;

		[SerializeField]
		public float m_topBeakFactorRotation = -40f;

		[SerializeField]
		public float m_lowerBeakMaxRotation = -40f;

		[SerializeField]
		public float m_lowerBeakFactorRotation = -120f;

		[SerializeField]
		public string m_volumeMeterName = "BabyVocals";

		[SerializeField]
		public string[] m_animationsThatPreventBeakSyncNames;

		public bool IsAnimationThatPreventsBeakSyncPlaying(ref Animation watchedAnimation)
		{
			string[] animationsThatPreventBeakSyncNames = m_animationsThatPreventBeakSyncNames;
			foreach (string text in animationsThatPreventBeakSyncNames)
			{
				if (watchedAnimation.IsPlaying(text))
				{
					return true;
				}
			}
			return false;
		}
	}
}
