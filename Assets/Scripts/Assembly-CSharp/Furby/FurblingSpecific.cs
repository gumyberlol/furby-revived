using System;

namespace Furby
{
	[Serializable]
	public class FurblingSpecific
	{
		public FurbyBabyTypeInfo m_Type;

		public FurbyBabyPersonality m_Personality;

		public string[] m_Flair = new string[0];
	}
}
