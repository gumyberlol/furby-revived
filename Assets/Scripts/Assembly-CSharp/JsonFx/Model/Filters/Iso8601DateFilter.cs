using System;
using System.Collections.Generic;
using System.Globalization;
using JsonFx.IO;
using JsonFx.Serialization;

namespace JsonFx.Model.Filters
{
	public class Iso8601DateFilter : ModelFilter<DateTime>
	{
		public enum Precision
		{
			Seconds = 0,
			Milliseconds = 1,
			Ticks = 2
		}

		private const string ShortFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

		private const string LongFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";

		private const string FullFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFFK";

		private string iso8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";

		public Precision Format
		{
			get
			{
				switch (iso8601Format)
				{
				case "yyyy'-'MM'-'dd'T'HH':'mm':'ssK":
					return Precision.Seconds;
				case "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK":
					return Precision.Milliseconds;
				default:
					return Precision.Ticks;
				}
			}
			set
			{
				switch (value)
				{
				case Precision.Seconds:
					iso8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";
					break;
				case Precision.Milliseconds:
					iso8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffK";
					break;
				default:
					iso8601Format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFFK";
					break;
				}
			}
		}

		public override bool TryRead(DataReaderSettings settings, IStream<Token<ModelTokenType>> tokens, out DateTime value)
		{
			Token<ModelTokenType> token = tokens.Peek();
			if (token == null || token.TokenType != ModelTokenType.Primitive || !(token.Value is string))
			{
				value = default(DateTime);
				return false;
			}
			if (!TryParseIso8601(token.ValueAsString(), out value))
			{
				value = default(DateTime);
				return false;
			}
			tokens.Pop();
			return true;
		}

		public override bool TryWrite(DataWriterSettings settings, DateTime value, out IEnumerable<Token<ModelTokenType>> tokens)
		{
			tokens = new Token<ModelTokenType>[1] { ModelGrammar.TokenPrimitive(FormatIso8601(value)) };
			return true;
		}

		private static bool TryParseIso8601(string date, out DateTime value)
		{
			try
			{
				value = DateTime.ParseExact(date, "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.NoCurrentDateDefault | DateTimeStyles.RoundtripKind);
			}
			catch (Exception)
			{
				value = default(DateTime);
				return false;
			}
			if (value.Kind == DateTimeKind.Local)
			{
				value = value.ToUniversalTime();
			}
			return true;
		}

		private string FormatIso8601(DateTime value)
		{
			if (value.Kind == DateTimeKind.Local)
			{
				value = value.ToUniversalTime();
			}
			return value.ToString(iso8601Format);
		}
	}
}
