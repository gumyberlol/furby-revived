using System;
using System.Collections.Generic;
using JsonFx.IO;
using JsonFx.Serialization;

namespace JsonFx.Model
{
	public static class ModelSubsequencer
	{
		private const string ErrorUnexpectedEndOfInput = "Unexpected end of token stream";

		private const string ErrorInvalidPropertyValue = "Invalid property value token ({0})";

		public static bool IsPrimitive(this IEnumerable<Token<ModelTokenType>> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IList<Token<ModelTokenType>> list = source as IList<Token<ModelTokenType>>;
			if (list != null)
			{
				return list.Count > 0 && list[0].TokenType == ModelTokenType.Primitive;
			}
			using (IEnumerator<Token<ModelTokenType>> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Token<ModelTokenType> current = enumerator.Current;
					return current.TokenType == ModelTokenType.Primitive;
				}
			}
			return false;
		}

		public static bool IsObject(this IEnumerable<Token<ModelTokenType>> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IList<Token<ModelTokenType>> list = source as IList<Token<ModelTokenType>>;
			if (list != null)
			{
				return list.Count > 0 && list[0].TokenType == ModelTokenType.ObjectBegin;
			}
			using (IEnumerator<Token<ModelTokenType>> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Token<ModelTokenType> current = enumerator.Current;
					return current.TokenType == ModelTokenType.ObjectBegin;
				}
			}
			return false;
		}

		public static bool HasProperty(this IEnumerable<Token<ModelTokenType>> source, Func<DataName, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IStream<Token<ModelTokenType>> stream = Stream<Token<ModelTokenType>>.Create(source);
			if (stream.IsCompleted || stream.Pop().TokenType != ModelTokenType.ObjectBegin)
			{
				return false;
			}
			int num = 0;
			while (!stream.IsCompleted)
			{
				Token<ModelTokenType> token = stream.Peek();
				switch (token.TokenType)
				{
				case ModelTokenType.ObjectBegin:
				case ModelTokenType.ArrayBegin:
					num++;
					stream.Pop();
					break;
				case ModelTokenType.ArrayEnd:
					num--;
					stream.Pop();
					break;
				case ModelTokenType.ObjectEnd:
					if (num != 0)
					{
						num--;
						stream.Pop();
						break;
					}
					return false;
				case ModelTokenType.Property:
					stream.Pop();
					if (num != 0 || (predicate != null && !predicate(token.Name)))
					{
						break;
					}
					return true;
				default:
					stream.Pop();
					break;
				}
			}
			return false;
		}

		public static IEnumerable<Token<ModelTokenType>> Property(this IEnumerable<Token<ModelTokenType>> source, DataName propertyName)
		{
			using (IEnumerator<KeyValuePair<DataName, IEnumerable<Token<ModelTokenType>>>> enumerator = source.Properties((DataName name) => name == propertyName).GetEnumerator())
			{
				object result;
				if (enumerator.MoveNext())
				{
					IEnumerable<Token<ModelTokenType>> value = enumerator.Current.Value;
					result = value;
				}
				else
				{
					result = null;
				}
				return (IEnumerable<Token<ModelTokenType>>)result;
			}
		}

		public static IEnumerable<KeyValuePair<DataName, IEnumerable<Token<ModelTokenType>>>> Properties(this IEnumerable<Token<ModelTokenType>> source)
		{
			return source.Properties(null);
		}

		public static IEnumerable<KeyValuePair<DataName, IEnumerable<Token<ModelTokenType>>>> Properties(this IEnumerable<Token<ModelTokenType>> source, Func<DataName, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(source is IList<Token<ModelTokenType>>))
			{
				source = new SequenceBuffer<Token<ModelTokenType>>(source);
			}
			return PropertiesIterator(source, predicate);
		}

		private static IEnumerable<KeyValuePair<DataName, IEnumerable<Token<ModelTokenType>>>> PropertiesIterator(IEnumerable<Token<ModelTokenType>> source, Func<DataName, bool> predicate)
		{
			IStream<Token<ModelTokenType>> stream = Stream<Token<ModelTokenType>>.Create(source);
			if (stream.IsCompleted || stream.Pop().TokenType != ModelTokenType.ObjectBegin)
			{
				yield break;
			}
			int depth = 0;
			while (!stream.IsCompleted)
			{
				Token<ModelTokenType> token = stream.Peek();
				switch (token.TokenType)
				{
				case ModelTokenType.ObjectBegin:
				case ModelTokenType.ArrayBegin:
					depth++;
					stream.Pop();
					break;
				case ModelTokenType.ArrayEnd:
					depth--;
					stream.Pop();
					break;
				case ModelTokenType.ObjectEnd:
					if (depth != 0)
					{
						depth--;
						stream.Pop();
						break;
					}
					yield break;
				case ModelTokenType.Property:
					stream.Pop();
					if (depth == 0 && (predicate == null || predicate(token.Name)))
					{
						yield return new KeyValuePair<DataName, IEnumerable<Token<ModelTokenType>>>(token.Name, SpliceNextValue(stream));
					}
					break;
				default:
					stream.Pop();
					break;
				}
			}
		}

		public static bool IsArray(this IEnumerable<Token<ModelTokenType>> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			IList<Token<ModelTokenType>> list = source as IList<Token<ModelTokenType>>;
			if (list != null)
			{
				return list.Count > 0 && list[0].TokenType == ModelTokenType.ArrayBegin;
			}
			using (IEnumerator<Token<ModelTokenType>> enumerator = source.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Token<ModelTokenType> current = enumerator.Current;
					return current.TokenType == ModelTokenType.ArrayBegin;
				}
			}
			return false;
		}

		public static IEnumerable<IEnumerable<Token<ModelTokenType>>> ArrayItems(this IEnumerable<Token<ModelTokenType>> source)
		{
			return source.ArrayItems(null);
		}

		public static IEnumerable<IEnumerable<Token<ModelTokenType>>> ArrayItems(this IEnumerable<Token<ModelTokenType>> source, Func<int, bool> predicate)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(source is IList<Token<ModelTokenType>>))
			{
				source = new SequenceBuffer<Token<ModelTokenType>>(source);
			}
			return ArrayItemsIterator(source, predicate);
		}

		private static IEnumerable<IEnumerable<Token<ModelTokenType>>> ArrayItemsIterator(IEnumerable<Token<ModelTokenType>> source, Func<int, bool> predicate)
		{
			IStream<Token<ModelTokenType>> stream = Stream<Token<ModelTokenType>>.Create(source);
			if (stream.IsCompleted)
			{
				yield break;
			}
			if (stream.Pop().TokenType != ModelTokenType.ArrayBegin)
			{
				yield return source;
				yield break;
			}
			int index = 0;
			while (!stream.IsCompleted)
			{
				Token<ModelTokenType> token = stream.Peek();
				if (token.TokenType == ModelTokenType.ArrayEnd)
				{
					break;
				}
				if (predicate == null || predicate(index))
				{
					yield return SpliceNextValue(stream);
				}
				else
				{
					SkipNextValue(stream);
				}
				index++;
			}
		}

		public static IEnumerable<IEnumerable<Token<ModelTokenType>>> Descendants(this IEnumerable<Token<ModelTokenType>> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(source is IList<Token<ModelTokenType>>))
			{
				source = new SequenceBuffer<Token<ModelTokenType>>(source);
			}
			return DescendantsIterator(source);
		}

		private static IEnumerable<IEnumerable<Token<ModelTokenType>>> DescendantsIterator(IEnumerable<Token<ModelTokenType>> source)
		{
			if (source.IsPrimitive())
			{
				yield break;
			}
			if (source.IsObject())
			{
				foreach (KeyValuePair<DataName, IEnumerable<Token<ModelTokenType>>> property in source.Properties(null))
				{
					yield return property.Value;
					foreach (IEnumerable<Token<ModelTokenType>> item2 in property.Value.Descendants())
					{
						yield return item2;
					}
				}
				yield break;
			}
			if (!source.IsArray())
			{
				yield break;
			}
			foreach (IEnumerable<Token<ModelTokenType>> item in source.ArrayItems(null))
			{
				yield return item;
				foreach (IEnumerable<Token<ModelTokenType>> item3 in item.Descendants())
				{
					yield return item3;
				}
			}
		}

		public static IEnumerable<IEnumerable<Token<ModelTokenType>>> DescendantsAndSelf(this IEnumerable<Token<ModelTokenType>> source)
		{
			if (source == null)
			{
				throw new ArgumentNullException("source");
			}
			if (!(source is IList<Token<ModelTokenType>>))
			{
				source = new SequenceBuffer<Token<ModelTokenType>>(source);
			}
			return DescendantsAndSelfIterator(source);
		}

		private static IEnumerable<IEnumerable<Token<ModelTokenType>>> DescendantsAndSelfIterator(IEnumerable<Token<ModelTokenType>> source)
		{
			yield return source;
			foreach (IEnumerable<Token<ModelTokenType>> item in DescendantsIterator(source))
			{
				yield return item;
			}
		}

		internal static IEnumerable<IEnumerable<Token<ModelTokenType>>> SplitValues(this IEnumerable<Token<ModelTokenType>> source)
		{
			if (source == null)
			{
				return new IEnumerable<Token<ModelTokenType>>[0];
			}
			if (!(source is IList<Token<ModelTokenType>>))
			{
				source = new SequenceBuffer<Token<ModelTokenType>>(source);
			}
			return SplitValuesIterator(source);
		}

		private static IEnumerable<IEnumerable<Token<ModelTokenType>>> SplitValuesIterator(IEnumerable<Token<ModelTokenType>> source)
		{
			using (IStream<Token<ModelTokenType>> stream = Stream<Token<ModelTokenType>>.Create(source))
			{
				while (!stream.IsCompleted)
				{
					yield return SpliceNextValue(stream);
				}
			}
		}

		[Obsolete("TODO: Lazy does not mix well with shared IStream<T>.", true)]
		private static IEnumerable<Token<ModelTokenType>> SpliceNextValueLazy(IStream<Token<ModelTokenType>> stream)
		{
			if (stream.IsCompleted)
			{
				yield break;
			}
			int depth = 0;
			Token<ModelTokenType> token = stream.Pop();
			switch (token.TokenType)
			{
			case ModelTokenType.Primitive:
				yield return token;
				break;
			case ModelTokenType.ObjectBegin:
			case ModelTokenType.ArrayBegin:
				depth++;
				yield return token;
				while (!stream.IsCompleted && depth > 0)
				{
					token = stream.Pop();
					switch (token.TokenType)
					{
					case ModelTokenType.ObjectBegin:
					case ModelTokenType.ArrayBegin:
						depth++;
						yield return token;
						break;
					case ModelTokenType.ObjectEnd:
					case ModelTokenType.ArrayEnd:
						depth--;
						yield return token;
						break;
					default:
						yield return token;
						break;
					}
				}
				if (depth > 0)
				{
					throw new TokenException<ModelTokenType>(ModelGrammar.TokenNone, "Unexpected end of token stream");
				}
				break;
			default:
				throw new TokenException<ModelTokenType>(token, string.Format("Invalid property value token ({0})", token.TokenType));
			}
		}

		private static IEnumerable<Token<ModelTokenType>> SpliceNextValue(IStream<Token<ModelTokenType>> stream)
		{
			if (stream.IsCompleted)
			{
				throw new TokenException<ModelTokenType>(ModelGrammar.TokenNone, "Unexpected end of token stream");
			}
			stream.BeginChunk();
			int num = 0;
			Token<ModelTokenType> token = stream.Pop();
			switch (token.TokenType)
			{
			case ModelTokenType.Primitive:
				return stream.EndChunk();
			case ModelTokenType.ObjectBegin:
			case ModelTokenType.ArrayBegin:
				num++;
				while (!stream.IsCompleted && num > 0)
				{
					switch (stream.Pop().TokenType)
					{
					case ModelTokenType.ObjectBegin:
					case ModelTokenType.ArrayBegin:
						num++;
						break;
					case ModelTokenType.ObjectEnd:
					case ModelTokenType.ArrayEnd:
						num--;
						break;
					}
				}
				if (num > 0)
				{
					throw new TokenException<ModelTokenType>(ModelGrammar.TokenNone, "Unexpected end of token stream");
				}
				return stream.EndChunk();
			default:
				throw new TokenException<ModelTokenType>(token, string.Format("Invalid property value token ({0})", token.TokenType));
			}
		}

		private static void SkipNextValue(IStream<Token<ModelTokenType>> stream)
		{
			if (stream.IsCompleted)
			{
				return;
			}
			int num = 0;
			Token<ModelTokenType> token = stream.Pop();
			switch (token.TokenType)
			{
			case ModelTokenType.Primitive:
				break;
			case ModelTokenType.ObjectBegin:
			case ModelTokenType.ArrayBegin:
				num++;
				while (!stream.IsCompleted && num > 0)
				{
					switch (stream.Pop().TokenType)
					{
					case ModelTokenType.ObjectBegin:
					case ModelTokenType.ArrayBegin:
						num++;
						break;
					case ModelTokenType.ObjectEnd:
					case ModelTokenType.ArrayEnd:
						num--;
						break;
					}
				}
				if (num > 0)
				{
					throw new TokenException<ModelTokenType>(ModelGrammar.TokenNone, "Unexpected end of token stream");
				}
				break;
			default:
				throw new TokenException<ModelTokenType>(token, string.Format("Invalid property value token ({0})", token.TokenType));
			}
		}
	}
}
