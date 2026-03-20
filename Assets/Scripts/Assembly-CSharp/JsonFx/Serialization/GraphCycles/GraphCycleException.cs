using System;
using System.Runtime.Serialization;

namespace JsonFx.Serialization.GraphCycles
{
	public class GraphCycleException : SerializationException
	{
		private readonly GraphCycleType GraphCycleType;

		public GraphCycleType CycleType
		{
			get
			{
				return GraphCycleType;
			}
		}

		public GraphCycleException(GraphCycleType cycleType)
		{
			GraphCycleType = cycleType;
		}

		public GraphCycleException(GraphCycleType cycleType, string message)
			: base(message)
		{
			GraphCycleType = cycleType;
		}

		public GraphCycleException(GraphCycleType cycleType, string message, Exception innerException)
			: base(message, innerException)
		{
			GraphCycleType = cycleType;
		}

		public GraphCycleException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
