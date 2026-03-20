using System.Collections.Generic;
using System.Reflection;

namespace JsonFx.Serialization.Resolvers
{
	public interface IResolverStrategy
	{
		bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType);

		bool IsFieldIgnored(FieldInfo member);

		ValueIgnoredDelegate GetValueIgnoredCallback(MemberInfo member);

		IEnumerable<DataName> GetName(MemberInfo member);

		IEnumerable<MemberMap> SortMembers(IEnumerable<MemberMap> members);
	}
}
