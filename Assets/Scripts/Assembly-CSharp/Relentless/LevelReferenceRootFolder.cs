using System;

namespace Relentless
{
	[AttributeUsage(AttributeTargets.Field)]
	public class LevelReferenceRootFolder : Attribute
	{
		public readonly string m_root;

		public LevelReferenceRootFolder(string root)
		{
			m_root = root;
		}
	}
}
