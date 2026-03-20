using System;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Serialization;
using JsonFx.Serialization.Filters;
using JsonFx.Serialization.GraphCycles;
using JsonFx.Serialization.Resolvers;

namespace JsonFx.Model
{
	public class ModelWalkerRS : IObjectWalker<ModelTokenType>
	{
		private readonly DataWriterSettingsRS Settings;

		private readonly IEnumerable<IDataFilterRS<ModelTokenType>> Filters;

		public ModelWalkerRS(DataWriterSettingsRS settings)
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
		}

		public IEnumerable<Token<ModelTokenType>> GetTokens(object value)
		{
			GraphCycleType graphCycles = Settings.GraphCycles;
			ICycleDetector detector = ((graphCycles != GraphCycleType.MaxDepth) ? ((ICycleDetector)new ReferenceSet()) : ((ICycleDetector)new DepthCounter(Settings.MaxDepth)));
			List<Token<ModelTokenType>> list = new List<Token<ModelTokenType>>();
			GetTokens(list, detector, value);
			return list;
		}

		private void GetTokens(List<Token<ModelTokenType>> tokens, ICycleDetector detector, object value)
		{
			if (value == null)
			{
				tokens.Add(ModelGrammar.TokenNull);
				return;
			}
			if (detector.Add(value))
			{
				switch (Settings.GraphCycles)
				{
				case GraphCycleType.Reference:
					throw new GraphCycleException(GraphCycleType.Reference, "Graph cycle detected: repeated references");
				case GraphCycleType.MaxDepth:
					throw new GraphCycleException(GraphCycleType.MaxDepth, "Graph cycle potentially detected: maximum depth exceeded");
				}
				tokens.Add(ModelGrammar.TokenNull);
				return;
			}
			try
			{
				foreach (IDataFilterRS<ModelTokenType> filter in Filters)
				{
					IEnumerable<Token<ModelTokenType>> tokens2;
					if (filter.TryWrite(Settings, value, out tokens2))
					{
						tokens.AddRange(tokens2);
						return;
					}
				}
				Type type = value.GetType();
				if (type.IsEnum)
				{
					tokens.Add(ModelGrammar.TokenPrimitive((Enum)value));
					return;
				}
				switch (Type.GetTypeCode(type))
				{
				case TypeCode.Boolean:
					tokens.Add((!true.Equals(value)) ? ModelGrammar.TokenFalse : ModelGrammar.TokenTrue);
					break;
				case TypeCode.SByte:
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Decimal:
					tokens.Add(ModelGrammar.TokenPrimitive((ValueType)value));
					break;
				case TypeCode.Double:
				{
					double num2 = (double)value;
					if (double.IsNaN(num2))
					{
						tokens.Add(ModelGrammar.TokenNaN);
					}
					else if (double.IsPositiveInfinity(num2))
					{
						tokens.Add(ModelGrammar.TokenPositiveInfinity);
					}
					else if (double.IsNegativeInfinity(num2))
					{
						tokens.Add(ModelGrammar.TokenNegativeInfinity);
					}
					else
					{
						tokens.Add(ModelGrammar.TokenPrimitive(num2));
					}
					break;
				}
				case TypeCode.Single:
				{
					float num = (float)value;
					if (float.IsNaN(num))
					{
						tokens.Add(ModelGrammar.TokenNaN);
					}
					else if (float.IsPositiveInfinity(num))
					{
						tokens.Add(ModelGrammar.TokenPositiveInfinity);
					}
					else if (float.IsNegativeInfinity(num))
					{
						tokens.Add(ModelGrammar.TokenNegativeInfinity);
					}
					else
					{
						tokens.Add(ModelGrammar.TokenPrimitive(num));
					}
					break;
				}
				case TypeCode.Char:
				case TypeCode.DateTime:
				case TypeCode.String:
					tokens.Add(ModelGrammar.TokenPrimitive(value));
					break;
				case TypeCode.Empty:
				case TypeCode.DBNull:
					tokens.Add(ModelGrammar.TokenNull);
					break;
				default:
					if (value is IEnumerable)
					{
						GetArrayTokens(tokens, detector, (IEnumerable)value);
					}
					else if (value is Guid || value is Uri || value is Version)
					{
						tokens.Add(ModelGrammar.TokenPrimitive(value));
					}
					else if (value is TimeSpan)
					{
						tokens.Add(ModelGrammar.TokenPrimitive((TimeSpan)value));
					}
					else
					{
						GetObjectTokens(tokens, detector, type, value);
					}
					break;
				}
			}
			finally
			{
				detector.Remove(value);
			}
		}

		private void GetArrayTokens(List<Token<ModelTokenType>> tokens, ICycleDetector detector, IEnumerable value)
		{
			DataName typeName = GetTypeName(value);
			IEnumerator enumerator = value.GetEnumerator();
			if (enumerator is IEnumerator<KeyValuePair<string, object>>)
			{
				GetObjectTokens(tokens, detector, typeName, (IEnumerator<KeyValuePair<string, object>>)enumerator);
				return;
			}
			if (enumerator is IDictionaryEnumerator)
			{
				GetObjectTokens(tokens, detector, typeName, (IDictionaryEnumerator)enumerator);
				return;
			}
			tokens.Add(ModelGrammar.TokenArrayBegin(typeName));
			while (enumerator.MoveNext())
			{
				GetTokens(tokens, detector, enumerator.Current);
			}
			tokens.Add(ModelGrammar.TokenArrayEnd);
		}

		private void GetObjectTokens(List<Token<ModelTokenType>> tokens, ICycleDetector detector, DataName typeName, IDictionaryEnumerator enumerator)
		{
			tokens.Add(ModelGrammar.TokenObjectBegin(typeName));
			while (enumerator.MoveNext())
			{
				tokens.Add(ModelGrammar.TokenProperty(enumerator.Key));
				GetTokens(tokens, detector, enumerator.Value);
			}
			tokens.Add(ModelGrammar.TokenObjectEnd);
		}

		private void GetObjectTokens(List<Token<ModelTokenType>> tokens, ICycleDetector detector, DataName typeName, IEnumerator<KeyValuePair<string, object>> enumerator)
		{
			tokens.Add(ModelGrammar.TokenObjectBegin(typeName));
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, object> current = enumerator.Current;
				tokens.Add(ModelGrammar.TokenProperty(current.Key));
				GetTokens(tokens, detector, current.Value);
			}
			tokens.Add(ModelGrammar.TokenObjectEnd);
		}

		private void GetObjectTokens(List<Token<ModelTokenType>> tokens, ICycleDetector detector, Type type, object value)
		{
			DataName typeName = GetTypeName(value);
			tokens.Add(ModelGrammar.TokenObjectBegin(typeName));
			IDictionary<string, MemberMapRS> dictionary = Settings.Resolver.LoadMaps(type);
			if (dictionary == null)
			{
				tokens.Add(ModelGrammar.TokenObjectEnd);
				return;
			}
			IEnumerable<MemberMapRS> enumerable = Settings.Resolver.SortMembers(dictionary.Values);
			foreach (MemberMapRS item in enumerable)
			{
				if (!item.IsAlternate && item.Getter != null)
				{
					object obj = item.Getter.Call(value);
					if (item.IsIgnored == null || !item.IsIgnored(value, obj))
					{
						tokens.Add(ModelGrammar.TokenProperty(item.DataName));
						GetTokens(tokens, detector, obj);
					}
				}
			}
			tokens.Add(ModelGrammar.TokenObjectEnd);
		}

		private DataName GetTypeName(object value)
		{
			IEnumerable<DataName> enumerable = Settings.Resolver.LoadTypeName((value == null) ? null : value.GetType());
			if (enumerable != null)
			{
				foreach (DataName item in enumerable)
				{
					if (!item.IsEmpty)
					{
						return item;
					}
				}
			}
			return DataName.Empty;
		}
	}
}
