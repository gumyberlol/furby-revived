using System;
using System.Collections.Generic;
using System.IO;

namespace JsonFx.Serialization
{
	internal class TransformTokenizer<TIn, TOut> : IDisposable, ITextTokenizer<TOut>
	{
		private readonly ITextTokenizer<TIn> Tokenizer;

		private readonly IDataTransformer<TIn, TOut> Transformer;

		public int Column
		{
			get
			{
				return Tokenizer.Column;
			}
		}

		public int Line
		{
			get
			{
				return Tokenizer.Line;
			}
		}

		public long Index
		{
			get
			{
				return Tokenizer.Index;
			}
		}

		public TransformTokenizer(ITextTokenizer<TIn> tokenizer, IDataTransformer<TIn, TOut> transformer)
		{
			if (tokenizer == null)
			{
				new ArgumentNullException("tokenizer");
			}
			if (transformer == null)
			{
				new ArgumentNullException("transformer");
			}
			Tokenizer = tokenizer;
			Transformer = transformer;
		}

		public IEnumerable<Token<TOut>> GetTokens(TextReader reader)
		{
			IEnumerable<Token<TIn>> tokens = Tokenizer.GetTokens(reader);
			return Transformer.Transform(tokens);
		}

		public IEnumerable<Token<TOut>> GetTokens(string text)
		{
			IEnumerable<Token<TIn>> tokens = Tokenizer.GetTokens(text);
			return Transformer.Transform(tokens);
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
