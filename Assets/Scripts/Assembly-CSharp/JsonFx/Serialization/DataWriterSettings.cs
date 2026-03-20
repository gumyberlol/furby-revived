using System;
using System.Collections.Generic;
using JsonFx.Model;
using JsonFx.Model.Filters;
using JsonFx.Serialization.Filters;
using JsonFx.Serialization.GraphCycles;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Serialization
{
	public sealed class DataWriterSettings : IResolverCacheContainer
	{
		private bool prettyPrint;

		private GraphCycleType graphCycles;

		private string tab = "\t";

		private int maxDepth;

		private string newLine = Environment.NewLine;

		private readonly ResolverCache ResolverCache;

		private readonly IEnumerable<IDataFilter<ModelTokenType>> ModelFilters;

		ResolverCache IResolverCacheContainer.ResolverCache
		{
			get
			{
				return ResolverCache;
			}
		}

		public GraphCycleType GraphCycles
		{
			get
			{
				return graphCycles;
			}
			set
			{
				graphCycles = value;
			}
		}

		public int MaxDepth
		{
			get
			{
				return maxDepth;
			}
			set
			{
				maxDepth = value;
			}
		}

		public bool PrettyPrint
		{
			get
			{
				return prettyPrint;
			}
			set
			{
				prettyPrint = value;
			}
		}

		public string Tab
		{
			get
			{
				return tab;
			}
			set
			{
				tab = value;
			}
		}

		public string NewLine
		{
			get
			{
				return newLine;
			}
			set
			{
				newLine = value;
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

		public DataWriterSettings()
			: this(new PocoResolverStrategy(), new Iso8601DateFilter())
		{
		}

		public DataWriterSettings(params IDataFilter<ModelTokenType>[] filters)
			: this(new PocoResolverStrategy(), filters)
		{
		}

		public DataWriterSettings(IEnumerable<IDataFilter<ModelTokenType>> filters)
			: this(new PocoResolverStrategy(), filters)
		{
		}

		public DataWriterSettings(IResolverStrategy strategy, params IDataFilter<ModelTokenType>[] filters)
			: this(new ResolverCache(strategy), filters)
		{
		}

		public DataWriterSettings(IResolverStrategy strategy, IEnumerable<IDataFilter<ModelTokenType>> filters)
			: this(new ResolverCache(strategy), filters)
		{
		}

		public DataWriterSettings(ResolverCache resolverCache, params IDataFilter<ModelTokenType>[] filters)
			: this(resolverCache, (IEnumerable<IDataFilter<ModelTokenType>>)filters)
		{
		}

		public DataWriterSettings(ResolverCache resolverCache, IEnumerable<IDataFilter<ModelTokenType>> filters)
		{
			ResolverCache = resolverCache;
			ModelFilters = filters;
		}
	}
}
