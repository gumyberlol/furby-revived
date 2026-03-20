using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using JsonFx.IO;
using JsonFx.Markup;
using JsonFx.Model;
using JsonFx.Serialization;

namespace JsonFx.Xml
{
	public class XmlWriter : ModelWriter
	{
		public class XmlFormatter : ITextFormatter<MarkupTokenType>
		{
			private class XmlWriterAdapter : TextWriter
			{
				public System.Xml.XmlWriter Writer { get; private set; }

				public override Encoding Encoding
				{
					get
					{
						return Writer.Settings.Encoding;
					}
				}

				public XmlWriterAdapter(System.Xml.XmlWriter writer)
				{
					Writer = writer;
				}

				public override void Write(char value)
				{
					Writer.WriteRaw(char.ToString(value));
				}

				public override void Write(char[] buffer)
				{
					if (buffer != null)
					{
						Writer.WriteRaw(buffer, 0, buffer.Length);
					}
				}

				public override void Write(char[] buffer, int index, int count)
				{
					if (buffer != null)
					{
						Writer.WriteRaw(buffer, index, count);
					}
				}

				public override void Write(string value)
				{
					Writer.WriteRaw(value);
				}

				public override void Flush()
				{
					Writer.Flush();
				}
			}

			private const string ErrorUnexpectedToken = "Unexpected token ({0})";

			private readonly DataWriterSettings Settings;

			public XmlFormatter(DataWriterSettings settings)
			{
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				Settings = settings;
			}

			public string Format(IEnumerable<Token<MarkupTokenType>> tokens)
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					Format(tokens, stringWriter);
					return stringWriter.GetStringBuilder().ToString();
				}
			}

			public void Format(IEnumerable<Token<MarkupTokenType>> tokens, TextWriter writer)
			{
				if (writer == null)
				{
					throw new ArgumentNullException("writer");
				}
				XmlWriterAdapter xmlWriterAdapter = writer as XmlWriterAdapter;
				if (xmlWriterAdapter != null)
				{
					Format(xmlWriterAdapter.Writer, tokens);
					return;
				}
				using (System.Xml.XmlWriter writer2 = System.Xml.XmlWriter.Create(writer, new XmlWriterSettings
				{
					CheckCharacters = false,
					ConformanceLevel = ConformanceLevel.Auto,
					Encoding = Encoding.UTF8,
					Indent = Settings.PrettyPrint,
					IndentChars = Settings.Tab,
					NewLineChars = Settings.NewLine,
					NewLineHandling = NewLineHandling.None,
					OmitXmlDeclaration = true
				}))
				{
					Format(writer2, tokens);
				}
			}

			public void Format(System.Xml.XmlWriter writer, IEnumerable<Token<MarkupTokenType>> tokens)
			{
				if (writer == null)
				{
					throw new ArgumentNullException("writer");
				}
				if (tokens == null)
				{
					throw new ArgumentNullException("tokens");
				}
				int num = 0;
				IStream<Token<MarkupTokenType>> stream = Stream<Token<MarkupTokenType>>.Create(tokens);
				Token<MarkupTokenType> token = stream.Peek();
				try
				{
					while (!stream.IsCompleted)
					{
						switch (token.TokenType)
						{
						case MarkupTokenType.ElementBegin:
						{
							num++;
							DataName name4 = token.Name;
							string prefix2 = name4.Prefix;
							DataName name5 = token.Name;
							string localName2 = name5.LocalName;
							DataName name6 = token.Name;
							writer.WriteStartElement(prefix2, localName2, name6.NamespaceUri);
							stream.Pop();
							token = stream.Peek();
							break;
						}
						case MarkupTokenType.ElementVoid:
						{
							DataName name = token.Name;
							string prefix = name.Prefix;
							DataName name2 = token.Name;
							string localName = name2.LocalName;
							DataName name3 = token.Name;
							writer.WriteStartElement(prefix, localName, name3.NamespaceUri);
							stream.Pop();
							token = stream.Peek();
							while (!stream.IsCompleted && token.TokenType == MarkupTokenType.Attribute)
							{
								FormatAttribute(writer, stream);
								token = stream.Peek();
							}
							writer.WriteEndElement();
							break;
						}
						case MarkupTokenType.ElementEnd:
							num--;
							writer.WriteEndElement();
							stream.Pop();
							token = stream.Peek();
							break;
						case MarkupTokenType.Attribute:
							FormatAttribute(writer, stream);
							token = stream.Peek();
							break;
						case MarkupTokenType.Primitive:
						{
							ITextFormattable<MarkupTokenType> textFormattable = token.Value as ITextFormattable<MarkupTokenType>;
							if (textFormattable != null)
							{
								textFormattable.Format(this, new XmlWriterAdapter(writer));
							}
							else
							{
								writer.WriteString(token.ValueAsString());
							}
							stream.Pop();
							token = stream.Peek();
							break;
						}
						default:
							throw new TokenException<MarkupTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
						}
					}
					while (num-- > 0)
					{
						writer.WriteEndElement();
					}
				}
				catch (Exception ex)
				{
					throw new TokenException<MarkupTokenType>(token, ex.Message, ex);
				}
			}

			private void FormatAttribute(System.Xml.XmlWriter writer, IStream<Token<MarkupTokenType>> stream)
			{
				Token<MarkupTokenType> token = stream.Peek();
				DataName name = token.Name;
				string prefix = name.Prefix;
				DataName name2 = token.Name;
				string localName = name2.LocalName;
				DataName name3 = token.Name;
				writer.WriteStartAttribute(prefix, localName, name3.NamespaceUri);
				stream.Pop();
				token = stream.Peek();
				MarkupTokenType tokenType = token.TokenType;
				if (tokenType == MarkupTokenType.Primitive)
				{
					ITextFormattable<MarkupTokenType> textFormattable = token.Value as ITextFormattable<MarkupTokenType>;
					if (textFormattable != null)
					{
						textFormattable.Format(this, new XmlWriterAdapter(writer));
					}
					else
					{
						writer.WriteString(token.ValueAsString());
					}
					stream.Pop();
					writer.WriteEndAttribute();
					return;
				}
				throw new TokenException<MarkupTokenType>(token, string.Format("Unexpected token ({0})", token));
			}
		}

		public class XmlOutTransformer : IDataTransformer<ModelTokenType, MarkupTokenType>
		{
			private const string ErrorUnexpectedToken = "Unexpected token ({0})";

			private readonly PrefixScopeChain ScopeChain = new PrefixScopeChain();

			private readonly DataWriterSettings Settings;

			private int depth;

			private bool pendingNewLine;

			public XmlOutTransformer(DataWriterSettings settings)
			{
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				Settings = settings;
			}

			public IEnumerable<Token<MarkupTokenType>> Transform(IEnumerable<Token<ModelTokenType>> input)
			{
				if (input == null)
				{
					throw new ArgumentNullException("input");
				}
				IStream<Token<ModelTokenType>> stream = Stream<Token<ModelTokenType>>.Create(input);
				List<Token<MarkupTokenType>> list = new List<Token<MarkupTokenType>>();
				ScopeChain.Clear();
				while (!stream.IsCompleted)
				{
					TransformValue(list, stream, DataName.Empty);
				}
				return list;
			}

			private void TransformValue(List<Token<MarkupTokenType>> output, IStream<Token<ModelTokenType>> input, DataName propertyName)
			{
				if (pendingNewLine)
				{
					if (Settings.PrettyPrint)
					{
						depth++;
						EmitNewLine(output);
					}
					pendingNewLine = false;
				}
				Token<ModelTokenType> token = input.Peek();
				switch (token.TokenType)
				{
				case ModelTokenType.ArrayBegin:
					TransformArray(output, input, propertyName);
					break;
				case ModelTokenType.ObjectBegin:
					TransformObject(output, input, propertyName);
					break;
				case ModelTokenType.Primitive:
					input.Pop();
					if (propertyName.IsEmpty)
					{
						propertyName = token.Name;
					}
					if (token.Value == null)
					{
						propertyName = EncodeName(propertyName, null);
						EmitTag(output, propertyName, null, MarkupTokenType.ElementVoid);
						break;
					}
					propertyName = EncodeName(propertyName, token.Value.GetType());
					EmitTag(output, propertyName, null, MarkupTokenType.ElementBegin);
					output.Add(token.ChangeType(MarkupTokenType.Primitive));
					EmitTag(output, propertyName, null, MarkupTokenType.ElementEnd);
					break;
				default:
					throw new TokenException<ModelTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
				}
			}

			private void TransformArray(List<Token<MarkupTokenType>> output, IStream<Token<ModelTokenType>> input, DataName propertyName)
			{
				Token<ModelTokenType> token = input.Pop();
				propertyName = EncodeName((!propertyName.IsEmpty) ? propertyName : token.Name, typeof(Array));
				DataName propertyName2 = new DataName("item");
				EmitTag(output, propertyName, null, MarkupTokenType.ElementBegin);
				pendingNewLine = true;
				bool flag = false;
				while (!input.IsCompleted)
				{
					token = input.Peek();
					switch (token.TokenType)
					{
					case ModelTokenType.ArrayEnd:
						input.Pop();
						if (pendingNewLine)
						{
							pendingNewLine = false;
						}
						else if (Settings.PrettyPrint)
						{
							depth--;
							EmitNewLine(output);
						}
						EmitTag(output, propertyName, null, MarkupTokenType.ElementEnd);
						pendingNewLine = true;
						return;
					case ModelTokenType.ObjectBegin:
					case ModelTokenType.ArrayBegin:
					case ModelTokenType.Primitive:
						if (flag)
						{
							if (Settings.PrettyPrint)
							{
								EmitNewLine(output);
							}
							flag = false;
						}
						if (pendingNewLine)
						{
							if (Settings.PrettyPrint)
							{
								depth++;
								EmitNewLine(output);
							}
							pendingNewLine = false;
						}
						break;
					default:
						throw new TokenException<ModelTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
					}
					TransformValue(output, input, propertyName2);
					pendingNewLine = false;
					flag = true;
				}
			}

			private void TransformObject(List<Token<MarkupTokenType>> output, IStream<Token<ModelTokenType>> input, DataName propertyName)
			{
				Token<ModelTokenType> token = input.Pop();
				propertyName = EncodeName((!propertyName.IsEmpty) ? propertyName : token.Name, typeof(object));
				bool flag = true;
				IDictionary<DataName, Token<ModelTokenType>> dictionary = null;
				bool flag2 = false;
				while (!input.IsCompleted)
				{
					token = input.Peek();
					switch (token.TokenType)
					{
					case ModelTokenType.ObjectEnd:
						input.Pop();
						if (flag)
						{
							flag = false;
							EmitTag(output, propertyName, dictionary, MarkupTokenType.ElementBegin);
							pendingNewLine = true;
						}
						if (pendingNewLine)
						{
							pendingNewLine = false;
						}
						else if (Settings.PrettyPrint)
						{
							depth--;
							EmitNewLine(output);
						}
						EmitTag(output, propertyName, null, MarkupTokenType.ElementEnd);
						pendingNewLine = true;
						return;
					case ModelTokenType.Property:
						input.Pop();
						if (flag2)
						{
							if (Settings.PrettyPrint)
							{
								EmitNewLine(output);
							}
							flag2 = false;
						}
						if (flag)
						{
							DataName name = token.Name;
							if (name.IsAttribute)
							{
								if (dictionary == null)
								{
									dictionary = new SortedList<DataName, Token<ModelTokenType>>();
								}
								DataName name2 = token.Name;
								token = input.Peek();
								if (token.TokenType != ModelTokenType.Primitive)
								{
									throw new TokenException<ModelTokenType>(token, "Attribute values must be primitive input.");
								}
								input.Pop();
								if (name2.IsEmpty)
								{
									name2 = token.Name;
								}
								if (!dictionary.ContainsKey(name2))
								{
									dictionary.Add(name2, token);
								}
								pendingNewLine = false;
								flag2 = true;
								break;
							}
							flag = false;
							EmitTag(output, propertyName, dictionary, MarkupTokenType.ElementBegin);
							pendingNewLine = true;
						}
						if (pendingNewLine)
						{
							if (Settings.PrettyPrint)
							{
								depth++;
								EmitNewLine(output);
							}
							pendingNewLine = false;
						}
						TransformValue(output, input, token.Name);
						pendingNewLine = false;
						flag2 = true;
						break;
					default:
						throw new TokenException<ModelTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
					}
				}
			}

			private void EmitTag(List<Token<MarkupTokenType>> output, DataName elementName, IDictionary<DataName, Token<ModelTokenType>> attributes, MarkupTokenType tagType)
			{
				if (pendingNewLine)
				{
					if (Settings.PrettyPrint)
					{
						depth++;
						EmitNewLine(output);
					}
					pendingNewLine = false;
				}
				PrefixScopeChain.Scope scope = new PrefixScopeChain.Scope();
				scope.TagName = elementName;
				if (!ScopeChain.ContainsNamespace(elementName.NamespaceUri))
				{
					scope[elementName.Prefix] = elementName.NamespaceUri;
				}
				ScopeChain.Push(scope);
				switch (tagType)
				{
				case MarkupTokenType.ElementVoid:
					output.Add(MarkupGrammar.TokenElementVoid(elementName));
					break;
				case MarkupTokenType.ElementEnd:
					output.Add(MarkupGrammar.TokenElementEnd);
					break;
				default:
					output.Add(MarkupGrammar.TokenElementBegin(elementName));
					break;
				}
				if (attributes == null)
				{
					return;
				}
				foreach (KeyValuePair<DataName, Token<ModelTokenType>> attribute in attributes)
				{
					output.Add(MarkupGrammar.TokenAttribute(attribute.Key));
					output.Add(attribute.Value.ChangeType(MarkupTokenType.Primitive));
				}
				attributes.Clear();
			}

			private void EmitNewLine(List<Token<MarkupTokenType>> output)
			{
				bool flag = string.IsNullOrEmpty(Settings.Tab);
				if (flag && string.IsNullOrEmpty(Settings.NewLine))
				{
					return;
				}
				StringBuilder stringBuilder = new StringBuilder(Settings.NewLine);
				if (!flag)
				{
					if (Settings.Tab.Length == 1)
					{
						stringBuilder.Append(Settings.Tab[0], depth);
					}
					else
					{
						for (int i = 0; i < depth; i++)
						{
							stringBuilder.Append(Settings.Tab);
						}
					}
				}
				output.Add(MarkupGrammar.TokenPrimitive(stringBuilder.ToString()));
			}

			private DataName EncodeName(DataName name, Type type)
			{
				if (string.IsNullOrEmpty(name.LocalName))
				{
					foreach (DataName item in Settings.Resolver.LoadTypeName(type))
					{
						if (item.IsEmpty)
						{
							continue;
						}
						return item;
					}
				}
				string text = XmlConvert.EncodeLocalName(name.LocalName);
				if (name.LocalName == text)
				{
					return name;
				}
				return new DataName(text, name.Prefix, name.NamespaceUri, name.IsAttribute);
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

		public override IEnumerable<string> FileExtension
		{
			get
			{
				yield return ".xml";
			}
		}

		public XmlWriter()
			: base(new DataWriterSettings())
		{
		}

		public XmlWriter(DataWriterSettings settings)
			: base((settings == null) ? new DataWriterSettings() : settings)
		{
		}

		public XmlWriter(DataWriterSettings settings, params string[] contentTypes)
			: base((settings == null) ? new DataWriterSettings() : settings)
		{
			if (contentTypes == null)
			{
				throw new NullReferenceException("contentTypes");
			}
			ContentTypes = new string[contentTypes.Length];
			Array.Copy(contentTypes, ContentTypes, contentTypes.Length);
		}

		protected override ITextFormatter<ModelTokenType> GetFormatter()
		{
			return new TransformFormatter<ModelTokenType, MarkupTokenType>(new XmlFormatter(Settings), new XmlOutTransformer(Settings));
		}
	}
}
