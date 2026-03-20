using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using JsonFx.IO;
using JsonFx.Markup;
using JsonFx.Model;
using JsonFx.Serialization;
using JsonFx.Utils;

namespace JsonFx.Xml
{
	public class XmlReader : ModelReader
	{
		public class XmlInTransformer : IDataTransformer<MarkupTokenType, ModelTokenType>
		{
			private const string ErrorUnexpectedToken = "Unexpected token ({0})";

			private const string ErrorUnterminatedObject = "Unterminated object";

			private const string ErrorInvalidAttribute = "Invalid attribute value token ({0})";

			private static DataName DefaultArrayName = new DataName(typeof(Array));

			private static DataName DefaultItemName = new DataName("item");

			private readonly PrefixScopeChain ScopeChain = new PrefixScopeChain();

			private readonly DataReaderSettings Settings;

			public XmlInTransformer(DataReaderSettings settings)
			{
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				Settings = settings;
			}

			public IEnumerable<Token<ModelTokenType>> Transform(IEnumerable<Token<MarkupTokenType>> input)
			{
				if (input == null)
				{
					throw new ArgumentNullException("input");
				}
				ScopeChain.Clear();
				IStream<Token<MarkupTokenType>> stream = Stream<Token<MarkupTokenType>>.Create(input);
				while (!stream.IsCompleted)
				{
					foreach (Token<ModelTokenType> item in TransformValue(stream, true))
					{
						yield return item;
					}
				}
			}

			private IList<Token<ModelTokenType>> TransformValue(IStream<Token<MarkupTokenType>> input, bool isStandAlone)
			{
				Token<MarkupTokenType> token = input.Peek();
				switch (token.TokenType)
				{
				case MarkupTokenType.Primitive:
					input.Pop();
					return new Token<ModelTokenType>[1] { token.ChangeType(ModelTokenType.Primitive) };
				case MarkupTokenType.ElementBegin:
				case MarkupTokenType.ElementVoid:
					return TransformElement(input, isStandAlone);
				default:
					throw new TokenException<MarkupTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
				}
			}

			private IList<Token<ModelTokenType>> TransformElement(IStream<Token<MarkupTokenType>> input, bool isStandAlone)
			{
				Token<MarkupTokenType> token = input.Peek();
				DataName dataName = DecodeName(token.Name, typeof(object));
				bool flag = token.TokenType == MarkupTokenType.ElementVoid;
				input.Pop();
				IDictionary<DataName, IList<IList<Token<ModelTokenType>>>> dictionary = null;
				while (!input.IsCompleted)
				{
					token = input.Peek();
					if (token.TokenType == MarkupTokenType.ElementEnd || (flag && token.TokenType != MarkupTokenType.Attribute))
					{
						if (!flag)
						{
							input.Pop();
						}
						List<Token<ModelTokenType>> list = new List<Token<ModelTokenType>>();
						if (dictionary == null || dictionary.Count <= 1 || dataName == DefaultArrayName)
						{
							DataName dataName2 = DataName.Empty;
							IList<IList<Token<ModelTokenType>>> list2 = null;
							if (dictionary != null)
							{
								using (IEnumerator<KeyValuePair<DataName, IList<IList<Token<ModelTokenType>>>>> enumerator = dictionary.GetEnumerator())
								{
									if (enumerator.MoveNext())
									{
										list2 = enumerator.Current.Value;
										dataName2 = enumerator.Current.Key;
									}
								}
							}
							if ((list2 != null && list2.Count > 1) || (list2 == null && dataName == DefaultArrayName) || dataName2 == DefaultItemName)
							{
								list.Add((!dataName.IsEmpty) ? ModelGrammar.TokenArrayBegin(DecodeName(dataName, typeof(Array))) : ModelGrammar.TokenArrayBeginUnnamed);
								if (list2 != null)
								{
									foreach (IList<Token<ModelTokenType>> item in list2)
									{
										list.AddRange(item);
									}
								}
								list.Add(ModelGrammar.TokenArrayEnd);
								return list;
							}
						}
						if (isStandAlone)
						{
							list.Add((!dataName.IsEmpty) ? ModelGrammar.TokenObjectBegin(dataName) : ModelGrammar.TokenObjectBeginUnnamed);
						}
						if (dictionary != null)
						{
							foreach (KeyValuePair<DataName, IList<IList<Token<ModelTokenType>>>> item2 in dictionary)
							{
								if (item2.Value.Count == 1)
								{
									if (isStandAlone)
									{
										DataName name = DecodeName(item2.Key, typeof(object));
										list.Add((!name.IsEmpty) ? ModelGrammar.TokenProperty(name) : ModelGrammar.TokenProperty(dataName));
									}
									list.AddRange(item2.Value[0]);
								}
								else
								{
									if (item2.Key.IsEmpty)
									{
										continue;
									}
									list.Add((!item2.Key.IsEmpty) ? ModelGrammar.TokenArrayBegin(DecodeName(item2.Key, typeof(Array))) : ModelGrammar.TokenArrayBeginUnnamed);
									foreach (IList<Token<ModelTokenType>> item3 in item2.Value)
									{
										list.AddRange(item3);
									}
									list.Add(ModelGrammar.TokenArrayEnd);
								}
							}
						}
						else if (!isStandAlone)
						{
							list.Add(ModelGrammar.TokenNull);
						}
						if (isStandAlone)
						{
							list.Add(ModelGrammar.TokenObjectEnd);
						}
						return list;
					}
					DataName name2 = token.Name;
					if (token.TokenType == MarkupTokenType.Attribute)
					{
						input.Pop();
					}
					if (dictionary == null)
					{
						dictionary = new Dictionary<DataName, IList<IList<Token<ModelTokenType>>>>();
					}
					if (!dictionary.ContainsKey(name2))
					{
						dictionary[name2] = new List<IList<Token<ModelTokenType>>>();
					}
					IList<Token<ModelTokenType>> list3 = TransformValue(input, !isStandAlone);
					if (list3.Count != 1 || list3[0].TokenType != ModelTokenType.Primitive || list3[0].Value == null || !CharUtility.IsNullOrWhiteSpace(list3[0].ValueAsString()))
					{
						dictionary[name2].Add(list3);
					}
				}
				throw new TokenException<MarkupTokenType>(token, "Unterminated object");
			}

			private DataName DecodeName(DataName name, Type type)
			{
				IEnumerable<DataName> enumerable = Settings.Resolver.LoadTypeName(type);
				if (enumerable != null)
				{
					foreach (DataName item in enumerable)
					{
						if (name == item)
						{
							return DataName.Empty;
						}
					}
				}
				string text = XmlConvert.DecodeName(name.LocalName);
				if (name.LocalName == text)
				{
					return name;
				}
				return new DataName(text, name.Prefix, name.NamespaceUri, name.IsAttribute);
			}
		}

		public class XmlTokenizer : IDisposable, ITextTokenizer<MarkupTokenType>
		{
			private readonly XmlReaderSettings Settings;

			public int Column
			{
				get
				{
					return 0;
				}
			}

			public int Line
			{
				get
				{
					return 0;
				}
			}

			public long Index
			{
				get
				{
					return -1L;
				}
			}

			public XmlTokenizer()
				: this(null)
			{
			}

			public XmlTokenizer(XmlReaderSettings settings)
			{
				if (settings == null)
				{
					Settings = new XmlReaderSettings
					{
						CheckCharacters = false,
						ConformanceLevel = ConformanceLevel.Auto
					};
				}
				else
				{
					Settings = settings;
				}
			}

			public IEnumerable<Token<MarkupTokenType>> GetTokens(string text)
			{
				return GetTokens(new StringReader(text ?? string.Empty));
			}

			public IEnumerable<Token<MarkupTokenType>> GetTokens(TextReader reader)
			{
				if (reader == null)
				{
					throw new ArgumentNullException("reader");
				}
				System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(reader, Settings);
				XmlTextReader xmlTextReader = xmlReader as XmlTextReader;
				if (xmlTextReader != null)
				{
					xmlTextReader.Normalization = false;
					xmlTextReader.WhitespaceHandling = WhitespaceHandling.All;
				}
				return GetTokens(xmlReader);
			}

			public IEnumerable<Token<MarkupTokenType>> GetTokens(System.Xml.XmlReader reader)
			{
				if (reader == null)
				{
					throw new ArgumentNullException("reader");
				}
				while (true)
				{
					try
					{
						if (!reader.Read())
						{
							((IDisposable)reader).Dispose();
							break;
						}
					}
					catch (XmlException ex)
					{
						XmlException ex2 = ex;
						throw new DeserializationException(ex2.Message, ex2.LinePosition, ex2.LineNumber, -1, ex2);
					}
					catch (Exception ex3)
					{
						Exception ex4 = ex3;
						throw new DeserializationException(ex4.Message, -1L, ex4);
					}
					switch (reader.NodeType)
					{
					case XmlNodeType.Element:
					{
						DataName tagName = new DataName(reader.LocalName, reader.Prefix, reader.NamespaceURI);
						bool isVoidTag = reader.IsEmptyElement;
						IDictionary<DataName, string> attributes;
						if (reader.HasAttributes)
						{
							attributes = new SortedList<DataName, string>();
							while (reader.MoveToNextAttribute())
							{
								if ((!string.IsNullOrEmpty(reader.Prefix) || !(reader.LocalName == "xmlns")) && !(reader.Prefix == "xmlns"))
								{
									attributes[new DataName(reader.LocalName, reader.Prefix, reader.NamespaceURI)] = reader.Value;
								}
							}
						}
						else
						{
							attributes = null;
						}
						if (isVoidTag)
						{
							yield return MarkupGrammar.TokenElementVoid(tagName);
						}
						else
						{
							yield return MarkupGrammar.TokenElementBegin(tagName);
						}
						if (attributes == null)
						{
							break;
						}
						foreach (KeyValuePair<DataName, string> attribute in attributes)
						{
							yield return MarkupGrammar.TokenAttribute(attribute.Key);
							yield return MarkupGrammar.TokenPrimitive(attribute.Value);
						}
						break;
					}
					case XmlNodeType.EndElement:
						yield return MarkupGrammar.TokenElementEnd;
						break;
					case XmlNodeType.Attribute:
						yield return MarkupGrammar.TokenAttribute(new DataName(reader.Name, reader.Prefix, reader.NamespaceURI, true));
						yield return MarkupGrammar.TokenPrimitive(reader.Value);
						break;
					case XmlNodeType.Text:
						yield return MarkupGrammar.TokenPrimitive(reader.Value);
						break;
					case XmlNodeType.Whitespace:
					case XmlNodeType.SignificantWhitespace:
						yield return MarkupGrammar.TokenPrimitive(reader.Value);
						break;
					case XmlNodeType.CDATA:
						yield return MarkupGrammar.TokenPrimitive(reader.Value);
						break;
					case XmlNodeType.ProcessingInstruction:
					case XmlNodeType.XmlDeclaration:
						yield return MarkupGrammar.TokenUnparsed("?", "?", reader.Name + " " + reader.Value);
						break;
					case XmlNodeType.Comment:
						yield return MarkupGrammar.TokenUnparsed("!--", "--", reader.Value);
						break;
					case XmlNodeType.DocumentType:
						yield return MarkupGrammar.TokenUnparsed("!DOCTYPE ", string.Empty, reader.Value);
						break;
					case XmlNodeType.Notation:
						yield return MarkupGrammar.TokenUnparsed("!NOTATION ", string.Empty, reader.Value);
						break;
					case XmlNodeType.None:
						((IDisposable)reader).Dispose();
						yield break;
					}
				}
			}

			public void Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}

			protected virtual void Dispose(bool disposing)
			{
				if (!disposing)
				{
				}
			}
		}

		private readonly string[] ContentTypes;

		public override IEnumerable<string> ContentType
		{
			get
			{
				if (ContentTypes != null)
				{
					string[] contentTypes = ContentTypes;
					for (int i = 0; i < contentTypes.Length; i++)
					{
						yield return contentTypes[i];
					}
				}
				else
				{
					yield return "application/xml";
					yield return "text/xml";
				}
			}
		}

		public XmlReader()
			: this(new DataReaderSettings())
		{
		}

		public XmlReader(DataReaderSettings settings)
			: base((settings == null) ? new DataReaderSettings() : settings)
		{
		}

		public XmlReader(DataReaderSettings settings, params string[] contentTypes)
			: base((settings == null) ? new DataReaderSettings() : settings)
		{
			if (contentTypes == null)
			{
				throw new NullReferenceException("contentTypes");
			}
			ContentTypes = new string[contentTypes.Length];
			Array.Copy(contentTypes, ContentTypes, contentTypes.Length);
		}

		protected override ITextTokenizer<ModelTokenType> GetTokenizer()
		{
			return new TransformTokenizer<MarkupTokenType, ModelTokenType>(new XmlTokenizer(), new XmlInTransformer(Settings));
		}
	}
}
