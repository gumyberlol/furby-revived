using UnityEngine;

namespace Eye3.Ware
{
	public static class BoundsExtensions
	{
		public static bool Contains(this Bounds bounds, Bounds other)
		{
			if (bounds.Contains(other.min) && bounds.Contains(other.max))
			{
				return true;
			}
			return false;
		}
	}
}
