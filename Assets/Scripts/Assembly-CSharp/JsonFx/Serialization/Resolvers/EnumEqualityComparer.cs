using System;
using System.Collections.Generic;

namespace JsonFx.Serialization.Resolvers
{
	public class EnumEqualityComparer : IEqualityComparer<Enum>
	{
		public bool Equals(Enum x, Enum y)
		{
			return x == y;
		}

		public int GetHashCode(Enum obj)
		{
			return obj.ToString().GetHashCode();
		}
	}
}
