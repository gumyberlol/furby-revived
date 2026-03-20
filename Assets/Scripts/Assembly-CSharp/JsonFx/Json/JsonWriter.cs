using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using JsonFx.Model;
using JsonFx.Serialization;
using JsonFx.Utils;

namespace JsonFx.Json
{
	public class JsonWriter : ModelWriter
	{
		public class JsonFormatter : ITextFormatter<ModelTokenType>
		{
			private readonly DataWriterSettings Settings;

			private bool encodeLessThan;

			public bool EncodeLessThan
			{
				get
				{
					return encodeLessThan;
				}
				set
				{
					encodeLessThan = value;
				}
			}

			public JsonFormatter(DataWriterSettings settings)
			{
				if (settings == null)
				{
					throw new ArgumentNullException("settings");
				}
				Settings = settings;
			}

			public string Format(IEnumerable<Token<ModelTokenType>> tokens)
			{
				using (StringWriter stringWriter = new StringWriter())
				{
					Format(tokens, stringWriter);
					return stringWriter.GetStringBuilder().ToString();
				}
			}

			public void Format(IEnumerable<Token<ModelTokenType>> tokens, TextWriter writer)
			{
				if (tokens == null)
				{
					throw new ArgumentNullException("tokens");
				}
				bool prettyPrint = Settings.PrettyPrint;
				bool flag = false;
				bool flag2 = false;
				int num = 0;
				foreach (Token<ModelTokenType> token in tokens)
				{
					switch (token.TokenType)
					{
					case ModelTokenType.ArrayBegin:
						if (flag2)
						{
							writer.Write(',');
							if (prettyPrint)
							{
								WriteLine(writer, num);
							}
						}
						if (flag)
						{
							if (prettyPrint)
							{
								WriteLine(writer, ++num);
							}
							flag = false;
						}
						writer.Write('[');
						flag = true;
						flag2 = false;
						break;
					case ModelTokenType.ArrayEnd:
						if (flag)
						{
							flag = false;
						}
						else if (prettyPrint)
						{
							WriteLine(writer, --num);
						}
						writer.Write(']');
						flag2 = true;
						break;
					case ModelTokenType.Primitive:
					{
						if (flag2)
						{
							writer.Write(',');
							if (prettyPrint)
							{
								WriteLine(writer, num);
							}
						}
						if (flag)
						{
							if (prettyPrint)
							{
								WriteLine(writer, ++num);
							}
							flag = false;
						}
						Type type = ((token.Value != null) ? token.Value.GetType() : null);
						TypeCode typeCode = Type.GetTypeCode(type);
						switch (typeCode)
						{
						case TypeCode.Boolean:
							writer.Write((!true.Equals(token.Value)) ? "false" : "true");
							break;
						case TypeCode.SByte:
						case TypeCode.Byte:
						case TypeCode.Int16:
						case TypeCode.UInt16:
						case TypeCode.Int32:
						case TypeCode.UInt32:
						case TypeCode.Int64:
						case TypeCode.UInt64:
						case TypeCode.Single:
						case TypeCode.Double:
						case TypeCode.Decimal:
							if (type.IsEnum)
							{
								goto default;
							}
							WriteNumber(writer, token.Value, typeCode);
							break;
						case TypeCode.Empty:
						case TypeCode.DBNull:
							writer.Write("null");
							break;
						default:
						{
							ITextFormattable<ModelTokenType> textFormattable = token.Value as ITextFormattable<ModelTokenType>;
							if (textFormattable != null)
							{
								textFormattable.Format(this, writer);
							}
							else
							{
								WritePrimitive(writer, token.Value);
							}
							break;
						}
						}
						flag2 = true;
						break;
					}
					case ModelTokenType.ObjectBegin:
						if (flag2)
						{
							writer.Write(',');
							if (prettyPrint)
							{
								WriteLine(writer, num);
							}
						}
						if (flag)
						{
							if (prettyPrint)
							{
								WriteLine(writer, ++num);
							}
							flag = false;
						}
						writer.Write('{');
						flag = true;
						flag2 = false;
						break;
					case ModelTokenType.ObjectEnd:
						if (flag)
						{
							flag = false;
						}
						else if (prettyPrint)
						{
							WriteLine(writer, --num);
						}
						writer.Write('}');
						flag2 = true;
						break;
					case ModelTokenType.Property:
					{
						if (flag2)
						{
							writer.Write(',');
							if (prettyPrint)
							{
								WriteLine(writer, num);
							}
						}
						if (flag)
						{
							if (prettyPrint)
							{
								WriteLine(writer, ++num);
							}
							flag = false;
						}
						DataName name = token.Name;
						WritePropertyName(writer, name.LocalName);
						if (prettyPrint)
						{
							writer.Write(" ");
							writer.Write(':');
							writer.Write(" ");
						}
						else
						{
							writer.Write(':');
						}
						flag2 = false;
						break;
					}
					default:
						throw new TokenException<ModelTokenType>(token, string.Format("Unexpected token ({0})", token.TokenType));
					}
				}
			}

			protected virtual void WritePrimitive(TextWriter writer, object value)
			{
				if (value is TimeSpan)
				{
					WriteNumber(writer, ((TimeSpan)value).Ticks, TypeCode.Int64);
				}
				else
				{
					WriteString(writer, FormatString(value));
				}
			}

			protected virtual void WritePropertyName(TextWriter writer, string propertyName)
			{
				WriteString(writer, FormatString(propertyName));
			}

			protected virtual void WriteNumber(TextWriter writer, object value, TypeCode typeCode)
			{
				bool flag = false;
				string value2;
				switch (typeCode)
				{
				case TypeCode.Byte:
					value2 = ((byte)value).ToString("g", CultureInfo.InvariantCulture);
					break;
				case TypeCode.Boolean:
					value2 = ((!true.Equals(value)) ? "0" : "1");
					break;
				case TypeCode.Decimal:
					flag = true;
					value2 = ((decimal)value).ToString("g", CultureInfo.InvariantCulture);
					break;
				case TypeCode.Double:
				{
					double d = (double)value;
					if (double.IsNaN(d))
					{
						WriteNaN(writer);
						return;
					}
					if (double.IsInfinity(d))
					{
						if (double.IsNegativeInfinity(d))
						{
							WriteNegativeInfinity(writer);
						}
						else
						{
							WritePositiveInfinity(writer);
						}
						return;
					}
					value2 = d.ToString("r", CultureInfo.InvariantCulture);
					break;
				}
				case TypeCode.Int16:
					value2 = ((short)value).ToString("g", CultureInfo.InvariantCulture);
					break;
				case TypeCode.Int32:
					value2 = ((int)value).ToString("g", CultureInfo.InvariantCulture);
					break;
				case TypeCode.Int64:
					flag = true;
					value2 = ((long)value).ToString("g", CultureInfo.InvariantCulture);
					break;
				case TypeCode.SByte:
					value2 = ((sbyte)value).ToString("g", CultureInfo.InvariantCulture);
					break;
				case TypeCode.Single:
				{
					float f = (float)value;
					if (float.IsNaN(f))
					{
						WriteNaN(writer);
						return;
					}
					if (float.IsInfinity(f))
					{
						if (float.IsNegativeInfinity(f))
						{
							WriteNegativeInfinity(writer);
						}
						else
						{
							WritePositiveInfinity(writer);
						}
						return;
					}
					value2 = f.ToString("r", CultureInfo.InvariantCulture);
					break;
				}
				case TypeCode.UInt16:
					value2 = ((ushort)value).ToString("g", CultureInfo.InvariantCulture);
					break;
				case TypeCode.UInt32:
					flag = true;
					value2 = ((uint)value).ToString("g", CultureInfo.InvariantCulture);
					break;
				case TypeCode.UInt64:
					flag = true;
					value2 = ((ulong)value).ToString("g", CultureInfo.InvariantCulture);
					break;
				default:
					throw new TokenException<ModelTokenType>(ModelGrammar.TokenPrimitive(value), "Invalid number token");
				}
				if (flag && InvalidIEEE754(Convert.ToDecimal(value)))
				{
					WriteString(writer, value2);
				}
				else
				{
					writer.Write(value2);
				}
			}

			protected virtual void WriteNegativeInfinity(TextWriter writer)
			{
				writer.Write("null");
			}

			protected virtual void WritePositiveInfinity(TextWriter writer)
			{
				writer.Write("null");
			}

			protected virtual void WriteNaN(TextWriter writer)
			{
				writer.Write("null");
			}

			protected virtual void WriteString(TextWriter writer, string value)
			{
				int num = 0;
				int length = value.Length;
				writer.Write('"');
				for (int i = num; i < length; i++)
				{
					char c = value[i];
					if (c <= '\u001f' || c >= '\u007f' || (encodeLessThan && c == '<') || c == '"' || c == '\\')
					{
						if (i > num)
						{
							writer.Write(value.Substring(num, i - num));
						}
						num = i + 1;
						switch (c)
						{
						case '"':
						case '\\':
							writer.Write('\\');
							writer.Write(c);
							break;
						case '\b':
							writer.Write("\\b");
							break;
						case '\f':
							writer.Write("\\f");
							break;
						case '\n':
							writer.Write("\\n");
							break;
						case '\r':
							writer.Write("\\r");
							break;
						case '\t':
							writer.Write("\\t");
							break;
						default:
							writer.Write("\\u");
							writer.Write(CharUtility.ConvertToUtf32(value, i).ToString("X4"));
							break;
						}
					}
				}
				if (length > num)
				{
					writer.Write(value.Substring(num, length - num));
				}
				writer.Write('"');
			}

			private void WriteLine(TextWriter writer, int depth)
			{
				writer.Write(Settings.NewLine);
				for (int i = 0; i < depth; i++)
				{
					writer.Write(Settings.Tab);
				}
			}

			protected virtual string FormatString(object value)
			{
				if (value is Enum)
				{
					return FormatEnum((Enum)value);
				}
				return Token<ModelTokenType>.ToString(value);
			}

			private string FormatEnum(Enum value)
			{
				Type type = value.GetType();
				IDictionary<Enum, string> dictionary = Settings.Resolver.LoadEnumMaps(type);
				if (type.IsDefined(typeof(FlagsAttribute), true) && !Enum.IsDefined(type, value))
				{
					Enum[] flagList = GetFlagList(type, value);
					string[] array = new string[flagList.Length];
					for (int i = 0; i < flagList.Length; i++)
					{
						if (!dictionary.TryGetValue(flagList[i], out array[i]) || string.IsNullOrEmpty(array[i]))
						{
							array[i] = flagList[i].ToString("f");
						}
					}
					return string.Join(", ", array);
				}
				string value2;
				if (!dictionary.TryGetValue(value, out value2) || string.IsNullOrEmpty(value2))
				{
					return value.ToString("f");
				}
				return value2;
			}

			private static Enum[] GetFlagList(Type enumType, object value)
			{
				ulong num = Convert.ToUInt64(value);
				Array values = Enum.GetValues(enumType);
				List<Enum> list = new List<Enum>(values.Length);
				if (num == 0L)
				{
					list.Add((Enum)Convert.ChangeType(value, enumType, CultureInfo.InvariantCulture));
					return list.ToArray();
				}
				for (int num2 = values.Length - 1; num2 >= 0; num2--)
				{
					ulong num3 = Convert.ToUInt64(values.GetValue(num2));
					if ((num2 != 0 || num3 != 0L) && (num & num3) == num3)
					{
						num -= num3;
						list.Add(values.GetValue(num2) as Enum);
					}
				}
				if (num != 0L)
				{
					list.Add(Enum.ToObject(enumType, num) as Enum);
				}
				return list.ToArray();
			}

			protected virtual bool InvalidIEEE754(decimal value)
			{
				try
				{
					return (decimal)decimal.ToDouble(value) != value;
				}
				catch
				{
					return true;
				}
			}
		}

		private const string ErrorUnexpectedToken = "Unexpected token ({0})";

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

		public override IEnumerable<string> FileExtension
		{
			get
			{
				yield return ".json";
			}
		}

		public JsonWriter()
			: base(new DataWriterSettings())
		{
		}

		public JsonWriter(DataWriterSettings settings)
			: base((settings == null) ? new DataWriterSettings() : settings)
		{
		}

		public JsonWriter(DataWriterSettings settings, params string[] contentTypes)
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
			return new JsonFormatter(Settings);
		}
	}
}
