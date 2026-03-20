using System;
using System.Collections.Generic;
using JsonFx.IO;
using JsonFx.Markup;
using JsonFx.Model;
using JsonFx.Serialization;

namespace JsonFx.Html
{
	public class HtmlOutTransformer : IDataTransformer<ModelTokenType, MarkupTokenType>
	{
		private const string ErrorUnexpectedToken = "Unexpected token ({0})";

		private static readonly DataName RootTagName = new DataName("div");

		private static readonly DataName ArrayTagName = new DataName("ol");

		private static readonly DataName ArrayItemTagName = new DataName("li");

		private static readonly DataName ObjectTagName = new DataName("dl");

		private static readonly DataName ObjectPropertyKeyTagName = new DataName("dt");

		private static readonly DataName ObjectPropertyValueTagName = new DataName("dd");

		private static readonly DataName PrimitiveTagName = new DataName("span");

		private static readonly DataName HintAttributeName = new DataName("title");

		public IEnumerable<Token<MarkupTokenType>> Transform(IEnumerable<Token<ModelTokenType>> input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			IStream<Token<ModelTokenType>> stream = Stream<Token<ModelTokenType>>.Create(input);
			List<Token<MarkupTokenType>> list = new List<Token<MarkupTokenType>>();
			while (!stream.IsCompleted)
			{
				list.Add(MarkupGrammar.TokenElementBegin(RootTagName));
				TransformValue(list, stream);
				list.Add(MarkupGrammar.TokenElementEnd);
			}
			return list;
		}

		private void TransformValue(List<Token<MarkupTokenType>> output, IStream<Token<ModelTokenType>> input)
		{
			Token<ModelTokenType> token = input.Peek();
			switch (token.TokenType)
			{
			case ModelTokenType.ArrayBegin:
				TransformArray(output, input);
				break;
			case ModelTokenType.ObjectBegin:
				TransformObject(output, input);
				break;
			case ModelTokenType.Primitive:
				TransformPrimitive(output, input);
				break;
			default:
				throw new TokenException<ModelTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
			}
		}

		private void TransformPrimitive(List<Token<MarkupTokenType>> output, IStream<Token<ModelTokenType>> input)
		{
			Token<ModelTokenType> token = input.Pop();
			bool flag = !token.Name.IsEmpty;
			if (flag)
			{
				output.Add(MarkupGrammar.TokenElementBegin(PrimitiveTagName));
				output.Add(MarkupGrammar.TokenAttribute(HintAttributeName));
				output.Add(MarkupGrammar.TokenPrimitive(token.Name));
			}
			output.Add(token.ChangeType(MarkupTokenType.Primitive));
			if (flag)
			{
				output.Add(MarkupGrammar.TokenElementEnd);
			}
		}

		private void TransformArray(List<Token<MarkupTokenType>> output, IStream<Token<ModelTokenType>> input)
		{
			Token<ModelTokenType> token = input.Pop();
			output.Add(MarkupGrammar.TokenElementBegin(ArrayTagName));
			if (!token.Name.IsEmpty)
			{
				output.Add(MarkupGrammar.TokenAttribute(HintAttributeName));
				output.Add(MarkupGrammar.TokenPrimitive(token.Name));
			}
			while (!input.IsCompleted)
			{
				token = input.Peek();
				switch (token.TokenType)
				{
				case ModelTokenType.ArrayEnd:
					input.Pop();
					output.Add(MarkupGrammar.TokenElementEnd);
					return;
				case ModelTokenType.ObjectBegin:
				case ModelTokenType.ArrayBegin:
				case ModelTokenType.Primitive:
					break;
				default:
					throw new TokenException<ModelTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
				}
				output.Add(MarkupGrammar.TokenElementBegin(ArrayItemTagName));
				TransformValue(output, input);
				output.Add(MarkupGrammar.TokenElementEnd);
			}
		}

		private void TransformObject(List<Token<MarkupTokenType>> output, IStream<Token<ModelTokenType>> input)
		{
			Token<ModelTokenType> token = input.Pop();
			output.Add(MarkupGrammar.TokenElementBegin(ObjectTagName));
			if (!token.Name.IsEmpty)
			{
				output.Add(MarkupGrammar.TokenAttribute(HintAttributeName));
				output.Add(MarkupGrammar.TokenPrimitive(token.Name));
			}
			while (!input.IsCompleted)
			{
				token = input.Peek();
				switch (token.TokenType)
				{
				case ModelTokenType.ObjectEnd:
					input.Pop();
					output.Add(MarkupGrammar.TokenElementEnd);
					return;
				case ModelTokenType.Property:
					break;
				default:
					throw new TokenException<ModelTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
				}
				input.Pop();
				output.Add(MarkupGrammar.TokenElementBegin(ObjectPropertyKeyTagName));
				output.Add(MarkupGrammar.TokenPrimitive(token.Name));
				output.Add(MarkupGrammar.TokenElementEnd);
				output.Add(MarkupGrammar.TokenElementBegin(ObjectPropertyValueTagName));
				TransformValue(output, input);
				output.Add(MarkupGrammar.TokenElementEnd);
			}
		}
	}
}
