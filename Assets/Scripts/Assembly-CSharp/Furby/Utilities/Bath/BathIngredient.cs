using System;
using UnityEngine;

namespace Furby.Utilities.Bath
{
	[Serializable]
	public class BathIngredient
	{
		public string Name;

		public UIAtlas GraphicAtlas;

		public string Graphic;

		public bool Unlocked;

		public int Cost;

		public int Count;

		public Score Score;

		public string Sound;

		public Color Colour;
	}
}
