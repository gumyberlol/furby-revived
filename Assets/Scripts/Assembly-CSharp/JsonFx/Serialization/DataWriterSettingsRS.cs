using System;
using System.Collections.Generic;
using JsonFx.Model;
using JsonFx.Model.Filters;
using JsonFx.Serialization.Filters;
using JsonFx.Serialization.GraphCycles;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Serialization
{
	public sealed class DataWriterSettingsRS : IResolverCacheContainerRS
	{
		private bool prettyPrint;

		private GraphCycleType graphCycles;

		private string tab = "\t";

		private int maxDepth;

		private string newLine = Environment.NewLine;

		private readonly ResolverCacheRS ResolverCache;

		private readonly IEnumerable<IDataFilterRS<ModelTokenType>> ModelFilters;

		ResolverCacheRS IResolverCacheContainerRS.ResolverCache
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

		public DataWriterSettingsRS()
			: this(new PocoResolverStrategyRS(), new Iso8601DateFilterRS())
		{
		}

		public DataWriterSettingsRS(params IDataFilterRS<ModelTokenType>[] filters)
			: this(new PocoResolverStrategyRS(), filters)
		{
		}

		public DataWriterSettingsRS(IEnumerable<IDataFilterRS<ModelTokenType>> filters)
			: this(new PocoResolverStrategyRS(), filters)
		{
		}

		public DataWriterSettingsRS(IResolverStrategyRS strategy, params IDataFilterRS<ModelTokenType>[] filters)
			: this(new ResolverCacheRS(strategy), filters)
		{
		}

		public DataWriterSettingsRS(IResolverStrategyRS strategy, IEnumerable<IDataFilterRS<ModelTokenType>> filters)
			: this(new ResolverCacheRS(strategy), filters)
		{
		}

		public DataWriterSettingsRS(ResolverCacheRS resolverCache, params IDataFilterRS<ModelTokenType>[] filters)
			: this(resolverCache, (IEnumerable<IDataFilterRS<ModelTokenType>>)filters)
		{
		}

		public DataWriterSettingsRS(ResolverCacheRS resolverCache, IEnumerable<IDataFilterRS<ModelTokenType>> filters)
		{
			ResolverCache = resolverCache;
			ModelFilters = filters;
		}
	}
}
