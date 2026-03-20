using System.Collections.Generic;
using System.Reflection;

namespace JsonFx.Serialization.Resolvers
{
	public interface IResolverStrategyRS
	{
		bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType);

		bool IsFieldIgnored(FieldInfo member);

		ValueIgnoredDelegate GetValueIgnoredCallback(MemberInfo member);

		IEnumerable<DataName> GetName(MemberInfo member);

		IEnumerable<MemberMapRS> SortMembers(IEnumerable<MemberMapRS> members);
	}
}
