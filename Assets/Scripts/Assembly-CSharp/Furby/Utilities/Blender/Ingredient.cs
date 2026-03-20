using System;
using UnityEngine;

namespace Furby.Utilities.Blender
{
	[Serializable]
	public class Ingredient : BabyUtilityItem
	{
		public string Sound;

		public BabyBlenderReaction Reaction;

		public Color Colour = Color.black;
	}
}
