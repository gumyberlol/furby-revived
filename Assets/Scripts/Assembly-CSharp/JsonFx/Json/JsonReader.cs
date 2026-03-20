using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using JsonFx.IO;
using JsonFx.Model;
using JsonFx.Serialization;
using JsonFx.Utils;

namespace JsonFx.Json
{
	public class JsonReader : ModelReader
	{
		public class JsonTokenizer : IDisposable, ITextTokenizer<ModelTokenType>
		{
			private enum NeedsValueDelim
			{
				Forbidden = 0,
				CurrentIsDelim = 1,
				Required = 2
			}

			private const string ErrorUnrecognizedToken = "Illegal JSON sequence";

			private const string ErrorUnterminatedComment = "Unterminated comment block";

			private const string ErrorUnterminatedString = "Unterminated JSON string";

			private const string ErrorIllegalNumber = "Illegal JSON number";

			private const string ErrorMissingValueDelim = "Missing value delimiter";

			private const string ErrorExtraValueDelim = "Extraneous value delimiter";

			private const int DefaultBufferSize = 32;

			private ITextStream Scanner = TextReaderStream.Null;

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

			protected IEnumerable<Token<ModelTokenType>> GetTokens(ITextStream scanner)
			{
				if (scanner == null)
				{
					throw new ArgumentNullException("scanner");
				}
				Scanner = scanner;
				int depth = 0;
				NeedsValueDelim needsValueDelim = NeedsValueDelim.Forbidden;
				long strPos;
				int strLine;
				int strCol;
				while (true)
				{
					SkipCommentsAndWhitespace(scanner);
					if (scanner.IsCompleted)
					{
						Scanner = StringStream.Null;
						scanner.Dispose();
						yield break;
					}
					bool hasUnaryOp = false;
					char ch = scanner.Peek();
					switch (ch)
					{
					case '[':
						scanner.Pop();
						if (needsValueDelim == NeedsValueDelim.Required)
						{
							throw new DeserializationException("Missing value delimiter", scanner.Index, scanner.Line, scanner.Column);
						}
						yield return ModelGrammar.TokenArrayBeginUnnamed;
						depth++;
						needsValueDelim = NeedsValueDelim.Forbidden;
						continue;
					case ']':
						scanner.Pop();
						if (needsValueDelim == NeedsValueDelim.CurrentIsDelim)
						{
							throw new DeserializationException("Extraneous value delimiter", scanner.Index, scanner.Line, scanner.Column);
						}
						yield return ModelGrammar.TokenArrayEnd;
						depth--;
						needsValueDelim = ((depth > 0) ? NeedsValueDelim.Required : NeedsValueDelim.Forbidden);
						continue;
					case '{':
						scanner.Pop();
						if (needsValueDelim == NeedsValueDelim.Required)
						{
							throw new DeserializationException("Missing value delimiter", scanner.Index, scanner.Line, scanner.Column);
						}
						yield return ModelGrammar.TokenObjectBeginUnnamed;
						depth++;
						needsValueDelim = NeedsValueDelim.Forbidden;
						continue;
					case '}':
						scanner.Pop();
						if (needsValueDelim == NeedsValueDelim.CurrentIsDelim)
						{
							throw new DeserializationException("Extraneous value delimiter", scanner.Index, scanner.Line, scanner.Column);
						}
						yield return ModelGrammar.TokenObjectEnd;
						depth--;
						needsValueDelim = ((depth > 0) ? NeedsValueDelim.Required : NeedsValueDelim.Forbidden);
						continue;
					case '"':
					case '\'':
					{
						if (needsValueDelim == NeedsValueDelim.Required)
						{
							throw new DeserializationException("Missing value delimiter", scanner.Index, scanner.Line, scanner.Column);
						}
						string value = ScanString(scanner);
						SkipCommentsAndWhitespace(scanner);
						if (scanner.Peek() == ':')
						{
							scanner.Pop();
							yield return ModelGrammar.TokenProperty(new DataName(value));
							needsValueDelim = NeedsValueDelim.Forbidden;
						}
						else
						{
							yield return ModelGrammar.TokenPrimitive(value);
							needsValueDelim = NeedsValueDelim.Required;
						}
						continue;
					}
					case '+':
					case '-':
						hasUnaryOp = true;
						break;
					case ',':
						scanner.Pop();
						if (needsValueDelim != NeedsValueDelim.Required)
						{
							throw new DeserializationException("Extraneous value delimiter", scanner.Index, scanner.Line, scanner.Column);
						}
						needsValueDelim = NeedsValueDelim.CurrentIsDelim;
						continue;
					case ':':
						throw new DeserializationException("Illegal JSON sequence", scanner.Index + 1, scanner.Line, scanner.Column);
					}
					if (needsValueDelim == NeedsValueDelim.Required)
					{
						throw new DeserializationException("Missing value delimiter", scanner.Index, scanner.Line, scanner.Column);
					}
					Token<ModelTokenType> token = ScanNumber(scanner);
					if (token != null)
					{
						yield return token;
						needsValueDelim = NeedsValueDelim.Required;
						continue;
					}
					if (!hasUnaryOp)
					{
						ch = '\0';
					}
					strPos = scanner.Index + 1;
					strLine = scanner.Line;
					strCol = scanner.Column;
					string ident = ScanIdentifier(scanner);
					if (string.IsNullOrEmpty(ident))
					{
						break;
					}
					token = ScanKeywords(scanner, ident, ch, out needsValueDelim);
					if (!(token != null))
					{
						break;
					}
					yield return token;
				}
				throw new DeserializationException("Illegal JSON sequence", strPos, strLine, strCol);
			}

			private static void SkipCommentsAndWhitespace(ITextStream scanner)
			{
				long index;
				int column;
				int line;
				while (true)
				{
					SkipWhitespace(scanner);
					if (scanner.IsCompleted || scanner.Peek() != "/*"[0])
					{
						return;
					}
					scanner.Pop();
					index = scanner.Index;
					column = scanner.Column;
					line = scanner.Line;
					if (scanner.IsCompleted)
					{
						throw new DeserializationException("Unterminated comment block", index, line, column);
					}
					char c = scanner.Peek();
					bool flag;
					if (c == "/*"[1])
					{
						flag = true;
					}
					else
					{
						if (c != "//"[1])
						{
							break;
						}
						flag = false;
					}
					if (flag)
					{
						while (true)
						{
							scanner.Pop();
							if (scanner.IsCompleted)
							{
								throw new DeserializationException("Unterminated comment block", index, line, column);
							}
							if (scanner.Peek() == "*/"[0])
							{
								scanner.Pop();
								if (scanner.IsCompleted)
								{
									throw new DeserializationException("Unterminated comment block", index, line, column);
								}
								if (scanner.Peek() == "*/"[1])
								{
									break;
								}
							}
						}
						scanner.Pop();
					}
					else
					{
						do
						{
							scanner.Pop();
							c = scanner.Peek();
						}
						while (!scanner.IsCompleted && c != '\r' && c != '\n');
					}
				}
				throw new DeserializationException("Unterminated comment block", index, line, column);
			}

			private static void SkipWhitespace(ITextStream scanner)
			{
				while (!scanner.IsCompleted && CharUtility.IsWhiteSpace(scanner.Peek()))
				{
					scanner.Pop();
				}
			}

			private static Token<ModelTokenType> ScanNumber(ITextStream scanner)
			{
				long index = scanner.Index + 1;
				int line = scanner.Line;
				int column = scanner.Column;
				scanner.BeginChunk();
				char c = scanner.Peek();
				bool flag = false;
				switch (c)
				{
				case '+':
					scanner.Pop();
					c = scanner.Peek();
					scanner.BeginChunk();
					break;
				case '-':
					scanner.Pop();
					c = scanner.Peek();
					flag = true;
					break;
				}
				if (!CharUtility.IsDigit(c) && c != '.')
				{
					scanner.EndChunk();
					return null;
				}
				while (!scanner.IsCompleted && CharUtility.IsDigit(c))
				{
					scanner.Pop();
					c = scanner.Peek();
				}
				bool flag2 = false;
				if (!scanner.IsCompleted && c == '.')
				{
					scanner.Pop();
					c = scanner.Peek();
					while (!scanner.IsCompleted && CharUtility.IsDigit(c))
					{
						scanner.Pop();
						c = scanner.Peek();
						flag2 = true;
					}
					if (!flag2)
					{
						throw new DeserializationException("Illegal JSON number", index, line, column);
					}
				}
				int num = scanner.ChunkSize;
				if (flag2)
				{
					num--;
				}
				if (flag)
				{
					num--;
				}
				if (num < 1)
				{
					throw new DeserializationException("Illegal JSON number", index, line, column);
				}
				bool flag3 = false;
				if (!scanner.IsCompleted && (c == 'e' || c == 'E'))
				{
					scanner.Pop();
					c = scanner.Peek();
					if ((!scanner.IsCompleted && c == '-') || c == '+')
					{
						scanner.Pop();
						c = scanner.Peek();
					}
					while (!scanner.IsCompleted && CharUtility.IsDigit(c))
					{
						scanner.Pop();
						c = scanner.Peek();
						flag3 = true;
					}
					if (!flag3)
					{
						throw new DeserializationException("Illegal JSON number", index, line, column);
					}
				}
				if (!scanner.IsCompleted && CharUtility.IsLetter(c))
				{
					throw new DeserializationException("Illegal JSON number", index, line, column);
				}
				string s = scanner.EndChunk();
				if (!flag2 && !flag3 && num < 19)
				{
					decimal num2 = 0m;
					try
					{
						num2 = decimal.Parse(s, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
					}
					catch (Exception)
					{
						throw new DeserializationException("Illegal JSON number", index, line, column);
					}
					if (num2 >= -2147483648m && num2 <= 2147483647m)
					{
						return ModelGrammar.TokenPrimitive((int)num2);
					}
					if (num2 >= -9223372036854775808m && num2 <= 9223372036854775807m)
					{
						return ModelGrammar.TokenPrimitive((long)num2);
					}
					return ModelGrammar.TokenPrimitive(num2);
				}
				double num3;
				try
				{
					num3 = double.Parse(s, NumberStyles.Float, NumberFormatInfo.InvariantInfo);
				}
				catch (Exception)
				{
					throw new DeserializationException("Illegal JSON number", index, line, column);
				}
				return ModelGrammar.TokenPrimitive(num3);
			}

			private static string ScanString(ITextStream scanner)
			{
				long index = scanner.Index + 1;
				int line = scanner.Line;
				int column = scanner.Column;
				char c = scanner.Peek();
				scanner.Pop();
				char c2 = scanner.Peek();
				scanner.BeginChunk();
				StringBuilder stringBuilder = new StringBuilder(32);
				while (true)
				{
					if (scanner.IsCompleted || (CharUtility.IsControl(c2) && c2 != '\t'))
					{
						throw new DeserializationException("Unterminated JSON string", index, line, column);
					}
					if (c2 == c)
					{
						scanner.EndChunk(stringBuilder);
						scanner.Pop();
						return stringBuilder.ToString();
					}
					if (c2 != '\\')
					{
						scanner.Pop();
						c2 = scanner.Peek();
						continue;
					}
					scanner.EndChunk(stringBuilder);
					scanner.Pop();
					c2 = scanner.Peek();
					if (scanner.IsCompleted || (CharUtility.IsControl(c2) && c2 != '\t'))
					{
						break;
					}
					switch (c2)
					{
					case '0':
						scanner.Pop();
						c2 = scanner.Peek();
						break;
					case 'b':
						stringBuilder.Append('\b');
						scanner.Pop();
						c2 = scanner.Peek();
						break;
					case 'f':
						stringBuilder.Append('\f');
						scanner.Pop();
						c2 = scanner.Peek();
						break;
					case 'n':
						stringBuilder.Append('\n');
						scanner.Pop();
						c2 = scanner.Peek();
						break;
					case 'r':
						stringBuilder.Append('\r');
						scanner.Pop();
						c2 = scanner.Peek();
						break;
					case 't':
						stringBuilder.Append('\t');
						scanner.Pop();
						c2 = scanner.Peek();
						break;
					case 'u':
					{
						scanner.Pop();
						c2 = scanner.Peek();
						string text = string.Empty;
						int num = 4;
						while (!scanner.IsCompleted && CharUtility.IsHexDigit(c2) && num > 0)
						{
							text += c2;
							scanner.Pop();
							c2 = scanner.Peek();
							num--;
						}
						int utf = 0;
						bool flag = true;
						try
						{
							utf = int.Parse(text, NumberStyles.AllowHexSpecifier, NumberFormatInfo.InvariantInfo);
						}
						catch (Exception)
						{
							flag = false;
						}
						if (text.Length == 4 && flag)
						{
							stringBuilder.Append(CharUtility.ConvertFromUtf32(utf));
							break;
						}
						stringBuilder.Append('u');
						stringBuilder.Append(text);
						break;
					}
					default:
						stringBuilder.Append(c2);
						scanner.Pop();
						c2 = scanner.Peek();
						break;
					}
					scanner.BeginChunk();
				}
				throw new DeserializationException("Unterminated JSON string", index, line, column);
			}

			private static Token<ModelTokenType> ScanKeywords(ITextStream scanner, string ident, char unary, out NeedsValueDelim needsValueDelim)
			{
				needsValueDelim = NeedsValueDelim.Required;
				switch (ident)
				{
				case "false":
					if (unary != 0)
					{
						return null;
					}
					return ModelGrammar.TokenFalse;
				case "true":
					if (unary != 0)
					{
						return null;
					}
					return ModelGrammar.TokenTrue;
				case "null":
					if (unary != 0)
					{
						return null;
					}
					return ModelGrammar.TokenNull;
				case "NaN":
					if (unary != 0)
					{
						return null;
					}
					return ModelGrammar.TokenNaN;
				case "Infinity":
					switch (unary)
					{
					case '\0':
					case '+':
						return ModelGrammar.TokenPositiveInfinity;
					case '-':
						return ModelGrammar.TokenNegativeInfinity;
					default:
						return null;
					}
				case "undefined":
					if (unary != 0)
					{
						return null;
					}
					return ModelGrammar.TokenNull;
				default:
					if (unary != 0)
					{
						ident = char.ToString(unary) + ident;
					}
					SkipCommentsAndWhitespace(scanner);
					if (scanner.Peek() == ':')
					{
						scanner.Pop();
						needsValueDelim = NeedsValueDelim.Forbidden;
						return ModelGrammar.TokenProperty(new DataName(ident));
					}
					return null;
				}
			}

			private static string ScanIdentifier(ITextStream scanner)
			{
				bool flag = false;
				scanner.BeginChunk();
				while (true)
				{
					char c = scanner.Peek();
					if ((!flag || !CharUtility.IsDigit(c)) && !CharUtility.IsLetter(c) && c != '_' && c != '$')
					{
						break;
					}
					flag = true;
					scanner.Pop();
					c = scanner.Peek();
				}
				return scanner.EndChunk();
			}

			public IEnumerable<Token<ModelTokenType>> GetTokens(TextReader reader)
			{
				return new SequenceBuffer<Token<ModelTokenType>>(GetTokens(new TextReaderStream(reader)));
			}

			public IEnumerable<Token<ModelTokenType>> GetTokens(string text)
			{
				return new SequenceBuffer<Token<ModelTokenType>>(GetTokens(new StringStream(text)));
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
					yield return "application/json";
					yield return "text/json";
					yield return "text/x-json";
				}
			}
		}

		public JsonReader()
			: this(new DataReaderSettings())
		{
		}

		public JsonReader(DataReaderSettings settings)
			: base((settings == null) ? new DataReaderSettings() : settings)
		{
		}

		public JsonReader(DataReaderSettings settings, params string[] contentTypes)
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
			return new JsonTokenizer();
		}
	}
}
