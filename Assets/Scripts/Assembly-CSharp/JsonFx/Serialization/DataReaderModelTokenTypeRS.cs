using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JsonFx.Model;

namespace JsonFx.Serialization
{
	public abstract class DataReaderModelTokenTypeRS : IDataReaderRS
	{
		private readonly DataReaderSettingsRS settings;

		public abstract IEnumerable<string> ContentType { get; }

		public DataReaderSettingsRS Settings
		{
			get
			{
				return settings;
			}
		}

		protected DataReaderModelTokenTypeRS(DataReaderSettingsRS settings)
		{
			if (settings == null)
			{
				throw new NullReferenceException("settings");
			}
			this.settings = settings;
		}

		public TResult Read<TResult>(TextReader input, TResult ignored)
		{
			return Read<TResult>(input);
		}

		public TResult Read<TResult>(TextReader input)
		{
			object obj = Read(input, typeof(TResult));
			return (!(obj is TResult)) ? default(TResult) : ((TResult)obj);
		}

		public object Read(TextReader input)
		{
			return Read(input, null);
		}

		public object Read(TextReader input, Type targetType)
		{
			ITextTokenizer<ModelTokenType> tokenizer = GetTokenizer();
			if (tokenizer == null)
			{
				throw new InvalidOperationException("Tokenizer is invalid");
			}
			return ReadSingle(tokenizer, tokenizer.GetTokens(input), targetType);
		}

		public TResult Read<TResult>(string input, TResult ignored)
		{
			return Read<TResult>(input);
		}

		public TResult Read<TResult>(string input)
		{
			object obj = Read(input, typeof(TResult));
			return (!(obj is TResult)) ? default(TResult) : ((TResult)obj);
		}

		public virtual object Read(string input)
		{
			return Read(input, null);
		}

		public virtual object Read(string input, Type targetType)
		{
			ITextTokenizer<ModelTokenType> tokenizer = GetTokenizer();
			if (tokenizer == null)
			{
				throw new ArgumentNullException("tokenizer");
			}
			return ReadSingle(tokenizer, tokenizer.GetTokens(input), targetType);
		}

		public IEnumerable ReadMany(TextReader input)
		{
			ITextTokenizer<ModelTokenType> tokenizer = GetTokenizer();
			if (tokenizer == null)
			{
				throw new ArgumentNullException("tokenizer");
			}
			ITokenAnalyzerRS<ModelTokenType> analyzer = GetAnalyzer();
			if (analyzer == null)
			{
				throw new ArgumentNullException("analyzer");
			}
			try
			{
				return analyzer.Analyze(tokenizer.GetTokens(input));
			}
			catch (DeserializationException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new DeserializationException(ex2.Message, tokenizer.Index, tokenizer.Line, tokenizer.Column, ex2);
			}
		}

		private object ReadSingle(ITextTokenizer<ModelTokenType> tokenizer, IEnumerable<Token<ModelTokenType>> tokens, Type targetType)
		{
			ITokenAnalyzerRS<ModelTokenType> analyzer = GetAnalyzer();
			if (analyzer == null)
			{
				throw new ArgumentNullException("analyzer");
			}
			try
			{
				IEnumerator enumerator = analyzer.Analyze(tokens, targetType).GetEnumerator();
				if (!enumerator.MoveNext())
				{
					return null;
				}
				object current = enumerator.Current;
				if (!Settings.AllowTrailingContent && enumerator.MoveNext())
				{
					throw new DeserializationException("Invalid trailing content", tokenizer.Index, tokenizer.Line, tokenizer.Column);
				}
				return current;
			}
			catch (DeserializationException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new DeserializationException(ex2.Message, tokenizer.Index, tokenizer.Line, tokenizer.Column, ex2);
			}
		}

		protected abstract ITextTokenizer<ModelTokenType> GetTokenizer();

		protected abstract ITokenAnalyzerRS<ModelTokenType> GetAnalyzer();
	}
}
