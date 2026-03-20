using System;
using System.Collections.Generic;
using Relentless;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class PlayroomPersonalityIdling : Singleton<PlayroomPersonalityIdling>
	{
		[SerializeField]
		public int m_ThresholdAs5Percents;

		[SerializeField]
		public List<PersonalitySpecificIdleAnimations> m_PersonalityIdles;

		[SerializeField]
		public List<FurbyIdleAnimation> m_GenericIdleAnimations;

		public List<FurbyIdleAnimation> m_CachedPersonalityIncidents;

		private int m_PreviousAnimIndex_PersonalitySpecific = -1;

		private int m_PreviousAnimIndex_Generic = -1;

		private bool m_HaveCachedPersonalityIdles;

		public void Initialise()
		{
			if ((bool)FurbyGlobals.Player)
			{
				CachePersonalityIdles();
			}
		}

		private void CachePersonalityIdles()
		{
			if (FurbyGlobals.Player.SelectedFurbyBaby != null)
			{
				GetIdleAnimationsAndEventsForPersonality(FurbyGlobals.Player.SelectedFurbyBaby.Personality);
				m_HaveCachedPersonalityIdles = true;
			}
		}

		private void GetIdleAnimationsAndEventsForPersonality(FurbyBabyPersonality personality)
		{
			m_CachedPersonalityIncidents = new List<FurbyIdleAnimation>();
			if (personality != FurbyBabyPersonality.None)
			{
				foreach (PersonalitySpecificIdleAnimations personalityIdle in m_PersonalityIdles)
				{
					if ((personality & personalityIdle.m_Personality) == personalityIdle.m_Personality)
					{
						m_CachedPersonalityIncidents.AddRange(personalityIdle.m_FurbyIdleAnimations);
					}
				}
			}
			Logging.Log("Got " + m_CachedPersonalityIncidents.Count + " idle animations for personality: " + personality.ToString());
		}

		public AnimationClip GetSuitableIdleAnimation_AlgorithmTypeI()
		{
			float num = UnityEngine.Random.Range(0, 100);
			float num2 = m_ThresholdAs5Percents * 5;
			if (num > num2)
			{
				return SelectPersonalitySpecificIdle().m_AnimationClip;
			}
			return SelectGenericIdle().m_AnimationClip;
		}

		public FurbyIdleAnimation GetFurbyIdleAnimation()
		{
			float num = UnityEngine.Random.Range(0, 100);
			float num2 = m_ThresholdAs5Percents * 5;
			if (num > num2)
			{
				return SelectPersonalitySpecificIdle();
			}
			return SelectGenericIdle();
		}

		public FurbyIdleAnimation GetGenericFurbyIdleAnimation()
		{
			return SelectGenericIdle();
		}

		private FurbyIdleAnimation SelectPersonalitySpecificIdle()
		{
			if (!m_HaveCachedPersonalityIdles)
			{
				CachePersonalityIdles();
			}
			if (m_CachedPersonalityIncidents.Count == 0)
			{
				return SelectGenericIdle();
			}
			if (m_CachedPersonalityIncidents.Count > 1)
			{
				int num;
				for (num = UnityEngine.Random.Range(0, m_CachedPersonalityIncidents.Count); num == m_PreviousAnimIndex_PersonalitySpecific; num = UnityEngine.Random.Range(0, m_CachedPersonalityIncidents.Count))
				{
				}
				m_PreviousAnimIndex_PersonalitySpecific = num;
				return m_CachedPersonalityIncidents[num];
			}
			return m_CachedPersonalityIncidents[0];
		}

		private FurbyIdleAnimation SelectGenericIdle()
		{
			if (m_GenericIdleAnimations.Count > 1)
			{
				int num;
				for (num = UnityEngine.Random.Range(0, m_GenericIdleAnimations.Count); num == m_PreviousAnimIndex_Generic; num = UnityEngine.Random.Range(0, m_GenericIdleAnimations.Count))
				{
				}
				m_PreviousAnimIndex_Generic = num;
				return m_GenericIdleAnimations[num];
			}
			return m_GenericIdleAnimations[0];
		}

		public bool IsAnimationAGenericIdle(string animName)
		{
			foreach (FurbyIdleAnimation genericIdleAnimation in m_GenericIdleAnimations)
			{
				if (animName.Equals(genericIdleAnimation.m_AnimationClip.name))
				{
					return true;
				}
			}
			return false;
		}
	}
}
