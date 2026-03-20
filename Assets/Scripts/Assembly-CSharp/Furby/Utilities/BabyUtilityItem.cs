using System;

namespace Furby.Utilities
{
	[Serializable]
	public abstract class BabyUtilityItem
	{
		public string Name;

		public UIAtlas GraphicAtlas;

		public string Graphic;

		public bool Unlocked;

		public int Cost;

		public int Count;

		public Score Score = new Score();

		public override int GetHashCode()
		{
			return Name.GetHashCode() ^ Cost ^ Graphic.GetHashCode();
		}
	}
}
