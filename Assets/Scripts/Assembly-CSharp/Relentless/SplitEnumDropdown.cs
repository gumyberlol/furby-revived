using UnityEngine;

namespace Relentless
{
	public class SplitEnumDropdown : PropertyAttribute
	{
		private string m_splitSequence = string.Empty;

		public string SplitSequence
		{
			get
			{
				return m_splitSequence;
			}
		}

		public SplitEnumDropdown()
		{
		}

		public SplitEnumDropdown(string splitSequence)
		{
			m_splitSequence = splitSequence;
		}
	}
}
