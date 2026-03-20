using System;
using System.Runtime.Serialization;

namespace Eye3.TestStar
{
	[Serializable]
	public class MissingPrefab : TestStarException
	{
		public MissingPrefab()
		{
		}

		public MissingPrefab(string message)
			: base(message)
		{
		}

		public MissingPrefab(string message, Exception e)
			: base(message, e)
		{
		}

		protected MissingPrefab(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
