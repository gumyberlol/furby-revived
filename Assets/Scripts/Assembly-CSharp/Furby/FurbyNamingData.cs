using System;
using UnityEngine;

namespace Furby
{
	public class FurbyNamingData : ScriptableObject
	{
		[Serializable]
		public class DisallowedName
		{
			[SerializeField]
			public string m_disallowedLeft;

			[SerializeField]
			public string m_disallowedRight;
		}

		public string[] m_leftNames;

		public string[] m_rightNames;

		public DisallowedName[] m_disallowedNames;
	}
}
