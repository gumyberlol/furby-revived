using System;
using System.Collections;
using System.Collections.Generic;
using JsonFx.IO;
using JsonFx.Serialization;
using JsonFx.Serialization.Filters;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Model
{
	public class ModelAnalyzerRS : ITokenAnalyzerRS<ModelTokenType>
	{
		private const string ErrorUnexpectedToken = "Unexpected token ({0})";

		private const string ErrorExpectedArray = "Expected array start ({0})";

		private const string ErrorExpectedArrayItem = "Expected array item or end of array ({0})";

		private const string ErrorExpectedArrayItemDelim = "Expected array item delimiter or end of array ({0})";

		private const string ErrorMissingArrayItem = "Missing array item";

		private const string ErrorUnterminatedArray = "Unterminated array";

		private const string ErrorExpectedObject = "Expected object start ({0})";

		private const string ErrorExpectedPropertyName = "Expected object property name or end of object ({0})";

		private const string ErrorExpectedObjectValueDelim = "Expected value delimiter or end of object ({0})";

		private const string ErrorMissingObjectProperty = "Missing object property";

		private const string ErrorUnterminatedObject = "Unterminated object";

		private readonly DataReaderSettingsRS Settings;

		private readonly TypeCoercionUtilityRS Coercion;

		private readonly IEnumerable<IDataFilterRS<ModelTokenType>> Filters;

		DataReaderSettingsRS ITokenAnalyzerRS<ModelTokenType>.Settings
		{
			get
			{
				return Settings;
			}
		}

		public ModelAnalyzerRS(DataReaderSettingsRS settings)
		{
			if (settings == null)
			{
				throw new ArgumentNullException("settings");
			}
			Settings = settings;
			List<IDataFilterRS<ModelTokenType>> list = new List<IDataFilterRS<ModelTokenType>>();
			if (settings.Filters != null)
			{
				foreach (IDataFilterRS<ModelTokenType> filter in settings.Filters)
				{
					if (filter != null)
					{
						list.Add(filter);
					}
				}
			}
			Filters = list;
			Coercion = new TypeCoercionUtilityRS(settings, settings.AllowNullValueTypes);
		}

		public IEnumerable Analyze(IEnumerable<Token<ModelTokenType>> tokens)
		{
			return Analyze(tokens, null);
		}

		public IEnumerable Analyze(IEnumerable<Token<ModelTokenType>> tokens, Type targetType)
		{
			if (tokens == null)
			{
				throw new ArgumentNullException("tokens");
			}
			IStream<Token<ModelTokenType>> stream = Stream<Token<ModelTokenType>>.Create(tokens);
			while (!stream.IsCompleted)
			{
				yield return ConsumeValue(stream, targetType);
			}
		}

		public IEnumerable<TResult> Analyze<TResult>(IEnumerable<Token<ModelTokenType>> tokens)
		{
			if (tokens == null)
			{
				throw new ArgumentNullException("tokens");
			}
			Type resultType = typeof(TResult);
			IStream<Token<ModelTokenType>> stream = Stream<Token<ModelTokenType>>.Create(tokens);
			while (!stream.IsCompleted)
			{
				yield return (TResult)ConsumeValue(stream, resultType);
			}
		}

		public IEnumerable<TResult> Analyze<TResult>(IEnumerable<Token<ModelTokenType>> tokens, TResult ignored)
		{
			return Analyze<TResult>(tokens);
		}

		private object ConsumeValue(IStream<Token<ModelTokenType>> tokens, Type targetType)
		{
			object result;
			if (TryReadFilters(tokens, out result))
			{
				return result;
			}
			Token<ModelTokenType> token = tokens.Peek();
			switch (token.TokenType)
			{
			case ModelTokenType.ArrayBegin:
				return ConsumeArray(tokens, targetType);
			case ModelTokenType.ObjectBegin:
				return ConsumeObject(tokens, targetType);
			case ModelTokenType.Primitive:
				tokens.Pop();
				return Coercion.CoerceType(targetType, token.Value);
			default:
				tokens.Pop();
				throw new TokenException<ModelTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
			}
		}

		private object ConsumeObject(IStream<Token<ModelTokenType>> tokens, Type targetType)
		{
			Token<ModelTokenType> token = tokens.Pop();
			if (token.TokenType != ModelTokenType.ObjectBegin)
			{
				throw new TokenException<ModelTokenType>(token, string.Format("Expected object start ({0})", token.TokenType));
			}
			IDictionary<string, MemberMapRS> dictionary = Settings.Resolver.LoadMaps(targetType);
			Type dictionaryItemType = TypeCoercionUtility.GetDictionaryItemType(targetType);
			object obj = Coercion.InstantiateObjectDefaultCtor(targetType);
			while (!tokens.IsCompleted)
			{
				token = tokens.Peek();
				string localName;
				MemberMapRS value;
				Type targetType2;
				switch (token.TokenType)
				{
				case ModelTokenType.Property:
				{
					tokens.Pop();
					DataName name = token.Name;
					localName = name.LocalName;
					if (dictionaryItemType != null)
					{
						value = null;
						targetType2 = dictionaryItemType;
					}
					else if (dictionary != null && dictionary.TryGetValue(localName, out value))
					{
						targetType2 = ((value == null) ? null : value.Type);
					}
					else
					{
						value = null;
						targetType2 = null;
					}
					break;
				}
				case ModelTokenType.ObjectEnd:
					tokens.Pop();
					return Coercion.CoerceType(targetType, obj);
				default:
					tokens.Pop();
					throw new TokenException<ModelTokenType>(token, string.Format("Expected object property name or end of object ({0})", token.TokenType));
				}
				object memberValue = ConsumeValue(tokens, targetType2);
				Coercion.SetMemberValue(obj, targetType, value, localName, memberValue);
			}
			throw new TokenException<ModelTokenType>(ModelGrammar.TokenNone, "Unterminated object");
		}

		private object ConsumeArray(IStream<Token<ModelTokenType>> tokens, Type arrayType)
		{
			Token<ModelTokenType> token = tokens.Pop();
			if (token.TokenType != ModelTokenType.ArrayBegin)
			{
				throw new TokenException<ModelTokenType>(token, string.Format("Expected array start ({0})", token.TokenType));
			}
			Type type = TypeCoercionUtility.GetElementType(arrayType);
			bool flag = type == null;
			IList list = new List<object>();
			while (!tokens.IsCompleted)
			{
				token = tokens.Peek();
				object result;
				switch (token.TokenType)
				{
				case ModelTokenType.ArrayBegin:
					result = ConsumeArray(tokens, (!flag) ? type : null);
					break;
				case ModelTokenType.ArrayEnd:
					tokens.Pop();
					return Coercion.CoerceCollection(arrayType, type, list);
				case ModelTokenType.ObjectBegin:
					result = ConsumeObject(tokens, (!flag) ? type : null);
					break;
				case ModelTokenType.Primitive:
					if (!TryReadFilters(tokens, out result))
					{
						token = tokens.Pop();
						result = ((!(token != null)) ? null : token.Value);
					}
					if (!flag)
					{
						result = Coercion.CoerceType(type, result);
					}
					break;
				default:
					tokens.Pop();
					throw new TokenException<ModelTokenType>(token, string.Format("Expected array item or end of array ({0})", token.TokenType));
				}
				type = TypeCoercionUtility.FindCommonType(type, result);
				list.Add(result);
			}
			throw new TokenException<ModelTokenType>(ModelGrammar.TokenNone, "Unterminated array");
		}

		private bool TryReadFilters(IStream<Token<ModelTokenType>> tokens, out object result)
		{
			if (!tokens.IsCompleted)
			{
				foreach (IDataFilterRS<ModelTokenType> filter in Filters)
				{
					if (filter.TryRead(Settings, tokens, out result))
					{
						return true;
					}
				}
			}
			result = null;
			return false;
		}
	}
}
