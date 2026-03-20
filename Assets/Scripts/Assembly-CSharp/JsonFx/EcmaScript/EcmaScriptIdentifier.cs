using System;
using System.IO;
using JsonFx.Model;
using JsonFx.Serialization;

namespace JsonFx.EcmaScript
{
	public class EcmaScriptIdentifier : ITextFormattable<ModelTokenType>
	{
		private readonly string identifier;

		public string Identifier
		{
			get
			{
				return identifier;
			}
		}

		public EcmaScriptIdentifier()
			: this(null)
		{
		}

		public EcmaScriptIdentifier(string ident)
		{
			identifier = ((!string.IsNullOrEmpty(ident)) ? VerifyIdentifier(ident, true) : string.Empty);
		}

		void ITextFormattable<ModelTokenType>.Format(ITextFormatter<ModelTokenType> formatter, TextWriter writer)
		{
			if (string.IsNullOrEmpty(identifier))
			{
				writer.Write("null");
			}
			else
			{
				writer.Write(identifier);
			}
		}

		public static string VerifyIdentifier(string ident, bool nested)
		{
			return VerifyIdentifier(ident, nested, true);
		}

		public static string VerifyIdentifier(string ident, bool nested, bool throwOnEmpty)
		{
			if (string.IsNullOrEmpty(ident))
			{
				if (throwOnEmpty)
				{
					throw new ArgumentException("Identifier is empty.");
				}
				return string.Empty;
			}
			ident = ident.Replace(" ", string.Empty);
			if (!IsValidIdentifier(ident, nested))
			{
				throw new ArgumentException("Identifier \"" + ident + "\" is not supported.");
			}
			return ident;
		}

		public static bool IsValidIdentifier(string ident, bool nested)
		{
			if (string.IsNullOrEmpty(ident))
			{
				return false;
			}
			if (nested)
			{
				string[] array = ident.Split('.');
				string[] array2 = array;
				foreach (string ident2 in array2)
				{
					if (!IsValidIdentifier(ident2, false))
					{
						return false;
					}
				}
				return true;
			}
			if (IsReservedWord(ident))
			{
				return false;
			}
			bool flag = false;
			int j = 0;
			for (int length = ident.Length; j < length; j++)
			{
				char c = ident[j];
				if (!flag || c < '0' || c > '9')
				{
					if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && c != '_' && c != '$')
					{
						return false;
					}
					flag = true;
				}
			}
			return true;
		}

		private static bool IsReservedWord(string ident)
		{
			switch (ident)
			{
			case "null":
			case "false":
			case "true":
			case "break":
			case "case":
			case "catch":
			case "continue":
			case "debugger":
			case "default":
			case "delete":
			case "do":
			case "else":
			case "finally":
			case "for":
			case "function":
			case "if":
			case "in":
			case "instanceof":
			case "new":
			case "return":
			case "switch":
			case "this":
			case "throw":
			case "try":
			case "typeof":
			case "var":
			case "void":
			case "while":
			case "with":
			case "abstract":
			case "boolean":
			case "byte":
			case "char":
			case "class":
			case "const":
			case "double":
			case "enum":
			case "export":
			case "extends":
			case "final":
			case "float":
			case "goto":
			case "implements":
			case "import":
			case "int":
			case "interface":
			case "long":
			case "native":
			case "package":
			case "private":
			case "protected":
			case "public":
			case "short":
			case "static":
			case "super":
			case "synchronized":
			case "throws":
			case "transient":
			case "volatile":
			case "let":
			case "yield":
				return true;
			default:
				return false;
			}
		}

		public static EcmaScriptIdentifier Parse(string value)
		{
			return new EcmaScriptIdentifier(value);
		}

		public override string ToString()
		{
			return identifier;
		}

		public override bool Equals(object obj)
		{
			EcmaScriptIdentifier ecmaScriptIdentifier = obj as EcmaScriptIdentifier;
			if (ecmaScriptIdentifier == null)
			{
				return base.Equals(obj);
			}
			if (string.IsNullOrEmpty(identifier) && string.IsNullOrEmpty(ecmaScriptIdentifier.identifier))
			{
				return true;
			}
			return StringComparer.Ordinal.Equals(identifier, ecmaScriptIdentifier.identifier);
		}

		public override int GetHashCode()
		{
			return StringComparer.Ordinal.GetHashCode(identifier);
		}

		public static implicit operator string(EcmaScriptIdentifier ident)
		{
			if (ident == null)
			{
				return string.Empty;
			}
			return ident.identifier;
		}

		public static implicit operator EcmaScriptIdentifier(string ident)
		{
			return new EcmaScriptIdentifier(ident);
		}
	}
}
