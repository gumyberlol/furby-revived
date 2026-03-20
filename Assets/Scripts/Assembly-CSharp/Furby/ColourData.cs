using UnityEngine;

namespace Furby
{
	public class ColourData : ScriptableObject
	{
		public Color Colour = Color.white;

		public static implicit operator Color(ColourData col)
		{
			return col.Colour;
		}
	}
}
