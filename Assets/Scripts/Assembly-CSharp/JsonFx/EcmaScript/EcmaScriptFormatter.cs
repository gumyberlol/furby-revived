using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JsonFx.Json;
using JsonFx.Serialization;

namespace JsonFx.EcmaScript
{
	public class EcmaScriptFormatter : JsonWriter.JsonFormatter
	{
		private const string EcmaScriptDateCtor1 = "new Date({0})";

		private const string EcmaScriptDateCtor7 = "new Date({0:0000},{1},{2},{3},{4},{5},{6})";

		private const string EmptyRegExpLiteral = "(?:)";

		private const char RegExpLiteralDelim = '/';

		private const char OperatorCharEscape = '\\';

		private const string NamespaceDelim = ".";

		private const string RootDeclarationDebug = "\r\n/* namespace {1} */\r\nvar {0};";

		private const string RootDeclaration = "var {0};";

		private const string NamespaceCheck = "if(\"undefined\"===typeof {0}){{{0}={{}};}}";

		private const string NamespaceCheckDebug = "\r\nif (\"undefined\" === typeof {0}) {{\r\n\t{0} = {{}};\r\n}}";

		private static readonly DateTime EcmaScriptEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

		private static readonly char[] NamespaceDelims = new char[1] { '.' };

		private static readonly IList<string> BrowserObjects = new List<string>(new string[10] { "console", "document", "event", "frames", "history", "location", "navigator", "opera", "screen", "window" });

		public EcmaScriptFormatter(DataWriterSettings settings)
			: base(settings)
		{
		}

		public static bool WriteNamespaceDeclaration(TextWriter writer, string ident, List<string> namespaces, bool prettyPrint)
		{
			if (string.IsNullOrEmpty(ident))
			{
				return false;
			}
			if (namespaces == null)
			{
				namespaces = new List<string>();
			}
			string[] array = ident.Split(NamespaceDelims, StringSplitOptions.RemoveEmptyEntries);
			string text = array[0];
			bool flag = false;
			for (int i = 0; i < array.Length - 1; i++)
			{
				flag = true;
				if (i > 0)
				{
					text += ".";
					text += array[i];
				}
				if (namespaces.Contains(text) || BrowserObjects.Contains(text))
				{
					continue;
				}
				namespaces.Add(text);
				if (i == 0)
				{
					if (prettyPrint)
					{
						writer.Write("\r\n/* namespace {1} */\r\nvar {0};", text, string.Join(".", array, 0, array.Length - 1));
					}
					else
					{
						writer.Write("var {0};", text);
					}
				}
				if (prettyPrint)
				{
					writer.WriteLine("\r\nif (\"undefined\" === typeof {0}) {{\r\n\t{0} = {{}};\r\n}}", text);
				}
				else
				{
					writer.Write("if(\"undefined\"===typeof {0}){{{0}={{}};}}", text);
				}
			}
			if (prettyPrint && flag)
			{
				writer.WriteLine();
			}
			return flag;
		}

		protected override void WriteNaN(TextWriter writer)
		{
			writer.Write("NaN");
		}

		protected override void WriteNegativeInfinity(TextWriter writer)
		{
			writer.Write('-');
			writer.Write("Infinity");
		}

		protected override void WritePositiveInfinity(TextWriter writer)
		{
			writer.Write("Infinity");
		}

		protected override void WritePrimitive(TextWriter writer, object value)
		{
			if (value is DateTime)
			{
				WriteEcmaScriptDate(writer, (DateTime)value);
			}
			else if (value is Regex)
			{
				WriteEcmaScriptRegExp(writer, (Regex)value);
			}
			else
			{
				base.WritePrimitive(writer, value);
			}
		}

		protected override void WritePropertyName(TextWriter writer, string propertyName)
		{
			if (EcmaScriptIdentifier.IsValidIdentifier(propertyName, false))
			{
				writer.Write(propertyName);
			}
			else
			{
				base.WritePropertyName(writer, propertyName);
			}
		}

		public static void WriteEcmaScriptDate(TextWriter writer, DateTime value)
		{
			if (value.Kind == DateTimeKind.Unspecified)
			{
				writer.Write("new Date({0:0000},{1},{2},{3},{4},{5},{6})", value.Year, value.Month - 1, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond);
			}
			else
			{
				if (value.Kind == DateTimeKind.Local)
				{
					value = value.ToUniversalTime();
				}
				long num = (long)value.Subtract(EcmaScriptEpoch).TotalMilliseconds;
				writer.Write("new Date({0})", num);
			}
		}

		public static void WriteEcmaScriptRegExp(TextWriter writer, Regex regex)
		{
			WriteEcmaScriptRegExp(writer, regex, false);
		}

		public static void WriteEcmaScriptRegExp(TextWriter writer, Regex regex, bool isGlobal)
		{
			if (regex == null)
			{
				writer.Write("null");
				return;
			}
			string text = regex.ToString();
			if (string.IsNullOrEmpty(text))
			{
				text = "(?:)";
			}
			string text2 = ((!isGlobal) ? string.Empty : "g");
			switch (regex.Options & (RegexOptions.IgnoreCase | RegexOptions.Multiline))
			{
			case RegexOptions.IgnoreCase:
				text2 += "i";
				break;
			case RegexOptions.Multiline:
				text2 += "m";
				break;
			case RegexOptions.IgnoreCase | RegexOptions.Multiline:
				text2 += "im";
				break;
			}
			writer.Write('/');
			int length = text.Length;
			int num = 0;
			for (int i = num; i < length; i++)
			{
				char c = text[i];
				if (c == '/')
				{
					writer.Write(text.Substring(num, i - num));
					num = i + 1;
					writer.Write('\\');
					writer.Write(text[i]);
				}
			}
			writer.Write(text.Substring(num, length - num));
			writer.Write('/');
			writer.Write(text2);
		}
	}
}
