using System;

namespace Relentless
{
	[AttributeUsage(AttributeTargets.Field)]
	public class LinearMappingEditorSnap : Attribute
	{
		public readonly float m_snap;

		public LinearMappingEditorSnap(float snap)
		{
			m_snap = snap;
		}
	}
}
