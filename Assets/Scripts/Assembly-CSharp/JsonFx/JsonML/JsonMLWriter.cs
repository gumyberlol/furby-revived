using System;
using System.Collections.Generic;
using JsonFx.IO;
using JsonFx.Markup;
using JsonFx.Model;
using JsonFx.Serialization;

namespace JsonFx.JsonML
{
	public class JsonMLWriter
	{
		public class JsonMLOutTransformer : IDataTransformer<ModelTokenType, MarkupTokenType>
		{
			private const string ErrorUnexpectedToken = "Unexpected token ({0})";

			private const string ErrorMissingTagName = "Missing JsonML tag name";

			private const string ErrorInvalidAttributeValue = "Invalid attribute value token ({0})";

			private const string ErrorUnterminatedAttributeBlock = "Unterminated attribute block ({0})";

			public IEnumerable<Token<MarkupTokenType>> Transform(IEnumerable<Token<ModelTokenType>> input)
			{
				if (input == null)
				{
					throw new ArgumentNullException("input");
				}
				IStream<Token<ModelTokenType>> stream = Stream<Token<ModelTokenType>>.Create(input);
				Token<ModelTokenType> token = stream.Peek();
				while (!stream.IsCompleted)
				{
					switch (token.TokenType)
					{
					case ModelTokenType.ArrayBegin:
					{
						stream.Pop();
						token = stream.Peek();
						if (token.TokenType != ModelTokenType.Primitive)
						{
							throw new TokenException<ModelTokenType>(token, "Missing JsonML tag name");
						}
						DataName tagName = ((!(token.Value is DataName)) ? new DataName(token.ValueAsString()) : ((DataName)token.Value));
						stream.Pop();
						token = stream.Peek();
						yield return MarkupGrammar.TokenElementBegin(tagName);
						if (token.TokenType != ModelTokenType.ObjectBegin)
						{
							break;
						}
						stream.Pop();
						token = stream.Peek();
						while (token.TokenType == ModelTokenType.Property)
						{
							yield return token.ChangeType(MarkupTokenType.Attribute);
							stream.Pop();
							token = stream.Peek();
							ModelTokenType tokenType = token.TokenType;
							if (tokenType == ModelTokenType.Primitive)
							{
								yield return token.ChangeType(MarkupTokenType.Primitive);
								stream.Pop();
								token = stream.Peek();
								continue;
							}
							throw new TokenException<ModelTokenType>(token, string.Format("Invalid attribute value token ({0})", token.TokenType));
						}
						if (token.TokenType != ModelTokenType.ObjectEnd)
						{
							throw new TokenException<ModelTokenType>(token, string.Format("Unterminated attribute block ({0})", token.TokenType));
						}
						stream.Pop();
						token = stream.Peek();
						break;
					}
					case ModelTokenType.ArrayEnd:
						yield return MarkupGrammar.TokenElementEnd;
						stream.Pop();
						token = stream.Peek();
						break;
					case ModelTokenType.Primitive:
						yield return token.ChangeType(MarkupTokenType.Primitive);
						stream.Pop();
						token = stream.Peek();
						break;
					default:
						throw new TokenException<ModelTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
					}
				}
			}
		}
	}
}
