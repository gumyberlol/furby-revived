using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using JsonFx.IO;
using JsonFx.Markup;
using JsonFx.Serialization;
using JsonFx.Utils;

namespace JsonFx.Html
{
	public class HtmlTokenizer : IDisposable, ITextTokenizer<MarkupTokenType>
	{
		private class QName
		{
			private static readonly char[] NameDelim = new char[1] { ':' };

			public readonly string Prefix;

			public readonly string Name;

			public QName(string name)
			{
				if (string.IsNullOrEmpty(name))
				{
					throw new ArgumentNullException("name");
				}
				string[] array = name.Split(NameDelim, StringSplitOptions.RemoveEmptyEntries);
				switch (array.Length)
				{
				case 1:
					Prefix = string.Empty;
					Name = array[0];
					break;
				case 2:
					Prefix = array[0];
					Name = array[1];
					break;
				default:
					throw new ArgumentException("name");
				}
			}

			public override string ToString()
			{
				if (string.IsNullOrEmpty(Prefix))
				{
					return Name;
				}
				return Prefix + ':' + Name;
			}

			public override int GetHashCode()
			{
				int num = 908675878;
				num = -1521134295 * num + StringComparer.Ordinal.GetHashCode(Name ?? string.Empty);
				return -1521134295 * num + StringComparer.Ordinal.GetHashCode(Prefix ?? string.Empty);
			}

			public override bool Equals(object obj)
			{
				return Equals(obj as QName, null);
			}

			public bool Equals(QName that)
			{
				return Equals(that, null);
			}

			public bool Equals(QName that, IEqualityComparer<string> comparer)
			{
				if (comparer == null)
				{
					comparer = StringComparer.Ordinal;
				}
				if (that == null)
				{
					return false;
				}
				return comparer.Equals(Prefix, that.Prefix) && comparer.Equals(Name, that.Name);
			}

			public static bool operator ==(QName a, QName b)
			{
				if (object.ReferenceEquals(a, null))
				{
					return object.ReferenceEquals(b, null);
				}
				return a.Equals(b);
			}

			public static bool operator !=(QName a, QName b)
			{
				return !(a == b);
			}
		}

		private class Attrib
		{
			public QName QName { get; set; }

			public Token<MarkupTokenType> Value { get; set; }

			public override string ToString()
			{
				return string.Concat(QName, '=', '"', (!(Value != null)) ? string.Empty : Value.ValueAsString(), '"');
			}
		}

		private const int DefaultBufferSize = 32;

		private readonly PrefixScopeChain ScopeChain = new PrefixScopeChain();

		private ITextStream Scanner = TextReaderStream.Null;

		private IList<QName> unparsedTags;

		private bool autoBalanceTags;

		public bool AutoBalanceTags
		{
			get
			{
				return autoBalanceTags;
			}
			set
			{
				autoBalanceTags = value;
			}
		}

		public bool UnwrapUnparsedComments { get; set; }

		public IEnumerable<string> UnparsedTags
		{
			get
			{
				if (unparsedTags == null)
				{
					yield return null;
				}
				foreach (QName name in unparsedTags)
				{
					yield return name.ToString();
				}
			}
			set
			{
				if (value == null)
				{
					unparsedTags = null;
					return;
				}
				unparsedTags = new List<QName>();
				foreach (string item in value)
				{
					unparsedTags.Add(new QName(item));
				}
				if (unparsedTags.Count < 1)
				{
					unparsedTags = null;
				}
			}
		}

		public int Column
		{
			get
			{
				return Scanner.Column;
			}
		}

		public int Line
		{
			get
			{
				return Scanner.Line;
			}
		}

		public long Index
		{
			get
			{
				return Scanner.Index;
			}
		}

		private void GetTokens(List<Token<MarkupTokenType>> tokens, ITextStream scanner)
		{
			ScopeChain.Clear();
			try
			{
				QName qName = null;
				scanner.BeginChunk();
				while (!scanner.IsCompleted)
				{
					switch (scanner.Peek())
					{
					case '<':
					{
						EmitText(tokens, scanner.EndChunk());
						QName qName2 = ScanTag(tokens, scanner, qName);
						if (qName == null)
						{
							if (qName2 != null)
							{
								qName = qName2;
							}
						}
						else if (qName2 == qName)
						{
							qName = null;
						}
						scanner.BeginChunk();
						break;
					}
					case '&':
						EmitText(tokens, scanner.EndChunk());
						EmitText(tokens, DecodeEntity(scanner));
						scanner.BeginChunk();
						break;
					default:
						scanner.Pop();
						break;
					}
				}
				EmitText(tokens, scanner.EndChunk());
				if (ScopeChain.HasScope && autoBalanceTags)
				{
					while (ScopeChain.HasScope)
					{
						ScopeChain.Pop();
						tokens.Add(MarkupGrammar.TokenElementEnd);
					}
				}
			}
			catch (DeserializationException)
			{
				throw;
			}
			catch (Exception ex2)
			{
				throw new DeserializationException(ex2.Message, scanner.Index, scanner.Line, scanner.Column, ex2);
			}
		}

		private QName ScanTag(List<Token<MarkupTokenType>> tokens, ITextStream scanner, QName unparsedName)
		{
			if (scanner.Pop() != '<')
			{
				throw new DeserializationException("Invalid tag start char", scanner.Index, scanner.Line, scanner.Column);
			}
			if (scanner.IsCompleted)
			{
				EmitText(tokens, char.ToString('<'));
				return null;
			}
			Token<MarkupTokenType> token = ScanUnparsedBlock(scanner);
			if (token != null)
			{
				if (UnwrapUnparsedComments && unparsedName != null)
				{
					UnparsedBlock unparsedBlock = token.Value as UnparsedBlock;
					if (unparsedBlock != null && unparsedBlock.Begin == "!--")
					{
						token = new Token<MarkupTokenType>(token.TokenType, token.Name, unparsedBlock.Value);
					}
				}
				tokens.Add(token);
				return null;
			}
			char c = scanner.Peek();
			MarkupTokenType tagType = MarkupTokenType.ElementBegin;
			if (c == '/')
			{
				tagType = MarkupTokenType.ElementEnd;
				scanner.Pop();
				c = scanner.Peek();
			}
			QName qName = ScanQName(scanner);
			if (qName == null)
			{
				string text = char.ToString('<');
				if (tagType == MarkupTokenType.ElementEnd)
				{
					text += '/';
				}
				EmitText(tokens, text);
				return null;
			}
			if (unparsedName != null && (qName != unparsedName || tagType != MarkupTokenType.ElementEnd))
			{
				string text2 = char.ToString('<');
				if (tagType == MarkupTokenType.ElementEnd)
				{
					text2 += '/';
				}
				text2 += qName.ToString();
				EmitText(tokens, text2);
				return null;
			}
			List<Attrib> list = null;
			while (!IsTagComplete(scanner, ref tagType))
			{
				Attrib attrib = new Attrib();
				attrib.QName = ScanQName(scanner);
				attrib.Value = ScanAttributeValue(scanner);
				Attrib attrib2 = attrib;
				if (attrib2.QName == null)
				{
					throw new DeserializationException("Malformed attribute name", scanner.Index, scanner.Line, scanner.Column);
				}
				if (list == null)
				{
					list = new List<Attrib>();
				}
				list.Add(attrib2);
			}
			EmitTag(tokens, tagType, qName, list);
			return (unparsedTags == null || !unparsedTags.Contains(qName)) ? null : qName;
		}

		private Token<MarkupTokenType> ScanUnparsedBlock(ITextStream scanner)
		{
			switch (scanner.Peek())
			{
			case '!':
				scanner.Pop();
				switch (scanner.Peek())
				{
				case '-':
				{
					string text2 = ScanUnparsedValue(scanner, "--", "--");
					if (text2 != null)
					{
						return MarkupGrammar.TokenUnparsed("!--", "--", text2);
					}
					break;
				}
				case '[':
				{
					string text = ScanUnparsedValue(scanner, "[CDATA[", "]]");
					if (text != null)
					{
						return MarkupGrammar.TokenPrimitive(text);
					}
					break;
				}
				}
				return MarkupGrammar.TokenUnparsed("!", string.Empty, ScanUnparsedValue(scanner, string.Empty, string.Empty));
			case '?':
			{
				scanner.Pop();
				char c2 = scanner.Peek();
				if (c2 == '=')
				{
					return MarkupGrammar.TokenUnparsed("?=", "?", ScanUnparsedValue(scanner, "?=", "?"));
				}
				return MarkupGrammar.TokenUnparsed("?", "?", ScanUnparsedValue(scanner, string.Empty, "?"));
			}
			case '%':
			{
				scanner.Pop();
				char c = scanner.Peek();
				switch (c)
				{
				case '-':
					return MarkupGrammar.TokenUnparsed("%--", "--%", ScanUnparsedValue(scanner, "--", "--" + '%'));
				case '!':
				case '#':
				case '$':
				case ':':
				case '=':
				case '@':
					scanner.Pop();
					return MarkupGrammar.TokenUnparsed(string.Concat('%', c), "%", ScanUnparsedValue(scanner, string.Empty, "%"));
				default:
					return MarkupGrammar.TokenUnparsed("%", "%", ScanUnparsedValue(scanner, string.Empty, "%"));
				}
			}
			case '#':
			{
				scanner.Pop();
				char c = scanner.Peek();
				switch (c)
				{
				case '-':
					return MarkupGrammar.TokenUnparsed("#--", "--#", ScanUnparsedValue(scanner, "--", "--" + '#'));
				case '+':
				case '=':
				case '@':
					scanner.Pop();
					return MarkupGrammar.TokenUnparsed(string.Concat('#', c), "#", ScanUnparsedValue(scanner, string.Empty, "#"));
				default:
					return MarkupGrammar.TokenUnparsed("#", "#", ScanUnparsedValue(scanner, string.Empty, "#"));
				}
			}
			default:
				return null;
			}
		}

		private string ScanUnparsedValue(ITextStream scanner, string begin, string end)
		{
			char c = scanner.Peek();
			int length = begin.Length;
			for (int i = 0; i < length; i++)
			{
				if (!scanner.IsCompleted && c == begin[i])
				{
					scanner.Pop();
					c = scanner.Peek();
					continue;
				}
				if (i == 0)
				{
					return null;
				}
				throw new DeserializationException("Unrecognized unparsed tag", scanner.Index, scanner.Line, scanner.Column);
			}
			end += '>';
			scanner.BeginChunk();
			int length2 = end.Length;
			int num = 0;
			while (!scanner.IsCompleted)
			{
				if (c == end[num])
				{
					num++;
					if (num >= length2)
					{
						string text = scanner.EndChunk();
						scanner.Pop();
						length2--;
						if (length2 > 0)
						{
							text = text.Remove(text.Length - length2);
						}
						return text;
					}
				}
				else
				{
					num = 0;
				}
				scanner.Pop();
				c = scanner.Peek();
			}
			throw new DeserializationException("Unexpected end of file", scanner.Index, scanner.Line, scanner.Column);
		}

		private Token<MarkupTokenType> ScanAttributeValue(ITextStream scanner)
		{
			SkipWhitespace(scanner);
			if (scanner.Peek() != '=')
			{
				return MarkupGrammar.TokenPrimitive(string.Empty);
			}
			scanner.Pop();
			SkipWhitespace(scanner);
			char c = scanner.Peek();
			if (c == '"' || c == '\'')
			{
				scanner.Pop();
				char c2 = scanner.Peek();
				scanner.BeginChunk();
				if (c2 == '<')
				{
					scanner.Pop();
					Token<MarkupTokenType> token = ScanUnparsedBlock(scanner);
					if (token != null)
					{
						c2 = scanner.Peek();
						while (!scanner.IsCompleted && !CharUtility.IsWhiteSpace(c2) && c2 != c)
						{
							scanner.Pop();
							c2 = scanner.Peek();
						}
						if (scanner.IsCompleted || c2 != c)
						{
							throw new DeserializationException("Missing attribute value closing delimiter", scanner.Index, scanner.Line, scanner.Column);
						}
						scanner.Pop();
						return token;
					}
				}
				while (!scanner.IsCompleted && c2 != c)
				{
					scanner.Pop();
					c2 = scanner.Peek();
				}
				if (scanner.IsCompleted)
				{
					throw new DeserializationException("Unexpected end of file", scanner.Index, scanner.Line, scanner.Column);
				}
				string value = scanner.EndChunk();
				scanner.Pop();
				return MarkupGrammar.TokenPrimitive(value);
			}
			scanner.BeginChunk();
			if (c == '<')
			{
				scanner.Pop();
				Token<MarkupTokenType> token2 = ScanUnparsedBlock(scanner);
				if (token2 != null)
				{
					return token2;
				}
			}
			char c3 = scanner.Peek();
			while (!scanner.IsCompleted && c3 != '/' && c3 != '>' && !CharUtility.IsWhiteSpace(c3))
			{
				scanner.Pop();
				c3 = scanner.Peek();
			}
			if (scanner.IsCompleted)
			{
				throw new DeserializationException("Unexpected end of file", scanner.Index, scanner.Line, scanner.Column);
			}
			return MarkupGrammar.TokenPrimitive(scanner.EndChunk());
		}

		private static QName ScanQName(ITextStream scanner)
		{
			char ch = scanner.Peek();
			if (!IsNameStartChar(ch))
			{
				return null;
			}
			scanner.BeginChunk();
			do
			{
				scanner.Pop();
				ch = scanner.Peek();
			}
			while (!scanner.IsCompleted && IsNameChar(ch));
			string text = scanner.EndChunk();
			try
			{
				return new QName(text);
			}
			catch (Exception innerException)
			{
				throw new DeserializationException(string.Format("Invalid element name ({0})", text), scanner.Index, scanner.Line, scanner.Column, innerException);
			}
		}

		private bool IsTagComplete(ITextStream scanner, ref MarkupTokenType tagType)
		{
			if (scanner.IsCompleted)
			{
				throw new DeserializationException("Unexpected end of file", scanner.Index, scanner.Line, scanner.Column);
			}
			SkipWhitespace(scanner);
			switch (scanner.Peek())
			{
			case '/':
				scanner.Pop();
				if (scanner.Peek() == '>')
				{
					if (tagType != MarkupTokenType.ElementBegin)
					{
						throw new DeserializationException("Malformed element tag", scanner.Index, scanner.Line, scanner.Column);
					}
					scanner.Pop();
					tagType = MarkupTokenType.ElementVoid;
					return true;
				}
				throw new DeserializationException("Malformed element tag", scanner.Index, scanner.Line, scanner.Column);
			case '>':
				scanner.Pop();
				return true;
			default:
				return false;
			}
		}

		private void EmitTag(List<Token<MarkupTokenType>> tokens, MarkupTokenType tagType, QName qName, List<Attrib> attributes)
		{
			PrefixScopeChain.Scope scope;
			if (tagType == MarkupTokenType.ElementEnd)
			{
				DataName dataName = new DataName(qName.Name, qName.Prefix, ScopeChain.GetNamespace(qName.Prefix, false));
				scope = ScopeChain.Pop();
				if (scope == null || scope.TagName != dataName)
				{
					if (!autoBalanceTags)
					{
						if (scope != null)
						{
							ScopeChain.Push(scope);
						}
						if (!string.IsNullOrEmpty(dataName.Prefix) && !ScopeChain.ContainsPrefix(dataName.Prefix) && string.IsNullOrEmpty(dataName.NamespaceUri) && !ScopeChain.ContainsPrefix(string.Empty))
						{
							dataName = new DataName(dataName.LocalName);
						}
						tokens.Add(MarkupGrammar.TokenElementEnd);
						return;
					}
					if (!ScopeChain.ContainsTag(dataName))
					{
						if (scope != null)
						{
							ScopeChain.Push(scope);
						}
						return;
					}
				}
				do
				{
					tokens.Add(MarkupGrammar.TokenElementEnd);
				}
				while (scope.TagName != dataName && (scope = ScopeChain.Pop()) != null);
				return;
			}
			scope = new PrefixScopeChain.Scope();
			if (attributes != null)
			{
				for (int num = attributes.Count - 1; num >= 0; num--)
				{
					Attrib attrib = attributes[num];
					if (string.IsNullOrEmpty(attrib.QName.Prefix) && attrib.QName.Name == "xmlns")
					{
						if (attrib.Value == null)
						{
							throw new InvalidOperationException("xmlns value was null");
						}
						scope[string.Empty] = attrib.Value.ValueAsString();
						attributes.RemoveAt(num);
					}
					else if (attrib.QName.Prefix == "xmlns")
					{
						if (attrib.Value == null)
						{
							throw new InvalidOperationException("xmlns value was null");
						}
						scope[attrib.QName.Name] = attrib.Value.ValueAsString();
						attributes.RemoveAt(num);
					}
				}
			}
			ScopeChain.Push(scope);
			if (!string.IsNullOrEmpty(qName.Prefix) && !ScopeChain.ContainsPrefix(qName.Prefix) && ScopeChain.ContainsPrefix(string.Empty))
			{
				scope[qName.Prefix] = string.Empty;
			}
			scope.TagName = new DataName(qName.Name, qName.Prefix, ScopeChain.GetNamespace(qName.Prefix, false));
			if (tagType == MarkupTokenType.ElementVoid)
			{
				tokens.Add(MarkupGrammar.TokenElementVoid(scope.TagName));
			}
			else
			{
				tokens.Add(MarkupGrammar.TokenElementBegin(scope.TagName));
			}
			if (attributes != null)
			{
				foreach (Attrib attribute in attributes)
				{
					DataName name = new DataName(attribute.QName.Name, attribute.QName.Prefix, ScopeChain.GetNamespace(attribute.QName.Prefix, false));
					tokens.Add(MarkupGrammar.TokenAttribute(name));
					tokens.Add(attribute.Value);
				}
			}
			if (tagType == MarkupTokenType.ElementVoid)
			{
				ScopeChain.Pop();
			}
		}

		private void EmitText(List<Token<MarkupTokenType>> tokens, string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				int count = tokens.Count;
				Token<MarkupTokenType> token = ((count <= 0) ? null : tokens[count - 1]);
				if (token != null && token.TokenType == MarkupTokenType.Primitive && token.Value is string && (count == 1 || tokens[count - 2].TokenType != MarkupTokenType.Attribute))
				{
					tokens[count - 1] = MarkupGrammar.TokenPrimitive(string.Concat(token.Value, value));
				}
				else
				{
					tokens.Add(MarkupGrammar.TokenPrimitive(value));
				}
			}
		}

		private static void SkipWhitespace(ITextStream scanner)
		{
			while (!scanner.IsCompleted && CharUtility.IsWhiteSpace(scanner.Peek()))
			{
				scanner.Pop();
			}
		}

		public string DecodeEntity(ITextStream scanner)
		{
			if (scanner.Pop() != '&')
			{
				throw new DeserializationException("Malformed entity", scanner.Index, scanner.Line, scanner.Column);
			}
			char c = scanner.Peek();
			if (!scanner.IsCompleted && !CharUtility.IsWhiteSpace(c))
			{
				switch (c)
				{
				case '&':
				case '<':
					break;
				case '#':
				{
					scanner.Pop();
					c = scanner.Peek();
					bool flag = false;
					if (!scanner.IsCompleted && (c == 'x' || c == 'X'))
					{
						flag = true;
						scanner.Pop();
						c = scanner.Peek();
					}
					scanner.BeginChunk();
					while (!scanner.IsCompleted && CharUtility.IsHexDigit(c))
					{
						scanner.Pop();
						c = scanner.Peek();
					}
					string text = scanner.EndChunk();
					int result;
					if (int.TryParse(text, flag ? NumberStyles.AllowHexSpecifier : NumberStyles.None, CultureInfo.InvariantCulture, out result))
					{
						string result2 = CharUtility.ConvertFromUtf32(result);
						if (!scanner.IsCompleted && c == ';')
						{
							scanner.Pop();
						}
						return result2;
					}
					if (flag)
					{
						return string.Concat('&', '#', 'x', text);
					}
					return string.Concat('&', '#', text);
				}
				default:
				{
					scanner.BeginChunk();
					while (!scanner.IsCompleted && CharUtility.IsLetter(c))
					{
						scanner.Pop();
						c = scanner.Peek();
					}
					string text = scanner.EndChunk();
					int num = DecodeEntityName(text);
					if (num < 0)
					{
						return '&' + text;
					}
					if (!scanner.IsCompleted && c == ';')
					{
						scanner.Pop();
					}
					return CharUtility.ConvertFromUtf32(num);
				}
				}
			}
			return char.ToString('&');
		}

		private static int DecodeEntityName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return -1;
			}
			switch (name[0])
			{
			case 'A':
				if (name.Equals("AElig"))
				{
					return 198;
				}
				if (name.Equals("Aacute"))
				{
					return 193;
				}
				if (name.Equals("Acirc"))
				{
					return 194;
				}
				if (name.Equals("Agrave"))
				{
					return 192;
				}
				if (name.Equals("Alpha"))
				{
					return 913;
				}
				if (name.Equals("Aring"))
				{
					return 197;
				}
				if (name.Equals("Atilde"))
				{
					return 195;
				}
				if (name.Equals("Auml"))
				{
					return 196;
				}
				break;
			case 'B':
				if (name.Equals("Beta"))
				{
					return 914;
				}
				break;
			case 'C':
				if (name.Equals("Ccedil"))
				{
					return 199;
				}
				if (name.Equals("Chi"))
				{
					return 935;
				}
				break;
			case 'D':
				if (name.Equals("Dagger"))
				{
					return 8225;
				}
				if (name.Equals("Delta"))
				{
					return 916;
				}
				break;
			case 'E':
				if (name.Equals("ETH"))
				{
					return 208;
				}
				if (name.Equals("Eacute"))
				{
					return 201;
				}
				if (name.Equals("Ecirc"))
				{
					return 202;
				}
				if (name.Equals("Egrave"))
				{
					return 200;
				}
				if (name.Equals("Epsilon"))
				{
					return 917;
				}
				if (name.Equals("Eta"))
				{
					return 919;
				}
				if (name.Equals("Euml"))
				{
					return 203;
				}
				break;
			case 'G':
				if (name.Equals("Gamma"))
				{
					return 915;
				}
				break;
			case 'I':
				if (name.Equals("Iacute"))
				{
					return 205;
				}
				if (name.Equals("Icirc"))
				{
					return 206;
				}
				if (name.Equals("Igrave"))
				{
					return 204;
				}
				if (name.Equals("Iota"))
				{
					return 921;
				}
				if (name.Equals("Iuml"))
				{
					return 207;
				}
				break;
			case 'K':
				if (name.Equals("Kappa"))
				{
					return 922;
				}
				break;
			case 'L':
				if (name.Equals("Lambda"))
				{
					return 923;
				}
				break;
			case 'M':
				if (name.Equals("Mu"))
				{
					return 924;
				}
				break;
			case 'N':
				if (name.Equals("Ntilde"))
				{
					return 209;
				}
				if (name.Equals("Nu"))
				{
					return 925;
				}
				break;
			case 'O':
				if (name.Equals("OElig"))
				{
					return 338;
				}
				if (name.Equals("Oacute"))
				{
					return 211;
				}
				if (name.Equals("Ocirc"))
				{
					return 212;
				}
				if (name.Equals("Ograve"))
				{
					return 210;
				}
				if (name.Equals("Omega"))
				{
					return 937;
				}
				if (name.Equals("Omicron"))
				{
					return 927;
				}
				if (name.Equals("Oslash"))
				{
					return 216;
				}
				if (name.Equals("Otilde"))
				{
					return 213;
				}
				if (name.Equals("Ouml"))
				{
					return 214;
				}
				break;
			case 'P':
				if (name.Equals("Phi"))
				{
					return 934;
				}
				if (name.Equals("Pi"))
				{
					return 928;
				}
				if (name.Equals("Prime"))
				{
					return 8243;
				}
				if (name.Equals("Psi"))
				{
					return 936;
				}
				break;
			case 'R':
				if (name.Equals("Rho"))
				{
					return 929;
				}
				break;
			case 'S':
				if (name.Equals("Scaron"))
				{
					return 352;
				}
				if (name.Equals("Sigma"))
				{
					return 931;
				}
				break;
			case 'T':
				if (name.Equals("THORN"))
				{
					return 222;
				}
				if (name.Equals("Tau"))
				{
					return 932;
				}
				if (name.Equals("Theta"))
				{
					return 920;
				}
				break;
			case 'U':
				if (name.Equals("Uacute"))
				{
					return 218;
				}
				if (name.Equals("Ucirc"))
				{
					return 219;
				}
				if (name.Equals("Ugrave"))
				{
					return 217;
				}
				if (name.Equals("Upsilon"))
				{
					return 933;
				}
				if (name.Equals("Uuml"))
				{
					return 220;
				}
				break;
			case 'X':
				if (name.Equals("Xi"))
				{
					return 926;
				}
				break;
			case 'Y':
				if (name.Equals("Yacute"))
				{
					return 221;
				}
				if (name.Equals("Yuml"))
				{
					return 376;
				}
				break;
			case 'Z':
				if (name.Equals("Zeta"))
				{
					return 918;
				}
				break;
			case 'a':
				if (name.Equals("aacute"))
				{
					return 225;
				}
				if (name.Equals("acirc"))
				{
					return 226;
				}
				if (name.Equals("acute"))
				{
					return 180;
				}
				if (name.Equals("aelig"))
				{
					return 230;
				}
				if (name.Equals("agrave"))
				{
					return 224;
				}
				if (name.Equals("alefsym"))
				{
					return 8501;
				}
				if (name.Equals("alpha"))
				{
					return 945;
				}
				if (name.Equals("amp"))
				{
					return 38;
				}
				if (name.Equals("and"))
				{
					return 8743;
				}
				if (name.Equals("ang"))
				{
					return 8736;
				}
				if (name.Equals("apos"))
				{
					return 39;
				}
				if (name.Equals("aring"))
				{
					return 229;
				}
				if (name.Equals("asymp"))
				{
					return 8776;
				}
				if (name.Equals("atilde"))
				{
					return 227;
				}
				if (name.Equals("auml"))
				{
					return 228;
				}
				break;
			case 'b':
				if (name.Equals("bdquo"))
				{
					return 8222;
				}
				if (name.Equals("beta"))
				{
					return 946;
				}
				if (name.Equals("brvbar"))
				{
					return 166;
				}
				if (name.Equals("bull"))
				{
					return 8226;
				}
				break;
			case 'c':
				if (name.Equals("cap"))
				{
					return 8745;
				}
				if (name.Equals("ccedil"))
				{
					return 231;
				}
				if (name.Equals("cedil"))
				{
					return 184;
				}
				if (name.Equals("cent"))
				{
					return 162;
				}
				if (name.Equals("chi"))
				{
					return 967;
				}
				if (name.Equals("circ"))
				{
					return 710;
				}
				if (name.Equals("clubs"))
				{
					return 9827;
				}
				if (name.Equals("cong"))
				{
					return 8773;
				}
				if (name.Equals("copy"))
				{
					return 169;
				}
				if (name.Equals("crarr"))
				{
					return 8629;
				}
				if (name.Equals("cup"))
				{
					return 8746;
				}
				if (name.Equals("curren"))
				{
					return 164;
				}
				break;
			case 'd':
				if (name.Equals("dArr"))
				{
					return 8659;
				}
				if (name.Equals("dagger"))
				{
					return 8224;
				}
				if (name.Equals("darr"))
				{
					return 8495;
				}
				if (name.Equals("deg"))
				{
					return 176;
				}
				if (name.Equals("delta"))
				{
					return 948;
				}
				if (name.Equals("diams"))
				{
					return 9830;
				}
				if (name.Equals("divide"))
				{
					return 247;
				}
				break;
			case 'e':
				if (name.Equals("eacute"))
				{
					return 233;
				}
				if (name.Equals("ecirc"))
				{
					return 234;
				}
				if (name.Equals("egrave"))
				{
					return 232;
				}
				if (name.Equals("empty"))
				{
					return 8709;
				}
				if (name.Equals("emsp"))
				{
					return 8195;
				}
				if (name.Equals("ensp"))
				{
					return 8194;
				}
				if (name.Equals("epsilon"))
				{
					return 949;
				}
				if (name.Equals("equiv"))
				{
					return 8801;
				}
				if (name.Equals("eta"))
				{
					return 951;
				}
				if (name.Equals("eth"))
				{
					return 240;
				}
				if (name.Equals("euml"))
				{
					return 235;
				}
				if (name.Equals("euro"))
				{
					return 8364;
				}
				if (name.Equals("exist"))
				{
					return 8707;
				}
				break;
			case 'f':
				if (name.Equals("fnof"))
				{
					return 402;
				}
				if (name.Equals("forall"))
				{
					return 8704;
				}
				if (name.Equals("frac12"))
				{
					return 189;
				}
				if (name.Equals("frac14"))
				{
					return 188;
				}
				if (name.Equals("frac34"))
				{
					return 190;
				}
				if (name.Equals("frasl"))
				{
					return 8260;
				}
				break;
			case 'g':
				if (name.Equals("gamma"))
				{
					return 947;
				}
				if (name.Equals("ge"))
				{
					return 8805;
				}
				if (name.Equals("gt"))
				{
					return 62;
				}
				break;
			case 'h':
				if (name.Equals("hArr"))
				{
					return 8660;
				}
				if (name.Equals("harr"))
				{
					return 8596;
				}
				if (name.Equals("hearts"))
				{
					return 9829;
				}
				if (name.Equals("hellip"))
				{
					return 8230;
				}
				break;
			case 'i':
				if (name.Equals("iacute"))
				{
					return 237;
				}
				if (name.Equals("icirc"))
				{
					return 238;
				}
				if (name.Equals("iexcl"))
				{
					return 161;
				}
				if (name.Equals("igrave"))
				{
					return 236;
				}
				if (name.Equals("image"))
				{
					return 8465;
				}
				if (name.Equals("infin"))
				{
					return 8734;
				}
				if (name.Equals("int"))
				{
					return 8747;
				}
				if (name.Equals("iota"))
				{
					return 953;
				}
				if (name.Equals("iquest"))
				{
					return 191;
				}
				if (name.Equals("isin"))
				{
					return 8712;
				}
				if (name.Equals("iuml"))
				{
					return 239;
				}
				break;
			case 'k':
				if (name.Equals("kappa"))
				{
					return 954;
				}
				break;
			case 'l':
				if (name.Equals("lArr"))
				{
					return 8656;
				}
				if (name.Equals("lambda"))
				{
					return 955;
				}
				if (name.Equals("lang"))
				{
					return 9001;
				}
				if (name.Equals("laquo"))
				{
					return 171;
				}
				if (name.Equals("larr"))
				{
					return 8592;
				}
				if (name.Equals("lceil"))
				{
					return 8968;
				}
				if (name.Equals("ldquo"))
				{
					return 8220;
				}
				if (name.Equals("le"))
				{
					return 8804;
				}
				if (name.Equals("lfloor"))
				{
					return 8970;
				}
				if (name.Equals("lowast"))
				{
					return 8727;
				}
				if (name.Equals("loz"))
				{
					return 9674;
				}
				if (name.Equals("lrm"))
				{
					return 8206;
				}
				if (name.Equals("lsaquo"))
				{
					return 8249;
				}
				if (name.Equals("lsquo"))
				{
					return 8216;
				}
				if (name.Equals("lt"))
				{
					return 60;
				}
				break;
			case 'm':
				if (name.Equals("macr"))
				{
					return 175;
				}
				if (name.Equals("mdash"))
				{
					return 8212;
				}
				if (name.Equals("micro"))
				{
					return 181;
				}
				if (name.Equals("middot"))
				{
					return 183;
				}
				if (name.Equals("minus"))
				{
					return 8722;
				}
				if (name.Equals("mu"))
				{
					return 956;
				}
				break;
			case 'n':
				if (name.Equals("nabla"))
				{
					return 8711;
				}
				if (name.Equals("nbsp"))
				{
					return 160;
				}
				if (name.Equals("ndash"))
				{
					return 8211;
				}
				if (name.Equals("ne"))
				{
					return 8800;
				}
				if (name.Equals("ni"))
				{
					return 8715;
				}
				if (name.Equals("not"))
				{
					return 172;
				}
				if (name.Equals("notin"))
				{
					return 8713;
				}
				if (name.Equals("nsub"))
				{
					return 8836;
				}
				if (name.Equals("ntilde"))
				{
					return 241;
				}
				if (name.Equals("nu"))
				{
					return 957;
				}
				break;
			case 'o':
				if (name.Equals("oacute"))
				{
					return 243;
				}
				if (name.Equals("ocirc"))
				{
					return 244;
				}
				if (name.Equals("oelig"))
				{
					return 339;
				}
				if (name.Equals("ograve"))
				{
					return 242;
				}
				if (name.Equals("oline"))
				{
					return 8254;
				}
				if (name.Equals("omega"))
				{
					return 969;
				}
				if (name.Equals("omicron"))
				{
					return 959;
				}
				if (name.Equals("oplus"))
				{
					return 8853;
				}
				if (name.Equals("or"))
				{
					return 8744;
				}
				if (name.Equals("ordf"))
				{
					return 170;
				}
				if (name.Equals("ordm"))
				{
					return 186;
				}
				if (name.Equals("oslash"))
				{
					return 248;
				}
				if (name.Equals("otilde"))
				{
					return 245;
				}
				if (name.Equals("otimes"))
				{
					return 8855;
				}
				if (name.Equals("ouml"))
				{
					return 246;
				}
				break;
			case 'p':
				if (name.Equals("para"))
				{
					return 182;
				}
				if (name.Equals("part"))
				{
					return 8706;
				}
				if (name.Equals("permil"))
				{
					return 8240;
				}
				if (name.Equals("perp"))
				{
					return 8869;
				}
				if (name.Equals("phi"))
				{
					return 966;
				}
				if (name.Equals("pi"))
				{
					return 960;
				}
				if (name.Equals("piv"))
				{
					return 982;
				}
				if (name.Equals("plusmn"))
				{
					return 177;
				}
				if (name.Equals("pound"))
				{
					return 163;
				}
				if (name.Equals("prime"))
				{
					return 8242;
				}
				if (name.Equals("prod"))
				{
					return 8719;
				}
				if (name.Equals("prop"))
				{
					return 8733;
				}
				if (name.Equals("psi"))
				{
					return 968;
				}
				break;
			case 'q':
				if (name.Equals("quot"))
				{
					return 34;
				}
				break;
			case 'r':
				if (name.Equals("rArr"))
				{
					return 8658;
				}
				if (name.Equals("radic"))
				{
					return 8730;
				}
				if (name.Equals("rang"))
				{
					return 9002;
				}
				if (name.Equals("raquo"))
				{
					return 187;
				}
				if (name.Equals("rarr"))
				{
					return 8594;
				}
				if (name.Equals("rceil"))
				{
					return 8969;
				}
				if (name.Equals("rdquo"))
				{
					return 8221;
				}
				if (name.Equals("real"))
				{
					return 8476;
				}
				if (name.Equals("reg"))
				{
					return 174;
				}
				if (name.Equals("rfloor"))
				{
					return 8971;
				}
				if (name.Equals("rho"))
				{
					return 961;
				}
				if (name.Equals("rlm"))
				{
					return 8207;
				}
				if (name.Equals("rsaquo"))
				{
					return 8250;
				}
				if (name.Equals("rsquo"))
				{
					return 8217;
				}
				break;
			case 's':
				if (name.Equals("sbquo"))
				{
					return 8218;
				}
				if (name.Equals("scaron"))
				{
					return 353;
				}
				if (name.Equals("sdot"))
				{
					return 8901;
				}
				if (name.Equals("sect"))
				{
					return 167;
				}
				if (name.Equals("shy"))
				{
					return 173;
				}
				if (name.Equals("sigma"))
				{
					return 963;
				}
				if (name.Equals("sigmaf"))
				{
					return 962;
				}
				if (name.Equals("sim"))
				{
					return 8764;
				}
				if (name.Equals("spades"))
				{
					return 9824;
				}
				if (name.Equals("sub"))
				{
					return 8834;
				}
				if (name.Equals("sube"))
				{
					return 8838;
				}
				if (name.Equals("sum"))
				{
					return 8721;
				}
				if (name.Equals("sup"))
				{
					return 8835;
				}
				if (name.Equals("sup1"))
				{
					return 185;
				}
				if (name.Equals("sup2"))
				{
					return 178;
				}
				if (name.Equals("sup3"))
				{
					return 179;
				}
				if (name.Equals("supe"))
				{
					return 8839;
				}
				if (name.Equals("szlig"))
				{
					return 223;
				}
				break;
			case 't':
				if (name.Equals("tau"))
				{
					return 964;
				}
				if (name.Equals("there4"))
				{
					return 8756;
				}
				if (name.Equals("theta"))
				{
					return 952;
				}
				if (name.Equals("thetasym"))
				{
					return 977;
				}
				if (name.Equals("thinsp"))
				{
					return 8201;
				}
				if (name.Equals("thorn"))
				{
					return 254;
				}
				if (name.Equals("tilde"))
				{
					return 732;
				}
				if (name.Equals("times"))
				{
					return 215;
				}
				if (name.Equals("trade"))
				{
					return 8482;
				}
				break;
			case 'u':
				if (name.Equals("uArr"))
				{
					return 8657;
				}
				if (name.Equals("uacute"))
				{
					return 250;
				}
				if (name.Equals("uarr"))
				{
					return 8593;
				}
				if (name.Equals("ucirc"))
				{
					return 251;
				}
				if (name.Equals("ugrave"))
				{
					return 249;
				}
				if (name.Equals("uml"))
				{
					return 168;
				}
				if (name.Equals("upsih"))
				{
					return 978;
				}
				if (name.Equals("upsilon"))
				{
					return 965;
				}
				if (name.Equals("uuml"))
				{
					return 252;
				}
				break;
			case 'w':
				if (name.Equals("weierp"))
				{
					return 8472;
				}
				break;
			case 'x':
				if (name.Equals("xi"))
				{
					return 958;
				}
				break;
			case 'y':
				if (name.Equals("yacute"))
				{
					return 253;
				}
				if (name.Equals("yen"))
				{
					return 165;
				}
				if (name.Equals("yuml"))
				{
					return 255;
				}
				break;
			case 'z':
				if (name.Equals("zeta"))
				{
					return 950;
				}
				if (name.Equals("zwj"))
				{
					return 8205;
				}
				if (name.Equals("zwnj"))
				{
					return 8204;
				}
				break;
			}
			return -1;
		}

		private static bool IsNameStartChar(char ch)
{
    return (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch == ':' || ch == '_' || (ch >= '\u00C0' && ch <= '\u00D6') || (ch >= '\u00D8' && ch <= '\u00F6') || (ch >= '\u00F8' && ch <= '\u02FF') || (ch >= '\u0370' && ch <= '\u037D') || (ch >= '\u037F' && ch <= '\u1FFF') || (ch >= '\u200C' && ch <= '\u200D') || (ch >= '\u2070' && ch <= '\u218F') || (ch >= '\u2C00' && ch <= '\u2FEF') || (ch >= '\u3001' && ch <= '\uD7FF') || (ch >= '\uF900' && ch <= '\uFDCF') || (ch >= '\uFDF0' && ch <= '\uFFFD');
}

private static bool IsNameChar(char ch)
{
    return IsNameStartChar(ch) || (ch >= '0' && ch <= '9') || ch == '-' || ch == '.' || ch == '\u00B7' || (ch >= '\u0300' && ch <= '\u036F') || (ch >= '\u203F' && ch <= '\u2040');
}

		public IEnumerable<Token<MarkupTokenType>> GetTokens(TextReader reader)
		{
			List<Token<MarkupTokenType>> list = new List<Token<MarkupTokenType>>();
			GetTokens(list, Scanner = new TextReaderStream(reader));
			return list;
		}

		public IEnumerable<Token<MarkupTokenType>> GetTokens(string text)
		{
			List<Token<MarkupTokenType>> list = new List<Token<MarkupTokenType>>();
			GetTokens(list, Scanner = new StringStream(text));
			return list;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				Scanner.Dispose();
			}
		}
	}
}
