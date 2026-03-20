using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JsonFx.IO;
using JsonFx.Markup;
using JsonFx.Model;
using JsonFx.Serialization;
using JsonFx.Utils;

namespace JsonFx.JsonML
{
	public class JsonMLReader
	{
		public class JsonMLInTransformer : IDataTransformer<MarkupTokenType, ModelTokenType>
		{
			private const string SingleSpace = " ";

			private const string ErrorUnexpectedToken = "Unexpected token ({0})";

			private static readonly Regex RegexWhitespace = new Regex("\\s+", RegexOptions.ECMAScript | RegexOptions.CultureInvariant);

			public WhitespaceType Whitespace { get; set; }

			public IEnumerable<Token<ModelTokenType>> Transform(IEnumerable<Token<MarkupTokenType>> input)
			{
				if (input == null)
				{
					throw new ArgumentNullException("input");
				}
				IStream<Token<MarkupTokenType>> stream = Stream<Token<MarkupTokenType>>.Create(input);
				PrefixScopeChain scopeChain = new PrefixScopeChain();
				Token<MarkupTokenType> token = stream.Peek();
				while (!stream.IsCompleted)
				{
					switch (token.TokenType)
					{
					case MarkupTokenType.ElementBegin:
					case MarkupTokenType.ElementVoid:
					{
						bool hasProperties = false;
						bool isVoid = token.TokenType == MarkupTokenType.ElementVoid;
						DataName tagName = token.Name;
						yield return ModelGrammar.TokenArrayBeginUnnamed;
						yield return ModelGrammar.TokenPrimitive(tagName.ToPrefixedName());
						PrefixScopeChain.Scope scope = new PrefixScopeChain.Scope();
						string prefix = scopeChain.GetPrefix(tagName.NamespaceUri, false);
						if (!StringComparer.Ordinal.Equals(prefix, tagName.Prefix) && !string.IsNullOrEmpty(tagName.NamespaceUri))
						{
							scope[tagName.Prefix] = tagName.NamespaceUri;
							hasProperties = true;
							yield return ModelGrammar.TokenObjectBeginUnnamed;
						}
						scope.TagName = tagName;
						scopeChain.Push(scope);
						stream.Pop();
						token = stream.Peek();
						while (!stream.IsCompleted && token.TokenType == MarkupTokenType.Attribute)
						{
							if (!hasProperties)
							{
								hasProperties = true;
								yield return ModelGrammar.TokenObjectBeginUnnamed;
							}
							DataName attrName = token.Name;
							prefix = scopeChain.GetPrefix(attrName.NamespaceUri, false);
							if (!StringComparer.Ordinal.Equals(prefix, attrName.Prefix) && !string.IsNullOrEmpty(attrName.NamespaceUri))
							{
								scope[attrName.Prefix] = attrName.NamespaceUri;
							}
							yield return ModelGrammar.TokenProperty(new DataName(attrName.ToPrefixedName()));
							stream.Pop();
							token = stream.Peek();
							MarkupTokenType tokenType = token.TokenType;
							if (tokenType == MarkupTokenType.Primitive)
							{
								yield return token.ChangeType(ModelTokenType.Primitive);
								stream.Pop();
								token = stream.Peek();
								continue;
							}
							throw new TokenException<MarkupTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
						}
						if (hasProperties)
						{
							foreach (KeyValuePair<string, string> xmlns in scope)
							{
								if (string.IsNullOrEmpty(xmlns.Key))
								{
									yield return ModelGrammar.TokenProperty("xmlns");
								}
								else
								{
									yield return ModelGrammar.TokenProperty("xmlns:" + xmlns.Key);
								}
								yield return ModelGrammar.TokenPrimitive(xmlns.Value);
							}
							yield return ModelGrammar.TokenObjectEnd;
						}
						if (isVoid)
						{
							yield return ModelGrammar.TokenArrayEnd;
							scopeChain.Pop();
						}
						break;
					}
					case MarkupTokenType.ElementEnd:
						if (scopeChain.Count > 0)
						{
							yield return ModelGrammar.TokenArrayEnd;
						}
						scopeChain.Pop();
						stream.Pop();
						token = stream.Peek();
						break;
					case MarkupTokenType.Primitive:
					{
						if (token.Value is ITextFormattable<ModelTokenType> || token.Value is ITextFormattable<MarkupTokenType>)
						{
							yield return token.ChangeType(ModelTokenType.Primitive);
							stream.Pop();
							token = stream.Peek();
							break;
						}
						string value = token.ValueAsString();
						stream.Pop();
						token = stream.Peek();
						while (!stream.IsCompleted && token.TokenType == MarkupTokenType.Primitive && !(token.Value is ITextFormattable<ModelTokenType>) && !(token.Value is ITextFormattable<MarkupTokenType>))
						{
							value += token.ValueAsString();
							stream.Pop();
							token = stream.Peek();
						}
						switch (Whitespace)
						{
						case WhitespaceType.Normalize:
							value = RegexWhitespace.Replace(value, " ");
							break;
						case WhitespaceType.None:
							if (CharUtility.IsNullOrWhiteSpace(value))
							{
								goto end_IL_00b2;
							}
							break;
						}
						yield return ModelGrammar.TokenPrimitive(value);
						break;
					}
					default:
						{
							throw new TokenException<MarkupTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
						}
						end_IL_00b2:
						break;
					}
				}
				while (scopeChain.Count > 0)
				{
					scopeChain.Pop();
					yield return ModelGrammar.TokenArrayEnd;
				}
			}
		}
	}
}
