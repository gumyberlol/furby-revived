using System;
using System.Collections.Generic;
using System.Linq;
using Relentless;
using UnityEngine;

namespace Furby
{
	public class PersonalityLibrary : MonoBehaviour
	{
		[Serializable]
		public class PersonalityData
		{
			public FurbyBabyPersonality personality;

			public string[] flairs = new string[0];
		}

		[EasyEditArray]
		public PersonalityData[] Personalities;

		public string[] RandomAdditionalFlairs;

		public PersonalityData GetPersonalityData(FurbyBabyPersonality personality)
		{
			List<PersonalityData> list = Personalities.Where((PersonalityData x) => x.personality == personality).DefaultIfEmpty(new PersonalityData()).ToList();
			if (list.Count() > 0)
			{
				return list[UnityEngine.Random.Range(0, list.Count())];
			}
			return null;
		}

		public string GetRandomAdditionalFlair()
		{
			return RandomAdditionalFlairs[UnityEngine.Random.Range(0, RandomAdditionalFlairs.Length)];
		}
	}
}
