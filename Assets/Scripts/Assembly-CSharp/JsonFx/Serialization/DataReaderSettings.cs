using System.Collections.Generic;
using JsonFx.Model;
using JsonFx.Model.Filters;
using JsonFx.Serialization.Filters;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Serialization
{
	public sealed class DataReaderSettings : IResolverCacheContainer
	{
		private bool allowNullValueTypes = true;

		private bool allowTrailingContent = true;

		private readonly ResolverCache ResolverCache;

		private readonly IEnumerable<IDataFilter<ModelTokenType>> ModelFilters;

		ResolverCache IResolverCacheContainer.ResolverCache
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

		public ResolverCache Resolver
		{
			get
			{
				return ResolverCache;
			}
		}

		public IEnumerable<IDataFilter<ModelTokenType>> Filters
		{
			get
			{
				return ModelFilters;
			}
		}

		public DataReaderSettings()
			: this(new PocoResolverStrategy(), new Iso8601DateFilter())
		{
		}

		public DataReaderSettings(params IDataFilter<ModelTokenType>[] filters)
			: this(new PocoResolverStrategy(), filters)
		{
		}

		public DataReaderSettings(IEnumerable<IDataFilter<ModelTokenType>> filters)
			: this(new PocoResolverStrategy(), filters)
		{
		}

		public DataReaderSettings(IResolverStrategy strategy, params IDataFilter<ModelTokenType>[] filters)
			: this(new ResolverCache(strategy), filters)
		{
		}

		public DataReaderSettings(IResolverStrategy strategy, IEnumerable<IDataFilter<ModelTokenType>> filters)
			: this(new ResolverCache(strategy), filters)
		{
		}

		public DataReaderSettings(ResolverCache resolverCache, params IDataFilter<ModelTokenType>[] filters)
			: this(resolverCache, (IEnumerable<IDataFilter<ModelTokenType>>)filters)
		{
		}

		public DataReaderSettings(ResolverCache resolverCache, IEnumerable<IDataFilter<ModelTokenType>> filters)
		{
			ResolverCache = resolverCache;
			ModelFilters = filters;
		}
	}
}
