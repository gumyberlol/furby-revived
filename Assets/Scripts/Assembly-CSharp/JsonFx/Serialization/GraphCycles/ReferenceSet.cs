using System;

namespace JsonFx.Serialization.GraphCycles
{
	public class ReferenceSet : ICycleDetector
	{
		private const int InitialSize = 4;

		private const int GrowthFactor = 2;

		private static readonly object[] EmptyArray = new object[0];

		private object[] array = EmptyArray;

		private int size;

		public bool Add(object item)
		{
			if (item == null || item is ValueType || item is string)
			{
				return false;
			}
			for (int num = size - 1; num >= 0; num--)
			{
				if (object.ReferenceEquals(array[num], item))
				{
					return true;
				}
			}
			if (size >= array.Length)
			{
				object[] destinationArray = new object[(array.Length != 0) ? (2 * array.Length) : 4];
				if (size > 0)
				{
					Array.Copy(array, 0, destinationArray, 0, size);
				}
				array = destinationArray;
			}
			array[size++] = item;
			return false;
		}

		public void Remove(object item)
		{
			if (item == null || item is ValueType || item is string)
			{
				return;
			}
			for (int num = size - 1; num >= 0; num--)
			{
				if (object.ReferenceEquals(array[num], item))
				{
					if (num + 1 == size)
					{
						array[num] = null;
						size--;
					}
					else
					{
						Array.Copy(array, num + 1, array, num, size - num);
					}
					break;
				}
			}
		}
	}
}
