using System.Collections.Generic;
using System.Reflection;

namespace JsonFx.Serialization.Resolvers
{
	public sealed class CallbackResolverStrategy : IResolverStrategy
	{
		public delegate bool PropertyIgnoredDelegate(PropertyInfo propertyInfo, bool isAnonymous);

		public delegate bool FieldIgnoredDelegate(FieldInfo fieldInfo);

		public delegate ValueIgnoredDelegate GetValueIgnoredDelegate(MemberInfo memberInfo);

		public delegate IEnumerable<DataName> GetNameDelegate(MemberInfo memberInfo);

		public delegate IEnumerable<MemberMap> SortMembersDelegate(IEnumerable<MemberMap> members);

		public PropertyIgnoredDelegate IsPropertyIgnored { get; set; }

		public FieldIgnoredDelegate IsFieldIgnored { get; set; }

		public GetValueIgnoredDelegate GetValueIgnored { get; set; }

		public GetNameDelegate GetName { get; set; }

		public SortMembersDelegate SortMembers { get; set; }

		bool IResolverStrategy.IsPropertyIgnored(PropertyInfo member, bool isImmutableType)
		{
			if (IsPropertyIgnored == null)
			{
				return false;
			}
			return IsPropertyIgnored(member, isImmutableType);
		}

		bool IResolverStrategy.IsFieldIgnored(FieldInfo member)
		{
			if (IsFieldIgnored == null)
			{
				return false;
			}
			return IsFieldIgnored(member);
		}

		ValueIgnoredDelegate IResolverStrategy.GetValueIgnoredCallback(MemberInfo member)
		{
			if (GetValueIgnored == null)
			{
				return null;
			}
			return GetValueIgnored(member);
		}

		IEnumerable<DataName> IResolverStrategy.GetName(MemberInfo member)
		{
			if (GetName == null)
			{
				return null;
			}
			return GetName(member);
		}

		IEnumerable<MemberMap> IResolverStrategy.SortMembers(IEnumerable<MemberMap> members)
		{
			if (SortMembers == null)
			{
				return members;
			}
			return SortMembers(members);
		}
	}
}
