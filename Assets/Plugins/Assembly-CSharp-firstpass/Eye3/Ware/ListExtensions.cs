using System.Collections.Generic;

namespace Eye3.Ware
{
	public static class ListExtensions
	{
		public static bool AddOnce<T>(this List<T> aList, T toAdd)
		{
			if (aList.Contains(toAdd))
			{
				return false;
			}
			aList.Add(toAdd);
			return true;
		}

		public static bool RemoveEvery<T>(this List<T> aList, T toRemove)
		{
			if (aList.RemoveAll((T anItem) => anItem.Equals(toRemove)) > 0)
			{
				return true;
			}
			return false;
		}

		public static void Clean<T>(this List<T> aList)
		{
			aList.RemoveAll((T item) => item == null);
		}
	}
}
