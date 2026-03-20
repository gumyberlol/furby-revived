using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using JsonFx.IO;
using JsonFx.Serialization;

namespace JsonFx.Model.Filters
{
	public class MSAjaxDateFilter : ModelFilter<DateTime>
	{
		private const long MinValueMilliseconds = -62135596800000L;

		private const long MaxValueMilliseconds = 253402300800000L;

		private const string MSAjaxDatePattern = "^/Date\\(([+\\-]?\\d+?)([+\\-]\\d{4})?\\)/$";

		private const string MSAjaxDatePrefix = "/Date(";

		private const string MSAjaxDateSuffix = ")/";

		private static readonly DateTime EcmaScriptEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		private static readonly Regex MSAjaxDateRegex = new Regex("^/Date\\(([+\\-]?\\d+?)([+\\-]\\d{4})?\\)/$", RegexOptions.ECMAScript | RegexOptions.CultureInvariant);

		public override bool TryRead(DataReaderSettings settings, IStream<Token<ModelTokenType>> tokens, out DateTime value)
		{
			Token<ModelTokenType> token = tokens.Peek();
			if (token == null || token.TokenType != ModelTokenType.Primitive || !(token.Value is string))
			{
				value = default(DateTime);
				return false;
			}
			if (!TryParseMSAjaxDate(token.ValueAsString(), out value))
			{
				value = default(DateTime);
				return false;
			}
			tokens.Pop();
			return true;
		}

		public override bool TryWrite(DataWriterSettings settings, DateTime value, out IEnumerable<Token<ModelTokenType>> tokens)
		{
			tokens = new Token<ModelTokenType>[1] { ModelGrammar.TokenPrimitive(FormatMSAjaxDate(value)) };
			return true;
		}

		internal static bool TryParseMSAjaxDate(string date, out DateTime value)
		{
			if (string.IsNullOrEmpty(date))
			{
				value = default(DateTime);
				return false;
			}
			Match match = MSAjaxDateRegex.Match(date);
			long result;
			if (!match.Success || !long.TryParse(match.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
			{
				value = default(DateTime);
				return false;
			}
			if (result <= -62135596800000L)
			{
				value = DateTime.MinValue;
			}
			else if (result >= 253402300800000L)
			{
				value = DateTime.MaxValue;
			}
			else
			{
				value = EcmaScriptEpoch.AddMilliseconds(result);
			}
			return true;
		}

		private string FormatMSAjaxDate(DateTime value)
		{
			if (value.Kind == DateTimeKind.Local)
			{
				value = value.ToUniversalTime();
			}
			long num = (long)value.Subtract(EcmaScriptEpoch).TotalMilliseconds;
			return "/Date(" + num + ")/";
		}
	}
}
