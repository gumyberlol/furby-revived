using System;
using System.Collections.Generic;
using UnityEngine;

namespace Furby.Playroom
{
	[Serializable]
	public class PersonalitySpecificIdleAnimations
	{
		[SerializeField]
		public FurbyBabyPersonality m_Personality;

		public List<FurbyIdleAnimation> m_FurbyIdleAnimations;
	}
}
