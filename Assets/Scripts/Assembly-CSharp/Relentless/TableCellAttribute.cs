using System;
using UnityEngine;

namespace Relentless
{
	[AttributeUsage(AttributeTargets.Field)]
	public class TableCellAttribute : Attribute
	{
		public GUILayoutOption Width;

		public GUILayoutOption Height;

		public string Text;

		public TableCellAttribute(string text, int width, int height)
		{
			Width = GUILayout.Width(width);
			Height = GUILayout.Height(height);
			Text = text;
		}
	}
}
