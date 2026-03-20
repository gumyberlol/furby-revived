using System;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class ComAirGameEventTranslator : GameEventTranslator<ComAirReaction>
	{
		[Flags]
		private enum PersonalityFlag
		{
			RockStar = 1,
			SweetBelle = 0x10,
			Gobbler = 4,
			ToughGirl = 8,
			Kooky = 2
		}

		public ComAirReaction[] m_eventTable;

		[SerializeField]
		[DisplayAsMaskDropdown(typeof(PersonalityFlag))]
		private PersonalityFlag m_QualifyingPersonalities = PersonalityFlag.RockStar | PersonalityFlag.SweetBelle | PersonalityFlag.Gobbler | PersonalityFlag.ToughGirl | PersonalityFlag.Kooky;

		protected override ComAirReaction[] EventTable
		{
			get
			{
				return m_eventTable;
			}
		}

		private static PersonalityFlag GetPersonalityFlag(FurbyPersonality personality)
		{
			return (PersonalityFlag)(1 << (int)(personality - 917));
		}

		private bool IsQualifyingPersonality(FurbyPersonality personality)
		{
			return 0 != (m_QualifyingPersonalities & GetPersonalityFlag(personality));
		}

		protected override void OnEvent(Enum enumValue, GameObject gObj, params object[] list)
		{
			FurbyPersonality personality = Singleton<FurbyDataChannel>.Instance.FurbyStatus.Personality;
			if (IsQualifyingPersonality(personality))
			{
				base.OnEvent(enumValue, base.gameObject, list);
			}
		}
	}
}
