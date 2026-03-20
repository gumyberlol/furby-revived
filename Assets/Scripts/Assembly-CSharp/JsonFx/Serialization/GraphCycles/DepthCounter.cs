using System;

namespace JsonFx.Serialization.GraphCycles
{
	public class DepthCounter : ICycleDetector
	{
		private readonly int MaxDepth;

		private int depth;

		public DepthCounter(int maxDepth)
		{
			if (maxDepth < 1)
			{
				throw new ArgumentException("MaxDepth must be a positive value", "maxDepth");
			}
			MaxDepth = maxDepth;
		}

		public bool Add(object item)
		{
			depth++;
			return depth >= MaxDepth;
		}

		public void Remove(object item)
		{
			depth--;
		}
	}
}
