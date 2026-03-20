using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JsonFx.Linq;
using JsonFx.Serialization;

namespace JsonFx.Model
{
	public abstract class ModelReader : DataReader<ModelTokenType>, IQueryableReader, IDataReader
	{
		protected ModelReader(DataReaderSettings settings)
			: base(settings)
		{
		}

		IQueryable<T> IQueryableReader.Query<T>(TextReader input, T ignored)
		{
			return Query<T>(input);
		}

		IQueryable<T> IQueryableReader.Query<T>(TextReader input)
		{
			return Query<T>(input);
		}

		IQueryable<T> IQueryableReader.Query<T>(string input, T ignored)
		{
			return Query<T>(input);
		}

		IQueryable<T> IQueryableReader.Query<T>(string input)
		{
			return Query<T>(input);
		}

		protected override ITokenAnalyzer<ModelTokenType> GetAnalyzer()
		{
			return new ModelAnalyzer(Settings);
		}

		public Query<T> Query<T>(TextReader input, T ignored)
		{
			return Query<T>(input);
		}

		public Query<T> Query<T>(TextReader input)
		{
			ITextTokenizer<ModelTokenType> tokenizer = GetTokenizer();
			if (tokenizer == null)
			{
				throw new InvalidOperationException("Tokenizer is invalid");
			}
			IEnumerable<Token<ModelTokenType>> tokens = tokenizer.GetTokens(input);
			return new Query<T>(GetAnalyzer(), tokens);
		}

		public IQueryable Query(TextReader input)
		{
			ITextTokenizer<ModelTokenType> tokenizer = GetTokenizer();
			if (tokenizer == null)
			{
				throw new InvalidOperationException("Tokenizer is invalid");
			}
			IEnumerable<Token<ModelTokenType>> tokens = tokenizer.GetTokens(input);
			return new Query<object>(GetAnalyzer(), tokens);
		}

		public IQueryable Query(TextReader input, Type targetType)
		{
			ITextTokenizer<ModelTokenType> tokenizer = GetTokenizer();
			if (tokenizer == null)
			{
				throw new InvalidOperationException("Tokenizer is invalid");
			}
			IEnumerable<Token<ModelTokenType>> tokens = tokenizer.GetTokens(input);
			try
			{
				return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(targetType), GetAnalyzer(), tokens);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}

		public Query<T> Query<T>(string input, T ignored)
		{
			return Query<T>(input);
		}

		public Query<T> Query<T>(string input)
		{
			ITextTokenizer<ModelTokenType> tokenizer = GetTokenizer();
			if (tokenizer == null)
			{
				throw new InvalidOperationException("Tokenizer is invalid");
			}
			IEnumerable<Token<ModelTokenType>> tokens = tokenizer.GetTokens(input);
			return new Query<T>(GetAnalyzer(), tokens);
		}

		public IQueryable Query(string input)
		{
			ITextTokenizer<ModelTokenType> tokenizer = GetTokenizer();
			if (tokenizer == null)
			{
				throw new InvalidOperationException("Tokenizer is invalid");
			}
			IEnumerable<Token<ModelTokenType>> tokens = tokenizer.GetTokens(input);
			return new Query<object>(GetAnalyzer(), tokens);
		}

		public IQueryable Query(string input, Type targetType)
		{
			ITextTokenizer<ModelTokenType> tokenizer = GetTokenizer();
			if (tokenizer == null)
			{
				throw new InvalidOperationException("Tokenizer is invalid");
			}
			IEnumerable<Token<ModelTokenType>> tokens = tokenizer.GetTokens(input);
			try
			{
				return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(targetType), GetAnalyzer(), tokens);
			}
			catch (TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}
	}
}
