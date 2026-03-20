using System.Collections;

namespace com.google.zxing.common
{
	public sealed class Collections
	{
		private Collections()
		{
		}

		public static void insertionSort(ArrayList vector, Comparator comparator)
		{
			int count = vector.Count;
			for (int i = 1; i < count; i++)
			{
				object obj = vector[i];
				int num = i - 1;
				object value;
				while (num >= 0 && comparator.compare(value = vector[num], obj) > 0)
				{
					vector[num + 1] = value;
					num--;
				}
				vector[num + 1] = obj;
			}
		}
	}
}
