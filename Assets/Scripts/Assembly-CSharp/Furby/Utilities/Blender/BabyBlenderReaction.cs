using System;

namespace Furby.Utilities.Blender
{
	[Flags]
	public enum BabyBlenderReaction
	{
		Generic = 0,
		Vomit = 1,
		Burp = 2,
		Fart = 4,
		Sour = 8,
		Spicy = 0x10
	}
}
