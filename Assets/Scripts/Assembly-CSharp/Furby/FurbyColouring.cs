using System;
using UnityEngine;

namespace Furby
{
	[Serializable]
	public class FurbyColouring
	{
		public Texture FurTexture;

		public Material FurMaterialHigh;

		public ColourData FeetColor;

		public ColourData EyeColor;

		public ColourData EarColor;

		public ColourData EyelidColor;

		public string FurbySpriteName;
	}
}
