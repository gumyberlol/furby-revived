using System;

namespace Relentless
{
	[AttributeUsage(AttributeTargets.Field)]
	public class InspectorLabel : Attribute
	{
		public readonly string Label;

		public InspectorLabel(string label)
		{
			Label = label;
		}
	}
}
