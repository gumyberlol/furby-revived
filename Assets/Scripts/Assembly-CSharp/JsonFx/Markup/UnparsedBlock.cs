using System;
using System.IO;
using JsonFx.Serialization;

namespace JsonFx.Markup
{
	public class UnparsedBlock : ITextFormattable<MarkupTokenType>
	{
		public string Begin { get; set; }

		public string End { get; set; }

		public string Value { get; set; }

		public UnparsedBlock()
		{
		}

		public UnparsedBlock(string begin, string end, string value)
		{
			Begin = begin;
			End = end;
			Value = value;
		}

		public void Format(ITextFormatter<MarkupTokenType> formatter, TextWriter writer)
		{
			writer.Write('<');
			writer.Write(Begin);
			writer.Write(Value);
			writer.Write(End);
			writer.Write('>');
		}

		public override string ToString()
		{
			return '<' + Begin + Value + End + '>';
		}

		public override bool Equals(object value)
		{
			UnparsedBlock unparsedBlock = value as UnparsedBlock;
			if (unparsedBlock == null)
			{
				return false;
			}
			return StringComparer.Ordinal.Equals(Begin, unparsedBlock.Begin) && StringComparer.Ordinal.Equals(End, unparsedBlock.End) && StringComparer.Ordinal.Equals(Value, unparsedBlock.Value);
		}

		public override int GetHashCode()
		{
			int num = -118650411;
			num = -1521134295 * num + StringComparer.Ordinal.GetHashCode(Begin);
			num = -1521134295 * num + StringComparer.Ordinal.GetHashCode(End);
			return -1521134295 * num + StringComparer.Ordinal.GetHashCode(Value);
		}
	}
}
