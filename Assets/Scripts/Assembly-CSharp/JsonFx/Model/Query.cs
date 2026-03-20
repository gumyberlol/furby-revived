using System;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Linq;
using JsonFx.Serialization;

namespace JsonFx.Model
{
	public class Query<T> : JsonFx.Linq.Query<T>
	{
		private readonly ITokenAnalyzer<ModelTokenType> Analyzer;

		private readonly IEnumerable<IEnumerable<Token<ModelTokenType>>> Sequences;

		internal Query(ITokenAnalyzer<ModelTokenType> analyzer, IEnumerable<Token<ModelTokenType>> sequence)
			: this(analyzer, sequence.SplitValues())
		{
		}

		internal Query(ITokenAnalyzer<ModelTokenType> analyzer, IEnumerable<IEnumerable<Token<ModelTokenType>>> sequences)
			: base((IQueryProvider)new QueryProvider(analyzer, sequences.AsQueryable()))
		{
			Analyzer = analyzer;
			Sequences = sequences;
		}

		public Query<T> Descendants()
		{
			return new Query<T>(Analyzer, Sequences.SelectMany((IEnumerable<Token<ModelTokenType>> sequence) => sequence.Descendants()));
		}

		public Query<T> DescendantsAndSelf()
		{
			return new Query<T>(Analyzer, Sequences.SelectMany((IEnumerable<Token<ModelTokenType>> sequence) => sequence.DescendantsAndSelf()));
		}

		public Query<T> ArrayItems()
		{
			return new Query<T>(Analyzer, Sequences.SelectMany((IEnumerable<Token<ModelTokenType>> sequence) => sequence.ArrayItems()));
		}

		public Query<T> ArrayItems(Func<int, bool> predicate)
		{
			return new Query<T>(Analyzer, Sequences.SelectMany((IEnumerable<Token<ModelTokenType>> sequence) => sequence.ArrayItems(predicate)));
		}

		public Query<T> WhereHasProperty(Func<DataName, bool> predicate)
		{
			return new Query<T>(Analyzer, Sequences.Where((IEnumerable<Token<ModelTokenType>> sequence) => sequence.HasProperty(predicate)));
		}

		public ILookup<DataName, Query<T>> Properties(Func<DataName, bool> predicate)
		{
			return Sequences.SelectMany((IEnumerable<Token<ModelTokenType>> sequence) => sequence.Properties(predicate)).ToLookup((KeyValuePair<DataName, IEnumerable<Token<ModelTokenType>>> pair) => pair.Key, (KeyValuePair<DataName, IEnumerable<Token<ModelTokenType>>> pair) => new Query<T>(Analyzer, pair.Value));
		}

		public Query<T> WhereIsArray()
		{
			return new Query<T>(Analyzer, Sequences.Where((IEnumerable<Token<ModelTokenType>> sequence) => sequence.IsArray()));
		}

		public Query<T> WhereIsObject()
		{
			return new Query<T>(Analyzer, Sequences.Where((IEnumerable<Token<ModelTokenType>> sequence) => sequence.IsObject()));
		}

		public Query<T> WhereIsPrimitive()
		{
			return new Query<T>(Analyzer, Sequences.Where((IEnumerable<Token<ModelTokenType>> sequence) => sequence.IsPrimitive()));
		}
	}
}
