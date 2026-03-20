using System;
using System.Runtime.Serialization;

namespace Eye3.TestStar
{
	[Serializable]
	public class CurrentSceneResultNotInitialized : TestStarException
	{
		public CurrentSceneResultNotInitialized()
		{
		}

		public CurrentSceneResultNotInitialized(string message)
			: base(message)
		{
		}

		public CurrentSceneResultNotInitialized(string message, Exception e)
			: base(message, e)
		{
		}

		protected CurrentSceneResultNotInitialized(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
