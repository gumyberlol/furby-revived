using System;
using System.Collections.Generic;
using System.Reflection;

namespace JsonFx.Serialization.Resolvers
{
	public sealed class CombinedResolverStrategy : IResolverStrategy
	{
		private readonly IEnumerable<IResolverStrategy> InnerStrategies;

		public CombinedResolverStrategy(params IResolverStrategy[] strategies)
			: this((IEnumerable<IResolverStrategy>)strategies)
		{
		}

		public CombinedResolverStrategy(IEnumerable<IResolverStrategy> strategies)
		{
			if (strategies == null)
			{
				throw new ArgumentNullException("strategies");
			}
			InnerStrategies = strategies;
		}

		public bool IsPropertyIgnored(PropertyInfo member, bool isImmutableType)
		{
			foreach (IResolverStrategy innerStrategy in InnerStrategies)
			{
				if (innerStrategy.IsPropertyIgnored(member, isImmutableType))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsFieldIgnored(FieldInfo member)
		{
			foreach (IResolverStrategy innerStrategy in InnerStrategies)
			{
				if (innerStrategy.IsFieldIgnored(member))
				{
					return true;
				}
			}
			return false;
		}

		public ValueIgnoredDelegate GetValueIgnoredCallback(MemberInfo member)
		{
			List<ValueIgnoredDelegate> ignoredDelegates = new List<ValueIgnoredDelegate>();
			foreach (IResolverStrategy innerStrategy in InnerStrategies)
			{
				ValueIgnoredDelegate valueIgnoredCallback = innerStrategy.GetValueIgnoredCallback(member);
				if (valueIgnoredCallback != null)
				{
					ignoredDelegates.Add(valueIgnoredCallback);
				}
			}
			if (ignoredDelegates.Count < 1)
			{
				return null;
			}
			if (ignoredDelegates.Count == 1)
			{
				return ignoredDelegates[0];
			}
			return delegate(object instance, object memberValue)
			{
				foreach (ValueIgnoredDelegate item in ignoredDelegates)
				{
					if (item(instance, memberValue))
					{
						return true;
					}
				}
				return false;
			};
		}

		public IEnumerable<DataName> GetName(MemberInfo member)
		{
			foreach (IResolverStrategy strategy in InnerStrategies)
			{
				foreach (DataName name in strategy.GetName(member))
				{
					if (!name.IsEmpty)
					{
						yield return name;
					}
				}
			}
		}

		public IEnumerable<MemberMap> SortMembers(IEnumerable<MemberMap> members)
		{
			foreach (IResolverStrategy innerStrategy in InnerStrategies)
			{
				members = innerStrategy.SortMembers(members) ?? members;
			}
			return members;
		}
	}
}
