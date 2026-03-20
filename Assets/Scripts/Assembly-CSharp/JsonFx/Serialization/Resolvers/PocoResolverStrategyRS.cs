using System.Collections.Generic;
using System.Reflection;

namespace JsonFx.Serialization.Resolvers
{
	public class PocoResolverStrategyRS : IResolverStrategyRS
	{
		public virtual bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType)
		{
			MethodInfo methodInfo = ((!member.CanRead) ? null : member.GetGetMethod());
			MethodInfo methodInfo2 = ((!member.CanWrite) ? null : member.GetSetMethod());
			return methodInfo == null || !methodInfo.IsPublic || (!isImmutableType && (methodInfo2 == null || !methodInfo2.IsPublic));
		}

		public virtual bool IsFieldIgnored(FieldInfo member)
		{
			return !member.IsPublic || member.IsInitOnly;
		}

		public virtual ValueIgnoredDelegate GetValueIgnoredCallback(MemberInfo member)
		{
			return null;
		}

		public virtual IEnumerable<DataName> GetName(MemberInfo member)
		{
			return null;
		}

		public virtual IEnumerable<MemberMapRS> SortMembers(IEnumerable<MemberMapRS> members)
		{
			return members;
		}
	}
}
