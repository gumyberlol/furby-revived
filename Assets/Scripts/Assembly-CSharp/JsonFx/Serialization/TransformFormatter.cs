using System;
using System.Collections.Generic;
using System.IO;

namespace JsonFx.Serialization
{
	internal class TransformFormatter<TIn, TOut> : ITextFormatter<TIn>
	{
		private readonly ITextFormatter<TOut> Formatter;

		private readonly IDataTransformer<TIn, TOut> Transformer;

		public TransformFormatter(ITextFormatter<TOut> formatter, IDataTransformer<TIn, TOut> transformer)
		{
			if (formatter == null)
			{
				new ArgumentNullException("formatter");
			}
			if (transformer == null)
			{
				new ArgumentNullException("transformer");
			}
			Formatter = formatter;
			Transformer = transformer;
		}

		public void Format(IEnumerable<Token<TIn>> tokens, TextWriter writer)
		{
			IEnumerable<Token<TOut>> tokens2 = Transformer.Transform(tokens);
			Formatter.Format(tokens2, writer);
		}

		public string Format(IEnumerable<Token<TIn>> tokens)
		{
			IEnumerable<Token<TOut>> tokens2 = Transformer.Transform(tokens);
			return Formatter.Format(tokens2);
		}
	}
}
