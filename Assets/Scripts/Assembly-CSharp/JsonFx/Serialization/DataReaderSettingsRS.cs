using System.Collections.Generic;
using JsonFx.Model;
using JsonFx.Model.Filters;
using JsonFx.Serialization.Filters;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Serialization
{
	public sealed class DataReaderSettingsRS : IResolverCacheContainerRS
	{
		private bool allowNullValueTypes = true;

		private bool allowTrailingContent = true;

		private readonly ResolverCacheRS ResolverCache;

		private readonly IEnumerable<IDataFilterRS<ModelTokenType>> ModelFilters;

		ResolverCacheRS IResolverCacheContainerRS.ResolverCache
		{
			get
			{
				return ResolverCache;
			}
		}

		public bool AllowNullValueTypes
		{
			get
			{
				return allowNullValueTypes;
			}
			set
			{
				allowNullValueTypes = value;
			}
		}

		public bool AllowTrailingContent
		{
			get
			{
				return allowTrailingContent;
			}
			set
			{
				allowTrailingContent = value;
			}
		}

		public ResolverCacheRS Resolver
		{
			get
			{
				return ResolverCache;
			}
		}

		public IEnumerable<IDataFilterRS<ModelTokenType>> Filters
		{
			get
			{
				return ModelFilters;
			}
		}

		public DataReaderSettingsRS()
			: this(new PocoResolverStrategyRS(), new Iso8601DateFilterRS())
		{
		}

		public DataReaderSettingsRS(params IDataFilterRS<ModelTokenType>[] filters)
			: this(new PocoResolverStrategyRS(), filters)
		{
		}

		public DataReaderSettingsRS(IEnumerable<IDataFilterRS<ModelTokenType>> filters)
			: this(new PocoResolverStrategyRS(), filters)
		{
		}

		public DataReaderSettingsRS(IResolverStrategyRS strategy, params IDataFilterRS<ModelTokenType>[] filters)
			: this(new ResolverCacheRS(strategy), filters)
		{
		}

		public DataReaderSettingsRS(IResolverStrategyRS strategy, IEnumerable<IDataFilterRS<ModelTokenType>> filters)
			: this(new ResolverCacheRS(strategy), filters)
		{
		}

		public DataReaderSettingsRS(ResolverCacheRS resolverCache, params IDataFilter<ModelTokenType>[] filters)
			: this(resolverCache, (IEnumerable<IDataFilterRS<ModelTokenType>>)filters)
		{
		}

		public DataReaderSettingsRS(ResolverCacheRS resolverCache, IEnumerable<IDataFilterRS<ModelTokenType>> filters)
		{
			ResolverCache = resolverCache;
			ModelFilters = filters;
		}
	}
}
